using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/AreaData", fileName = "AreaData")]
public class AreaData : ScriptableObject
{
    [Serializable]
    public enum AreaType
    {
        Red,
        Yellow,
        Blue,
        None,
        OutSpace
    }
    public AreaType type;
    public Color color;
    public int score;

    public override string ToString()
    {
        return $"{type}, {color}, {score}";
    }
}
