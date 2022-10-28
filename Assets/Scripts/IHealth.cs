using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    public interface IHealth
    {
        public void ReceiveDamage(float damage);
        public void Die();
        public float Health { get; }
        public float MaxHealth { get; }
    }
}
