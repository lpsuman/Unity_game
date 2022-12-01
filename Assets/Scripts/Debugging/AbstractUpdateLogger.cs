using System;
using UnityEngine;
using Mirror;

namespace Bluaniman.SpaceGame.Debugging
{
	public abstract class AbstractUpdateLogger : NetworkBehaviour
	{
        [Header("Update")]
		[SerializeField] private bool logUpdate = false;
        [SerializeField] private bool pauseAtUpdate = false;
        [Header("FixedUpdate")]
        [SerializeField] private bool logFixed = false;
        [SerializeField] private bool pauseAtFixed = false;
        [Header("LateUpdate")]
        [SerializeField] private bool logLate = false;
        [SerializeField] private bool pauseAtLate = false;

        public Transform optionalTransformToLog = null;

        protected const string updatePrefix = "Update";
        protected const string fixedPrefix = "Fixed";
        protected const string latePrefix = "Late";
        private readonly string suffix;

        protected AbstractUpdateLogger(string suffix)
        {
            this.suffix = suffix;
        }

        private void Start()
        {
            enabled = DebugHandler.ShouldDebug(DebugHandler.UpdateOrder());
        }

        private void DoLog(string prefix)
        {
            string msg = $"{prefix} {suffix}!";
            if (optionalTransformToLog != null)
            {
                msg += "\nTransform = {optionalTransformToLog.position}";
            }
            DebugHandler.NetworkLog(msg, this);
        }

        private void DoPause()
        {
            
        }

        public void Update()
        {
            if (logUpdate && DebugHandler.ShouldDebug(DebugHandler.UpdateOrder()))
            {
                DoLog(updatePrefix);
                OnUpdate();
                if (pauseAtUpdate)
                {
                    DoPause();
                }
            }
        }
        protected virtual void OnUpdate()
        {

        }

        public void FixedUpdate()
        {
            if (logFixed && DebugHandler.ShouldDebug(DebugHandler.UpdateOrder()))
            {
                DoLog(fixedPrefix);
                OnFixedUpdate();
                if (pauseAtFixed)
                {
                    DoPause();
                }
            }
        }

        protected virtual void OnFixedUpdate()
        {

        }

        public void LateUpdate()
        {
            if (logLate && DebugHandler.ShouldDebug(DebugHandler.UpdateOrder()))
            {
                DoLog(latePrefix);
                OnLateUpdate();
                if (pauseAtLate)
                {
                    DoPause();
                }
            }
        }

        protected virtual void OnLateUpdate()
        {

        }
    }
}