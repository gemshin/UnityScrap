using UnityEngine;
using System.Collections.Generic;

namespace ZKit.PathFinder
{
    public class AStar : NonPubSingleton<AStar>
    {
        private Node2D[] _openList = null;
        public Node2D[] _closedList = null;
        private List<int> _sortedList = new List<int>();

        //private Cell[,] _map = null;
        public Node2D[,] _map = null;
        public Node2D _start;
        public Node2D _end;

        PackCellData _cells;

        public void SetMap(PackCellData cells)
        {
            if (cells.IsEmpty) return;
            _cells = cells;

            _map = new Node2D[_cells.CountY, _cells.CountX];

            for (int y = 0; y < _cells.CountY; ++y)
            {
                for (int x = 0; x < _cells.CountX; ++x)
                {
                    bool cango = false;
                    if (cells[y, x].Type == CellType.Normal) cango = true;
                    else cango = false;

                    _map[y, x] = new Node2D((y * _cells.CountX + x), x, y, cells[y, x].Height, cango);
                }
            }
        }

        private bool Prepare()
        {
            if (_map == null) return false;
            _openList = new Node2D[_cells.CountX * _cells.CountY];
            _closedList = new Node2D[_cells.CountX * _cells.CountY];
            _sortedList.Clear();
            foreach (Node2D node in _map)
            {
                node.Initialize();
            }
            return true;
        }

        public List<Point> Find(Point start, Point end)
        {
            if (!Prepare()) return null;

            _start = _map[start.y, start.x];
            _end = _map[end.y, end.x];

            AddOpenList(_start);

            return Find();
        }

        private List<Point> Find()
        {
            //////////////////////////////////////////////////
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            //////////////////////////////////////////////////
            int i = 0;
            for (i = 1; _sortedList.Count != 0; ++i)
            {
                if (_openList[_sortedList[0]] == _end)
                    break;

                ScanAround(_openList[_sortedList[0]]);
            }
            //////////////////////////////////////////////////
            sw.Stop();
            Debug.Log(string.Format("길찾기 {0} 회전. 시간 : {1} ms", i, sw.ElapsedMilliseconds));
            //////////////////////////////////////////////////
            List<Point> result = new List<Point>();
            for (Node2D j = _end; j.Parent != null; j = j.Parent)
            {
                result.Insert(0, j.Index);
            }
            result.Insert(0, _start.Index);
            return result;
        }

        private void AddOpenList(Node2D node)
        {
            if (_sortedList.Count == 0)
                _sortedList.Add(node.ID);
            else
            {
                bool insert = false;
                for (int i = 0; i < _sortedList.Count; ++i)
                {
                    if (_openList[_sortedList[i]].F >= node.F)
                    {
                        _sortedList.Insert(i, node.ID);
                        insert = true;
                        break;
                    }
                }
                if (!insert)
                {
                    _sortedList.Add(node.ID);
                }
            }

            _openList[node.ID] = node;
            _closedList[node.ID] = null;
        }

        private void AddClosedList(Node2D node)
        {
            if (ExistOnOpenList(node))
            {
                for (int i = 0; i < _sortedList.Count; ++i)
                {
                    if (_sortedList[i] == node.ID)
                    {
                        _sortedList.RemoveAt(i);
                        break;
                    }
                }
                _openList[node.ID] = null;
            }
            _closedList[node.ID] = node;
        }

        private bool ExistOnOpenList(Node2D node)
        {
            if (_openList[node.ID] != null) return true;
            return false;
        }

        private bool ExistOnClosedList(Node2D node)
        {
            if (_closedList[node.ID] != null) return true;
            return false;
        }

        private void GetG(Node2D current, Node2D from)
        {
            current.G = Util.GetMoveCost(current, from) + from.G;
        }

        private void GetH(Node2D current)
        {
            current.H = Util.PerpendicularDistance(current, _end);
            //current.H = Util.PrependicularCount(current, _end);
        }

        private void GetF(Node2D current, Node2D from)
        {
            GetH(current);
            GetG(current, from);
            current.F = current.H + current.G;
        }

        private void ScanAround(Node2D current)
        {
            for (int y = -1; y < 2; ++y)
            {
                int newY = current.Y + y;
                if (newY < 0 || newY >= _cells.CountY) continue;
                for (int x = -1; x < 2; ++x)
                {
                    int newX = current.X + x;
                    if (newX < 0 || newX >= _cells.CountX) continue;

                    if (ExistOnClosedList(_map[newY, newX])) continue;
                    if (ExistOnOpenList(_map[newY, newX])) continue;

                    if (!_map[newY, newX].CanGo)
                    {
                        AddClosedList(_map[newY, newX]);
                        continue;
                    }

                    if (System.Math.Abs(current.Height - _map[newY, newX].Height) >= _cells.HeightLimit)
                    {
                        continue;
                    }

                    GetF(_map[newY, newX], current);
                    _map[newY, newX].Parent = current;
                    AddOpenList(_map[newY, newX]);
                }
            }
            AddClosedList(current);
        }
    }
}