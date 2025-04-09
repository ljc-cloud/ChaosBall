using System;
using System.Collections.Generic;
using System.Linq;
using ChaosBall.Event.Game;
using ChaosBall.Game.State;
using ChaosBall.Net;
using ChaosBall.UI;
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

        private BirdCollide _birdCollide;
        private float _mBirdRadius;
        private Collider[] _mAreaTriggerArray;
        
        private Stack<int> _scoreStack = new();
        
        private void Awake()
        {
            _birdCollide = GetComponent<BirdCollide>();
            _mBirdRadius = GetComponent<SphereCollider>().radius;
            int size = Enum.GetValues(typeof(Area.AreaType)).Length;
            _mAreaTriggerArray = new Collider[size];
        }

        private void Start()
        {
            _birdCollide.OnBirdEnterArea += OnBirdEnterArea;
            _birdCollide.OnBirdExitArea += OnBirdExitArea;
        }

        private void OnDestroy()
        {
            _birdCollide.OnBirdEnterArea -= OnBirdEnterArea;
            _birdCollide.OnBirdExitArea -= OnBirdExitArea;
        }

        private void OnBirdEnterArea(object sender, BirdCollide.OnBirdEnterAreaEventArgs e)
        {
            Area area = e.area;
            Debug.Log($"Enter area: {area.Score}");
            _scoreStack.Push(area.Score);
            Score = _scoreStack.Peek();
            Debug.Log($"Enter, Score: {Score}");
        }
        private void OnBirdExitArea(object sender, EventArgs e)
        {
            int pop = _scoreStack.Pop();
            Debug.Log($"Exit area: {pop}");
            if (_scoreStack.TryPeek(out var score))
            {
                Score = score;
            }
            Debug.Log($"Exit, Score: {Score}");
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
    }
}