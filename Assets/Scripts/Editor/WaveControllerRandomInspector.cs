using Phoenix.Editor;
using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Phoenix
{
    [CustomEditor(typeof(WaveControllerRandom))]
    public class WaveControllerRandomInspector : OdinEditor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open Editor"))
            {
                WaveControllerEditor.OpenWindow(target as WaveController, serializedObject);
            }

            base.OnInspectorGUI();

        }
    }
}
