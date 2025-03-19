using UnityEngine;

namespace ChaosBall.Game
{
    public interface IBirdCollideBehaviour
    {
        /// <summary>
        /// 碰撞到其他Bird
        /// </summary>
        /// <param name="thisTransform">Shoot状态的Bird</param>
        /// <param name="other">Stop状态的Bird</param>
        void OnCollideOtherBird(Transform thisTransform, Collision other);
    }
}