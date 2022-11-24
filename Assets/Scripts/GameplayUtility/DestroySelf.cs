using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    public class DestroySelf : MonoBehaviour
    {
        [SerializeField]
        float delay = 0f;

        public void Init(float delay)
        {
            this.delay = delay;
        }

        public void Invoke()
        {
            StartCoroutine(Delay());
            IEnumerator Delay()
            {
                yield return new WaitForSeconds(delay);
                Destroy(gameObject);
            }
        }
    }
}