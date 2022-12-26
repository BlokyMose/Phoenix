using Encore.Utility;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace Phoenix
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceRandom : MonoBehaviour
    {
        [Serializable]
        public class AudioPack
        {
            [LabelText("Possible Clips")]
            public List<AudioClip> clips = new List<AudioClip>();

            [HorizontalGroup("Volume", 0.66f)]
            public float volume = 1f;
            
            [HorizontalGroup("Volume"), LabelText("+/-"), LabelWidth(25)]
            public float volumeRandomRange = 0f;

            [HorizontalGroup("Pitch", 0.66f)]
            public float pitch = 1f;

            [HorizontalGroup("Pitch"), LabelText("+/-"), LabelWidth(25)]
            public float pitchRandomRange = 0.15f;

            public void Play(AudioSource audioSource)
            {
                audioSource.pitch = Random.Range(pitch - pitchRandomRange, pitch + pitchRandomRange);
                audioSource.PlayOneShot(clips.GetRandom(), Random.Range(volume - volumeRandomRange, volume + volumeRandomRange));
            }
        }

        [SerializeField]
        List<AudioPack> audioPacks = new();

        [FoldoutGroup("Audio Source"), SerializeField]
        AudioClip audioClip;

        [FoldoutGroup("Audio Source"), SerializeField]
        AudioMixerGroup output;

        [FoldoutGroup("Audio Source"), SerializeField]
        bool mute;

        [FoldoutGroup("Audio Source"), SerializeField]
        bool bypassEffect;

        [FoldoutGroup("Audio Source"), SerializeField]
        bool bypassListenerEffect;

        [FoldoutGroup("Audio Source"), SerializeField]
        bool bypassReverbZones;

        [FoldoutGroup("Audio Source"), SerializeField]
        bool playOnAwake = true;

        [FoldoutGroup("Audio Source"), SerializeField]
        bool loop;

        [FoldoutGroup("Audio Source"), SerializeField, Range(0,256)]
        int priority = 128;

        [FoldoutGroup("Audio Source"), SerializeField, Range(0,1)]
        float volume = 1;

        [FoldoutGroup("Audio Source"), SerializeField, Range(-3,3)]
        float pitch = 1;

        [FoldoutGroup("Audio Source"), SerializeField, Range(-1,1)]
        float stereoPan = 0;

        [FoldoutGroup("Audio Source"), SerializeField, Range(0,1)]
        float spatialBlend;

        [FoldoutGroup("Audio Source"), SerializeField, Range(0,1.1f)]
        float reverbZoneMix = 1;

        AudioSource audioSource;

        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            SyncAudioSourceProperties();

            if (playOnAwake)
                Play();
        }

        [FoldoutGroup("Audio Source"), Button("Sync")]
        void SyncAudioSourceProperties()
        {
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();

            audioSource.outputAudioMixerGroup = output;

            audioSource.mute = mute;
            audioSource.bypassEffects = bypassEffect;
            audioSource.bypassListenerEffects = bypassListenerEffect;
            audioSource.bypassReverbZones = bypassReverbZones;
            audioSource.playOnAwake = playOnAwake;
            audioSource.loop = loop;

            audioSource.priority = priority;
            audioSource.volume = volume;
            audioSource.pitch= pitch;
            audioSource.panStereo = stereoPan;
            audioSource.spatialBlend = spatialBlend;
            audioSource.reverbZoneMix = reverbZoneMix;
        }

        [HorizontalGroup("Buttons"), Button, PropertyOrder(-1)]
        void Play()
        {
#if UNITY_EDITOR
            audioSource = GetComponent<AudioSource>();
#endif
            foreach (var pack in audioPacks)
                pack.Play(audioSource);
        }
    }
}
