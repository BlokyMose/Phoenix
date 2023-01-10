using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    public class ElementContainer : MonoBehaviour
    {
        [SerializeField]
        Element element;

        public Element Element => element;

        public Action<Element> OnNewElement;

        protected virtual void Awake()
        {
            
        }

        public void Init(Element element, ref Func<float,Element,float> getProcessedDamage)
        {
            this.element = element;
            getProcessedDamage = (damage, otherElement) => { return element.GetDamage(damage, otherElement); };

        }

        public void Init(ref Action<Element> onSwitchElement)
        {
            onSwitchElement += SetNewElement;
        }

        public void Exit(ref Action<Element> onSwitchElement)
        {
            onSwitchElement += SetNewElement;
        }

        public void Init(ShieldProperties properties)
        {
            element = properties.Element;
        }

        public void SetNewElement(Element newElement)
        {
            element = newElement;
            OnNewElement?.Invoke(element);
        }
    }
}
