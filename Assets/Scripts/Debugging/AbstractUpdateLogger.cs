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

        private void DoPause()
        {
            Debug.Log("Lol Debug.Break is useless.");
            //Debug.Break();
        }

        [ServerCallback]
        public void Update()
        {
            if (logUpdate)
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

        [ServerCallback]
        public void FixedUpdate()
        {
            if (logFixed)
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

        [ServerCallback]
        public void LateUpdate()
        {
            if (logLate)
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