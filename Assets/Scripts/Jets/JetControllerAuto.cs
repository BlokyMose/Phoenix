using Phoenix.JAM;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    public class JetControllerAuto : JetController
    {
        public struct InitialParameters
        {
            Vector2 direction;
            Vector2 position;

            public Vector2 Direction => direction;
            public Vector2 Position => position;

            public InitialParameters(Vector2 direction, Vector2 position)
            {
                this.direction = direction;
                this.position = position;
            }
        }

        [SerializeField]
        JetAutoMovement movement;

        InitialParameters initialParameters;
        public InitialParameters InitialParams => initialParameters;
        float time;
        object cachedData;
        protected override void OnEnable()
        {
            base.OnEnable();
            movement.ModifyJetControllerAuto(this, out cachedData);
            initialParameters = new InitialParameters(transform.localEulerAngles, transform.position);
        }

        protected override void FixedUpdate()
        {
            time += Time.deltaTime;
            Move(movement.GetMoveDirection(this, time, cachedData));
        }

    }
}
