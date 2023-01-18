using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    [InlineEditor]
    public abstract class BulletMovement : ScriptableObject
    {
        public virtual void ModifyBullet(BulletController bullet, out System.Object cache) => cache = null;

        public abstract void Move(BulletController bullet, ref System.Object cache);
    }
}
