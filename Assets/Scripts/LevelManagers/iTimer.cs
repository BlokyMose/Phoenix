using System.Collections;
using UnityEngine;

namespace Phoenix
{
    public interface iTimer 
    {
        public void Init();
        public float Duration { get; set; }
        public float TimeElapsed { get; set; }
        public float TimeRemaining { get; }
    }
}