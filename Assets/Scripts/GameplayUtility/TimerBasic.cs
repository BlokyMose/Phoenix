using Encore.Utility;
using System;
using System.Collections;
using UnityEngine;

namespace Phoenix
{
    public class TimerBasic : Timer
    {
        [SerializeField]
        float duration = 3f;

        float time;
        bool isPaused;

        public override float Duration
        {
            get => duration;
            set => duration = value;
        }

        public override float TimeElapsed 
        {
            get => time;
            set => time = value;
        }

        public override float TimeRemaining
        {
            get => duration - time;
        }
        public override bool IsPaused { get => isPaused; protected set => isPaused = value; }

        public Action OnEnd;
        Coroutine corCounting;

        public override void Init()
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

        public override void Resume()
        {
            throw new NotImplementedException();
        }

        public override void Pause()
        {
            throw new NotImplementedException();
        }
    }
}