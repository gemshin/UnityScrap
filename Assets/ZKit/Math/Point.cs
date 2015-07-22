namespace ZKit
{
    public struct Point
    {
        public int x;
        public int y;

        public Point(int x, int y) { this.x = x; this.y = y; }

        public static Point operator +(Point l, Point r)
        {
            l.x += r.x; l.y += r.y;
            return l;
        }

        public static Point operator -(Point l, Point r)
        {
            l.x -= r.x; l.y -= r.y;
            return l;
        }

        public static Point operator *(Point l, Point r)
        {
            l.x *= r.x; l.y *= r.y;
            return l;
        }

        public static Point operator *(Point l, int r)
        {
            l.x *= r; l.y *= r;
            return l;
        }

        public void FromString(string value)
        {
            string[] tmp = value.Replace(" ", "").Split(',');
            x = int.Parse(tmp[0]);
            y = int.Parse(tmp[1]);
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}", x, y);
        }

        public UnityEngine.Vector2 ToVec2()
        {
            return new UnityEngine.Vector2(x, y);
        }

        public UnityEngine.Vector3 Tovec3(float centerValue = 0)
        {
            return new UnityEngine.Vector3(x, centerValue, y);
        }
    }
}