using Encore.Utility;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Phoenix
{
    public class HealthBarUIUnit : MonoBehaviour
    {
        public enum FillStatus { Full, Empty, Filling}

        [SerializeField]
        Image healthBarFill;

        [SerializeField]
        Image healthBarBorder;        
        
        [SerializeField]
        Image healthBarLight;

        [SerializeField]
        List<GameObject> affectedGOs = new List<GameObject>();

        [SerializeField]
        Vector2 blinkingRange = new Vector2(0.1f, 1f);

        [SerializeField]
        bool useAlternateFirstSprite = false;
        [SerializeField, ShowIf(nameof(useAlternateFirstSprite))]
        Sprite alternateFirstSpriteFill;
        [SerializeField, ShowIf(nameof(useAlternateFirstSprite))]
        Sprite alternateFirstSpriteBorder;
        [SerializeField, ShowIf(nameof(useAlternateFirstSprite))]
        Sprite alternateFirstSpriteLight;

        float nextBlink;
        FillStatus status = FillStatus.Full;
        public FillStatus Status => status;
        Sprite normalSpriteFill, normalSpriteBorder, normalSpriteLight;
        List<Image> images = new List<Image>();
        Coroutine corFillingHealthBar;

        void Awake()
        {
            normalSpriteFill = healthBarFill.sprite;
            normalSpriteBorder = healthBarBorder.sprite;
            normalSpriteLight = healthBarLight.sprite;
            images = new List<Image>() { healthBarFill, healthBarBorder, healthBarLight };
        }

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

        [Button]
        public void UseAlternateFirstSprite()
        {
            if (useAlternateFirstSprite)
            {
                healthBarFill.sprite = alternateFirstSpriteFill;
                healthBarBorder.sprite = alternateFirstSpriteBorder;
                healthBarLight.sprite = alternateFirstSpriteLight;
                //foreach (var image in images)
                //    image.SetNativeSize();
            }
        }

        public void UseNormalSprite()
        {
            healthBarFill.sprite = normalSpriteFill;
            healthBarBorder.sprite = normalSpriteBorder;
            healthBarLight.sprite = normalSpriteLight;
            foreach (var image in images)
                image.SetNativeSize();
        }
    }
}
