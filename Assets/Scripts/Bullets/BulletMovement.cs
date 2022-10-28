using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    public abstract class BulletMovement : ScriptableObject
    {
        public abstract void ModifyBullet(BulletController bullet);
        public abstract void Move(BulletController bullet);
    }
}
