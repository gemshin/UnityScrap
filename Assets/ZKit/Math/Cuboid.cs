namespace ZKit
{
    public struct Cuboid
    {
        public float xMax;
        public float yMax;
        public float zMax;
        public float xMin;
        public float yMin;
        public float zMin;

        public Cuboid(Cuboid source)
        {
            this.xMin = source.xMin; this.xMax = source.xMax;
            this.yMin = source.yMin; this.yMax = source.yMax;
            this.zMin = source.zMin; this.zMax = source.zMax;
        }

        public Cuboid(float xMin, float xMax, float yMin, float yMax, float zMin, float zMax)
        {
            this.xMin = xMin; this.xMax = xMax;
            this.yMin = yMin; this.yMax = yMax;
            this.zMin = zMin; this.zMax = zMax;
        }

        //public static Cuboid operator +(Point l, Point r)
        //{
        //    l.x += r.x; l.y += r.y;
        //    return l;
        //}

        //public static Cuboid operator -(Point l, Point r)
        //{
        //    l.x -= r.x; l.y -= r.y;
        //    return l;
        //}

        //public static Cuboid operator *(Point l, Point r)
        //{
        //    l.x *= r.x; l.y *= r.y;
        //    return l;
        //}

        //public static Cuboid operator *(Point l, int r)
        //{
        //    l.x *= r; l.y *= r;
        //    return l;
        //}

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}, {4}, {5}", xMin, xMax, yMin, yMax, zMin, zMax);
        }
    }
}