using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Phoenix
{
    [RequireComponent(typeof(Canvas))]
    public class CursorDisplayer : MonoBehaviour
    {
        [SerializeField]
        float cursorScale = 1f;

        GameObject cursor;
        Vector2 pointerPos;
        float speed;
        RectTransform rect;
        Vector2 lastPointerPos;
        int int_mode;


        public Action<Vector2> OnCursorPosition;

        #region [Methods: Initialization]

        public void Init(ref Action<Vector2> onPointerPos, ref Action<bool> onFiring, JetPropertiesStatic jetProperties)
        {
            Init(ref onPointerPos, ref onFiring, jetProperties.CursorSpeed, jetProperties.Cursor);
        }

        public void Init(FireController fireController)
        {
            fireController.OnNextBullet += (bulletProperties) =>
            {
                var allSRs = new List<Image>(cursor.GetComponentsInChildren<Image>());
                foreach (var sr in allSRs)
                {
                    sr.color = bulletProperties.Element.Color;
                }
            };
        }

        public void Init(ref Action<Vector2> onPointerPos, ref Action<bool> onFiring,  float speed, GameObject cursorPrefab)
        {
            rect = GetComponent<RectTransform>();
            onPointerPos += (pos) => { pointerPos = pos; };
            this.speed = speed;

            var canvas = GetComponent<Canvas>();
            canvas.worldCamera = Camera.main;
            cursor = Instantiate(cursorPrefab, transform);
            cursor.transform.localScale = new Vector3(cursorScale, cursorScale, cursorScale);
            var cursorAnimator = cursor.GetComponent<Animator>();
            int_mode = Animator.StringToHash(nameof(int_mode));
            onFiring += (isFiring) =>
            {
                cursorAnimator.SetInteger(int_mode, isFiring ? 1 : 0);  
            };

            var functionalCursorGO = new GameObject("functionalCursor");

            StartCoroutine(Update());
            IEnumerator Update()
            {
                while (true)
                {
                    SetCursorPosition();
                    yield return null;
                }
            }
        }

        #endregion


        /// <summary>
        /// Left, Bottom is (0,0); Right, Top is the screen's max witdh and height
        /// </summary>
        /// <param name="newPos"></param>
        void SetCursorPosition()
        {
            lastPointerPos = Vector2.Lerp(lastPointerPos, pointerPos, speed);

            var newPos = (Vector2)cursor.transform.position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, pointerPos, Camera.main, out newPos);
            cursor.transform.position = newPos*rect.localScale;

            OnCursorPosition?.Invoke(Camera.main.ScreenToWorldPoint(lastPointerPos));
        }
    }
}
