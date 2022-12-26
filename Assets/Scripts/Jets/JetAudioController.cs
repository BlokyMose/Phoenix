using Encore.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace Phoenix
{
    [RequireComponent(typeof(AudioSource))]
    public class JetAudioController : MonoBehaviour
    {
        [SerializeField]
        float pitchMultiplerSpeed = 2f;

        [SerializeField]
        Vector2 volumeRange = new Vector2(0.5f, 1.33f);


        AudioSource audioSource;
        public AudioSource AudioSource => audioSource;

        bool isMoving = false;

        Coroutine corPlaying;

        public void Init(ref Action<Vector2> onMoveDirection)
        {
            audioSource = GetComponent<AudioSource>();
            onMoveDirection += SetIsMoving;

            corPlaying = this.RestartCoroutine(Update());
            IEnumerator Update()
            {
                var originalPitch = audioSource.pitch;
                var multiplier = 0f;
                while (true)
                {
                    audioSource.pitch = originalPitch * multiplier;

                    if (isMoving)
                    {
                        multiplier += Time.deltaTime * pitchMultiplerSpeed;
                        if (multiplier > volumeRange.y)
                            multiplier = volumeRange.y;
                    }
                    else
                    {
                        multiplier -= Time.deltaTime * pitchMultiplerSpeed;
                        if (multiplier < volumeRange.x)
                            multiplier = volumeRange.x;
                    }

                    yield return null;
                }
            }
        }

        public void Exit(ref Action<Vector2> onMoveDirection)
        {
            onMoveDirection -= SetIsMoving;
            StopAllCoroutines();
        }

        void SetIsMoving(Vector2 inputMove)
        {
            isMoving = inputMove.magnitude != 0;
        }
    }
}
