using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    [RequireComponent(typeof(Canvas))]
    public class CursorDisplayer : MonoBehaviour
    {
        #region [Vars: Components]

        GameObject cursor;

        #endregion


        #region [Vars: Data Handlers]

        Vector2 pointerPos;
        float speed;

        #endregion

        public Action<Vector2> OnCursorPosition;

        #region [Methods: Initialization]

        public void Init(ref Action<Vector2> onPointerPos, JetPropertiesStatic jetProperties)
        {
            Init(ref onPointerPos, jetProperties.CursorSpeed, jetProperties.Cursor);
        }

        public void Init(ref Action<Vector2> onPointerPos, float speed, GameObject cursorPrefab)
        {
            onPointerPos += (pos) => { pointerPos = pos; };
            this.speed = speed;

            var canvas = GetComponent<Canvas>();
            canvas.worldCamera = Camera.main;

            cursor = Instantiate(cursorPrefab, transform);
        }

        #endregion

        void Update()
        {
            SetCursorPosition();
        }

        /// <summary>
        /// Left, Bottom is (0,0); Right, Top is the screen's max witdh and height
        /// </summary>
        /// <param name="newPos"></param>
        void SetCursorPosition()
        {
            cursor.transform.position = Vector2.Lerp(cursor.transform.position, pointerPos, speed);
            OnCursorPosition?.Invoke(cursor.transform.position);
        }
    }
}
