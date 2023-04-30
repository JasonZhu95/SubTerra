namespace Game2DWaterKit
{
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Rendering;
    using UnityEditor.AnimatedValues;
    using UnityEditor;
    using UnityEditorInternal;

    public class Game2DWaterKitManagerWindow : EditorWindow
    {
#if UNITY_2019_1_OR_NEWER
#if UNITY_2019_3_OR_NEWER
        private const string _SRPName = "UniversalRenderPipeline";
        private const string _SRPFullName = "Universal Render Pipeline (URP)";
        private const string _SRPShortName = "URP";
        private const string _srpShadersPackageGUID = "84c4cf967e584394795bfc5ebae62b25";
        private const string _srpShaderName = "Game2DWaterKit/Universal Render Pipeline/Unlit/Water";
        private const string _srpDefineSymbol = "GAME_2D_WATER_KIT_URP";
        private const string _lwrpDefineSymbol = "GAME_2D_WATER_KIT_LWRP";
#else
        private const string _SRPName = "LightweightRenderPipeline";
        private const string _SRPFullName = "Lightweight Render Pipeline (LWRP)";
        private const string _SRPShortName = "LWRP";
        private const string _srpShadersPackageGUID = "bec4a79152db4a84799833513c8eba9f";
        private const string _srpShaderName = "Game2DWaterKit/Lightweight Render Pipeline/Unlit/Water";
        private const string _srpDefineSymbol = "GAME_2D_WATER_KIT_LWRP";
#endif
        private static readonly string _projectSRPAssetSRPMessage = string.Format("The project is using the {0}. The asset is fully configured to use {1} features. No action is required!", _SRPFullName, _SRPShortName);
        private static readonly string _projectSRPAssetBuiltinMessage = string.Format("The project is using the {0}. The asset is not yet fully configured to use {1} features!", _SRPFullName, _SRPShortName);
        private static readonly string _projectBuiltinAssetSRPMessage = string.Format("The project is using the Built-in Render Pipeline. The asset is currently configured to use the {0} features!", _SRPFullName);
        private static readonly string _projectBuiltinAssetBuiltinMessage = "The project is using the Built-in Render Pipeline. The asset is fully configured to use the Built-in Render Pipeline features. No action is required!";
#endif
        private static Texture _assetLogo;
        private static GUIStyle _helpBoxStyle;
        private static GUIStyle _groupBoxStyle;
        private static GUIStyle _greenLabel;
        private static GUIStyle _orangeLabel;
        private static GUIStyle _redLabel;

        private static bool _isInitialized;
        private static Vector2 _scrollPosition;

        private static AnimBool _isSceneViewColorsPropertiesVisibleFoldout;

        private static UnityAction _repaint;

        public static string ActionRequiredMessage { get; private set; }
        public static bool IsOpen { get; private set; }

        [MenuItem("Window/Game2D Water Kit Asset Manager")]
        public static void ShowWindow()
        {
            GetWindow<Game2DWaterKitManagerWindow>(true, "G2DWK Manager", true);
        }

        private void OnEnable()
        {
            IsOpen = true;

#if UNITY_2019_3_OR_NEWER
            minSize = new Vector2(390, 360);
#else
            minSize = new Vector2(390, 340);
#endif

            _repaint = Repaint;
        }

        private void OnDisable()
        {
            IsOpen = false;
        }

        private void OnGUI()
        {
            if (!_isInitialized)
                Initialize();

            DrawAssetInformation();

            using(var scrollView = new EditorGUILayout.ScrollViewScope(_scrollPosition))
            {
#if UNITY_2019_1_OR_NEWER
                DrawPipelineOptions();
#endif
                DrawEditorPreferences();

                _scrollPosition = scrollView.scrollPosition;
            }
        }

        public static bool HasActionRequired()
        {
#if UNITY_2019_1_OR_NEWER
            var defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            bool isSRPCurrentActiveRenderPipeline = RenderPipelineManager.currentPipeline != null && RenderPipelineManager.currentPipeline.GetType().Name == _SRPName;
            bool hasSRPScriptingDefineSymbol = defineSymbols.Contains(_srpDefineSymbol);
            bool hasSRPShadersImported = Shader.Find(_srpShaderName) != null;

            if (isSRPCurrentActiveRenderPipeline)
            {
                if(!hasSRPScriptingDefineSymbol || !hasSRPShadersImported)
                {
                    ActionRequiredMessage = _projectSRPAssetBuiltinMessage;
                    return true;
                }
            }
            else
            {
                if (hasSRPScriptingDefineSymbol)
                {
                    ActionRequiredMessage = _projectBuiltinAssetSRPMessage;
                    return true;
                }
            }
#endif

            ActionRequiredMessage = null;
            return false;
        }

        private static void Initialize()
        {
            _helpBoxStyle = new GUIStyle("HelpBox");
            _groupBoxStyle = new GUIStyle("GroupBox");

            _greenLabel = new GUIStyle(EditorStyles.miniBoldLabel);
            _orangeLabel = new GUIStyle(EditorStyles.miniBoldLabel);
            _redLabel = new GUIStyle(EditorStyles.miniBoldLabel);

            _greenLabel.normal.textColor = GetColor(46, 139, 87);
            _orangeLabel.normal.textColor = GetColor(255, 140, 0);
            _redLabel.normal.textColor = Color.red;

            _assetLogo = Resources.Load("Images/asset_logo") as Texture;

            if (!Game2DWaterKitInspector.Game2DWaterKitStyles.IsInitialized)
                Game2DWaterKitInspector.Game2DWaterKitStyles.Initialize();

            Game2DWaterKitInspector.Game2DWaterKitStyles.UpdateHandlesColors();

            _isSceneViewColorsPropertiesVisibleFoldout = new AnimBool(() => _repaint.Invoke());

            _isInitialized = true;
        }

#if UNITY_2019_1_OR_NEWER
        private static void DrawPipelineOptions()
        {
            var defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            bool isSRPCurrentActiveRenderPipeline = RenderPipelineManager.currentPipeline != null && RenderPipelineManager.currentPipeline.GetType().Name == _SRPName;
            bool hasSRPScriptingDefineSymbol = defineSymbols.Contains(_srpDefineSymbol);
#if UNITY_2019_3_OR_NEWER
            bool hasLWRPScriptingDefineSymbol = defineSymbols.Contains(_lwrpDefineSymbol);
#endif
            bool hasSRPShadersImported = Shader.Find(_srpShaderName) != null;

            BeginBoxGroup(true);
            EditorGUILayout.LabelField("Render Pipeline", EditorStyles.boldLabel);

            if (isSRPCurrentActiveRenderPipeline && hasSRPScriptingDefineSymbol && hasSRPShadersImported)
                EditorGUILayout.HelpBox(_projectSRPAssetSRPMessage, MessageType.Info);

#if UNITY_2019_3_OR_NEWER
            if (!isSRPCurrentActiveRenderPipeline && !hasSRPScriptingDefineSymbol && !hasLWRPScriptingDefineSymbol)
#else
            if (!isSRPCurrentActiveRenderPipeline && !hasSRPScriptingDefineSymbol)
#endif
                EditorGUILayout.HelpBox(_projectBuiltinAssetBuiltinMessage, MessageType.Info);

            if (isSRPCurrentActiveRenderPipeline && (!hasSRPScriptingDefineSymbol || !hasSRPShadersImported))
            {
                EditorGUILayout.HelpBox(_projectSRPAssetBuiltinMessage, MessageType.Error);

                BeginBoxGroup(true);
                EditorGUILayout.LabelField(string.Format("{0} Define Symbol:", _SRPShortName), hasSRPScriptingDefineSymbol ? "Defined" : "Missing", hasSRPScriptingDefineSymbol ? _greenLabel : _redLabel);
                EditorGUILayout.LabelField(string.Format("{0} Shaders:", _SRPShortName), hasSRPShadersImported ? "Imported" : "Missing", hasSRPShadersImported ? _greenLabel : _redLabel);
                EndBoxGroup();

                if (GUILayout.Button(string.Format("Configure Asset to use {0}", _SRPShortName)))
                {
                    if (!hasSRPScriptingDefineSymbol)
                    {
                        Debug.Log(string.Format("Game 2D Water Kit - {0} : Adding {0} Scripting Define Symbol \"{1}\"", _SRPShortName, _srpDefineSymbol));
                        AddDefineSymbolToAllBuildTargetGroups(_srpDefineSymbol);
#if UNITY_2019_3_OR_NEWER
                        if (hasLWRPScriptingDefineSymbol)
                            RemoveDefineSymbolFromAllBuildTargetGroups(_lwrpDefineSymbol);
#endif
                    }

                    if (!hasSRPShadersImported)
                    {
                        Debug.Log(string.Format("Game 2D Water Kit - {0} : Importing {0} Compatible Shaders", _SRPShortName));
                        AssetDatabase.ImportPackage(AssetDatabase.GUIDToAssetPath(_srpShadersPackageGUID), false);
                    }
                }
            }

#if UNITY_2019_3_OR_NEWER
            if (!isSRPCurrentActiveRenderPipeline && (hasSRPScriptingDefineSymbol || hasLWRPScriptingDefineSymbol))
#else
            if (!isSRPCurrentActiveRenderPipeline && hasSRPScriptingDefineSymbol)
#endif
            {
                EditorGUILayout.HelpBox(_projectBuiltinAssetSRPMessage, MessageType.Error);

                if(GUILayout.Button("Configure Asset to use the Built-in Render Pipeline"))
                {
                    if (hasSRPScriptingDefineSymbol)
                    {
                        Debug.Log(string.Format("Game 2D Water Kit - Built-in Render Pipeline : Removing {0} Scripting Define Symbol \"{1}\"", _SRPShortName, _srpDefineSymbol));
                        RemoveDefineSymbolFromAllBuildTargetGroups(_srpDefineSymbol);
#if UNITY_2019_3_OR_NEWER
                        if (hasLWRPScriptingDefineSymbol)
                            RemoveDefineSymbolFromAllBuildTargetGroups(_lwrpDefineSymbol);
#endif
                    }
                }
            }

            EndBoxGroup();
        }
#endif

        private static void DrawAssetInformation()
        {
            EditorGUILayout.Space();

#if UNITY_2019_3_OR_NEWER
            const float logoDimensions = 130f;
#else
            const float logoDimensions = 120f;
#endif

            var totalRect = EditorGUILayout.GetControlRect(false, logoDimensions);

            var logoRect = new Rect(totalRect);
            logoRect.xMax = logoRect.xMin + logoDimensions;

            GUI.DrawTexture(logoRect, _assetLogo, ScaleMode.StretchToFill);

            var detailsRect = new Rect(totalRect);
            detailsRect.xMin += logoDimensions + 5f;

            GUI.Box(detailsRect, GUIContent.none, _groupBoxStyle);
            detailsRect.xMin += 5f;
            detailsRect.xMax -= 5f;
            detailsRect.yMin += 5f;
            detailsRect.height = EditorGUIUtility.singleLineHeight;
            GUI.Label(detailsRect, "Game 2D Water Kit v1.4.8 (b16.10.22)", EditorStyles.boldLabel);
            detailsRect.y += EditorGUIUtility.singleLineHeight - 3f;
            GUI.Label(detailsRect, "haydeludos.github.io/Game2DWaterKit-Documentation", EditorStyles.miniBoldLabel);
            EditorGUI.DrawRect(new Rect(detailsRect.x, detailsRect.y + EditorGUIUtility.singleLineHeight, detailsRect.width, 1f), EditorStyles.label.normal.textColor);
            detailsRect.y += EditorGUIUtility.singleLineHeight + 7f;
            if (GUI.Button(detailsRect, "Asset Website"))
                Application.OpenURL("https://haydeludos.github.io/Game2DWaterKit-Documentation/");
            detailsRect.y += EditorGUIUtility.singleLineHeight + 3f;
            if (GUI.Button(detailsRect, "Getting Started Guide"))
                Application.OpenURL("https://haydeludos.github.io/Game2DWaterKit-Documentation/#getting-started");
            detailsRect.y += EditorGUIUtility.singleLineHeight + 3f;
            if (GUI.Button(detailsRect, "Contact Publisher"))
                Application.OpenURL("https://haydeludos.github.io/Game2DWaterKit-Documentation/contact/");
            detailsRect.y += EditorGUIUtility.singleLineHeight + 3f;
            if (GUI.Button(detailsRect, "Rate/Review Asset"))
                Application.OpenURL("https://assetstore.unity.com/packages/slug/118057#reviews");
        }

        private void DrawEditorPreferences()
        {
            BeginBoxGroup(true);

            EditorGUI.BeginChangeCheck();

            EditorGUIUtility.labelWidth = 300f;

            var rect = EditorGUILayout.GetControlRect();
            var xMax = rect.xMax;

            rect.xMax = rect.xMin + EditorGUIUtility.labelWidth;

            _isSceneViewColorsPropertiesVisibleFoldout.target = EditorGUI.Foldout(rect, _isSceneViewColorsPropertiesVisibleFoldout.target, "Scene View Handles Colors", true, Game2DWaterKitInspector.Game2DWaterKitStyles.BoldFoldoutStyle);

            rect.xMin = rect.xMax;
            rect.xMax = xMax;

            if (GUI.Button(rect, "Reset"))
                Game2DWaterKitInspector.Game2DWaterKitStyles.ResetDefaultColors();

            using(var group = new EditorGUILayout.FadeGroupScope(_isSceneViewColorsPropertiesVisibleFoldout.faded))
            {
                if (group.visible)
                {
                    DrawColorField(Game2DWaterKitInspector.Game2DWaterKitStyles.BuoyancySurfaceLevelPreviewColor, "BuoyancySurfaceLevelPreviewColor", "Buoyancy Surface Level");
                    DrawColorField(Game2DWaterKitInspector.Game2DWaterKitStyles.BoxColliderPreviewColor, "BoxColliderPreviewColor", "Box Collider 2D");
                    DrawColorField(Game2DWaterKitInspector.Game2DWaterKitStyles.WaterSubdivisionsPreviewColor, "WaterSubdivisionsPreviewColor", "Water Subdivisions");
                    DrawColorField(Game2DWaterKitInspector.Game2DWaterKitStyles.CustomBoundariesPreviewColor, "CustomBoundariesPreviewColor", "Water Boundaries");
                    EditorGUILayout.LabelField("Simulation Mode:", EditorStyles.miniBoldLabel);
                    DrawColorField(Game2DWaterKitInspector.Game2DWaterKitStyles.OnCollisionRipplesSimulationRegionBoundariesColor, "OnCollisionRipplesSimulationRegionBoundariesColor", "On-Collision Ripples Region");
                    DrawColorField(Game2DWaterKitInspector.Game2DWaterKitStyles.ScriptGeneratedRipplesSimulationRegionBoundariesColor, "ScriptGeneratedRipplesSimulationRegionBoundariesColor", "Script-Generated Ripple Source Position");
                    EditorGUILayout.LabelField("Mesh Mask Editor:");
                    DrawColorField(Game2DWaterKitInspector.Game2DWaterKitStyles.MeshMaskToolDefaultControlPointColor, "MeshMaskToolDefaultControlPointColor", "Control Point - Default Color");
                    DrawColorField(Game2DWaterKitInspector.Game2DWaterKitStyles.MeshMaskToolDefaultControlPointHoveredColor, "MeshMaskToolDefaultControlPointHoveredColor", "Control Point - Hovered Color");
                    DrawColorField(Game2DWaterKitInspector.Game2DWaterKitStyles.MeshMaskToolDefaultControlPointSelectedColor, "MeshMaskToolDefaultControlPointSelectedColor", "Control Point - Selected Color");
                    DrawColorField(Game2DWaterKitInspector.Game2DWaterKitStyles.MeshMaskToolDefaultControlPointDisabledColor, "MeshMaskToolDefaultControlPointDisabledColor", "Control Point - Disabled Color");
                    DrawColorField(Game2DWaterKitInspector.Game2DWaterKitStyles.MeshMaskToolHandleColor, "MeshMaskToolHandleColor", "Tangent - Default Color");
                    DrawColorField(Game2DWaterKitInspector.Game2DWaterKitStyles.MeshMaskToolHandleDefaultHoveredColor, "MeshMaskToolHandleDefaultHoveredColor", "Tangent - Hovered Color");
                    DrawColorField(Game2DWaterKitInspector.Game2DWaterKitStyles.MeshMaskToolHandleDefaultSelectedColor, "MeshMaskToolHandleDefaultSelectedColor", "Tangent - Selected Color");
                    DrawColorField(Game2DWaterKitInspector.Game2DWaterKitStyles.MeshMaskToolSegmentColor, "MeshMaskToolSegmentColor", "Segment - Default Color");
                    DrawColorField(Game2DWaterKitInspector.Game2DWaterKitStyles.MeshMaskToolSegmentHoveredColor, "MeshMaskToolSegmentHoveredColor", "Segment - Hovered Color");
                    DrawColorField(Game2DWaterKitInspector.Game2DWaterKitStyles.MeshMaskToolSegmentSelectedColor, "MeshMaskToolSegmentSelectedColor", "Segment - Selected Color");
                    DrawColorField(Game2DWaterKitInspector.Game2DWaterKitStyles.MeshMaskToolDefaultEdgeColliderPointColor, "MeshMaskToolDefaultEdgeColliderPointColor", "Edge Collider - Default Color");
                    DrawColorField(Game2DWaterKitInspector.Game2DWaterKitStyles.MeshMaskToolDefaultEdgeColliderPointSelectedColor, "MeshMaskToolDefaultEdgeColliderPointSelectedColor", "Edge Collider - Selected Color");
                    DrawColorField(Game2DWaterKitInspector.Game2DWaterKitStyles.MeshMaskToolEdgeColliderOutlinePreviewColor, "MeshMaskToolEdgeColliderOutlinePreviewColor", "Edge Collider Outline - Preview Color");
                    DrawColorField(Game2DWaterKitInspector.Game2DWaterKitStyles.MeshMaskToolPolygonColliderOutlinePreviewColor, "MeshMaskToolPolygonColliderOutlinePreviewColor", "Polygon Collider Outline - Preview Color");
                    DrawColorField(Game2DWaterKitInspector.Game2DWaterKitStyles.MeshMaskToolObjectOutlinePreviewColor, "MeshMaskToolObjectOutlinePreviewColor", "Water/Waterfall Outline - Preview Color");
                    DrawColorField(Game2DWaterKitInspector.Game2DWaterKitStyles.MeshMaskToolMaskOutlinePreviewColor, "MeshMaskToolMaskOutlinePreviewColor", "Mask Outline - Preview Color");
                }
            }

            if (EditorGUI.EndChangeCheck())
                Game2DWaterKitInspector.Game2DWaterKitStyles.UpdateHandlesColors();

            EndBoxGroup();
        }

        private void DrawColorField(Color color, string key, string label)
        {
            EditorGUI.BeginChangeCheck();
            color = EditorGUILayout.ColorField(label, color);
            if (EditorGUI.EndChangeCheck())
                Game2DWaterKitInspector.Game2DWaterKitStyles.SetColorInEditorPrefs(key, color);
        }

        private static void BeginBoxGroup(bool useHelpBoxStyle = false)
        {
            EditorGUILayout.BeginVertical(useHelpBoxStyle ? _helpBoxStyle : _groupBoxStyle);
        }

        private static void EndBoxGroup()
        {
            EditorGUILayout.EndVertical();
        }

#if UNITY_2019_1_OR_NEWER
        private static void AddDefineSymbolToAllBuildTargetGroups(string defineSymbol)
        {
            AddDefineSymbolToBuildTargetGroup(BuildTargetGroup.Standalone, defineSymbol);
            AddDefineSymbolToBuildTargetGroup(BuildTargetGroup.Android, defineSymbol);
            AddDefineSymbolToBuildTargetGroup(BuildTargetGroup.WSA, defineSymbol);
            AddDefineSymbolToBuildTargetGroup(BuildTargetGroup.iOS, defineSymbol);
            AddDefineSymbolToBuildTargetGroup(BuildTargetGroup.tvOS, defineSymbol);
            AddDefineSymbolToBuildTargetGroup(BuildTargetGroup.Switch, defineSymbol);
            AddDefineSymbolToBuildTargetGroup(BuildTargetGroup.XboxOne, defineSymbol);
            AddDefineSymbolToBuildTargetGroup(BuildTargetGroup.PS4, defineSymbol);
            AddDefineSymbolToBuildTargetGroup(BuildTargetGroup.WebGL, defineSymbol);
#if UNITY_2019_3_OR_NEWER
            AddDefineSymbolToBuildTargetGroup(BuildTargetGroup.Stadia, defineSymbol);
#else
            AddDefineSymbolToBuildTargetGroup(BuildTargetGroup.Facebook, defineSymbol);
#endif
        }

        private static void RemoveDefineSymbolFromAllBuildTargetGroups(string defineSymbol)
        {
            RemoveDefineSymbolFromBuildTargetGroup(BuildTargetGroup.Standalone, defineSymbol);
            RemoveDefineSymbolFromBuildTargetGroup(BuildTargetGroup.Android, defineSymbol);
            RemoveDefineSymbolFromBuildTargetGroup(BuildTargetGroup.WSA, defineSymbol);
            RemoveDefineSymbolFromBuildTargetGroup(BuildTargetGroup.iOS, defineSymbol);
            RemoveDefineSymbolFromBuildTargetGroup(BuildTargetGroup.tvOS, defineSymbol);
            RemoveDefineSymbolFromBuildTargetGroup(BuildTargetGroup.Switch, defineSymbol);
            RemoveDefineSymbolFromBuildTargetGroup(BuildTargetGroup.XboxOne, defineSymbol);
            RemoveDefineSymbolFromBuildTargetGroup(BuildTargetGroup.PS4, defineSymbol);
            RemoveDefineSymbolFromBuildTargetGroup(BuildTargetGroup.WebGL, defineSymbol);
#if UNITY_2019_3_OR_NEWER
            RemoveDefineSymbolFromBuildTargetGroup(BuildTargetGroup.Stadia, defineSymbol);
#else
            RemoveDefineSymbolFromBuildTargetGroup(BuildTargetGroup.Facebook, defineSymbol);
#endif
        }

        private static void AddDefineSymbolToBuildTargetGroup(BuildTargetGroup buildTargetGroup, string defineSymbol)
        {
            var defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

            if (defineSymbols.Contains(defineSymbol))
                return;

            defineSymbols += ";" + _srpDefineSymbol;

            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defineSymbols);
        }

        private static void RemoveDefineSymbolFromBuildTargetGroup(BuildTargetGroup buildTargetGroup, string defineSymbol)
        {
            var defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

            if (!defineSymbols.Contains(defineSymbol))
                return;

            int index = defineSymbols.IndexOf(defineSymbol);
            int count = defineSymbol.Length;
            if (index > 0)
            {
                // count for the ";" preceding the define symbol
                index--;
                count++;
            }
            defineSymbols = defineSymbols.Remove(index, count);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defineSymbols);
        }
#endif

        private static Color GetColor(float r, float g, float b)
        {
            return new Color(r / 255, g / 255, b / 255);
        }
    }

}