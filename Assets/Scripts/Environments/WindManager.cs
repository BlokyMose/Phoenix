using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    [RequireComponent(typeof(PointEffector2D))]
    public class WindManager : MonoBehaviour
    {
        #region [Vars: Properties]

        [SerializeField]
        [MinMaxSlider(-10,0)]
        Vector2 magnitudeRange = new Vector2(-1, -5);

        [SerializeField]
        [MinMaxSlider(1,10)]
        Vector2 updateRange = new Vector2(3, 5);

        #endregion


        #region [Vars: Components]

        PointEffector2D pointEffector;

        #endregion


        #region [Vars: Data Handlers]

        float updateMagnitudeCooldown = 0;

        #endregion


        void Awake()
        {
            pointEffector = GetComponent<PointEffector2D>();
        }

        void Start()
        {

            StartCoroutine(Updating());
            IEnumerator Updating()
            {
                while (true)
                {
                    updateMagnitudeCooldown -= Time.deltaTime;
                    if(updateMagnitudeCooldown < 0)
                    {
                        updateMagnitudeCooldown = Random.Range(updateRange.x, updateRange.y);
                        pointEffector.forceMagnitude = Random.Range(magnitudeRange.x, magnitudeRange.y);
                    }

                    yield return null;
                }
            }
        }
    }
}
