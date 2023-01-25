using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using System;
using UnityEngine.UI;
using Encore.Utility;

namespace Phoenix
{
    [RequireComponent(typeof(Animator))]
    public class SideDialogueCanvas : MonoBehaviour
    {
        [Serializable]
        public class Monologue
        {
            [SerializeField, LabelWidth(0.1f)]
            Character character;
            public Character Character => character;

            [SerializeField, Multiline, LabelWidth(0.1f)]
            string text;
            public string Text => text;

            public Monologue(Character character, string text)
            {
                this.character = character;
                this.text = text;
            }
        }

        [SerializeField]
        float readingDuration = 5f;

        [SerializeField]
        List<Monologue> dialogue = new();

        [SerializeField]
        UnityEvent onHidden;

        [Header("Components")]

        [SerializeField]
        Image characterImage;

        [SerializeField]
        Transform bubblesParent;

        [SerializeField]
        SideDialogueBubble bubbleControllerPrefab;

        Animator animator;
        int currentIndex;
        Monologue currentMonologue => dialogue[currentIndex];
        SideDialogueBubble currentBubble;
        readonly float destroyDelay = 2f;
        int boo_show;
        bool isShowing = false;

        private void Awake()
        {
            ResetComponents();
        }

        public void Init()
        {
            gameObject.SetActive(true);
            ResetComponents();

            animator = GetComponent<Animator>();
            boo_show = Animator.StringToHash(nameof(boo_show));

            BeginDialogue();
        }

        void BeginDialogue()
        {
            if (isShowing) return;
            isShowing = true;

            StartCoroutine(Speaking());
            IEnumerator Speaking()
            {
                currentIndex = 0;
                currentBubble = CreateBubble(currentMonologue);
                characterImage.sprite = currentMonologue.Character.CharacterIcon;
                characterImage.color = characterImage.color.ChangeAlpha(1f);
                animator.SetBool(boo_show, true);

                var time = 0f;
                while (true)
                {
                    if (Time.timeScale > 0f)
                    {
                        if (time > readingDuration)
                        {
                            time = 0f;
                            currentIndex++;
                            if (currentIndex == dialogue.Count)
                                break;

                            if (currentBubble != null)
                                currentBubble.HideAndDestroy();

                            currentBubble = CreateBubble(currentMonologue);
                            characterImage.sprite = currentMonologue.Character.Sprite;
                        }

                        time += Time.deltaTime;
                    }

                    yield return null;
                }


                animator.SetBool(boo_show, false);
                isShowing = false;
                onHidden.Invoke();
            }
        }

        SideDialogueBubble CreateBubble(Monologue monologue)
        {
            var bubble = Instantiate(bubbleControllerPrefab, bubblesParent);
            bubble.transform.SetSiblingIndex(0);
            bubble.Init(monologue.Character, monologue.Text);
            return bubble;
        }

        void ResetComponents()
        {
            for (int i = bubblesParent.childCount - 1; i >= 0; i--)
                Destroy(bubblesParent.GetChild(i).gameObject);
            characterImage.sprite = null;
            characterImage.color = characterImage.color.ChangeAlpha(0f);

        }

    }
}
