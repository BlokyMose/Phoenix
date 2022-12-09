using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    public class LevelObjectActivator : MonoBehaviour
    {
        [SerializeField, LabelText("Deactivate Self"), Tooltip("Prevent initalization before LevelManager is intialized")]
        bool isDeactivateSelfAtAwake = false;
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
        }

        public void NextStage()
        {
            onNextStage?.Invoke();
        }

    }
}
