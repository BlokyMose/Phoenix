using Encore.Utility;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Phoenix.JetController;

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

        #endregion


        #region [Vars: Data Handlers]

        PlayerBrain playerBrain;
        Cinemachine.CinemachineVirtualCamera vCam;

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
        }
    }
}
