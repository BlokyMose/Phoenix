using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Phoenix.FireComponents;
using static UnityEditor.Rendering.Universal.ShaderGUI.LitGUI;

namespace Phoenix
{
    public class FireController : MonoBehaviour
    {
        [SerializeField]
        JetProperties jetProperties;
        public JetProperties JetProperties { get { return jetProperties; } }


        [SerializeField]
        List<BulletProperties> bulletProperties = new List<BulletProperties>();
        public List<BulletProperties> BulletProperties => bulletProperties;

        [SerializeField]
        LayerMask bulletLayer;

        float fireCooldown;

        int currentFireModeIndex;
        int currentFireOrigin;
        int currentBulletIndex;
        BulletProperties currentBullet => bulletProperties[currentBulletIndex];

        [SerializeField, InlineButton(nameof(InstantiateJet), "Show", ShowIf = "@!" + nameof(components)), PropertyOrder(-1)]
        FireComponents components;

        public Action OnNextBullet;

        private void OnEnable()
        {
            var brain = GetComponent<Brain>();
            if (brain != null) 
                Init(brain);

            var playerBrain = GetComponent<PlayerBrain>();
            if (playerBrain != null)
                playerBrain.ConnectToBulletList(this);
                
        }

        private void OnDisable()
        {
            var brain = GetComponent<Brain>();
            if (brain != null) 
                Disable(brain);

            var playerBrain = GetComponent<PlayerBrain>();
            if (playerBrain != null)
                playerBrain.DisconnectFromBulletList(this);
        }

        public void Init(Brain brain)
        {
            brain.OnFireInput += Fire;
            brain.OnNextFireModeInput += NextFireMode;
            brain.OnNextBulletInput += NextBullet;

            currentFireModeIndex = 0;
            currentFireOrigin = 0;
            currentBulletIndex = 0;
        }

        public void Disable(Brain brain)
        {
            brain.OnFireInput -= Fire;
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
            OnNextBullet?.Invoke();
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
            var bullet = bulletGO.AddComponent<BulletController>();
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
