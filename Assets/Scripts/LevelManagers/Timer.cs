using System.Collections;
using UnityEngine;

namespace Phoenix
{
    public abstract class Timer : MonoBehaviour
    {
        public abstract void Init();
        public virtual void Resume() => IsPaused = false;
        public virtual void Pause() => IsPaused = true;
        public abstract bool IsPaused { get; protected set; }
        public abstract float Duration { get; set; }
        public abstract float TimeElapsed { get; set; }
        public abstract float TimeRemaining { get; }
    }
}