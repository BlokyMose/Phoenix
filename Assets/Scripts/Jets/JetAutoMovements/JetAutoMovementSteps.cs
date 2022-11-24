using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Encore.Utility;
using Sirenix.OdinInspector;

namespace Phoenix.JAM
{
    [CreateAssetMenu(menuName = "SO/Jet Auto Movements/Steps", fileName = "JAM_Steps")]
    public class JetAutoMovementSteps : JetAutoMovement
    {
        [Serializable]
        public class Step
        {
            public enum MoveMode { MoveRelative, MoveAbsolute, Stop, None }
            public enum StopAxis { XY, X, Y }


            [HorizontalGroup(width: 0.35f), SerializeField, LabelWidth(0.1f)]
            MoveMode mode;
            public MoveMode Mode => mode;

            #region [Mode Options]

            [HorizontalGroup(width: 0.35f), SerializeField, LabelWidth(0.1f), ShowIf("@" + nameof(mode) + "==" + nameof(MoveMode) + "." + nameof(MoveMode.MoveRelative))]
            Direction4 directionRelative;

            [HorizontalGroup(width: 0.35f), SerializeField, LabelWidth(0.1f), ShowIf("@" + nameof(mode) + "==" + nameof(MoveMode) + "." + nameof(MoveMode.MoveAbsolute))]
            Direction4 directionAbsolute;

            [HorizontalGroup(width: 0.35f), SerializeField, LabelWidth(0.1f), ShowIf("@" + nameof(mode) + "==" + nameof(MoveMode) + "." + nameof(MoveMode.Stop))]
            StopAxis stopAxis;

            #endregion

            [HorizontalGroup(width: 55f), SerializeField, LabelWidth(0.1f), SuffixLabel("sec", true), ValidateInput(nameof(ValidateDuration), "Must be more than 0")]
            float duration = 1f;

            public float Duration => duration;

            public Vector2 GetDirectionVector(Transform relativeTransform)
            {
                switch (mode)
                {
                    case MoveMode.MoveRelative:
                        switch (directionRelative)
                        {
                            case Direction4.Up:
                                return relativeTransform.up;
                            case Direction4.Down:
                                return -relativeTransform.up;
                            case Direction4.Right:
                                return relativeTransform.right;
                            case Direction4.Left:
                                return -relativeTransform.right;
                        }
                        break;

                    case MoveMode.MoveAbsolute:
                        switch (directionAbsolute)
                        {
                            case Direction4.Up:
                                return Vector2.up;
                            case Direction4.Down:
                                return Vector2.down;
                            case Direction4.Right:
                                return Vector2.right;
                            case Direction4.Left:
                                return Vector2.left;
                        }
                        break;

                    case MoveMode.Stop:
                        return Vector2.zero;

                    case MoveMode.None:
                        return Vector2.zero;
                }

                return Vector2.zero;
            }

            public void StopRigidBody(Rigidbody2D rb)
            {
                switch (stopAxis)
                {
                    case StopAxis.XY: rb.velocity = Vector2.zero;
                        break;
                    case StopAxis.X:
                        rb.velocity = new Vector2(0, rb.velocity.y);
                        break;
                    case StopAxis.Y:
                        rb.velocity = new Vector2(rb.velocity.x, 0);
                        break;
                }
            }

            bool ValidateDuration(float duration)
            {
                return duration > 0f;
            }
        }

        public class MoveData
        {
            float totalDuration;
            public float TotalDuration => totalDuration;

            public MoveData(List<Step> steps)
            {
                foreach (var step in steps)
                    this.totalDuration += step.Duration;
            }
        }

        [SerializeField]
        List<Step> steps = new List<Step>();



        public override void ModifyJetControllerAuto(JetControllerAuto jetController, out float reduceVelocityMultipler, out object cachedData)
        {
            reduceVelocityMultipler = 0f;
            cachedData = new MoveData(steps);
        }

        public override Vector2 GetMoveDirection(JetControllerAuto jetController, float time, object cachedData)
        {
            var moveData = cachedData as MoveData;
            var timeClamped = time % moveData.TotalDuration;
            var direction = Vector2.zero;
            foreach (var step in steps)
            {
                if (step.Mode == Step.MoveMode.Stop && timeClamped < step.Duration)
                {
                    step.StopRigidBody(jetController.RB);
                    break;
                }
                else if (timeClamped < step.Duration)
                {
                    direction = step.GetDirectionVector(jetController.transform);
                    break;
                }
                else
                {
                    timeClamped -= step.Duration;
                }
            }

            return direction.normalized;
        }
       
    }
}
