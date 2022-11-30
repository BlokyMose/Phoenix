using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
            base.OnInspectorGUI();
            if (GUILayout.Button("Open Editor"))
            {
                WaveController waveController = (WaveController)target;
                WaveControllerEditor.OpenWindow(waveController, null);
            }
        }

    }
}
