using ChaosBall.Model;
using ChaosBall.Utility;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/LevelData", fileName = "LevelData")]
public class LevelData : ScriptableObject
{
    
    public LevelEnum level;
    public string levelName;
    public Sprite levelSprite;
    public string levelDescription;
    public GameObject[] balls;
    public BallDescription[] ballDescriptions;
    public bool isLocked;
    
}
