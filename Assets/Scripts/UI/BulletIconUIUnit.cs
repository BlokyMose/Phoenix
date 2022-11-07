using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Phoenix
{
    [RequireComponent(typeof(Animator))]
    public class BulletIconUIUnit : MonoBehaviour
    {
        public enum Mode { Invisble, Faded, Idle }

        [SerializeField]
        Image iconImage;

        Animator animator;
        int int_mode;
        BulletProperties properties;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            int_mode = Animator.StringToHash(nameof(int_mode));
        }

        public void Init(BulletProperties properties)
        {
            this.properties = properties;
            iconImage.sprite = properties.Icon;
        }

        public void PlayAnimation(Mode mode)
        {
            animator.SetInteger(int_mode, (int)mode);
        }
    }
}
