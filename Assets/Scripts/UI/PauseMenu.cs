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
        EventTrigger restartBut;

        [SerializeField]
        EventTrigger quitBut;

        [Header("SFX")]

        [SerializeField]
        AudioSourceRandom showAudioSource;

        [SerializeField]
        AudioSourceRandom buttonAudioSource;

        #endregion

        CanvasGroup canvasGroup;
        Animator animator;
        int boo_show;
        List<EventTrigger> buttons = new();

        public Action OnResume;
        public Action OnRestart;
        public Action OnQuit;
        public Action<string> OnLoadScene;

        public void Init()
        {
            var canvas = GetComponent<Canvas>();
            canvas.worldCamera = Camera.main;

            canvasGroup = GetComponent<CanvasGroup>();
            animator = GetComponent<Animator>();
            boo_show = Animator.StringToHash(nameof(boo_show));

            EventTrigger.Entry resumeEntry = new();
            resumeEntry.callback.AddListener((data) => { Resume(); });
            resumeBut.triggers.Add(resumeEntry);

            EventTrigger.Entry settingsEntry = new();
            settingsEntry.callback.AddListener((data) => { Settings(); });
            settingsBut.triggers.Add(settingsEntry);

            EventTrigger.Entry restartEntry = new();
            restartEntry.callback.AddListener((data) => { Restart(); });
            restartBut.triggers.Add(restartEntry);

            EventTrigger.Entry quitEntry = new();
            quitEntry.callback.AddListener((data) => { Quit(); });
            quitBut.triggers.Add(quitEntry);

            buttons = new() { resumeBut, settingsBut, restartBut, quitBut };
            foreach (var button in buttons)
            {
                EventTrigger.Entry entry = new();
                entry.eventID = EventTriggerType.PointerEnter;
                entry.callback.AddListener((data) => { buttonAudioSource.Play(); });
                button.triggers.Add(entry);
            }

            Show(false, playSFX: false);
        }

        public void Show(bool isShowing, bool playSFX = true)
        {
            canvasGroup.interactable = isShowing;
            canvasGroup.blocksRaycasts = isShowing;
            animator.SetBool(boo_show, isShowing);

            if (playSFX)
                showAudioSource.Play();
        }

        public void Resume()
        {
            OnResume?.Invoke();
        }

        public void Settings()
        {
        }

        public void Restart()
        {
            OnRestart?.Invoke();
        }

        public void Quit()
        {
            OnQuit?.Invoke();
        }

        public void LoadScene(string sceneName)
        {
            OnLoadScene?.Invoke(sceneName);
        }


        public void LoadLevel(Level level)
        {
            OnLoadScene?.Invoke(level.SceneName);
        }
    }
}
