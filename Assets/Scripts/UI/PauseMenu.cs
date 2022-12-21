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



        #endregion

        CanvasGroup canvasGroup;
        Animator animator;
        int boo_show;

        public Action OnResume;
        public Action OnQuit;

        public void Init()
        {
            var canvas = GetComponent<Canvas>();
            canvas.worldCamera = Camera.main;

            canvasGroup = GetComponent<CanvasGroup>();
            animator = GetComponent<Animator>();
            boo_show = Animator.StringToHash(nameof(boo_show));

            EventTrigger.Entry resumeEntry = new();
            resumeEntry.eventID = EventTriggerType.PointerClick;
            resumeEntry.callback.AddListener((data) => { Resume(); });
            resumeBut.triggers.Add(resumeEntry);

            EventTrigger.Entry settingsEntry = new();
            settingsEntry.callback.AddListener((data) => { Settings(); });
            settingsBut.triggers.Add(settingsEntry);

            EventTrigger.Entry quitEntry = new();
            quitEntry.callback.AddListener((data) => { Quit(); });
            quitBut.triggers.Add(quitEntry);

            Show(false);
        }

        public void Show(bool isShowing)
        {
            canvasGroup.interactable = isShowing;
            canvasGroup.blocksRaycasts = isShowing;
            animator.SetBool(boo_show, isShowing);
        }

        public void Resume()
        {
            OnResume?.Invoke();
        }

        public void Settings()
        {
        }

        public void Quit()
        {
            OnQuit?.Invoke();
        }
    }
}
