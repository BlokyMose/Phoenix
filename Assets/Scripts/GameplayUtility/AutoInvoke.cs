using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Phoenix
{
    public class AutoInvoke : MonoBehaviour
    {
        [SerializeField]
        float delay = 0;

        [SerializeField]
        UnityEvent unityEvent;


        void Start()
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
