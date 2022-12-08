using System.Collections;
using UnityEngine;

namespace Phoenix
{
    public abstract class JetProperties : ScriptableObject
    {
        public enum MoveMode { Smooth, Constant }

        public abstract GameObject JetPrefab { get; }
        public abstract MoveMode Mode { get;}
        public abstract float MoveSpeed { get; }
        public abstract float MaxVelocity { get; }
        public abstract float CursorSpeed { get; }
        public abstract float LinearDrag { get; }
        public abstract float RPS { get; }
        public abstract GameObject Cursor { get; }

        

    }
}