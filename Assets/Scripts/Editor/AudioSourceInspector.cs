//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;

//namespace Phoenix.Editor
//{
//    [CustomEditor(typeof(AudioSource), true)]
//    [CanEditMultipleObjects]
//    public class AudioSourceInspector : UnityEditor.Editor  
//    {
//        UnityEditor.Editor defaultEditor;

//        private void OnEnable()
//        {
//            if (target != null)
//                defaultEditor = UnityEditor.Editor.CreateEditor(target, Type.GetType("UnityEditor.AudioSourceInspector, UnityEditor"));
//        }

//        public override void OnInspectorGUI()
//        {
//            if (GUILayout.Button("Play"))
//            {
//                var audio = target as AudioSource;
//                audio.Play();
//            }

//            if (defaultEditor!=null)
//                defaultEditor.OnInspectorGUI();
//        }
//    }
//}
