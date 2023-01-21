using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

namespace Phoenix
{
    [InlineEditor]
    public class LevelObjectActivator : MonoBehaviour
    {
        public enum NextStageMode { Deactivate, AsIs }

        #if UNITY_EDITOR
        public enum DebugMode { Off, DontActivate }
        [SerializeField]
        DebugMode debugMode;
        #endif

        [SerializeField, LabelText("Auto-Deactivate"), Tooltip("Prevent initalization before LevelManager is intialized")]
        bool isDeactivateSelfAtAwake = true;

        [SerializeField]
        NextStageMode nextStage;
        

        [SerializeField]
        float delayActivation = 0f;
        public float DelayActivation => delayActivation;

        [SerializeField]
        float delayDeactivation = 0f;
        public float DelayDeactivation => delayDeactivation;

        public UnityEvent onActivated;


        bool isInitialized = false;

        Action onNextStage;

        void Awake()
        {
            if(isDeactivateSelfAtAwake && !isInitialized)
                Activate(false);

        }

        public void Init(Action onNextStage)
        {
            this.onNextStage += onNextStage;    
            isInitialized = true;
            Activate(false);
        }

        public void Activate(bool isActive)
        {
            #if UNITY_EDITOR
            if (debugMode == DebugMode.DontActivate && isActive) return;
            #endif

            gameObject.SetActive(isActive);
            if (isActive)
                onActivated.Invoke();
        }

        public void EndStage()
        {
            switch (nextStage)
            {
                case NextStageMode.Deactivate:
                    Activate(false);
                    break;
                case NextStageMode.AsIs:
                    break;
            }
        }

        public void NextStage()
        {
            onNextStage?.Invoke();
        }

    }
}
