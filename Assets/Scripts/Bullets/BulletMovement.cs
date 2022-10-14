using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    public abstract class BulletMovement : ScriptableObject
    {
        public abstract void ModifyBullet(Bullet bullet);
        public abstract void Move(Bullet bullet);
    }
}
