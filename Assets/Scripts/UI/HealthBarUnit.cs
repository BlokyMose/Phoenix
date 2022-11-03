using Encore.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Phoenix
{
    public class HealthBarUnit : MonoBehaviour
    {
        public enum FillStatus { Full, Empty, Filling}

        [SerializeField]
        Image healthBarFill;

        [SerializeField]
        Image healthBarBorder;

        [SerializeField]
        List<GameObject> affectedGOs = new List<GameObject>();

        [SerializeField]
        Vector2 blinkingRange = new Vector2(0.1f, 1f);

        float nextBlink;
        FillStatus status = FillStatus.Full;
        public FillStatus Status => status;

        Coroutine corFillingHealthBar;


        public void Fill(float time, float duration)
        {
            status = FillStatus.Filling;

            healthBarFill.fillAmount = time / duration;

            if (time > nextBlink)
            {
                nextBlink += (duration - time) / duration * blinkingRange.y;
                healthBarFill.color = healthBarFill.color.ChangeAlpha(healthBarFill.color.a == 0f ? 0.5f : 0f);
            }
        }

        public void Empty()
        {
            status = FillStatus.Empty;
            healthBarFill.fillAmount = 0f;
            nextBlink = 0f;

            foreach (var go in affectedGOs)
                go.SetActive(false);
        }

        public void Full()
        {
            // TODO: VFX
            status = FillStatus.Full;
            healthBarFill.fillAmount = 1f;
            healthBarFill.color = healthBarFill.color.ChangeAlpha(1f);

            foreach (var go in affectedGOs)
                go.SetActive(true);
        }

        public void StopFillingHealthBar()
        {
            if (corFillingHealthBar!=null)
                StopCoroutine(corFillingHealthBar);
        }
    }
}
