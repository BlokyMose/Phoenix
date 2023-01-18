using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Phoenix
{
    class PhoenixBrain_Level_3 : PhoenixBrain
    {
        [SerializeField, Range(1, 0)]
        float toNextFireModeWhenHealth = 0.5f;

        public override void Init()
        {
            base.Init();

            if (TryGetComponent<HealthController>(out var health))
            {
                bool isCalled = false;
                health.OnDamaged += NextFireMode;

                void NextFireMode(float damage)
                {
                    if (!isCalled && health.Health < health.MaxHealth * toNextFireModeWhenHealth)
                    {
                        isCalled = true;
                        OnNextFireModeInput();
                    }
                }
            }


        }
    }
}
