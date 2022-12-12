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
            window.waveControllerID = waveController.GetInstanceID();
            window.waveColors = new WaveColors(waveController);
            window.Show();
        }

        #endregion


        #region [Classes]

        class CopiedWave
        {
            public WavePropertiesStatic wave;
            public int spawnerIndex;
            public int waveIndex;

            public CopiedWave(WavePropertiesStatic wave, int spawnerIndex, int waveIndex)
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
                public WavePropertiesStatic wave;
                public Color color;

                public WaveColor(WavePropertiesStatic wave, Color color)
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
                        // TODO: change this later
                        var _wave = wave as WavePropertiesStatic;

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
                            waveColors.Add(new WaveColor(_wave, randomColor));
                        }
                    }
                }
            }



            public bool HasWave(WavePropertiesStatic wave)
            {
                foreach (var waveColor in waveColors)
                    if (waveColor.wave == wave)
                        return true;

                return false;
            }

            public Color GetColorOf(WavePropertiesStatic wave)
            {
                foreach (var waveColor in waveColors)
                    if (waveColor.wave == wave)
                        return waveColor.color;

                return Color.gray;
            }

            public void AddWaveColor(WavePropertiesStatic wave)
            {
                waveColors.Add(new WaveColor(wave, randomColor));
            }


            public void Reset(WaveController waveController)
            {
                this.waveController = waveController;
                this.waveColors = new List<WaveColor>();
                foreach (var spawner in waveController.Spawners)
                {
                    foreach (var wave in spawner.Waves)
                    {
                        // TODO: change this later
                        var _wave = wave as WavePropertiesStatic;

                        bool alreadyAdded = false;
                        foreach (var waveColor in waveColors)
                        {
                            if (waveColor.wave == _wave)
                            {
                                alreadyAdded = true;
                                break;
                            }
                        }
                        if (!alreadyAdded)
                        {
                            waveColors.Add(new WaveColor(_wave, randomColor));
                        }
                    }
                }
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

        int wavePropertiesRowCount = 0;
        readonly Padding padding = new(5,5,10,10);
        readonly EditorPos defaultFieldSize = new EditorPos(100, 25);
        readonly EditorPos iconSize = new EditorPos(25, 25);
        readonly EditorPos addSubButtonsSize = new EditorPos(40, 25);
        readonly float spaceBetweenRow = 15f;
        readonly float spaceBetweenWaveColumn = 15f;
        WaveColors waveColors = new WaveColors();

        #endregion

        #region [Vars: Data Handlers]

        float rowLength => wavePropertiesRowCount * defaultFieldSize.Height + spaceBetweenRow;
        WaveController waveController;
        int waveControllerID = -1;
        CopiedWave copiedWaveProperties;
        Vector2 scrollViewPos;
        readonly string iconsFolder = "Assets/Scripts/Editor/Icons/";
        Texture2D iconDelay, iconClock, iconCount, iconCopy, iconPaste, iconDelete, iconAdd, iconLoop,iconWarning, iconOn, iconOff,iconPrefab,iconWrench;
        EditorColors guiColors = new EditorColors();
        bool isShowPrefab = true, isShowDelay = true, isShowPeriod = true, isShowCount = true, isShowSOControls = true;
        float incrementBy = 0.1f;

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
            iconLoop = AssetDatabase.LoadAssetAtPath<Texture2D>(iconsFolder + "ic_loop.png");
            iconWarning = AssetDatabase.LoadAssetAtPath<Texture2D>(iconsFolder + "ic_warning.png");
            iconOn = AssetDatabase.LoadAssetAtPath<Texture2D>(iconsFolder + "ic_on.png");
            iconOff = AssetDatabase.LoadAssetAtPath<Texture2D>(iconsFolder + "ic_off.png");
            iconPrefab = AssetDatabase.LoadAssetAtPath<Texture2D>(iconsFolder + "ic_prefab.png");
            iconWrench = AssetDatabase.LoadAssetAtPath<Texture2D>(iconsFolder + "ic_wrench.png");
            guiColors = new EditorColors(GUI.color, GUI.contentColor, GUI.backgroundColor);

            EditorApplication.playModeStateChanged += (mode) =>
            {
                if(mode == PlayModeStateChange.EnteredEditMode)
                {
                    var waveCs = FindObjectsOfType<WaveController>();
                    foreach (var waveC in waveCs)
                        if (waveControllerID != -1 && waveC.GetInstanceID() == waveControllerID)
                        {
                            waveController = waveC;
                        }
                }

            };
        }

        int CountShownWavePropertiesRow()
        {
            var count = 1;
            if(isShowPrefab)
                count++;
            if (isShowDelay)
                count++;
            if (isShowPeriod)
                count++;
            if (isShowCount)
                count++;
            if (isShowSOControls)
                count++;

            if (count < 2)
                count = 2; // soName is always true, and generalInfo requires 2 rows to look good.

            return count;
        }

        void OnGUI()
        {
            if (waveController == null)
            {
                var labelRect = new Rect(0, 0, position.width, position.height);
                var labelStyle = GUI.skin.label;
                labelStyle.alignment = TextAnchor.MiddleCenter;
                EditorGUI.LabelField(labelRect, "Choose a WaveController", labelStyle);
                return;
            }

            var spawners = new List<WaveController.Spawner>(waveController.Spawners);
            if (waveColors.WaveController != waveController)
                waveColors.Reset(waveController);

            wavePropertiesRowCount = CountShownWavePropertiesRow();

            var pos = new EditorPos(padding.left, padding.top);
            pos.AddRow(MakeTopBar(pos.Vector2));

            #region [Begin ScrollView]

            var windowSize = new Vector2(position.width - padding.Horizontal, position.height - padding.Vertical);
            var scrollViewRect = new Rect(pos.Vector2, windowSize);
            var viewRect = new Rect(Vector2.zero, GetContentViewSize(spawners));
            scrollViewPos = GUI.BeginScrollView(scrollViewRect, scrollViewPos, viewRect);

            #endregion

            pos = new EditorPos(0, 0);
            MakeSpawnerGeneralInfosColumn(pos.Vector2, spawners);
            MakeWavesView(new Vector2(defaultFieldSize.Width + 20, pos.Height), spawners);

            GUI.EndScrollView();
            Undo.RecordObject(this, "Wave");

            float MakeTopBar(Vector2 originPos)
            {
                var topBarPos = new EditorPos(originPos);


                topBarPos.AddColumn(MakeLabel(topBarPos.Vector2, "Show"));
                topBarPos.AddColumn(MakeShowButton(topBarPos.Vector2, iconPrefab, ref isShowPrefab, nameof(isShowPrefab)));
                topBarPos.AddColumn(MakeShowButton(topBarPos.Vector2, iconDelay, ref isShowDelay, nameof(isShowDelay)));
                topBarPos.AddColumn(MakeShowButton(topBarPos.Vector2, iconClock, ref isShowPeriod, nameof(isShowPeriod)));
                topBarPos.AddColumn(MakeShowButton(topBarPos.Vector2, iconCount, ref isShowCount, nameof(isShowCount)));
                topBarPos.AddColumn(MakeShowButton(topBarPos.Vector2, iconWrench, ref isShowSOControls, nameof(isShowSOControls)));

                topBarPos.AddColumn(20f);
                topBarPos.AddColumn(MakeLabel(topBarPos.Vector2, "+/- by", "IncrementBy; used wehn using + or - buttons"));
                topBarPos.AddColumn(MakeFloatInput(topBarPos.Vector2, ref incrementBy));


                return defaultFieldSize.Height;

                float MakeLabel(Vector2 labelPos, string labelName, string tooltip = "")
                {
                    var rect = new Rect(labelPos, new Vector2(defaultFieldSize.Width / 2, defaultFieldSize.Height));
                    EditorGUI.LabelField(rect, new GUIContent(labelName + ": ", tooltip));
                    return rect.width;
                }

                float MakeShowButton(Vector2 buttonPos, Texture2D icon, ref bool boolValue, string boolName)
                {
                    var buttonRect = new Rect(buttonPos, iconSize.Vector2);
                    if (boolValue)
                    {
                        if (GUI.Button(buttonRect, new GUIContent(icon, boolName + ": true")))
                            boolValue = false;
                    }
                    else
                    {
                        GUI.color = Color.white.ChangeAlpha(0.5f);
                        if (GUI.Button(buttonRect, new GUIContent(icon, boolName + ": false")))
                            boolValue = true;
                    }

                    GUI.color = guiColors.color;

                    return buttonRect.width;
                }

                float MakeFloatInput(Vector2 textInputPos, ref float floatVar)
                {
                    var rect = new Rect(textInputPos, new Vector2(defaultFieldSize.Width / 2, defaultFieldSize.Height));
                    floatVar = EditorGUI.FloatField(rect, floatVar);
                    return rect.width;
                }
            }

            void MakeSpawnerGeneralInfosColumn(Vector2 originPos, List<WaveController.Spawner> spawners)
            {
                var rowPos = new EditorPos(originPos);
                int spawnerIndex = 0;
                foreach (var spawner in spawners)
                {
                    rowPos.AddRow(MakeGeneralInfo(spawner, rowPos.Vector2, spawnerIndex));
                    spawnerIndex++;
                }

                float MakeGeneralInfo(WaveController.Spawner spawner, Vector2 originPos, int spawnerIndex)
                {
                    var infoPos = new EditorPos(originPos);
                    infoPos.AddRow(MakeTransform(infoPos.Vector2, spawner));
                    infoPos.AddRow(MakeToggles(infoPos.Vector2, spawner));

                    return rowLength;

                    float MakeTransform(Vector2 pos, WaveController.Spawner spawner)
                    {
                        var transformRect = new Rect(pos, defaultFieldSize.Vector2);
                        spawner.Position = (Transform)EditorGUI.ObjectField(transformRect, spawner.Position, typeof(Transform), true);
                        return transformRect.height;
                    }

                    float MakeToggles(Vector2 pos, WaveController.Spawner spawner)
                    {
                        var togglePos = new EditorPos(pos);
                        togglePos.AddColumn(MakeIsActive(togglePos.Vector2, spawner));
                        togglePos.AddColumn(MakeIsLoop(togglePos.Vector2, spawner));

                        return defaultFieldSize.Height;
                    }

                    float MakeIsLoop(Vector2 pos, WaveController.Spawner spawner)
                    {
                        var isLoopRect = new Rect(pos, iconSize.Vector2);
                        if (spawner.IsLoop)
                        {
                            if (GUI.Button(isLoopRect, new GUIContent(iconLoop, "isLooping: true")))
                                spawner.IsLoop = false;
                        }
                        else
                        {
                            GUI.color = guiColors.color.ChangeAlpha(0.25f);
                            if (GUI.Button(isLoopRect, new GUIContent(iconLoop, "isLooping: false")))
                                spawner.IsLoop = true;
                        }

                        GUI.color = guiColors.color;

                        return isLoopRect.width;
                    }

                    float MakeIsActive(Vector2 pos, WaveController.Spawner spawner)
                    {
                        var isActiveRect = new Rect(pos, iconSize.Vector2);
                        if (spawner.IsActive)
                        {
                            if (GUI.Button(isActiveRect, new GUIContent(iconOn, "isActive: true")))
                                spawner.IsActive = false;
                        }
                        else
                        {
                            GUI.color = guiColors.color.ChangeAlpha(0.25f);
                            if (GUI.Button(isActiveRect, new GUIContent(iconOff, "isActive: false")))
                                spawner.IsActive = true;
                        }

                        GUI.color = guiColors.color;

                        return isActiveRect.width;
                    }
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
                    var waves = new List<WavePropertiesStatic>();
                    foreach (var wave in spawner.Waves)
                        waves.Add(wave as WavePropertiesStatic);

                    foreach (var wave in waves)
                    {
                        colPos.AddColumn(MakeWave(wave, colPos.Vector2, spawnerIndex, waveIndex) + spaceBetweenWaveColumn);
                        waveIndex++;
                    }

                    colPos.AddColumn(MakeAddWave(colPos.Vector2, spawnerIndex));

                    return rowLength;

                    float MakeWave(WavePropertiesStatic wave, Vector2 originPos, int spawnerIndex, int waveIndex)
                    {
                        var waveColor = waveColors.GetColorOf(wave);

                        var wavePos = new EditorPos(originPos);

                        wavePos.AddRow(MakeSOName(wavePos, wave).height);
                        if(isShowPrefab)
                            wavePos.AddRow(MakePrefab(wavePos, wave).height);
                        if(isShowDelay)
                            wavePos.AddRow(MakeDelay(wavePos, wave).height);
                        if(isShowPeriod)
                            wavePos.AddRow(MakePeriod(wavePos, wave).height);
                        if(isShowCount)
                            wavePos.AddRow(MakeCount(wavePos, wave).height);
                        if(isShowSOControls)
                            wavePos.AddRow(MakeSOControls(wavePos, wave, spawnerIndex, waveIndex).height);

                        return defaultFieldSize.Width;

                        Rect MakeSOName(EditorPos pos, WavePropertiesStatic wave)
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

                        Rect MakePrefab(EditorPos pos, WavePropertiesStatic wave)
                        {
                            var prefabRect = new Rect(wavePos.Vector2, defaultFieldSize.Vector2);
                            if (wave.Prefab == null)
                            {
                                var warningRect = MakeIcon(wavePos, iconWarning, "Prefab: Null", Color.yellow);
                                prefabRect.width -= warningRect.width;
                                prefabRect.x += warningRect.width;
                            }
                            GUI.color = waveColor;
                            var newPrefab = EditorGUI.ObjectField(prefabRect, wave.Prefab, typeof(GameObject), true);
                            GUI.color = guiColors.color;
                            wave.Prefab = newPrefab as GameObject;
                            return prefabRect;
                        }

                        Rect MakeDelay(EditorPos pos, WavePropertiesStatic wave)
                        {
                            var thisPos = new EditorPos(pos.Vector2);

                            var iconRect = MakeIcon(thisPos, iconDelay, "Delay");
                            thisPos.AddColumn(iconRect.width);

                            var delayRect = new Rect(thisPos.Vector2, new Vector2(defaultFieldSize.Width - iconRect.width - addSubButtonsSize.Width , defaultFieldSize.Height));
                            var newDelay = EditorGUI.FloatField(delayRect, wave.Delay);
                            thisPos.AddColumn(delayRect.width);
                            wave.Delay = newDelay;

                            var addSubRect = MakeAddSubButtons(thisPos, wave.Delay, (newDelay) => { wave.Delay = newDelay; }, incrementBy);


                            return new Rect(pos.Width, pos.Height, delayRect.width + addSubRect.width, delayRect.height);
                        }

                        Rect MakePeriod(EditorPos pos, WavePropertiesStatic wave)
                        {
                            var thisPos = new EditorPos(pos.Vector2);

                            var iconRect = MakeIcon(thisPos, iconClock, "Period");
                            thisPos.AddColumn(iconRect.width);

                            var delayRect = new Rect(thisPos.Vector2, new Vector2(defaultFieldSize.Width - iconRect.width - addSubButtonsSize.Width , defaultFieldSize.Height));
                            var newPeriod = EditorGUI.FloatField(delayRect, wave.Period);
                            thisPos.AddColumn(delayRect.width);
                            wave.Period = newPeriod;

                            var addSubRect = MakeAddSubButtons(thisPos, wave.Period, (newPeriod) => { wave.Period = newPeriod; }, incrementBy);

                            return new Rect(pos.Width, pos.Height, delayRect.width + addSubRect.width, delayRect.height);
                        }

                        Rect MakeCount(EditorPos pos, WavePropertiesStatic wave)
                        {
                            var thisPos = new EditorPos(pos.Vector2);

                            var iconRect = MakeIcon(thisPos, iconCount, "Count");
                            thisPos.AddColumn(iconRect.width);

                            var delayRect = new Rect(thisPos.Vector2, new Vector2(defaultFieldSize.Width - iconRect.width - addSubButtonsSize.Width, defaultFieldSize.Height));
                            var newCount = EditorGUI.IntField(delayRect, wave.Count);
                            thisPos.AddColumn(delayRect.width);
                            wave.Count = newCount;

                            var addSubRect = MakeAddSubButtons(thisPos, wave.Count, (newCount) => { wave.Count = newCount; }, 1);

                            return new Rect(pos.Width, pos.Height, delayRect.width + addSubRect.width, delayRect.height);
                        }

                        Rect MakeSOControls(EditorPos pos, WavePropertiesStatic wave, int spawnerIndex, int waveIndex)
                        {
                            var thisPos = new EditorPos(pos.Vector2);
                            int buttonCount = 0;
                            
                            thisPos.AddColumn(MakeCopyButton(wave, thisPos.Vector2, iconSize.Vector2).width);
                            buttonCount++;
                            thisPos.AddColumn(MakePasteButton(wave, thisPos.Vector2, iconSize.Vector2).width);
                            buttonCount++;
                            thisPos.AddColumn(MakeDeleteButton(wave, thisPos.Vector2, iconSize.Vector2).width);
                            buttonCount++;
                            
                            return new Rect(pos.Width, pos.Height, iconSize.Width * buttonCount, iconSize.Height);

                            Rect MakeCopyButton(WavePropertiesStatic wave, Vector2 pos, Vector2 buttonSize)
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

                            Rect MakePasteButton(WavePropertiesStatic wave, Vector2 pos, Vector2 buttonSize)
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

                            Rect MakeDeleteButton(WavePropertiesStatic wave, Vector2 pos, Vector2 buttonSize)
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
                                var newWaveProperties = CreateInstance<WavePropertiesStatic>();
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

                var width = mostWaveCount * defaultFieldSize.Width + defaultFieldSize.Width;

                return new Vector2(width, height);
            }


        }

        Rect MakeIcon(EditorPos pos, Texture2D icon, string tooltip, Color? color = null)
        {
            var rect = new Rect(pos.Vector2, iconSize.Vector2);
            var iconReducedSize = new Vector2(rect.width * 0.66f, rect.height * 0.66f);
            var iconReducedPos = new Vector2(rect.x + rect.width/2 - iconReducedSize.x/2, rect.y + rect.height / 2 - iconReducedSize.y / 2);
            var iconReducedRect = new Rect(iconReducedPos, iconReducedSize);

            GUI.color = (color != null) ? (Color)color : Color.white.ChangeAlpha(0.5f);
            EditorGUI.LabelField(iconReducedRect, new GUIContent(icon, tooltip));
            GUI.color = guiColors.color;

            return rect;
        }

        Rect MakeAddSubButtons(EditorPos pos, int initialValue, Action<int> onNewValue, int increment)
        {
            var thisPos = new EditorPos(pos.Vector2);
            var buttonSize = new EditorPos(addSubButtonsSize.Width / 2, addSubButtonsSize.Height);

            var addRect = new Rect(thisPos.Vector2, buttonSize.Vector2);
            if (GUI.Button(addRect, "+"))
                onNewValue(initialValue + increment);
            thisPos.AddColumn(addRect.width);

            var subRect = new Rect(thisPos.Vector2, buttonSize.Vector2);
            if (GUI.Button(subRect, "-"))
                onNewValue(initialValue - increment);
            thisPos.AddColumn(subRect.width);

            return new Rect(pos.Width, pos.Height, addRect.width + subRect.width, addRect.height);
        }

        Rect MakeAddSubButtons(EditorPos pos, float initialValue, Action<float> onNewValue, float increment)
        {
            var thisPos = new EditorPos(pos.Vector2);
            var buttonSize = new EditorPos(addSubButtonsSize.Width/2, addSubButtonsSize.Height);

            var addRect = new Rect(thisPos.Vector2, buttonSize.Vector2);

            if(GUI.Button(addRect, "+"))
                onNewValue(initialValue + increment);
            thisPos.AddColumn(addRect.width);

            var subRect = new Rect(thisPos.Vector2, buttonSize.Vector2);
            if (GUI.Button(subRect, "-"))
                onNewValue(initialValue - increment);
            thisPos.AddColumn(subRect.width);

            return new Rect(pos.Width, pos.Height, addRect.width + subRect.width, addRect.height);
        }
    }
}
