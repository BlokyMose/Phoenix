using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Phoenix
{
    public class LevelLoader : MonoBehaviour
    {
        [InlineEditor]
        public Level level;

        public void Load()
        {
            Load(level);
        }

        public void Load(Level level)
        {
            Load(level.SceneName);
        }

        public void Load(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
