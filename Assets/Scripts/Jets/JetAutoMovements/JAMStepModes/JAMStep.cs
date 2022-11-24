using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix.JAM
{
    [InlineEditor(Expanded = true)]
    public abstract class JAMStep : ScriptableObject
    {
        public abstract float Duration { get; }
        public abstract Vector2 GetDirectionVector(Transform relativeTransform);
    }
}
