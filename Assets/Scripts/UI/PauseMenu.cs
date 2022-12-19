using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Phoenix
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(Animator))]
    public class PauseMenu : MonoBehaviour
    {
        #region [Components]

        [SerializeField]
        EventTrigger resumeBut;
        
        [SerializeField]
        EventTrigger settingsBut;

        [SerializeField]
        EventTrigger quitBut;

        [SerializeField]
        Object mainMenuScene;

        #endregion

        CanvasGroup canvasGroup;
        Animator animator;
        int boo_show;

        public void Init(Object mainMenuScene)
        {
            var canvas = GetComponent<Canvas>();
            canvas.worldCamera = Camera.main;

            this.mainMenuScene = mainMenuScene;

            canvasGroup = GetComponent<CanvasGroup>();
            animator = GetComponent<Animator>();
            boo_show = Animator.StringToHash(nameof(boo_show));

            EventTrigger.Entry resumeEntry = new();
            resumeEntry.eventID = EventTriggerType.PointerClick;
            resumeEntry.callback.AddListener((data) => { OnResume(); });
            resumeBut.triggers.Add(resumeEntry);

            EventTrigger.Entry settingsEntry = new();
            settingsEntry.callback.AddListener((data) => { OnSettings(); });
            settingsBut.triggers.Add(settingsEntry);

            EventTrigger.Entry quitEntry = new();
            quitEntry.callback.AddListener((data) => { OnQuit(); });
            quitBut.triggers.Add(quitEntry);

            Show(false);
        }

        public void Show(bool isShowing)
        {
            canvasGroup.interactable = isShowing;
            canvasGroup.blocksRaycasts = isShowing;
            animator.SetBool(boo_show, isShowing);

            if (isShowing)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1f;
            }
        }

        public void OnResume()
        {
            Show(false);
        }

        public void OnSettings()
        {
        }

        public void OnQuit()
        {
            SceneManager.LoadScene(mainMenuScene.name);
        }
    }
}
