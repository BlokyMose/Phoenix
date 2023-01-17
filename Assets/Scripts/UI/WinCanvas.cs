using Encore.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Phoenix
{
    [RequireComponent(typeof(Canvas))]
    public class WinCanvas : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField]
        TextMeshProUGUI titleText;

        [SerializeField]
        TextMeshProUGUI pointText;

        [SerializeField]
        Image levelSealImage;

        [SerializeField]
        Image gradeImage;

        [SerializeField]
        AudioSourceRandom buttonAudioSource;

        [Header("Buttons")]
        [SerializeField]
        EventTrigger nextLevelBut;

        [SerializeField]
        EventTrigger restartBut;

        [SerializeField]
        EventTrigger mainMenuBut;

        List<EventTrigger> buttons = new();

        public Action OnMainMenu;
        public Action OnRestart;
        public Action OnGoToNextLevel;

        void Awake()
        {
            EventTrigger.Entry nextLevelBut_entry_click = new EventTrigger.Entry();
            nextLevelBut_entry_click.callback.AddListener((data) => OnGoToNextLevel?.Invoke());
            nextLevelBut.triggers.Add(nextLevelBut_entry_click);

            EventTrigger.Entry restartBut_entry_click = new EventTrigger.Entry();
            restartBut_entry_click.callback.AddListener((data) => OnRestart?.Invoke());
            restartBut.triggers.Add(restartBut_entry_click);

            EventTrigger.Entry mainMenuBut_entry_click = new EventTrigger.Entry();
            mainMenuBut_entry_click.callback.AddListener((data) => OnMainMenu?.Invoke());
            mainMenuBut.triggers.Add(mainMenuBut_entry_click);

            buttons = new() { nextLevelBut, restartBut, mainMenuBut };
            foreach (var button in buttons)
            {
                EventTrigger.Entry entry = new();
                entry.eventID = EventTriggerType.PointerEnter;
                entry.callback.AddListener((data) => { buttonAudioSource.Play(); });
                button.triggers.Add(entry);
            }
        }

        public void Init(Level level, LevelGradingRules gradingRules, LevelGradingRules.Score score)
        {
            var canvas = GetComponent<Canvas>();
            canvas.worldCamera = Camera.main;

            titleText.text = level.LevelDisplayName;
            levelSealImage.sprite = level.Seal;
            levelSealImage.color = level.Color.ChangeAlpha(0.3f);

            var totalPoint = gradingRules.LevelScoringRules.Evaluate(score);
            var grade = gradingRules.Evalutate(totalPoint);
            gradeImage.sprite = grade.Seal;

            var timeRemaining = MathUtility.SecondsToTimeString(score.timeRemaining);
            pointText.text = 
                "Kill: <pos=35%>"       + score.killCount   + "<pos=50%><size=24> x"    + gradingRules.LevelScoringRules.killPoint          + "pt</size>" + "\r\n" +
                "Damaged: <pos=35%>"    + score.damaged     + "<pos=50%><size=24> x"    + gradingRules.LevelScoringRules.damagedPoint       + "pt</size>" + "\r\n" +
                "Time left: <pos=35%>"  + timeRemaining     + "<pos=50%><size=24> x"    + gradingRules.LevelScoringRules.timeRemainingPoint + "pt</size>" + "\r\n" +
                "<size=52>Total: <pos=35%>" + totalPoint+ " pt</size>";

            if (OnGoToNextLevel == null)
                nextLevelBut.gameObject.SetActive(false);
        }

        
    }
}
