using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace Phoenix.JAM
{
    public class JAMStepMoveDuration : JAMStep
    {
        public enum Direction { Forward, Backward, Right, Left}

        [SerializeField]
        Direction direction;

        [SerializeField]
        float duration = 1f;

        public override float Duration => duration;

        public override Vector2 GetDirectionVector(Transform relativeTransform)
        {
            return direction switch
            {
                Direction.Forward => (Vector2)relativeTransform.up,
                Direction.Backward => (Vector2)(-relativeTransform.up),
                Direction.Right => (Vector2)relativeTransform.right,
                Direction.Left => (Vector2)(-relativeTransform.right),
                _ => Vector2.zero,
            };
        }

        private void OnValidate()
        {
            if (duration <= 0) duration = 1f;
        }
    }
}