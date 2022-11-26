using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Encore.Utility;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using Sirenix.Utilities;

namespace Phoenix
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class CameraController : MonoBehaviour
    {
        public enum ZoomMode { ZoomOut, ZoomIn, Both }

        [SerializeField, LabelText("Aspect Ratio")]
        Vector2 aspectRatioVector2 = new Vector2(16, 9);

        [SerializeField, Range(0f,1f)]
        float fitWidth = 1f;

        [SerializeField]
        ZoomMode zoom = ZoomMode.ZoomOut;

        CinemachineVirtualCamera vCam;
        float defaultOrthoSize;

        void Awake()
        {
            vCam = GetComponent<CinemachineVirtualCamera>();
            defaultOrthoSize = vCam.m_Lens.OrthographicSize;
            AdjustOrthoSize();
        }

        void AdjustOrthoSize()
        {
            float screenAspectRatio = Screen.currentResolution.width / Screen.currentResolution.height;
            float aspectRatio = aspectRatioVector2.x / aspectRatioVector2.y;

#if UNITY_EDITOR
            screenAspectRatio = UnityEditor.CameraEditorUtils.GameViewAspectRatio;
#endif

            var newOrthoSize = ((aspectRatio / screenAspectRatio  * defaultOrthoSize) - defaultOrthoSize) * fitWidth + defaultOrthoSize;

            switch (zoom)
            {
                case ZoomMode.ZoomOut:
                    if (newOrthoSize > vCam.m_Lens.OrthographicSize)
                        vCam.m_Lens.OrthographicSize = newOrthoSize;
                    break;

                case ZoomMode.ZoomIn:
                    if (newOrthoSize < vCam.m_Lens.OrthographicSize)
                        vCam.m_Lens.OrthographicSize = newOrthoSize;
                    break;

                case ZoomMode.Both:
                    vCam.m_Lens.OrthographicSize = newOrthoSize;
                    break;
            }
        }
    }
}
