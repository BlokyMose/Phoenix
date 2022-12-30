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

        public void Init(Element element, ref Func<float,Element,float> getProcessedDamage)
        {
            this.element = element;
            getProcessedDamage = (damage, otherElement) => { return element.GetDamage(damage, otherElement); };

        }

        public void Init(ref Action<Element> onSwitchElement)
        {
            onSwitchElement += (newElement) => 
            { 
                this.element = newElement; 
                OnNewElement?.Invoke(element); 
            };
        }

        public void Init(ShieldProperties properties)
        {
            element = properties.Element;
        }
    }
}
