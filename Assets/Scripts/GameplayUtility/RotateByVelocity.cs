using Encore.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    public class RotateByVelocity : MonoBehaviour
    {
        Rigidbody2D rb;

        void Start()
        {
            rb = this.GetComponentInFamily<Rigidbody2D>();
        }

        void FixedUpdate()
        {
            if (rb != null)
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, rb.velocity.ToAngle());
        }
    }
}
