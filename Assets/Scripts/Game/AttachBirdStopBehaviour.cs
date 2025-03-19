using UnityEngine;

namespace ChaosBall.Game
{
    public class AttachBirdStopBehaviour : MonoBehaviour, IBirdStopBehaviour
    {
        public void OnStop()
        {
            if (transform.TryGetComponent(out FixedJoint fixedJoint))
            {
                Destroy(fixedJoint);
            }
        }
    }
}