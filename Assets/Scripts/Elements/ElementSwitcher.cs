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

        private void OnEnable()
        {
            var brain = GetComponent<Brain>();
            if (brain != null)
                Init(brain);
        }

        private void OnDisable()
        {
            var brain = GetComponent<Brain>();
            if (brain != null)
                Disable(brain);
        }

        public void SwitchToNextElement()
        {
            currentElementIndex = (currentElementIndex + 1) % elements.Count;
            OnElementSwitched(elements[currentElementIndex]);
        }

        public void Init(Brain brain)
        {
            //brain.OnElementInput
        }

        public void Disable(Brain brain)
        {

        }
    }
}
