using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace ZKit
{
    public class MathUtil
    {
        static public string DecimalToHex32(int num)
        {
            return num.ToString("X8");
        }

//Octants:
//  \2|1/
//  3\|/0
// ---+---
//  4/|\7
//  /5|6\
        static private Point SwitchToOctantZeroFrom(int octant, Point p)
        {
            switch (octant)
            {
                case 0: return p;
                case 1:
                case -1: return new Point(p.y, p.x);
                case 2: return new Point(p.y, -p.x);
                case -2: return new Point(-p.y, p.x);
                case 3:
                case -3: return new Point(-p.x, p.y);
                case 4:
                case -4: return new Point(-p.x, -p.y);
                case 5:
                case -5: return new Point(-p.y, -p.x);
                case 6: return new Point(-p.y, p.x);
                case -6: return new Point(p.y, -p.x);
                case 7:
                case -7: return new Point(p.x, -p.y);
            }
            return p;
        }

        static private UnityEngine.Vector3 SwitchToOctantZeroFrom(int octant, UnityEngine.Vector3 p)
        {
            switch (octant)
            {
                case 0: return p;
                case 1:
                case -1: return new UnityEngine.Vector3(p.z, p.y, p.x);
                case 2: return new UnityEngine.Vector3(p.z, p.y, -p.x);
                case -2: return new UnityEngine.Vector3(-p.z, p.y, p.x);
                case 3:
                case -3: return new UnityEngine.Vector3(-p.x, p.y, p.z);
                case 4:
                case -4: return new UnityEngine.Vector3(-p.x, p.y, -p.z);
                case 5:
                case -5: return new UnityEngine.Vector3(-p.z, p.y, -p.x);
                case 6: return new UnityEngine.Vector3(-p.z, p.y, p.x);
                case -6: return new UnityEngine.Vector3(p.z, p.y, -p.x);
                case 7:
                case -7: return new UnityEngine.Vector3(p.x, p.y, -p.z);
            }
            return p;
        }

        static public List<Point> BresenhamLine(Point p1, Point p2)
        {
            List<Point> ret = new List<Point>();

            Point dp = p2 - p1;
            int octant = 0;
            if (dp.x >= 0) // 0 1 6 7
            {
                if (dp.y >= 0) // 0 1
                {
                    if (Math.Abs(dp.x) >= Math.Abs(dp.y)) // 0
                        octant = 0;
                    else // 1
                        octant = 1;
                }
                else if (dp.y < 0) // 6 7
                {
                    if (Math.Abs(dp.x) >= Math.Abs(dp.y)) // 7
                        octant = 7;
                    else // 6
                        octant = 6;
                }
            }
            else if (dp.x < 0) // 2 3 4 5
            {
                if (dp.y >= 0) // 2 3
                {
                    if (Math.Abs(dp.x) >= Math.Abs(dp.y)) // 3
                        octant = 3;
                    else // 2
                        octant = 2;
                }
                else if (dp.y < 0) // 4 5
                {
                    if (Math.Abs(dp.x) >= Math.Abs(dp.y)) // 4
                        octant = 4;
                    else // 5
                        octant = 5;
                }
            }
            dp = SwitchToOctantZeroFrom(octant, dp);
            ret.Add(p1);
            int d = 2*dp.y - dp.x; // first D
            int y = 0;
            for (int x = 1; x <= dp.x; ++x)
            {
                if (d > 0)
                {
                    ++y;
                    d += (2 * dp.y) - (2 * dp.x);
                }
                else // d <= 0
                    d += 2 * dp.y;

                ret.Add(p1 + SwitchToOctantZeroFrom(-octant, new Point(x, y)));
            }

            return ret;
        }

        static public List<Point> BresenhamLineEx(Point p1, Point p2)
        {
            List<Point> ret = new List<Point>();
            Point dp = p2 - p1;

            if (dp.x == 0 || dp.y == 0) return ret;

            int octant = 0;
            if (dp.x >= 0) // 0 1 6 7
            {
                if (dp.y >= 0) // 0 1
                {
                    if (Math.Abs(dp.x) >= Math.Abs(dp.y)) // 0
                        octant = 0;
                    else // 1
                        octant = 1;
                }
                else if (dp.y < 0) // 6 7
                {
                    if (Math.Abs(dp.x) >= Math.Abs(dp.y)) // 7
                        octant = 7;
                    else // 6
                        octant = 6;
                }
            }
            else if (dp.x < 0) // 2 3 4 5
            {
                if (dp.y >= 0) // 2 3
                {
                    if (Math.Abs(dp.x) >= Math.Abs(dp.y)) // 3
                        octant = 3;
                    else // 2
                        octant = 2;
                }
                else if (dp.y < 0) // 4 5
                {
                    if (Math.Abs(dp.x) >= Math.Abs(dp.y)) // 4
                        octant = 4;
                    else // 5
                        octant = 5;
                }
            }

            dp = SwitchToOctantZeroFrom(octant, dp);
            float d = (float)dp.y / (float)dp.x;

            if (d == 1) return ret;

            ret.Add(p1);

            for (int x = 1; x < dp.x; ++x)
            {
                float tmp = d * (float)x;
                int y = (int)tmp;
                float dec = tmp - y;

                ret.Add(p1 + SwitchToOctantZeroFrom(-octant, new Point(x, y)));
                if (dec == 0) continue;
                ++y;
                ret.Add(p1 + SwitchToOctantZeroFrom(-octant, new Point(x, y)));
            }
            ret.Add(p2);

            return ret;
        }

        static public List<UnityEngine.Vector3> SupercoverLine(UnityEngine.Vector3 p1, UnityEngine.Vector3 p2)
        {
            List<UnityEngine.Vector3> ret = new List<UnityEngine.Vector3>();
            UnityEngine.Vector3 dp = p2 - p1;

            if (dp.x == 0 || dp.z == 0) return ret;

            int octant = 0;
            if (dp.x >= 0) // 0 1 6 7
            {
                if (dp.z >= 0) // 0 1
                {
                    if (Math.Abs(dp.x) >= Math.Abs(dp.z)) // 0
                        octant = 0;
                    else // 1
                        octant = 1;
                }
                else if (dp.z < 0) // 6 7
                {
                    if (Math.Abs(dp.x) >= Math.Abs(dp.z)) // 7
                        octant = 7;
                    else // 6
                        octant = 6;
                }
            }
            else if (dp.x < 0) // 2 3 4 5
            {
                if (dp.z >= 0) // 2 3
                {
                    if (Math.Abs(dp.x) >= Math.Abs(dp.z)) // 3
                        octant = 3;
                    else // 2
                        octant = 2;
                }
                else if (dp.z < 0) // 4 5
                {
                    if (Math.Abs(dp.x) >= Math.Abs(dp.z)) // 4
                        octant = 4;
                    else // 5
                        octant = 5;
                }
            }

            dp = SwitchToOctantZeroFrom(octant, dp);
            float dd = (float)dp.z / (float)dp.x;

            if (dd == 1) return ret;

            ret.Add(p1);

            for (int x = 1; x < dp.x; ++x) // cell 크기가 1X1 인 가정.
            {
                float tmp = dd * (float)x;
                int z = (int)tmp;
                float dec = tmp - z;

                ret.Add(p1 + SwitchToOctantZeroFrom(-octant, new UnityEngine.Vector3(x, 0f, z)));
                if (dec == 0) continue;
                ++z;
                ret.Add(p1 + SwitchToOctantZeroFrom(-octant, new UnityEngine.Vector3(x, 0f, z)));
            }
            ret.Add(p2);

            return ret;
        }

        static public bool Intersects(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 intersection)
        {
            intersection = Vector2.zero;

            Vector2 a = a2 - a1;
            Vector2 b = b2 - b1;
            float aDotbPerp_ = Vector2.Dot(a, b);
            float aDotbPerp = a.x * b.y - a.y * b.x;

            // if b dot d == 0, it means the lines are parallel so have infinite intersection points
            if (aDotbPerp == 0)
                return false;

            Vector2 c = b1 - a1;
            float t = (c.x * b.y - c.y * b.x) / aDotbPerp;
            if (t < 0 || t > 1)
                return false;

            float u = (c.x * a.y - c.y * a.x) / aDotbPerp;
            if (u < 0 || u > 1)
                return false;

            intersection = a1 + t * a;

            return true;
        }
    }
}