using UnityEngine;

namespace ZKit.Math.Geometry
{
    public static class Collision2D
    {
        /// <summary>
        /// 두 선의 교차 여부와 교차지점을 구한다.
        /// </summary>
        /// <param name="a1">선a의 점1</param>
        /// <param name="a2">선a의 점2</param>
        /// <param name="b1">선b의 점1</param>
        /// <param name="b2">선b의 점2</param>
        /// <param name="intersection">(out) 교차지점</param>
        /// <returns>교차여부</returns>
        public static bool Intersects(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 intersection)
        {
            intersection = Vector2.zero;

            Vector2 a = a2 - a1;
            Vector2 b = b2 - b1;
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
        /// <summary>
        /// 점에서 부터 원의 외접점까지의 선을 구한다.
        /// </summary>
        /// <param name="circle_position">원의 위치</param>
        /// <param name="circle_radius">원의 반지름</param>
        /// <param name="point">점 좌표</param>
        /// <param name="tangentR">점기준 우측 외접선</param>
        /// <param name="tangentL">점기준 좌측 외접선</param>
        public static void GetExTangentOnCircle(Vector2 circle_position, float circle_radius, Vector2 point, out Vector2 tangentR, out Vector2 tangentL)
        {
            Vector2 pointSpaceCircle = circle_position - point;
            float len = pointSpaceCircle.magnitude;
            float tanAng = Mathf.Asin(circle_radius / len);
            float circleAng = Mathf.Atan2(pointSpaceCircle.y, pointSpaceCircle.x);

            tangentR = circle_position + new Vector2(Mathf.Sin(circleAng - tanAng), -Mathf.Cos(circleAng - tanAng)) * circle_radius;
            tangentL = circle_position + new Vector2(-Mathf.Sin(circleAng + tanAng), Mathf.Cos(circleAng + tanAng)) * circle_radius;
        }
        /// <summary>
        /// 원의 외접선 각도를 구한다. 기준은 (0,1)
        /// </summary>
        /// <param name="circle_position">윈의 위치</param>
        /// <param name="circle_radius">원의 반지름</param>
        /// <param name="point">점 좌표</param>
        /// <param name="tanRadR">점기준 우측 외접선 각도</param>
        /// <param name="tanRadL">점기준 좌측 외접선 각도</param>
        /// <param name="halfLimit"></param>
        public static void GetExTangentAngleOnCircle(Vector2 circle_position, float circle_radius, Vector2 point, out float tanRadR, out float tanRadL, bool halfLimit = false)
        {
            Vector2 pointSpaceCircle = circle_position - point;
            float len = pointSpaceCircle.magnitude;
            float tanAng = Mathf.Asin(circle_radius / len);
            float circleAng = Mathf.Atan2(pointSpaceCircle.x, pointSpaceCircle.y);

            tanRadR = circleAng + tanAng;
            tanRadL = circleAng - tanAng;


            if (tanRadR < 0f) tanRadR += 2f * Mathf.PI;
            if (tanRadL < 0f) tanRadL += 2f * Mathf.PI;

            if (halfLimit)
            {
                tanRadR = tanRadR > Mathf.PI ? -(2f * Mathf.PI - tanRadR) : tanRadR;
                tanRadL = tanRadL > Mathf.PI ? -(2f * Mathf.PI - tanRadL) : tanRadL;
            }
        }

        public static bool CollisionDetect2DDot(Box box, Vector2 dot_position)
        {
            Vector2 boxSpaceDot = dot_position - box.Position2D;
            float boxRadius = Mathf.Sqrt((box.size.x * box.size.x) + (box.size.y * box.size.y)) * 0.5f;
            if (boxSpaceDot.magnitude <= boxRadius)
            {
                float rad = box.rotate_y * Mathf.Deg2Rad;
                float cos = Mathf.Cos(rad);
                float sin = Mathf.Sin(rad);

                float x = (boxSpaceDot.x * cos) + (boxSpaceDot.y * -sin);
                float y = (boxSpaceDot.x * sin) + (boxSpaceDot.y * cos);

                boxSpaceDot = new Vector2(x, y);

                if ((box.size.x * 0.5f) >= x && (box.size.x * -0.5f) <= x && (box.size.y * 0.5f) >= y && (box.size.y * -0.5f) <= y)
                    return true;
            }
            return false;
        }
        public static bool CollisionDetect2DDot(Circle circle, Vector2 dot_position)
        {
            Vector2 circleSpaceDot = dot_position - circle.position2D;
            if (circleSpaceDot.magnitude <= circle.radius)
                return true;
            return false;
        }

        public static bool CollisionDetect2DLine(Box box, Vector2 lineStart_position, Vector2 lineEnd_position)
        {
            Vector2 boxSpaceStart = lineStart_position - box.Position2D;
            Vector2 boxSpaceEnd = lineEnd_position - box.Position2D;

            float rad = box.rotate_y * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);

            Vector2 originStart = new Vector2();
            Vector2 originEnd = new Vector2();
            originStart.x = (boxSpaceStart.x * cos) + (boxSpaceStart.y * -sin);
            originStart.y = (boxSpaceStart.x * sin) + (boxSpaceStart.y * cos);
            originEnd.x = (boxSpaceEnd.x * cos) + (boxSpaceEnd.y * -sin);
            originEnd.y = (boxSpaceEnd.x * sin) + (boxSpaceEnd.y * cos);

            float halfWidth = box.size.x * 0.5f;
            float halfHeight = box.size.y * 0.5f;

            Vector2 tl = new Vector2(-halfWidth, halfHeight);
            Vector2 tr = new Vector2(halfWidth, halfHeight);
            Vector2 bl = new Vector2(-halfWidth, -halfHeight);
            Vector2 br = new Vector2(halfWidth, -halfHeight);

            Vector2 pResult;
            if (Intersects(originStart, originEnd, tl, bl, out pResult))
                return true;
            if (Intersects(originStart, originEnd, tl, tr, out pResult))
                return true;
            if (Intersects(originStart, originEnd, tr, br, out pResult))
                return true;
            if (Intersects(originStart, originEnd, bl, br, out pResult))
                return true;

            return false;
        }
        public static bool CollisionDetect2DLine(Circle circle, Vector2 lineStart_position, Vector2 lineEnd_position)
        {
            Vector2 circleSpaceStart = lineStart_position - circle.position2D;
            Vector2 circleSpaceEnd = lineEnd_position - circle.position2D;

            if (circleSpaceStart.magnitude <= circle.radius) return true;
            if (circleSpaceEnd.magnitude <= circle.radius) return true;

            if (Vector2.Dot(circleSpaceStart, circleSpaceEnd - circleSpaceStart) >= 0f) return false;
            if (Vector2.Dot(circleSpaceEnd, circleSpaceEnd - circleSpaceStart) <= 0f) return false;

            float dr = (circleSpaceEnd - circleSpaceStart).magnitude;
            float D = circleSpaceStart.x * circleSpaceEnd.y - circleSpaceEnd.x * circleSpaceStart.y;
            float di = (circle.radius * circle.radius) * (dr * dr) - (D * D);

            if (di < 0) return false;
            return true;
        }

        public static bool CollisionDetect2DRay(Box box, Vector2 lineStart_position, Vector2 line_direction)
        {
            return CollisionDetect2DLine(box, lineStart_position, lineStart_position + (line_direction * 1000.0f));
        }
        public static bool CollisionDetect2DRay(Circle circle, Vector2 lineStart_position, Vector2 line_direction)
        {
            Vector2 circleSpaceStart = lineStart_position - circle.position2D;
            Vector2 circleSpaceEnd = (lineStart_position + line_direction.normalized) - circle.position2D;

            if (circleSpaceStart.magnitude <= circle.radius) return true;
            if (Vector2.Dot(circle.position2D - circleSpaceStart, circleSpaceStart - circleSpaceEnd) >= 0f) return false;
            float D = circleSpaceStart.x * circleSpaceEnd.y - circleSpaceEnd.x * circleSpaceStart.y;
            float di = (circle.radius * circle.radius) - (D * D);

            if (di < 0) return false;
            return true;
        }

        public static bool CollisionDetect2DBox(Vector2 dot_position, Box box)
        {
            return CollisionDetect2DDot(box, dot_position);
        }
        public static bool CollisionDetect2DBox(Vector2 lineStart_position, Vector2 lineEnd_position, Box box)
        {
            return CollisionDetect2DLine(box, lineStart_position, lineEnd_position);
        }
        public static bool CollisionDetect2DBox(Box box, Box box_a)
        {
            return false;
        } // 작업중.
        public static bool CollisionDetect2DBox(Circle circle, Box box)
        {
            return CollisionDetect2DCircle(box, circle);
        }

        public static bool CollisionDetect2DCircle(Vector2 dot_position, Circle circle)
        {
            return CollisionDetect2DDot(circle, dot_position);
        }
        public static bool CollisionDetect2DCircle(Vector2 lineStart_position, Vector2 lineEnd_position, Circle circle)
        {
            return CollisionDetect2DLine(circle, lineStart_position, lineEnd_position);
        }
        public static bool CollisionDetect2DCircle(Box box, Circle circle)
        {
            Vector2 boxSpaceCircle = circle.position2D - box.Position2D;

            float rad = box.rotate_y * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);

            float circle_distance_x = Mathf.Abs((boxSpaceCircle.x * cos) + (boxSpaceCircle.y * -sin));
            float circle_distance_y = Mathf.Abs((boxSpaceCircle.x * sin) + (boxSpaceCircle.y * cos));

            if (circle_distance_x > (box.size.x * 0.5f) + circle.radius) return false;
            if (circle_distance_y > (box.size.y * 0.5f) + circle.radius) return false;

            if (circle_distance_x <= (box.size.x * 0.5f)) return true;
            if (circle_distance_y <= (box.size.y * 0.5f)) return true;

            float corner_distance_sq = (circle_distance_x - (box.size.x * 0.5f)) * (circle_distance_x - (box.size.x * 0.5f))
                + (circle_distance_y - (box.size.y * 0.5f)) * (circle_distance_y - (box.size.y * 0.5f));

            return corner_distance_sq <= (circle.radius * circle.radius);
        }
        public static bool CollisionDetect2DCircle(Circle circle, Circle circle_a)
        {
            return (circle.position2D - circle_a.position2D).magnitude <= circle.radius + circle_a.radius;
        }

        public static bool CollisionDetect2DSector(Vector2 dot_position, Sector sector)
        {
            Vector2 sectorSpaceDot = dot_position - sector.position2D;
            if (sectorSpaceDot.magnitude > sector.radius) return false;

            float rad = sector.rotate_y * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);
            float x = (sectorSpaceDot.x * cos) + (sectorSpaceDot.y * -sin);
            float y = (sectorSpaceDot.x * sin) + (sectorSpaceDot.y * cos);

            sectorSpaceDot.x = x;
            sectorSpaceDot.y = y;

            rad = sector.angle * 0.5f * Mathf.Deg2Rad;

            if (sector.angle < 180f)
            {
                if (((-Mathf.Sin(rad) * sectorSpaceDot.y - sectorSpaceDot.x * Mathf.Cos(rad)) <= 0f)
                    && ((Mathf.Sin(rad) * sectorSpaceDot.y - sectorSpaceDot.x * Mathf.Cos(-rad)) >= 0f))
                    return true;
            }
            else
            {
                if (((-Mathf.Sin(rad) * sectorSpaceDot.y - sectorSpaceDot.x * Mathf.Cos(rad)) <= 0f)
                    || ((Mathf.Sin(rad) * sectorSpaceDot.y - sectorSpaceDot.x * Mathf.Cos(-rad)) >= 0f))
                    return true;
            }

            return false;
        }
        public static bool CollisionDetect2DSector(Vector2 lineStart_position, Vector2 lineEnd_position, Sector sector)
        {
            //Vector2 sectorSpaceStart = lineStart_position - sector.position2D;
            //Vector2 sectorSpaceEnd = lineEnd_position - sector.position2D;

            //float rad = sector.rotate_y * Mathf.Deg2Rad;
            //float cos = Mathf.Cos(rad);
            //float sin = Mathf.Sin(rad);
            //float x = (sectorSpaceStart.x * cos) + (sectorSpaceStart.y * -sin);
            //float y = (sectorSpaceStart.x * sin) + (sectorSpaceStart.y * cos);
            //sectorSpaceStart.x = x;
            //sectorSpaceStart.y = y;
            //x = (sectorSpaceEnd.x * cos) + (sectorSpaceEnd.y * -sin);
            //y = (sectorSpaceEnd.x * sin) + (sectorSpaceEnd.y * cos);
            //sectorSpaceEnd.x = x;
            //sectorSpaceEnd.y = y;

            //if (CollisionDetect2DSector(sectorSpaceStart, sector)) return true;
            //if (CollisionDetect2DSector(sectorSpaceEnd, sector)) return true;

            //rad = (-sector.rotate_y - sector.angle * 0.5f) * Mathf.Deg2Rad;
            //Vector2 right = new Vector2(-Mathf.Sin(rad), Mathf.Cos(rad)) * sector.radius;
            //rad = (-sector.rotate_y + sector.angle * 0.5f) * Mathf.Deg2Rad;
            //Vector2 left = new Vector2(-Mathf.Sin(rad), Mathf.Cos(rad)) * sector.radius;
            //Vector2 tmp;
            //if (Intersects(sectorSpaceStart, sectorSpaceEnd, Vector2.zero, left, out tmp)) return true;
            //if (Intersects(sectorSpaceStart, sectorSpaceEnd, Vector2.zero, right, out tmp)) return true;

            return false;
        }
        public static bool CollisionDetect2DSector(Box box, Sector sector)
        {
            return false;
        }
        public static bool CollisionDetect2DSector(Circle circle, Sector sector)
        {
            Vector2 sectorSpaceCircle = circle.position2D - sector.position2D;
            if (sectorSpaceCircle.magnitude - circle.radius > sector.radius) return false;

            float rad = sector.rotate_y * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);
            float x = (sectorSpaceCircle.x * cos) + (sectorSpaceCircle.y * -sin);
            float y = (sectorSpaceCircle.x * sin) + (sectorSpaceCircle.y * cos);

            sectorSpaceCircle.x = x;
            sectorSpaceCircle.y = y;

            float tanR, tanL;
            GetExTangentAngleOnCircle(sectorSpaceCircle, circle.radius, Vector2.zero, out tanR, out tanL, true);

            if (float.IsNaN(tanR) || float.IsNaN(tanL)) return true;

            float sectorHalfAng = sector.angle * 0.5f;

            float circleAng = Mathf.Atan2(sectorSpaceCircle.x, sectorSpaceCircle.y) * Mathf.Rad2Deg;
            if (circleAng >= 0f && sectorHalfAng >= circleAng) return true;
            if (circleAng < 0f && -sectorHalfAng <= circleAng) return true;

            if (tanL >= 0f && sectorHalfAng >= tanL * Mathf.Rad2Deg) return true;
            if (tanL < 0f && -sectorHalfAng <= tanL * Mathf.Rad2Deg) return true;

            if (tanR >= 0f && sectorHalfAng >= tanR * Mathf.Rad2Deg) return true;
            if (tanR < 0f && -sectorHalfAng <= tanR * Mathf.Rad2Deg) return true;

            //Vector2 sectorL, sectorR;

            //sectorL = new Vector2();
            //sectorL.x = -Mathf.Sin(sector.angle * 0.5f * Mathf.Deg2Rad) * sector.radius;
            //sectorL.y = Mathf.Cos(sector.angle * 0.5f * Mathf.Deg2Rad) * sector.radius;

            //sectorR = new Vector2();
            //sectorR.x = -Mathf.Sin(-sector.angle * 0.5f * Mathf.Deg2Rad) * sector.radius;
            //sectorR.y = Mathf.Cos(-sector.angle * 0.5f * Mathf.Deg2Rad) * sector.radius;

            //Vector2 dir = sectorSpaceCircle - sector.position2D;
            //Debug.Log(sectorL);
            //if ((dir.x * sectorR.y - dir.y * sectorR.x) > 0f)
            //{
            //    if (!CollisionDetect2DCircle(sector.position2D, sector.position2D + sectorR, circle)) return false;
            //}
            //if ((dir.x * sectorL.y - dir.y * sectorL.x) < 0f)
            //{
            //    if (!CollisionDetect2DCircle(sector.position2D, sector.position2D + sectorL, circle)) return false;
            //}

            return false;
        }
        public static bool CollisionDetect2DSector(Sector sector, Sector sector_a)
        {
            return false;
        }
        //private int FindLineCircleIntersections(float cx, float cy, float radius,
        //	PointF point1, PointF point2, out PointF intersection1, out PointF intersection2)
        //{
        //	float dx, dy, A, B, C, det, t;
        //
        //	dx = point2.X - point1.X;
        //	dy = point2.Y - point1.Y;
        //
        //	A = dx * dx + dy * dy;
        //	B = 2 * (dx * (point1.X - cx) + dy * (point1.Y - cy));
        //	C = (point1.X - cx) * (point1.X - cx) + (point1.Y - cy) * (point1.Y - cy) - radius * radius;
        //
        //	det = B * B - 4 * A * C;
        //	if ((A <= 0.0000001) || (det < 0))
        //	{
        //		// No real solutions.
        //		intersection1 = new PointF(float.NaN, float.NaN);
        //		intersection2 = new PointF(float.NaN, float.NaN);
        //		return 0;
        //	}
        //	else if (det == 0)
        //	{
        //		// One solution.
        //		t = -B / (2 * A);
        //		intersection1 = new PointF(point1.X + t * dx, point1.Y + t * dy);
        //		intersection2 = new PointF(float.NaN, float.NaN);
        //		return 1;
        //	}
        //	else
        //	{
        //		// Two solutions.
        //		t = (float)((-B + Math.Sqrt(det)) / (2 * A));
        //		intersection1 = new PointF(point1.X + t * dx, point1.Y + t * dy);
        //		t = (float)((-B - Math.Sqrt(det)) / (2 * A));
        //		intersection2 = new PointF(point1.X + t * dx, point1.Y + t * dy);
        //		return 2;
        //	}
        //}
    }
}