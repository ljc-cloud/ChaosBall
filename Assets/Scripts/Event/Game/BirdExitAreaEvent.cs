using ChaosBall.Game;

namespace ChaosBall.Event.Game
{
    /// <summary>
    /// 离开得分区域事件
    /// </summary>
    public struct BirdExitAreaEvent : IEvent
    {
        public Area area;
    }
}