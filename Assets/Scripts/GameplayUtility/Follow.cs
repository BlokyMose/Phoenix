using Encore.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    public class Follow : MonoBehaviour
    {
        [SerializeField]
        Transform target;

        FollowMode followMode;

        public void Init(FollowMode followMode, Transform target)
        {
            this.followMode = followMode;
            this.target = target;
        }

        void Update()
        {
            if (target == null)
                return;

            if (followMode.HasFlag(FollowMode.Position))
                transform.position = target.position;
            if (followMode.HasFlag(FollowMode.Rotation))
                transform.localEulerAngles = target.localEulerAngles;
            if (followMode.HasFlag(FollowMode.Scale))
                transform.localScale = target.localScale;
        }
    }
}
