using Encore.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    public class LoadingCanvas : MonoBehaviour
    {
        [SerializeField]
        Animator animator;

        [SerializeField]
        float inDuration = 1.25f;
        public float InDuration => inDuration;

        [SerializeField]
        float outDuration = 1.25f;
        public float OutDuration => outDuration;

        Coroutine corAnimating;
        int boo_loaded;

        public void Init(Action onCovered)
        {
            DontDestroyOnLoad(this);

            boo_loaded = Animator.StringToHash(nameof(boo_loaded));

            corAnimating = this.RestartCoroutine(Delay());
            IEnumerator Delay()
            {
                yield return new WaitForSeconds(inDuration);
                onCovered();
            }
        }

        public void Exit()
        {
            animator.SetBool(boo_loaded, true);
            Destroy(gameObject, outDuration);
        }
    }
}
