using Encore.Utility;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Phoenix
{
    [DisallowMultipleComponent]
    public abstract class LevelManager: MonoBehaviour
    {
        #region [Classes]

        public enum ScreenMode { Loading, Play, Pause, GameOver, GameOverCanvas, Win, WinCanvas }

        [Serializable]
        public class StartPoint
        {
            public Transform point;
        }

        [Serializable]
        public class Stage
        {
            public enum StageActiveMode { Active, Passive, Inactive }
            public enum NextRequirement { All, Any }

            [SerializeField, GUIColor(nameof(_GetActiveModeColor)), Tooltip("- Active: activate & deactivate objects based on order \n- Passive: deactivate objects at start \n- Inactive: do nothing")]
            StageActiveMode activeMode = StageActiveMode.Active;
            public StageActiveMode ActiveMode => activeMode;

            [SerializeField, LabelText("Next Req."), Tooltip("Requirement proceed to the next stage:\n- All: all object has to be done \n- Any: one object has to be done")]
            NextRequirement nextRequirement = NextRequirement.All;

            [SerializeField]
            List<LevelObjectActivator> objectActivators = new();
            public List<LevelObjectActivator> ObjectActivators => objectActivators;

            LevelManager levelManager;
            int activatedObjectsCount = 0;
            bool isStageCleared = false;
            public Action OnStageCleared;


            #region [Methods: Inspector]

            Color _GetActiveModeColor()
            {
                switch (activeMode)
                {
                    case StageActiveMode.Active:
                        return Encore.Utility.ColorUtility.lightGreen;
                    case StageActiveMode.Passive:
                        return Encore.Utility.ColorUtility.orange;
                    case StageActiveMode.Inactive:
                        return Encore.Utility.ColorUtility.salmon;
                    default:
                        return Encore.Utility.ColorUtility.darkSlateGray;
                }
            }

            #endregion


            public void Init(LevelManager levelManager, ScreenMode activeInScreenMode)
            {
                this.levelManager = levelManager;

                foreach (var objectActivator in objectActivators)
                {
                    objectActivator.Init(OnNextStage);
                }

                void OnNextStage()
                {
                    if (levelManager.Screen != activeInScreenMode) return;

                    activatedObjectsCount++;
                    if (CanGoToNextStage())
                    {
                        EndStage();
                        OnStageCleared?.Invoke();
                    }
                }
            }

            public void StartStage()
            {
                foreach (var objectActivator in objectActivators)
                    levelManager.StartCoroutine(ActivatingAfter(objectActivator, objectActivator.DelayActivation));

                IEnumerator ActivatingAfter(LevelObjectActivator activator, float delay)
                {
                    if (activator == null){ Debug.Log("Activator is null"); yield break; }
                    yield return new WaitForSeconds(delay);
                    activator.Activate(true);
                }
            }

            public void EndStage()
            {
                foreach (var objectActivator in objectActivators)
                {
                    levelManager.StartCoroutine(EndingActivatorAfter(objectActivator, objectActivator.DelayDeactivation));
                }

                IEnumerator EndingActivatorAfter(LevelObjectActivator activator, float delay)
                {
                    yield return new WaitForSeconds(delay);

                    if (activator != null)
                        activator.EndStage();
                }
            }

            bool CanGoToNextStage()
            {
                switch (nextRequirement)
                {
                    case NextRequirement.All:
                        isStageCleared = activatedObjectsCount >= objectActivators.Count;
                        break;

                    case NextRequirement.Any:
                        if (objectActivators.Count > 0)
                            isStageCleared = activatedObjectsCount > 0;
                        else
                            isStageCleared = true;
                        break;

                    default:
                        isStageCleared = true;
                        break;
                }

                return isStageCleared;

            }

            public void Reset()
            {
                activatedObjectsCount = 0;
                isStageCleared = false;
            }
        }

        [Serializable]
        public class StagesSet
        {
            [SerializeField, ListDrawerSettings(Expanded = true)]
            List<Stage> stages = new();

            Stage currentStage => stages.GetAt(currentStageIndex,null);
            int currentStageIndex;
            Action OnEnd;

            public void Init(LevelManager levelManager, ScreenMode activeInScreenMode, Action onEnd)
            {
                this.OnEnd = onEnd;
                currentStageIndex = 0;


                foreach (var stage in stages)
                {
                    if (stage.ActiveMode != Stage.StageActiveMode.Inactive)
                    {
                        stage.OnStageCleared = OnStageCleared;
                        stage.Init(levelManager, activeInScreenMode);
                    }
                }

                var activeStageIndex = -1;
                for (int i = 0; i < stages.Count; i++)
                {
                    if (stages[i].ActiveMode == Stage.StageActiveMode.Active)
                    {
                        activeStageIndex = i;
                        break;
                    }
                }

                if (activeStageIndex != -1)
                {
                    currentStageIndex = activeStageIndex;
                    currentStage.StartStage();
                }
                else
                {
                    onEnd?.Invoke();
                }


            }
            void OnStageCleared()
            {
                if (currentStageIndex >= stages.Count - 1)
                    Exit();

                for (int i = currentStageIndex + 1; i < stages.Count; i++)
                {
                    if (stages[i].ActiveMode == Stage.StageActiveMode.Active)
                    {
                        currentStageIndex = i;
                        currentStage.StartStage();
                        break;
                    }
                }
            }

            public void Exit()
            {
                foreach (var stage in stages)
                    stage.Reset();

                OnEnd?.Invoke();
            }

            public void ForceToNextStage()
            {
                currentStage?.EndStage();
                OnStageCleared();
            }
        }

            #endregion

            #region [Vars: Properties]

            [SerializeField]
        Level level;

        [SerializeField]
        Level nextLevel;

        [SerializeField]
        LevelGradingRules gradingRules;

        [Header("Player")]
        [SerializeField]
        SaveCache saveCache;
        public SaveCache SaveCache => saveCache;

        [SerializeField]
        bool usePlayerInScene = true;

        [SerializeField, ShowIf("@!" + nameof(usePlayerInScene))]
        PlayerBrain playerPrefab;

        [SerializeField, ShowIf("@!"+nameof(usePlayerInScene))]
        StartPoint startPoint;

        [SerializeField]
        bool useVCamInScene = true;

        [Header("Gameplay")]

        [SerializeField]
        Timer timer;

        [Header("UI")]
        [SerializeField]
        LoadingCanvas loadingCanvasPrefab;

        [SerializeField]
        PauseMenu pauseMenuPrefab;
        PauseMenu pauseMenu;

        [SerializeField]
        GameOverCanvas gameOverCanvasPrefab;
        GameOverCanvas gameOverCanvas;

        [SerializeField]
        WinCanvas winCanvasPrefab;
        WinCanvas winCanvas;

        [SerializeField]
        Level mainMenu;

        [Header("Audio")]

        AudioMixer audioMixer;
        const string BGM_Volume = nameof(BGM_Volume);
        const string PauseFX_Volume = nameof(PauseFX_Volume);

        [Header("Stages")]

        [SerializeField]
        StagesSet mainStages;

        [SerializeField]
        StagesSet gameOverStages;

        [SerializeField]
        StagesSet winStages;

        #endregion

        #region [Vars: Data Handlers]

        PlayerBrain player;
        public PlayerBrain Player => player;
        Cinemachine.CinemachineVirtualCamera vCam;
        LevelGradingRules.Score score = new();
        ScreenMode screenMode = ScreenMode.Play;
        public ScreenMode Screen => screenMode;

        #endregion

        public Action OnPause;
        public Action OnResume;
        public Action<float> OnStartQuitting;
        public Action OnInit;
        public Action OnShowWinCanvas;
        public Action OnShowGameOverCanvas;


        protected virtual void Awake()
        {
            // TODO: remove this when GameManager is already control the initialization
            Init();
        }

        protected virtual void OnDestroy()
        {
            Exit();
        }

        public virtual void Init()
        {
            #region [Player]

            if (usePlayerInScene)
            {
                player = FindObjectOfType<PlayerBrain>();
                if (player == null)
                {
                    Debug.LogWarning("usePlayerInScene is True, but there is no PlayerBrain found");
                    return;
                }
            }
            else
            {
                player = Instantiate(playerPrefab);
                player.transform.position = startPoint.point.position;
                player.transform.rotation = startPoint.point.rotation;
            }

            player.Init(this);
            player.OnQuitInput += TogglePause;
            if (player.TryGetComponent<HealthController>(out var playerHealthController))
            {
                playerHealthController.OnDie += EnterGameOverStages;
                playerHealthController.OnDamaged += OnPlayerDamaged;
            }

            if (player.TryGetComponent<HealthBarrierController>(out var playerHealthBarrier))
            {
                playerHealthBarrier.OnDamaged += OnPlayerDamaged;
            }

            if (player.TryGetComponent<FireController>(out var playerFireController))
            {
                playerFireController.OnKill += OnPlayerKills;
            }

            #endregion

            #region [Camera]

            if (useVCamInScene)
            {
                player.InstatiateVCam = false;
                vCam = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
                if (vCam == null)
                    Debug.LogWarning("useVCamInScene is True, but there is no VCam found");
            }

            #endregion

            #region [Pause Menu]

            if (pauseMenuPrefab != null)
            {
                pauseMenu = Instantiate(pauseMenuPrefab);
                pauseMenu.Init();
                pauseMenu.OnResume += Resume;
                pauseMenu.OnRestart += Restart;
                pauseMenu.OnQuit += Quit;
            }

            #endregion

            #region [Dialogue Canvas]

            var dialogueCanvases = new List<DialogueCanvas>(FindObjectsOfType<DialogueCanvas>(true));
            foreach (var canvas in dialogueCanvases)
            {
                canvas.Init(this);
            }

            #endregion

            #region [Exit Loading Canvas]

            var loadingCanvas = FindObjectOfType<LoadingCanvas>();
            if (loadingCanvas != null)
                loadingCanvas.Exit();

            #endregion

            #region [Level BGM Controller]

            if(TryGetComponent<LevelBGMController>(out var levelBGMController))
                levelBGMController.Init(this);

            #endregion

            EnterMainStages();

            OnInit?.Invoke();
        }

        public virtual void Exit()
        {
            if (pauseMenu != null)
            {
                pauseMenu.OnResume -= Resume;
                pauseMenu.OnRestart -= Restart;
                pauseMenu.OnQuit -= Quit;
            }

            if (gameOverCanvas != null)
            {
                gameOverCanvas.OnRestart -= Restart;
                gameOverCanvas.OnMainMenu -= Quit;
            }

            if (winCanvas != null)
            {
                if (nextLevel != null)
                    winCanvas.OnGoToNextLevel -= GoToNextLevel;
                winCanvas.OnRestart -= Restart;
                winCanvas.OnMainMenu -= Quit;
            }

            if (TryGetComponent<LevelBGMController>(out var levelBGMController))
                levelBGMController.Exit(this);

            if (player != null)
            {
                player.OnQuitInput -= TogglePause;
                if (player.TryGetComponent<HealthController>(out var playerHealthController))
                {
                    playerHealthController.OnDie -= EnterGameOverStages;
                    playerHealthController.OnDamaged -= OnPlayerDamaged;
                }

                if (player.TryGetComponent<HealthBarrierController>(out var playerHealthBarrier))
                {
                    playerHealthBarrier.OnDamaged -= OnPlayerDamaged;
                }

                if (player.TryGetComponent<FireController>(out var playerFireController))
                {
                    playerFireController.OnKill -= OnPlayerKills;
                }
            }
        }

        void OnPlayerDamaged(float damage)
        {
            score.damaged += damage;
        }

        void OnPlayerKills()
        {
            score.killCount++;
        }

        public void TogglePause()
        {
            if (screenMode == ScreenMode.Play)
                Pause();
            else if (screenMode == ScreenMode.Pause)
                Resume();
        }

        public void EnterMainStages()
        {
            screenMode = ScreenMode.Play;
            mainStages.Init(this, ScreenMode.Play, EnterWinStages);
        }

        public void NextStageOnMainStages() => mainStages.ForceToNextStage();

        public void EnterGameOverStages()
        {
            screenMode = ScreenMode.GameOver;
            player.DisplayCursorMenu();
            player.DeactivateJetCollider();
            gameOverStages.Init(this, ScreenMode.GameOver, ShowGameOverCanvas);

        }

        public void EnterWinStages()
        {
            screenMode = ScreenMode.Win;
            player.DisplayCursorMenu();
            player.DeactivateJetCollider();
            winStages.Init(this, ScreenMode.Win, ShowWinCanvas);

        }

        void ShowGameOverCanvas()
        {
            if (screenMode == ScreenMode.GameOverCanvas || screenMode == ScreenMode.WinCanvas) return;
            screenMode = ScreenMode.GameOverCanvas;

            if (gameOverCanvasPrefab != null)
            {
                gameOverCanvas = Instantiate(gameOverCanvasPrefab);
                gameOverCanvas.Init();
                gameOverCanvas.OnRestart += Restart;
                gameOverCanvas.OnMainMenu += Quit;
            }

            player.DisplayCursorMenu();
            OnShowGameOverCanvas?.Invoke();
        }


        void ShowWinCanvas()
        {
            if (screenMode == ScreenMode.WinCanvas || screenMode == ScreenMode.GameOverCanvas) return;
            screenMode = ScreenMode.WinCanvas;

            if (timer != null)
            {
                score.timeRemaining = (int) timer.TimeRemaining;
                score.timeElapsed = (int) timer.TimeElapsed;
            }

            if (winCanvasPrefab != null)
            {
                winCanvas = Instantiate(winCanvasPrefab);
                winCanvas.OnRestart += Restart;
                winCanvas.OnMainMenu += Quit;
                if (nextLevel != null)
                    winCanvas.OnGoToNextLevel += GoToNextLevel;
                winCanvas.Init(level, gradingRules, score);
            }

            player.DisplayCursorMenu();
            OnShowWinCanvas?.Invoke();
        }


        public void Pause()
        {
            screenMode = ScreenMode.Pause;
            Time.timeScale = 0;

            OnPause?.Invoke();

            player.DisplayCursorMenu();

            if (pauseMenu != null) 
                pauseMenu.Show(true);
        }

        public void Resume()
        {
            screenMode = ScreenMode.Play;
            Time.timeScale = 1;

            OnResume?.Invoke();

            player.DisplayCursorGame();

            if (pauseMenu != null)
                pauseMenu.Show(false);
        }

        public void Restart()
        {
            LoadScene(SceneManager.GetActiveScene().name);
        }

        public void GoToNextLevel()
        {
            LoadScene(nextLevel.SceneName);
        }
            
        public void Quit()
        {
            LoadScene(mainMenu.SceneName);
        }

        public void LoadScene(string sceneName)
        {
            if (screenMode == ScreenMode.Loading) return;

            screenMode = ScreenMode.Loading;
            Time.timeScale = 1;

            var loadingDuration = 2f;
            if (loadingCanvasPrefab != null)
            {
                var loadingCanvas = Instantiate(loadingCanvasPrefab, null);
                loadingCanvas.Init(() => { SceneManager.LoadScene(sceneName); });
                loadingDuration = loadingCanvas.InDuration;
            }
            else
            {
                SceneManager.LoadScene(sceneName);
            }

            OnStartQuitting?.Invoke(loadingDuration);

        }
    }
}
