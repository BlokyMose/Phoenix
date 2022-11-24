using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Phoenix.JetControllerAuto;

namespace Phoenix.JAM
{
    [CreateAssetMenu(menuName = "SO/Jet Auto Movements/Wave", fileName = "JAM_Wave")]
    public class JetAutoMovementWave : JetAutoMovement
    {
        [SerializeField]
        float period;

        [SerializeField]
        float amplitude;

        public override void ModifyJetControllerAuto(JetControllerAuto jetController, out float reduceVelocityMultipler, out object cachedData)
        {
            reduceVelocityMultipler = 1f;
            cachedData = null;
        }

        public override Vector2 GetMoveDirection(JetControllerAuto jetController, float time, object cachedData)
        {
            var timeRatio = time / period % period;
            var inverse = 1;
            if (timeRatio > period * 0.25f && timeRatio < period * 0.75f)
                inverse = -1;
            var x = amplitude;
            var clampedRatio = timeRatio % 0.25f * 4f;
            var y = clampedRatio * amplitude * inverse;

            var direction = (jetController.InitialParams.Direction + new Vector2(x,y)).normalized;
            direction = new Vector2(1, direction.y);
            //Debug.Log(nameof(direction) + " : " + direction);
            return direction;
        }
    }
}
