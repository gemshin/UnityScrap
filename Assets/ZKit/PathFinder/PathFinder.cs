﻿using UnityEngine;
using System.Collections.Generic;

namespace ZKit.PathFinder
{
    public class JPS : NonPubSingleton<JPS>
    {
        public enum Direction
        {
            Up = 0,
            Down,
            Left,
            Right
        }
        public enum Diagonal
        {
            UpLeft = 0,
            UpRight,
            DownLeft,
            DownRight
        }
        public enum EightDirection
        {
            Up = 0,
            Down,
            Left,
            Right,
            UpLeft,
            UpRight,
            DownLeft,
            DownRight
        }

        private List<Node> _jumpPoint = new List<Node>();
        private void JumpPointAdd(Node node, Node from)
        {
            if (_jumpPoint.Contains(node))
            {
                //float f = GetH(node) + GetG(node, from);
                //if (node.F > f)
                //{
                //    node.G = GetG(node, from);
                //    node.H = GetH(node);
                //    node.F = f;
                //    node.Parent = from;
                //}
                return;
            }
            node.Parent = from;
            node.G = GetG(node, from);
            node.H = GetH(node);
            node.F = node.G + node.H;

            bool insert = false;
            for (int i = 0; i < _jumpPoint.Count; ++i)
            {
                if (_jumpPoint[i].F >= node.F)
                {
                    _jumpPoint.Insert(i, node);
                    insert = true;
                    break;
                }
            }
            if (!insert)
                _jumpPoint.Add(node);
            //TEST.Add(node);
        }
        private void JumpPointRemove(Node node) { _jumpPoint.Remove(node); }
        private void ClosedListAdd(Node node) { _closedList[node.ID] = node; }
        //public List<Node> TEST = new List<Node>();
        public Node[] _closedList = null;

        public Node[,] _map = null;
        private int _countX = 0, _countY = 0;
        public int GetCountX { get { return _countX; } }
        public int GetCountY { get { return _countY; } }
        private float _heightLimit = 1f;

        public Node _start;
        public Node _end;

        public PackCellData _cells;

        public void SetMap(PackCellData cells)
        {
            if (cells.IsEmpty) return;

            _cells = cells;

            _countX = cells.CountX;
            _countY = cells.CountY;
            _heightLimit = cells.HeightLimit;// *cells.CellSize;
            _map = new Node[_countY, _countX];

            for (int y = 0; y < _countY; ++y)
            {
                for (int x = 0; x < _countX; ++x)
                {
                    bool cango = false;
                    if (cells[y, x].Type == CellType.Normal) cango = true;
                    else cango = false;

                    _map[y, x] = new Node((y * _countX + x), x, y, cells[y, x].Height, cango);
                }
            }
            _closedList = new Node[_countX * _countY];
        }

        private bool Prepare()
        {
            if (_map == null) return false;
            //_closedList = new Node[_countX * _countY];
            for(int i = 0; i < _closedList.Length; ++i)
            {
                _closedList[i] = null;
            }
            _jumpPoint.Clear();
            //TEST.Clear();
            foreach (Node node in _map)
            {
                node.Initialize();
            }

            return true;
        }

        private float GetG(Node current, Node from)
        {
            return (from == null ? 0f : Util.GetMoveCost(current, from)) + (from == null ? 0f : from.G);
        }

        private float GetH(Node current)
        {
            //return Util.PerpendicularDistance(current, _end);
            return Util.PrependicularCount(current, _end);
            //return System.Math.Abs(current.X * _end.Y - _end.X * current.Y);
        }

        public List<Vector3> Find(Vector3 start, Vector3 end)
        {
            if (!Prepare()) return null;

            Point sp = cells.GetNearIndex(start);
            Point ep = cells.GetNearIndex(end);
            if (!_map[ep.y, ep.x].CanGo)
            {
                if (!FindMovablePoint(ep, sp, out ep)) return null;
            }

            _start = _map[sp.y, sp.x];
            _end = _map[ep.y, ep.x];

            JumpPointAdd(_map[_start.Y, _start.X], null);

            for (int i = 1; _jumpPoint.Count != 0; ++i)
            {
                Node tmp = _jumpPoint[0];
                if (Scan(tmp)) break;
                ClosedListAdd(tmp);
                JumpPointRemove(tmp);
            }

            List<Point> pathResult = new List<Point>();
            for (Node j = _end; j.Parent != null; j = j.Parent)
            {
                pathResult.Insert(0, j.Index);
            }
            pathResult.Insert(0, _start.Index);

            if (pathResult.Count > 2)
            {
                List<Point> delList = new List<Point>();
                for (int i = 0; i < pathResult.Count; ++i)
                {
                    if (delList.Contains(pathResult[i])) continue;

                    for (int j = i + 2; j < pathResult.Count; ++j)
                    {
                        bool canGo = true;
                        List<Point> line = ZKit.Math.Geometry.Util.BresenhamLineEx(pathResult[i], pathResult[j]);
                        if (line.Count == 0) canGo = false;
                        foreach (Point ele in line)
                        {
                            if (!_map[ele.y, ele.x].CanGo)
                            {
                                canGo = false;
                                break;
                            }
                        }
                        if (canGo)
                        {
                            delList.Add(pathResult[j - 1]);
                        }
                    }
                }
                foreach (Point i in delList)
                {
                    pathResult.Remove(i);
                }
            }
            pathResult.RemoveAt(0);
            List<Vector3> result = new List<Vector3>();
            foreach (Point ele in pathResult)
            {
                result.Add(cells.GetPosVec3(_map[ele.y, ele.x].Index, _map[ele.y, ele.x].Height));
            }
            
            return result;
            //List<Vector3> result = new List<Vector3>();
            //for (Node j = _end; j.Parent != null; j = j.Parent)
            //{
            //    result.Insert(0, cells.GetPosVec3(j.Index, j.Height));
            //}
            ////result.Insert(0, cells.GetPosVec3(_start.Index, _start.Height));
            //return result;
        }

        public List<Point> Find(Point start, Point end)
        {
            if (!Prepare()) return null;

            Point ep = end;
            if (!_map[ep.y, ep.x].CanGo)
            {
                if (!FindMovablePoint(ep, start, out ep)) return null;
            }

            _start = _map[start.y, start.x];
            _end = _map[ep.y, ep.x];

            JumpPointAdd(_map[_start.Y, _start.X], null);

            //////////////////////////////////////////////////
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            //////////////////////////////////////////////////
            int findCount = 0;
            for (int i = 1; _jumpPoint.Count != 0; ++i, ++findCount)
            {
                Node tmp = _jumpPoint[0];
                if (Scan(tmp)) break;
                ClosedListAdd(tmp);
                JumpPointRemove(tmp);
            }
            //////////////////////////////////////////////////
            sw.Stop();
            Debug.Log(string.Format("J길찾기 {0} 회전. 시간 : {1} ms", findCount, sw.ElapsedMilliseconds));
            //////////////////////////////////////////////////
            List<Point> pathResult = new List<Point>();
            for (Node j = _end; j.Parent != null; j = j.Parent)
            {
                pathResult.Insert(0, j.Index);
            }
            pathResult.Insert(0, _start.Index);

            if (pathResult.Count > 2)
            {
                List<Point> delList = new List<Point>();
                for (int i = 0; i < pathResult.Count; ++i)
                {
                    if (delList.Contains(pathResult[i])) continue;

                    for (int j = i + 2; j < pathResult.Count; ++j)
                    {
                        bool canGo = true;
                        List<Point> line = ZKit.Math.Geometry.Util.BresenhamLineEx(pathResult[i], pathResult[j]);
                        if (line.Count == 0) canGo = false;
                        foreach (Point ele in line)
                        {
                            if (!_map[ele.y, ele.x].CanGo)
                            {
                                canGo = false;
                                break;
                            }
                        }
                        if (canGo)
                            delList.Add(pathResult[j - 1]);
                    }
                }
                foreach (Point i in delList)
                {
                    pathResult.Remove(i);
                }
            }
            //pathResult.RemoveAt(0);

            return pathResult;
        }

        private bool FindMovablePoint(Point point, Point player, out Point movablePoint, int maxLevel = 32)
        {
            movablePoint = new Point();

            if(_map == null) return false;

            int octant = ZKit.Math.Geometry.Util.GetOctant(point, player);

            int axis_i, axis_ei, axis_ci;
            int axis_j, axis_ej, axis_cj;
            bool changed = false;

            for (int level = 1; level < maxLevel; ++level)
            {
                switch (octant)
                {
                    default:
                    case 1:
                        axis_i = point.y + level;
                        axis_ei = point.y - level;
                        axis_ci = -1;

                        axis_j = point.x + level;
                        axis_ej = point.x - level;
                        axis_cj = -1;
                        break;
                    case 2:
                        axis_i = point.y + level;
                        axis_ei = point.y - level;
                        axis_ci = -1;

                        axis_j = point.x - level;
                        axis_ej = point.x + level;
                        axis_cj = 1;
                        break;
                    case 3:
                        axis_i = point.x - level;
                        axis_ei = point.x + level;
                        axis_ci = 1;

                        axis_j = point.y + level;
                        axis_ej = point.y - level;
                        axis_cj = -1;
                        changed = true;
                        break;
                    case 4:
                        axis_i = point.x - level;
                        axis_ei = point.x + level;
                        axis_ci = 1;

                        axis_j = point.y - level;
                        axis_ej = point.y + level;
                        axis_cj = 1;
                        changed = true;
                        break;
                    case 5:
                        axis_i = point.y - level;
                        axis_ei = point.y + level;
                        axis_ci = 1;

                        axis_j = point.x - level;
                        axis_ej = point.x + level;
                        axis_cj = 1;
                        break;
                    case 6:
                        axis_i = point.y - level;
                        axis_ei = point.y + level;
                        axis_ci = 1;

                        axis_j = point.x + level;
                        axis_ej = point.x - level;
                        axis_cj = -1;
                        break;
                    case 7:
                        axis_i = point.x + level;
                        axis_ei = point.x - level;
                        axis_ci = -1;

                        axis_j = point.y - level;
                        axis_ej = point.y + level;
                        axis_cj = 1;
                        changed = true;
                        break;
                    case 0:
                        axis_i = point.x + level;
                        axis_ei = point.x - level;
                        axis_ci = -1;

                        axis_j = point.y + level;
                        axis_ej = point.y - level;
                        axis_cj = -1;
                        changed = true;
                        break;
                }

                for (int i = axis_i;; i += axis_ci)
                {
                    if (axis_ei > axis_i && i > axis_ei) break;
                    if (axis_ei < axis_i && i < axis_ei) break;

                    if (i < 0) continue;
                    if (changed) { if (GetCountX <= i) continue; }
                    else { if (GetCountY <= i) continue; }
                    int count = axis_cj;
                    if (i != axis_i && i != axis_ei) // not( first || last )
                        count = 2 * level * axis_cj;

                    for (int j = axis_j;; j += count)
                    {
                        if (axis_ej > axis_j && j > axis_ej) break;
                        if (axis_ej < axis_j && j < axis_ej) break;
                        if (j < 0) continue;
                        if (changed) { if (GetCountY <= j) continue; }
                        else { if (GetCountX <= j) continue; }
                        if (changed)
                        {
                            if (_map[j, i].CanGo == true)
                            {
                                movablePoint = new Point(i, j);
                                return true;
                            }
                        }
                        else
                        {
                            if (_map[i, j].CanGo == true)
                            {
                                movablePoint = new Point(j, i);
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private bool Scan(Node current)
        {
            if (ScanStraight(current)) return true;
            if (ScanDiagonal(current)) return true;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        private bool ScanStraight(Node current)
        {
            foreach (Direction dir in System.Enum.GetValues(typeof(Direction))) // 사방 검사.
            {
                for (int count = 1; ; ++count)
                {
                    int newX = current.X;
                    int newY = current.Y;
                    EightDirection edir = EightDirection.Up;
                    switch (dir)
                    {
                        case Direction.Up: newY += count; edir = EightDirection.Up; break;
                        case Direction.Down: newY -= count; edir = EightDirection.Down; break;
                        case Direction.Left: newX -= count; edir = EightDirection.Left; break;
                        case Direction.Right: newX += count; edir = EightDirection.Right; break;
                    }
                    // Border checks.
                    if (newX < 0 || newX >= _countX) break;
                    if (newY < 0 || newY >= _countY) break;

                    // Goal checks.
                    if (_map[newY, newX] == _end)
                    {
                        _end.Parent = current;
                        return true;
                    }

                    // Obstacle checks.
                    if (_map[newY, newX].CanGo == false) break;

                    // 
                    if (_closedList[_map[newY, newX].ID] != null) break;
                    if (_map[newY, newX].Parent != null) break;

                    if (ScanAround(_map[newY, newX], edir))
                    {
                        JumpPointAdd(_map[newY, newX], current);
                        break;
                    }
                    ClosedListAdd(_map[newY, newX]);
                }
            }
            return false;
        }

        private bool ScanStraightViaDiag(Node current, Node from, out bool findJP)
        {
            findJP = false;
            foreach (Direction dir in System.Enum.GetValues(typeof(Direction))) // 사방 검사.
            {
                for (int count = 1; ; ++count)
                {
                    int newX = current.X;
                    int newY = current.Y;
                    EightDirection edir = EightDirection.Up;
                    switch (dir)
                    {
                        case Direction.Up: newY += count; edir = EightDirection.Up; break;
                        case Direction.Down: newY -= count; edir = EightDirection.Down; break;
                        case Direction.Left: newX -= count; edir = EightDirection.Left; break;
                        case Direction.Right: newX += count; edir = EightDirection.Right; break;
                    }
                    // Border checks.
                    if (newX < 0 || newX >= _countX) break;
                    if (newY < 0 || newY >= _countY) break;

                    // Goal checks.
                    if (_map[newY, newX] == _end)
                    {
                        _end.Parent = current;
                        return true;
                    }

                    // Obstacle checks.
                    if (_map[newY, newX].CanGo == false) break;

                    // 
                    if (_closedList[_map[newY, newX].ID] != null) break;
                    if (_map[newY, newX].Parent != null) break;

                    if (ScanAround(_map[newY, newX], edir))
                    {
                        JumpPointAdd(current, from);
                        JumpPointAdd(_map[newY, newX], current);
                        findJP = true;
                        break;
                    }
                    ClosedListAdd(_map[newY, newX]);
                }
            }
            return false;
        }

        private bool ScanDiagonal(Node current)
        {
            foreach (Diagonal dia in System.Enum.GetValues(typeof(Diagonal)))
            {
                for (int count = 1; ; ++count)
                {
                    int newX = current.X;
                    int newY = current.Y;
                    EightDirection edir = EightDirection.UpRight;
                    switch (dia)
                    {
                        case Diagonal.UpRight: newX += count; newY += count; edir = EightDirection.UpRight; break;
                        case Diagonal.UpLeft: newX -= count; newY += count; edir = EightDirection.UpLeft; break;
                        case Diagonal.DownRight: newX += count; newY -= count; edir = EightDirection.DownRight; break;
                        case Diagonal.DownLeft: newX -= count; newY -= count; edir = EightDirection.DownLeft; break;
                    }
                    if (newX < 0 || newX >= _countX) break;
                    if (newY < 0 || newY >= _countY) break;

                    if (_map[newY, newX] == _end)
                    {
                        _end.Parent = current;
                        return true;
                    }

                    if (_map[newY, newX].CanGo == false) break;

                    if (_closedList[_map[newY, newX].ID] != null) break;
                    if (_map[newY, newX].Parent != null) break;

                    if (ScanAround(_map[newY, newX], edir))
                    {
                        JumpPointAdd(_map[newY, newX], current);
                        break;
                    }
                    bool jp;
                    _map[newY, newX].Parent = current;
                    if (ScanStraightViaDiag(_map[newY, newX], current, out jp))
                    {
                        _end.Parent = _map[newY, newX];
                        return true;
                    }
                    if (!jp)
                        ClosedListAdd(_map[newY, newX]);
                }
            }
            return false;
        }

        /// <summary>
        /// 노드가 Jump Point인지 검사한다.
        /// </summary>
        /// <returns>블럭이 있다. 없다.</returns>
        private bool ScanAround(Node current, EightDirection dir)
        {
            if (dir != EightDirection.Up && dir != EightDirection.Down)
            {
                for (int y = -1; y < 2; y += 2) // up and down
                {
                    int newY = current.Y + y;
                    if (newY < 0 || newY >= _countY) continue;

                    if (_map[newY, current.X].CanGo == false) // obstacle 
                    {
                        int newX = current.X;
                        if (dir == EightDirection.Right) newX += 1;
                        else if (dir == EightDirection.Left) newX -= 1;
                        else if (dir == EightDirection.UpLeft || dir == EightDirection.DownRight) newX += y == -1 ? -1 : 1;
                        else if (dir == EightDirection.UpRight || dir == EightDirection.DownLeft) newX += y == -1 ? 1 : -1;

                        if (newX < 0 || newX >= _countX) continue;
                        if (System.Math.Abs(current.Height - _map[newY, newX].Height) >= _heightLimit) continue;

                        if (_map[newY, newX].CanGo == true)
                            return true;
                    }
                }
            }
            else
            {
                for (int x = -1; x < 2; x += 2)
                {
                    int newX = current.X + x;
                    if (newX < 0 || newX >= _countX) continue;

                    if (_map[current.Y, newX].CanGo == false) // obstacle 
                    {
                        int newY = current.Y;
                        if (dir == EightDirection.Up) newY += 1;
                        else if (dir == EightDirection.Down) newY -= 1;

                        if (newY < 0 || newY >= _countY) continue;

                        if (_map[newY, newX].CanGo == true)
                            return true;
                    }
                }
            }
            return false;
        }
    }

    public class AStar : NonPubSingleton<AStar>
    {
        private Node[] _openList = null;
        public Node[] _closedList = null;
        private List<int> _sortedList = new List<int>();

        //private Cell[,] _map = null;
        public Node[,] _map = null;
        private int _countX = 0, _countY = 0;
        public int GetCountX { get { return _countX; } }
        public int GetCountY { get { return _countY; } }
        private float _heightLimit = 1f;

        public Node _start;
        public Node _end;

        public void SetMap(PackCellData cells)
        {
            if (cells.IsEmpty) return;

            _countX = cells.CountX;
            _countY = cells.CountY;
            _heightLimit = cells.HeightLimit * cells.CellSize;
            _map = new Node[_countY, _countX];

            for (int y = 0; y < _countY; ++y)
            {
                for (int x = 0; x < _countX; ++x)
                {
                    bool cango = false;
                    if (cells[y, x].Type == CellType.Normal) cango = true;
                    else cango = false;

                    _map[y, x] = new Node((y * _countX + x), x, y, cells[y, x].Height, cango);
                }
            }
        }

        private bool Prepare()
        {
            if (_map == null) return false;
            _openList = new Node[_countX * _countY];
            _closedList = new Node[_countX * _countY];
            _sortedList.Clear();
            foreach (Node node in _map)
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
            for (Node j = _end; j.Parent != null; j = j.Parent)
            {
                result.Insert(0, j.Index);
            }
            result.Insert(0, _start.Index);
            return result;
        }

        private void AddOpenList(Node node)
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

        private void AddClosedList(Node node)
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

        private bool ExistOnOpenList(Node node)
        {
            if (_openList[node.ID] != null) return true;
            return false;
        }

        private bool ExistOnClosedList(Node node)
        {
            if (_closedList[node.ID] != null) return true;
            return false;
        }

        private void GetG(Node current, Node from)
        {
            current.G = Util.GetMoveCost(current, from) + from.G;
        }

        private void GetH(Node current)
        {
            current.H = Util.PerpendicularDistance(current, _end);
            //current.H = Util.PrependicularCount(current, _end);
        }

        private void GetF(Node current, Node from)
        {
            GetH(current);
            GetG(current, from);
            current.F = current.H + current.G;
        }

        private void ScanAround(Node current)
        {
            for (int y = -1; y < 2; ++y)
            {
                int newY = current.Y + y;
                if (newY < 0 || newY >= _countY) continue;
                for (int x = -1; x < 2; ++x)
                {
                    int newX = current.X + x;
                    if (newX < 0 || newX >= _countX) continue;

                    if (ExistOnClosedList(_map[newY, newX])) continue;
                    if (ExistOnOpenList(_map[newY, newX])) continue;

                    if (!_map[newY, newX].CanGo)
                    {
                        AddClosedList(_map[newY, newX]);
                        continue;
                    }

                    if (System.Math.Abs(current.Height - _map[newY, newX].Height) >= _heightLimit)
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