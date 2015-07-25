using System;

namespace LogicSpawn.GTARPG.Core
{
    public static class Random
    {
        private static System.Random random = new System.Random((int)DateTime.Now.Ticks);

        public static float Range(float minimum, float maximum)
        {
            return (float)(random.NextDouble() * (maximum - minimum) + minimum);
        }
        public static int Range(int minimum, int maximum)
        {
            return random.Next(minimum, maximum);
        }
    }
}
