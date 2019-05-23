﻿namespace IceAndFire
{
    public static class Strategies
    {
        public static readonly BaseStrategy Base = new BaseStrategy();
        public static readonly GrowthStrategy Growth = new GrowthStrategy();
        public static readonly DefenseStrategy Defense = new DefenseStrategy();
    }
}