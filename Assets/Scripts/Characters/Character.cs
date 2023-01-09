using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    [CreateAssetMenu(fileName = "Ch_", menuName ="SO/Character")]
    public class Character : ScriptableObject
    {
        [SerializeField, PreviewField]
        Sprite sprite;
        public Sprite Sprite => sprite;

        [SerializeField, PreviewField]
        Sprite characterIcon;
        public Sprite CharacterIcon => characterIcon;

        [SerializeField]
        string characterName;
        public string CharacterName => characterName;

        [SerializeField]
        Color color;
        public Color Color => color;
    }
}
