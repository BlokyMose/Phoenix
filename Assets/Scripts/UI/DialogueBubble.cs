using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Phoenix
{
    [RequireComponent(typeof(Animator))]
    public class DialogueBubble : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI nameText;

        [SerializeField]
        TextMeshProUGUI dialogueText;

        [SerializeField]
        Image highlightImage;

        Animator animator;
        int boo_show;
        readonly float destroyDelay = 1f;

        public void Init(Character character, string text)
        {
            Init(character.CharacterName, character.Color, text);
        }

        public void Init(string name, Color color, string text)
        {
            animator = GetComponent<Animator>();
            boo_show = Animator.StringToHash(nameof(boo_show));
            nameText.text = name;
            dialogueText.text = text;
            highlightImage.color = new Color(color.r, color.g, color.b, highlightImage.color.a);
        }

        public void HideAndDestroy()
        {
            animator.SetBool(boo_show, false);
            Destroy(gameObject, destroyDelay);
        }

    }
}
