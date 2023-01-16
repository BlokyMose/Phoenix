using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Phoenix
{
    public class LoadStage : MonoBehaviour
    {
        [SerializeField] GameObject[] _stagePrefabs;

        void Start()
        {
            int select = PlayerPrefs.GetInt("StageSelect", 0);
            GameObject stageobj = _stagePrefabs[select];
            Instantiate(stageobj, transform.position, Quaternion.identity);
        }
        public void StageSelect()
        {
            SceneManager.LoadScene("SelectScene");
        }
    }
}
