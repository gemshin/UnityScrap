using UnityEngine;
using System.Collections.Generic;
using ZKit.Math.Geometry;

namespace ZKit.PathFinder
{
    public class Voxel
    {
        public bool _ex = false;    // 제외된 쎌을 설정한다.인데... 

        public uint Index { get { return _index; } }
        private uint _index;        // 인덱스.

        public bool FaceReverse { get { return _reverse; } set { _reverse = value; } }
        private bool _reverse = false;

        public bool IsSpan { get { return _span; } set { _span = value; } }
        private bool _span = false;

        public bool IsTop { get { return (spanTop == _index); } }
        public bool IsBottom { get { return (spanNext == _index); } }

        public uint SpanTop { get { return spanTop; } set { spanTop = value; } }
        private uint spanTop;

        public uint SpanNext { get { return spanNext; } set { spanNext = value; } }
        private uint spanNext;

        public Voxel(uint index) { _index = index; spanTop = spanNext = index; }
    }
}











//public class VoxelArea__ // Voxel 공간 데이터.
//{
//    public enum DIRECTION
//    {
//        Left,
//        Right,
//        Forward,
//        Backward,
//        Upper,
//        Lower,
//    }

//    private Bounds _bound;
//    private float _cellSize;    // Voxel 길이와 너비
//    private float _cellHeight;  // Voxel 높이
//    private uint _widthCount;   // Voxel 너비 갯수 (x Axis)
//    private uint _depthCount;   // Voxel 깊이 갯수 (z Axis)
//    private uint _heightCount;  // Voxel 높이 갯수 (y Axis)
//    private float _agentRadius; // 플레이어 반지름
//    private float _agentHeight; // 플레이어 높이
//    private float _maxSlope;    // 오를수 있는 경사도
//    private float _maxClimb;    // 오를수 있는 높이

//    public float CellSize { get { return _cellSize; } }
//    public float CellHeight { get { return _cellHeight; } }
//    public Bounds AreaBound { get { return _bound; } }
//    public uint WidthCount { get { return _widthCount; } }
//    public uint HeightCount { get { return _heightCount; } }
//    public uint DepthCount { get { return _depthCount; } }
//    public float AgentHeight { get { return _agentHeight; } }
//    public float AgentRadius { get { return _agentRadius; } }
//    public float MaxSlope { get { return _maxSlope; } }
//    public float MaxClimb { get { return _maxClimb; } }

//    private SortedList<uint, Voxel__> _voxel = new SortedList<uint, Voxel__>();
//    public SortedList<uint, Voxel__> Voxel
//    {
//        get { return _voxel; }
//    }

//    private Voxel__[] _voxelCells;    // Voxels..
//    public Voxel__[] Voxels { get { return _voxelCells; } }
//    public IList<Voxel__> WalkableCells { get { return _walkableCells.Values; } }
//    private SortedList<uint, Voxel__> _walkableCells = new SortedList<uint, Voxel__>();
//    public IList<Voxel__> FirstLedges { get { return _firstLedges.Values; } }
//    private SortedList<uint, Voxel__> _firstLedges = new SortedList<uint, Voxel__>();
//    public void AddFirstLedge(Voxel__ firstLedge) { _firstLedges.Add(firstLedge.Index, firstLedge); }
//    public IList<Voxel__> Ledges { get { return _ledges.Values; } }
//    private SortedList<uint, Voxel__> _ledges = new SortedList<uint, Voxel__>();
//    public void AddLedge(Voxel__ ledge) { _ledges.Add(ledge.Index, ledge); }

//    public VoxelArea__(Bounds areaBound, float cellSize, float cellHeight)
//    {
//        _cellSize = cellSize;
//        _cellHeight = cellHeight;
//        _bound = areaBound;

//        _widthCount = (uint)Mathf.Ceil(_bound.size.x / _cellSize);
//        _depthCount = (uint)Mathf.Ceil(_bound.size.z / _cellSize);
//        _heightCount = (uint)Mathf.Ceil(_bound.size.y / _cellHeight);

//        _bound.max = new Vector3(_bound.min.x + (_widthCount*_cellSize)
//                                , _bound.min.y + (_heightCount * _cellHeight)
//                                , _bound.min.z + (_depthCount * _cellSize));
//        _voxelCells = new Voxel__[_widthCount * _heightCount * _depthCount];

//        for (uint i = 0; i < _voxelCells.Length; ++i)
//            _voxelCells[i] = new Voxel__(i);
//    }
//    /// <summary>
//    /// Agent 정보를 입력한다. VoxelArea 생성시 호출되어야 하기 때문에 나중에 구조조정이 필요.  
//    /// </summary>
//    /// <param name="agentHeight"></param>
//    /// <param name="agentRadius"></param>
//    /// <param name="maxClimb"></param>
//    /// <param name="maxSlope"></param>
//    public void SetAgentInfo(float agentHeight, float agentRadius, float maxClimb, float maxSlope)
//    {
//        _agentHeight = agentHeight;
//        _agentRadius = agentRadius;
//        _maxClimb = maxClimb;
//        _maxSlope = maxSlope;
//    }
//    /// <summary>
//    /// 연결된 쎌의 꼭대기 쎌을 가져온다.
//    /// </summary>
//    /// <param name="index">기준 쎌</param>
//    /// <param name="max">검사를 수행할 수</param>
//    /// <returns>꼭대기 쎌, 아니면 null</returns>
//    public Voxel__ GetTopSpanCell(uint index, uint max = 1024)
//    {
//        uint x, y, z;
//        if (!GetCellCount(index, out x, out y, out z)) return null;
//        Voxel__ tmp;
//        for(uint i = 1; i < max; ++i)
//        {
//            if ((tmp = GetCell(x, y + i, z)) == null) return null;
//            if (tmp.TopSpan) return tmp;
//        }
//        return null;
//    }
//    /// <summary>
//    /// 연결된 쎌의 바닥 쎌을 가져온다.
//    /// </summary>
//    /// <param name="index">기준 쎌</param>
//    /// <param name="max">검사를 수행할 수</param>
//    /// <returns>바닥 쎌, 아니면 null</returns>
//    public Voxel__ GetBottomSpanCell(uint index, uint max = 1024)
//    {
//        uint x, y, z;
//        if (!GetCellCount(index, out x, out y, out z)) return null;

//        Voxel__ tmp;
//        for (uint i = 1; i < max; ++i)
//        {
//            if ((tmp = GetCell(x, y - i, z)) == null) return null;
//            if (tmp.TopSpan) return tmp;
//        }
//        return null;
//    }
//    /// <summary>
//    /// 기준 쎌로부터 지정된 방향의 쎌을 가져온다.
//    /// </summary>
//    /// <param name="direction">방향</param>
//    /// <param name="index">기준 쎌 인덱스</param>
//    /// <returns>지정된 방향의 쎌, 아니면 null</returns>
//    public Voxel__ GetCell(DIRECTION direction, uint index)
//    {
//        uint x, y, z;
//        if (!GetCellCount(index, out x, out y, out z)) return null;
//        switch(direction)
//        {
//            case DIRECTION.Left:
//                ++x;
//                break;
//            case DIRECTION.Right:
//                --x;
//                break;
//            case DIRECTION.Forward:
//                ++z;
//                break;
//            case DIRECTION.Backward:
//                --z;
//                break;
//            case DIRECTION.Upper:
//                ++y;
//                break;
//            case DIRECTION.Lower:
//                --y;
//                break;
//        }
//        return GetCell(x, y, z);
//    }
//    /// <summary>
//    /// Get Cell
//    /// </summary>
//    /// <param name="x">Horizontal count</param>
//    /// <param name="y">Vertical count</param>
//    /// <param name="z">Depth count</param>
//    /// <returns>VoxelCell or null</returns>
//    public Voxel__ GetCell(uint x, uint y, uint z)
//    {
//        uint index;
//        if (!GetCellIndex(out index, x, y, z)) return null;
//        return _voxelCells[index];
//    }
//    /// <summary>
//    /// Get Cell
//    /// </summary>
//    /// <param name="index">index</param>
//    /// <returns>VoxelCell or null</returns>
//    public Voxel__ GetCell(uint index)
//    {
//        if (_voxelCells == null || index >= _voxelCells.Length) return null;
//        return _voxelCells[index];
//    }
//    /// <summary>
//    /// Get Cell
//    /// </summary>
//    /// <param name="point">Point position(world)</param>
//    /// <returns>VoxelCell or null</returns>
//    public Voxel__ GetCell(Vector3 point)
//    {
//        int x, y, z;
//        if (!GetCellCount(point, out x, out y, out z)) return null;
//        return GetCell((uint)x, (uint)y, (uint)z);
//    }
//    /// <summary>
//    /// Get Cell Index
//    /// </summary>
//    /// <param name="index">out Index</param>
//    /// <param name="x">Horizontal count</param>
//    /// <param name="y">Vertical count</param>
//    /// <param name="z">Depth count</param>
//    /// <returns>Success or Fail</returns>
//    public bool GetCellIndex(out uint index, uint x, uint y, uint z)
//    {
//        index = (x + (y * _widthCount) + (z * _widthCount * _heightCount));
//        if (_voxelCells == null || index >= _voxelCells.Length) return false;
//        return true;
//    }
//    /// <summary>
//    /// Get Cell Count
//    /// </summary>
//    /// <param name="index">Index</param>
//    /// <param name="x">out Horizontal count</param>
//    /// <param name="y">out Vertical count</param>
//    /// <param name="z">out Depth count</param>
//    /// <returns>Success or Fail</returns>
//    public bool GetCellCount(uint index, out uint x, out uint y, out uint z)
//    {
//        x = y = z = 0u;
//        if (index >= _voxelCells.Length) return false;

//        z = index / (_widthCount * _heightCount);
//        uint tmp = z * _widthCount * _heightCount;
//        y = (index - tmp) / _widthCount;
//        x = (index - tmp) % _widthCount;
//        return true;
//    }
//    /// <summary>
//    /// Get Cell Count
//    /// </summary>
//    /// <param name="point">Point position(world)</param>
//    /// <param name="x">out Horizontal count</param>
//    /// <param name="y">out Vertical count</param>
//    /// <param name="z">out Depth count</param>
//    /// <returns>Success or Fail</returns>
//    public bool GetCellCount(Vector3 point, out int x, out int y, out int z)
//    {
//        x = y = z = 0;
//        if (!_bound.Contains(point)) return false;

//        var areaPosition = point - _bound.min;
//        x = areaPosition.x == 0f ? 0 : (int)(areaPosition.x / _cellSize);
//        y = areaPosition.y == 0f ? 0 : (int)(areaPosition.y / _cellHeight);
//        z = areaPosition.z == 0f ? 0 : (int)(areaPosition.z / _cellSize);
//        if (x >= _widthCount || y >= _heightCount || z >= _depthCount) return false;
//        return true;
//    }
//    /// <summary>
//    /// Get Cell Center Position
//    /// </summary>
//    /// <param name="index">Cell Index</param>
//    /// <param name="position">out Cell Center Position</param>
//    /// <returns></returns>
//    public bool GetCellPosition(uint index, out Vector3 position)
//    {
//        uint x, y, z;
//        position = Vector3.zero;
//        if (!GetCellCount(index, out x, out y, out z)) return false;
//        position.x = x * _cellSize + _cellSize * 0.5f;
//        position.y = y * _cellHeight + _cellHeight * 0.5f;
//        position.z = z * _cellSize + _cellSize * 0.5f;
//        position = position + _bound.min;
//        position.x = Mathf.Round(position.x * 1000f) * 0.001f;
//        position.y = Mathf.Round(position.y * 1000f) * 0.001f;
//        position.z = Mathf.Round(position.z * 1000f) * 0.001f;
//        return true;
//    }

//    public void RemoveWalkableCell(uint index)
//    {
//        ////GetTopCell(index);
//        //if(_walkableCells[index].TopSpan)
//        //{
//        //    var lower = GetCell(DIRECTION.Lower, index);
//        //    if (lower != null && lower.CellType == VOXEL_TYPE.Walkable)
//        //        lower.TopSpan = true;
//        //}
//        //_walkableCells[index].CellType = VOXEL_TYPE.None;
//        _walkableCells.Remove(index);
//    }

//    public void SetWalkableCell(Voxel__ cell, bool reverseSide = false)
//    {
//        if (!reverseSide)
//            cell.CellType = VOXEL_NAVI_TYPE.Walkable;
//        else
//        {
//            cell.CellType = VOXEL_NAVI_TYPE.ReverseSide;
//            return;
//        }

//        cell.TopSpan = true;

//        Voxel__ upperCell = GetCell(DIRECTION.Upper, cell.Index);
//        if( upperCell != null )
//        {
//            if (upperCell.CellType == VOXEL_NAVI_TYPE.Walkable || upperCell.CellType == VOXEL_NAVI_TYPE.ReverseSide)
//            {
//                cell.TopSpan = false;
//            }
//        }
//        Voxel__ lowerCell = GetCell(DIRECTION.Lower, cell.Index);
//        if (lowerCell != null)
//        {
//            if ((lowerCell.CellType == VOXEL_NAVI_TYPE.Walkable || lowerCell.CellType == VOXEL_NAVI_TYPE.ReverseSide) && lowerCell.TopSpan)
//            {
//                _walkableCells.Remove(lowerCell.Index);
//                lowerCell.TopSpan = false;
//            }
//        }

//        //if (cell.TopSpan && !reverseSide) _walkableCells.Add(cell.Index, cell); // top만 등록.
//        if (cell.TopSpan) _walkableCells.Add(cell.Index, cell); // top만 등록.
//    }
//}

//public enum VOXEL_NAVI_TYPE
//{
//    None = 0,   // 암것도 아님.
//    ReverseSide,   // 반대면.
//    Excepted,   // 제외된 쎌.
//    FirstLedge, // 첫 외곽. 무조껀 제외.
//    Ledge,      // 외곽. 플레이어의 너비에 따라 달라지는 외곽.
//    Walkable,   // ...
//}

//public class Voxel__
//{
//    private uint _index;
//    private VOXEL_FACE _faceType = VOXEL_FACE.Normar;
//    public VOXEL_FACE FaceType { get { return _faceType; } set { _faceType = value; } }

//    private VOXEL_NAVI_TYPE _cellType = VOXEL_NAVI_TYPE.None;
//    public VOXEL_NAVI_TYPE CellType { set { _cellType = value; } get { return _cellType; } }

//    private int _connectinon = 0;

//    public bool TopSpan
//    {
//        get { return (_connectinon & 16) > 0 ? true : false; }
//        set { _connectinon = value ? _connectinon | 16 : _connectinon & ~16; }
//    }

//    public bool ConnectionLeft
//    {
//        get { return (_connectinon & 1) > 0 ? true : false; }
//        set { _connectinon = value ? _connectinon | 1 : _connectinon & ~1; }
//    }
//    public bool ConnectionRight
//    {
//        get { return (_connectinon & 2) > 0 ? true : false; }
//        set { _connectinon = value ? _connectinon | 2 : _connectinon & ~2; }
//    }
//    public bool ConnectionForward
//    {
//        get { return (_connectinon & 4) > 0 ? true : false; }
//        set { _connectinon = value ? _connectinon | 4 : _connectinon & ~4; }
//    }
//    public bool ConnectionBackward
//    {
//        get { return (_connectinon & 8) > 0 ? true : false; }
//        set { _connectinon = value ? _connectinon | 8 : _connectinon & ~8; }
//    }

//    public float distance = 0f;

//    public uint Index { get { return _index; } }

//    public Voxel__(uint index) { _index = index; }
//}