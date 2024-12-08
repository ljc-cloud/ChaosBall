using UnityEngine;

namespace ChaosBall.Balls
{
    /// <summary>
    /// 碰上其他球的第一件事就是黏在它身上
    /// </summary>
    public class AttachBall : Ball
    {
        [SerializeField] private PhysicMaterial physicMaterial;
        private Ball _otherBall;

        public override void EffectOtherBall(Ball other)
        {
            if (_otherBall != null) return;
            Debug.Log($"Attach Effect, {transform.name}-{other.transform.name}!");
            _otherBall = other;
            other.EffectBall = this;
            
            // _collider.material = null;
            // _otherBall.GetComponent<CapsuleCollider>().material = null;
            
            other.transform.SetParent(transform);
            
            // _collider.material = physicMaterial;
            // _otherBall.GetComponent<CapsuleCollider>().material = physicMaterial;
            
            _rigidbody.velocity = _beforeBallCollideLastVelocity;
        }

        public override void ProcessEffect(Ball other)
        {
            if (CurrentBallState != BallState.Launched) return;
            other.transform.localPosition = Vector3.ClampMagnitude(other.transform.localPosition, new Vector3(3, 0, 3).magnitude);
        }

        // protected override void Update()
        // {
        //     base.Update();
        //     if (_otherBall != null)
        //     {
        //         _otherBall.localPosition = Vector3.ClampMagnitude(_otherBall.localPosition, new Vector3(3, 0, 3).magnitude);
        //     }
        // }

        protected override void OnBallStopped()
        {
            base.OnBallStopped();
            if (_otherBall == null) return;
            _otherBall.transform.SetParent(null);
            _otherBall.MakeBallStop();
            _otherBall.EffectBall = null;
            _otherBall = null;
        }
    }
}