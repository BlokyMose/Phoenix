using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    [CreateAssetMenu(menuName = "SO/Level/Level")]
    public class Level : ScriptableObject
    {
        [SerializeField]
        string levelDisplayName;
        public string LevelDisplayName => levelDisplayName;

        [SerializeField, OnValueChanged(nameof(ChangeLevelNameByLevelObject))]
        Object sceneObject;

        public Object LevelObject => sceneObject;

        [SerializeField, ReadOnly]
        string sceneName;
        public string SceneName => sceneName;

        [SerializeField, PreviewField]
        Sprite seal;
        public Sprite Seal => seal;

        [SerializeField]
        Color color;
        public Color Color => color;

        void ChangeLevelNameByLevelObject() => sceneName = sceneObject.name;
    }
}
