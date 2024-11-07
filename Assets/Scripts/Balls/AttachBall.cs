using UnityEngine;

namespace ChaosBall.Balls
{
    /// <summary>
    /// TODO 开朗的小球，碰上其他球的第一件事就是黏在它身上
    /// </summary>
    public class AttachBall : Ball
    {
        protected override void OnCollideOtherBall(Collision other)
        {
            base.OnCollideOtherBall(other);
            // 黏住其他球 or 让其他球黏住自己
            transform.SetParent(other.transform);
        }
    }
}