using Encore.Utility;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

namespace Phoenix
{
    public class LevelBGMController : MonoBehaviour
    {
        [SerializeField]
        AudioMixer audioMixer;

        [SerializeField, SuffixLabel("sec", true)]
        float bgmFadeDuration = 2f;

        [SerializeField]
        string BGM_VOLUME = "BGM_Volume";

        [SerializeField]
        string PAUSEFX_VOLUME = "PauseFX_Volume";

        SaveCache saveCache;

        Coroutine corModifyingBGMVolume;
        Coroutine corModifyingPauseFXVolume;

        public void Init(LevelManager levelManager)
        {
            saveCache = levelManager.SaveCache;
            audioMixer.SetFloatLog(PAUSEFX_VOLUME, 0);

            levelManager.OnInit += IncreaseBGMVolume;
            levelManager.OnPause += ApplyPauseFX;
            levelManager.OnResume += RemovePauseFX;
            levelManager.OnStartQuitting += DecreaseAllSounds;
            levelManager.OnShowWinCanvas += ApplyPauseFX;
            levelManager.OnShowGameOverCanvas += ApplyPauseFX;
        }

        public void Exit(LevelManager levelManager)
        {
            levelManager.OnInit -= IncreaseBGMVolume;
            levelManager.OnPause -= ApplyPauseFX;
            levelManager.OnResume -= RemovePauseFX;
            levelManager.OnStartQuitting -= DecreaseAllSounds;
            levelManager.OnShowWinCanvas -= ApplyPauseFX;
            levelManager.OnShowGameOverCanvas -= ApplyPauseFX;
            StopAllCoroutines();
        }

        void IncreaseBGMVolume()
        {
            corModifyingBGMVolume = this.RestartCoroutine(SetParam(BGM_VOLUME, bgmFadeDuration, 0, saveCache != null ? saveCache.audioData.bgmVolume : 1f));
        }

        void ApplyPauseFX()
        {
            if (corModifyingBGMVolume != null) StopCoroutine(corModifyingBGMVolume);
            if (corModifyingPauseFXVolume != null) StopCoroutine(corModifyingPauseFXVolume);

            audioMixer.SetFloatLog(BGM_VOLUME, saveCache != null ? saveCache.audioData.bgmVolume/2f : 0.5f);
            audioMixer.SetFloatLog(PAUSEFX_VOLUME, saveCache != null ? saveCache.audioData.bgmVolume : 1f);
        }

        void RemovePauseFX()
        {
            if (corModifyingBGMVolume != null) StopCoroutine(corModifyingBGMVolume);
            if (corModifyingPauseFXVolume != null) StopCoroutine(corModifyingPauseFXVolume);

            audioMixer.SetFloatLog(BGM_VOLUME, saveCache != null ? saveCache.audioData.bgmVolume : 1f);
            audioMixer.SetFloatLog(PAUSEFX_VOLUME, 0);
        }

        void DecreaseAllSounds(float duration)
        {
            duration -= 0.2f;
            corModifyingBGMVolume = this.RestartCoroutine(SetParam(BGM_VOLUME, duration, audioMixer.GetFloatExp(BGM_VOLUME), 0));
            corModifyingPauseFXVolume = this.RestartCoroutine(SetParam(PAUSEFX_VOLUME, duration, audioMixer.GetFloatExp(PAUSEFX_VOLUME), 0));
        }

        IEnumerator SetParam(string paramName, float duration, float startValue, float endValue)
        {
            var time = 0f;
            var marginValue = Mathf.Abs(startValue - endValue);

            audioMixer.SetFloatLog(paramName, startValue);

            if (startValue < endValue)
            {
                while (true)
                {
                    audioMixer.SetFloatLog(paramName, (time / duration * marginValue) + startValue);
                    if (audioMixer.GetFloatExp(paramName) > endValue)
                        break;
                    time += Time.deltaTime;

                    yield return null;
                }

            }

            else 
            {
                while (true)
                {
                    audioMixer.SetFloatLog(paramName, startValue - (time / duration * marginValue));
                    if (audioMixer.GetFloatExp(paramName) < endValue)
                        break;
                    time += Time.deltaTime;

                    yield return null;
                }
            }

        }
    }
}
