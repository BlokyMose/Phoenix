using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Phoenix
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorParamSetter : EventTrigger
    {
        [Serializable]
        public class AnimatorParameterEvent : GameplayUtilityClass.AnimatorParameterStatic
        {
            [SerializeField, HorizontalGroup("1"), LabelWidth(0.1f)]
            EventTriggerType triggerType;
            public EventTriggerType TriggerType => triggerType;

            public AnimatorParameterEvent(EventTriggerType triggerType, string paramName, int intValue) : base(paramName, intValue)
            {
                this.triggerType = triggerType;
            }

            public AnimatorParameterEvent(EventTriggerType triggerType, string paramName, float floatValue) : base(paramName, floatValue)
            {
                this.triggerType = triggerType;
            }

            public AnimatorParameterEvent(EventTriggerType triggerType, string paramName, bool boolValue) : base(paramName, boolValue)
            {
                this.triggerType = triggerType;
            }

            public AnimatorParameterEvent(EventTriggerType triggerType, string paramName) : base(paramName)
            {
                this.triggerType = triggerType;
            }

        }

        [SerializeField]
        List<AnimatorParameterEvent> events = new();

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
