using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Phoenix
{
    [CreateAssetMenu(fileName = "Ch_", menuName ="SO/Character")]
    public class Character : ScriptableObject
    {
        [Serializable]
        public class Frame
        {
            [SerializeField, PreviewField]
            Sprite sprite;
            public Sprite Sprite => sprite;

            [SerializeField]
            float delay = 0.33f;
            public float Delay => delay;

            [SerializeField]
            float delayRange = 0.1f;

            public float RandomizedDelay { get; private set; }

            public void RandomizeDelay() => RandomizedDelay = Random.Range(delay - delayRange, delay + delayRange);
        }

        [SerializeField, PreviewField]
        Sprite sprite;
        public Sprite Sprite => sprite;

        [SerializeField, PreviewField]
        Sprite characterIcon;
        public Sprite CharacterIcon => characterIcon;

        [SerializeField]
        string characterName;
        public string CharacterName => characterName;

        [SerializeField]
        Color color;
        public Color Color => color;

        [Header("Animations")]
        [SerializeField]
        List<Frame> talkFrames = new();
        public List<Frame> TalkFrames => talkFrames;

        public IEnumerator PlayFrames(Image image, List<Frame> frames)
        {
            var time = 0f;
            var frameIndex = 0;
            foreach (var frame in frames)
                frame.RandomizeDelay();

            image.sprite = frames[frameIndex].Sprite;

            while (true)
            {
                if (time > frames[frameIndex].RandomizedDelay)
                {
                    if (frameIndex == frames.Count - 1)
                    {
                        frameIndex = 0;
                        foreach (var frame in frames)
                            frame.RandomizeDelay();
                    }
                    else
                    {
                        frameIndex++;
                    }
                    image.sprite = frames[frameIndex].Sprite;
                    time = 0f;

                }

                time += Time.deltaTime * Time.timeScale;
                yield return null;
            }
        }
    }
}
