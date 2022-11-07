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
        float animationDelay = 25f / 60f;

        List<BulletIconUIUnit> icons = new List<BulletIconUIUnit>();

        public void Init(FireController fireController)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }

            InstantiateIcons(fireController.BulletProperties);
            fireController.OnNextBullet += NextBullet;
        }

        void InstantiateIcons(List<BulletProperties> bullets)
        {
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                var iconUI = Instantiate(bulletIconPrefab, transform);
                iconUI.Init(bullets[i]);
                iconUI.PlayAnimation(BulletIconUIUnit.Mode.Faded);
                icons.Add(iconUI);
            }
            icons.GetLast().PlayAnimation(BulletIconUIUnit.Mode.Idle);
        }

        void NextBullet()
        {
            StartCoroutine(Delay());
            IEnumerator Delay()
            {
                var iconLast = icons.GetLast();
                var iconSecondLast = icons.GetLast(1);
                iconLast.PlayAnimation(BulletIconUIUnit.Mode.Invisble);
                iconSecondLast.PlayAnimation(BulletIconUIUnit.Mode.Idle);
                
                yield return new WaitForSeconds(animationDelay);

                iconLast.transform.SetSiblingIndex(0);
                icons.Remove(iconLast);
                icons.Insert(0,iconLast);

                // TODO: Handle multiple cases: from 1 only bullet to 3 or more bullets
                icons[0].PlayAnimation(BulletIconUIUnit.Mode.Faded);
            }
        }
    }
}
