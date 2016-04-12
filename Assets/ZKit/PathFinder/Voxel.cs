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
        private SortedList<uint, VoxelCell> _walkableCells = new SortedList<uint, VoxelCell>();

        public float CellSize { get { return _cellSize; } }
        public float CellHeight { get { return _cellHeight; } }
        public Bounds AreaBound { get { return _bound; } }
        public uint WidthCount { get { return _widthCount; } }
        public uint HeightCount { get { return _heightCount; } }
        public uint DepthCount { get { return _depthCount; } }

        public IList<VoxelCell> WalkableCells { get { return _walkableCells.Values; } }

        public VoxelArea(Bounds areaBound, float cellSize, float cellHeight)
        {
            _cellSize = cellSize;
            _cellHeight = cellHeight;
            _bound = areaBound;

            _widthCount = (uint)Mathf.Ceil(_bound.size.x / _cellSize);
            _depthCount = (uint)Mathf.Ceil(_bound.size.z / _cellSize);
            _heightCount = (uint)Mathf.Ceil(_bound.size.y / _cellHeight);

            _bound.max = new Vector3(_bound.min.x + (_widthCount*_cellSize)
                                    , _bound.min.y + (_heightCount * _cellHeight)
                                    , _bound.min.z + (_depthCount * _cellSize));

            _voxelCells = new VoxelCell[_widthCount * _heightCount * _depthCount];

            for (uint i = 0; i < _voxelCells.Length; ++i)
                _voxelCells[i] = new VoxelCell(i);
        }

        /// <summary>
        /// Get Cell
        /// </summary>
        /// <param name="x">Horizontal count</param>
        /// <param name="y">Vertical count</param>
        /// <param name="z">Depth count</param>
        /// <returns>VoxelCell or null</returns>
        public VoxelCell GetCell(uint x, uint y, uint z)
        {
            uint index;
            if (!GetCellIndex(out index, x, y, z)) return null;
            return _voxelCells[index];
        }
        /// <summary>
        /// Get Cell
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>VoxelCell or null</returns>
        public VoxelCell GetCell(uint index)
        {
            if (_voxelCells == null || index >= _voxelCells.Length) return null;
            return _voxelCells[index];
        }
        /// <summary>
        /// Get Cell
        /// </summary>
        /// <param name="point">Point position(world)</param>
        /// <returns>VoxelCell or null</returns>
        public VoxelCell GetCell(Vector3 point)
        {
            int x, y, z;
            if (!GetCellCount(point, out x, out y, out z)) return null;
            return GetCell((uint)x, (uint)y, (uint)z);
        }
        /// <summary>
        /// Get Cell Index
        /// </summary>
        /// <param name="index">out Index</param>
        /// <param name="x">Horizontal count</param>
        /// <param name="y">Vertical count</param>
        /// <param name="z">Depth count</param>
        /// <returns>Success or Fail</returns>
        public bool GetCellIndex(out uint index, uint x, uint y, uint z)
        {
            index = (x + (y * _widthCount) + (z * _widthCount * _heightCount));
            if (_voxelCells == null || index >= _voxelCells.Length) return false;
            return true;
        }
        /// <summary>
        /// Get Cell Count
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="x">out Horizontal count</param>
        /// <param name="y">out Vertical count</param>
        /// <param name="z">out Depth count</param>
        /// <returns>Success or Fail</returns>
        public bool GetCellCount(uint index, out int x, out int y, out int z)
        {
            x = y = z = 0;
            if (index >= _voxelCells.Length) return false;

            z = (int)(index / (_widthCount * _heightCount));
            int tmp = (int)(z * _widthCount * _heightCount);
            y = (int)((index - tmp) / _widthCount);
            x = (int)((index - tmp) % _widthCount);
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
            x = areaPosition.x == 0f ? 0 : (int)(areaPosition.x / _cellSize);
            y = areaPosition.y == 0f ? 0 : (int)(areaPosition.y / _cellHeight);
            z = areaPosition.z == 0f ? 0 : (int)(areaPosition.z / _cellSize);
            if (x >= _widthCount || y >= _heightCount || z >= _depthCount) return false;
            return true;
        }
        /// <summary>
        /// Get Cell Center Position
        /// </summary>
        /// <param name="index">Cell Index</param>
        /// <param name="position">out Cell Center Position</param>
        /// <returns></returns>
        public bool GetCellPosition(uint index, out Vector3 position)
        {
            int x, y, z;
            position = Vector3.zero;
            if (!GetCellCount(index, out x, out y, out z)) return false;
            position.x = x * _cellSize + _cellSize * 0.5f;
            position.y = y * _cellHeight + _cellHeight * 0.5f;
            position.z = z * _cellSize + _cellSize * 0.5f;
            position = position + _bound.min;
            return true;
        }

        public void SetWalkableCell(Vector3 point)
        {
            var cell = GetCell(point);
            cell.walkable = true;
            _walkableCells.Add(cell.Index, cell);
        }

        public void SetWalkableCell(VoxelCell cell)
        {
            cell.walkable = true;
            _walkableCells.Add(cell.Index, cell);
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
            _area = new VoxelArea(UUtil.ScanMapSize3D(_pathLayerMask, _obstacleLayerMask, 0f), cellSize, cellHeight);
            _inObjects.Clear();
        }

        public void ScanVoxelSpace()
        {
            CollectObjects();
            Voxelize();
        }

        private void CollectObjects()
        {
            foreach (MeshFilter filter in GameObject.FindObjectsOfType<MeshFilter>())
            {
                var renderer = filter.GetComponent<MeshRenderer>();
                if (filter.sharedMesh == null || !renderer.enabled) continue;
                if (((_pathLayerMask | _obstacleLayerMask) & (1 << filter.gameObject.layer)) == 0) continue;

                if (_area.AreaBound.Intersects(renderer.bounds))
                {
                    Mesh mesh = filter.sharedMesh;
                    SimpleMesh smesh = new SimpleMesh();
                    smesh._matrix = renderer.localToWorldMatrix;

                    // 중복 버티스 제거 코드인데 더 느리다. 삭제준비중.
                    //var cacheVertices = new List<Vector3>();
                    //var cacheTris = new List<int>(mesh.triangles);
                    //foreach(var vertice in mesh.vertices)
                    //    if (!cacheVertices.Contains(vertice))
                    //        cacheVertices.Add(vertice);
                    //for (int i = 0; i < cacheTris.Count; ++i)
                    //{
                    //    cacheTris[i] = cacheVertices.FindIndex(x => x == mesh.vertices[cacheTris[i]]);
                    //}
                    //smesh._vertices = cacheVertices.ToArray();
                    //smesh._triangles = cacheTris.ToArray();

                    smesh._vertices = mesh.vertices;
                    smesh._triangles = mesh.triangles;

                    smesh._bounds = renderer.bounds;

                    _inObjects.Add(smesh);
                }
            }
        }
        private void Voxelize()
        {
            foreach (var obj in _inObjects)
            {
                VoxelizeObject(obj);
            }
        }

        private void VoxelizeObject(SimpleMesh mesh)
        {
            VoxelArea va = VoxelArea;

            for(int i = 0; i < mesh._triangles.Length; i+=3)
            {
                int min_x, min_y, min_z, max_x, max_y, max_z;
                Vector3 minPos = Vector3.Min(Vector3.Min(mesh._vertices[mesh._triangles[i]], mesh._vertices[mesh._triangles[i + 1]]), mesh._vertices[mesh._triangles[i + 2]]);
                Vector3 maxPos = Vector3.Max(Vector3.Max(mesh._vertices[mesh._triangles[i]], mesh._vertices[mesh._triangles[i + 1]]), mesh._vertices[mesh._triangles[i + 2]]);
                minPos = mesh._matrix.MultiplyPoint3x4(minPos);
                maxPos = mesh._matrix.MultiplyPoint3x4(maxPos);

                if (!va.GetCellCount(minPos, out min_x, out min_y, out min_z))
                {
                    Debug.Log("bound min err");
                    return;
                }
                if (!va.GetCellCount(maxPos, out max_x, out max_y, out max_z))
                {
                    Debug.Log("bound max err");
                    return;
                }

                for (uint z = (uint)min_z; z <= max_z; ++z)
                {
                    for (uint y = (uint)min_y; y <= max_y; ++y)
                    {
                        for (uint x = (uint)min_x; x <= max_x; ++x)
                        {
                            var cell = va.GetCell(x, y, z);
                            if (cell == null || cell.walkable) continue;
                            va.SetWalkableCell(cell);
                        }
                    }
                }
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
        private uint _index;
        public bool walkable = false;

        private VoxelSpan span;

        public uint Index { get { return _index; } }

        public VoxelCell(uint index) { _index = index; }
    }
}