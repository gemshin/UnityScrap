using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    public class VoxelArea
    {
        private Bounds _bound;
        private float _cellSize;
        private float _cellHeight;
        private uint _widthCount;
        private uint _depthCount;
        private uint _heightCount;

        //private VoxelSpan[] _voxelSpans;
        private VoxelCell[] _voxelCells;

        public float CellSize { get { return _cellSize; } }
        public float CellHeight { get { return _cellHeight; } }
        public Bounds AreaBound { get { return _bound; } }

        public VoxelArea(Bounds areaBound, float cellSize, float cellHeight)
        {
            _cellSize = cellSize;
            _cellHeight = cellHeight;
            _bound = areaBound;

            _widthCount = (uint)Mathf.Ceil(areaBound.size.x / cellSize);
            _depthCount = (uint)Mathf.Ceil(areaBound.size.z / cellSize);
            _heightCount = (uint)Mathf.Ceil(areaBound.size.y / cellHeight);

            _voxelCells = new VoxelCell[_widthCount * _heightCount * _depthCount];
        }

        /// <summary>
        /// Get Cell
        /// </summary>
        /// <param name="x">Horizontal count</param>
        /// <param name="y">Vertical count</param>
        /// <param name="z">Depth count</param>
        /// <returns>VoxelCell</returns>
        public VoxelCell GetCell(uint x, uint y, uint z) { return _voxelCells[x + (y*_widthCount) + (z * (_widthCount*_heightCount))]; }
        /// <summary>
        /// Get Cell
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>VoxelCell</returns>
        public VoxelCell GetCell(uint index) { return _voxelCells[index]; }
        /// <summary>
        /// Get Cell
        /// </summary>
        /// <param name="point">Point position(world)</param>
        /// <returns></returns>
        public VoxelCell GetCell(Vector3 point)
        {
            int x, y, z;
            if (!GetCellCount(point, out x, out y, out z)) return null;
            return GetCell((uint)x, (uint)y, (uint)z);
        }
        /// <summary>
        /// Get Index
        /// </summary>
        /// <param name="x">Horizontal count</param>
        /// <param name="y">Vertical count</param>
        /// <param name="z">Depth count</param>
        /// <returns>index</returns>
        public uint GetIndex(uint x, uint y, uint z) { return (x + (y * _widthCount) + (z * _widthCount * _heightCount)); }
        /// <summary>
        /// Get Cell Count
        /// </summary>
        /// <param name="index">index</param>
        /// <param name="x">out Horizontal count</param>
        /// <param name="y">out Vertical count</param>
        /// <param name="z">out Depth count</param>
        /// <returns>Success or Fail</returns>
        public bool GetCellCount(uint index, out int x, out int y, out int z)
        {
            x = y = z = 0;
            if (index >= _voxelCells.Length) return false;

            z = Mathf.FloorToInt(index / (_widthCount * _heightCount));
            y = Mathf.FloorToInt((index - z) / _widthCount);
            x = Mathf.FloorToInt((index - z) % _widthCount);
            return true;
        }
        /// <summary>
        /// Get Cell Count
        /// </summary>
        /// <param name="point">Point position(world)</param>
        /// <param name="x">out Horizontal count</param>
        /// <param name="y">out Vertical count</param>
        /// <param name="z">out Depth count</param>
        /// <returns>Success or Fail</returns>
        public bool GetCellCount(Vector3 point, out int x, out int y, out int z)
        {
            x = y = z = 0;
            if (!_bound.Contains(point)) return false;

            var areaPosition = point - _bound.min;
            x = Mathf.FloorToInt(_cellSize / areaPosition.x);
            y = Mathf.FloorToInt(_cellHeight / areaPosition.y);
            z = Mathf.FloorToInt(_cellSize / areaPosition.z);
            return true;
        }
    }

    public class Voxel : NonPubSingleton<Voxel>
    {
        private VoxelArea _area;

        private List<SimpleMesh> _inObjects = new List<SimpleMesh>();

        private int _pathLayerMask = 0;
        private int _obstacleLayerMask = 0;

        public VoxelArea VoxelArea { get { return _area; } }

        public void InitVoxelArea(float cellSize, float cellHeight, int pathLayerMask, int obstacleLayerMask)
        {
            _pathLayerMask = pathLayerMask;
            _obstacleLayerMask = obstacleLayerMask;
            _area = new VoxelArea(UUtil.ScanMapSize3D(_pathLayerMask, _obstacleLayerMask), cellSize, cellHeight);
            _inObjects.Clear();
        }

        public void ScanVoxelSpace()
        {
            CollectObjects();
            Voxelize();
        }

        private void CollectObjects()
        {
            //var cachedVertices = new Dictionary<Mesh, Vector3[]>();
            //var cachedTris = new Dictionary<Mesh, int[]>();

            foreach (MeshFilter filter in GameObject.FindObjectsOfType<MeshFilter>())
            {
                var renderer = filter.GetComponent<MeshRenderer>();
                if (filter.sharedMesh == null || !renderer.enabled) continue;
                if (((_pathLayerMask | _obstacleLayerMask) & (1 << filter.gameObject.layer)) == 0) continue;
                
                if( _area.AreaBound.Intersects(renderer.bounds) )
                {
                    Mesh mesh = filter.sharedMesh;
                    SimpleMesh smesh = new SimpleMesh();

                    smesh._matrix = renderer.localToWorldMatrix;

                    //if (cachedVertices.ContainsKey(mesh))
                    //{
                    //    smesh._vertices = cachedVertices[mesh];
                    //    smesh._triangles = cachedTris[mesh];
                    //}
                    //else
                    {
                        smesh._vertices = mesh.vertices;
                        smesh._triangles = mesh.triangles;
                        //cachedVertices[mesh] = smesh._vertices;
                        //cachedTris[mesh] = smesh._triangles;
                    }
                    smesh._bounds = renderer.bounds;

                    _inObjects.Add(smesh);
                }
            }
        }
        private void Voxelize()
        {
            foreach(var obj in _inObjects)
            {
                VoxelizeObject(obj);
            }
        }

        private void VoxelizeObject(SimpleMesh mesh)
        {
            VoxelArea va = VoxelArea;

            for (int i = 0; i < mesh._triangles.Length; i+=3)
            {
                var p1 = mesh._vertices[i];
                var p2 = mesh._vertices[i + 1];
                var p3 = mesh._vertices[i + 2];
            }
        }
    }

    public class VoxelSpan
    {
        private uint y;
        private uint h;

        public VoxelSpan next;

    }

    public class VoxelCell
    {
        private uint index;

        private VoxelSpan span;
    }
}