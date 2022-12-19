using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Phoenix
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorParamSetter : EventTrigger
    {
        [Serializable]
        public class AnimatorEvent
        {
            public enum DataType { Float, Int, Bool, Trigger }

            [SerializeField, HorizontalGroup("1"), LabelWidth(0.1f)]
            EventTriggerType triggerType;
            public EventTriggerType TriggerType => triggerType;


            [SerializeField, HorizontalGroup("1"), LabelWidth(0.1f), SuffixLabel("name", true)]
            string paramName;
            public string ParamName => paramName;

            [SerializeField, HorizontalGroup("2"), LabelWidth(0.1f)]
            DataType dataType;
            public DataType Type => dataType;

            [SerializeField, HorizontalGroup("2"), LabelWidth(0.1f), SuffixLabel("value", true), ShowIf("@"+nameof(dataType)+"=="+nameof(DataType)+"."+nameof(DataType.Int))]
            int intValue;
            public int IntValue => intValue;

            [SerializeField, HorizontalGroup("2"), LabelWidth(0.1f), SuffixLabel("value", true), ShowIf("@"+nameof(dataType)+"=="+nameof(DataType)+"."+nameof(DataType.Float))]
            float floatValue;
            public float FloatValue => floatValue;

            [SerializeField, HorizontalGroup("2"), LabelWidth(0.1f), SuffixLabel("value", true), ShowIf("@"+nameof(dataType)+"=="+nameof(DataType)+"."+nameof(DataType.Bool))]
            bool boolValue;
            public bool BoolValue => boolValue;

            int hash;

            public void Init()
            {
                hash = Animator.StringToHash(paramName);
            }

            public AnimatorEvent(EventTriggerType triggerType, string paramName, int intValue)
            {
                this.triggerType = triggerType;
                this.paramName = paramName;
                this.dataType = DataType.Int;
                this.intValue = intValue;
            }

            public AnimatorEvent(EventTriggerType triggerType, string paramName, float floatValue)
            {
                this.triggerType = triggerType;
                this.paramName = paramName;
                this.dataType = DataType.Float;
                this.floatValue = floatValue;
            }

            public AnimatorEvent(EventTriggerType triggerType, string paramName, bool boolValue)
            {
                this.triggerType = triggerType;
                this.paramName = paramName;
                this.dataType = DataType.Bool;
                this.boolValue = boolValue;
            }

            public AnimatorEvent(EventTriggerType triggerType, string paramName)
            {
                this.triggerType = triggerType;
                this.paramName = paramName;
                this.dataType = DataType.Trigger;
            }

            public void SetParam(Animator animator)
            {
                switch (dataType)
                {
                    case DataType.Float:
                        animator.SetFloat(hash, floatValue);
                        break;
                    case DataType.Int:
                        animator.SetInteger(hash, intValue);
                        break;
                    case DataType.Bool:
                        animator.SetBool(hash, boolValue);
                        break;
                    case DataType.Trigger:
                        animator.SetTrigger(hash);
                        break;
                    default:
                        break;
                }
            }
        }

        [SerializeField]
        List<AnimatorEvent> events = new();

        Animator animator;

        void Start()
        {
            animator = GetComponent<Animator>();
            foreach (var animEvent in events)
                animEvent.Init();
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            foreach (var animEvent in events)
            {
                if(animEvent.TriggerType == EventTriggerType.PointerClick)
                    animEvent.SetParam(animator);
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            foreach (var animEvent in events)
            {
                if (animEvent.TriggerType == EventTriggerType.PointerEnter)
                    animEvent.SetParam(animator);
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            foreach (var animEvent in events)
            {
                if (animEvent.TriggerType == EventTriggerType.PointerExit)
                    animEvent.SetParam(animator);
            }
        }
    }
}
