using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static AsOne.AsOne_GameManager;
using Grid = AsOne.AsOne_GameManager.Grid;

namespace AsOne
{
    public class AsOne_CharacterController : MonoBehaviour
    {
        public enum MoveDirection { Up, Right, Down, Left }

        #region [Vars: Properties]

        [SerializeField]
        Transform spritesParent;

        [SerializeField]
        int maxHealth = 100;

        [Header("Movement")]

        [SerializeField]
        AsOne_MoveModule moveModule;

        [SerializeField]
        AsOne_AttackModule attackNormalModule;
        public AsOne_AttackModule AttackNormalModule => attackNormalModule;

        [SerializeField]
        int maxActionCount = 1;
        public int ActionCount => maxActionCount;

        [SerializeField]
        Vector2Int cellsSize = new Vector2Int(1, 1);

        [Header("Tile Colors")]


        [SerializeField]
        Color moveTileColor = new Color(0.4f, 0.8f, 0.4f, 1f);        
        
        [SerializeField]
        Color hoverTileColor = new Color(0.25f, 0.95f, 0.25f, 1f);

        [SerializeField]
        Color attackTileColor = new Color(0.9f, 0.2f, 0.2f, 0.75f);

        [SerializeField]
        Color receiveAttackTileColor = new Color(0.9f, 0.2f, 0.2f, 0.75f);

        [Header("Animation")]

        [SerializeField]
        Animator animator;

        [SerializeField]
        float walkAnimationDuration = 0.75f;

        [SerializeField]
        float walkAnticipationAnimationDuration = 0.5f;

        [SerializeField]
        float attackNormalAnimationDuration = 0.5f;

        [SerializeField]
        float attackNormalAnticipationAnimationDuration = 0.5f;

        #endregion

        #region [Vars: Data Handlers]

        Vector2 pointerPos;

        int currentActionCount;

        int currentHealth;
        public int CurrentHealth => currentHealth;

        Color originalColor;

        const int moveDistance = 1;

        List<Cell> actionableCells;
        public List<Cell> ActionableCells => actionableCells;

        List<Cell> currentCells;
        public List<Cell> CurrentCells => currentCells;
        Cell currentHoveredCell;

        int tri_walk, tri_attack_normal, tri_attack_heavy, tri_damaged;

        public enum ActionType { None, Move, Attack }
        ActionType currentActionType = ActionType.None;

        Material spritesMaterial;

        #endregion

        public Action OnActionDone;
        public Action<int> OnAction;
        public Func<List<AsOne_CharacterController>> GetCharacters;
        public Func<Grid> GetGrid;

        public void Init(Cell startingCell, 
            Action<int> onAction,
            Action onActionDone,
            Func<List<AsOne_CharacterController>> getCharacters,
            Func<Grid> getGrid
            )
        {
            this.OnAction = onAction;
            this.OnActionDone = onActionDone;
            this.GetCharacters = getCharacters;
            this.GetGrid = getGrid;

            tri_walk = Animator.StringToHash(nameof(tri_walk));
            tri_attack_normal = Animator.StringToHash(nameof(tri_attack_normal));
            tri_attack_heavy = Animator.StringToHash(nameof(tri_attack_heavy));
            tri_damaged = Animator.StringToHash(nameof(tri_damaged));

            currentHealth = maxHealth;
            transform.position = startingCell.Position;
            currentCells = GetGrid().GetCells(startingCell, cellsSize);
            actionableCells = moveModule.GetMoveableCells(startingCell, currentCells, GetGrid());
            actionableCells.AddRange(attackNormalModule.GetAttackableCells(startingCell, currentCells, GetGrid()));

            var spriteRenderers = new List<SpriteRenderer> (spritesParent.GetComponentsInChildren<SpriteRenderer>());
            spritesMaterial = new Material(spriteRenderers[0].material);
            foreach (var sr in spriteRenderers)
            {
                sr.material = spritesMaterial;
            }
        }



        // Current system doesn't fully support simple directions for WASD
        public void Move(MoveDirection direction)
        {
            var moveableCells = moveModule.GetMoveableCells(currentCells[0], currentCells, GetGrid());
            if (moveableCells.Count <= 0) return;

            Cell selectedCell = null;

            switch (direction)
            {
                case MoveDirection.Up:
                    Cell mostTopCell = moveableCells[0];
                    foreach (var cell in moveableCells)
                    {
                        if (cell.GridLocation.y > mostTopCell.GridLocation.y)
                            mostTopCell = cell;
                    }
                    selectedCell = mostTopCell;
                    break;
                case MoveDirection.Right:
                    Cell mostRightCell = moveableCells[0];
                    foreach (var cell in moveableCells)
                    {
                        if (cell.GridLocation.x > mostRightCell.GridLocation.x)
                            mostRightCell = cell;
                    }
                    selectedCell = mostRightCell;
                    break;

                case MoveDirection.Down:
                    Cell mostDownCell = moveableCells[0];
                    foreach (var cell in moveableCells)
                    {
                        if (cell.GridLocation.y > mostDownCell.GridLocation.y)
                            mostDownCell = cell;
                    }
                    selectedCell = mostDownCell;
                    break;
                case MoveDirection.Left:
                    Cell mostLeftCell = moveableCells[0];
                    foreach (var cell in moveableCells)
                    {
                        if (cell.GridLocation.x > mostLeftCell.GridLocation.x)
                            mostLeftCell = cell;
                    }
                    selectedCell = mostLeftCell;
                    break;
            }

            OnPointerWorldPos(selectedCell.Position);
            Move();
        }

        Coroutine corAnimationWalk;

        public void AutoAction()
        {
            switch (currentActionType)
            {
                case ActionType.None:
                    break;
                case ActionType.Move: Move();
                    break;
                case ActionType.Attack: Attack();
                    break;
            }
        }

        public void Move()
        {
            if (currentActionCount <= 0) return;
            if (currentActionType != ActionType.Move) return;
            if (corAnimationWalk != null) return;

            var moveToCell = moveModule.Move(currentCells[0], currentHoveredCell, GetGrid());
            if (moveToCell.Position.x > transform.position.x)
            {
                animator.transform.localEulerAngles = new Vector3(0, 0, 0);
            }
            else if (moveToCell.Position.x < transform.position.x)
            {
                animator.transform.localEulerAngles = new Vector3(0, 180, 0);
            }


            corAnimationWalk = StartCoroutine(AnimatingWalk());
            IEnumerator AnimatingWalk()
            {
                animator.SetTrigger(tri_walk);

                yield return new WaitForSeconds(walkAnticipationAnimationDuration);

                AnimationCurve curveX = AnimationCurve.EaseInOut(0, transform.position.x, walkAnimationDuration, moveToCell.Position.x);
                AnimationCurve curveY = AnimationCurve.EaseInOut(0, transform.position.y, walkAnimationDuration, moveToCell.Position.y);

                float time = 0;
                while(time < walkAnimationDuration)
                {
                    time += Time.deltaTime;
                    transform.position = new Vector2(curveX.Evaluate(time), curveY.Evaluate(time));
                    yield return null;
                }

                transform.position = moveToCell.Position;

                foreach (var cell in actionableCells)
                    cell.ResetColor();

                currentCells = GetGrid().GetCells(moveToCell, cellsSize);
                DecreaseCurrentActionCount();
                corAnimationWalk = null;
            }

        }

        Coroutine corAttackAnimation;
        public void Attack()
        {
            if (corAttackAnimation != null) return;
            if (currentActionCount <= 0) return;
            if (currentActionType != ActionType.Attack) return;

            if (currentHoveredCell.Position.x > transform.position.x)
            {
                animator.transform.localEulerAngles = new Vector3(0, 0, 0);
            }
            else if (currentHoveredCell.Position.x < transform.position.x)
            {
                animator.transform.localEulerAngles = new Vector3(0, 180, 0);
            }
            var attackCharacters = attackNormalModule.Attack(currentCells[0], currentHoveredCell, GetGrid(), GetCharacters());

            corAttackAnimation = StartCoroutine(AttackAnimation());
            IEnumerator AttackAnimation()
            {
                animator.SetTrigger(tri_attack_normal);
                yield return new WaitForSeconds(attackNormalAnticipationAnimationDuration);
                foreach (var character in attackCharacters)
                {
                    character.ReceiveAttack(attackNormalModule.Damage);
                }
                corAttackAnimation = null;
                DecreaseCurrentActionCount();
            }
        }

        public void Attack(Cell targetCell)
        {
            OnPointerWorldPos(targetCell.Position);
            Attack();
        }

        public List<AsOne_CharacterController> GetAttackableCharacters()
        {
            var attackableCells = attackNormalModule.GetAttackableCells(currentCells[0], currentCells, GetGrid());
            var attackableCharacters = new List<AsOne_CharacterController>();
            foreach (var cell in attackableCells)
            {
                foreach (var character in GetCharacters())
                {
                    if (character.CurrentCells.Contains(cell))
                    {
                        attackableCharacters.Add(character);
                    }
                }
            }

            return attackableCharacters;
        }

        public List<Cell> GetAttackableCells()
        {
            var attackableCells = attackNormalModule.GetAttackableCells(currentCells[0], currentCells, GetGrid());
            var accurateAttackableCells = new List<Cell>();
            foreach (var cell in attackableCells)
            {
                foreach (var character in GetCharacters())
                {
                    if (character.CurrentCells.Contains(cell))
                    {
                        accurateAttackableCells.Add(cell);
                    }
                }
            }

            return accurateAttackableCells;
        }

        public void ReceiveAttack(int damage)
        {
            currentHealth -= damage;
            animator.SetTrigger(tri_damaged);
            if (currentHealth <= 0)
                Die();


            StartCoroutine(TurnRed(0.33f));
            IEnumerator TurnRed(float delay)
            {
                SetColor(receiveAttackTileColor);
                yield return new WaitForSeconds(delay);
                ResetColor();
            }
        }

        public void Die()
        {
            currentHealth = 0;
        }

        public void OnPointerWorldPos(Vector2 pointerPos)
        {
            this.pointerPos = pointerPos;

            UnhoverAllActionableCells();
            ResetColorAllCharacter();
            currentHoveredCell = CheckMouseInsideAnyActionalCells();

            void ResetColorAllCharacter()
            {
                foreach (var character in GetCharacters())
                {
                    character.ResetColor();
                }
            }

            Cell CheckMouseInsideAnyActionalCells()
            {
                foreach (var cell in actionableCells)
                {
                    if (IsMouseInsideBox(pointerPos, cell))
                    {
                        var attackableCharacter = attackNormalModule.GetAttackableCharacter(currentCells[0], cell, GetGrid(), GetCharacters());
                        if (attackableCharacter != null)
                        {
                            attackableCharacter.SetColor(attackTileColor);
                            var sameCells = attackNormalModule.GetSameCells(currentCells[0], cell, GetGrid());
                            foreach (var sameCell in sameCells)
                            {
                                sameCell.SetColor(attackTileColor);
                            }
                            currentActionType = ActionType.Attack;
                        }
                        else
                        {
                            var sameCells = moveModule.GetSameCells(currentCells[0], cell, GetGrid());
                            foreach (var sameCell in sameCells)
                            {
                                sameCell.SetColor(hoverTileColor);
                            }
                            currentActionType = ActionType.Move;
                        }

                        return cell;
                    }
                }

                currentActionType = ActionType.None;
                return null;
            }
        }

        void UnhoverAllActionableCells()
        {
            foreach (var cell in actionableCells)
            {
                cell.SetColor(moveTileColor);
            }
        }

        public void SetFullActionCount()
        {
            actionableCells = moveModule.GetMoveableCells(currentCells[0], currentCells, GetGrid());
            actionableCells.AddRange(attackNormalModule.GetAttackableCells(currentCells[0], currentCells, GetGrid()));
            currentActionCount = maxActionCount;
            foreach (var cell in actionableCells)
                cell.SetColor(moveTileColor);
        }

        void DecreaseCurrentActionCount()
        {
            currentActionCount--;
            actionableCells = moveModule.GetMoveableCells(currentCells[0], currentCells, GetGrid());
            actionableCells.AddRange(attackNormalModule.GetAttackableCells(currentCells[0], currentCells, GetGrid()));

            UnhoverAllActionableCells();
            if (currentActionCount <= 0)
            {
                foreach (var cell in actionableCells)
                    cell.ResetColor();

                actionableCells.Clear();
                OnActionDone?.Invoke();
            }
        }

        bool IsMouseInsideBox(Vector2 pointerPos, Cell cell)
        {
            return (pointerPos.x < cell.Position.x + cell.Size.x / 2 &&
                    pointerPos.x > cell.Position.x - cell.Size.x / 2 &&
                    pointerPos.y < cell.Position.y + cell.Size.x / 2 &&
                    pointerPos.y > cell.Position.y - cell.Size.x / 2);

        }

        void SetColor(Color color)
        {
            spritesMaterial.color = color;
        }

        void ResetColor()
        {
            spritesMaterial.color = Color.white;
        }
    }
}
