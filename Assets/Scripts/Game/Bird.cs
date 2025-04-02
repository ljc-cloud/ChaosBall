using System;
using System.Linq;
using ChaosBall.Event.Game;
using UnityEngine;

namespace ChaosBall.Game
{
    public class Bird : MonoBehaviour
    {
        /// <summary>
        /// 第几个球
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 这个球的得分
        /// </summary>
        public int Score { get; private set; }

        private float _mBirdRadius;
        private Collider[] _mAreaTriggerArray;

        private void Awake()
        {
            _mBirdRadius = GetComponent<SphereCollider>().radius;
            int size = Enum.GetValues(typeof(Area.AreaType)).Length;
            _mAreaTriggerArray = new Collider[size];
        }

        private void Start()
        {
            GameInterface.Interface.EventSystem.Subscribe<BirdEnterAreaEvent>(OnBirdEnterArea);
            GameInterface.Interface.EventSystem.Subscribe<BirdExitAreaEvent>(OnBirdExitArea);
        }

        private void OnDestroy()
        {
            GameInterface.Interface.EventSystem.Unsubscribe<BirdEnterAreaEvent>(OnBirdEnterArea);
            GameInterface.Interface.EventSystem.Unsubscribe<BirdExitAreaEvent>(OnBirdExitArea);
        }

        private void CalcScore(/*BirdCalcScoreEvent _*/)
        {
            float areaLayerDetectOffset = .5f;
            int size = Physics.OverlapSphereNonAlloc(transform.position + Vector3.up * _mBirdRadius
                , _mBirdRadius + areaLayerDetectOffset, _mAreaTriggerArray
                , 1 << GameAssets.AREA_LAYER, QueryTriggerInteraction.Collide);
            if (size == 0)
            {
                Debug.LogError("Not Collide With Area!");
                return;
            }
            Area[] areaArray = _mAreaTriggerArray.Select(item => item?.transform.GetComponent<Area>()).ToArray();
                
            Array.Sort(areaArray);
            Area correctArea = areaArray[^1];
            Score = correctArea.Score;
        }

        private void OnBirdEnterArea(BirdEnterAreaEvent e)
        {
            Score = e.area.Score;
        }
        private void OnBirdExitArea(BirdExitAreaEvent e)
        {
            Score = 0;
        }
    }
}