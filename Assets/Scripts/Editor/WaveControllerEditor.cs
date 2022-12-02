using Encore.Utility;
using System;
using System.Collections.Generic;
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

        public static void OpenWindow(WaveController waveController, SerializedObject serializedObject)
        {
            var window = GetWindow<WaveControllerEditor>("Wave");
            window.waveController = waveController;
            window.waveColors = new WaveColors(waveController);
            window.Show();
        }

        #endregion


        #region [Classes]

        class CopiedWave
        {
            public WaveProperties wave;
            public int spawnerIndex;
            public int waveIndex;

            public CopiedWave(WaveProperties wave, int spawnerIndex, int waveIndex)
            {
                this.wave = wave;
                this.spawnerIndex = spawnerIndex;
                this.waveIndex = waveIndex;
            }
        }

        class WaveColors
        {
            public class WaveColor
            {
                public WaveProperties wave;
                public Color color;

                public WaveColor(WaveProperties wave, Color color)
                {
                    this.wave = wave;
                    this.color = color;
                }
            }

            public List<WaveColor> waveColors = new List<WaveColor>();
            readonly List<Color> randomColors = new List<Color>()
            {
                Encore.Utility.ColorUtility.aqua,
                Encore.Utility.ColorUtility.orange,
                Encore.Utility.ColorUtility.dodgerBlue,
                Encore.Utility.ColorUtility.violet,
                Encore.Utility.ColorUtility.mediumSpringGreen,
                Encore.Utility.ColorUtility.najavoWhite,
                Encore.Utility.ColorUtility.lightSalmon,
                Encore.Utility.ColorUtility.lime,
                Encore.Utility.ColorUtility.cadetBlue,
            };
            int currentRandomColorIndex = -1;
            Color randomColor
            {
                get
                {
                    currentRandomColorIndex = (currentRandomColorIndex + 1) % randomColors.Count;
                    return randomColors[currentRandomColorIndex];
                }
            }

            WaveController waveController;
            public WaveController WaveController => waveController;

            public WaveColors()
            {
                this.waveColors = new List<WaveColor>();
            }

            public WaveColors(WaveController waveController)
            {
                this.waveController = waveController;
                this.waveColors = new List<WaveColor>();
                foreach (var spawner in waveController.Spawners)
                {
                    foreach (var wave in spawner.Waves)
                    {
                        bool alreadyAdded = false;
                        foreach (var waveColor in waveColors)
                        {
                            if (waveColor.wave == wave)
                            {
                                alreadyAdded = true;
                                break;
                            }
                        }
                        if (!alreadyAdded)
                        {
                            waveColors.Add(new WaveColor(wave, randomColor));
                        }
                    }
                }
            }



            public bool HasWave(WaveProperties wave)
            {
                foreach (var waveColor in waveColors)
                    if (waveColor.wave == wave)
                        return true;

                return false;
            }

            public Color GetColorOf(WaveProperties wave)
            {
                foreach (var waveColor in waveColors)
                    if (waveColor.wave == wave)
                        return waveColor.color;

                return Color.gray;
            }

            public void AddWaveColor(WaveProperties wave)
            {
                waveColors.Add(new WaveColor(wave, randomColor));
            }





            public void Reset()
            {
                waveColors = new List<WaveColor>();
                currentRandomColorIndex = -1;
            }

        }


        class EditorColors
        {
            public Color color;
            public Color content;
            public Color background;
            public EditorColors()
            {

            }

            public EditorColors(Color color, Color content, Color background)
            {
                this.color = color;
                this.content = content;
                this.background = background;
            }
        }

        #endregion

        #region [Vars: Properties]

        readonly int wavePropertiesRowCount = 6;
        readonly Padding padding = new(10,10,10,10);
        readonly EditorPos defaultFieldSize = new EditorPos(100, 20);
        readonly EditorPos generalInfoFieldSize = new EditorPos(100, 20);
        readonly float spaceBetweenRow = 15f;
        readonly float spaceBetweenWaveColumn = 5f;
        WaveColors waveColors = new WaveColors();

        #endregion

        #region [Vars: Data Handlers]

        float rowLength => wavePropertiesRowCount * generalInfoFieldSize.Height + spaceBetweenRow;
        WaveController waveController;
        CopiedWave copiedWaveProperties;
        Vector2 scrollViewPos;
        readonly string iconsFolder = "Assets/Scripts/Editor/Icons/";
        Texture2D iconDelay, iconClock, iconCount, iconCopy, iconPaste, iconDelete, iconAdd;
        EditorColors guiColors = new EditorColors();

        #endregion

        void OnEnable()
        {
            iconDelay = AssetDatabase.LoadAssetAtPath<Texture2D>(iconsFolder + "ic_delay.png");
            iconClock = AssetDatabase.LoadAssetAtPath<Texture2D>(iconsFolder + "ic_clock.png");
            iconCount = AssetDatabase.LoadAssetAtPath<Texture2D>(iconsFolder + "ic_count.png");
            iconCopy = AssetDatabase.LoadAssetAtPath<Texture2D>(iconsFolder + "ic_copy.png");
            iconPaste = AssetDatabase.LoadAssetAtPath<Texture2D>(iconsFolder + "ic_paste.png");
            iconDelete = AssetDatabase.LoadAssetAtPath<Texture2D>(iconsFolder + "ic_delete.png");
            iconAdd = AssetDatabase.LoadAssetAtPath<Texture2D>(iconsFolder + "ic_add.png");
            guiColors = new EditorColors(GUI.color, GUI.contentColor, GUI.backgroundColor);

            EditorApplication.playModeStateChanged += (mode) =>
            {
                if(mode == PlayModeStateChange.EnteredEditMode)
                {
                    var waveCs = FindObjectsOfType<WaveController>();
                    foreach (var waveC in waveCs)
                        if (waveC.GetInstanceID() == waveController.GetInstanceID())
                            waveController = waveC;
                }

            };
        }

        void OnGUI()
        {
            var spawners = new List<WaveController.Spawner>(waveController.Spawners);
            if (waveColors.WaveController != waveController)
                waveColors.Reset();

            #region [Begin ScrollView]

            var pos = new EditorPos(padding.left, padding.top);
            var windowSize = new Vector2(position.width - padding.Horizontal, position.height - padding.Vertical);
            var scrollViewRect = new Rect(pos.Vector2, windowSize);
            var viewRect = new Rect(Vector2.zero, GetContentViewSize(spawners));
            scrollViewPos = GUI.BeginScrollView(scrollViewRect, scrollViewPos, viewRect);

            #endregion

            MakeSpawnerGeneralInfosColumn(pos.Vector2, spawners);
            MakeWavesView(new Vector2(generalInfoFieldSize.Width + 20, pos.Height), spawners);

            GUI.EndScrollView();
            Undo.RecordObject(this, "Wave");

            void MakeSpawnerGeneralInfosColumn(Vector2 originPos, List<WaveController.Spawner> spawners)
            {
                var rowPos = new EditorPos(originPos);
                foreach (var spawner in spawners)
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

            void MakeWavesView(Vector2 originPos, List<WaveController.Spawner> spawners)
            {
                var viewPos = new EditorPos(originPos);
                int spawnerIndex = 0;
                foreach (var spawner in spawners)
                {
                    viewPos.AddRow(MakeWaveColumns(spawner, viewPos.Vector2, spawnerIndex));
                    spawnerIndex++;
                }

                float MakeWaveColumns(WaveController.Spawner spawner, Vector2 originPos, int spawnerIndex)
                {
                    var colPos = new EditorPos(originPos);
                    int waveIndex = 0;
                    var waves = new List<WaveProperties>(spawner.Waves);
                    foreach (var wave in waves)
                    {
                        colPos.AddColumn(MakeWave(wave, colPos.Vector2, spawnerIndex, waveIndex) + spaceBetweenWaveColumn);
                        waveIndex++;
                    }

                    colPos.AddColumn(MakeAddWave(colPos.Vector2, spawnerIndex));

                    return rowLength;

                    float MakeWave(WaveProperties wave, Vector2 originPos, int spawnerIndex, int waveIndex)
                    {
                        var waveColor = waveColors.GetColorOf(wave);

                        var wavePos = new EditorPos(originPos);

                        wavePos.AddRow(MakeSOName(wavePos, wave).height);
                        wavePos.AddRow(MakePrefab(wavePos, wave).height);
                        wavePos.AddRow(MakeDelay(wavePos, wave).height);
                        wavePos.AddRow(MakePeriod(wavePos, wave).height);
                        wavePos.AddRow(MakeCount(wavePos, wave).height);
                        wavePos.AddRow(MakeSOControls(wavePos, wave, spawnerIndex, waveIndex).height);

                        return defaultFieldSize.Width;

                        Rect MakeSOName(EditorPos pos, WaveProperties wave)
                        {
                            var nameRect = new Rect(pos.Vector2, defaultFieldSize.Vector2);
                            GUI.color = waveColor;
                            var newName = EditorGUI.TextField(nameRect, wave.SOName);
                            GUI.color = guiColors.color;
                            wave.SOName = newName;
                            if (wave!=null)
                                wave.name = newName;
                            return nameRect;
                        }

                        Rect MakePrefab(EditorPos pos, WaveProperties wave)
                        {
                            var prefabRect = new Rect(wavePos.Vector2, defaultFieldSize.Vector2);
                            if (wave.Prefab == null)
                            {

                            }
                            GUI.color = waveColor;
                            var newPrefab = EditorGUI.ObjectField(prefabRect, wave.Prefab, typeof(GameObject), true);
                            GUI.color = guiColors.color;
                            wave.Prefab = newPrefab as GameObject;
                            return prefabRect;
                        }

                        Rect MakeDelay(EditorPos pos, WaveProperties wave)
                        {
                            var thisPos = new EditorPos(pos.Vector2);

                            var iconRect = MakeIcon(thisPos, iconDelay, "Delay");
                            thisPos.AddColumn(iconRect.width);

                            var delayRect = new Rect(thisPos.Vector2, new Vector2(defaultFieldSize.Width - iconRect.width - 40f, defaultFieldSize.Height));
                            var newDelay = EditorGUI.FloatField(delayRect, wave.Delay);
                            thisPos.AddColumn(delayRect.width);
                            wave.Delay = newDelay;

                            var addSubRect = MakeAddSubButtons(thisPos, wave.Delay, (newDelay) => { wave.Delay = newDelay; }, 0.1f);


                            return new Rect(pos.Width, pos.Height, delayRect.width + addSubRect.width, delayRect.height);
                        }

                        Rect MakePeriod(EditorPos pos, WaveProperties wave)
                        {
                            var thisPos = new EditorPos(pos.Vector2);

                            var iconRect = MakeIcon(thisPos, iconClock, "Period");
                            thisPos.AddColumn(iconRect.width);

                            var delayRect = new Rect(thisPos.Vector2, new Vector2(defaultFieldSize.Width - iconRect.width - 40f, defaultFieldSize.Height));
                            var newPeriod = EditorGUI.FloatField(delayRect, wave.Period);
                            thisPos.AddColumn(delayRect.width);
                            wave.Period = newPeriod;

                            var addSubRect = MakeAddSubButtons(thisPos, wave.Period, (newPeriod) => { wave.Period = newPeriod; }, 0.1f);

                            return new Rect(pos.Width, pos.Height, delayRect.width + addSubRect.width, delayRect.height);
                        }

                        Rect MakeCount(EditorPos pos, WaveProperties wave)
                        {
                            var thisPos = new EditorPos(pos.Vector2);

                            var iconRect = MakeIcon(thisPos, iconCount, "Count");
                            thisPos.AddColumn(iconRect.width);

                            var delayRect = new Rect(thisPos.Vector2, new Vector2(defaultFieldSize.Width - iconRect.width - 40f, defaultFieldSize.Height));
                            var newCount = EditorGUI.IntField(delayRect, wave.Count);
                            thisPos.AddColumn(delayRect.width);
                            wave.Count = newCount;

                            var addSubRect = MakeAddSubButtons(thisPos, wave.Count, (newCount) => { wave.Count = newCount; }, 1);

                            return new Rect(pos.Width, pos.Height, delayRect.width + addSubRect.width, delayRect.height);
                        }

                        Rect MakeSOControls(EditorPos pos, WaveProperties wave, int spawnerIndex, int waveIndex)
                        {
                            var thisPos = new EditorPos(pos.Vector2);
                            var buttonSize = new Vector2(25f, defaultFieldSize.Height);
                            int buttonCount = 0;
                            
                            thisPos.AddColumn(MakeCopyButton(wave, thisPos.Vector2, buttonSize).width);
                            buttonCount++;
                            thisPos.AddColumn(MakePasteButton(wave, thisPos.Vector2, buttonSize).width);
                            buttonCount++;
                            thisPos.AddColumn(MakeDeleteButton(wave, thisPos.Vector2, buttonSize).width);
                            buttonCount++;
                            
                            return new Rect(pos.Width, pos.Height, buttonSize.x * buttonCount, buttonSize.y);

                            Rect MakeCopyButton(WaveProperties wave, Vector2 pos, Vector2 buttonSize)
                            {
                                var rect = new Rect(pos.x, pos.y, buttonSize.x, defaultFieldSize.Height);
                                if (copiedWaveProperties != null && copiedWaveProperties.wave == wave)
                                {
                                    GUI.color = Encore.Utility.ColorUtility.goldenRod;
                                    if (GUI.Button(rect, new GUIContent(iconCopy, "Copy")))
                                        copiedWaveProperties = null;
                                }
                                else 
                                {
                                    if (copiedWaveProperties != null)
                                        GUI.color = guiColors.color.ChangeAlpha(0.25f);

                                    if (GUI.Button(rect, new GUIContent(iconCopy, "Copy")))
                                    {
                                        copiedWaveProperties = new CopiedWave(wave, spawnerIndex, waveIndex);
                                    }
                                }

                                GUI.color = guiColors.color;
                                return rect;
                            }

                            Rect MakePasteButton(WaveProperties wave, Vector2 pos, Vector2 buttonSize)
                            {
                                if (copiedWaveProperties != null)
                                {
                                    var rect = new Rect(pos.x, pos.y, buttonSize.x, defaultFieldSize.Height);
                                    if (GUI.Button(rect, new GUIContent(iconPaste, "Paste")))
                                    {
                                        waveController.Spawners[spawnerIndex].Waves.Insert(waveIndex, copiedWaveProperties.wave);
                                        waveController.Spawners[spawnerIndex].Waves.RemoveAt(waveIndex+1);
                                        copiedWaveProperties = null;
                                        
                                    }
                                    return rect;
                                }
                                return new Rect(pos, Vector2.zero);
                            }

                            Rect MakeDeleteButton(WaveProperties wave, Vector2 pos, Vector2 buttonSize)
                            {
                                if (copiedWaveProperties == null)
                                {
                                    var rect = new Rect(pos.x, pos.y, buttonSize.x, defaultFieldSize.Height);
                                    GUI.color = Encore.Utility.ColorUtility.salmon;
                                    if (GUI.Button(rect, new GUIContent(iconDelete, "Delete")))
                                    {
                                        waveController.Spawners[spawnerIndex].Waves.RemoveAt(waveIndex);
                                    }
                                    GUI.color = guiColors.color;

                                    return rect;
                                }
                                return new Rect(pos, Vector2.zero);
                            }
                        }
                    }

                    float MakeAddWave(Vector2 originPos, int spawnerIndex)
                    {
                        var wholeRect = new Rect(originPos, new Vector2(defaultFieldSize.Width, defaultFieldSize.Height * wavePropertiesRowCount));
                        var addRectSize = new Vector2(32, 32);
                        var addRectPos = new Vector2(wholeRect.x + wholeRect.width/2 - addRectSize.x/2, wholeRect.y + wholeRect.height/2 - addRectSize.y/2);
                        var addRect = new Rect(addRectPos, addRectSize);

                        GUI.color = guiColors.color.ChangeAlpha(0.25f);
                        if (copiedWaveProperties == null)
                        {
                            if (GUI.Button(addRect, new GUIContent(iconAdd, "New Wave")))
                            {
                                var newWaveProperties = CreateInstance<WaveProperties>();
                                waveController.Spawners[spawnerIndex].Waves.Add(newWaveProperties);
                                waveColors.AddWaveColor(newWaveProperties);
                            }
                        }
                        else
                        {
                            if (GUI.Button(addRect, new GUIContent(iconPaste, "Paste Wave")))
                            {
                                waveController.Spawners[spawnerIndex].Waves.Add(copiedWaveProperties.wave);
                                copiedWaveProperties = null;
                            }
                        }

                        GUI.color = guiColors.color;

                        return wholeRect.width;
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
            GUI.color = guiColors.color;
            rect.width += 5f;
            return rect;
        }

        Rect MakeAddSubButtons(EditorPos pos, int initialValue, Action<int> onNewValue, int increment)
        {
            var buttonWidth = 20f;

            var thisPos = new EditorPos(pos.Vector2);

            var addRect = new Rect(thisPos.Width, thisPos.Height, buttonWidth, defaultFieldSize.Height);

            if (GUI.Button(addRect, "+"))
                onNewValue(initialValue + increment);
            thisPos.AddColumn(addRect.width);

            var subRect = new Rect(thisPos.Width, thisPos.Height, buttonWidth, defaultFieldSize.Height);
            if (GUI.Button(subRect, "-"))
                onNewValue(initialValue - increment);
            thisPos.AddColumn(subRect.width);

            return new Rect(pos.Width, pos.Height, addRect.width + subRect.width, addRect.height);
        }

        Rect MakeAddSubButtons(EditorPos pos, float initialValue, Action<float> onNewValue, float increment)
        {
            var buttonWidth = 20f;

            var thisPos = new EditorPos(pos.Vector2);

            var addRect = new Rect(thisPos.Width, thisPos.Height, buttonWidth, defaultFieldSize.Height);

            if(GUI.Button(addRect, "+"))
                onNewValue(initialValue + increment);
            thisPos.AddColumn(addRect.width);

            var subRect = new Rect(thisPos.Width, thisPos.Height, buttonWidth, defaultFieldSize.Height);
            if (GUI.Button(subRect, "-"))
                onNewValue(initialValue - increment);
            thisPos.AddColumn(subRect.width);

            return new Rect(pos.Width, pos.Height, addRect.width + subRect.width, addRect.height);
        }
    }
}
