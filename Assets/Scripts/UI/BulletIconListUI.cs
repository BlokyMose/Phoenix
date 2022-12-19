using Encore.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    public class BulletIconListUI : MonoBehaviour
    {
        [SerializeField]
        BulletIconUIUnit bulletIconPrefab;

        [SerializeField]
        float animationDelay = 15f / 60f;

        [SerializeField]
        int shownFadedIconCount = 2;

        List<BulletIconUIUnit> icons = new List<BulletIconUIUnit>();

        public void Init(FireController fireController)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }

            SetupIcons(fireController.BulletProperties);
            fireController.OnNextBullet += NextBullet;
        }

        public void Exit(FireController fireController)
        {
            fireController.OnNextBullet -= NextBullet;
        }

        void SetupIcons(List<BulletProperties> bullets)
        {
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                var iconUI = Instantiate(bulletIconPrefab, transform);
                iconUI.Init(bullets[i]);
                icons.Add(iconUI);
            }

            if (shownFadedIconCount + 1 > icons.Count)
                shownFadedIconCount = icons.Count - 1;

            for (int i = icons.Count - (shownFadedIconCount + 1); i < icons.Count - 1; i++)
            {
                icons[i].PlayAnimation(BulletIconUIUnit.Mode.Faded);
            }

            icons.GetLast().PlayAnimation(BulletIconUIUnit.Mode.Idle);
        }

        int nextBulletQueue = 0;
        void NextBullet(BulletProperties bulletProperties)
        {
            nextBulletQueue++;
            if (nextBulletQueue == 1)
                StartCoroutine(PlayingAnimation());

            IEnumerator PlayingAnimation()
            {
                while(nextBulletQueue > 0)
                {
                    var iconLast = icons.GetLast();
                    var iconSecondLast = icons.GetLast(1);
                    iconLast.PlayAnimation(BulletIconUIUnit.Mode.Invisble);
                    iconSecondLast.PlayAnimation(BulletIconUIUnit.Mode.Idle);

                    var newIconUI = Instantiate(bulletIconPrefab, transform);
                    newIconUI.transform.SetAsFirstSibling();
                    newIconUI.Init(iconLast.BulletProperties);
                    icons.Insert(0, newIconUI);
                    icons.GetLast(shownFadedIconCount + 1).PlayAnimation(BulletIconUIUnit.Mode.Faded);

                    yield return new WaitForSeconds(animationDelay);

                    Destroy(iconLast.gameObject);
                    icons.Remove(iconLast);
                    nextBulletQueue--;

                }

            }
        }
    }
}
