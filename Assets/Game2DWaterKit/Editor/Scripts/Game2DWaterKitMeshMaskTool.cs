namespace Game2DWaterKit
{
    using Game2DWaterKit.Main;
    using Game2DWaterKit.Material;
    using Game2DWaterKit.Rendering.Mask;
    using Game2DWaterKit.Utils;
    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;

    public abstract partial class Game2DWaterKitInspector : Editor
    {

        protected static class Game2DWaterKitMeshMaskTool
        {
            #region Variables

            private static readonly float _clipperLibFloatToIntPrecision = 100000f;

            private static readonly int _segmentHandleHash = "segmentHandle".GetHashCode();
            private static readonly int _controlPointHandleHash = "controlPointHandle".GetHashCode();
            private static readonly int _controlPointHandlePointHandleHash = "controlPointHandlePointHandleHash".GetHashCode();

            private static readonly string[] _controlPointTypePopupDisplayOptions = { "Cusp (Shift + C)", "Smooth (Shift + S)", "Symetric (Shift + Y)" };

            private static Texture2D _controlPointHandleTexture;
            private static Texture2D _controlPointCuspTexture;
            private static Texture2D _controlPointSymetricTexture;
            private static Texture2D _controlPointSmoothTexture;
            private static Texture2D _controlPointNewTexture;

            private static UnityEngine.Material _handlesMaterial;

            private static float _handlesScale = 0.11f;
            private static float _anchorPointRadius;
            private static float _handlePointSize;

            private static MeshMask _meshMask;
            private static MainModule _mainModule;
            private static MaterialModule _materialModule;
            private static Transform _targetObjectTransform;
            private static SerializedObject _serializedObject;

            private static List<MeshMask.ControlPoint> _controlPointsWorldSpace;
            private static int _selectedControlPointIndex = -1;
            private static int _selectedSegmentIndex = -1;
            private static Vector3[] _segmentPoints;

            private static bool _insertControlPoints;
            private static bool _removeControlPoints;

            private static bool _editEdgeCollider;
            private static int _edgeColliderStartPoint = -1;
            private static int _edgeColliderEndPoint = -1;
            private static int _edgeColliderCandidateEndPoint = -1;
            private static float _edgeColliderMatchMaskOutline = 1f;

            private static bool _editPolygonCollider;
            private static int _editPolygonColliderStep = 1;
            private static float _polygonColliderMatchMaskOutline = 1f;

            private static Matrix4x4 _maskToWorldSpace;
            private static Matrix4x4 _worldToMaskSpace;

            private static System.Action _repaintInspectorCallback;
            private static bool _shouldRepaintInspector;

            private static List<Vector2[]> _polygonColliderPreviewShape;
            private static int _polygonColliderPreviewShapeUsedSubdivisionCount;

            private static Vector2[] _edgeColliderPreviewShape;
            private static int _edgeColliderPreviewShapeUsedSubdivisionCount;
            private static int _edgeColliderPreviewShapeUsedEndPointIndex;

            private static bool _updateEdgeColliderPreviewShape;
            private static bool _updatePolygonColliderPreviewShape;

            public static int snapMode = 0; // 0 -> Grid ; 1 -> Closest Point
            public static Vector2 snapToGridStepSize = Vector2.one * 0.5f;

            #endregion

            internal static void Initialize(Object targetObject, System.Action repaintInspector)
            {
                _targetObjectTransform = (targetObject as Game2DWaterKitObject).transform;

                if (_isEditingWaterObject)
                {
                    var waterObject = (targetObject as Game2DWater);
                    _mainModule = waterObject.MainModule;
                    _materialModule = waterObject.MaterialModule;
                    _meshMask = waterObject.RenderingModule.MeshMask;
                }
                else
                {
                    var waterfallObject = (targetObject as Game2DWaterfall);
                    _mainModule = waterfallObject.MainModule;
                    _materialModule = waterfallObject.MaterialModule;
                    _meshMask = waterfallObject.RenderingModule.MeshMask;
                }

                _serializedObject = new SerializedObject(targetObject);
                _controlPointsWorldSpace = new List<MeshMask.ControlPoint>();

                _repaintInspectorCallback = repaintInspector;

                if (_handlesMaterial == null)
                    LoadResources();
            }

            internal static void DrawInspectorProperties()
            {
                _serializedObject.Update();

                EditorGUI.BeginChangeCheck();

                if (Event.current.type == EventType.Layout)
                    UpdateCachedVariables();

                BeginBoxGroup(false);

                DrawProperty(_serializedObject.FindProperty("_meshMaskSubdivisions"), Game2DWaterKitStyles.MeshMaskSubdivisionsPropertyLabel);

                int colinearVerticesRemovedNumber = ((_meshMask.Subdivisions + 1) * _meshMask.ControlPoints.Count) - _meshMask.Vertices.Length;

                if (colinearVerticesRemovedNumber > 0)
                    EditorGUILayout.HelpBox(string.Format("The mask mesh has {0} vertices. {1} colinear vertices were removed.", _meshMask.Vertices.Length, colinearVerticesRemovedNumber), MessageType.None);
                else
                    EditorGUILayout.HelpBox(string.Format("The mask mesh has {0} vertices.", _meshMask.Vertices.Length), MessageType.None);

                EditorGUI.BeginDisabledGroup(_editPolygonCollider);
                DrawEdgeColliderProperties();
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(_editEdgeCollider);
                DrawPolygonColliderProperties();
                EditorGUI.EndDisabledGroup();

                EndBoxGroup();

                if (!_editEdgeCollider && !_editPolygonCollider)
                {
                    BeginBoxGroup(false);

                    snapMode = EditorGUILayout.Popup(snapMode, new[] { "Snap To Grid (Shift)", "Snap To Control Points (Shift)" });
                    if (snapMode == 0)
                    {
                        EditorGUI.BeginChangeCheck();
                        snapToGridStepSize = EditorGUILayout.Vector2Field("Step Size", snapToGridStepSize);
                        if (EditorGUI.EndChangeCheck())
                            snapToGridStepSize = new Vector2(Mathf.Clamp01(snapToGridStepSize.x), Mathf.Clamp01(snapToGridStepSize.y));
                    }

                    EditorGUILayout.Space();

                    EditorGUI.BeginChangeCheck();
                    DrawInsertRemoveControlPointsButtons();
                    if (EditorGUI.EndChangeCheck())
                        _selectedControlPointIndex = _selectedSegmentIndex = -1;

                    if (_selectedSegmentIndex > -1 && GUILayout.Button("Make the selected segment a straight-line (Shift + S)"))
                        MakeSelectedSegmentStraightLine();

                    if (_selectedControlPointIndex > -1)
                    {
                        var controlPointProperty = _serializedObject.FindProperty("_meshMaskControlPoints").GetArrayElementAtIndex(_selectedControlPointIndex);
                        var controlPointTypeProperty = controlPointProperty.FindPropertyRelative("type");

                        BeginBoxGroup();

                        var controlPointAnchorPointPositionProperty = controlPointProperty.FindPropertyRelative("anchorPointPosition");
                        var rect = EditorGUI.PrefixLabel(EditorGUILayout.GetControlRect(), Game2DWaterKitStyles.GetTempLabel("Control Point Position"));
                        EditorGUI.BeginChangeCheck();
                        var position = EditorGUI.Vector2Field(rect, GUIContent.none, _maskToWorldSpace.MultiplyPoint3x4(controlPointAnchorPointPositionProperty.vector2Value));
                        if (EditorGUI.EndChangeCheck())
                        {
                            var currentControlPointWorldSpace = _controlPointsWorldSpace[_selectedControlPointIndex];

                            var delta = position - currentControlPointWorldSpace.anchorPointPosition;

                            currentControlPointWorldSpace.anchorPointPosition = position;
                            currentControlPointWorldSpace.firstHandlePosition += delta;
                            currentControlPointWorldSpace.secondHandlePosition += delta;

                            _controlPointsWorldSpace[_selectedControlPointIndex] = currentControlPointWorldSpace;

                            UpdateControlPoint(_selectedControlPointIndex);
                        }

                        EditorGUI.BeginChangeCheck();
                        controlPointTypeProperty.enumValueIndex = EditorGUILayout.Popup("Control Point Type", controlPointTypeProperty.enumValueIndex, _controlPointTypePopupDisplayOptions);
                        if (EditorGUI.EndChangeCheck())
                            UpdateControlPointType(controlPointProperty, (MeshMask.ControlPoint.ControlPointType)controlPointTypeProperty.enumValueIndex);

                        EndBoxGroup();
                    }

                    EndBoxGroup();
                }

                if (EditorGUI.EndChangeCheck())
                    ApplyModifications();
            }

            internal static void DrawSceneViewHandles()
            {
                if (_editPolygonCollider && _editPolygonColliderStep == 1)
                    Tools.hidden = false;

                var e = Event.current;

                if (e.type == EventType.Layout)
                {
                    UpdateCachedVariables();

                    if (_shouldRepaintInspector)
                    {
                        RepaintInspector();
                        _shouldRepaintInspector = false;
                    }

                    if (e.control || e.alt)
                    {
                        _selectedControlPointIndex = _selectedSegmentIndex = -1;
                        _shouldRepaintInspector = true;
                    }
                }

                for (int currentControlPointIndex = 0, max = _controlPointsWorldSpace.Count; currentControlPointIndex < max; currentControlPointIndex++)
                {
                    EditorGUI.BeginChangeCheck();

                    var currentControlPoint = _controlPointsWorldSpace[currentControlPointIndex];

                    var nextControlPointIndex = currentControlPointIndex + 1 < max ? currentControlPointIndex + 1 : 0;
                    var nextControlPoint = _controlPointsWorldSpace[nextControlPointIndex];

                    ComputeBezierCurve(_segmentPoints, currentControlPoint.anchorPointPosition, nextControlPoint.anchorPointPosition, currentControlPoint.secondHandlePosition, nextControlPoint.firstHandlePosition);
                    DrawSegment(currentControlPointIndex, _segmentPoints);

                    bool hasRemovedControlPoint = DrawControlPoint(currentControlPointIndex);

                    if (EditorGUI.EndChangeCheck())
                    {
                        if (hasRemovedControlPoint)
                            RemoveControlPoint(currentControlPointIndex);
                        else
                            UpdateControlPoint(currentControlPointIndex);

                        ApplyModifications();
                        return;
                    }

                    // Insert New Control Point
                    if (!_editEdgeCollider && !_editPolygonCollider && (Event.current.control || _insertControlPoints))
                    {
                        Vector3 newPointPosition = GetCurvePointAt(currentControlPoint.anchorPointPosition, nextControlPoint.anchorPointPosition, currentControlPoint.secondHandlePosition, nextControlPoint.firstHandlePosition, 0.5f);

                        if (DrawNewControlPoint(newPointPosition))
                        {
                            var newSegment = new Rendering.Mask.MeshMask.ControlPoint();
                            newSegment.anchorPointPosition = newPointPosition;
                            newSegment.firstHandlePosition = (currentControlPoint.anchorPointPosition + 2f * currentControlPoint.secondHandlePosition + nextControlPoint.firstHandlePosition) * 0.25f;
                            newSegment.secondHandlePosition = (nextControlPoint.anchorPointPosition + 2f * nextControlPoint.firstHandlePosition + currentControlPoint.secondHandlePosition) * 0.25f;

                            currentControlPoint.secondHandlePosition = (currentControlPoint.anchorPointPosition + currentControlPoint.secondHandlePosition) * 0.5f;
                            nextControlPoint.firstHandlePosition = (nextControlPoint.anchorPointPosition + nextControlPoint.firstHandlePosition) * 0.5f;

                            _controlPointsWorldSpace[currentControlPointIndex] = currentControlPoint;
                            _controlPointsWorldSpace[nextControlPointIndex] = nextControlPoint;

                            UpdateControlPoint(currentControlPointIndex);
                            UpdateControlPoint(nextControlPointIndex);
                            InsertControlPoint(nextControlPointIndex, newSegment);
                            ApplyModifications();
                            return;
                        }
                    }
                }

                if (!_editEdgeCollider && !_editPolygonCollider && e.type == EventType.KeyDown && _selectedSegmentIndex > -1 && e.shift && e.keyCode == KeyCode.S)
                    MakeSelectedSegmentStraightLine();

                // Draw edge collider preview - from start point to (candidate) end point

                if (_editEdgeCollider && ((_edgeColliderStartPoint > -1 && _edgeColliderCandidateEndPoint > -1) || _edgeColliderEndPoint > -1))
                {
                    UpdateEdgeColliderPreviewShapeIfRequired();

                    using (new Handles.DrawingScope(_maskToWorldSpace))
                    {
                        DrawPath(Game2DWaterKitStyles.MeshMaskToolEdgeColliderOutlinePreviewColor, _edgeColliderPreviewShape, false);
                    }

                    _edgeColliderCandidateEndPoint = -1;
                }

                // Draw polygon collider preview

                if (_editPolygonCollider && _editPolygonColliderStep == 2)
                {
                    UpdatePolygonColliderPreviewShapeIfRequired();

                    using (new Handles.DrawingScope(_maskToWorldSpace))
                    {
                        for (int i = 0, imax = _polygonColliderPreviewShape.Count; i < imax; i++)
                        {
                            DrawPath(Game2DWaterKitStyles.MeshMaskToolPolygonColliderOutlinePreviewColor, _polygonColliderPreviewShape[i], true);
                        }
                    }
                }
            }

            internal static void ResetShape()
            {
                _serializedObject.FindProperty("_meshMaskControlPoints").ClearArray();

                InsertControlPoint(0, new Vector2(-0.5f, 0.5f), new Vector2(-0.5f, 0.25f), new Vector2(-0.25f, 0.5f));
                InsertControlPoint(1, new Vector2(0.5f, 0.5f), new Vector2(0.25f, 0.5f), new Vector2(0.5f, 0.25f));
                InsertControlPoint(2, new Vector2(0.5f, -0.5f), new Vector2(0.5f, -0.25f), new Vector2(0.25f, -0.5f));
                InsertControlPoint(3, new Vector2(-0.5f, -0.5f), new Vector2(-0.25f, -0.5f), new Vector2(-0.5f, -0.25f));

                ApplyModifications();

                var edgeCollider = _targetObjectTransform.GetComponent<EdgeCollider2D>();
                if (edgeCollider != null)
                    edgeCollider.Reset();

                var polygonCollider = _targetObjectTransform.GetComponent<PolygonCollider2D>();
                if (polygonCollider != null)
                {
                    polygonCollider.offset = Vector2.zero;
                    polygonCollider.pathCount = 0;
                }

                _edgeColliderStartPoint = _edgeColliderEndPoint = _edgeColliderCandidateEndPoint = -1;
                _edgeColliderMatchMaskOutline = 1f;
                _selectedControlPointIndex = _selectedSegmentIndex = -1;
                _previewEdgeCollider = false;
            }

            internal static void DrawMeshMaskOutlinePreview()
            {
                if (_isEditingMeshMask)
                    return;

                Matrix4x4 _maskToWorldSpace;

                if (_meshMask.ArePositionAndSizeLocked)
                    _maskToWorldSpace = Matrix4x4.TRS(_meshMask.Position, _targetObjectTransform.rotation, _meshMask.Size);
                else
                    _maskToWorldSpace = Matrix4x4.TRS(_targetObjectTransform.position, _targetObjectTransform.rotation, Vector3.Scale(new Vector3(_mainModule.Width, _mainModule.Height, 1f), _mainModule.Scale));

                var vertices = _meshMask.Vertices;

                using (new Handles.DrawingScope(Game2DWaterKitStyles.MeshMaskToolMaskOutlinePreviewColor, _maskToWorldSpace))
                {
                    Handles.DrawAAPolyLine(5f, vertices);
                    Handles.DrawAAPolyLine(5f, vertices[vertices.Length - 1], vertices[0]);
                }
            }

            private static void DrawEdgeColliderProperties()
            {
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Edge Collider Properties", EditorStyles.miniBoldLabel);

                var edgeCollider = _targetObjectTransform.GetComponent<EdgeCollider2D>();

                EditorGUI.BeginChangeCheck();
                bool hasCollider = EditorGUILayout.Toggle("Use Edge Collider 2D", edgeCollider != null);
                if (EditorGUI.EndChangeCheck())
                {
                    if (hasCollider)
                        edgeCollider = _targetObjectTransform.gameObject.AddComponent<EdgeCollider2D>();
                    else
                        DestroyImmediate(edgeCollider);

                    SwitchEdgeColliderEditingOnOff(hasCollider);
                }

                if (hasCollider)
                {
                    if (!_editEdgeCollider)
                    {
                        var rect = EditorGUILayout.GetControlRect();
                        rect.height += 1f;
                        float xMax = rect.xMax;
                        rect.xMax -= 27f;
                        EditorGUI.HelpBox(rect, string.Format("The edge collider has {0} points.", edgeCollider.pointCount), MessageType.None);
                        rect.xMax = xMax;
                        rect.xMin = xMax - 25f;
                        _previewEdgeCollider = DrawPreviewButton(rect, _previewEdgeCollider);
                    }

                    EditorGUI.BeginChangeCheck();
                    var editEdgeCollider = GUILayout.Toggle(_editEdgeCollider, "Edit Collider", "button");
                    if (EditorGUI.EndChangeCheck())
                        SwitchEdgeColliderEditingOnOff(editEdgeCollider);

                    if (_editEdgeCollider)
                    {
                        BeginBoxGroup();
                        if (_edgeColliderStartPoint == -1)
                            EditorGUILayout.HelpBox("Step 1/3: Please choose the edge collider's start point in the scene view.", MessageType.None);
                        else if (_edgeColliderEndPoint == -1)
                            EditorGUILayout.HelpBox("Step 2/3: Please choose the edge collider's end point in the scene view.", MessageType.None);
                        else
                        {
                            EditorGUILayout.HelpBox("Step 3/3: Please set how much you would like to match the edge collider to the mask outline, and then validate the edge collider.", MessageType.None);

                            _edgeColliderMatchMaskOutline = EditorGUILayout.Slider("Match Mask Outline", _edgeColliderMatchMaskOutline, 0.01f, 1f);

                            UpdateEdgeColliderPreviewShapeIfRequired();
                            EditorGUILayout.HelpBox(string.Format("The edge collider will have {0} points. Any colinear points are removed.", _edgeColliderPreviewShape.Length), MessageType.Info);

                            if (GUILayout.Button("Validate Edge Collider"))
                                ValidateEdgeCollider();
                        }
                        EndBoxGroup();
                    }
                }
            }

            private static void SwitchEdgeColliderEditingOnOff(bool editEdgeCollider)
            {
                _editEdgeCollider = editEdgeCollider;
                _edgeColliderStartPoint = _edgeColliderEndPoint = _edgeColliderCandidateEndPoint = -1;
                _edgeColliderMatchMaskOutline = 1f;
                _selectedControlPointIndex = _selectedSegmentIndex = -1;
                _previewEdgeCollider = _previewPolygonCollider = false;
            }

            private static void DrawPolygonColliderProperties()
            {
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Polygon Collider Properties", EditorStyles.miniBoldLabel);

                var polygonCollider = _targetObjectTransform.GetComponent<PolygonCollider2D>();

                EditorGUI.BeginChangeCheck();
                bool hasCollider = EditorGUILayout.Toggle("Use Polygon Collider 2D", polygonCollider != null);
                if (EditorGUI.EndChangeCheck())
                {
                    if (hasCollider)
                    {
                        DestroyImmediate(_targetObjectTransform.gameObject.GetComponent<BoxCollider2D>());
                        polygonCollider = _targetObjectTransform.gameObject.AddComponent<PolygonCollider2D>();
                        polygonCollider.isTrigger = true;
                        polygonCollider.usedByEffector = true;
                        polygonCollider.pathCount = 0;
                    }
                    else
                    {
                        DestroyImmediate(polygonCollider);
                        var boxCollider = _targetObjectTransform.gameObject.AddComponent<BoxCollider2D>();
                        boxCollider.isTrigger = true;
                        boxCollider.usedByEffector = true;
                    }

                    SwitchPolygonColliderEditingOnOff(hasCollider);
                }
                if (!hasCollider)
                    EditorGUILayout.HelpBox("You can use a Polygon Collider 2D instead of the Box Collider 2D. It will be used by the Buoyancy Effector 2D, and also to detect collisions.", MessageType.Info);

                if (hasCollider)
                {
                    if (!_editPolygonCollider)
                    {
                        var rect = EditorGUILayout.GetControlRect();
                        rect.height += 1f;
                        float xMax = rect.xMax;
                        rect.xMax -= 27f;
                        EditorGUI.HelpBox(rect, string.Format("The polygon collider has {0} {1}, and {2} points in total.", polygonCollider.pathCount, polygonCollider.pathCount > 1 ? "paths" : "path", polygonCollider.GetTotalPointCount()), MessageType.None);
                        rect.xMax = xMax;
                        rect.xMin = xMax - 25f;
                        _previewPolygonCollider = DrawPreviewButton(rect, _previewPolygonCollider);
                    }

                    EditorGUI.BeginChangeCheck();
                    var editPolygonCollider = GUILayout.Toggle(_editPolygonCollider, "Edit Collider", "button");
                    if (EditorGUI.EndChangeCheck())
                        SwitchPolygonColliderEditingOnOff(editPolygonCollider);

                    if (_editPolygonCollider)
                    {
                        bool getInsideMask = _materialModule.Material.GetInt("_SpriteMaskInteraction") == 4;

                        BeginBoxGroup();
                        if (_editPolygonColliderStep == 1)
                        {
                            EditorGUILayout.HelpBox(string.Format("Step 1/2: The algorithm will try to compute the {0} the water and the mask in order to determine the shape of the polygon collider. If you intend to animate the water's size, please make sure that the water is at its maximum size at the moment.", getInsideMask ? "intersection of" : "difference between"), MessageType.None);
                            if (GUILayout.Button("Next Step"))
                            {
                                _editPolygonColliderStep = 2;
                                _updatePolygonColliderPreviewShape = true;
                            }
                        }
                        else
                        {
                            EditorGUILayout.HelpBox(string.Format("Step 2/2: Please set how much you would like to match the polygon collider to the mask shape. The algorithm computes the convex hulls of the computed {0} polygons, so the polygon collider might not match the mask shape exactly even when \"Match Mask Shape\" is set to 1.", getInsideMask ? "intersection" : "difference"), MessageType.None);

                            _polygonColliderMatchMaskOutline = EditorGUILayout.Slider("Match Mask Shape", _polygonColliderMatchMaskOutline, 0.01f, 1f);

                            UpdatePolygonColliderPreviewShapeIfRequired();
                            EditorGUILayout.HelpBox(string.Format("The polygon collider will have {0} {1}, and {2} points in total. Any colinear points are removed.", _polygonColliderPreviewShape.Count, _polygonColliderPreviewShape.Count > 1 ? "paths" : "path", GetTotalElementCount(_polygonColliderPreviewShape)), MessageType.Info);

                            if (GUILayout.Button("Validate Polygon Collider"))
                                ValidatePolygonCollider();
                        }
                        EndBoxGroup();
                    }
                    else if (polygonCollider.shapeCount > polygonCollider.pathCount)
                        EditorGUILayout.HelpBox(string.Format("The polygon collider has {0} {1} and {2} shapes. For the Buoyancy Effector 2D to work properly, a polygon collider should have at most 1 shape per path. Please try editing the polygon collider again with a smaller \"Match Mask Shape\" value.", polygonCollider.pathCount, polygonCollider.pathCount > 1 ? "paths" : "path", polygonCollider.shapeCount), MessageType.Warning);
                }
            }

            private static void SwitchPolygonColliderEditingOnOff(bool editPolygonCollider)
            {
                _editPolygonCollider = editPolygonCollider;
                _polygonColliderMatchMaskOutline = 1f;
                _selectedControlPointIndex = _selectedSegmentIndex = -1;
                _previewPolygonCollider = _previewEdgeCollider = false;
                _editPolygonColliderStep = 1;
                _polygonColliderPreviewShapeUsedSubdivisionCount = -1;
            }

            private static void DrawPath(Color color, Vector2[] path, bool closedPath)
            {
                Handles.color = color;

                for (int j = 0, jmax = closedPath ? path.Length : path.Length - 1, pointCount = path.Length; j < jmax; j++)
                {
                    var currentPoint = path[j];
                    var nextPoint = path[j + 1 < pointCount ? j + 1 : 0];

                    Handles.DrawAAPolyLine(_handlesScale * 33f + 3f, currentPoint, nextPoint);
                }
            }

            private static void UpdateEdgeColliderPreviewShapeIfRequired()
            {
                int endPoint = _edgeColliderEndPoint > -1 ? _edgeColliderEndPoint : _edgeColliderCandidateEndPoint;
                int subdivisions = Mathf.CeilToInt(_meshMask.Subdivisions * _edgeColliderMatchMaskOutline);

                if (endPoint < 0)
                    return;

                if (_updateEdgeColliderPreviewShape || _edgeColliderPreviewShape == null || endPoint != _edgeColliderPreviewShapeUsedEndPointIndex || subdivisions != _edgeColliderPreviewShapeUsedSubdivisionCount)
                {
                    var controlPoints = _meshMask.ControlPoints;
                    int controlPointCount = controlPoints.Count;
                    int pointCount;

                    if (_edgeColliderStartPoint == endPoint)
                        pointCount = controlPointCount;
                    else if (endPoint < _edgeColliderStartPoint)
                        pointCount = endPoint + controlPointCount - _edgeColliderStartPoint;
                    else
                        pointCount = endPoint - _edgeColliderStartPoint;

                    var edgeColliderPoints = new List<Vector2>();
                    var edgeColliderPointCount = 0;

                    Vector2 pointPosition;

                    for (int i = 0, imax = pointCount; i < imax; i++)
                    {
                        int currentControlPointIndex = _edgeColliderStartPoint + i < controlPointCount ? _edgeColliderStartPoint + i : (_edgeColliderStartPoint + i) - controlPointCount;
                        int nextControlPointIndex = currentControlPointIndex + 1 < controlPointCount ? currentControlPointIndex + 1 : 0;

                        var currentSegment = controlPoints[currentControlPointIndex];
                        var nextSegment = controlPoints[nextControlPointIndex];

                        for (int j = 0, jmax = subdivisions + 1; j < jmax; j++)
                        {
                            float t = j / (float)jmax;

                            pointPosition = (1f - t) * (1f - t) * (1f - t) * currentSegment.anchorPointPosition + t * t * t * nextSegment.anchorPointPosition + 3f * t * (1f - t) * (1f - t) * currentSegment.secondHandlePosition + 3f * (1f - t) * t * t * nextSegment.firstHandlePosition;

                            if (edgeColliderPointCount > 1 && WaterUtility.AreColinear(edgeColliderPoints[edgeColliderPointCount - 2], edgeColliderPoints[edgeColliderPointCount - 1], pointPosition))
                            {
                                edgeColliderPoints[edgeColliderPointCount - 1] = pointPosition;
                            }
                            else
                            {
                                edgeColliderPoints.Add(pointPosition);
                                edgeColliderPointCount++;
                            }
                        }
                    }

                    pointPosition = controlPoints[endPoint].anchorPointPosition;

                    if (edgeColliderPointCount > 1 && WaterUtility.AreColinear(edgeColliderPoints[edgeColliderPointCount - 2], edgeColliderPoints[edgeColliderPointCount - 1], pointPosition))
                        edgeColliderPoints[edgeColliderPointCount - 1] = pointPosition;
                    else
                        edgeColliderPoints.Add(pointPosition);

                    _edgeColliderPreviewShape = edgeColliderPoints.ToArray();
                    _edgeColliderPreviewShapeUsedEndPointIndex = endPoint;
                    _edgeColliderPreviewShapeUsedSubdivisionCount = subdivisions;

                    _updateEdgeColliderPreviewShape = false;
                }
            }

            private static void UpdatePolygonColliderPreviewShapeIfRequired()
            {
                var subdivision = (int)(_polygonColliderMatchMaskOutline * _meshMask.Subdivisions);

                if (_updatePolygonColliderPreviewShape || _polygonColliderPreviewShape == null || _polygonColliderPreviewShapeUsedSubdivisionCount != subdivision)
                {
                    bool getInsideMask = _materialModule.Material.GetInt("_SpriteMaskInteraction") == 4;
                    _polygonColliderPreviewShape = GetIntersectionOrDifferenceBetweenMaskAndWaterOrWaterfallObjectInMaskSpace(getInsideMask);
                    _polygonColliderPreviewShapeUsedSubdivisionCount = subdivision;

                    _updatePolygonColliderPreviewShape = false;
                }
            }

            private static void LoadResources()
            {
                _handlesMaterial = new UnityEngine.Material(Shader.Find("Hidden/TintedTexturedHandle"));

                _controlPointCuspTexture = Resources.Load("images/h_meshMaskControlPointCusp") as Texture2D;
                _controlPointSmoothTexture = Resources.Load("images/h_meshMaskControlPointSmooth") as Texture2D;
                _controlPointSymetricTexture = Resources.Load("images/h_meshMaskControlPointSymetric") as Texture2D;
                _controlPointNewTexture = Resources.Load("images/h_meshMaskControlPointNew") as Texture2D;
                _controlPointHandleTexture = Resources.Load("images/h_meshMaskControlPointHandle") as Texture2D;
            }

            private static void UpdateCachedVariables()
            {
                var segmentPointCount = 2 + _meshMask.Subdivisions;

                if (_segmentPoints == null || _segmentPoints.Length != segmentPointCount)
                    _segmentPoints = new Vector3[segmentPointCount];

                var handlesBaseSize = HandleUtility.GetHandleSize(_targetObjectTransform.position) * _handlesScale;

                _anchorPointRadius = handlesBaseSize;
                _handlePointSize = handlesBaseSize * 0.8f;

                if (_meshMask.ArePositionAndSizeLocked)
                    _maskToWorldSpace = Matrix4x4.TRS(_meshMask.Position, _targetObjectTransform.rotation, _meshMask.Size);
                else
                    _maskToWorldSpace = Matrix4x4.TRS(_targetObjectTransform.position, _targetObjectTransform.rotation, Vector3.Scale(new Vector3(_mainModule.Width, _mainModule.Height, 1f), _mainModule.Scale));

                _worldToMaskSpace = _maskToWorldSpace.inverse;

                _controlPointsWorldSpace.Clear();

                var controlPoints = _meshMask.ControlPoints;

                for (int i = 0, imax = controlPoints.Count; i < imax; i++)
                {
                    var segmentProperty = controlPoints[i];

                    var segment = new MeshMask.ControlPoint()
                    {
                        anchorPointPosition = _maskToWorldSpace.MultiplyPoint3x4(segmentProperty.anchorPointPosition),
                        firstHandlePosition = _maskToWorldSpace.MultiplyPoint3x4(segmentProperty.firstHandlePosition),
                        secondHandlePosition = _maskToWorldSpace.MultiplyPoint3x4(segmentProperty.secondHandlePosition),
                        type = segmentProperty.type
                    };

                    _controlPointsWorldSpace.Add(segment);
                }
            }

            private static void DrawInsertRemoveControlPointsButtons()
            {
                var rect = EditorGUILayout.GetControlRect();

                rect.width -= 3f;

                rect.width *= 0.5f;

                EditorGUI.BeginChangeCheck();
                bool insert = GUI.Toggle(rect, _insertControlPoints || Event.current.control, "Insert (Ctrl)", Game2DWaterKitStyles.ButtonStyle);
                if (EditorGUI.EndChangeCheck())
                {
                    _insertControlPoints = insert;
                    _removeControlPoints = false;
                }

                rect.x += rect.width + 3f;

                EditorGUI.BeginChangeCheck();
                bool remove = GUI.Toggle(rect, _removeControlPoints || Event.current.alt, "Remove (Alt)", Game2DWaterKitStyles.ButtonStyle);
                if (EditorGUI.EndChangeCheck())
                {
                    _removeControlPoints = remove;
                    _insertControlPoints = false;
                }
            }

            private static void DrawSegment(int index, Vector3[] points)
            {
                int controlID = GUIUtility.GetControlID(_segmentHandleHash, FocusType.Keyboard);

                var e = Event.current;

                switch (e.type)
                {
                    case EventType.Layout:
                        if (!(e.control || e.alt))
                            HandleUtility.AddControl(controlID, HandleUtility.DistanceToPolyLine(points));
                        break;
                    case EventType.Repaint:
                        bool isHovered = HandleUtility.nearestControl == controlID;
                        isHovered &= !(e.control || e.alt) && !_editEdgeCollider && !_editPolygonCollider;
                        Handles.color = _selectedSegmentIndex == index ? Game2DWaterKitStyles.MeshMaskToolSegmentSelectedColor : (isHovered ? Game2DWaterKitStyles.MeshMaskToolSegmentHoveredColor : Game2DWaterKitStyles.MeshMaskToolSegmentColor);
                        Handles.DrawAAPolyLine(isHovered ? _handlesScale * 33f + 3f : _handlesScale * 33f, points);
                        break;
                    case EventType.MouseDown:
                        if (HandleUtility.nearestControl == controlID && e.button == 0 && !_editEdgeCollider && !_editPolygonCollider)
                        {
                            _selectedSegmentIndex = (_selectedSegmentIndex == index) ? -1 : index;
                            _selectedControlPointIndex = -1;
                            _insertControlPoints = false;
                            _removeControlPoints = false;
                            _shouldRepaintInspector = true;
                            e.Use();
                        }
                        break;
                }
            }

            private static bool DrawControlPoint(int index)
            {
                bool hasRemovedPoint = false;

                var controlPoint = _controlPointsWorldSpace[index];

                int controlID = GUIUtility.GetControlID(_controlPointHandleHash, FocusType.Keyboard);

                bool isSelected = _selectedControlPointIndex == index;

                var e = Event.current;

                switch (e.type)
                {
                    case EventType.Layout:
                        HandleUtility.AddControl(controlID, HandleUtility.DistanceToCircle(controlPoint.anchorPointPosition, _anchorPointRadius));
                        break;
                    case EventType.Repaint:
                        var isHovered = HandleUtility.nearestControl == controlID;
                        if (_editEdgeCollider)
                        {
                            bool isCandidatePoint = isHovered && _edgeColliderStartPoint < 0;

                            if (isHovered && _edgeColliderStartPoint > -1 && _edgeColliderEndPoint < 0)
                            {
                                _edgeColliderCandidateEndPoint = index;
                                isCandidatePoint = true;
                            }

                            var controlPointTexture = _controlPointHandleTexture;
                            var color = (_edgeColliderStartPoint == index || _edgeColliderEndPoint == index || isCandidatePoint) ? Game2DWaterKitStyles.MeshMaskToolDefaultEdgeColliderPointSelectedColor : ((_edgeColliderStartPoint > -1 && _edgeColliderEndPoint > -1) ? Game2DWaterKitStyles.MeshMaskToolDefaultControlPointDisabledColor : Game2DWaterKitStyles.MeshMaskToolDefaultEdgeColliderPointColor);

                            DrawDot(controlPoint.anchorPointPosition, _anchorPointRadius, controlPointTexture, color);
                        }
                        else if (_editPolygonCollider)
                        {
                            var controlPointTexture = _controlPointHandleTexture;
                            var color = Game2DWaterKitStyles.MeshMaskToolDefaultControlPointDisabledColor;
                            color.a *= _editPolygonColliderStep == 1 ? 0f : 1f;
                            DrawDot(controlPoint.anchorPointPosition, _anchorPointRadius, controlPointTexture, color);
                        }
                        else
                        {
                            Texture2D controlPointTexture;

                            switch (controlPoint.type)
                            {
                                case MeshMask.ControlPoint.ControlPointType.Smooth:
                                    controlPointTexture = _controlPointSmoothTexture;
                                    break;
                                case MeshMask.ControlPoint.ControlPointType.Symetric:
                                    controlPointTexture = _controlPointSymetricTexture;
                                    break;
                                default:
                                    controlPointTexture = _controlPointCuspTexture;
                                    break;
                            }

                            if (isSelected && e.shift && snapMode > 0)
                            {
                                Vector2 controlPointPosition = _controlPointsWorldSpace[index].anchorPointPosition;

                                float distanceToClosestControlPointX = Mathf.Infinity;
                                float distanceToClosestControlPointY = Mathf.Infinity;
                                int closestControlPointIndexX = -1;
                                int closestControlPointIndexY = -1;
                                for (int i = 0, imax = _controlPointsWorldSpace.Count; i < imax; i++)
                                {
                                    if (i == index)
                                        continue;

                                    float distX = Mathf.Abs(_controlPointsWorldSpace[i].anchorPointPosition.x - controlPointPosition.x);
                                    float distY = Mathf.Abs(_controlPointsWorldSpace[i].anchorPointPosition.y - controlPointPosition.y);
                                    if (distX < distanceToClosestControlPointX)
                                    {
                                        distanceToClosestControlPointX = distX;
                                        closestControlPointIndexX = i;
                                    }
                                    if (distY < distanceToClosestControlPointY)
                                    {
                                        distanceToClosestControlPointY = distY;
                                        closestControlPointIndexY = i;
                                    }
                                }

                                float closestControlPointPositionX = _controlPointsWorldSpace[closestControlPointIndexX].anchorPointPosition.x;
                                float closestControlPointPositionY = _controlPointsWorldSpace[closestControlPointIndexY].anchorPointPosition.y;

                                if (distanceToClosestControlPointX < 0.15f)
                                    DrawSnapLine(new Vector2(closestControlPointPositionX, 0f), true);

                                if (distanceToClosestControlPointY < 0.15f)
                                    DrawSnapLine(new Vector2(0f, closestControlPointPositionY), false);
                            }

                            if (e.alt || _removeControlPoints)
                                DrawDot(controlPoint.anchorPointPosition, _anchorPointRadius, controlPointTexture, _controlPointsWorldSpace.Count > 3 ? (isHovered ? Game2DWaterKitStyles.MeshMaskToolRemoveControlPointHoveredColor : Game2DWaterKitStyles.MeshMaskToolRemoveControlPointColor) : Game2DWaterKitStyles.MeshMaskToolDefaultControlPointDisabledColor);
                            else if (e.control || _insertControlPoints)
                                DrawDot(controlPoint.anchorPointPosition, _anchorPointRadius, controlPointTexture, Game2DWaterKitStyles.MeshMaskToolDefaultControlPointDisabledColor);
                            else
                                DrawDot(controlPoint.anchorPointPosition, _anchorPointRadius, controlPointTexture, isSelected ? Game2DWaterKitStyles.MeshMaskToolDefaultControlPointSelectedColor : (isHovered ? Game2DWaterKitStyles.MeshMaskToolDefaultControlPointHoveredColor : Game2DWaterKitStyles.MeshMaskToolDefaultControlPointColor));
                        }
                        break;
                    case EventType.KeyDown:
                        if (!_editEdgeCollider && !_editPolygonCollider && isSelected && e.shift && (e.keyCode == KeyCode.C || e.keyCode == KeyCode.S || e.keyCode == KeyCode.Y))
                        {
                            switch (e.keyCode)
                            {
                                case KeyCode.C:
                                    controlPoint.type = MeshMask.ControlPoint.ControlPointType.Cusp;
                                    break;
                                case KeyCode.S:
                                    controlPoint.type = MeshMask.ControlPoint.ControlPointType.Smooth;
                                    break;
                                case KeyCode.Y:
                                    controlPoint.type = MeshMask.ControlPoint.ControlPointType.Symetric;
                                    break;
                            }

                            _controlPointsWorldSpace[index] = controlPoint;
                            GUI.changed = true;
                        }
                        break;
                    case EventType.MouseDown:
                        if (HandleUtility.nearestControl == controlID && e.button == 0)
                        {
                            if ((e.alt || _removeControlPoints) && !_editEdgeCollider && !_editPolygonCollider)
                            {
                                if (_controlPointsWorldSpace.Count > 3)
                                {
                                    hasRemovedPoint = true;
                                    GUI.changed = true;
                                }
                            }
                            else
                            {
                                GUIUtility.hotControl = controlID;
                                GUIUtility.keyboardControl = controlID;
                                _selectedControlPointIndex = index;
                                _selectedSegmentIndex = -1;
                                _insertControlPoints = false;
                                _removeControlPoints = false;
                                _shouldRepaintInspector = true;

                                if (_editEdgeCollider)
                                {
                                    if (_edgeColliderEndPoint < 0 && _edgeColliderStartPoint > -1)
                                        _edgeColliderEndPoint = index;

                                    if (_edgeColliderStartPoint < 0)
                                        _edgeColliderStartPoint = index;
                                }
                            }

                            e.Use();
                        }
                        break;
                    case EventType.MouseUp:
                        if (GUIUtility.hotControl == controlID)
                        {
                            GUIUtility.hotControl = 0;
                            GUIUtility.keyboardControl = 0;
                            e.Use();
                        }
                        break;
                    case EventType.MouseDrag:
                        if (!_editEdgeCollider && !_editPolygonCollider && isSelected && GUIUtility.hotControl == controlID)
                        {
                            if (e.delta != Vector2.zero)
                            {
                                Vector2 mousePosition = HandleUtility.GUIPointToWorldRay(e.mousePosition).origin;

                                if (e.shift)
                                {
                                    if (snapMode == 0)
                                    {
                                        float xFrac = Mathf.Abs(mousePosition.x) % 1;
                                        float yFrac = Mathf.Abs(mousePosition.y) % 1;

                                        mousePosition.x = (int)mousePosition.x + Mathf.Sign(mousePosition.x) * (Mathf.Round(xFrac / snapToGridStepSize.x) * snapToGridStepSize.x);
                                        mousePosition.y = (int)mousePosition.y + Mathf.Sign(mousePosition.y) * (Mathf.Round(yFrac / snapToGridStepSize.y) * snapToGridStepSize.y);
                                    }
                                    else
                                    {
                                        float distanceToClosestControlPointX = Mathf.Infinity;
                                        float distanceToClosestControlPointY = Mathf.Infinity;
                                        int closestControlPointIndexX = -1;
                                        int closestControlPointIndexY = -1;
                                        for (int i = 0, imax = _controlPointsWorldSpace.Count; i < imax; i++)
                                        {
                                            if (i == index)
                                                continue;

                                            float distX = Mathf.Abs(_controlPointsWorldSpace[i].anchorPointPosition.x - mousePosition.x);
                                            if (distX < distanceToClosestControlPointX)
                                            {
                                                distanceToClosestControlPointX = distX;
                                                closestControlPointIndexX = i;
                                            }

                                            float distY = Mathf.Abs(_controlPointsWorldSpace[i].anchorPointPosition.y - mousePosition.y);
                                            if (distY < distanceToClosestControlPointY)
                                            {
                                                distanceToClosestControlPointY = distY;
                                                closestControlPointIndexY = i;
                                            }
                                        }

                                        float closestControlPointPositionX = _controlPointsWorldSpace[closestControlPointIndexX].anchorPointPosition.x;
                                        if (distanceToClosestControlPointX < 0.15f)
                                            mousePosition.x = closestControlPointPositionX;

                                        float closestControlPointPositionY = _controlPointsWorldSpace[closestControlPointIndexY].anchorPointPosition.y;
                                        if (distanceToClosestControlPointY < 0.15f)
                                            mousePosition.y = closestControlPointPositionY;
                                    }
                                }

                                Vector2 deltaWorldSpace = mousePosition - controlPoint.anchorPointPosition;

                                controlPoint.anchorPointPosition = mousePosition;

                                controlPoint.firstHandlePosition += deltaWorldSpace;
                                controlPoint.secondHandlePosition += deltaWorldSpace;

                                _controlPointsWorldSpace[index] = controlPoint;
                                GUI.changed = true;
                            }
                            e.Use();
                        }
                        break;
                }

                if (!_editEdgeCollider && !_editPolygonCollider)
                {
                    bool drawFirstHandle = (isSelected || _selectedSegmentIndex == (index > 0 ? index - 1 : _controlPointsWorldSpace.Count - 1));
                    bool drawSecondHandle = (isSelected || _selectedSegmentIndex == index);

                    if (drawFirstHandle)
                        DrawControlPointHandle(index, isFirstHandleCurrent: true);

                    if (drawSecondHandle)
                        DrawControlPointHandle(index, isFirstHandleCurrent: false);
                }

                return hasRemovedPoint;
            }

            private static void DrawSnapLine(Vector2 pointOnLine, bool isSnappingX)
            {
                Handles.color = Color.green;

                if (isSnappingX)
                    Handles.DrawDottedLine(new Vector3(pointOnLine.x, -99999f), new Vector3(pointOnLine.x, 99999f), 3f);
                else
                    Handles.DrawDottedLine(new Vector3(-99999f, pointOnLine.y), new Vector3(99999f, pointOnLine.y), 3f);
            }

            private static void DrawControlPointHandle(int index, bool isFirstHandleCurrent)
            {
                var controlID = GUIUtility.GetControlID(_controlPointHandlePointHandleHash, FocusType.Keyboard);

                var controlPoint = _controlPointsWorldSpace[index];
                Vector2 anchorPointPosition = controlPoint.anchorPointPosition;
                Vector2 handlePointPosition = isFirstHandleCurrent ? controlPoint.firstHandlePosition : controlPoint.secondHandlePosition;
                Vector2 otherHandlePointPosition = isFirstHandleCurrent ? controlPoint.secondHandlePosition : controlPoint.firstHandlePosition;

                var e = Event.current;

                switch (e.type)
                {
                    case EventType.Layout:
                        HandleUtility.AddControl(controlID, HandleUtility.DistanceToCircle(handlePointPosition, _handlePointSize));
                        break;
                    case EventType.Repaint:
                        bool isSelected = GUIUtility.hotControl == controlID;
                        bool isHovered = HandleUtility.nearestControl == controlID;

                        Color color = isSelected ? Game2DWaterKitStyles.MeshMaskToolHandleDefaultSelectedColor : (isHovered ? Game2DWaterKitStyles.MeshMaskToolHandleDefaultHoveredColor : Game2DWaterKitStyles.MeshMaskToolHandleColor);
                        Handles.color = color;
                        Handles.DrawDottedLine(anchorPointPosition, handlePointPosition, 1f);
                        DrawDot(handlePointPosition, _handlePointSize, _controlPointHandleTexture, color);
                        break;
                    case EventType.MouseDown:
                        if (HandleUtility.nearestControl == controlID && e.button == 0)
                        {
                            GUIUtility.hotControl = controlID;
                            GUIUtility.keyboardControl = controlID;
                            e.Use();
                        }
                        break;
                    case EventType.MouseUp:
                        if (GUIUtility.hotControl == controlID && e.button == 0)
                        {
                            GUIUtility.hotControl = 0;
                            GUIUtility.keyboardControl = 0;
                            e.Use();
                        }
                        break;
                    case EventType.MouseDrag:
                        if (GUIUtility.hotControl == controlID)
                        {
                            if (e.delta != Vector2.zero)
                            {
                                handlePointPosition = HandleUtility.GUIPointToWorldRay(e.mousePosition).origin;

                                switch (controlPoint.type)
                                {
                                    case MeshMask.ControlPoint.ControlPointType.Smooth:
                                        var direction = (anchorPointPosition - handlePointPosition).normalized;
                                        var distance = (anchorPointPosition - otherHandlePointPosition).magnitude;
                                        otherHandlePointPosition = anchorPointPosition + direction * distance;
                                        break;
                                    case MeshMask.ControlPoint.ControlPointType.Symetric:
                                        otherHandlePointPosition = 2f * anchorPointPosition - handlePointPosition;
                                        break;
                                    default:
                                        break;
                                }

                                controlPoint.firstHandlePosition = isFirstHandleCurrent ? handlePointPosition : otherHandlePointPosition;
                                controlPoint.secondHandlePosition = isFirstHandleCurrent ? otherHandlePointPosition : handlePointPosition;

                                _controlPointsWorldSpace[index] = controlPoint;

                                GUI.changed = true;
                            }
                            e.Use();
                        }
                        break;
                }
            }

            private static bool DrawNewControlPoint(Vector3 position)
            {
                bool addPoint = false;

                var e = Event.current;

                int controlID = EditorGUIUtility.GetControlID(_controlPointHandleHash, FocusType.Keyboard);

                switch (e.type)
                {
                    case EventType.Layout:
                        HandleUtility.AddControl(controlID, HandleUtility.DistanceToCircle(position, _anchorPointRadius));
                        break;
                    case EventType.Repaint:
                        bool isHovered = HandleUtility.nearestControl == controlID;
                        DrawDot(position, _anchorPointRadius, _controlPointNewTexture, isHovered ? Game2DWaterKitStyles.MeshMaskToolInsertControlPointHoveredColor : Game2DWaterKitStyles.MeshMaskToolInsertControlPointColor);
                        break;
                    case EventType.MouseDown:
                        if (HandleUtility.nearestControl == controlID && e.button == 0)
                        {
                            addPoint = true;
                            e.Use();
                        }
                        break;
                }

                return addPoint;
            }

            private static void UpdateControlPoint(int index)
            {
                var controlPointWorldSpace = _controlPointsWorldSpace[index];

                var segmentProperty = _serializedObject.FindProperty("_meshMaskControlPoints").GetArrayElementAtIndex(index);
                segmentProperty.FindPropertyRelative("anchorPointPosition").vector2Value = _worldToMaskSpace.MultiplyPoint3x4(controlPointWorldSpace.anchorPointPosition);
                segmentProperty.FindPropertyRelative("firstHandlePosition").vector2Value = _worldToMaskSpace.MultiplyPoint3x4(controlPointWorldSpace.firstHandlePosition);
                segmentProperty.FindPropertyRelative("secondHandlePosition").vector2Value = _worldToMaskSpace.MultiplyPoint3x4(controlPointWorldSpace.secondHandlePosition);

                var controlPointType = (MeshMask.ControlPoint.ControlPointType)segmentProperty.FindPropertyRelative("type").enumValueIndex;

                if (controlPointWorldSpace.type != controlPointType)
                    UpdateControlPointType(segmentProperty, controlPointWorldSpace.type);
            }

            private static void UpdateControlPointType(SerializedProperty controlPointProperty, Rendering.Mask.MeshMask.ControlPoint.ControlPointType newType)
            {
                var controlPointAnchorPointPositionProperty = controlPointProperty.FindPropertyRelative("anchorPointPosition");
                var controlPointFirstHandlePointPositionProperty = controlPointProperty.FindPropertyRelative("firstHandlePosition");
                var controlPointSecondHandlePointPositionProperty = controlPointProperty.FindPropertyRelative("secondHandlePosition");
                var controlPointType = controlPointProperty.FindPropertyRelative("type");

                var anchorPointPosition = _maskToWorldSpace.MultiplyPoint3x4(controlPointAnchorPointPositionProperty.vector2Value);
                var firstHandlePosition = _maskToWorldSpace.MultiplyPoint3x4(controlPointFirstHandlePointPositionProperty.vector2Value);
                var secondHandlePosition = _maskToWorldSpace.MultiplyPoint3x4(controlPointSecondHandlePointPositionProperty.vector2Value);

                switch (newType)
                {
                    case MeshMask.ControlPoint.ControlPointType.Smooth:
                        var direction = (anchorPointPosition - firstHandlePosition).normalized;
                        var distance = (anchorPointPosition - secondHandlePosition).magnitude;
                        secondHandlePosition = anchorPointPosition + direction * distance;
                        break;
                    case MeshMask.ControlPoint.ControlPointType.Symetric:
                        secondHandlePosition = 2f * anchorPointPosition - firstHandlePosition;
                        break;
                    default:
                        break;
                }

                controlPointSecondHandlePointPositionProperty.vector2Value = _worldToMaskSpace.MultiplyPoint3x4(secondHandlePosition);
                controlPointType.enumValueIndex = (int)newType;
            }

            private static void InsertControlPoint(int index, Rendering.Mask.MeshMask.ControlPoint newControlPointWorldSpace)
            {
                var anchorPointPositionMaskSpace = _worldToMaskSpace.MultiplyPoint3x4(newControlPointWorldSpace.anchorPointPosition);
                var firstHandlePositionMaskSpace = _worldToMaskSpace.MultiplyPoint3x4(newControlPointWorldSpace.firstHandlePosition);
                var secondHandlePositionMaskSpace = _worldToMaskSpace.MultiplyPoint3x4(newControlPointWorldSpace.secondHandlePosition);

                InsertControlPoint(index, anchorPointPositionMaskSpace, firstHandlePositionMaskSpace, secondHandlePositionMaskSpace);
            }

            private static void InsertControlPoint(int index, Vector3 anchorPointPositionMaskSpace, Vector3 firstHandlePositionMaskSpace, Vector3 secondHandlePositionMaskSpace)
            {
                if (index == 0)
                    index = _serializedObject.FindProperty("_meshMaskControlPoints").arraySize;

                _serializedObject.FindProperty("_meshMaskControlPoints").InsertArrayElementAtIndex(index);

                var newSegmentProperty = _serializedObject.FindProperty("_meshMaskControlPoints").GetArrayElementAtIndex(index);

                newSegmentProperty.FindPropertyRelative("anchorPointPosition").vector2Value = anchorPointPositionMaskSpace;
                newSegmentProperty.FindPropertyRelative("firstHandlePosition").vector2Value = firstHandlePositionMaskSpace;
                newSegmentProperty.FindPropertyRelative("secondHandlePosition").vector2Value = secondHandlePositionMaskSpace;
                newSegmentProperty.FindPropertyRelative("type").enumValueIndex = 0;
            }

            private static void RemoveControlPoint(int index)
            {
                _serializedObject.FindProperty("_meshMaskControlPoints").DeleteArrayElementAtIndex(index);
            }

            private static void DrawDot(Vector3 position, float size, Texture2D texture, Color color)
            {
                if (Event.current.type != EventType.Repaint)
                    return;

                Vector3 right = (Camera.current == null ? Vector3.right : Camera.current.transform.right) * size;
                Vector3 up = (Camera.current == null ? Vector3.up : Camera.current.transform.up) * size;

                _handlesMaterial.SetColor("_TintColor", color);
                _handlesMaterial.SetTexture("_MainTex", texture);
                _handlesMaterial.SetPass(0);

                GL.Begin(GL.QUADS);
                GL.TexCoord2(1f, 1f);
                GL.Vertex(position + right + up);
                GL.TexCoord2(1f, 0f);
                GL.Vertex(position + right - up);
                GL.TexCoord2(0f, 0f);
                GL.Vertex(position - right - up);
                GL.TexCoord2(0f, 1f);
                GL.Vertex(position - right + up);
                GL.End();
            }

            private static void MakeSelectedSegmentStraightLine()
            {
                var startControlPointIndex = _selectedSegmentIndex;
                var endControlPointIndex = _selectedSegmentIndex + 1 < _controlPointsWorldSpace.Count ? _selectedSegmentIndex + 1 : 0;

                var startControlPoint = _controlPointsWorldSpace[startControlPointIndex];
                var endControlPoint = _controlPointsWorldSpace[endControlPointIndex];

                startControlPoint.type = endControlPoint.type = Rendering.Mask.MeshMask.ControlPoint.ControlPointType.Cusp;

                var dir = endControlPoint.anchorPointPosition - startControlPoint.anchorPointPosition;

                startControlPoint.secondHandlePosition = startControlPoint.anchorPointPosition + dir * 0.25f;
                endControlPoint.firstHandlePosition = endControlPoint.anchorPointPosition - dir * 0.25f;

                _controlPointsWorldSpace[startControlPointIndex] = startControlPoint;
                _controlPointsWorldSpace[endControlPointIndex] = endControlPoint;

                UpdateControlPoint(startControlPointIndex);
                UpdateControlPoint(endControlPointIndex);
                ApplyModifications();
            }

            private static void ValidateEdgeCollider()
            {
                UpdateEdgeColliderPreviewShapeIfRequired();

                var edgeCollider = _targetObjectTransform.GetComponent<EdgeCollider2D>();

                var scale = _meshMask.ArePositionAndSizeLocked ? _meshMask.Size : new Vector3(_mainModule.Width, _mainModule.Height, 1f);

                for (int i = 0, imax = _edgeColliderPreviewShape.Length; i < imax; i++)
                {
                    _edgeColliderPreviewShape[i] = Vector2.Scale(_edgeColliderPreviewShape[i], scale);
                }

                _edgeColliderStartPoint = _edgeColliderEndPoint = _edgeColliderCandidateEndPoint = -1;
                _editEdgeCollider = false;
                _edgeColliderMatchMaskOutline = 1f;
                _selectedControlPointIndex = _selectedSegmentIndex = -1;

                edgeCollider.points = _edgeColliderPreviewShape;
            }

            internal static void UpdateEdgeCollider(bool isSizeLocked, Vector2 waterSize, Vector2 lockedMaskSize)
            {
                var edgeCollider = _targetObjectTransform.GetComponent<EdgeCollider2D>();

                if (edgeCollider == null)
                    return;

                var edgeColliderPoints = edgeCollider.points;

                var scale = isSizeLocked ? DevideComponentWise(lockedMaskSize, waterSize) : DevideComponentWise(waterSize, lockedMaskSize);

                for (int i = 0, imax = edgeColliderPoints.Length; i < imax; i++)
                {
                    var point = edgeColliderPoints[i];
                    point.x *= scale.x;
                    point.y *= scale.y;
                    edgeColliderPoints[i] = point;
                }

                edgeCollider.points = edgeColliderPoints;
                edgeCollider.offset = Vector2.zero;
            }

            private static void ValidatePolygonCollider()
            {
                UpdatePolygonColliderPreviewShapeIfRequired();

                var polygonCollider = _targetObjectTransform.GetComponent<PolygonCollider2D>();

                var scale = _meshMask.ArePositionAndSizeLocked ? _meshMask.Size : new Vector3(_mainModule.Width, _mainModule.Height, 1f);

                polygonCollider.pathCount = _polygonColliderPreviewShape.Count;

                for (int i = 0, imax = _polygonColliderPreviewShape.Count; i < imax; i++)
                {
                    var path = _polygonColliderPreviewShape[i];

                    for (int j = 0, jmax = path.Length; j < jmax; j++)
                    {
                        path[j] = Vector2.Scale(path[j], scale);
                    }

                    polygonCollider.SetPath(i, path);
                }

                polygonCollider.offset = Vector2.zero;

                _editPolygonCollider = false;
                _polygonColliderMatchMaskOutline = 1f;
                _selectedControlPointIndex = _selectedSegmentIndex = -1;
            }

            internal static void UpdatePolygonCollider(bool isSizeLocked, Vector2 waterSize, Vector2 lockedMaskSize)
            {
                var polygonCollider = _targetObjectTransform.GetComponent<PolygonCollider2D>();

                if (polygonCollider == null)
                    return;

                var scale = isSizeLocked ? DevideComponentWise(lockedMaskSize, waterSize) : DevideComponentWise(waterSize, lockedMaskSize);

                for (int i = 0, imax = polygonCollider.pathCount; i < imax; i++)
                {
                    var path = polygonCollider.GetPath(i);

                    for (int j = 0, jmax = path.Length; j < jmax; j++)
                    {
                        var point = path[j];
                        point.x *= scale.x;
                        point.y *= scale.y;
                        path[j] = point;
                    }

                    polygonCollider.SetPath(i, path);
                }

                polygonCollider.offset = Vector2.zero;
            }

            private static void ComputeBezierCurve(Vector3[] points, Vector3 startAnchor, Vector3 endAnchor, Vector3 firstHandle, Vector3 endHandle)
            {
                for (int i = 0, imax = points.Length; i < imax; i++)
                {
                    float t = i / (float)(imax - 1);
                    points[i] = GetCurvePointAt(startAnchor, endAnchor, firstHandle, endHandle, t);
                }
            }

            private static Vector3 GetCurvePointAt(Vector3 startAnchor, Vector3 endAnchor, Vector3 startHandle, Vector3 endHandle, float t)
            {
                return (1f - t) * (1f - t) * (1f - t) * startAnchor + t * t * t * endAnchor + 3f * t * (1f - t) * (1f - t) * startHandle + 3f * (1f - t) * t * t * endHandle;
            }

            private static List<Vector2[]> GetIntersectionOrDifferenceBetweenMaskAndWaterOrWaterfallObjectInMaskSpace(bool getInsideMaskPolygons)
            {
                var maskPoints = GetMeshMaskPointsLocalSpace();
                var waterOrWaterfallPoints = GetWaterOrWaterfallPointsLocalSpace();
                var solutionPolygons = new List<List<ClipperLib.IntPoint>>();

                ClipperLib.Clipper clipper = new ClipperLib.Clipper();

                clipper.AddPath(maskPoints, ClipperLib.PolyType.ptClip, true);
                clipper.AddPath(waterOrWaterfallPoints, ClipperLib.PolyType.ptSubject, true);
                clipper.Execute(getInsideMaskPolygons ? ClipperLib.ClipType.ctIntersection : ClipperLib.ClipType.ctDifference, solutionPolygons, ClipperLib.PolyFillType.pftEvenOdd, ClipperLib.PolyFillType.pftEvenOdd);

                return GetSolutionPolygonsMaskSpace(solutionPolygons);
            }

            private static List<Vector2[]> GetSolutionPolygonsMaskSpace(List<List<ClipperLib.IntPoint>> solutionPolygons)
            {
                var localSpaceToMaskSpace = _worldToMaskSpace * _mainModule.LocalToWorldMatrix;

                var resultPolygons = new List<Vector2[]>();

                for (int i = 0, imax = solutionPolygons.Count; i < imax; i++)
                {
                    var solutionPolygon = solutionPolygons[i];

                    var resultPolygon = new Vector2[solutionPolygon.Count];

                    for (int j = 0, jmax = solutionPolygon.Count; j < jmax; j++)
                    {
                        var solutionPoint = solutionPolygon[j];
                        var resultPoint = localSpaceToMaskSpace.MultiplyPoint3x4(new Vector2(solutionPoint.X / _clipperLibFloatToIntPrecision, solutionPoint.Y / _clipperLibFloatToIntPrecision));

                        resultPolygon[j] = resultPoint;
                    }

                    resultPolygons.Add(GetConvexHull(resultPolygon));
                }

                return resultPolygons;
            }

            // simple naive algorithm to compute convex hull of polygon
            private static Vector2[] GetConvexHull(Vector2[] polygonPoints)
            {
                List<Vector2> hull = new List<Vector2>(polygonPoints);

                bool isConvexHull;

                do
                {
                    isConvexHull = true;

                    for (int i = 0, imax = hull.Count; i < imax; i++)
                    {
                        var currentPoint = hull[i];
                        var previousPoint = hull[i > 0 ? i - 1 : imax - 1];
                        var nextPoint = hull[i < imax - 1 ? i + 1 : 0];

                        var isCurrentPointReflex = !((nextPoint.x - previousPoint.x) * (currentPoint.y - previousPoint.y) - (nextPoint.y - previousPoint.y) * (currentPoint.x - previousPoint.x) < Mathf.Epsilon);

                        if (isCurrentPointReflex)
                        {
                            hull.RemoveAt(i);
                            isConvexHull = false;
                            break;
                        }
                    }

                } while (!isConvexHull);

                return hull.ToArray();
            }

            private static List<ClipperLib.IntPoint> GetMeshMaskPointsLocalSpace()
            {
                var points = new List<Vector3>();
                var subdivision = (int)(_polygonColliderMatchMaskOutline * _meshMask.Subdivisions);
                var controlPoints = _meshMask.ControlPoints;

                var pointCount = 0;

                for (int i = 0, imax = controlPoints.Count; i < imax; i++)
                {
                    var currentControlPoint = controlPoints[i];
                    var nextControlPoint = controlPoints[i + 1 < imax ? i + 1 : 0];

                    for (int j = 0, jmax = 1 + subdivision; j < jmax; j++)
                    {
                        var t = j / (float)(jmax);
                        var position = GetCurvePointAt(currentControlPoint.anchorPointPosition, nextControlPoint.anchorPointPosition, currentControlPoint.secondHandlePosition, nextControlPoint.firstHandlePosition, t);

                        if (pointCount > 1 && WaterUtility.AreColinear(points[pointCount - 2], points[pointCount - 1], position))
                        {
                            points[pointCount - 1] = position;
                        }
                        else
                        {
                            points.Add(position);
                            pointCount++;
                        }
                    }
                }

                Matrix4x4 maskSpaceToLocalSpace = _mainModule.WorldToLocalMatrix * _maskToWorldSpace;

                return GetClipperLibPointsListLocalSpace(points.ToArray(), maskSpaceToLocalSpace);
            }

            private static List<ClipperLib.IntPoint> GetWaterOrWaterfallPointsLocalSpace()
            {
                Vector2 halfSize = new Vector2(_mainModule.Width, _mainModule.Height) * 0.5f;

                var points = new Vector3[]
                {
                    new Vector3(-halfSize.x, -halfSize.y),
                    new Vector3(-halfSize.x, halfSize.y),
                    new Vector3(halfSize.x, halfSize.y),
                    new Vector3(halfSize.x, -halfSize.y)
                };

                return GetClipperLibPointsListLocalSpace(points, Matrix4x4.identity);
            }

            private static List<ClipperLib.IntPoint> GetClipperLibPointsListLocalSpace(Vector3[] points, Matrix4x4 toLocalSpaceMatrix)
            {
                var result = new List<ClipperLib.IntPoint>();

                for (int i = 0, imax = points.Length; i < imax; i++)
                {
                    var point = toLocalSpaceMatrix.MultiplyPoint3x4(points[i]);

                    result.Add(new ClipperLib.IntPoint(System.Math.Round(point.x, 5) * _clipperLibFloatToIntPrecision, System.Math.Round(point.y, 5) * _clipperLibFloatToIntPrecision));
                }

                return result;
            }

            private static void ApplyModifications()
            {
                _updateEdgeColliderPreviewShape = _updatePolygonColliderPreviewShape = true;
                _serializedObject.ApplyModifiedProperties();
            }

            private static void RepaintInspector()
            {
                if (_repaintInspectorCallback != null)
                    _repaintInspectorCallback.Invoke();
            }

            private static Vector2 DevideComponentWise(Vector2 a, Vector2 b)
            {
                return new Vector2
                {
                    x = a.x / b.x,
                    y = a.y / b.y
                };
            }

            private static int GetTotalElementCount<T>(List<T[]> list)
            {
                if (list == null)
                    return -1;

                int count = 0;

                for (int i = 0, imax = list.Count; i < imax; i++)
                {
                    count += list[i].Length;
                }

                return count;
            }

        }
    }

}