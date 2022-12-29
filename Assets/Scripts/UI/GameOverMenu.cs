using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Phoenix
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(Animator))]

    public class GameOverMenu : MonoBehaviour
    {
        #region [Components]

        [SerializeField]
        EventTrigger restartBut;

        [SerializeField]
        EventTrigger mainMenuBut;

        [SerializeField]
        AudioSourceRandom buttonAudioSource;

        #endregion

        CanvasGroup canvasGroup;
        Animator animator;
        int boo_show;
        List<EventTrigger> buttons = new();

        public Action OnRestart;
        public Action OnMainMenu;

        public void Init()
        {
            var canvas = GetComponent<Canvas>();
            canvas.worldCamera = Camera.main;

            canvasGroup = GetComponent<CanvasGroup>();
            animator = GetComponent<Animator>();
            boo_show = Animator.StringToHash(nameof(boo_show));

            EventTrigger.Entry restartEntry = new();
            restartEntry.callback.AddListener((data) => { Restart(); });
            restartBut.triggers.Add(restartEntry);

            EventTrigger.Entry mainMenuEntry = new();
            mainMenuEntry.callback.AddListener((data) => { OpenMainMenu(); });
            mainMenuBut.triggers.Add(mainMenuEntry);

            buttons = new() { restartBut, mainMenuBut };
            foreach (var button in buttons)
            {
                EventTrigger.Entry entry = new();
                entry.eventID = EventTriggerType.PointerEnter;
                entry.callback.AddListener((data) => { buttonAudioSource.Play(); });
                button.triggers.Add(entry);
            }

            Show(true);
        }

        public void Show(bool isShowing)
        {
            canvasGroup.interactable = isShowing;
            canvasGroup.blocksRaycasts = isShowing;
            animator.SetBool(boo_show, isShowing);
        }

        public void Restart()
        {
            OnRestart?.Invoke();
        }

        public void OpenMainMenu()
        {
            OnMainMenu?.Invoke();
        }
    }
}
