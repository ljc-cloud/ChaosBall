using System.Collections.Generic;
using ChaosBall.UI;
using UnityEngine;

namespace ChaosBall.So
{
    [CreateAssetMenu(menuName = "So/UIPanelSo", fileName = "UIPanelSo")]
    public class UIPanelSo : ScriptableObject
    {
        public UIPanelType uIPanelType;
        public string path;
    }
}
