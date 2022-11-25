using System;
using UnityEngine;

namespace Bluaniman.SpaceGame.Debugging
{
	public abstract class AbstractUpdateLogger : MonoBehaviour
	{
		[SerializeField] private bool logUpdate = false;
        [SerializeField] private bool logFixed = false;
        [SerializeField] private bool logLate = false;
        public Transform optionalTransformToLog = null;

        protected const string updatePrefix = "Update";
        protected const string fixedPrefix = "Fixed";
        protected const string latePrefix = "Late";
        private readonly string suffix;

        protected AbstractUpdateLogger(string suffix)
        {
            this.suffix = suffix;
        }

        private void DoLog(string prefix)
        {
            if (optionalTransformToLog == null)
            {
                Debug.Log($"{prefix} {suffix}!");
            } else
            {
                Debug.Log($"{prefix} {suffix}!\nTransform = {optionalTransformToLog.position}");
            }
        }

        public void Update()
        {
            if (logUpdate)
            {
                DoLog(updatePrefix);
                OnUpdate();
            }
        }
        protected void OnUpdate()
        {

        }

        public void FixedUpdate()
        {
            if (logFixed)
            {
                DoLog(fixedPrefix);
                OnFixedUpdate();
            }
        }

        protected void OnFixedUpdate()
        {

        }

        public void LateUpdate()
        {
            if (logLate)
            {
                DoLog(latePrefix);
                OnLateUpdate();
            }
        }

        protected void OnLateUpdate()
        {

        }
    }
}