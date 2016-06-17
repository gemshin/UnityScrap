using System.Collections.Generic;
using UnityEngine;

namespace ZKit.PathFinder
{
    public class VoxelArea // new Voxel 공간 데이터.
    {
        public enum DIRECTION
        {
            Left,
            Right,
            Forward,
            Backward,
            LeftForward,
            RightForward,
            LeftBackward,
            RightBackward,
            Upper,
            Lower,
            End,
        }

        private Bounds _bound;
        private float _cellSize;    // Voxel 길이와 너비
        private float _cellHeight;  // Voxel 높이
        private uint _widthCount;   // Voxel 너비 갯수 (x Axis)
        private uint _depthCount;   // Voxel 깊이 갯수 (z Axis)
        private uint _heightCount;  // Voxel 높이 갯수 (y Axis)
        private uint _length;       // Voxel 최대 수

        public Bounds AreaBound { get { return _bound; } }
        public float CellSize { get { return _cellSize; } }
        public float CellHeight { get { return _cellHeight; } }
        public uint WidthCount { get { return _widthCount; } }
        public uint HeightCount { get { return _heightCount; } }
        public uint DepthCount { get { return _depthCount; } }
        public uint Length { get { return _length; } }

        private SortedList<uint, Voxel> _voxels = new SortedList<uint, Voxel>();
        public SortedList<uint, Voxel> Voxels { get { return _voxels; } }

        public VoxelArea(Bounds areaBound, float cellSize, float cellHeight)
        {
            _cellSize = cellSize;
            _cellHeight = cellHeight;
            _bound = areaBound;

            _widthCount = (uint)Mathf.Ceil(_bound.size.x / _cellSize);
            _depthCount = (uint)Mathf.Ceil(_bound.size.z / _cellSize);
            _heightCount = (uint)Mathf.Ceil(_bound.size.y / _cellHeight);
            _length = _widthCount * _depthCount * _heightCount;

            _bound.max = new Vector3(_bound.min.x + (_widthCount * _cellSize)
                                    , _bound.min.y + (_heightCount * _cellHeight)
                                    , _bound.min.z + (_depthCount * _cellSize));
        }

        /// <summary>
        /// 쎌을 찾아온다.
        /// </summary>
        /// <param name="direction">방향</param>
        /// <param name="index">기준점</param>
        /// <returns>원하는 쎌.</returns>
        public Voxel GetCell(DIRECTION direction, uint index)
        {
            uint x, y, z;
            if (!GetCellCount(index, out x, out y, out z)) return null;
            switch (direction)
            {
                case DIRECTION.Left: --x; break;
                case DIRECTION.Right: ++x; break;
                case DIRECTION.Forward: ++z; break;
                case DIRECTION.Backward: --z; break;
                case DIRECTION.LeftForward: --x; ++z; break;
                case DIRECTION.RightForward: ++x; ++z; break;
                case DIRECTION.LeftBackward: --x; --z; break;
                case DIRECTION.RightBackward: ++x; --z; break;
                case DIRECTION.Upper: ++y; break;
                case DIRECTION.Lower: --y; break;
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
            if (!_voxels.ContainsKey(index)) return null;
            if (x >= _widthCount || y >= _heightCount || z >= _depthCount) return null;
            return _voxels[index];
        }
        /// <summary>
        /// Get Cell
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>VoxelCell or null</returns>
        public Voxel GetCell(uint index)
        {
            if (index >= _length) return null;
            if (!_voxels.ContainsKey(index)) return null;
            return _voxels[index];
        }
        /// <summary>
        /// Get Cell
        /// </summary>
        /// <param name="point">Point position(world)</param>
        /// <returns>VoxelCell or null</returns>
        public Voxel GetCell(Vector3 point)
        {
            uint x, y, z;
            if (!GetCellCount(point, out x, out y, out z)) return null;
            return GetCell(x, y, z);
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
            if (index >= _length) return false;
            return true;
        }
        /// <summary>
        /// Get Cell Count
        /// </summary>
        /// <param name="index">index</param>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <param name="z">z</param>
        /// <returns>Success or Fail</returns>
        public bool GetCellCount(uint index, out uint x, out uint y, out uint z)
        {
            x = y = z = 0u;
            if (index >= _length) return false;

            z = index / (_widthCount * _heightCount);
            uint tmp = z * _widthCount * _heightCount;
            y = (index - tmp) / _widthCount;
            x = (index - tmp) % _widthCount;
            if (x >= _widthCount || y >= _heightCount || z >= _depthCount) return false;
            return true;
        }
        /// <summary>
        /// Get Cell Count
        /// </summary>
        /// <param name="position"></param>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <param name="z">z</param>
        /// <returns>Success or Fail</returns>
        public bool GetCellCount(Vector3 position, out uint x, out uint y, out uint z)
        {
            x = y = z = 0u;
            if (!_bound.Contains(position)) return false;

            var areaPosition = position - _bound.min;
            x = areaPosition.x == 0f ? 0u : (uint)(areaPosition.x / _cellSize);
            y = areaPosition.y == 0f ? 0u : (uint)(areaPosition.y / _cellHeight);
            z = areaPosition.z == 0f ? 0u : (uint)(areaPosition.z / _cellSize);
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
            return true;
        }
        /// <summary>
        /// Get Cell Center Position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="position">out Cell Center Position</param>
        /// <returns></returns>
        public bool GetCellPosition(uint x, uint y, uint z, out Vector3 position)
        {
            position = Vector3.zero;
            position.x = x * _cellSize + (_cellSize * 0.5f);
            position.y = y * _cellHeight + (_cellHeight * 0.5f);
            position.z = z * _cellSize + (_cellSize * 0.5f);
            position = position + _bound.min;
            return true;
        }
    }
}