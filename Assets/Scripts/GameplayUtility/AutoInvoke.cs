using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Phoenix
{
    public class AutoInvoke : MonoBehaviour
    {
        public enum InvokeInMode { Awake, Start, OnEnable }

        [SerializeField]
        InvokeInMode invokeIn = InvokeInMode.Awake;

        [SerializeField]
        float delay = 0;

        [SerializeField]
        UnityEvent unityEvent;


        protected virtual void Awake()
        {
            if (invokeIn == InvokeInMode.Awake)
                Invoke();
        }

        protected virtual void Start()
        {
            if (invokeIn == InvokeInMode.Start)
                Invoke();
        }

        protected virtual void OnEnable()
        {
            if (invokeIn == InvokeInMode.OnEnable)
                Invoke();
        }

        void Invoke()
        {
            StartCoroutine(Invoking());

        }

        protected virtual IEnumerator Invoking()
        {
            yield return new WaitForSeconds(delay);
            unityEvent.Invoke();
        }
    }
}
