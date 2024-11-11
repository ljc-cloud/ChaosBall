using UnityEngine;
using UnityEngine.Animations;

namespace ChaosBall.Balls
{
    /// <summary>
    /// TODO 碰上其他球的第一件事就是黏在它身上
    /// </summary>
    public class AttachBall : Ball
    {
        [SerializeField] private PhysicMaterial physicMaterial;
        private Transform _otherBall;

        private PositionConstraint _positionConstraint;

        protected override void Awake()
        {
            base.Awake();
            _positionConstraint = GetComponent<PositionConstraint>();
        }

        // TODO
        protected override void EffectOtherBall(Ball other)
        {
            if (_otherBall != null) return;
            Debug.Log($"Attach Effect, {transform.name}-{other.transform.name}!");
            _otherBall = other.transform;
            _collider.material = null;
            _otherBall.GetComponent<CapsuleCollider>().material = null;
            _otherBall.SetParent(transform);
            _collider.material = physicMaterial;
            _otherBall.GetComponent<CapsuleCollider>().material = physicMaterial;
            _rigidbody.velocity = BeforeCollideLastVelocity;
        }

        protected override void Update()
        {
            base.Update();
            if (_otherBall != null)
            {
                _otherBall.localPosition = Vector3.ClampMagnitude(_otherBall.localPosition, new Vector3(3, 0, 3).magnitude);
            }
        }

        protected override void OnBallStopped()
        {
            base.OnBallStopped();
            if (_otherBall == null) return;
            Debug.Log($"Before: WorldPos: {_otherBall.position}, localPos: {_otherBall.localPosition}");
            
            _otherBall.SetParent(null);
            _otherBall = null;
            Debug.Log($"After: WorldPos: {_otherBall.position}, localPos: {_otherBall.localPosition}");
        }
    }
}