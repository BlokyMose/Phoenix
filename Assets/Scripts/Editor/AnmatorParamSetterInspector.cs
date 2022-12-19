using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Phoenix
{
    [CustomEditor(typeof(AnimatorParamSetter))]
    public class AnmatorParamSetterInspector : OdinEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}
