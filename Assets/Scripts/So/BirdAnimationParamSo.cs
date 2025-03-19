using UnityEngine;

namespace ChaosBall.So
{
    [CreateAssetMenu(fileName = "BirdAnimationSo", menuName = "So/BirdAnimationSo", order = 0)]
    public class BirdAnimationParamSo : ScriptableObject
    {
        public enum BirdAnimationType
        {
            Idle,
            MoveLeft,
            MoveRight,
            Ready,
            Shoot,
        }
        
        public string[] idleParams;
        public string[] moveLeftParam;
        public string[] moveRightParam;
        public string[] readyParam;
        public string[] shootParam;

        public string GetRandomAnimationParam(BirdAnimationType animationType)
        {
            switch (animationType)
            {
                default: return "";
                case BirdAnimationType.Idle:
                    return idleParams[UnityEngine.Random.Range(0, idleParams.Length)];
                case BirdAnimationType.MoveLeft:
                    return moveLeftParam[UnityEngine.Random.Range(0, moveLeftParam.Length)];
                case BirdAnimationType.MoveRight:
                    return moveRightParam[UnityEngine.Random.Range(0, moveRightParam.Length)];
                case BirdAnimationType.Ready:
                    return readyParam[UnityEngine.Random.Range(0, readyParam.Length)];
                case BirdAnimationType.Shoot:
                    return shootParam[UnityEngine.Random.Range(0, shootParam.Length)]; // fixme: 数组越界
            }
        }

        public string[] GetAllAnimationParams(BirdAnimationType animationType)
        {
            switch (animationType)
            {
                default: return null;
                case BirdAnimationType.Idle:
                    return idleParams;
                case BirdAnimationType.MoveLeft:
                    return moveLeftParam;
                case BirdAnimationType.MoveRight:
                    return moveRightParam;
                case BirdAnimationType.Ready:
                    return readyParam;
                case BirdAnimationType.Shoot:
                    return shootParam;
            }
        }
    }
}