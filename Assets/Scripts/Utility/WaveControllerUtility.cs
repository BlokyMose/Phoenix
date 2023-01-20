using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Phoenix.WaveController;
using static DialogueSyntax.DSyntaxUtility;
using static DialogueSyntax.DSyntaxSettings;
using Encore.Utility;
using DialogueSyntax;
using System;

namespace Phoenix
{
    public static class WaveControllerUtility
    {
        public const string SPAWNER = nameof(SPAWNER);
        public const string NULL = "null";
        public const string TYPE = "type";
        public const string DELAY = "delay";
        public const string PERIOD = "period";
        public const string COUNT = "count";
        public const string WAVE_STATIC = "static";
        public const string WAVE_INIT_RANDOM = "init_random";
        public const string WAVE_RANDOM = "random";

        public const string ALTERNATE_PREFABS = "alternate_prefabs";
        public const string DELAY_RANGE = "delay_range";
        public const string PERIOD_RANGE = "period_range";
        public const string COUNT_RANGE = "count_range";
        public const string COMMA = ", ";

        public static string ExportDSyntax(List<Spawner> spawners)
        {
            var result = "";
            #if UNITY_EDITOR
            var settings = LoadDSyntaxSettingsWave();

            if (settings != null)
            {
                foreach (var spawner in spawners)
                {
                    result += WriteCommand(settings, SPAWNER, spawner.Position.gameObject.name);
                    foreach (var wave in spawner.Waves)
                    {
                        result += WriteCommand(settings, wave.SOName, wave.Prefab != null ? wave.Prefab.name : NULL);
                        result += "  ";
                        result += WriteParameter(settings, new Parameter(TYPE, GetWaveTypeName(wave))) + " ";
                        result += WriteParameter(settings, new Parameter(DELAY, wave.Delay.ToString())) + " ";
                        result += WriteParameter(settings, new Parameter(PERIOD, wave.Period.ToString())) + " ";
                        result += WriteParameter(settings, new Parameter(COUNT, wave.Count.ToString())) + " ";
                        result += AddParametersOfChildClass(settings, wave);
                        result += "\n";
                    }
                    result += " \n\n";
                }
            }
            #endif

            return result;

            static string GetWaveTypeName(WaveProperties wave)
            {
                if (wave is WavePropertiesInitialRandom)
                    return WAVE_INIT_RANDOM;
                else if (wave is WavePropertiesRandom)
                    return WAVE_RANDOM;
                else
                    return WAVE_STATIC;
            }

            static string AddParametersOfChildClass(DSyntaxSettings settings, WaveProperties wave)
            {
                var allParameters = "";

                if (wave is WavePropertiesInitialRandom)
                {
                    var waveInitRandom = wave as WavePropertiesInitialRandom;

                    var alternatePrefabsNames = "";
                    foreach (var prefab in waveInitRandom.AlternatePrefabs)
                        alternatePrefabsNames += prefab.name + COMMA;
                    alternatePrefabsNames = alternatePrefabsNames[..^COMMA.Length];

                    allParameters += "\n  ";
                    allParameters += WriteParameter(settings, new Parameter(ALTERNATE_PREFABS, alternatePrefabsNames)) + " ";
                    allParameters += WriteParameter(settings, new Parameter(DELAY_RANGE, waveInitRandom.DelayRange.ToString())) + " ";
                    allParameters += WriteParameter(settings, new Parameter(PERIOD_RANGE, waveInitRandom.PeriodRange.ToString())) + " ";
                    allParameters += WriteParameter(settings, new Parameter(COUNT_RANGE, waveInitRandom.CountRange.ToString())) + " ";
                }
                else if (wave is WavePropertiesRandom)
                {
                    var waveRandom = wave as WavePropertiesRandom;

                    var alternatePrefabsNames = "";
                    foreach (var prefab in waveRandom.AlternatePrefabs)
                        alternatePrefabsNames += prefab.name + COMMA;
                    alternatePrefabsNames = alternatePrefabsNames[..^COMMA.Length];

                    allParameters += "\n  ";
                    allParameters += WriteParameter(settings, new Parameter(ALTERNATE_PREFABS, alternatePrefabsNames)) + " ";
                    allParameters += WriteParameter(settings, new Parameter(DELAY_RANGE, waveRandom.DelayRange.ToString())) + " ";
                    allParameters += WriteParameter(settings, new Parameter(PERIOD_RANGE, waveRandom.PeriodRange.ToString())) + " ";
                    allParameters += WriteParameter(settings, new Parameter(COUNT_RANGE, waveRandom.CountRange.ToString())) + " ";
                }

                return allParameters;
            }
        }

        public static string ExportDSyntaxAsFile(List<Spawner> spawners, out TextAsset exportedTextAsset, TextAsset textAsset = null, string fileName = "wave_dSyntax")
        {
            var dSyntax = ExportDSyntax(spawners);
            exportedTextAsset = null;
            #if UNITY_EDITOR

            string filePath;
            if (textAsset != null && UnityEditor.AssetDatabase.TryGetGUIDAndLocalFileIdentifier(textAsset, out var guid, out long _))
            {
                if (UnityEditor.EditorUtility.DisplayDialog("Export", "["+textAsset.name + ".txt] will be overridden \n\nChange GameObject name to export with another name", "Export", "Cancel"))
                    filePath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                else
                {
                    exportedTextAsset = textAsset;
                    return dSyntax;
                }
            }
            else
            {
                var parentFolderPath = "Assets/Contents/Waves";
                var folderPath = parentFolderPath + "/DSyntax";
                filePath = folderPath + "/" + fileName + ".txt";
                var foundTextAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(filePath);
                if (foundTextAsset)
                {
                    if (!UnityEditor.EditorUtility.DisplayDialog("Existing File", "[" + fileName + ".txt] has already existed \n\nChange GameObject name to export with another name.", "Override", "Cancel"))
                    {
                        exportedTextAsset = null;
                        return dSyntax;
                    }
                }
                else if (!UnityEditor.AssetDatabase.IsValidFolder(folderPath))
                        UnityEditor.AssetDatabase.CreateFolder(parentFolderPath, "DSyntax");
            }

            System.IO.File.WriteAllText(filePath, dSyntax);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();

            exportedTextAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(filePath);
            #endif

            return dSyntax;
        }

        public static List<Spawner> ImportDSyntax(string dSyntax, Func<string, GameObject> FindGO)
        {
            List<Spawner> spawnersList = new();
            List<WaveProperties> wavesList = new();
            var settings = LoadDSyntaxSettingsWave();

            var spawners = ReadCommandsByGroups(settings, dSyntax, SPAWNER);
            foreach (var commands in spawners)
            {
                Spawner newSpawner = new();
                spawnersList.Add(newSpawner);
                var positionGOName = commands[0].GetParameter(settings, 0);
                var position = FindGO(positionGOName);
                newSpawner.Position = position.transform;

                for (int i = 1; i < commands.Count; i++)
                {
                    var waveName = commands[i].name;
                    var wave = GetWaveFrom(wavesList, waveName);
                    if (wave == null)
                    {
                        wave = CreateWave(commands[i]);
                        wavesList.Add(wave);
                    }
                    newSpawner.Waves.Add(wave);
                }
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();

            }


            return spawnersList;

            static WaveProperties GetWaveFrom(List<WaveProperties> wavesList, string name)
            {
                foreach (var wave in wavesList)
                    if(wave.SOName == name)
                        return wave;
                    
                return null;
            }

            WaveProperties CreateWave(Command command)
            {
                var waveName = command.name;

                var prefabName = command.GetParameter(settings, 0);
                var waveType = command.GetParameter(settings, TYPE, 1);
                var prefab = FindGO(prefabName);
                var delay = float.Parse(command.GetParameter(settings, DELAY, 2));
                var period = float.Parse(command.GetParameter(settings, PERIOD, 3));
                var count = int.Parse(command.GetParameter(settings, COUNT, 4));


                if (waveType == WAVE_INIT_RANDOM)
                {
                    var alternatePrefabs = new List<GameObject>();
                    var alternatePrefabNames = command.GetParameter(settings, ALTERNATE_PREFABS, 5).Split(COMMA);
                    foreach (var name in alternatePrefabNames)
                        alternatePrefabs.Add(FindGO(name));

                    var delayRange = float.Parse(command.GetParameter(settings, DELAY_RANGE, 6));
                    var periodRange = float.Parse(command.GetParameter(settings, PERIOD_RANGE, 7));
                    var countRange = int.Parse(command.GetParameter(settings, COUNT_RANGE, 8));

                    var waveInitRandom = ScriptableObject.CreateInstance<WavePropertiesInitialRandom>();
                    waveInitRandom.Setup(waveName, prefab, delay, period, count, alternatePrefabs, delayRange, periodRange, countRange);
                    return waveInitRandom;
                }

                else if (waveType == WAVE_RANDOM)
                {
                    var alternatePrefabs = new List<GameObject>();
                    var alternatePrefabNames = command.GetParameter(settings, ALTERNATE_PREFABS, 5).Split(COMMA);
                    foreach (var name in alternatePrefabNames)
                        alternatePrefabs.Add(FindGO(name));

                    var delayRange = float.Parse(command.GetParameter(settings, DELAY_RANGE, 6));
                    var periodRange = float.Parse(command.GetParameter(settings, PERIOD_RANGE, 7));
                    var countRange = int.Parse(command.GetParameter(settings, COUNT_RANGE, 8));

                    var waveRandom = ScriptableObject.CreateInstance<WavePropertiesRandom>();
                    waveRandom.Setup(waveName, prefab, delay, period, count, alternatePrefabs, delayRange, periodRange, countRange);
                    return waveRandom;
                }
                else
                {
                    var waveStatic = ScriptableObject.CreateInstance<WavePropertiesStatic>();
                    waveStatic.Setup(waveName, prefab, delay, period, count);
                    return waveStatic;
                }

            }

        }

        public static DSyntaxSettings LoadDSyntaxSettingsWave()
        {
            var settingsGUID = UnityEditor.AssetDatabase.FindAssets("DSyntaxSettings_Wave")[0];
            if (settingsGUID != null)
            {
                var settingsPath = UnityEditor.AssetDatabase.GUIDToAssetPath(settingsGUID);
                var settings = UnityEditor.AssetDatabase.LoadAssetAtPath<DSyntaxSettings>(settingsPath);
                return settings;
            }

            return null;
        }
    }
}