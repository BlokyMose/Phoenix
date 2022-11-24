using System.Collections;
using UnityEngine;

namespace Phoenix.JAM
{
    public class JAMStepStop : JAMStep
    {
        public enum Axis { XY, X, Y }

        [SerializeField]
        Axis axis;

        public override float Duration => 0f;

        public override Vector2 GetDirectionVector(Transform relativeTransform)
        {
            return Vector2.zero;
        }
    }
}