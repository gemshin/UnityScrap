using System.Collections;
using System.Collections.Generic;

namespace ZKit
{
    public class MathUtil
    {
        static public string DecimalToHex32(int num)
        {
            return num.ToString("X8");
        }

        static public List<Point> Bresenham(Point p1, Point p2)
        {
            List<Point> ret = new List<Point>();
            Point dp = p2 - p1;

            ret.Add(p1);
            float d = 2*dp.y - dp.x; // first D

            int y = p1.y;

            for (int x = p1.x+1; x <= p2.x; ++x)
            {
                if (d > 0)
                {
                    ret.Add(new Point(x, ++y));
                    d += (2 * dp.y) - (2 * dp.x);
                }
                else // d <= 0
                {
                    ret.Add(new Point(x, y));
                    d += 2 * dp.y;
                }
            }

            return ret;
        }

        static public List<Point> BresenhamEx(Point p1, Point p2)
        {
            int i;               // loop counter 
            int ystep, xstep;    // the step on y and x axis 
            int error;           // the error accumulated during the increment 
            int errorprev;       // *vision the previous value of the error variable 

            int y = y1, x = x1;  // the line points 
            int ddy, ddx;        // compulsory variables: the double values of dy and dx 
            int dx = x2 - x1;
            int dy = y2 - y1;
            POINT(y1, x1);  // first point 
            // NB the last point can't be here, because of its previous point (which has to be verified) 
            if (dy < 0)
            {
                ystep = -1;
                dy = -dy;
            }
            else
                ystep = 1;
            if (dx < 0)
            {
                xstep = -1;
                dx = -dx;
            }
            else
                xstep = 1;
            ddy = 2 * dy;  // work with double values for full precision 
            ddx = 2 * dx;
            if (ddx >= ddy)
            {  // first octant (0 <= slope <= 1) 
                // compulsory initialization (even for errorprev, needed when dx==dy) 
                errorprev = error = dx;  // start in the middle of the square 
                for (i = 0; i < dx; i++)
                {  // do not use the first point (already done) 
                    x += xstep;
                    error += ddy;
                    if (error > ddx)
                    {  // increment y if AFTER the middle ( > ) 
                        y += ystep;
                        error -= ddx;
                        // three cases (octant == right->right-top for directions below): 
                        if (error + errorprev < ddx)  // bottom square also 
                            POINT(y - ystep, x);
                        else if (error + errorprev > ddx)  // left square also 
                            POINT(y, x - xstep);
                        else
                        {  // corner: bottom and left squares also 
                            POINT(y - ystep, x);
                            POINT(y, x - xstep);
                        }
                    }
                    POINT(y, x);
                    errorprev = error;
                }
            }
            else
            {  // the same as above 
                errorprev = error = dy;
                for (i = 0; i < dy; i++)
                {
                    y += ystep;
                    error += ddx;
                    if (error > ddy)
                    {
                        x += xstep;
                        error -= ddy;
                        if (error + errorprev < ddy)
                            POINT(y, x - xstep);
                        else if (error + errorprev > ddy)
                            POINT(y - ystep, x);
                        else
                        {
                            POINT(y, x - xstep);
                            POINT(y - ystep, x);
                        }
                    }
                    POINT(y, x);
                    errorprev = error;
                }
            }
            // assert ((y == y2) && (x == x2));  // the last point (y2,x2) has to be the same with the last point of the algorithm 
        }
    }
}