using Encore.Utility;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Phoenix
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(Animator))]
    public class DialogueCanvas : MonoBehaviour
    {
        [Serializable]
        public class Monologue
        {
            public enum CharacterPos { Left, Right }

            [SerializeField, LabelWidth(0.1f)]
            Character character;
            public Character Character => character;

            [SerializeField, Multiline, LabelWidth(0.1f)]
            string text;
            public string Text => text;

            [SerializeField]
            CharacterPos characterPos;
            public CharacterPos CharPos => characterPos;

            public Monologue(Character character, string text, CharacterPos characterPos)
            {
                this.character = character;
                this.text = text;
                this.characterPos = characterPos;
            }
        }

        [SerializeField]
        List<Monologue> dialogue = new();

        [SerializeField]
        UnityEvent onHidden;

        [Header("Components")]

        [SerializeField]
        Transform bubblesParent;

        [SerializeField]
        Image characterPosLeft;

        [SerializeField]
        Image characterPosRight;

        [SerializeField]
        DialogueBubble bubbleControllerPrefab;

        Animator animator;
        CanvasGroup canvasGroup;
        LevelManager levelManager;
        int currentIndex;
        Monologue currentMonologue => dialogue[currentIndex];
        DialogueBubble currentBubble;
        readonly float destroyDelay = 2f;
        int boo_show;
        bool isShowing = false;
        bool canNext = false;

        public void Init(LevelManager levelManager, List<Monologue> dialogue)
        {
            this.dialogue = dialogue;
            Init(levelManager);
        }

        public void Init(LevelManager levelManager)
        {
            this.levelManager = levelManager;
            levelManager.Player.OnFiring += (isFiring) => { if(isFiring) Next(); };

            for (int i = bubblesParent.childCount - 1; i >= 0; i--)
                Destroy(bubblesParent.GetChild(i).gameObject);

            canvasGroup = GetComponent<CanvasGroup>();
            animator = GetComponent<Animator>();
            boo_show = Animator.StringToHash(nameof(boo_show));

            if (!gameObject.activeInHierarchy)
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                return;
            }
            else
            {
                BeginDialogue();
            }
        }

        void Exit()
        {
            levelManager.Player.OnFiring -= (isFiring) => { if(isFiring) Next(); };
        }

        public void BeginDialogue()
        {
            if (isShowing) return;

            StartCoroutine(Delay(1f));
            IEnumerator Delay(float delay)
            {
                yield return new WaitForSeconds(delay);

                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;

                levelManager.Player.DisplayCursorMenu();
                currentIndex = 0;
                currentBubble = CreateBubble(currentMonologue);
                SetCharacterSprite(currentMonologue.Character.Sprite, currentMonologue.CharPos);

                isShowing = true;
                canNext = true;
            }
        }

        public void Next()
        {
            if (!canNext) return;

            if (currentIndex < dialogue.Count - 1)
            {
                currentIndex++;

                if (currentBubble != null)
                    currentBubble.HideAndDestroy();

                currentBubble = CreateBubble(currentMonologue);
                SetCharacterSprite(currentMonologue.Character.Sprite, currentMonologue.CharPos);
            }
            else
            {
                Hide();
            }
        }

        public void Hide()
        {
            if (!isShowing) return;

            isShowing = false;
            canNext = false;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            levelManager.Player.DisplayCursorGame();
            animator.SetBool(boo_show, false);
            onHidden.Invoke();
            Exit();
        }

        DialogueBubble CreateBubble(Monologue monologue)
        {
            var bubble = Instantiate(bubbleControllerPrefab, bubblesParent);
            bubble.transform.SetSiblingIndex(0);
            bubble.Init(monologue.Character, monologue.Text);
            return bubble;
        }

        void SetCharacterSprite(Sprite sprite, Monologue.CharacterPos characterPos)
        {
            switch (characterPos)
            {
                case Monologue.CharacterPos.Left:
                    characterPosLeft.sprite = sprite;
                    break;
                case Monologue.CharacterPos.Right:
                    characterPosRight.sprite = sprite;
                    break;
                default:
                    break;
            }
        }
    }
}
