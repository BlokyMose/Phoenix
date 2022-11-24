using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix.JAM
{
    public class JAMStepNone : JAMStep
    {
        [SerializeField]
        float duration = 1f;

        public override float Duration => duration;

        public override Vector2 GetDirectionVector(Transform relativeTransform)=> Vector2.zero;

        private void OnValidate()
        {
            if (duration <= 0) duration = 1f;
        }

    }
}
