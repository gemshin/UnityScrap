
using UnityEngine;
using System.Collections.Generic;

namespace ZKit.PathFinder
{
    public class RecastNaviMesh : NonPubSingleton<RecastNaviMesh>
    {
        public int _maxDistance = 0;

        private AgentInfo _agentInfo;
        private Voxelizer _voxelizer;
        public List<WalkableCell> _firstLedge = new List<WalkableCell>();
        public SortedList<uint, WalkableCell> _walkableCells = new SortedList<uint, WalkableCell>();
        public IList<WalkableCell> GetWalkable()
        {
            return _walkableCells.Values;
        }

        public void SetAgentInfo(AgentInfo info)
        {
            _agentInfo = info;
        }

        public Bounds GetMapSize()
        {
            if (_voxelizer == null) return new Bounds();
            return _voxelizer.VoxelArea.AreaBound;
        }

        public VoxelArea GetArea()
        {
            if(_voxelizer == null) return null;
            return _voxelizer.VoxelArea;
        }

        public void Build(float cellSize, float cellHeight, int pathLayerMask, int exceptLayerMask )
        {
            _maxDistance = 0;
            _firstLedge.Clear();
            _walkableCells.Clear();
            _voxelizer = new Voxelizer();

            _voxelizer.InitVoxelArea(cellSize, cellHeight, pathLayerMask, exceptLayerMask);
            _voxelizer.ScanVoxelSpace();

            foreach(var ele in _voxelizer.VoxelArea.Voxels.Values)
            {
                if (!ele.IsTop || ele.FaceReverse) continue;
                WalkableCell wc = new WalkableCell(ele.Index);
                _walkableCells.Add(ele.Index, wc);
            }

            FilterLowCeilingPlace();

            CalcDistanceField();

            FilterFirstLedge();

            FilterLedge();

            BuildContours();
        }

        /// <summary>
        /// 낮은 천장을 걸러낸다.
        /// </summary>
        private void FilterLowCeilingPlace()
        {
            var va = _voxelizer.VoxelArea;
            List<uint> deletingReservations = new List<uint>();

            uint ceilingHeightCount = (uint)Mathf.CeilToInt(_agentInfo.AgentHeight / va.CellHeight);
            foreach(var ele in _walkableCells.Values)
            {
                for (uint i = 1; i <= ceilingHeightCount; ++i)
                {
                    uint x, y, z;
                    if (!va.GetCellCount(ele.Index, out x, out y, out z)) break;
                    if ( va.GetCell(x, y + i, z) == null) continue;
                    deletingReservations.Add(ele.Index);
                }
            }
            foreach(var ele in deletingReservations)
            {
                _walkableCells.Remove(ele);
            }
        }

        /// <summary>
        /// 쎌 외곽선 1줄을 걸러낸다.
        /// </summary>
        private void FilterFirstLedge()
        {
            //BuildConnection();
            var va = _voxelizer.VoxelArea;
            List<WalkableCell> removeReserve = new List<WalkableCell>();
            foreach (var ele in _walkableCells.Values)
            {
                if(ele.Distance == 0)
                {
                    ele.FirstLegde = true;
                    removeReserve.Add(ele);
                    _firstLedge.Add(_walkableCells[ele.Index]);
                }
            }
            foreach(var ele in removeReserve)
                _walkableCells.Remove(ele.Index);
        }

        private void FilterLedge()
        {
            //root2 = 1.41421f
            var va = _voxelizer.VoxelArea;
            uint radiusCellCount = (uint)Mathf.CeilToInt(_agentInfo.AgentRadius / va.CellSize);
            //uint radiusDiagCellCount = (uint)Mathf.CeilToInt(_agentInfo.AgentRadius / (1.41421f * va.CellSize));
            //uint climbableCellCount = (uint)Mathf.CeilToInt(_agentInfo.MaxClimb / va.CellHeight);

            foreach(var ele in _walkableCells.Values)
            {
                if (ele.Distance <= radiusCellCount)
                    ele.Legde = true;
            }
        }

        private void CalcDistanceField()
        {
            var va = _voxelizer.VoxelArea;
            uint climbableCellCount = (uint)Mathf.CeilToInt(_agentInfo.MaxClimb / va.CellHeight);

            foreach (var ele in _walkableCells.Values)
            {
                Voxel[] eightDirVox = new Voxel[8];

                eightDirVox[0] = va.GetCell(VoxelArea.DIRECTION.Left, ele.Index);       // left
                eightDirVox[1] = va.GetCell(VoxelArea.DIRECTION.Right, ele.Index);      // right
                eightDirVox[2] = va.GetCell(VoxelArea.DIRECTION.Forward, ele.Index);    // forward
                eightDirVox[3] = va.GetCell(VoxelArea.DIRECTION.Backward, ele.Index);   // backward
                eightDirVox[4] = va.GetCell(VoxelArea.DIRECTION.LeftForward, ele.Index);    // LeftForward
                eightDirVox[5] = va.GetCell(VoxelArea.DIRECTION.RightForward, ele.Index);   // RightForward
                eightDirVox[6] = va.GetCell(VoxelArea.DIRECTION.LeftBackward, ele.Index);   // LeftBackward
                eightDirVox[7] = va.GetCell(VoxelArea.DIRECTION.RightBackward, ele.Index);  // RightBackward

                for (int i = 0; i < 8; ++i)
                {
                    // build connection
                    if (eightDirVox[i] != null && _walkableCells.ContainsKey(eightDirVox[i].Index))
                    {
                        ele.ConnectedID(EnumHelper.Parse<VoxelArea.DIRECTION, int>(i), eightDirVox[i].Index);
                    }
                    else
                    {
                        Voxel tmp;
                        uint x, y, z;
                        if (!va.GetCellCount(ele.Index, out x, out y, out z)) break;

                        if (i == 0) { --x; }
                        if (i == 1) { ++x; }
                        if (i == 2) { ++z; }
                        if (i == 3) { --z; }
                        if (i == 4) { --x; ++z; }
                        if (i == 5) { ++x; ++z; }
                        if (i == 6) { --x; --z; }
                        if (i == 7) { ++x; --z; }

                        for (uint j = 1; j <= climbableCellCount; ++j)
                        {
                            if ((tmp = va.GetCell(x, y - j, z)) != null && _walkableCells.ContainsKey(tmp.Index))
                            {
                                ele.ConnectedID(EnumHelper.Parse<VoxelArea.DIRECTION, int>(i), tmp.Index);  break;
                            }
                            if ((tmp = va.GetCell(x, y + j, z)) != null && _walkableCells.ContainsKey(tmp.Index))
                            {
                                ele.ConnectedID(EnumHelper.Parse<VoxelArea.DIRECTION, int>(i), tmp.Index);  break;
                            }
                        }
                    }
                }

                // forward tracing
                if ( ele.ConnectedID(VoxelArea.DIRECTION.Left)== null
                    || ele.ConnectedID(VoxelArea.DIRECTION.Right) == null
                    || ele.ConnectedID(VoxelArea.DIRECTION.Forward) == null
                    || ele.ConnectedID(VoxelArea.DIRECTION.Backward) == null )
                    ele.Distance = 0;
                else
                {
                    int minDist = int.MaxValue;

                    for (int i = 0; i < 8; ++i)
                    {
                        uint? index = ele.ConnectedID(EnumHelper.Parse<VoxelArea.DIRECTION, int>(i));
                        if(index != null)
                        {
                           if (/*_walkableCells.ContainsKey(index) &&*/ _walkableCells[index.Value].Distance != null)
                            {
                                minDist = Mathf.Min(_walkableCells[index.Value].Distance.Value, minDist);
                                _maxDistance = Mathf.Max(_walkableCells[index.Value].Distance.Value, _maxDistance);
                            }
                        }
                    }
                    if (minDist == int.MaxValue) minDist = 0;
                    ele.Distance = minDist + 1;
                }
            }

            // reverse tracing
            for(int j = _walkableCells.Count- 1; j >= 0; --j)
            {
                var ele = _walkableCells.Values[j];
                if (ele.Distance == 0u) continue;

                int minDist = int.MaxValue;

                for (int i = 0; i < 8; ++i)
                {
                    uint? index = ele.ConnectedID(EnumHelper.Parse<VoxelArea.DIRECTION, int>(i));
                    if (index != null)
                    {
                        if (/*_walkableCells.ContainsKey(index) &&*/ _walkableCells[index.Value].Distance != null)
                        {
                            minDist = Mathf.Min(_walkableCells[index.Value].Distance.Value, minDist);
                            _maxDistance = Mathf.Max(_walkableCells[index.Value].Distance.Value, _maxDistance);
                        }
                    }
                }
                if (minDist == int.MaxValue) minDist = 0;
                ele.Distance = minDist + 1;
            }
        }

        //private void Partitioning()
        //{
        //    int currentDist = _maxDistance;
        //    int regionID = 0;

        //    foreach (var ele in _walkableCells.Values)
        //    {
        //        if (ele.RegionID == null && ele.Distance == currentDist)
        //        {
        //            WaterShed(ele.Index, currentDist, regionID++);
        //        }
        //    }
        //}

        //private void WaterShed(uint index, int distance, int regionID)
        //{
        //    var cell = _walkableCells[index];
        //    if (cell.Distance == distance && cell.RegionID == null) // 해당 쎌이다.
        //}

        public List<uint> _exceptContours = new List<uint>();
        public List<List<Vector3>> _contours = new List<List<Vector3>>();

        private void Contours(ref List<Vector3> contours, uint first, uint index, bool? clockwise = null, VoxelArea.DIRECTION from = VoxelArea.DIRECTION.End)
        {
            var va = _voxelizer.VoxelArea;
            var cell = _walkableCells[index];
            float halfSize = va.CellSize * 0.5f;
            uint radiusCellCount = (uint)Mathf.CeilToInt(_agentInfo.AgentRadius / va.CellSize);
            Vector3 cellPos; va.GetCellPosition(index, out cellPos);

            VoxelArea.DIRECTION next = VoxelArea.DIRECTION.End;
            VoxelArea.DIRECTION prev = VoxelArea.DIRECTION.End;

            bool bStart = false, bFind = false, bEnd = false;

            uint? connectedIndex = null;

            #region start
            if (first == index && clockwise == null)
            {
                bStart = true;
                connectedIndex = cell.ConnectedID(VoxelArea.DIRECTION.Backward);
                if (connectedIndex.HasValue)
                {
                    if (_walkableCells[connectedIndex.Value] == null) return;
                    else if (_walkableCells[connectedIndex.Value].Distance == radiusCellCount)
                    {
                        clockwise = false;
                        Vector3 tmp = cellPos;
                        tmp.x -= halfSize;
                        tmp.z -= halfSize;
                        contours.Add(tmp);
                    }
                }

                if (!clockwise.HasValue)
                {
                    connectedIndex = cell.ConnectedID(VoxelArea.DIRECTION.Forward);
                    if (connectedIndex.HasValue)
                    {
                        if (_walkableCells[connectedIndex.Value] == null) return;
                        else if (_walkableCells[connectedIndex.Value].Distance == radiusCellCount)
                        {
                            clockwise = true;
                            Vector3 tmp = cellPos;
                            tmp.x -= halfSize;
                            tmp.z += halfSize;
                            contours.Add(tmp);
                        }
                    }
                }
            }
            if (!clockwise.HasValue) // solo or non
            {
                // non 시작시 non은 무조껀 우측으로 가는 상황만 걸리는듯.
                connectedIndex = cell.ConnectedID(VoxelArea.DIRECTION.Right);
                if (connectedIndex.HasValue)
                {
                    if (_walkableCells[connectedIndex.Value] == null) return;
                    else if (_walkableCells[connectedIndex.Value].Distance == radiusCellCount + 1)
                    {
                        // non 다음으로 패스...
                        Vector3 tmp_ = cellPos;
                        tmp_.x += halfSize;
                        tmp_.z += halfSize;
                        contours.Add(tmp_);
                        Contours(ref contours, first, connectedIndex.Value, clockwise = true, VoxelArea.DIRECTION.Left);
                        return;
                    }
                }
                // solo
                Vector3 tmp = cellPos;
                tmp.x -= halfSize;
                tmp.z -= halfSize;
                contours.Add(tmp);
                clockwise = false;
            }
            #endregion

            int count = 0;
            for (count = 0; count < 4; ++count)
            {
                if (bFind && bEnd) break;
                int j = count; // Left or End(first)
                if (clockwise.Value) // clockwise  // 0 = for, 1 = right, 2 = back, 3 = left
                {
                    if (from == VoxelArea.DIRECTION.Left || from == VoxelArea.DIRECTION.End) j = count; 
                    else if (from == VoxelArea.DIRECTION.Forward) j = count + 1; 
                    else if (from == VoxelArea.DIRECTION.Right) j = count + 2;
                    else if (from == VoxelArea.DIRECTION.Backward) j = count + 3;
                }
                else
                {
                    if (from == VoxelArea.DIRECTION.Left || from == VoxelArea.DIRECTION.End) j = count;
                    else if (from == VoxelArea.DIRECTION.Backward) j = count + 1;
                    else if (from == VoxelArea.DIRECTION.Right) j = count + 2;
                    else if (from == VoxelArea.DIRECTION.Forward) j = count + 3;
                }
                if (j > 3) j -= 4;
                VoxelArea.DIRECTION connected = VoxelArea.DIRECTION.End;
                if (clockwise.Value) // clockwise  // 0 = for, 1 = right, 2 = back, 3 = left
                {
                    if( j == 0) connected = VoxelArea.DIRECTION.Forward;
                    else if (j == 1) connected = VoxelArea.DIRECTION.Right;
                    else if (j == 2) connected = VoxelArea.DIRECTION.Backward;
                    else if (j == 3) connected = VoxelArea.DIRECTION.Left;
                }
                else // counterclockwise  // 0 = back, 1 = right, 2 = for, 3 = left
                {
                    if (j == 0) connected = VoxelArea.DIRECTION.Backward;
                    else if (j == 1) connected = VoxelArea.DIRECTION.Right;
                    else if (j == 2) connected = VoxelArea.DIRECTION.Forward;
                    else if (j == 3) connected = VoxelArea.DIRECTION.Left;
                }
                connectedIndex = cell.ConnectedID(connected);
                if (connectedIndex.HasValue)
                {
                    if (!_walkableCells.ContainsKey(connectedIndex.Value)) return;
                    else if (_walkableCells[connectedIndex.Value].Distance == radiusCellCount)
                    {
                        bFind = true;
                        Vector3 tmp = cellPos;
                        if (clockwise.Value)
                        {
                            if (connected == VoxelArea.DIRECTION.Left || connected == VoxelArea.DIRECTION.Backward)
                                tmp.x -= halfSize;
                            else
                                tmp.x += halfSize;
                            if (connected == VoxelArea.DIRECTION.Left || connected == VoxelArea.DIRECTION.Forward)
                                tmp.z += halfSize;
                            else
                                tmp.z -= halfSize;

                            switch (connected)
                            {
                                case VoxelArea.DIRECTION.Left:
                                    next = VoxelArea.DIRECTION.Forward;
                                    break;
                                case VoxelArea.DIRECTION.Forward:
                                    next = VoxelArea.DIRECTION.Right;
                                    break;
                                case VoxelArea.DIRECTION.Right:
                                    next = VoxelArea.DIRECTION.Backward;
                                    break;
                                case VoxelArea.DIRECTION.Backward:
                                    next = VoxelArea.DIRECTION.Left;
                                    break;
                            }
                        }
                        else
                        {
                            if (connected == VoxelArea.DIRECTION.Left || connected == VoxelArea.DIRECTION.Forward)
                                tmp.x -= halfSize;
                            else
                                tmp.x += halfSize;
                            if (connected == VoxelArea.DIRECTION.Right || connected == VoxelArea.DIRECTION.Forward)
                                tmp.z += halfSize;
                            else
                                tmp.z -= halfSize;

                            switch (connected)
                            {
                                case VoxelArea.DIRECTION.Left:
                                    next = VoxelArea.DIRECTION.Backward;
                                    break;
                                case VoxelArea.DIRECTION.Backward:
                                    next = VoxelArea.DIRECTION.Right;
                                    break;
                                case VoxelArea.DIRECTION.Right:
                                    next = VoxelArea.DIRECTION.Forward;
                                    break;
                                case VoxelArea.DIRECTION.Forward:
                                    next = VoxelArea.DIRECTION.Left;
                                    break;
                            }
                        }
                        contours.Add(tmp);
                    }
                    else if (_walkableCells[connectedIndex.Value].Distance == radiusCellCount + 1)
                    {
                        next = connected;
                        break;
                    }
                    else
                    {
                        bEnd = true;
                    }
                }
            }

            if ((first == index && !bStart) || (first == index && count >= 3))
            {
                _exceptContours.Add(index);
                return;
            }

            switch (next)
            {
                case VoxelArea.DIRECTION.Left:
                    prev = VoxelArea.DIRECTION.Right;
                    break;
                case VoxelArea.DIRECTION.Backward:
                    prev = VoxelArea.DIRECTION.Forward;
                    break;
                case VoxelArea.DIRECTION.Right:
                    prev = VoxelArea.DIRECTION.Left;
                    break;
                case VoxelArea.DIRECTION.Forward:
                    prev = VoxelArea.DIRECTION.Backward;
                    break;
            }

            _exceptContours.Add(index);

            var nextID = cell.ConnectedID(next);
            if (nextID == null) return;
            Contours(ref contours, first, nextID.Value, clockwise, prev);
        }

        private void Contours(uint first, uint index, ref List<Vector3> contours, VoxelArea.DIRECTION from = VoxelArea.DIRECTION.End)
        {
            if (!_walkableCells.ContainsKey(index)) return;

            var va = _voxelizer.VoxelArea;
            var cell = _walkableCells[index];
            if(cell == null)
            {
                Debug.Log("eeeee" + index);
                return;
            }
            float halfSize = va.CellSize * 0.5f;

            uint radiusCellCount = (uint)Mathf.CeilToInt(_agentInfo.AgentRadius / va.CellSize);

            Vector3 cellPos;
            va.GetCellPosition(index, out cellPos);

            VoxelArea.DIRECTION next = VoxelArea.DIRECTION.End;
            VoxelArea.DIRECTION prev = VoxelArea.DIRECTION.End;

            bool bFirst = false, bFind = false, bEnd = false;
            if (first == index) bFirst = true;
            int count = 0;

            uint? connectedIndex;

            for (int i = 0; i < 4; ++i)
            {
                if (bFind && bEnd) break;
                int j = i; // Left or End(first)
                if (from == VoxelArea.DIRECTION.Backward) j = i + 1;
                else if (from == VoxelArea.DIRECTION.Right) j = i + 2;
                else if (from == VoxelArea.DIRECTION.Forward) j = i + 3;
                if (j > 3) j -= 4;
                VoxelArea.DIRECTION connected = VoxelArea.DIRECTION.Backward;
                if (j == 1) connected = VoxelArea.DIRECTION.Right;
                else if (j == 2) connected = VoxelArea.DIRECTION.Forward;
                else if (j == 3) connected = VoxelArea.DIRECTION.Left;
                connectedIndex = cell.ConnectedID(connected);
                if (connectedIndex != null)
                {
                    if(_walkableCells[connectedIndex.Value] == null)
                    {
                        Debug.Log("errrrr " + connectedIndex);
                        return;
                    }
                    if (_walkableCells[connectedIndex.Value].Distance == radiusCellCount)
                    {
                        bFind = true;
                        ++count;
                        Vector3 tmp = cellPos;
                        if (bFirst)
                        {
                            if (connected == VoxelArea.DIRECTION.Left || connected == VoxelArea.DIRECTION.Backward)
                                tmp.x -= halfSize;
                            else
                                tmp.x += halfSize;
                            if (connected == VoxelArea.DIRECTION.Left || connected == VoxelArea.DIRECTION.Forward)
                                tmp.z += halfSize;
                            else
                                tmp.z -= halfSize;
                            contours.Add(tmp);
                            bFirst = false;
                        }
                        tmp = cellPos;
                        if (connected == VoxelArea.DIRECTION.Left || connected == VoxelArea.DIRECTION.Forward)
                            tmp.x -= halfSize;
                        else
                            tmp.x += halfSize;
                        if (connected == VoxelArea.DIRECTION.Right || connected == VoxelArea.DIRECTION.Forward)
                            tmp.z += halfSize;
                        else
                            tmp.z -= halfSize;
                        contours.Add(tmp);
                        switch (connected)
                        {
                            case VoxelArea.DIRECTION.Left:
                                next = VoxelArea.DIRECTION.Backward;
                                break;
                            case VoxelArea.DIRECTION.Backward:
                                next = VoxelArea.DIRECTION.Right;
                                break;
                            case VoxelArea.DIRECTION.Right:
                                next = VoxelArea.DIRECTION.Forward;
                                break;
                            case VoxelArea.DIRECTION.Forward:
                                next = VoxelArea.DIRECTION.Left;
                                break;
                        }
                    }
                    else if(_walkableCells[connectedIndex.Value].Distance == radiusCellCount+1)
                    {
                        next = connected;
                        break;
                    }
                    else
                    {
                        bEnd = true;
                    }
                }
                else
                {
                    //bEnd = true;
                }

                if (first == index && j == 3)
                {
                    _exceptContours.Add(index);
                    return;
                }
            }

            if (count >= 4) return;

            if (next == VoxelArea.DIRECTION.End)
            {
                switch (from)
                {
                    case VoxelArea.DIRECTION.Left:
                        next = VoxelArea.DIRECTION.Backward;
                        prev = VoxelArea.DIRECTION.Forward;
                        break;
                    case VoxelArea.DIRECTION.Backward:
                        next = VoxelArea.DIRECTION.Right;
                        prev = VoxelArea.DIRECTION.Left;
                        break;
                    case VoxelArea.DIRECTION.Right:
                        next = VoxelArea.DIRECTION.Forward;
                        prev = VoxelArea.DIRECTION.Backward;
                        break;
                    case VoxelArea.DIRECTION.Forward:
                        next = VoxelArea.DIRECTION.Left;
                        prev = VoxelArea.DIRECTION.Right;
                        break;
                    case VoxelArea.DIRECTION.End:
                        next = VoxelArea.DIRECTION.Right;
                        prev = VoxelArea.DIRECTION.Left;
                        break;
                }
            }
            else
            {
                switch (next)
                {
                    case VoxelArea.DIRECTION.Left:
                        prev = VoxelArea.DIRECTION.Right;
                        break;
                    case VoxelArea.DIRECTION.Backward:
                        prev = VoxelArea.DIRECTION.Forward;
                        break;
                    case VoxelArea.DIRECTION.Right:
                        prev = VoxelArea.DIRECTION.Left;
                        break;
                    case VoxelArea.DIRECTION.Forward:
                        prev = VoxelArea.DIRECTION.Backward;
                        break;
                }
            }

            _exceptContours.Add(index);

            var nextID = cell.ConnectedID(next);
            if (nextID == null) return;
            Contours(first, nextID.Value, ref contours, prev);
        }

        private void BuildContours()
        {
            _exceptContours.Clear();
            _contours.Clear();
            var va = _voxelizer.VoxelArea;

            uint radiusCellCount = (uint)Mathf.CeilToInt(_agentInfo.AgentRadius / va.CellSize);
            foreach (var ele in _walkableCells.Values)
            {
                if(ele.Distance == radiusCellCount + 1 && !_exceptContours.Contains(ele.Index))
                {
                    //if (ele.Index == 1213183)
                    if (ele.Index == 1215029)
                    {
                        int xxx = 0;
                    }
                    List<Vector3> contours = new List<Vector3>();
                    _contours.Add(contours);
                    Contours(ref contours, ele.Index, ele.Index);
                    Debug.Log(ele.Index);
                }
            }
        }
    }

    public class WalkableCell
    {
        public uint Index { get { return _index; } }
        private uint _index;
        
        private int _type = 0;

        private Dictionary<VoxelArea.DIRECTION, uint> _connection = new Dictionary<VoxelArea.DIRECTION, uint>();

        private int? _distance;
        public int? Distance { get { return _distance; } set { _distance = value; } }

        private int? _regionID;
        public int? RegionID { get { return _regionID; } set { _regionID = value; } }

        public bool Legde
        {
            get { return (_type & 0x200) > 0 ? true : false; }
            set { _type = value ? _type | 0x200 : _type & ~0x200; }
        }

        public bool FirstLegde
        {
            get { return (_type & 0x100) > 0 ? true : false; }
            set { _type = value ? _type | 0x100 : _type & ~0x100; }
        }

        public uint? ConnectedID(VoxelArea.DIRECTION dir)
        {
            if(_connection.ContainsKey(dir)) return _connection[dir];
            return null;
        }

        public void ConnectedID(VoxelArea.DIRECTION dir, uint value) { _connection[dir] = value; }

        public void RemoveConnectionID(VoxelArea.DIRECTION dir) { _connection.Remove(dir); }
        public void RemoveConnectionID() { _connection.Clear(); }

        public WalkableCell(uint index) { _index = index; }
    }
}