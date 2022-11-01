using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Appear : MonoBehaviour
    {
        private SpriteRenderer sp;
        private float count;
        private bool isOn = true;

        void Start()
        {
            sp = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            count += 0.5f;

            if (count % 500 == 0)
            {
                if (isOn)
                {
                    sp.enabled = false;
                    isOn = false;
                }
                else if (!isOn)
                {
                    sp.enabled = true;
                    isOn = true;
                }
            }
        }
    }

}
