namespace Game2DWaterKit.Utils
{
    using System.Collections.Generic;
    using UnityEngine;

    public static class WaterUtility
    {
        private static SimpleFixedSizeList<Vector2> _outputPoints = new SimpleFixedSizeList<Vector2>(8);
        internal static bool ClipPointsAgainstAABBEdge(SimpleFixedSizeList<Vector2> points, bool isBottomOrTopEdge, bool isBottomOrLeftEdge, float edgePosition)
        {
            int inputPointsCount = points.Count;

            if (inputPointsCount < 1)
                return false;

            _outputPoints.Clear();

            int coord = isBottomOrTopEdge ? 1 : 0; // 1->y : 0->x

            Vector2 previousPoint = points[inputPointsCount - 1];
            bool isPreviousPointInside = isBottomOrLeftEdge ? (previousPoint[coord] > edgePosition) : (previousPoint[coord] < edgePosition);

            bool areInputPointsUnchanged = isPreviousPointInside;

            for (int i = 0; i < inputPointsCount; i++)
            {
                Vector2 currentPoint = points[i];
                bool isCurrentPointInside = isBottomOrLeftEdge ? (currentPoint[coord] > edgePosition) : (currentPoint[coord] < edgePosition);

                if (isCurrentPointInside != isPreviousPointInside)
                {
                    //intersection
                    Vector2 dir = currentPoint - previousPoint;
                    float x = !isBottomOrTopEdge ? edgePosition : previousPoint.x + (dir.x / dir.y) * (edgePosition - previousPoint.y);
                    float y = isBottomOrTopEdge ? edgePosition : previousPoint.y + (dir.y / dir.x) * (edgePosition - previousPoint.x);
                    _outputPoints.Add(new Vector2(x, y));

                    areInputPointsUnchanged = false;
                }

                if (isCurrentPointInside)
                    _outputPoints.Add(currentPoint);

                previousPoint = currentPoint;
                isPreviousPointInside = isCurrentPointInside;
            }

            points.CopyFrom(_outputPoints);

            return areInputPointsUnchanged;
        }

        public static bool AreColinear(Vector2 point1, Vector2 point2, Vector2 point3)
        {
            return Mathf.Abs(point1.x * (point2.y - point3.y) + point2.x * (point3.y - point1.y) + point3.x * (point1.y - point2.y)) < 0.000005f;
        }

        public static float Min(float a, float b, float c, float d)
        {
            float m1 = a < b ? a : b;
            float m2 = c < d ? c : d;
            return m1 < m2 ? m1 : m2;
        }

        public static float Max(float a, float b, float c, float d)
        {
            float m1 = a > b ? a : b;
            float m2 = c > d ? c : d;
            return m1 > m2 ? m1 : m2;
        }

        public static void SafeDestroyObject(Object target)
        {
            if (target == null)
                return;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Object.DestroyImmediate(target);
                return;
            }
#endif
            Object.Destroy(target);
        }
    }

    public static class WaterTriangulator
    {
        private static Vector3[] _vertexPositions;
        private static List<int> _vertexIndices = new List<int>();

        public static int[] Triangulate(Vector3[] vertices)
        {
            _vertexPositions = vertices;

            SetupIndicesList();

            var vertexCount = _vertexPositions.Length;
            var triangleCount = 0;
            var currentVertexIndex = 0;

            var triangles = new List<int>();

            var maxIterations = vertexCount; // avoid infinite loop with self-intersecting polygons
            var processedVertexCount = 0;

            while ((maxIterations--) > 0)
            {
                var previousVertexIndex = GetPreviousValidIndex(currentVertexIndex);
                var nextVertexIndex = GetNextValidIndex(currentVertexIndex);

                if (!IsCurrentVertexReflex(currentVertexIndex) && IsValidTriangle(previousVertexIndex, currentVertexIndex, nextVertexIndex))
                {
                    triangles.Add(previousVertexIndex);
                    triangles.Add(currentVertexIndex);
                    triangles.Add(nextVertexIndex);
                    triangleCount++;

                    _vertexIndices[currentVertexIndex] = -1;
                    processedVertexCount++;

                    maxIterations = vertexCount - processedVertexCount;
                }

                currentVertexIndex = nextVertexIndex;
            }

            return triangles.ToArray();
        }

        private static bool IsValidTriangle(int previousVertexIndex, int currentVertexIndex, int nextVertexIndex)
        {
            var currentVertexPosition = _vertexPositions[currentVertexIndex];
            var previousVertexPosition = _vertexPositions[previousVertexIndex];
            var nextVertexPosition = _vertexPositions[nextVertexIndex];

            int i = GetNextValidIndex(nextVertexIndex);
            while (i != previousVertexIndex)
            {
                if (IsCurrentVertexReflex(i) && IsPointInsideTriangle(previousVertexPosition, currentVertexPosition, nextVertexPosition, _vertexPositions[i]))
                    return false;

                i = GetNextValidIndex(i);
            }

            return true;
        }

        private static void SetupIndicesList()
        {
            _vertexIndices.Clear();

            for (int i = 0, imax = _vertexPositions.Length; i < imax; i++)
            {
                _vertexIndices.Add(i);
            }
        }

        private static int GetPreviousValidIndex(int currentIndex)
        {
            int listCountMinusOne = _vertexIndices.Count - 1;
            int previousIndex = currentIndex - 1 < 0 ? listCountMinusOne : currentIndex - 1;

            while (_vertexIndices[previousIndex] == -1)
            {
                previousIndex = previousIndex - 1 < 0 ? listCountMinusOne : previousIndex - 1;
            }

            return previousIndex;
        }

        private static int GetNextValidIndex(int currentIndex)
        {
            int listCount = _vertexIndices.Count;
            int nextIndex = currentIndex + 1 < listCount ? currentIndex + 1 : 0;

            while (_vertexIndices[nextIndex] == -1)
            {
                nextIndex = nextIndex + 1 < listCount ? nextIndex + 1 : 0;
            }

            return nextIndex;
        }

        private static bool IsCurrentVertexReflex(int vertexIndex)
        {
            var currentVertexPosition = _vertexPositions[vertexIndex];
            var previousVertexPosition = _vertexPositions[GetPreviousValidIndex(vertexIndex)];
            var nextVertexPosition = _vertexPositions[GetNextValidIndex(vertexIndex)];

            return !IsCW(previousVertexPosition, currentVertexPosition, nextVertexPosition);
        }

        private static bool IsPointInsideTriangle(Vector2 a, Vector2 b, Vector2 c, Vector2 point)
        {
            return IsCW(a, b, point) && IsCW(b, c, point) && IsCW(c, a, point);
        }

        private static bool IsCW(Vector2 a, Vector2 b, Vector2 point)
        {
            return (b.x - a.x) * (point.y - a.y) - (b.y - a.y) * (point.x - a.x) < Mathf.Epsilon;
        }
    }

    internal class SimpleFixedSizeList<T>
    {
        private T[] _elements;
        private int _count;

        internal SimpleFixedSizeList(int size)
        {
            _elements = new T[size];
            _count = 0;
        }
        
        internal int Count { get { return _count; } }

        internal T this[int index]
        {
            get
            {
                //if (index < 0 || index > _count - 1)
                //    Debug.LogError("Index is out of range!");

                return _elements[index];
            }
        }

        internal void Add(T point)
        {
            //if (_count == _points.Length)
            //    Debug.LogError("Max size reached!");

            _elements[_count] = point;
            _count++;
        }

        internal void Clear()
        {
            _count = 0;
        }

        internal void CopyFrom(SimpleFixedSizeList<T> points)
        {
            //if (points._count > _count)
            //    Debug.LogError("Source array is larger than destination!");

            _count = points._count;
            for (int i = 0; i < _count; i++)
            {
                _elements[i] = points[i];
            }
        }
    }
}
