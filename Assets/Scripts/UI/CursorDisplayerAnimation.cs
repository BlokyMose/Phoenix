using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Phoenix
{
    public class CursorDisplayerAnimation : CursorDisplayer
    {
        Vector2 lastPointerPos;
        int int_mode;
        Animator cursorAnimator;

        public Action<Vector2> OnCursorPositionWorld;

        #region [Methods: Initialization]

        public void Init(FireController fireController)
        {
            fireController.OnNextBullet += OnNextBullet;
        }

        public void Exit(FireController fireController)
        {
            fireController.OnNextBullet -= OnNextBullet;
        }

        public void Init(Brain brain, JetPropertiesStatic jetProperties)
        {
            Init(brain, jetProperties.CursorSpeed, jetProperties.Cursor);
        }

        public override void Init(Brain brain, float speed, GameObject cursorPrefab)
        {
            base.Init(brain, speed, cursorPrefab);

            cursorAnimator = cursor.GetComponent<Animator>();
            int_mode = Animator.StringToHash(nameof(int_mode));
        }

        protected override void SetFiring(bool isFiring)
        {
            cursorAnimator.SetInteger(int_mode, isFiring ? 1 : 0);
        }

        void OnNextBullet(BulletProperties bulletProperties)
        {
            var allSRs = new List<Image>(cursor.GetComponentsInChildren<Image>());
            foreach (var sr in allSRs)
            {
                sr.color = bulletProperties.Element.Color;
            }
        }

        #endregion


        /// <summary>
        /// Left, Bottom is (0,0); Right, Top is the screen's max witdh and height
        /// </summary>
        /// <param name="newPos"></param>
        protected override void SetCursorPosition()
        {
            base.SetCursorPosition();
            lastPointerPos = Vector2.Lerp(lastPointerPos, pointerPos, speed);
            OnCursorPositionWorld?.Invoke(Camera.main.ScreenToWorldPoint(lastPointerPos));
        }
    }
}
