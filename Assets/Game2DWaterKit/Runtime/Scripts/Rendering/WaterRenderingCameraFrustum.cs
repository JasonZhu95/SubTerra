namespace Game2DWaterKit.Rendering
{
    using UnityEngine;
    using Game2DWaterKit.Main;
    using Game2DWaterKit.Utils;

    internal class WaterRenderingCameraFrustum
    {
        private MainModule _mainModule;
        internal Camera CurrentCamera { get; private set; }
        internal Vector3 Position { get; private set; }
        internal Vector3 Rotation { get; private set; }
        internal Vector2 TopLeft { get; private set; }
        internal Vector2 TopRight { get; private set; }
        internal Vector2 BottomLeft { get; private set; }
        internal Vector2 BottomRight { get; private set; }
        internal bool IsIsometric { get; private set; }
        internal Vector2 AABBMin
        {
            get
            {
                Vector2 min;
                min.x = WaterUtility.Min(TopLeft.x, TopRight.x, BottomLeft.x, BottomRight.x);
                min.y = WaterUtility.Min(TopLeft.y, TopRight.y, BottomLeft.y, BottomRight.y);
                return min;
            }
        }
        internal Vector2 AABBMax
        {
            get
            {
                Vector2 max;
                max.x = WaterUtility.Max(TopLeft.x, TopRight.x, BottomLeft.x, BottomRight.x);
                max.y = WaterUtility.Max(TopLeft.y, TopRight.y, BottomLeft.y, BottomRight.y);
                return max;
            }
        }

        internal WaterRenderingCameraFrustum(MainModule mainModule)
        {
            _mainModule = mainModule;
        }

        internal void Setup(RenderingCameraInformation renderingCameraInformation)
        {
            CurrentCamera = renderingCameraInformation.CurrentCamera;

            Position = _mainModule.TransformPointWorldToLocal(renderingCameraInformation.Position);

            var cameraRotationEulerAngles = renderingCameraInformation.RotationEulerAngles;
            cameraRotationEulerAngles.z -= _mainModule.ZRotation;
            Rotation = cameraRotationEulerAngles;

            IsIsometric = renderingCameraInformation.IsIsometric;

            var nTopLeft = _mainModule.TransformPointWorldToLocal(renderingCameraInformation.nearFrustumPlaneTopLeft);
            var nTopRight = _mainModule.TransformPointWorldToLocal(renderingCameraInformation.nearFrustumPlaneTopRight);
            var nBottomLeft = _mainModule.TransformPointWorldToLocal(renderingCameraInformation.nearFrustumPlaneBottomLeft);
            var nBottomRight = _mainModule.TransformPointWorldToLocal(renderingCameraInformation.nearFrustumPlaneBottomRight);

            if (CurrentCamera.orthographic)
            {
                if (!IsIsometric)
                {
                    TopLeft = nTopLeft;
                    TopRight = nTopRight;
                    BottomLeft = nBottomLeft;
                    BottomRight = nBottomRight;
                }
                else
                {
                    var cameraForwardDirection = _mainModule.TransformVectorWorldToLocal(renderingCameraInformation.ForwardDirection);

                    TopLeft = ComputeIntersectionOfLineWithWaterPlane(nTopLeft, cameraForwardDirection);
                    TopRight = ComputeIntersectionOfLineWithWaterPlane(nTopRight, cameraForwardDirection);
                    BottomLeft = ComputeIntersectionOfLineWithWaterPlane(nBottomLeft, cameraForwardDirection);
                    BottomRight = ComputeIntersectionOfLineWithWaterPlane(nBottomRight, cameraForwardDirection);
                }
            }
            else
            {
                var halfWidth = _mainModule.Width * 0.5f;
                var halfHeight = _mainModule.Height * 0.5f;

                var cameraPosition = Position;
                var cameraZPositionSign = cameraPosition.z > 0f;

                TopLeft = cameraZPositionSign ^ (nTopLeft.z > cameraPosition.z) ? ComputeIntersectionOfLineWithWaterPlane(cameraPosition, nTopLeft - cameraPosition) : new Vector2(-halfWidth, halfHeight);
                TopRight = cameraZPositionSign ^ (nTopRight.z > cameraPosition.z) ? ComputeIntersectionOfLineWithWaterPlane(cameraPosition, nTopRight - cameraPosition) : new Vector2(halfWidth, halfHeight);
                BottomLeft = cameraZPositionSign ^ (nBottomLeft.z > cameraPosition.z) ? ComputeIntersectionOfLineWithWaterPlane(cameraPosition, nBottomLeft - cameraPosition) : new Vector2(-halfWidth, -halfHeight);
                BottomRight = cameraZPositionSign ^ (nBottomRight.z > cameraPosition.z) ? ComputeIntersectionOfLineWithWaterPlane(cameraPosition, nBottomRight - cameraPosition) : new Vector2(halfWidth, -halfHeight);
            }
        }

        private Vector2 ComputeIntersectionOfLineWithWaterPlane(Vector3 linePoint, Vector3 lineDirection)
        {
            float d = -linePoint.z / lineDirection.z;
            return new Vector2(linePoint.x + lineDirection.x * d, linePoint.y + lineDirection.y * d);
        }
    }
}
