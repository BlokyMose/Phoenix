using Encore.Utility;
using FunkyCode.Rendering.Light;
using Sirenix.Serialization.Utilities.Editor;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Color = UnityEngine.Color;

namespace Phoenix.Editor
{
    public class WaveControllerEditor : EditorWindow
    {
        #region [Editor]

        [MenuItem("Tools/Wave Editor")]
        public static void OpenWindow()
        {
            GetWindow<WaveControllerEditor>("Wave").Show();
        }

        public static void OpenWindow(WaveController waveController, Action<List<WaveController.Spawner>> onSetNewSpawners)
        {
            var window = GetWindow<WaveControllerEditor>("Wave");
            window.waveController = waveController;
            window.OnSetNewSpawners = onSetNewSpawners;
            window.Show();
        }

        #endregion

        #region [Vars: Data Handlers]

        public Action<List<WaveController.Spawner>> OnSetNewSpawners;

        WaveController waveController;

        Vector2 scrollViewPos;

        readonly int rowPropertiesCount = 6;
        float rowLength => rowPropertiesCount * generalInfoFieldSize.Height + spaceBetweenRow;

        #endregion


        #region [Vars: Properties]

        readonly string iconsFolder = "Assets/Scripts/Editor/Icons/";
        Texture2D iconDelay, iconClock, iconCount, iconCopy, iconPaste, iconDelete;

        readonly Padding padding = new(10,10,10,10);
        readonly EditorPos defaultFieldSize = new EditorPos(100, 20);
        readonly EditorPos generalInfoFieldSize = new EditorPos(100, 20);
        readonly float spaceBetweenRow = 15f;
        readonly float spaceBetweenWaveColumn = 5f;

        Color originalColor;

        #endregion

        void OnEnable()
        {
            iconDelay = AssetDatabase.LoadAssetAtPath<Texture2D>(iconsFolder + "ic_delay.png");
            iconClock = AssetDatabase.LoadAssetAtPath<Texture2D>(iconsFolder + "ic_clock.png");
            iconCount = AssetDatabase.LoadAssetAtPath<Texture2D>(iconsFolder + "ic_count.png");
            iconCopy = AssetDatabase.LoadAssetAtPath<Texture2D>(iconsFolder + "ic_copy.png");
            iconPaste = AssetDatabase.LoadAssetAtPath<Texture2D>(iconsFolder + "ic_paste.png");
            iconDelete = AssetDatabase.LoadAssetAtPath<Texture2D>(iconsFolder + "ic_delete.png");
            originalColor = GUI.color;
        }



        void OnGUI()
        {
            #region [Begin ScrollView]

            var pos = new EditorPos(padding.left, padding.top);
            var windowSize = new Vector2(position.width - padding.Horizontal, position.height - padding.Vertical);
            var scrollViewRect = new Rect(pos.Vector2, windowSize);
            var viewRect = new Rect(Vector2.zero, GetContentViewSize(waveController.Spawners));
            scrollViewPos = GUI.BeginScrollView(scrollViewRect, scrollViewPos, viewRect);

            #endregion

            MakeSpawnerGeneralInfosColumn(pos.Vector2);
            MakeWavesView(new Vector2(generalInfoFieldSize.Width + 20, pos.Height));

            GUI.EndScrollView();
            Undo.RecordObject(this, "Wave");

            void MakeSpawnerGeneralInfosColumn(Vector2 originPos)
            {
                var rowPos = new EditorPos(originPos);
                foreach (var spawner in waveController.Spawners)
                    rowPos.AddRow(MakeGeneralInfo(spawner, rowPos.Vector2));

                float MakeGeneralInfo(WaveController.Spawner spawner, Vector2 originPos)
                {
                    var infoPos = new EditorPos(originPos);

                    var transformRect = new Rect(infoPos.Vector2, generalInfoFieldSize.Vector2);
                    Transform transform = null;
                    transform = (Transform)EditorGUI.ObjectField(transformRect, spawner.Position, typeof(Transform), true);
                    infoPos.AddRow(transformRect.height);

                    var isLoopRect = new Rect(infoPos.Vector2, generalInfoFieldSize.Vector2);
                    bool isLoop = false;
                    isLoop = EditorGUI.Toggle(isLoopRect, spawner.IsLoop);
                    infoPos.AddRow(isLoopRect.height);

                    return rowLength;
                }

            }

            void MakeWavesView(Vector2 originPos)
            {
                var viewPos = new EditorPos(originPos);
                foreach (var spawner in waveController.Spawners)
                    viewPos.AddRow(MakeWaveColumns(spawner, viewPos.Vector2));

                float MakeWaveColumns(WaveController.Spawner spawner, Vector2 originPos)
                {
                    var colPos = new EditorPos(originPos);
                    foreach (var wave in spawner.Waves)
                        colPos.AddColumn(MakeWave(wave, colPos.Vector2) + spaceBetweenWaveColumn);

                    return rowLength;

                    float MakeWave(WaveProperties wave, Vector2 originPos)
                    {
                        var wavePos = new EditorPos(originPos);

                        wavePos.AddRow(MakeSOName(wavePos).height);
                        wavePos.AddRow(MakePrefab(wavePos, wave.EnemyPrefab).height);
                        wavePos.AddRow(MakeDelay(wavePos, wave.Delay).height);
                        wavePos.AddRow(MakePeriod(wavePos, wave.Period).height);
                        wavePos.AddRow(MakeCount(wavePos, wave.Count).height);
                        wavePos.AddRow(MakeSOControls(wavePos).height);

                        return defaultFieldSize.Width;

                        Rect MakeSOName(EditorPos pos)
                        {
                            var nameRect = new Rect(pos.Vector2, defaultFieldSize.Vector2);
                            wave.name = EditorGUI.TextField(nameRect, wave.SOName);
                            return nameRect;
                        }

                        Rect MakePrefab(EditorPos pos, GameObject prefab)
                        {
                            var prefabRect = new Rect(wavePos.Vector2, defaultFieldSize.Vector2);
                            var newPrefab = EditorGUI.ObjectField(prefabRect, prefab, typeof(GameObject), true);
                            return prefabRect;
                        }

                        Rect MakeDelay(EditorPos pos, float delay)
                        {
                            var thisPos = new EditorPos(pos.Vector2);

                            var iconRect = MakeIcon(thisPos, iconDelay, "Delay");
                            thisPos.AddColumn(iconRect.width);

                            var delayRect = new Rect(thisPos.Vector2, new Vector2(defaultFieldSize.Width - iconRect.width, defaultFieldSize.Height));
                            var newDelay = EditorGUI.FloatField(delayRect, delay);
                            thisPos.AddColumn(delayRect.width);

                            var addSubRect = MakeAddSubButtons(thisPos, ref delay, 0.1f, false);

                            return new Rect(pos.Width, pos.Height, delayRect.width + addSubRect.width, delayRect.height);
                        }

                        Rect MakePeriod(EditorPos pos, float period)
                        {
                            var thisPos = new EditorPos(pos.Vector2);

                            var iconRect = MakeIcon(thisPos, iconClock, "Period");
                            thisPos.AddColumn(iconRect.width);

                            var delayRect = new Rect(thisPos.Vector2, new Vector2(defaultFieldSize.Width - iconRect.width, defaultFieldSize.Height));
                            var newDelay = EditorGUI.FloatField(delayRect, period);
                            thisPos.AddColumn(delayRect.width);

                            var addSubRect = MakeAddSubButtons(thisPos, ref period, 0.1f, false);

                            return new Rect(pos.Width, pos.Height, delayRect.width + addSubRect.width, delayRect.height);
                        }

                        Rect MakeCount(EditorPos pos, int count)
                        {
                            var thisPos = new EditorPos(pos.Vector2);

                            var iconRect = MakeIcon(thisPos, iconCount, "Count");
                            thisPos.AddColumn(iconRect.width);

                            var delayRect = new Rect(thisPos.Vector2, new Vector2(defaultFieldSize.Width - iconRect.width, defaultFieldSize.Height));
                            var newDelay = EditorGUI.FloatField(delayRect, count);
                            thisPos.AddColumn(delayRect.width);

                            var addSubRect = MakeAddSubButtons(thisPos, ref count, 1, false);

                            return new Rect(pos.Width, pos.Height, delayRect.width + addSubRect.width, delayRect.height);
                        }

                        Rect MakeSOControls(EditorPos pos)
                        {
                            var thisPos = new EditorPos(pos.Vector2);
                            var buttonWidth = 25f;

                            var copyRect = new Rect(thisPos.Width, thisPos.Height, buttonWidth, defaultFieldSize.Height);
                            if (GUI.Button(copyRect, new GUIContent(iconCopy, "Copy")))
                            {

                            }
                            thisPos.AddColumn(copyRect.width);

                            var pasteRect = new Rect(thisPos.Width, thisPos.Height, buttonWidth, defaultFieldSize.Height);
                            if (GUI.Button(pasteRect, new GUIContent(iconPaste, "Paste")))
                            {

                            }
                            thisPos.AddColumn(pasteRect.width);

                            var deleteRect = new Rect(thisPos.Width, thisPos.Height, buttonWidth, defaultFieldSize.Height);
                            GUI.color = Encore.Utility.ColorUtility.salmon;
                            if (GUI.Button(deleteRect, new GUIContent(iconDelete, "Delete")))
                            {

                            }
                            GUI.color = originalColor;
                            thisPos.AddColumn(deleteRect.width);

                            return new Rect(pos.Width, pos.Height, copyRect.width + pasteRect.width + deleteRect.width, copyRect.height);

                        }
                    }
                }
            }

            Vector2 GetContentViewSize(List<WaveController.Spawner> spawners)
            {
                var height = rowLength * spawners.Count;
                var mostWaveCount = 0;
                foreach (var spawner in spawners)
                    if (spawner.Waves.Count > mostWaveCount)
                        mostWaveCount = spawner.Waves.Count;

                var width = mostWaveCount * defaultFieldSize.Width + generalInfoFieldSize.Width;

                return new Vector2(width, height);
            }


        }

        Rect MakeIcon(EditorPos pos, Texture2D icon, string tooltip)
        {
            var rect = new Rect(pos.Width, pos.Height, 15f, defaultFieldSize.Height);
            GUI.color = Color.white.ChangeAlpha(0.5f);
            EditorGUI.LabelField(rect, new GUIContent(icon, tooltip));
            GUI.color = originalColor;

            rect.width += 5f;
            return rect;
        }

        Rect MakeAddSubButtons(EditorPos pos, ref int value, int increment, bool isPositionedAfter = true)
        {
            var buttonWidth = 20f;

            var thisPos = new EditorPos(pos.Vector2);
            if (!isPositionedAfter)
                thisPos.AddColumn(-buttonWidth * 2);

            var addRect = new Rect(thisPos.Width, thisPos.Height, buttonWidth, defaultFieldSize.Height);

            if (GUI.Button(addRect, "+"))
                value += increment;
            thisPos.AddColumn(addRect.width);

            var subRect = new Rect(thisPos.Width, thisPos.Height, buttonWidth, defaultFieldSize.Height);
            if (GUI.Button(subRect, "-"))
                value -= increment;
            thisPos.AddColumn(subRect.width);

            return new Rect(pos.Width, pos.Height, addRect.width + subRect.width, addRect.height);
        }

        Rect MakeAddSubButtons(EditorPos pos, ref float value, float increment, bool isPositionedAfter = true)
        {
            var buttonWidth = 20f;

            var thisPos = new EditorPos(pos.Vector2);
            if (!isPositionedAfter)
                thisPos.AddColumn(-buttonWidth * 2);

            var addRect = new Rect(thisPos.Width, thisPos.Height, buttonWidth, defaultFieldSize.Height);

            if(GUI.Button(addRect, "+"))
                value += increment;
            thisPos.AddColumn(addRect.width);

            var subRect = new Rect(thisPos.Width, thisPos.Height, buttonWidth, defaultFieldSize.Height);
            if (GUI.Button(subRect, "-"))
                value -= increment;
            thisPos.AddColumn(subRect.width);

            return new Rect(pos.Width, pos.Height, addRect.width + subRect.width, addRect.height);
        }
    }
}
