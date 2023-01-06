using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Phoenix
{
    public class SideDialogueBubbleController : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI dialogueText;

        Animator animator;
        int boo_show;
        readonly float destroyDelay = 1f;

        public void Init(Character character, string text)
        {
            Init(text);
        }

        public void Init(string text)
        {
            animator = GetComponent<Animator>();
            boo_show = Animator.StringToHash(nameof(boo_show));
            dialogueText.text = text;
        }

        public void HideAndDestroy()
        {
            animator.SetBool(boo_show, false);
            Destroy(gameObject, destroyDelay);
        }
    }
}
