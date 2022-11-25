using Phoenix;
using System.Collections;
using UnityEngine;

namespace Encore.Utility
{
    public static class ComponentUtility
    {
        public static T GetComponentInFamily<T>(this Component thisComponent) where T : Component
        {
            var targetComponent = thisComponent.GetComponent<T>();
            targetComponent ??= thisComponent.GetComponentInParent<T>();
            targetComponent ??= thisComponent.GetComponentInChildren<T>();

            return targetComponent;
        }

    }
}