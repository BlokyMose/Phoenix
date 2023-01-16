using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Phoenix
{
    public class StageSelect : MonoBehaviour
    {
        [SerializeField] private GameObject[] _stage;
        private int _selected = 0;

        void Start()
        {
            _selected = PlayerPrefs.GetInt("StageSelect", 0);
            _stage[_selected].SetActive(true);
        }

        public void Right()
        {
            _stage[_selected].SetActive(false);
            _selected++;

            if (_selected >= _stage.Length)
                _selected = 0;
            _stage[_selected].SetActive(true);
        }

        public void Left()
        {
            _stage[_selected].SetActive(false);
            _selected--;

            if (_selected < 0)
                _selected = _stage.Length - 1;
            _stage[_selected].SetActive(true);

        }
        public void SelectBall()
        {
            PlayerPrefs.SetInt("StageSelect", _selected);

            SceneManager.LoadScene("GameScene");
        }
    }
}
