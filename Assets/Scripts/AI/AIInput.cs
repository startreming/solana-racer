﻿using System;
using System.Linq;
using Car;
using Domain;
using Kart;
using UnityEngine;

namespace AI
{
    public class AIInput : InputCreator
    {
        [SerializeField] private Transform aiPath;
        
        private CarController _controller;
        private Transform[] _trackPoints;
        private int _trackPointId;
        private float _minDistanceToTrackPoint = 5f;
        
        private float _forward = 1f;
        private float _horizontal = 0f;
        
        public override float Horizontal => GenerateHorizontal();
        public override float Forward => GenerateForward();

        public override bool IsDrifting => GenerateIsDrifting();

        private void Start()
        {
            _trackPoints = aiPath.GetComponentsInChildren<Transform>().Skip(1).ToArray();
            _trackPointId = 1;
        }

        private void Update()
        {
            var currentForward = _controller.transform.forward;
            var toNextPoint = _trackPoints[_trackPointId].position - _controller.transform.position;
            currentForward.Normalize();
            toNextPoint.Normalize();
            
            var angle = Vector3.SignedAngle(currentForward, toNextPoint, Vector3.up);
            
            /*Debug.DrawRay(_controller.transform.position, currentForward * 5, Color.blue);
            Debug.DrawRay(_controller.transform.position, toNextPoint * 5, Color.red);*/
            
            //_horizontal = Mathf.Clamp(angle / 90f, -1f, 1f);
            if (angle > 0.1f)
            {
                // Turn right
                _horizontal = 1f;
            }
            else if (angle < -0.1f)
            {
                // Turn left
                _horizontal = -1f;
            }
            else
            {
                _horizontal = 0f;
            }
            
            var distance = Vector3.Distance(_controller.transform.position, _trackPoints[_trackPointId].position);
            if (distance < _minDistanceToTrackPoint)
            {
                _trackPointId++;
                _trackPointId %= _trackPoints.Length;
            }
        }

        private float GenerateForward()
        {
            return _forward;
        }

        private float GenerateHorizontal()
        {
            return _horizontal;
        }

        private bool GenerateIsDrifting()
        {
            return false;
        }

        public override void Initialize(CarController controller)
        {
            _controller = controller;
        }

        public override void UnInitialize()
        {
            
        }
    }
}