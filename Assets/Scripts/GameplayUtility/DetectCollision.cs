using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Phoenix
{
    public class DetectCollision : MonoBehaviour
    {
        public enum AfterCollision { None, Disabled }

        [SerializeField]
        AfterCollision afterCollision;

        [SerializeField]
        UnityEvent onCollision;


        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            onCollision.Invoke();

            switch (afterCollision)
            {
                case AfterCollision.Disabled:
                    enabled = false;
                    break;
            }
        }
    }
}
