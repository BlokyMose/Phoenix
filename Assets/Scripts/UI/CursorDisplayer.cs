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
        protected bool isShow;

        public virtual void Init(Brain brain, float speed, GameObject cursorPrefab)
        {
            brain.OnPointerPosInput += SetPointerPos;
            brain.OnFiring += SetFiring;

            canvasGroup = GetComponent<CanvasGroup>();
            rect = GetComponent<RectTransform>(); 
            var canvas = GetComponent<Canvas>();
            canvas.worldCamera = Camera.main;
            this.speed = speed;

            cursor = Instantiate(cursorPrefab != null ? cursorPrefab : new GameObject(), transform);
            cursor.transform.localScale = new Vector3(cursorScale, cursorScale, cursorScale);

            StartCoroutine(UpdatingCursorPosition());
        }

        public void Show(bool isShow)
        {
            this.isShow = isShow;
            canvasGroup.alpha = isShow ? 1 : 0;
        }

        protected virtual void SetCursorPosition()
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, pointerPos, Camera.main, out Vector2 newPos);
            cursor.transform.position = newPos * rect.localScale;
        }

        public virtual void Exit(Brain brain)
        {
            brain.OnPointerPosInput -= SetPointerPos;
            brain.OnFiring -= SetFiring;
        }


        protected virtual void SetPointerPos(Vector2 pos)
        {
            pointerPos = pos;
        }

        protected virtual void SetFiring(bool isFiring)
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