using Encore.Utility;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Phoenix
{
    [RequireComponent(typeof(Animator))]
    public class DamageCanvas : MonoBehaviour
    {
        public enum PlayMode { DestroyAfter, Loop }

        [Serializable]
        public class DamageLevelUI
        {
            [SerializeField]
            int healthFrom = 100;
            public int HealthFrom => healthFrom;

            [SerializeField, HorizontalGroup("Color")]
            Color color = Color.white;
            public Color Color => color;

            [SerializeField]
            int animParam = 0;
            public int AnimParam => animParam;

            public DamageLevelUI(int healthFrom, Color color, int animParam)
            {
                this.healthFrom = healthFrom;
                this.color = color;
                this.animParam = animParam;
            }
        }

        [SerializeField]
        TextMeshProUGUI text;

        [HorizontalGroup("PlayMode"), LabelWidth(0.1f)]
        [SerializeField]
        PlayMode playMode;

        [HorizontalGroup("PlayMode"), LabelWidth(0.1f)]
        [SerializeField, ShowIf("@"+nameof(playMode)+"=="+nameof(PlayMode)+"."+nameof(PlayMode.DestroyAfter)), SuffixLabel("sec",true)]
        float destroyAfterDuration = 2f;

        [SerializeField]
        List<DamageLevelUI> damageLevels = new();
        public List<DamageLevelUI> DamageLevels { get => damageLevels; }

        Animator animator;
        int int_mode, boo_loop;

        public void Init(float damage, Color? color = null, int? damageLevelIndex = null, bool isLooping = false)
        {
            transform.SetSiblingIndex(100);

            ArrangeHealthStagesFromHighest();
            animator = GetComponent<Animator>();
            int_mode = Animator.StringToHash(nameof(int_mode));
            boo_loop = Animator.StringToHash(nameof(boo_loop));
            playMode = isLooping ? PlayMode.Loop : PlayMode.DestroyAfter;

            // Setup Damage Level
            DamageLevelUI damageLevelUI = damageLevels.GetLast();
            if (damageLevelIndex == null)
            {
                foreach (var level in damageLevels)
                    if(level.HealthFrom <= damage)
                    {
                        damageLevelUI = level;
                        break;
                    }
            }
            else
            {
                damageLevelUI = damageLevels.GetAt((int)damageLevelIndex, damageLevelUI);
            }

            // Setup Text
            text.text = damage.ToString("#");
            if (color != null)
                text.color = (Color) color;
            else 
                text.color = damageLevelUI.Color;

            // Setup PlayMode
            if (playMode == PlayMode.Loop)
                animator.SetBool(boo_loop, true);
            else
                Destroy(gameObject, destroyAfterDuration);

            animator.SetInteger(int_mode, damageLevelUI.AnimParam);
        }

        public void Exit()
        {
            animator.SetBool(boo_loop, false);
            Destroy(gameObject, destroyAfterDuration);
        }

        void ArrangeHealthStagesFromHighest()
        {
            var newList = new List<DamageLevelUI>();
            for (int i = damageLevels.Count - 1; i >= 0; i--)
            {
                var top = damageLevels[0];
                var topIndex = 0;
                for (int k = 0; k < damageLevels.Count; k++)
                {
                    if (damageLevels[k].HealthFrom >= top.HealthFrom)
                    {
                        top = damageLevels[k];
                        topIndex = k;
                    }
                }

                damageLevels.RemoveAt(topIndex);
                newList.Add(top);
            }
            damageLevels = newList;
        }

    }
}
