using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Phoenix.MinimapController;

namespace Phoenix
{
    public class MinimapController : MonoBehaviour
    {
        public enum TrackerLineOrientation { Horizontal, Vertical }

        [SerializeField]
        float timer = 5f;

        [Header("Components")]
        [SerializeField]
        Image movingObject;

        [SerializeField]
        Image destination;

        [SerializeField]
        RectTransform trackerLine;

        [SerializeField]
        TrackerLineOrientation trackerLineOrientation;

        [SerializeField]
        UnityEvent onEnd;

        bool isPlaying = false;

        private void Awake()
        {
            Init(timer);
        }

        public void Init(float timer)
        {
            this.timer = timer;
            isPlaying = true;

            StartCoroutine(Moving());
            IEnumerator Moving()
            {
                var time = 0f;
                var originPos = movingObject.transform.position;
                var trackerLineLengthMax = 0f;
                switch (trackerLineOrientation)
                {
                    case TrackerLineOrientation.Horizontal:
                        trackerLineLengthMax = trackerLine.sizeDelta.x;
                        break;
                    case TrackerLineOrientation.Vertical:
                        trackerLineLengthMax = trackerLine.sizeDelta.y;
                        break;
                }

                while (time < timer)
                {
                    if (isPlaying)
                    {
                        movingObject.transform.position = 
                            (destination.transform.position * time / timer) + (originPos * (timer-time)/timer);

                        switch (trackerLineOrientation)
                        {
                            case TrackerLineOrientation.Horizontal:
                                trackerLine.sizeDelta = new Vector2(trackerLineLengthMax * (timer-time) / timer, trackerLine.sizeDelta.y);
                                break;
                            case TrackerLineOrientation.Vertical:
                                trackerLine.sizeDelta = new Vector2(trackerLine.sizeDelta.x, trackerLineLengthMax * (timer - time) / timer);
                                break;
                        }
                        time += Time.deltaTime;
                    }
                    yield return null;
                }

                onEnd.Invoke();
            }
        }

        public void Play()
        {
            isPlaying = true;
        }

        public void Pause()
        {
            isPlaying = false;
        }
    }
}
