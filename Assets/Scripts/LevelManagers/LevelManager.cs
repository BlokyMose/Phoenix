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

        public enum ScreenMode { Loading, Play, Pause, GameOver, Win }

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
            List<LevelObjectActivator> objectActivators = new List<LevelObjectActivator>();
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


            public void Init(LevelManager levelManager)
            {
                this.levelManager = levelManager;
                foreach (var objectActivator in objectActivators)
                {
                    objectActivator.Init(OnNextStage);
                }

                void OnNextStage()
                {
                    if (levelManager.Screen != ScreenMode.Play) return;

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
                {
                    levelManager.StartCoroutine(ActivatingAfter(objectActivator, objectActivator.DelayActivation));
                }

                IEnumerator ActivatingAfter(LevelObjectActivator activator, float delay)
                {
                    yield return new WaitForSeconds(delay);
                    activator.Activate(true);
                }
            }

            public void EndStage()
            {
                foreach (var objectActivator in objectActivators)
                {
                    levelManager.StartCoroutine(DectivatingAfter(objectActivator, objectActivator.DelayDeactivation));
                }

                IEnumerator DectivatingAfter(LevelObjectActivator activator, float delay)
                {
                    yield return new WaitForSeconds(delay);
                    activator.Activate(false);
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

        #endregion

        #region [Vars: Properties]

        [SerializeField]
        Level level;

        [SerializeField]
        LevelGradingRules gradingRules;

        [Header("Player")]
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
        bool isLoopingStages = false;

        [SerializeField]
        List<Stage> stages = new List<Stage>();

        [Header("Events")]
        [SerializeField]
        UnityEvent onGameOver;
        [SerializeField]
        UnityEvent onWin;

        #endregion

        #region [Vars: Data Handlers]

        PlayerBrain player;
        public PlayerBrain Player => player;
        Cinemachine.CinemachineVirtualCamera vCam;
        Stage currentStage => stages[currentStageIndex];
        int currentStageIndex;
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
                playerHealthController.OnDie += ShowGameOverMenu;
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

            #region [Stages]

            foreach (var stage in stages)
            {
                if (stage.ActiveMode != Stage.StageActiveMode.Inactive)
                {
                    stage.OnStageCleared += OnStageCleared;
                    stage.Init(this);
                }
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

            screenMode = ScreenMode.Play;

            #region [Start First Stage]

            for (int i = 0; i < stages.Count; i++)
            {
                if (stages[i].ActiveMode == Stage.StageActiveMode.Active)
                {
                    currentStageIndex = i;
                    currentStage.StartStage();
                    break;
                }
            }

            #endregion

            OnInit?.Invoke();

            void OnStageCleared()
            {
                if (isLoopingStages && currentStageIndex + 1 >= stages.Count)
                    currentStageIndex = -1;

                for (int i = currentStageIndex+1; i < stages.Count; i++)
                {
                    if (stages[i].ActiveMode == Stage.StageActiveMode.Active)
                    {
                        currentStageIndex = i;
                        currentStage.StartStage();
                        break;
                    }
                }
            }

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
                    playerHealthController.OnDie -= ShowGameOverMenu;
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

        public void ShowGameOverMenu()
        {
            if (screenMode == ScreenMode.GameOver || screenMode == ScreenMode.Win) return;

            screenMode = ScreenMode.GameOver;

            player.DisplayCursorGame();

            if (gameOverCanvasPrefab != null)
            {
                gameOverCanvas = Instantiate(gameOverCanvasPrefab);
                gameOverCanvas.Init();
                gameOverCanvas.OnRestart += Restart;
                gameOverCanvas.OnMainMenu += Quit;
            }

            player.DisplayCursorMenu();
            OnShowGameOverCanvas?.Invoke();
            onGameOver.Invoke();
        }

        public void ShowWinCanvas()
        {
            if (screenMode == ScreenMode.Win || screenMode == ScreenMode.GameOver) return;
            screenMode = ScreenMode.Win;

            if (timer != null)
            {
                score.timeRemaining = (int) timer.TimeRemaining;
                score.timeElapsed = (int) timer.TimeElapsed;
            }

            player.DisplayCursorGame();
            
            if (winCanvasPrefab != null)
            {
                winCanvas = Instantiate(winCanvasPrefab);
                winCanvas.Init(level, gradingRules, score);
                winCanvas.OnRestart += Restart;
                winCanvas.OnMainMenu += Quit;
            }

            player.DisplayCursorMenu();
            player.DeactivateJetCollider();
            OnShowWinCanvas?.Invoke();
            onWin.Invoke();
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
            
        public void Quit()
        {
            LoadScene(mainMenu.SceneName);
        }

        public void LoadScene(Object scene)
        {
            if (SceneManager.GetSceneByName(scene.name) != null)
                LoadScene(scene.name);
            else
                Debug.Log(nameof(scene.name) + " is not a scene",this);
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
