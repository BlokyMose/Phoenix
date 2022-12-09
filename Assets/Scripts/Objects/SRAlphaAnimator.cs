using Encore.Utility;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Phoenix
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SRAlphaAnimator : MonoBehaviour
    {
        [Serializable]
        public class AnimationProperties
        {
            [SerializeField]
            float duration = 1f;
            public float Duration => durationRandomRange > 0f
                ? Random.Range(duration - durationRandomRange /2, durationRandomRange + durationRandomRange/2)
                : duration;

            [SerializeField, Tooltip("Randomize duration by adding or substracting half of this value")]
            float durationRandomRange = -1f;

            [SerializeField, Range(0f,1f)]
            float toAlpha = 1f;
            public float ToAlpha => toAlpha;


            public AnimationProperties(float duration, float toAlpha)
            {
                this.duration = duration;
                this.toAlpha = toAlpha;
            }
        }

        [SerializeField]
        AnimationProperties inAnimation;

        [SerializeField]
        AnimationProperties outAnimation;

        SpriteRenderer sr;

        void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
        }

        void Start()
        {
            StartCoroutine(Update());
            IEnumerator Update()
            {
                var time = 0f;
                var currentAnimation = inAnimation;
                var currentCurve = AnimationCurve.EaseInOut(0, sr.color.a, currentAnimation.Duration, currentAnimation.ToAlpha);

                while (true)
                {
                    sr.color = sr.color.ChangeAlpha(currentCurve.Evaluate(time));
                    
                    time += Time.deltaTime;
                    if (time > currentAnimation.Duration)
                    {
                        currentAnimation = currentAnimation == inAnimation ? outAnimation : inAnimation;
                        currentCurve = AnimationCurve.EaseInOut(0, sr.color.a, currentAnimation.Duration, currentAnimation.ToAlpha);
                        time = 0f;
                    }

                    yield return null;
                }
            }

        }
    }
}
