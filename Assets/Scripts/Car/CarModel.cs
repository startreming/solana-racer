using DG.Tweening;
using UnityEngine;

namespace Car
{
    public class CarModel : MonoBehaviour
    {
        [Header("Model Wheel Axle")]
        [SerializeField] private Transform frontWheels;
        [SerializeField] private Transform backWheels;
        
        [Header("Model Individual Wheels")]
        [SerializeField] private Transform[] frontIndividualWheels;
        
        [Header("Model Details")]
        [SerializeField] private Transform steeringWheel;
        [SerializeField] private Transform kartChassis;

        [Header("Weight Transfer Simulation")]
        [SerializeField] private Axes bodyPitchAxis = Axes.X;
        [SerializeField] private Axes bodyRollAxis = Axes.Z;

        [SerializeField] private float bodyRollFactor = 0.7f;
        [SerializeField] private float speedFactor = 20f;
        [SerializeField] private bool invertBodyPitch = false;

        [Header("Physics visualizations")]
        [SerializeField] private float wheelVelocity = 0.5f;
        [SerializeField] private float maxSteeringAngle = 15f;
        [SerializeField] private float extraBodyAngle = 15f;


        
        // Tweeners
        private float _bodyRoll, _bodyPitch, _frontWheelAngle;
        private bool _drifting;
        private Tweener _tweenHopPosition;
        private Tweener _tweenHopScale;
        private Tweener _tweenLocalRotate;
        private Tweener _tweenTrick;
        private Tweener _tweenKnocked;
        private Tweener _tweenCarSquash;

        public enum Axes { X, Y, Z }

        public virtual void PointFrontWheels(float horizontalSteer, float rigidbodyMagnitude)
        {
            var targetFrontWheelAngle = (horizontalSteer * maxSteeringAngle) / Mathf.Clamp(rigidbodyMagnitude*0.1f, 1f, 10f);
            _frontWheelAngle = Mathf.Lerp(_frontWheelAngle, targetFrontWheelAngle, 0.1f);

            foreach (var frontWheel in frontIndividualWheels)
            {
                var newEuler = frontWheel.localEulerAngles;
                newEuler.y = _frontWheelAngle;
                newEuler.z += rigidbodyMagnitude * wheelVelocity;
                frontWheel.localEulerAngles = newEuler;
            }
        }

        public virtual void SpinBackWheels(float rigidbodyMagnitude)
        {
            backWheels.localEulerAngles += new Vector3(0, 0, rigidbodyMagnitude * wheelVelocity);
        }

        public virtual void PointSteeringWheel(float horizontalSteer)
        {
            steeringWheel.localEulerAngles = new Vector3(-25, 90, horizontalSteer * 45);
        }

        public virtual void PointTowardsTurn(float horizontalSteer)
        {
            transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, 
                new Vector3(0, 90 + (horizontalSteer * extraBodyAngle), transform.localEulerAngles.z), .05f);
            
        }

        public virtual Vector3 ApplyToCoordinates(Axes axes, float value)
        {
            Vector3 applied = new Vector3(0f, 0f, 0f);

            switch (axes)
            {
                case Axes.X:
                    applied.x = value;
                    break;
                case Axes.Y:
                    applied.y = value;
                    break;
                case Axes.Z:
                    applied.z = value;
                    break;
            }
            
            return applied;
        }
        
        public virtual void WeightTransfer(float speedDelta, float currentRotate, float velocityMagnitude)
        {
            
            var targetPitch = speedDelta * speedFactor;
            var targetRoll = -currentRotate * bodyRollFactor * (velocityMagnitude * 0.03f);

            
            if (invertBodyPitch) targetPitch = -targetPitch;
            
            _bodyPitch = Mathf.Lerp(_bodyPitch, targetPitch, 0.05f);
            _bodyRoll = Mathf.Lerp(_bodyRoll, targetRoll, 0.035f);

            Vector3 bodyPitchVector = ApplyToCoordinates(bodyPitchAxis, -_bodyPitch);
            Vector3 bodyRollVector = ApplyToCoordinates(bodyRollAxis, _bodyRoll);
            
            Vector3 targetVector = bodyPitchVector+bodyRollVector;

            kartChassis.localEulerAngles = targetVector;
        }

        public virtual void SetDrifting(bool drifting)
        {
            _drifting = drifting;
        }
    }
}

