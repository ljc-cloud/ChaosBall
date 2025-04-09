using System;
using System.Collections.Generic;
using ChaosBall.Net;
using ChaosBall.UI;
using UnityEngine;

namespace ChaosBall.Game.State
{
    public class BirdStateMachine : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float maxShootForce;
        [SerializeField] private float minShootForce;
        [SerializeField] private float readyShootDuration;
        [SerializeField] private ArrowForceUI arrowForceUI;
        [SerializeField] private BirdAnimation birdAnimation;
        [SerializeField] private BirdType birdType;
        
        private Rigidbody _rigidbody;
        // private BirdCollide _birdCollide;
        // private IBirdStopBehaviour _birdStopBehaviour;
        
        private BirdUnReadyState _unReadyState;
        private BirdReadyState _readyState;
        private BirdShootState _shootState;
        private BirdStopState _stopState;
        private BirdCollideState _collideState;
        private Entity _mEntity;

        // private float _mShootForce;
        
        public bool Initialized { get; private set; }
        public BirdState CurrentState { get; private set; }
        // public BirdAnimation BirdAnimation => birdAnimation;
        // public ArrowForceUI ArrowForceUI => arrowForceUI;
        // public Entity Entity { get; private set; }

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _mEntity = GetComponent<Entity>();
            
            _unReadyState = new BirdUnReadyState(this, birdAnimation, _mEntity, transform, speed, arrowForceUI);
            _readyState = new BirdReadyState(this, birdAnimation, _mEntity, minShootForce, maxShootForce,
                readyShootDuration, arrowForceUI);
            _shootState = new BirdShootState(this, birdAnimation, _mEntity, transform, _rigidbody, arrowForceUI);
            _stopState = new BirdStopState(this, birdAnimation, _mEntity, _rigidbody, null);
            _collideState = new BirdCollideState(this, birdAnimation, _mEntity, _rigidbody);
            
            ChangeState(BirdState.BirdStateEnum.UnReady);

            Initialized = true;
        }

        private void Update()
        {
            CurrentState?.Update();
        }

        #region Test

        // private BirdState _currentState;
        // private Dictionary<Type, List<Transition>> _transitions = new();
        // private List<Transition> _currentTransitions = new();
        // private List<Transition> _anyTransitions = new();
        // private static readonly List<Transition> EmptyTransitions = new();
        
        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="initState"></param>
        // public void SetState(BirdState newState)
        // {
        //     if (_currentState == newState) return;
        //     _currentState?.Exit();
        //     _currentState = newState;
        //
        //     _transitions.TryGetValue(_currentState.GetType(), out _currentTransitions);
        //     _currentTransitions ??= EmptyTransitions;
        //     
        //     _currentState.Enter();
        // }
        //
        // public void AddTransition(BirdState from, BirdState to, Func<bool> condition)
        // {
        //     if (_transitions.TryGetValue(from.GetType(), out var transitions))
        //     {
        //         transitions.Add(new Transition(to, condition));
        //         return;
        //     }
        //     transitions = new List<Transition>();
        //     _transitions[from.GetType()] = transitions;
        // }
        //
        // public void AddAnyTransition(BirdState state, Func<bool> condition)
        // {
        //     _anyTransitions.Add(new Transition(state, condition));
        // }
        //
        // private Transition GetTransition()
        // {
        //     foreach (var transition in _anyTransitions)
        //     {
        //         if (transition.Condition.Invoke()) return transition;
        //     }
        //
        //     foreach (var transition in _currentTransitions)
        //     {
        //         if (transition.Condition.Invoke()) return transition;
        //     }
        //     return null;
        // }

        #endregion

        public void ChangeState(BirdState.BirdStateEnum newState)
        {
            CurrentState?.Exit();
            
            // BirdState.BirdStateEnum? oldState = CurrentState?.State;
            
            switch (newState)
            {
                case BirdState.BirdStateEnum.UnReady:
                    CurrentState = _unReadyState;
                    break;
                case BirdState.BirdStateEnum.Ready:
                    CurrentState = _readyState;
                    break;
                case BirdState.BirdStateEnum.Shoot:
                    CurrentState = _shootState;
                    break;
                case BirdState.BirdStateEnum.Stop:
                    CurrentState = _stopState;
                    break;
                // case BirdState.BirdStateEnum.Count:
                //     // CurrentState = _mCountState;
                //     break;
                case BirdState.BirdStateEnum.Collided:
                    CurrentState = _collideState;
                    break;
                default: CurrentState = _unReadyState;
                    break;
            }

            // CurrentState.FromStateEnum = oldState ?? BirdState.BirdStateEnum.UnReady;
            
            CurrentState.Enter();
        }

        // public Vector3 GetDirection() => -arrowForceUI.transform.TransformDirection(arrowForceUI.transform.forward);
        // private float GetShootForce() => _mShootForce;
        //
        // public void SetShootForce(float newForce)
        // {
        //     _mShootForce = newForce;
        // }
    }
}