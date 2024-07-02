using System;
using System.Linq;
using Car;
using Domain;
using UnityEngine;

namespace AI
{
    public class AIInput : InputCreator
    {
        [SerializeField] private Transform aiPath;
        
        private CarController _controller;
        private Transform[] _trackPoints;
        private float _forward = 1f;
        private float _horizontal = 0f;
        private bool _drifting = false;
        
        private int _trackPointId;
        private float _minDistanceToTrackPoint = 15f;
        private float _lastDistance;
        private bool _changedTrackPoint;
        private float _minDifferenceDistance = 0.001f;
        private int _carStuckCounter;
        private int _maxCarStuckCounter = 10;
        private bool _isCarStuck;
        private bool _canMove;
        
        public override float Horizontal => _horizontal;
        public override float Forward => _forward;
        public override bool IsDrifting => _drifting;
        
        private void Start()
        {
            _trackPoints = aiPath.GetComponentsInChildren<Transform>().Skip(1).ToArray();
            _trackPointId = 1;
        }

        private void FixedUpdate()
        {
            if(!_canMove)
            {
                if(_controller.CurrentSpeed > 0)
                {
                    _forward = -1f;
                }
                else
                {
                    _forward = 0f;
                }
            }

            if (!_controller.CanMove)
                return;
            
            if (!_canMove)
                return;
            
            if (_isCarStuck) return;
            
            var currentForward = _controller.transform.forward;
            var toNextPoint = _trackPoints[_trackPointId].position - _controller.transform.position;
            currentForward.Normalize();
            toNextPoint.Normalize();
            
            var angle = Vector3.SignedAngle(currentForward, toNextPoint, Vector3.up);
            
            /*Debug.DrawRay(_controller.transform.position, currentForward * 5, Color.blue);
            Debug.DrawRay(_controller.transform.position, toNextPoint * 5, Color.red);*/
            
            _horizontal = Mathf.Clamp(angle / 45f, -1f, 1f);

            if (angle > 45f || angle < -45f)
            {
                _drifting = true;
            }
            else
            {
                _drifting = false;
            }
            
            var distance = Vector3.Distance(_controller.transform.position, _trackPoints[_trackPointId].position);

            if (distance < _minDistanceToTrackPoint)
            {
                _trackPointId++;
                _trackPointId %= _trackPoints.Length;
                _changedTrackPoint = true;
            }
            else if (!_isCarStuck)
            {
                var difference = _lastDistance - distance;
                if (!_changedTrackPoint && difference < _minDifferenceDistance && difference > -_minDifferenceDistance)
                {
                    _carStuckCounter++;
                    if (_carStuckCounter > _maxCarStuckCounter)
                    {
                        Debug.Log("Car is stuck");
                        _isCarStuck = true;
                        _forward = -1;
                        _horizontal = 0;
                        Invoke(nameof(SetCarStuckToFalse), 2f);
                    }
                }
                else
                {
                    _carStuckCounter = 0;
                }

                _changedTrackPoint = false;
            }
            _lastDistance = distance;
        }

        private void SetCarStuckToFalse()
        {
            Debug.Log("Car unstuck");
            _isCarStuck = false;
            _carStuckCounter = 0;
            _forward = 1;
        }

        public override void Initialize(CarController controller)
        {
            _controller = controller;
            _canMove = true;
        }

        public override void UnInitialize()
        {
            _canMove = false;
        }

        public override void Stop()
        {
            _forward = 0;
            _drifting = false;
            _horizontal = 0;
        }
    }
}