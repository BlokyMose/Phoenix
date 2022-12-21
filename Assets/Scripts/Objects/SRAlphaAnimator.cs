using Encore.Utility;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Phoenix
{
    public class SRAlphaAnimator : MonoBehaviour
    {
        [Serializable]
        public class AnimationProperties
        {
            public enum RandomMode { Initial, Constant }

            [SerializeField]
            float duration = 1f;
            public float Duration 
            {
                get
                {
                    if (randomMode == RandomMode.Constant)
                        return durationRandomRange > 0f
                        ? Random.Range(duration - durationRandomRange /2, durationRandomRange + durationRandomRange/2)
                        : duration;
                    else
                        return duration;
                }
            }

            [SerializeField, Range(0f,1f)]
            float toAlpha = 1f;
            public float ToAlpha => toAlpha;

            [SerializeField]
            RandomMode randomMode = RandomMode.Initial;

            [SerializeField, Tooltip("Randomize duration by adding or substracting half of this value")]
            float durationRandomRange = -1f;


            public void Init()
            {
                if (randomMode == RandomMode.Initial)
                    duration = Random.Range(duration - durationRandomRange / 2, durationRandomRange + durationRandomRange / 2);
            }

            public AnimationProperties(float duration, float toAlpha)
            {
                this.duration = duration;
                this.toAlpha = toAlpha;
            }
        }

        [SerializeField, LabelText("SpriteRenderers")]
        List<SpriteRenderer> srs = new List<SpriteRenderer>();

        [SerializeField]
        AnimationProperties inAnimation;

        [SerializeField]
        AnimationProperties outAnimation;

        void Awake()
        {
            if (srs.Count == 0)
                srs.Add(GetComponent<SpriteRenderer>());

            inAnimation.Init();
            outAnimation.Init();
        }

        void Start()
        {
            StartCoroutine(Update());
            IEnumerator Update()
            {

                var time = 0f;
                var currentAnimation = inAnimation;
                var currentCurve = AnimationCurve.EaseInOut(0, srs[0].color.a, currentAnimation.Duration, currentAnimation.ToAlpha);

                while (true)
                {
                    foreach (var sr in srs)
                        sr.color = sr.color.ChangeAlpha(currentCurve.Evaluate(time));
                    
                    time += Time.deltaTime;
                    if (time > currentAnimation.Duration)
                    {
                        currentAnimation = currentAnimation == inAnimation ? outAnimation : inAnimation;
                        currentCurve = AnimationCurve.EaseInOut(0, srs[0].color.a, currentAnimation.Duration, currentAnimation.ToAlpha);
                        time = 0f;
                    }

                    yield return null;
                }
            }

        }
    }
}
