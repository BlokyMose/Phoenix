using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    public class LevelObjectActivator : MonoBehaviour
    {
        Action onNextStage;

        public void Init(Action onNextStage)
        {
            this.onNextStage += onNextStage;    
            gameObject.SetActive(false);
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
