using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Phoenix
{
    [InlineEditor]
    public class LevelObjectActivator : MonoBehaviour
    {
        [SerializeField, LabelText("Auto-Deactivate"), Tooltip("Prevent initalization before LevelManager is intialized")]
        bool isDeactivateSelfAtAwake = true;

        [SerializeField]
        float delayActivation = 0f;
        public float DelayActivation => delayActivation;

        [SerializeField]
        float delayDeactivation = 0f;
        public float DelayDeactivation => delayDeactivation;

        [SerializeField]
        UnityEvent onActivated;


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
            gameObject.SetActive(isActive);
            if (isActive)
                onActivated.Invoke();
        }

        public void NextStage()
        {
            onNextStage?.Invoke();
        }

    }
}
