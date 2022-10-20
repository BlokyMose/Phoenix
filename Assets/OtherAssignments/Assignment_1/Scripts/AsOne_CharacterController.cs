using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AsOne
{
    public class AsOne_CharacterController : MonoBehaviour
    {
        #region [Classes]

        [System.Serializable]
        public class BoxInput
        {
            [SerializeField]
            BoxCollider2D boxCollider;
            public BoxCollider2D BoxCollider => boxCollider;

            [SerializeField]
            SpriteRenderer sr;
            public SpriteRenderer SR => sr;

            Color originalColor;

            [SerializeField]
            Vector2 direction;
            public Vector2 Direction => direction;

            public BoxInput(BoxCollider2D boxCollider, SpriteRenderer sr, Vector2 direction)
            {
                this.boxCollider = boxCollider;
                this.sr = sr;
                this.direction = direction;
                originalColor = sr.color;
            }

            public void TryGetSR()
            {
                sr = boxCollider.GetComponent<SpriteRenderer>();
            }

            public void SetCurrentColorToOriginalColor()
            {
                if (sr == null) return;
                originalColor = sr.color;
            }

            public void Hovered()
            {
                if (sr == null) return;
                ReturnToOriginalColor();
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
            }

            public void Unhovered()
            {
                if (sr == null) return;
                ReturnToOriginalColor();
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.33f);
            }

            public void Deactivate()
            {
                if (sr == null) return;
                ReturnToOriginalColor();
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f);
            }

            public void Attack()
            {
                if (sr == null) return;
                sr.color = new Color(0.8f, 0.2f, 0.2f, 0.8f);
            }

            public void ReturnToOriginalColor()
            {
                if (sr == null) return;
                sr.color = originalColor;
            }
        }

        #endregion

        #region [Vars: Properties]


        [SerializeField]
        int moveDistance = 1;

        [SerializeField]
        float border = 4.5f;

        [SerializeField]
        List<BoxInput> allBoxInputs = new List<BoxInput>();
        public List<BoxInput> AllBoxInputs => allBoxInputs;


        #endregion

        #region [Vars: Data Handlers]

        BoxInput currentBoxInput;

        Vector2 pointerPos;

        bool canAction = false;

        #endregion

        public Action OnActionDone;

        private void Awake()
        {
            foreach (var box in allBoxInputs)
            {
                box.TryGetSR();
                box.SetCurrentColorToOriginalColor();
            }
        }

        public void SetCanAction(bool canAction) { this.canAction = canAction;  }

        public void Move()
        {
            if (!canAction) return;
            if (currentBoxInput == null) return;

            var previousPositon = transform.position;
            transform.position = (Vector2)transform.position + currentBoxInput.Direction * moveDistance;
            if (IsPositionOutsideBorder())
            {
                transform.position = previousPositon;
            }
            else
            {
                OnActionDone?.Invoke();
                canAction = false;
            }

            bool IsPositionOutsideBorder()
            {
                return (transform.position.x <= -border || transform.position.x >= border ||
                    transform.position.y <= -border || transform.position.y >= border);
            }
        }

        public void Attack()
        {
            if (!canAction) return;

            if (currentBoxInput == null) return;

            StartCoroutine(Delay(0.5f));

            OnActionDone?.Invoke();
            canAction = false;

            IEnumerator Delay(float delay)
            {
                currentBoxInput.Attack();
                yield return new WaitForSeconds(delay);
                if (currentBoxInput != null)
                    currentBoxInput.ReturnToOriginalColor();
            }
        }

        public void OnPointerWorldPos(Vector2 pointerPos)
        {
            this.pointerPos = pointerPos;

            UnhoverAllBoxInputs();
            currentBoxInput = CheckMouseInsideAnyBoxInputs();


            void UnhoverAllBoxInputs()
            {
                foreach (var box in allBoxInputs)
                {
                    box.Unhovered();
                }
            }

            BoxInput CheckMouseInsideAnyBoxInputs()
            {
                foreach (var box in allBoxInputs)
                {
                    if (IsMouseInsideBox(pointerPos, box))
                    {
                        box.Hovered();
                        return box;
                    }
                }

                return null;
            }
        }



        bool IsMouseInsideBox(Vector2 pointerPos, BoxInput box)
        {
            return (pointerPos.x < box.BoxCollider.transform.position.x + box.BoxCollider.size.x / 2 &&
                    pointerPos.x > box.BoxCollider.transform.position.x - box.BoxCollider.size.x / 2 &&
                    pointerPos.y < box.BoxCollider.transform.position.y + box.BoxCollider.size.x / 2 &&
                    pointerPos.y > box.BoxCollider.transform.position.y - box.BoxCollider.size.x / 2);

        }
    }
}
