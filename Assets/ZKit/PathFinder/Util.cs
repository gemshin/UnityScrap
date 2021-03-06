﻿namespace ZKit.PathFinder
{
    public static partial class Util
    {
        private const int _cellStraight = 10;
        private const int _cellDiagonal = 14;

        public static double BeelineDistance(Node2D a, Node2D b)
        {
            return System.Math.Sqrt(System.Math.Pow(a.X - b.X, 2) + System.Math.Pow(a.Y - b.Y, 2)) * _cellStraight;
        }
        public static int PerpendicularDistance(Node2D a, Node2D b)
        {
            return PrependicularCount(a, b) * _cellStraight;
        }
        public static int PrependicularCount(Node2D a, Node2D b)
        {
            return (System.Math.Abs(a.X - b.X) + System.Math.Abs(a.Y - b.Y));
        }
        public static float GetMoveCost(Node2D from, Node2D to)
        {
            int x = System.Math.Abs(from.X - to.X);
            int y = System.Math.Abs(from.Y - to.Y);
            return (System.Math.Min(x, y) * _cellDiagonal) + ((System.Math.Max(x, y) - System.Math.Min(x, y)) * _cellStraight);
        }
    }
}