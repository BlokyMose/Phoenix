using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    [CreateAssetMenu(fileName = "curPack_", menuName ="SO/Cursor Pack")]
    public class CursorPack : ScriptableObject
    {

        [SerializeField, PreviewField]
        Sprite normal;
        public Sprite Normal => normal;

        [SerializeField, PreviewField]
        Sprite click;
        public Sprite Click => click;

    }
}
