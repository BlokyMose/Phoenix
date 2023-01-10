using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    [RequireComponent(typeof(ElementContainer))]
    public class ElementSwitcher : MonoBehaviour
    {
        [SerializeField, ListDrawerSettings(Expanded = true)]
        List<Element> elements = new List<Element>();

        int currentElementIndex = 0;

        Action<Element> OnElementSwitched;  

        void Awake()
        {
            var elementContainer = GetComponent<ElementContainer>();
            elementContainer.Init(ref OnElementSwitched);
            bool foundSameElement = false;
            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i] == elementContainer.Element)
                {
                    currentElementIndex = i;
                    foundSameElement = true;
                    break;
                }
            }

            if (!foundSameElement)
                currentElementIndex = -1;
        }

        void OnDestroy()
        {
            var elementContainer = GetComponent<ElementContainer>();
            elementContainer.Exit(ref OnElementSwitched);
        }


        public void SwitchToNextElement()
        {
            currentElementIndex = (currentElementIndex + 1) % elements.Count;
            OnElementSwitched?.Invoke(elements[currentElementIndex]);
        }

    }
}
