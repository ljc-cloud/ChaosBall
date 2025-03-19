using System;
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
        public int score;
        
        private BirdCollide _mBirdCollide;

        private void Awake()
        {
            _mBirdCollide = GetComponent<BirdCollide>();
        }

        private void Start()
        {
            _mBirdCollide.OnBirdEnterArea += OnBirdEnterArea;
            _mBirdCollide.OnBirdExitArea += OnBirdExitArea;
        }

        private void OnDestroy()
        {
            _mBirdCollide.OnBirdEnterArea -= OnBirdEnterArea;
            _mBirdCollide.OnBirdExitArea -= OnBirdExitArea;
        }

        private void OnBirdEnterArea(Area area)
        {
            score = area.Score;
        }
        private void OnBirdExitArea(Area area)
        {
            score = 0;
        }
    }
}