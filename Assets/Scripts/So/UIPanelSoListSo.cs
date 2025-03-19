using System.Collections.Generic;
using UnityEngine;

namespace ChaosBall.So
{
    [CreateAssetMenu(menuName = "So/UIPanelSoListSo", fileName = "UIPanelSoListSo")]
    public class UIPanelSoListSo : ScriptableObject
    {
        public List<UIPanelSo> uIPanelSoList;
    }
}