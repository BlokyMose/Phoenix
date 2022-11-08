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
        BulletProperties bulletProperties;
        public BulletProperties BulletProperties => bulletProperties;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            int_mode = Animator.StringToHash(nameof(int_mode));
        }

        public void Init(BulletProperties bulletProperties)
        {
            this.bulletProperties = bulletProperties;
            iconImage.sprite = bulletProperties.Icon;
        }

        public void PlayAnimation(Mode mode)
        {
            animator.SetInteger(int_mode, (int)mode);
        }
    }
}
