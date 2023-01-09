using Encore.Utility;
using System;
using System.Collections;
using UnityEngine;

namespace Phoenix
{
    public class Timer : MonoBehaviour, iTimer
    {
        [SerializeField]
        float duration = 3f;

        float time;

        public float Duration
        {
            get => duration;
            set => duration = value;
        }

        public float TimeElapsed 
        {
            get => time;
            set => time = value;
        }

        public float TimeRemaining
        {
            get => duration - time;
        }

        public Action OnEnd;
        Coroutine corCounting;

        public void Init()
        {
            corCounting = this.RestartCoroutine(duration > 0f ? CountingUntil(duration) : CountingInfinitely());
            
            IEnumerator CountingUntil(float duration)
            {
                time = 0f;
                while (time < duration)
                {
                    time += Time.deltaTime * Time.timeScale;
                    yield return null;
                }
            }

            IEnumerator CountingInfinitely()
            {
                time = 0f;
                while (true)
                {
                    time += Time.deltaTime * Time.timeScale;
                    yield return null;
                }
            }
        }
    }
}