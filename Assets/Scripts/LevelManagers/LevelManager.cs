using Encore.Utility;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Phoenix
{
    public abstract class LevelManager: MonoBehaviour
    {
        #region [Classes]

        [Serializable]
        public class StartPoint
        {
            public Transform point;
        }

        [Serializable]
        public class Stage
        {
            public enum StageActiveMode { Active, Passive, Inactive }
            public enum NextRequirement { All, One }

            [SerializeField, Tooltip("- Active: activate & deactivate objects based on order \n- Passive: deactivate objects at start \n- Inactive: do nothing")]
            StageActiveMode activeMode = StageActiveMode.Active;
            public StageActiveMode ActiveMode => activeMode;

            [SerializeField]
            NextRequirement next = NextRequirement.All;

            [SerializeField]
            List<LevelObjectActivator> objectActivators = new List<LevelObjectActivator>();
            public List<LevelObjectActivator> ObjectActivators => objectActivators;

            int activatedObjectsCount = 0;
            bool isStageCleared = false;
            public Action OnStageCleared;

            public void Init()
            {
                foreach (var objectActivator in objectActivators)
                {
                    objectActivator.Init(OnNextStage);
                }

                void OnNextStage()
                {
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
                    objectActivator.Activate(true);
                }
            }

            public void EndStage()
            {
                foreach (var objectActivator in objectActivators)
                {
                    objectActivator.Activate(false);
                }
            }

            bool CanGoToNextStage()
            {
                switch (next)
                {
                    case NextRequirement.All:
                        isStageCleared = activatedObjectsCount >= objectActivators.Count;
                        break;

                    case NextRequirement.One:
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

        [Header("Player")]
        [SerializeField]
        bool usePlayerInScene = true;

        [SerializeField, ShowIf("@!" + nameof(usePlayerInScene))]
        PlayerBrain playerPrefab;

        [SerializeField, ShowIf("@!"+nameof(usePlayerInScene))]
        StartPoint startPoint;

        [SerializeField]
        bool useVCamInScene = true;

        [Header("UI")]
        [SerializeField]
        PauseMenu pauseMenu;

        [SerializeField]
        Object mainMenuScene;

        [Header("Stages")]
        [SerializeField]
        bool isLoopingStages = false;

        [SerializeField]
        List<Stage> stages = new List<Stage>();


        #endregion

        #region [Vars: Data Handlers]

        PlayerBrain playerBrain;
        Cinemachine.CinemachineVirtualCamera vCam;
        Stage currentStage => stages[currentStageIndex];
        int currentStageIndex;

        #endregion

        void Awake()
        {
            // TODO: remove this when GameManager is already control the initialization
            Init();
        }

        public virtual void Init()
        {
            if (usePlayerInScene)
            {
                playerBrain = FindObjectOfType<PlayerBrain>();
                if (playerBrain == null)
                    Debug.LogWarning("usePlayerInScene is True, but there is no PlayerBrain found");
            }
            else
            {
                playerBrain = Instantiate(playerPrefab);
                playerBrain.transform.position = startPoint.point.position;
                playerBrain.transform.rotation = startPoint.point.rotation;
            }

            if (useVCamInScene)
            {
                playerBrain.InstatiateVCam = false;
                vCam = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
                if (vCam == null)
                    Debug.LogWarning("useVCamInScene is True, but there is no VCam found");
            }

            foreach (var stage in stages)
            {
                if (stage.ActiveMode != Stage.StageActiveMode.Inactive)
                {
                    stage.OnStageCleared += OnStageCleared;
                    stage.Init();
                }
            }

            pauseMenu = Instantiate(pauseMenu);
            pauseMenu.Init(mainMenuScene);

            StartLevel();

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

            void StartLevel()
            {
                for (int i = 0; i < stages.Count; i++)
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

        public void ShowPauseMenu()
        {
            pauseMenu.Show(true);
        }
    }
}
