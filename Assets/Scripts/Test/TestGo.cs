using ChaosBall.Event;
using UnityEngine;

namespace ChaosBall.Test
{
    public class TestGo : MonoBehaviour
    {
        public void Handler(TestEvent obj)
        {
            Debug.Log("Invoked " + obj.value);
        }
    }
}