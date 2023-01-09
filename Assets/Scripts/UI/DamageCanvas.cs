using Encore.Utility;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Phoenix.HealthController;

namespace Phoenix
{
    [RequireComponent(typeof(Animator))]
    public class DamageCanvas : MonoBehaviour
    {
        [Serializable]
        public class DamageLevelUI
        {
            [SerializeField]
            int healthFrom = 100;
            public int HealthFrom => healthFrom;

            [SerializeField, HorizontalGroup("Color"), LabelText("Match Element")]
            bool isColorMatchElement = true;
            public bool IsColorMatchElement => isColorMatchElement;

            [SerializeField, HorizontalGroup("Color"), LabelWidth(0.1f), HideIf(nameof(isColorMatchElement))]
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

        [SerializeField]
        List<DamageLevelUI> damageLevels = new();

        Animator animator;
        int int_mode;

        public void Init(float damage, Color? color = null)
        {
            ArrangeHealthStagesFromHighest();
            animator = GetComponent<Animator>();
            int_mode = Animator.StringToHash(nameof(int_mode));
            DamageLevelUI damageLevelUI = damageLevels.GetLast();
            foreach (var level in damageLevels)
                if(level.HealthFrom <= damage)
                {
                    damageLevelUI = level;
                    break;
                }

            text.text = damage.ToString("#");
            if (damageLevelUI.IsColorMatchElement && color != null)
                text.color = (Color) color;
            else
                text.color = damageLevelUI.Color;

            animator.SetInteger(int_mode, damageLevelUI.AnimParam);
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
