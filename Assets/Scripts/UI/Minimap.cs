using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Phoenix.Minimap;

namespace Phoenix
{
    public class Minimap : MonoBehaviour, iTimer
    {
        [Serializable]
        public class TimeEvent
        {
            public enum TimeUnit { Second, Percentage }

            [HorizontalGroup("Time"), SerializeField, LabelWidth(0.1f)]
            TimeUnit unit;
            public TimeUnit Unit => unit;

            [HorizontalGroup("Time"), ShowIf("@"+nameof(unit)+"=="+nameof(TimeUnit)+"."+nameof(TimeUnit.Second))]
            [SerializeField, SuffixLabel("sec", true), LabelWidth(0.1f)]
            float time;
            public float Time => time;

            [HorizontalGroup("Time"), ShowIf("@"+nameof(unit) + "==" + nameof(TimeUnit) + "." + nameof(TimeUnit.Percentage))]
            [SerializeField, Range(0,1), LabelWidth(0.1f)]
            float percentage = 0.5f;

            [SerializeField]
            UnityEvent onTime;
            public UnityEvent OnTime => onTime;

            bool isInvoked = false;
            public bool IsInvoked => isInvoked;

            public TimeEvent(float time, UnityEvent onTime)
            {
                this.time = time;
                this.onTime = onTime;
            }

            public void Init(float totalDuration)
            {
                if (unit == TimeUnit.Percentage)
                    time = totalDuration * percentage;
            }

            public void Invoke()
            {
                isInvoked = true;
                onTime.Invoke();
            }

            public void Reset()
            {
                isInvoked = false;
            }
        }

        public enum TrackerLineOrientation { Horizontal, Vertical }

        [SerializeField, SuffixLabel("sec", true)]
        float duration = 5f;

        [Header("Components")]
        [SerializeField]
        Image movingObject;

        [SerializeField]
        Image mapCover;

        [SerializeField]
        Image destination;
        Animator destinationAnimator;

        [SerializeField, Range(0,1)]
        float beepingAfter = 0.9f;

        [SerializeField]
        RectTransform trackerLine;

        [SerializeField]
        TrackerLineOrientation trackerLineOrientation;

        [SerializeField]
        List<TimeEvent> timeEvents = new();

        [SerializeField]
        UnityEvent onEnd;

        bool isPlaying = false;
        int boo_beeping, boo_arrived, flo_beepingSpeed;
        float time = 0f;

        public float Duration
        {
            get => duration;
            set => duration = value;
        }

        public float TimeElapsed
        {
            get => time;
            set => time = value;
        }

        public float TimeRemaining
        {
            get => duration - time;
        }

        public void Init()
        {
            destinationAnimator = destination.GetComponent<Animator>();
            boo_beeping = Animator.StringToHash(nameof(boo_beeping));
            boo_arrived = Animator.StringToHash(nameof(boo_arrived));
            flo_beepingSpeed = Animator.StringToHash(nameof(flo_beepingSpeed));

            isPlaying = true;

            foreach (var timeEvent in timeEvents)
                timeEvent.Init(this.duration);

            StartCoroutine(Moving());
            IEnumerator Moving()
            {
                time = 0f;
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

                foreach (var timeEvent in timeEvents)
                    timeEvent.Reset();

                destinationAnimator.SetBool(boo_beeping, false);
                destinationAnimator.SetBool(boo_arrived, false);

                while (time < this.duration)
                {
                    if (isPlaying && UnityEngine.Time.timeScale > 0f)
                    {
                        movingObject.transform.position = 
                            (destination.transform.position * time / this.duration) + (originPos * (this.duration - time) / this.duration);

                        mapCover.fillAmount = time / this.duration;

                        switch (trackerLineOrientation)
                        {
                            case TrackerLineOrientation.Horizontal:
                                trackerLine.sizeDelta = new Vector2(trackerLineLengthMax * (this.duration - time) / this.duration, trackerLine.sizeDelta.y);
                                break;
                            case TrackerLineOrientation.Vertical:
                                trackerLine.sizeDelta = new Vector2(trackerLine.sizeDelta.x, trackerLineLengthMax * (this.duration - time) / this.duration);
                                break;
                        }

                        foreach (var timeEvent in timeEvents)
                            if(timeEvent.Time < time && !timeEvent.IsInvoked)
                                timeEvent.Invoke();

                        if (time / this.duration > beepingAfter)
                        {
                            destinationAnimator.SetBool(boo_beeping, true);
                            destinationAnimator.SetFloat(flo_beepingSpeed, (time - this.duration * beepingAfter) / (this.duration - this.duration * beepingAfter) + 1f);
                        }

                        time += UnityEngine.Time.deltaTime;
                    }
                    yield return null;
                }
                destinationAnimator.SetBool(boo_arrived, true);

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
