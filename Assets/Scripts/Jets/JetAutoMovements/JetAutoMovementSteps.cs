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
            [Flags]
            public enum Direction 
            { 
                None = 0,
                Up = 1, 
                Down = 2, 
                Right = 4, 
                Left = 8, 
                Custom = 16 
            }
            public enum StopAxis { XY, X, Y }


            [HorizontalGroup("1",0.35f), SerializeField, LabelWidth(0.1f), GUIColor(nameof(GetModeColor))]
            MoveMode mode;
            public MoveMode Mode => mode;

            #region [Mode Options]
            
            [HorizontalGroup("1",0.35f), SerializeField, LabelWidth(0.1f), ShowIf("@" + nameof(mode) + "==" + nameof(MoveMode) + "." + nameof(MoveMode.MoveRelative))]
            Direction directionRelative;

            [HorizontalGroup("1",0.35f), SerializeField, LabelWidth(0.1f), ShowIf("@" + nameof(mode) + "==" + nameof(MoveMode) + "." + nameof(MoveMode.MoveAbsolute))]
            Direction directionAbsolute;

            [HorizontalGroup("1", 55f), SerializeField, LabelWidth(0.1f), ShowIf(nameof(ShowDirectionCustomField)), SuffixLabel("deg", true)]
            float directionCustomAngle = 90;

            [HorizontalGroup("1",0.35f), SerializeField, LabelWidth(0.1f), ShowIf("@" + nameof(mode) + "==" + nameof(MoveMode) + "." + nameof(MoveMode.Stop))]
            StopAxis stopAxis;

            #endregion

            [HorizontalGroup("2", 55f), SerializeField, LabelWidth(0.1f), SuffixLabel("sec", true)]
            float duration = 1f;

            [HorizontalGroup("2"), SerializeField, LabelWidth(0.1f), SuffixLabel("strength", true)]
            float strength = 1f;


            public float Duration => duration;

            public Vector2 GetDirectionVector(Transform relativeTransform)
            {
                var direction = Vector2.zero;
                switch (mode)
                {
                    case MoveMode.MoveRelative:
                        if (directionRelative.HasFlag(Direction.Up))
                            direction += (Vector2) (relativeTransform.up); 
                        if (directionRelative.HasFlag(Direction.Down))
                            direction += (Vector2) (-relativeTransform.up);
                        if (directionRelative.HasFlag(Direction.Right))
                            direction += (Vector2) (relativeTransform.right);
                        if (directionRelative.HasFlag(Direction.Left))
                            direction += (Vector2) (-relativeTransform.right);
                        if (directionRelative.HasFlag(Direction.Custom))
                            direction += (directionCustomAngle + relativeTransform.localEulerAngles.z).ToVector2();
                        break;

                    case MoveMode.MoveAbsolute:
                        if (directionAbsolute.HasFlag(Direction.Up))
                            direction += Vector2.up;
                        if (directionAbsolute.HasFlag(Direction.Down))
                            direction += Vector2.down;
                        if (directionAbsolute.HasFlag(Direction.Right))
                            direction += Vector2.right;
                        if (directionAbsolute.HasFlag(Direction.Left))
                            direction += Vector2.left;
                        if (directionAbsolute.HasFlag(Direction.Custom))
                            direction += directionCustomAngle.ToVector2();
                        break;

                    case MoveMode.Stop:
                        direction = Vector2.zero; 
                        break;

                    case MoveMode.None:
                        direction = Vector2.zero; 
                        break;
                }

                return direction * strength;
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


            #region [Methods: Inspector]

            bool ShowDirectionCustomField()
            {
                return 
                    (mode == MoveMode.MoveRelative && directionRelative.HasFlag(Direction.Custom)) ||
                    (mode == MoveMode.MoveAbsolute && directionAbsolute.HasFlag(Direction.Custom));
            }

            Color GetModeColor()
            {
                return mode switch
                {
                    MoveMode.MoveRelative => Encore.Utility.ColorUtility.paleGreen,
                    MoveMode.MoveAbsolute => Encore.Utility.ColorUtility.paleGreen,
                    MoveMode.Stop => Encore.Utility.ColorUtility.salmon,
                    _ => Color.gray
                };
            }

            #endregion

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


        public override void ModifyJetControllerAuto(JetControllerAuto jetController, out object cachedData)
        {
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
                    direction = Vector2.zero;
                    break;
                }
                else if (step.Duration <= 0)
                {
                    direction += step.GetDirectionVector(jetController.transform);
                }
                else if (timeClamped < step.Duration)
                {
                    direction += step.GetDirectionVector(jetController.transform);
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
