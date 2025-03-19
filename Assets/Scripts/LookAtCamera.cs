using System;
using UnityEngine;

namespace ChaosBall
{
    public class LookAtCamera : MonoBehaviour
    {
        private void LateUpdate()
        {
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        }
    }
}