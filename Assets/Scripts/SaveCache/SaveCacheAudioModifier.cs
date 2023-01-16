using Encore.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Phoenix
{
    public class SaveCacheAudioModifier : MonoBehaviour
    {
        [SerializeField]
        SaveCache saveCache;

        [SerializeField]
        AudioMixer audioMixer;

        [SerializeField]
        string BGM_VOLUME = "BGM_Volume";

        public void SetBGMVolume(float value)
        {
            saveCache.audioData.bgmVolume = value;
            audioMixer.SetFloatLog(BGM_VOLUME, value);
        }



    }
}
