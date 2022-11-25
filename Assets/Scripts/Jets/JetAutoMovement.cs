using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Phoenix.JetControllerAuto;

namespace Phoenix.JAM
{
    [InlineEditor]
    public abstract class JetAutoMovement : ScriptableObject
    {
        public virtual void ModifyJetControllerAuto(JetControllerAuto jetController, out object cachedData)
        {
            cachedData = null;
        }

        public abstract Vector2 GetMoveDirection(JetControllerAuto jetController, float time, object cachedData);
    }
}
