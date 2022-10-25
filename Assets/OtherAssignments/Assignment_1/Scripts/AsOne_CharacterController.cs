using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AsOne
{
    public class AsOne_CharacterController : MonoBehaviour
    {
        #region [Classes]

        [Serializable]
        public class BoxInput
        {
            [SerializeField]
            BoxCollider2D boxCollider;
            public BoxCollider2D BoxCollider => boxCollider;

            [SerializeField]
            SpriteRenderer sr;
            public SpriteRenderer SR => sr;

            public enum MoveDirection { Up, Down, Right, Left }

            [SerializeField]
            MoveDirection direction;
            public Vector2 Direction
            {
                get
                {
                    switch (direction)
                    {
                        case MoveDirection.Up: 
                            return Vector2.up;
                        case MoveDirection.Down:
                            return Vector2.down;
                        case MoveDirection.Right:
                            return Vector2.right;
                        case MoveDirection.Left:
                            return Vector2.left;
                        default:
                            return Vector2.zero;
                    }
                }
            }

            Color originalColor;
            bool isActivated = true;

            public BoxInput(BoxCollider2D boxCollider, SpriteRenderer sr, MoveDirection direction)
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
                if (sr == null || !isActivated) return;
                ReturnToOriginalColor();
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
            }

            public void Unhovered()
            {
                if (sr == null || !isActivated) return;
                ReturnToOriginalColor();
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.33f);
            }

            public void Activate()
            {
                if (sr == null) return;
                isActivated = true;
                ReturnToOriginalColor();
            }

            public void Deactivate()
            {
                if (sr == null) return;
                isActivated = false;
                ReturnToOriginalColor();
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f);
            }

            public void Attack()
            {
                if (sr == null || !isActivated) return;
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
        int maxHealth = 100;

        [SerializeField]
        int attackDamage = 10;

        [SerializeField]
        int maxActionCount = 1;
        public int ActionCount => maxActionCount;

        [SerializeField, ReadOnly]
        int moveDistance = 1;

        [SerializeField, ReadOnly]
        float border = 4.5f;

        [SerializeField]
        List<BoxInput> boxInputs = new List<BoxInput>();
        public List<BoxInput> AllBoxInputs => boxInputs;

        #endregion

        #region [Vars: Data Handlers]

        BoxInput currentBoxInput;

        Vector2 pointerPos;

        int currentActionCount;

        int currentHealth;

        Color originalColor;


        #endregion

        public Action OnActionDone;
        public Action<int> OnAction;
        public Func<List<AsOne_CharacterController>> GetCharacters;

        private void Awake()
        {
            foreach (var box in boxInputs)
            {
                box.TryGetSR();
                box.SetCurrentColorToOriginalColor();
                box.Deactivate();
            }

            currentHealth = maxHealth;
        }

        public void Move(Vector2 direction)
        {
            foreach (var box in boxInputs)
            {
                if(box.Direction == direction)
                {
                    OnPointerWorldPos(box.BoxCollider.transform.position);
                    Move();
                    break;
                }
            }
        }

        public void Move()
        {
            if (currentActionCount <= 0) return;
            if (currentBoxInput == null) return;

            var previousPositon = transform.position;
            transform.position = (Vector2)transform.position + currentBoxInput.Direction * moveDistance;
            if (IsPositionInvalid())
            {
                transform.position = previousPositon;
            }
            else
            {
                DecreaseCurrentActionCount();
            }

            bool IsPositionInvalid()
            {
                if (transform.position.x <= -border || transform.position.x >= border ||
                    transform.position.y <= -border || transform.position.y >= border)
                    return true;

                if (GetCharacters != null)
                    foreach (var character in GetCharacters())
                    {
                        if (character != this && transform.position == character.transform.position)
                            return true;
                    }
                return false;
            }
        }

        public void Attack()
        {
            if (currentActionCount <= 0) return;

            if (currentBoxInput == null) return;

            if(GetCharacters != null)
                foreach (var character in GetCharacters())
                {
                    if (character.transform.position == currentBoxInput.BoxCollider.transform.position)
                    {
                        character.ReceiveAttack(attackDamage);
                        break;
                    }
                }
            currentBoxInput.Attack();
            DecreaseCurrentActionCount();
        }

        public void ReceiveAttack(int damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
                Die();


            StartCoroutine(TurnRed(0.33f));
            IEnumerator TurnRed(float delay)
            {
                //sr.color = new Color(0.85f, 0.15f, 0.15f, 1f);
                yield return new WaitForSeconds(delay);
                //sr.color = originalColor;
            }
        }

        public void Die()
        {
            currentHealth = 0;
        }

        public void OnPointerWorldPos(Vector2 pointerPos)
        {
            this.pointerPos = pointerPos;

            UnhoverAllBoxInputs();
            currentBoxInput = CheckMouseInsideAnyBoxInputs();


            void UnhoverAllBoxInputs()
            {
                foreach (var box in boxInputs)
                {
                    box.Unhovered();
                }
            }

            BoxInput CheckMouseInsideAnyBoxInputs()
            {
                foreach (var box in boxInputs)
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

        public void SetFullActionCount()
        {
            currentActionCount = maxActionCount;
            foreach (var box in boxInputs)
                box.Activate();
        }

        void DecreaseCurrentActionCount()
        {
            currentActionCount--;
            if (currentActionCount <= 0)
            {
                foreach (var box in boxInputs)
                    box.Deactivate();

                OnActionDone?.Invoke();
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
