using Encore.Utility;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    public class ShieldBotBrain : Brain
    {
        [SerializeField, InlineButton(nameof(InstantiateShieldController), "Show", ShowIf = "@!"+nameof(shieldController))]
        ShieldController shieldController;

        [SerializeField, Required]
        ShieldController shieldPrefab;

        [SerializeField, Required]
        ShieldProperties shieldProperties;

        [SerializeField]
        Transform target;

        [SerializeField, LabelText("Rotaion Stability")]
        float cursorPosRatio = 100f;

        Vector2 currentPointerPos;

        public override void Init()
        {
            base.Init();

            // Setup ShieldController
            if (shieldController != null) 
                Destroy(shieldController.gameObject);
            InstantiateShieldController();

            // Setup HealthController
            var healthController = GetComponent<HealthController>();
            if (healthController != null)
            {
                healthController.OnDie += Die;
            }

            // Movement logic
            StartCoroutine(Update());
            IEnumerator Update()
            {
                while (true)
                {
                    var moveDirection = (target.position - transform.position).normalized;
                    OnMoveInput?.Invoke(moveDirection);

                    currentPointerPos = GetVector2(target.localEulerAngles.z, target.position, cursorPosRatio);
                    OnCursorWorldPos?.Invoke(currentPointerPos);

                    yield return null;
                }
                
                Vector2 GetVector2(float angle, Vector2 positionOffset, float multiply = 1f)
            {
                var direction = new Vector2(
                        Mathf.Cos(target.localEulerAngles.z * Mathf.Deg2Rad),
                        Mathf.Sin(target.localEulerAngles.z * Mathf.Deg2Rad));
                direction *= multiply;
                direction += positionOffset;

                return direction;
            }
            }

        }

        void InstantiateShieldController()
        {
            shieldController = Instantiate(shieldPrefab, transform);
            shieldController.Init(transform, shieldProperties);
            shieldController.gameObject.name = "Shield";
            shieldController.transform.localPosition = Vector2.zero;
            shieldController.transform.localEulerAngles = Vector2.zero;
            shieldController.transform.parent = null;
            shieldController.OnDie += RespawnShieldController;
        }

        void RespawnShieldController()
        {
            StartCoroutine(Delay(2f));
            IEnumerator Delay(float delay)
            {
                yield return new WaitForSeconds(delay);
                InstantiateShieldController();
            }
        }

        void Die()
        {
            StopAllCoroutines();
            StartCoroutine(AnimatingDie());
            IEnumerator AnimatingDie()
            {
                DestroyShieldController();
                DisableAllColliders();
                // TODO: do VFX and/or animation 
                
                yield return new WaitForSeconds(0f);

                Destroy(gameObject);
            }

            void DisableAllColliders()
            {
                foreach (var col in GetComponentsInChildren<Collider2D>())
                    col.enabled = false;
                foreach (var col in GetComponents<Collider2D>())
                    col.enabled = false;

            }

            void DestroyShieldController()
            {
                if (shieldController!=null)
                    shieldController.Die();
            }
        }
    }
}
