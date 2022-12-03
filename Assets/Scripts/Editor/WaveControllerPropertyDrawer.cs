using Sirenix.OdinInspector.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Phoenix.Editor
{
    [CustomEditor(typeof(WaveController))]
    public class WaveControllerPropertyDrawer : OdinEditor
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
