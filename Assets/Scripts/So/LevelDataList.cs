using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/LevelDataList", fileName = "LevelDataList")]
public class LevelDataList : ScriptableObject
{
    public List<LevelData> levelList;
}
