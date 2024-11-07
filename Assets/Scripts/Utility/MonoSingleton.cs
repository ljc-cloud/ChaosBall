using UnityEngine;

namespace ChaosBall.Utility
{
    public class MonoSingle<T> : MonoBehaviour where T : MonoSingle<T>
    {
        protected static T _instance;

        public static T Instance => _instance;

        protected virtual void Awake()
        {
            T instance = null;
            var type = typeof(T);
            
            instance = UnityEngine.Object.FindObjectOfType(type) as T;
            if (instance != null)
            {
                _instance = instance;
                return;
            }
            _instance = this as T;
        }
    }
}