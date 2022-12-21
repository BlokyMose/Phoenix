using FlyingWormConsole3.LiteNetLib.Utils;
using System;
using System.Collections;
using UnityEngine;

namespace Phoenix
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class CursorDisplayer : MonoBehaviour
    {
        [SerializeField]
        protected float cursorScale = 1f;

        protected GameObject cursor;
        protected Vector2 pointerPos;
        protected float speed;
        protected RectTransform rect;
        protected CanvasGroup canvasGroup;

        public virtual void Init(ref Action<Vector2> onPointerPos, ref Action<bool> onFiring, float speed, GameObject cursorPrefab)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            rect = GetComponent<RectTransform>(); 
            var canvas = GetComponent<Canvas>();
            canvas.worldCamera = Camera.main;
            onPointerPos += OnPointerPos;
            onFiring += OnFiring;
            this.speed = speed;

            cursor = Instantiate(cursorPrefab, transform);
            cursor.transform.localScale = new Vector3(cursorScale, cursorScale, cursorScale);

            StartCoroutine(UpdatingCursorPosition());
        }

        public void Show(bool isShow)
        {
            canvasGroup.alpha = isShow ? 1 : 0;
        }

        protected virtual void SetCursorPosition()
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, pointerPos, Camera.main, out Vector2 newPos);
            cursor.transform.position = newPos * rect.localScale;
        }

        public void Exit(ref Action<Vector2> onPointerPos, ref Action<bool> onFiring)
        {
            onPointerPos -= OnPointerPos;
            onFiring -= OnFiring;
        }


        protected virtual void OnPointerPos(Vector2 pos)
        {
            pointerPos = pos;
        }

        protected virtual void OnFiring(bool isFiring)
        {
        }

        protected IEnumerator UpdatingCursorPosition()
        {
            while (true)
            {
                SetCursorPosition();
                yield return null;
            }
        }
    }
}