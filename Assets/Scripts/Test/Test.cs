using System;
using ChaosBall.Event;
using UnityEngine;

namespace ChaosBall.Test
{
    public class Test : MonoBehaviour
    {
        [SerializeField] private TestGo go;

        private EventSystem eventSystem;
        
        private void Start()
        {
            eventSystem = new EventSystem();
            eventSystem.Subscribe<TestEvent>(go.Handler);
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.A))
            {
                Destroy(go);
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.F))
            {
                eventSystem.Publish(new TestEvent { value = 1 });
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.S))
            {
                eventSystem.Unsubscribe<TestEvent>(go.Handler);
            }
        }
    }
}