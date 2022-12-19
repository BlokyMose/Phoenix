using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    public class PhoenixBrain : Brain
    {

        #region [Vars: Components]

        [SerializeField]
        Transform targetPos;

        [SerializeField]
        HealthBarUI healthBarUI;

        #endregion


        #region [Vars: Properties]

        [SerializeField, MinMaxSlider(-2.5f, 2.5f)]
        Vector2 moveYRange = new Vector2(-2.5f, 2.5f);

        [SerializeField]
        Vector2 stayDurationRange = new Vector2(1, 2);

        #endregion

        #region [Vars: Data Handlers]


        #endregion

        public override void Init()
        {
            base.Init();
        }

        public override void Exit()
        {
            base.Exit();
        }

        private void Start()
        {
            StartCoroutine(Firing());
            IEnumerator Firing()
            {
                while (true)
                {
                    OnFireInput();
                    yield return null;
                }
            }

            StartCoroutine(Facing());
            IEnumerator Facing()
            {
                while (true)
                {
                    OnCursorWorldPos((Vector2)transform.position + Vector2.right * 10);
                    yield return null;
                }
            }

            StartCoroutine(Moving());
            IEnumerator Moving()
            {
                bool isSettingNewPos = false;
                while (true)
                {
                    if (!isSettingNewPos && Mathf.Abs(transform.position.y - targetPos.position.y) < 0.25f)
                    {
                        isSettingNewPos = true;
                        StartCoroutine(DelaySetPosition());
                        IEnumerator DelaySetPosition()
                        {
                            yield return new WaitForSeconds(Random.Range(stayDurationRange.x, stayDurationRange.y));
                            targetPos.position = new Vector2(targetPos.position.x, Random.Range(moveYRange.x, moveYRange.y));
                            isSettingNewPos = false;
                        }
                    }

                    if (transform.position.y > targetPos.position.y)
                        OnMoveInput(Vector2.down);
                    else
                        OnMoveInput(Vector2.up);

                    if (Mathf.Abs(transform.position.x - targetPos.position.x) > 0.25f)
                    {
                        if (transform.position.x > targetPos.position.x)
                            OnMoveInput(Vector2.left);
                        else
                            OnMoveInput(Vector2.right);
                    }


                    yield return null;


                }
            }
        }

        public void ConnectToHealthBar(HealthController healthController)
        {
            if (healthBarUI != null)
                healthBarUI.Init(healthController);
        }

    }
}
