using UnityEngine;
using System.Collections.Generic;

namespace ZKit.Math.Geometry
{
    public static class Util
    {
        /// <summary>
        /// 8분위로 나누어 어디 위치했는지 알아온다.
        /// </summary>
        /// <param name="point">위치</param>
        /// <returns>Octant</returns>
        /// Octants:
        ///    y
        ///  \2|1/
        ///  3\|/0
        /// ---+--- x
        ///  4/|\7
        ///  /5|6\
        public static int GetOctant(Point point)
        {
            int octant = 0;
            if (point.x >= 0) // 0 1 6 7
            {
                if (point.y >= 0) // 0 1
                {
                    if (System.Math.Abs(point.x) >= System.Math.Abs(point.y)) // 0
                        octant = 0;
                    else // 1
                        octant = 1;
                }
                else if (point.y < 0) // 6 7
                {
                    if (System.Math.Abs(point.x) >= System.Math.Abs(point.y)) // 7
                        octant = 7;
                    else // 6
                        octant = 6;
                }
            }
            else if (point.x < 0) // 2 3 4 5
            {
                if (point.y >= 0) // 2 3
                {
                    if (System.Math.Abs(point.x) >= System.Math.Abs(point.y)) // 3
                        octant = 3;
                    else // 2
                        octant = 2;
                }
                else if (point.y < 0) // 4 5
                {
                    if (System.Math.Abs(point.x) >= System.Math.Abs(point.y)) // 4
                        octant = 4;
                    else // 5
                        octant = 5;
                }
            }

            return octant;
        }
        /// <summary>
        /// 기준점을 기준으로 8분위로 나누어 어디 위치했는지 알아온다.
        /// </summary>
        /// <param name="datum">기준점</param>
        /// <param name="point">점</param>
        /// <returns>Octant</returns>
        public static int GetOctant(Point datum, Point point)
        {
            Point dp = point - datum;
            return GetOctant(dp);
        }

        /// <summary>
        /// 8분위로 나누어 어디 위치했는지 알아온다.
        /// </summary>
        /// <param name="pos">위치</param>
        /// <returns>Octant</returns>
        /// Octants:
        ///    y
        ///  \2|1/
        ///  3\|/0
        /// ---+--- x
        ///  4/|\7
        ///  /5|6\
        public static int GetOctant(Vector2 pos)
        {
            int octant = 0;
            if (pos.x >= 0) // 0 1 6 7
            {
                if (pos.y >= 0) // 0 1
                {
                    if (System.Math.Abs(pos.x) >= System.Math.Abs(pos.y)) // 0
                        octant = 0;
                    else // 1
                        octant = 1;
                }
                else if (pos.y < 0) // 6 7
                {
                    if (System.Math.Abs(pos.x) >= System.Math.Abs(pos.y)) // 7
                        octant = 7;
                    else // 6
                        octant = 6;
                }
            }
            else if (pos.x < 0) // 2 3 4 5
            {
                if (pos.y >= 0) // 2 3
                {
                    if (System.Math.Abs(pos.x) >= System.Math.Abs(pos.y)) // 3
                        octant = 3;
                    else // 2
                        octant = 2;
                }
                else if (pos.y < 0) // 4 5
                {
                    if (System.Math.Abs(pos.x) >= System.Math.Abs(pos.y)) // 4
                        octant = 4;
                    else // 5
                        octant = 5;
                }
            }

            return octant;
        }
        /// <summary>
        /// 기준점을 기준으로 8분위로 나누어 어디 위치했는지 알아온다.
        /// </summary>
        /// <param name="datum">기준점</param>
        /// <param name="pos">점</param>
        /// <returns>Octant</returns>
        public static int GetOctant(Vector2 datum, Vector2 pos)
        {
            Vector2 dp = pos - datum;
            return GetOctant(dp);
        }

        /// <summary>
        /// 3차원 좌표계에서 8분위로 나누어 어디 위치했는지 알아온다.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns>octant</returns>
        /// Solid Octants: top to down view
        ///          z                  z
        /// up     1 l 0       down   5 l 4 
        /// side  ------- x    side  ------- x
        /// y(+)   2 l 3       y(-)   6 l 7
        public static int GetSolidOctant(Vector3 pos)
        {
            int octant = 0;
            if (pos.x < 0) octant += 1;
            if (pos.z < 0) octant += 2;
            //octant = octant == 3 ? 2 : octant == 2 ? 3 : octant;
            if (octant == 3) octant = 2; else if (octant == 2) octant = 3;
            if (pos.y < 0) octant += 4;
            return octant;
        }

        private static Point SwitchToOctantZeroFrom(int octant, Point p)
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
        private static UnityEngine.Vector3 SwitchToOctantZeroFrom(int octant, UnityEngine.Vector3 p)
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

        public static List<Point> BresenhamLine(Point p1, Point p2)
        {
            List<Point> ret = new List<Point>();
            Point dp = p2 - p1;
            if (dp.x == 0 || dp.y == 0) return ret;
            int octant = GetOctant(dp);
            dp = SwitchToOctantZeroFrom(octant, dp);
            ret.Add(p1);
            int d = 2 * dp.y - dp.x; // first D
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
        public static List<Point> BresenhamLineEx(Point p1, Point p2)
        {
            List<Point> ret = new List<Point>();
            Point dp = p2 - p1;
            if (dp.x == 0 || dp.y == 0) return ret;
            int octant = GetOctant(dp);
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

        public static List<UnityEngine.Vector3> SupercoverLine(UnityEngine.Vector3 p1, UnityEngine.Vector3 p2)
        {
            List<UnityEngine.Vector3> ret = new List<UnityEngine.Vector3>();
            UnityEngine.Vector2 dp = (p2 - p1).ToVector2NoYaxis();

            if (dp.x == 0 || dp.y == 0) return ret;

            int octant = GetOctant(dp);
            dp = SwitchToOctantZeroFrom(octant, dp);
            float dd = (float)dp.y / (float)dp.x;

            if (dd == 1) return ret;

            ret.Add(p1);

            for (int x = 1; x < dp.x; ++x)
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
    }
}
