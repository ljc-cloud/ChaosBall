using System;

namespace ChaosBall.Game.State
{
    /// <summary>
    /// 状态转换类
    /// </summary>
    public class Transition
    {
        public BirdState To { get; }
        public Func<bool> Condition { get; }

        public Transition(BirdState to, Func<bool> condition)
        {
            To = to;
            Condition = condition;
        }
    }
}