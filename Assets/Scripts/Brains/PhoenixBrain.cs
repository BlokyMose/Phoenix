using Encore.Utility;
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

        [SerializeField]
        AudioSourceRandom audioSource;

        ElementSwitcher elementSwitcher;

        #endregion

        #region [Vars: Properties]

        [SerializeField, MinMaxSlider(-2.5f, 2.5f)]
        Vector2 moveYRange = new Vector2(-2.5f, 2.5f);

        [SerializeField]
        Vector2 stayDurationRange = new Vector2(1, 2);

        [SerializeField]
        float switchElementEvery = 5f;

        #endregion

        #region [Vars: Data Handlers]

        Coroutine corUpdate;

        #endregion

        public override void Init()
        {
            base.Init();
            elementSwitcher = GetComponent<ElementSwitcher>();
            audioSource = GetComponent<AudioSourceRandom>();

            if(TryGetComponent<HealthController>(out var health))
            {
                health.OnDie += EscapeAway;
            }
        }

        void Start()
        {
            audioSource.Play();

            corUpdate = this.RestartCoroutine(Update());
            IEnumerator Update()
            {
                var time = 0f;
                var switchElementAt = switchElementEvery;
                bool isSettingNewPos = false;
                
                while (true)
                {
                    Fire();
                    FaceRight();
                    Move();
                    SwitchElement();

                    time += Time.deltaTime * Time.timeScale;
                    yield return null;
                }

                void Fire()
                {
                    OnFireInput?.Invoke();
                }

                void FaceRight()
                {
                    OnCursorWorldPos((Vector2)transform.position + Vector2.right * 10);
                }

                void Move()
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
                }

                void SwitchElement()
                {
                    if(time > switchElementAt && elementSwitcher != null)
                    {
                        switchElementAt += switchElementEvery;
                        elementSwitcher.SwitchToNextElement();
                    }
                }
            }   
        }

        public override void Exit()
        {
            StopAllCoroutines();
            base.Exit();
        }

        void EscapeAway()
        {
            StopAllCoroutines();
            corUpdate = this.RestartCoroutine(Update());
            IEnumerator Update()
            {
                while (true)
                {
                    FaceRight();
                    Move();

                    yield return null;
                }

                void FaceRight()
                {
                    OnCursorWorldPos((Vector2)transform.position + Vector2.right * 10);
                }

                void Move()
                {
                    OnMoveInput(Vector2.right);
                }

            }
        }


    }
}
