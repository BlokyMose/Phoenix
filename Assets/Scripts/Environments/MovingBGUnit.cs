using Encore.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Phoenix.MovingBG;

namespace Phoenix
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class MovingBGUnit : MonoBehaviour
    { 
        SpriteRenderer sr;
        public SpriteRenderer SR => sr;
        public Action OnPassedDistance;
        public BG bg;
        public bool isPaused = false;

        Coroutine corMoving;

        public void Init(Transform parent, BG bg, Action onPassedDistance)
        {
            this.bg = bg;
            transform.parent = parent;
            sr = GetComponent<SpriteRenderer>();
            sr.sprite = bg.Sprite;
            sr.sortingOrder = bg.SortingOrder;
            sr.color = bg.Color;
            sr.sortingLayerName = "BG";
            OnPassedDistance += onPassedDistance;
        }

        public void StartMoving(Vector2 startPos, MovementProperties properties, bool isPaused = false)
        {
            this.isPaused = isPaused;
            transform.position = startPos;

            //Debug.Log(nameof(startPos) + " : " + startPos);
            corMoving = this.RestartCoroutine(Moving(transform, properties));

            IEnumerator Moving(Transform transform, MovementProperties properties)
            {
                var finishPos = GetFinishPos();
                while (Vector2.Distance(transform.position, finishPos) > 1)
                {
                    if (!this.isPaused)
                        transform.position = (Vector2)transform.position + properties.speed* Time.deltaTime * properties.direction;
                    yield return null;
                }

                OnPassedDistance?.Invoke();

                Vector2 GetFinishPos()
                {
                    return properties.moveTo switch
                    {
                        Direction4.Right => (Vector2)transform.parent.position + new Vector2(properties.distance, 0),
                        Direction4.Left => (Vector2)transform.parent.position + new Vector2(-properties.distance, 0),
                        Direction4.Up => (Vector2)transform.parent.position + new Vector2(0, properties.distance),
                        Direction4.Down => (Vector2)transform.parent.position + new Vector2(0, -properties.distance),
                        _ => transform.parent.position,
                    };
                }
            }
        }

    }
}
