using System.Collections;
using System.Collections.Generic;
using System;

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
                case 1: return new Point(p.y, p.x);
                case -1: return new Point(p.y, p.x);
                case 2: return new Point(p.y, -p.x);
                case -2: return new Point(-p.y, p.x);
                case 3: return new Point(-p.x, p.y);
                case -3: return new Point(-p.x, p.y);
                case 4: return new Point(-p.x, -p.y);
                case -4: return new Point(-p.x, -p.y);
                case 5: return new Point(-p.y, -p.x);
                case -5: return new Point(-p.y, -p.x);

                case 6: return new Point(p.y, -p.x);
                case -6: return new Point(-p.y, p.x);

                case 7: return new Point(p.x, -p.y);
                case -7: return new Point(p.x, -p.y);
            }
            return p;
        }

        static public List<Point> BresenhamLine(Point a, Point b)
        {
            List<Point> ret = new List<Point>();

            Point dp = b - a;

            int octant = 0;
            Point p1, p2;
            if (dp.x >= 0) // 0 1 6 7
            {
                if( dp.y >= 0) // 0 1
                {
                    if ( Math.Abs(dp.x) >= Math.Abs(dp.y) ) // 0
                        octant = 0;
                    else // 1
                        octant = 1;
                }
                else if( dp.y < 0) // 6 7
                {
                    if ( Math.Abs(dp.x) >= Math.Abs(dp.y) ) // 7
                        octant = 7;
                    else // 6
                        octant = 6;
                }
            }
            else if (dp.x < 0) // 2 3 4 5
            {
                if( dp.y >= 0) // 2 3
                {
                    if( Math.Abs(dp.x) >= Math.Abs(dp.y) ) // 3
                        octant = 3;
                    else // 2
                        octant = 2;
                }
                else if( dp.y < 0) // 4 5
                {
                    if( Math.Abs(dp.x) >= Math.Abs(dp.y) ) // 4
                        octant = 4;
                    else // 5
                        octant = 5;
                }
            }
            p1 = SwitchToOctantZeroFrom(octant, a);
            p2 = SwitchToOctantZeroFrom(octant, b);
            //p1 = a;
            //p2 = b;
            UnityEngine.Debug.Log("bf: " + dp.ToString() + " p1 " + a.ToString() + " p2 " + b.ToString() + "    " + octant.ToString());

            dp = p2 - p1;

            UnityEngine.Debug.Log("af: " + dp.ToString() + " p1 " + p1.ToString() + " p2 " + p2.ToString());

            ret.Add(SwitchToOctantZeroFrom(-octant, p1));
            int d = 2*dp.y - dp.x; // first D

            int y = p1.y;
            //UnityEngine.Debug.Log(string.Format("dp:{0} d:{1} y:{2}", dp.ToString(), d.ToString(), y.ToString()));
            for (int x = p1.x+1; x <= p2.x; ++x)
            {
                if (d > 0)
                {
                    ++y;
                    d += (2 * dp.y) - (2 * dp.x);
                }
                else // d <= 0
                    d += 2 * dp.y;

                ret.Add(SwitchToOctantZeroFrom(-octant, new Point(x, y)));
                Point kkk = SwitchToOctantZeroFrom(-octant, new Point(x, y));
            }

            return ret;
        }

        static public List<Point> BresenhamLineEx(Point p1, Point p2)
        {
            List<Point> ret = new List<Point>();

            return ret;
        }

    //    static public List<Point> BresenhamLineEx(Point p1, Point p2)
    //    {
    //        int i;               // loop counter 
    //        int ystep, xstep;    // the step on y and x axis 
    //        int error;           // the error accumulated during the increment 
    //        int errorprev;       // *vision the previous value of the error variable 

    //        int y = y1, x = x1;  // the line points 
    //        int ddy, ddx;        // compulsory variables: the double values of dy and dx 
    //        int dx = x2 - x1;
    //        int dy = y2 - y1;

    //        POINT(y1, x1);  // first point 
    //        // NB the last point can't be here, because of its previous point (which has to be verified) 
    //        if (dy < 0)
    //        {
    //            ystep = -1;
    //            dy = -dy;
    //        }
    //        else
    //            ystep = 1;
    //        if (dx < 0)
    //        {
    //            xstep = -1;
    //            dx = -dx;
    //        }
    //        else
    //            xstep = 1;
    //        ddy = 2 * dy;  // work with double values for full precision 
    //        ddx = 2 * dx;
    //        if (ddx >= ddy)
    //        {  // first octant (0 <= slope <= 1) 
    //            // compulsory initialization (even for errorprev, needed when dx==dy) 
    //            errorprev = error = dx;  // start in the middle of the square 
    //            for (i = 0; i < dx; i++)
    //            {  // do not use the first point (already done) 
    //                x += xstep;
    //                error += ddy;
    //                if (error > ddx)
    //                {  // increment y if AFTER the middle ( > ) 
    //                    y += ystep;
    //                    error -= ddx;
    //                    // three cases (octant == right->right-top for directions below): 
    //                    if (error + errorprev < ddx)  // bottom square also 
    //                        POINT(y - ystep, x);
    //                    else if (error + errorprev > ddx)  // left square also 
    //                        POINT(y, x - xstep);
    //                    else
    //                    {  // corner: bottom and left squares also 
    //                        POINT(y - ystep, x);
    //                        POINT(y, x - xstep);
    //                    }
    //                }
    //                POINT(y, x);
    //                errorprev = error;
    //            }
    //        }
    //        else
    //        {  // the same as above 
    //            errorprev = error = dy;
    //            for (i = 0; i < dy; i++)
    //            {
    //                y += ystep;
    //                error += ddx;
    //                if (error > ddy)
    //                {
    //                    x += xstep;
    //                    error -= ddy;
    //                    if (error + errorprev < ddy)
    //                        POINT(y, x - xstep);
    //                    else if (error + errorprev > ddy)
    //                        POINT(y - ystep, x);
    //                    else
    //                    {
    //                        POINT(y, x - xstep);
    //                        POINT(y - ystep, x);
    //                    }
    //                }
    //                POINT(y, x);
    //                errorprev = error;
    //            }
    //        }
    //        // assert ((y == y2) && (x == x2));  // the last point (y2,x2) has to be the same with the last point of the algorithm 
    //    }
    }
}