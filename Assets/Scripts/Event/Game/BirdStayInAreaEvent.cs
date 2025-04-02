using ChaosBall.Game;

namespace ChaosBall.Event.Game
{
    /// <summary>
    /// 留在得分区域事件
    /// </summary>
    public struct BirdStayInAreaEvent : IEvent
    {
        public Area area;
    }
}