using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    [CreateAssetMenu(menuName ="SO/Save Cache")]
    public class SaveCache : ScriptableObject
    {
        [Serializable]
        public class AudioData
        {
            public float masterVolume = 1f;
            public float bgmVolume = 1f;
            public float sfxVolume = 1f;
        }

        public string saveName = "save";

        public AudioData audioData = new();
    }
}
