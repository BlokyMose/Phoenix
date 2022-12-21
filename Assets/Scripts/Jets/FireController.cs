using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Phoenix.FireComponents;

namespace Phoenix
{
    public class FireController : MonoBehaviour
    {

        [SerializeField]
        JetPropertiesStatic jetProperties;
        public JetPropertiesStatic JetProperties { get { return jetProperties; } }

        [SerializeField]
        List<BulletProperties> bulletProperties = new List<BulletProperties>();
        public List<BulletProperties> BulletProperties => bulletProperties;

        [SerializeField]
        LayerMask bulletLayer;

        [SerializeField]
        BulletIconListUI bulletIconListUI;

        float fireCooldown;

        int currentFireModeIndex;
        int currentFireOrigin;
        int currentBulletIndex;
        BulletProperties currentBullet => bulletProperties[currentBulletIndex];

        [SerializeField, InlineButton(nameof(InstantiateJet), "Show", ShowIf = "@!" + nameof(components)), PropertyOrder(-1)]
        FireComponents components;

        public Action<BulletProperties> OnNextBullet;



        public void Init(Brain brain)
        {
            brain.OnFireInput += Fire;
            brain.OnNextFireModeInput += NextFireMode;
            brain.OnNextBulletInput += NextBullet;

            currentFireModeIndex = 0;
            currentFireOrigin = 0;
            currentBulletIndex = 0;

            if (bulletIconListUI != null)
            {
                bulletIconListUI.Init(this);
            }

            if (brain is PlayerBrain)
            {
                var playerBrain = brain as PlayerBrain;
                playerBrain.ConnectToCursorDisplayer(this);
            }
        }

        public void Exit(Brain brain)
        {
            brain.OnFireInput -= Fire;
            brain.OnNextFireModeInput -= NextFireMode;
            brain.OnNextBulletInput -= NextBullet;

            if (bulletIconListUI != null)
            {
                bulletIconListUI.Exit(this);
            }

            if (brain is PlayerBrain)
            {
                var playerBrain = brain as PlayerBrain;
                playerBrain.DisconnectFromCursorDisplayer(this);
            }
        }

        void InstantiateJet()
        {
            if (components == null)
            {
                components = gameObject.GetComponent<FireComponents>();
                if (components == null)
                {
                    components = gameObject.GetComponentInChildren<FireComponents>();
                }

                if (components == null)
                {
                    var fireComponents = Instantiate(jetProperties.JetPrefab, transform).GetComponent<FireComponents>();
                    if (fireComponents != null)
                    {
                        components = fireComponents;
                        components.name = "Jet";
                    }
                }
            }

        }

        public void NextFireMode()
        {
            currentFireModeIndex = (currentFireModeIndex + 1) % components.FireModes.Count;
        }

        public void NextBullet()
        {
            currentBulletIndex = (currentBulletIndex + 1) % bulletProperties.Count;
            OnNextBullet?.Invoke(currentBullet);
        }

        void Update()
        {
            fireCooldown -= Time.deltaTime;
        }


        void Fire()
        {
            if (fireCooldown > 0) return;

            fireCooldown = 1 / jetProperties.RPS;
            var currentFireMode = components.FireModes[currentFireModeIndex];

            var bulletGO = new GameObject("Bullet");

            bulletGO.gameObject.layer = (int)Mathf.Log(bulletLayer.value, 2);
            bulletGO.transform.position = currentFireMode.origins[currentFireOrigin].transform.position;
            bulletGO.transform.eulerAngles = currentFireMode.origins[currentFireOrigin].eulerAngles;
            BulletController bullet = null;
            switch (currentBullet.CollisionMode)
            {
                case Phoenix.BulletProperties.BulletCollisionMode.Collide:
                    bullet = bulletGO.AddComponent<BulletControllerCollide>();
                    break;
                case Phoenix.BulletProperties.BulletCollisionMode.TriggerEnter:
                    bullet = bulletGO.AddComponent<BulletControllerTriggerEnter>();
                    break;
            }

            bullet.Init(currentBullet);

            currentFireOrigin++;

            switch (currentFireMode.pattern)
            {
                case FireMode.FirePattern.Sequence:
                    currentFireOrigin %= currentFireMode.origins.Count;
                    break;
                case FireMode.FirePattern.ConcurrentInstant:
                    if (currentFireOrigin / currentFireMode.origins.Count != 1)
                    {
                        fireCooldown = 0;
                        Fire();
                    }
                    else
                        currentFireOrigin = 0;
                    break;
                case FireMode.FirePattern.ConcurrentCooldown:
                    if (currentFireOrigin / currentFireMode.origins.Count != 1)
                        Fire();
                    else
                        currentFireOrigin = 0;
                    break;
                case FireMode.FirePattern.SequenceRandom:
                    currentFireOrigin = UnityEngine.Random.Range(0, currentFireMode.origins.Count);
                    break;

            }
        }
    }
}
