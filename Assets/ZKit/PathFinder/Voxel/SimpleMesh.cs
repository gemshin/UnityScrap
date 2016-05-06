using UnityEngine;

namespace ZKit.PathFinder
{
    public struct SimpleMesh
    {
        //public MeshFilter original;

        public int _area;
        public Vector3[] _vertices;
        public int[] _triangles;

        /** World bounds of the mesh. Assumed to already be multiplied with the matrix */
        public Bounds _bounds;

        public Matrix4x4 _matrix;

        public SimpleMesh(Vector3[] vertices, int[] triangles, Bounds bounds)
        {
            _matrix = Matrix4x4.identity;
            _vertices = vertices;
            _triangles = triangles;
            _bounds = bounds;
            //original = null;
            _area = 0;
        }

        public SimpleMesh(Vector3[] vertices, int[] triangles, Bounds bounds, Matrix4x4 matrix)
        {
            this._matrix = matrix;
            _vertices = vertices;
            _triangles = triangles;
            _bounds = bounds;
            //original = null;
            _area = 0;
        }

        /** Recalculate the bounds based on vertices and matrix */
        public void RecalculateBounds()
        {
            Bounds b = new Bounds(_matrix.MultiplyPoint3x4(_vertices[0]), Vector3.zero);

            for (int i = 1; i < _vertices.Length; i++)
            {
                b.Encapsulate(_matrix.MultiplyPoint3x4(_vertices[i]));
            }
            //Assigned here to avoid changing bounds if vertices would happen to be null
            _bounds = b;
        }
    }
}