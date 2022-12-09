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


        void Awake()
        {
            if (invokeIn == InvokeInMode.Awake)
                Invoke();
        }

        void Start()
        {
            if (invokeIn == InvokeInMode.Start)
                Invoke();
        }

        void OnEnable()
        {
            if (invokeIn == InvokeInMode.OnEnable)
                Invoke();
        }

        void Invoke()
        {
            StartCoroutine(Delay());
            IEnumerator Delay()
            {
                yield return new WaitForSeconds(delay);
                unityEvent.Invoke();
            }
        }
    }
}
