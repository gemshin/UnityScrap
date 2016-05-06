using UnityEngine;
using System.Collections.Generic;
using ZKit.Math.Geometry;

namespace ZKit.PathFinder
{
    public class VoxelArea // Voxel 공간 데이터.
    {
        public enum DIRECTION
        {
            Left,
            Right,
            Forward,
            Backward,
            Upper,
            Lower,
        }

        private Bounds _bound;
        private float _cellSize;    // Voxel 길이와 너비
        private float _cellHeight;  // Voxel 높이
        private uint _widthCount;   // Voxel 너비 갯수 (x Axis)
        private uint _depthCount;   // Voxel 깊이 갯수 (z Axis)
        private uint _heightCount;  // Voxel 높이 갯수 (y Axis)
        private float _agentRadius; // 플레이어 반지름
        private float _agentHeight; // 플레이어 높이
        private float _maxSlope;    // 오를수 있는 경사도
        private float _maxClimb;    // 오를수 있는 높이

        public float CellSize { get { return _cellSize; } }
        public float CellHeight { get { return _cellHeight; } }
        public Bounds AreaBound { get { return _bound; } }
        public uint WidthCount { get { return _widthCount; } }
        public uint HeightCount { get { return _heightCount; } }
        public uint DepthCount { get { return _depthCount; } }
        public float AgentHeight { get { return _agentHeight; } }
        public float AgentRadius { get { return _agentRadius; } }
        public float MaxSlope { get { return _maxSlope; } }
        public float MaxClimb { get { return _maxClimb; } }

        private Voxel[] _voxelCells;    // Voxels..
        public Voxel[] Voxels { get { return _voxelCells; } }
        public IList<Voxel> WalkableCells { get { return _walkableCells.Values; } }
        private SortedList<uint, Voxel> _walkableCells = new SortedList<uint, Voxel>();
        public IList<Voxel> FirstLedges { get { return _firstLedges.Values; } }
        private SortedList<uint, Voxel> _firstLedges = new SortedList<uint, Voxel>();
        public void AddFirstLedge(Voxel firstLedge) { _firstLedges.Add(firstLedge.Index, firstLedge); }
        public IList<Voxel> Ledges { get { return _ledges.Values; } }
        private SortedList<uint, Voxel> _ledges = new SortedList<uint, Voxel>();
        public void AddLedge(Voxel ledge) { _ledges.Add(ledge.Index, ledge); }

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
            _voxelCells = new Voxel[_widthCount * _heightCount * _depthCount];

            for (uint i = 0; i < _voxelCells.Length; ++i)
                _voxelCells[i] = new Voxel(i);
        }
        /// <summary>
        /// Agent 정보를 입력한다. VoxelArea 생성시 호출되어야 하기 때문에 나중에 구조조정이 필요.  
        /// </summary>
        /// <param name="agentHeight"></param>
        /// <param name="agentRadius"></param>
        /// <param name="maxClimb"></param>
        /// <param name="maxSlope"></param>
        public void SetAgentInfo(float agentHeight, float agentRadius, float maxClimb, float maxSlope)
        {
            _agentHeight = agentHeight;
            _agentRadius = agentRadius;
            _maxClimb = maxClimb;
            _maxSlope = maxSlope;
        }
        /// <summary>
        /// 연결된 쎌의 꼭대기 쎌을 가져온다.
        /// </summary>
        /// <param name="index">기준 쎌</param>
        /// <param name="max">검사를 수행할 수</param>
        /// <returns>꼭대기 쎌, 아니면 null</returns>
        public Voxel GetTopSpanCell(uint index, uint max = 1024)
        {
            uint x, y, z;
            if (!GetCellCount(index, out x, out y, out z)) return null;
            Voxel tmp;
            for(uint i = 1; i < max; ++i)
            {
                if ((tmp = GetCell(x, y + i, z)) == null) return null;
                if (tmp.TopSpan) return tmp;
            }
            return null;
        }
        /// <summary>
        /// 연결된 쎌의 바닥 쎌을 가져온다.
        /// </summary>
        /// <param name="index">기준 쎌</param>
        /// <param name="max">검사를 수행할 수</param>
        /// <returns>바닥 쎌, 아니면 null</returns>
        public Voxel GetBottomSpanCell(uint index, uint max = 1024)
        {
            uint x, y, z;
            if (!GetCellCount(index, out x, out y, out z)) return null;

            Voxel tmp;
            for (uint i = 1; i < max; ++i)
            {
                if ((tmp = GetCell(x, y - i, z)) == null) return null;
                if (tmp.TopSpan) return tmp;
            }
            return null;
        }
        /// <summary>
        /// 기준 쎌로부터 지정된 방향의 쎌을 가져온다.
        /// </summary>
        /// <param name="direction">방향</param>
        /// <param name="index">기준 쎌 인덱스</param>
        /// <returns>지정된 방향의 쎌, 아니면 null</returns>
        public Voxel GetCell(DIRECTION direction, uint index)
        {
            uint x, y, z;
            if (!GetCellCount(index, out x, out y, out z)) return null;
            switch(direction)
            {
                case DIRECTION.Left:
                    ++x;
                    break;
                case DIRECTION.Right:
                    --x;
                    break;
                case DIRECTION.Forward:
                    ++z;
                    break;
                case DIRECTION.Backward:
                    --z;
                    break;
                case DIRECTION.Upper:
                    ++y;
                    break;
                case DIRECTION.Lower:
                    --y;
                    break;
            }
            return GetCell(x, y, z);
        }
        /// <summary>
        /// Get Cell
        /// </summary>
        /// <param name="x">Horizontal count</param>
        /// <param name="y">Vertical count</param>
        /// <param name="z">Depth count</param>
        /// <returns>VoxelCell or null</returns>
        public Voxel GetCell(uint x, uint y, uint z)
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
        public Voxel GetCell(uint index)
        {
            if (_voxelCells == null || index >= _voxelCells.Length) return null;
            return _voxelCells[index];
        }
        /// <summary>
        /// Get Cell
        /// </summary>
        /// <param name="point">Point position(world)</param>
        /// <returns>VoxelCell or null</returns>
        public Voxel GetCell(Vector3 point)
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
        public bool GetCellCount(uint index, out uint x, out uint y, out uint z)
        {
            x = y = z = 0u;
            if (index >= _voxelCells.Length) return false;

            z = index / (_widthCount * _heightCount);
            uint tmp = z * _widthCount * _heightCount;
            y = (index - tmp) / _widthCount;
            x = (index - tmp) % _widthCount;
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
            uint x, y, z;
            position = Vector3.zero;
            if (!GetCellCount(index, out x, out y, out z)) return false;
            position.x = x * _cellSize + _cellSize * 0.5f;
            position.y = y * _cellHeight + _cellHeight * 0.5f;
            position.z = z * _cellSize + _cellSize * 0.5f;
            position = position + _bound.min;
            position.x = Mathf.Round(position.x * 1000f) * 0.001f;
            position.y = Mathf.Round(position.y * 1000f) * 0.001f;
            position.z = Mathf.Round(position.z * 1000f) * 0.001f;
            return true;
        }

        //public void SetWalkableCell(Vector3 point)
        //{
        //    SetWalkableCell(GetCell(point));
        //}

        public void RemoveWalkableCell(uint index)
        {
            ////GetTopCell(index);
            //if(_walkableCells[index].TopSpan)
            //{
            //    var lower = GetCell(DIRECTION.Lower, index);
            //    if (lower != null && lower.CellType == VOXEL_TYPE.Walkable)
            //        lower.TopSpan = true;
            //}
            //_walkableCells[index].CellType = VOXEL_TYPE.None;
            _walkableCells.Remove(index);
        }

        public void SetWalkableCell(Voxel cell, bool reverseSide = false)
        {
            if (!reverseSide)
                cell.CellType = VOXEL_NAVI_TYPE.Walkable;
            else
            {
                cell.CellType = VOXEL_NAVI_TYPE.ReverseSide;
                return;
            }

            cell.TopSpan = true;

            Voxel upperCell = GetCell(DIRECTION.Upper, cell.Index);
            if( upperCell != null )
            {
                if (upperCell.CellType == VOXEL_NAVI_TYPE.Walkable || upperCell.CellType == VOXEL_NAVI_TYPE.ReverseSide)
                {
                    cell.TopSpan = false;
                }
            }
            Voxel lowerCell = GetCell(DIRECTION.Lower, cell.Index);
            if (lowerCell != null)
            {
                if ((lowerCell.CellType == VOXEL_NAVI_TYPE.Walkable || lowerCell.CellType == VOXEL_NAVI_TYPE.ReverseSide) && lowerCell.TopSpan)
                {
                    _walkableCells.Remove(lowerCell.Index);
                    lowerCell.TopSpan = false;
                }
            }

            //if (cell.TopSpan && !reverseSide) _walkableCells.Add(cell.Index, cell); // top만 등록.
            if (cell.TopSpan) _walkableCells.Add(cell.Index, cell); // top만 등록.
        }
    }

    public class Voxelizer : NonPubSingleton<Voxelizer>
    {
        private VoxelArea _area;

        private List<SimpleMesh> _inObjects = new List<SimpleMesh>();
        private List<SimpleMesh> _exceptObjects = new List<SimpleMesh>();

        private int _pathLayerMask = 0;
        private int _obstacleLayerMask = 0;
        private int _exceptLayerMask = 0;

        public VoxelArea VoxelArea { get { return _area; } }

        public void InitVoxelArea(float cellSize, float cellHeight, int pathLayerMask, int obstacleLayerMask, int exceptLayerMask)
        {
            _pathLayerMask = pathLayerMask;
            _obstacleLayerMask = obstacleLayerMask;
            _exceptLayerMask = exceptLayerMask;
            _area = new VoxelArea(UUtil.ScanMapSize3D(_pathLayerMask, _obstacleLayerMask, cellSize), cellSize, cellHeight);
            _inObjects.Clear();
            _exceptObjects.Clear();
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
                if (((_pathLayerMask | _obstacleLayerMask | _exceptLayerMask) & (1 << filter.gameObject.layer)) == 0) continue;

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

                    if ((_exceptLayerMask & (1 << filter.gameObject.layer)) != 0)
                        _exceptObjects.Add(smesh);
                    else
                        _inObjects.Add(smesh);
                }
            }
        }
        private void Voxelize()
        {
            foreach(var obj in _exceptObjects)
            {
                RegistExceptVoxel(obj);
            }
            foreach (var obj in _inObjects)
            {
                VoxelizeObject(obj);
            }
            FilterFirstLedge();
            FilterLowCeilingPlace();
            FilterLedge();

            //BuildConnection();
            //BuildDistanceField();
        }

        /// <summary>
        /// 쎌 외곽선 1줄을 걸러낸다.
        /// </summary>
        private void FilterFirstLedge()
        {
            BuildConnection();
            var va = VoxelArea;
            List<uint> deletingReservations = new List<uint>();
            foreach (var ele in va.WalkableCells)
            {
                if (!ele.ConnectionBackward || !ele.ConnectionForward || !ele.ConnectionLeft || !ele.ConnectionRight)
                    deletingReservations.Add(ele.Index);
            }
            // 지워질 애들은 다 Top 이다.
            foreach (var ele in deletingReservations)
            {
                va.RemoveWalkableCell(ele);
                var ledge = va.GetCell(ele);
                if( ledge != null )
                {
                    va.AddFirstLedge(ledge);
                    ledge.CellType = VOXEL_NAVI_TYPE.FirstLedge;
                }
            }
        }

        private void FilterLedge()
        {
            BuildConnection();
            var va = VoxelArea;
            List<uint> deletingReservations = new List<uint>();
            foreach (var ele in va.WalkableCells)
            {
                if (!ele.ConnectionBackward || !ele.ConnectionForward || !ele.ConnectionLeft || !ele.ConnectionRight)
                    deletingReservations.Add(ele.Index);
            }
            foreach (var ele in deletingReservations)
            {
                va.RemoveWalkableCell(ele);
                var ledge = va.GetCell(ele);
                if (ledge != null)
                {
                    va.AddLedge(ledge);
                    ledge.CellType = VOXEL_NAVI_TYPE.Ledge;
                }
            }
        }
        /// <summary>
        /// 낮은 천장을 걸러낸다.
        /// </summary>
        private void FilterLowCeilingPlace()
        {
            var va = VoxelArea;
            List<uint> deletingReservations = new List<uint>();
            uint ceilingHeightCount = (uint)Mathf.CeilToInt(va.AgentHeight / va.CellHeight);
            foreach (var ele in va.WalkableCells)
            {
                bool findLowCeiling = false;
                for(uint i = 0, index = ele.Index; i < ceilingHeightCount; ++i)
                {
                    var upperCell = va.GetCell(VoxelArea.DIRECTION.Upper, index);
                    if (upperCell == null) break;
                    if (upperCell.CellType == VOXEL_NAVI_TYPE.Walkable)
                    {
                        findLowCeiling = true;
                        break;
                    }
                    index = upperCell.Index;
                }

                if( findLowCeiling )
                    deletingReservations.Add(ele.Index);
            }
            foreach (var ele in deletingReservations)
            {
                va.RemoveWalkableCell(ele);
                var ledge = va.GetCell(ele);
                if (ledge != null)
                {
                    va.AddFirstLedge(ledge);
                    ledge.CellType = VOXEL_NAVI_TYPE.FirstLedge;
                }
            }

        }

        /// <summary>
        /// Climb 수치를 참조하여 쎌간에 연결을 만든다. 오직 Walkable 쎌만 연결한다.
        /// </summary>
        private void BuildConnection()
        {
            var va = VoxelArea;
            uint climbableCellCount = (uint)Mathf.CeilToInt(va.MaxClimb / va.CellHeight);

            foreach (var ele in VoxelArea.WalkableCells)
            {
                Vector3 curPos;
                va.GetCellPosition(ele.Index, out curPos);

                Voxel tmp;
                if ((tmp = va.GetCell(VoxelArea.DIRECTION.Left, ele.Index)) != null)
                {
                    if (tmp.CellType == VOXEL_NAVI_TYPE.Walkable)
                    {
                        if (tmp.TopSpan)
                            ele.ConnectionLeft = true;
                        else
                        {
                            if (va.GetTopSpanCell(tmp.Index, climbableCellCount) == null)
                                ele.ConnectionLeft = false;
                            else
                                ele.ConnectionLeft = true;
                        }
                    }
                    else
                    {
                        var heigherCell = va.GetTopSpanCell(tmp.Index, climbableCellCount);
                        var lowerCell = va.GetBottomSpanCell(tmp.Index, climbableCellCount);

                        if (heigherCell != null || lowerCell != null)
                            ele.ConnectionLeft = true;
                        else
                            ele.ConnectionLeft = false;
                    }
                }
                else ele.ConnectionLeft = false;

                if( (tmp = va.GetCell(VoxelArea.DIRECTION.Right, ele.Index)) != null)
                {
                    if (tmp.CellType == VOXEL_NAVI_TYPE.Walkable)
                    {
                        if (tmp.TopSpan)
                            ele.ConnectionRight = true;
                        else
                        {
                            if (va.GetTopSpanCell(tmp.Index, climbableCellCount) == null)
                                ele.ConnectionRight = false;
                            else
                                ele.ConnectionRight = true;
                        }
                    }
                    else
                    {
                        var heigherCell = va.GetTopSpanCell(tmp.Index, climbableCellCount);
                        var lowerCell = va.GetBottomSpanCell(tmp.Index, climbableCellCount);

                        if (heigherCell != null || lowerCell != null)
                            ele.ConnectionRight = true;
                        else
                            ele.ConnectionRight = false;
                    }
                }
                else ele.ConnectionRight = false;

                if ( (tmp = va.GetCell(VoxelArea.DIRECTION.Forward, ele.Index)) != null)
                {
                    if (tmp.CellType == VOXEL_NAVI_TYPE.Walkable)
                    {
                        if (tmp.TopSpan)
                            ele.ConnectionForward = true;
                        else
                        {
                            if (va.GetTopSpanCell(tmp.Index, climbableCellCount) == null)
                                ele.ConnectionForward = false;
                            else
                                ele.ConnectionForward = true;
                        }
                    }
                    else
                    {
                        var heigherCell = va.GetTopSpanCell(tmp.Index, climbableCellCount);
                        var lowerCell = va.GetBottomSpanCell(tmp.Index, climbableCellCount);

                        if (heigherCell != null || lowerCell != null)
                            ele.ConnectionForward = true;
                        else
                            ele.ConnectionForward = false;
                    }
                }
                else ele.ConnectionForward = false;

                if ( (tmp = va.GetCell(VoxelArea.DIRECTION.Backward, ele.Index)) != null)
                {
                    if (tmp.CellType == VOXEL_NAVI_TYPE.Walkable)
                    {
                        if (tmp.TopSpan)
                            ele.ConnectionBackward = true;
                        else
                        {
                            if (va.GetTopSpanCell(tmp.Index, climbableCellCount) == null)
                                ele.ConnectionBackward = false;
                            else
                                ele.ConnectionBackward = true;
                        }
                    }
                    else
                    {
                        var heigherCell = va.GetTopSpanCell(tmp.Index, climbableCellCount);
                        var lowerCell = va.GetBottomSpanCell(tmp.Index, climbableCellCount);

                        if (heigherCell != null || lowerCell != null)
                            ele.ConnectionBackward = true;
                        else
                            ele.ConnectionBackward = false;
                    }
                }
                else ele.ConnectionBackward = false;
            }
        }

        private void BuildDistanceField()
        {
            var va = VoxelArea;
            var wc = VoxelArea.WalkableCells;
            foreach(var ele in wc)
            {
                
            }
        }

        private void RegistExceptVoxel(SimpleMesh mesh)
        {
            VoxelArea va = VoxelArea;

            for (int i = 0; i < mesh._triangles.Length; i += 3)
            {
                int min_x, min_y, min_z, max_x, max_y, max_z;

                Vector3 tri1 = mesh._matrix.MultiplyPoint3x4(mesh._vertices[mesh._triangles[i]]);
                Vector3 tri2 = mesh._matrix.MultiplyPoint3x4(mesh._vertices[mesh._triangles[i + 1]]);
                Vector3 tri3 = mesh._matrix.MultiplyPoint3x4(mesh._vertices[mesh._triangles[i + 2]]);

                Vector3 minPos = Vector3.Min(Vector3.Min(tri1, tri2), tri3);
                Vector3 maxPos = Vector3.Max(Vector3.Max(tri1, tri2), tri3);

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
                            if (cell == null || cell.CellType == VOXEL_NAVI_TYPE.Excepted) continue;

                            Vector3 position, size;
                            va.GetCellPosition(cell.Index, out position);
                            size = new Vector3(va.CellSize, va.CellHeight, va.CellSize);
                            AABox box = new AABox(position, size);
                            Triangle triangle = new Triangle(tri1, tri2, tri3);
                            bool notUsed;
                            if (Collision3D.CollisionDetectTriangle(triangle, box, out notUsed))
                                cell.CellType = VOXEL_NAVI_TYPE.Excepted;
                        }
                    }
                }
            }
        }

        private void VoxelizeObject(SimpleMesh mesh)
        {
            VoxelArea va = VoxelArea;

            for (int i = 0; i < mesh._triangles.Length; i+=3)
            {
                int min_x, min_y, min_z, max_x, max_y, max_z;

                Vector3 tri1 = mesh._matrix.MultiplyPoint3x4(mesh._vertices[mesh._triangles[i]]);
                Vector3 tri2 = mesh._matrix.MultiplyPoint3x4(mesh._vertices[mesh._triangles[i+1]]);
                Vector3 tri3 = mesh._matrix.MultiplyPoint3x4(mesh._vertices[mesh._triangles[i+2]]);

                Vector3 minPos = Vector3.Min(Vector3.Min(tri1, tri2), tri3);
                Vector3 maxPos = Vector3.Max(Vector3.Max(tri1, tri2), tri3);

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
                            if (cell == null || cell.CellType != VOXEL_NAVI_TYPE.None) continue;

                            Vector3 position, size;
                            va.GetCellPosition(cell.Index, out position);
                            size = new Vector3(va.CellSize, va.CellHeight, va.CellSize);
                            AABox box = new AABox(position, size);
                            Triangle triangle = new Triangle(tri1, tri2, tri3);
                            //if(Collision3D.CollisionDetectTriangle(triangle, box, Vector3.up))
                            bool reverseSide;
                            if (Collision3D.CollisionDetectTriangle(triangle, box, out reverseSide, Vector3.up))
                            {
                                if (reverseSide)    va.SetWalkableCell(cell, true);
                                else                va.SetWalkableCell(cell);
                            }
                                
                        }
                    }
                }
            }
        }
    }

    public enum VOXEL_FACE
    {
        Normar = 0,
        Reverse,
    }

    public enum VOXEL_NAVI_TYPE
    {
        None = 0,   // 암것도 아님.
        ReverseSide,   // 반대면.
        Excepted,   // 제외된 쎌.
        FirstLedge, // 첫 외곽. 무조껀 제외.
        Ledge,      // 외곽. 플레이어의 너비에 따라 달라지는 외곽.
        Walkable,   // ...
    }

    public class Voxel
    {
        private uint _index;
        private VOXEL_NAVI_TYPE _cellType = VOXEL_NAVI_TYPE.None;
        public VOXEL_NAVI_TYPE CellType { set { _cellType = value; } get { return _cellType; } }

        private int _connectinon = 0;

        public bool TopSpan
        {
            get { return (_connectinon & 16) > 0 ? true : false; }
            set { _connectinon = value ? _connectinon | 16 : _connectinon & ~16; }
        }

        public bool ConnectionLeft
        {
            get { return (_connectinon & 1) > 0 ? true : false; }
            set { _connectinon = value ? _connectinon | 1 : _connectinon & ~1; }
        }
        public bool ConnectionRight
        {
            get { return (_connectinon & 2) > 0 ? true : false; }
            set { _connectinon = value ? _connectinon | 2 : _connectinon & ~2; }
        }
        public bool ConnectionForward
        {
            get { return (_connectinon & 4) > 0 ? true : false; }
            set { _connectinon = value ? _connectinon | 4 : _connectinon & ~4; }
        }
        public bool ConnectionBackward
        {
            get { return (_connectinon & 8) > 0 ? true : false; }
            set { _connectinon = value ? _connectinon | 8 : _connectinon & ~8; }
        }

        public float distance = 0f;

        public uint Index { get { return _index; } }

        public Voxel(uint index) { _index = index; }
    }
}