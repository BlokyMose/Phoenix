using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    public class PhoenixBrain : Brain
    {

        #region [Vars: Properties]

        [SerializeField, MinMaxSlider(-2.5f, 2.5f)]
        Vector2 moveYRange = new Vector2(-2.5f, 2.5f);

        #endregion

        #region [Vars: Data Handlers]

        Vector2 destination = new Vector2(0, 0);

        #endregion


        public override Vector2 GetCursorWorldPosition()
        {
            return (Vector2)transform.position + Vector2.right * 10;
        }

        private void Start()
        {
            StartCoroutine(Firing());
            IEnumerator Firing()
            {
                while (true)
                {
                    OnFireInput(true);
                    yield return null;
                }
            }


            StartCoroutine(Moving());
            IEnumerator Moving()
            {
                while (true)
                {
                    if (Mathf.Abs(transform.position.y) - Mathf.Abs(destination.y) < 3)
                    {
                        destination = new Vector2(destination.x, Random.Range(moveYRange.x,moveYRange.y));
                    }

                    if (transform.position.y > destination.y)
                        OnMoveInput(Vector2.down);
                    else
                        OnMoveInput(Vector2.up);

                    yield return null;
                }
            }
        }


    }
}
