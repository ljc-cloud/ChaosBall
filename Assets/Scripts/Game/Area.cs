using System;
using UnityEngine;

namespace ChaosBall.Game
{
    public class Area : MonoBehaviour, IComparable<Area>
    {
        public enum AreaType
        {
            None,
            // 1分
            Blue,
            // 2分
            Yellow,
            // 4分
            Red,
        }
        
        [SerializeField] private AreaType areaType;
        [SerializeField] private int score;
        
        public int Score => score;

        public int CompareTo(Area other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (other is null) return 1;
            var areaTypeComparison = areaType.CompareTo(other.areaType);
            if (areaTypeComparison != 0) return areaTypeComparison;
            return score.CompareTo(other.score);
        }

        public override string ToString()
        {
            return $"areaType: {areaType}, score: {score}";
        }
    }
}