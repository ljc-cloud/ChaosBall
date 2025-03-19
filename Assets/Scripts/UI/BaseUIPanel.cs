using UnityEngine;

namespace ChaosBall.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class BaseUIPanel : MonoBehaviour
    {
        public virtual void OnInit()
        {
            
        }

        public virtual void OnBeforeShow()
        {
            
        }
        
        public virtual void OnShow()
        {
            
        }

        public virtual void OnAfterShow()
        {
            
        }
    }
}