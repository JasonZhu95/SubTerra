using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Utilities.GO
{
    public class Bobber : MonoBehaviour
    {
        [SerializeField] private float yOffset = 1;
        [SerializeField] private float bobDuration;
        [SerializeField] private float stopDuration;
        [SerializeField] private AnimationCurve bobCurve;
        [SerializeField] private AnimationCurve stopCurve;

        private Vector3 startingPos;
        private Vector3 pos;
        private float startTime;
        private float stopTime;
        private float offsetAtStop;

        private bool bob;
        private bool stopBob;

        private void Awake()
        {
            startingPos = transform.localPosition;
        }

        public void StartBobbing()
        {
            stopBob = false;
            bob = true;
            startTime = Time.time;
        }

        public void StopBobbing()
        {
            stopBob = true;
            stopTime = Time.time;
            offsetAtStop = transform.localPosition.y;
        }

        private void Update()
        {
            if (!bob) return;

            if (stopBob)
            {
                var eval = (Time.time - stopTime) / (stopDuration);
                var evalVal = stopCurve.Evaluate(eval);
                pos.Set(startingPos.x, startingPos.y + (offsetAtStop * evalVal), startingPos.z);
                transform.localPosition = pos;
                if (Time.time >= stopTime + stopDuration)
                {
                    bob = false;
                }
            }
            else
            {
                var evaluationPoint = (Time.time - startTime) / (bobDuration / 2);
                var evaluatedValue = bobCurve.Evaluate(evaluationPoint);
                pos.Set(startingPos.x, startingPos.y + (yOffset * evaluatedValue), startingPos.y);
                transform.localPosition = pos;
            }
        }
    }
}