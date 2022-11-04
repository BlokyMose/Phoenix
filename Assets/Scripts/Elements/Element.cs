using Encore.Utility;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using static UnityEditor.Rendering.FilterWindow;

namespace Phoenix
{
    [CreateAssetMenu(menuName ="SO/Element/Element", fileName ="Element_")]
    public class Element : ScriptableObject
    {
        [System.Serializable]
        public class Rule
        {
            [HorizontalGroup("1")]
            [SerializeField, LabelWidth(0.1f)]
            Element element;
            public Element Element => element;

            [HorizontalGroup("1")]
            [SerializeField, LabelWidth(15), LabelText("x"), SuffixLabel("damage", true)]
            float damageMultiplication = 1;
            public float DamageMultiplication => damageMultiplication;

            public Rule(Element element, float damageMultiplication = 1)
            {
                this.element = element;
                this.damageMultiplication = damageMultiplication;
            }
        }

        [SerializeField]
        string elementName = "";
        public string ElementName => elementName;

        [SerializeField, ColorUsage(true,true)]
        Color color = Color.white;
        public Color Color => color;

        [SerializeField, ListDrawerSettings(Expanded = true)]
        List<Rule> rules = new List<Rule>();
        public List<Rule> Rules => rules;

        public float GetDamage(float damage, Element otherElement)
        {
            if (otherElement == null)
                return damage;

            foreach (var rule in rules)
            {
                if (rule.Element == otherElement)
                    return damage * rule.DamageMultiplication;
            }


            return damage;
        }

        [Button]
        void AddAllElementsToList()
        {
#if UNITY_EDITOR

            var allElementGUIDs = UnityEditor.AssetDatabase.FindAssets("t:" + nameof(Element));

            foreach (var guid in allElementGUIDs)
            {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var element = UnityEditor.AssetDatabase.LoadAssetAtPath<Element>(path);
                var doAdd = true;
                foreach (var rule in rules)
                    if (rule.Element == element) doAdd = false;

                if (doAdd)
                    rules.Add(new Rule(element));
            }

            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

    }
}
