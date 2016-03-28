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
        private Bounds _size;
        private float _cellSize;
        private float _cellHeight;
        private uint _widthCount;
        private uint _depthCount;
        private uint _heightCount;

        private VoxelSpan[] _voxelSpans;
        private VoxelCell[] _voxelCells;

        public float CellSize { get { return _cellSize; } }
        public float CellHeight { get { return _cellHeight; } }
        public Bounds AreaSize { get { return _size; } }

        public VoxelCell VoxelCell(int x, int z) { return _voxelCells[x + (z * _widthCount)]; }

        public VoxelArea(Bounds areaSize, float cellSize, float cellHeight)
        {
            _cellSize = cellSize;
            _cellHeight = cellHeight;
            _size = areaSize;

            _widthCount = (uint)Mathf.Ceil(areaSize.size.x / cellSize);
            _depthCount = (uint)Mathf.Ceil(areaSize.size.z / cellSize);
            _heightCount = (uint)Mathf.Ceil(areaSize.size.y / cellHeight);

            _voxelCells = new VoxelCell[_widthCount * _depthCount];
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
                
                if( _area.AreaSize.Intersects(renderer.bounds) )
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
                foreach(var ele in obj._triangles)
                {
                    
                }
                break;
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