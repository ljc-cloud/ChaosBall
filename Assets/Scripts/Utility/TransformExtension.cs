using UnityEngine;

namespace ChaosBall.Utility
{
    public static class TransformExtension
    {
        public static void Active(this Transform transform)
        {
            transform.gameObject.SetActive(true);
        }
        
        public static void DeActive(this Transform transform)
        {
            transform.gameObject.SetActive(false);
        }
    }
}