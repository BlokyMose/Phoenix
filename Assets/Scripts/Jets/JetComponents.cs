using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    public class JetComponents : MonoBehaviour
    {
        #region [Classes]

        [Serializable]
        public class FireMode
        {
            public string name;
            public List<Transform> origins = new List<Transform>();
            public enum FirePattern { Sequence, ConcurrentInstant, ConcurrentCooldown }
            public FirePattern pattern = FirePattern.Sequence;
        }

        #endregion

        #region [Vars: Properties]

        [SerializeField]
        List<FireMode> fireModes = new List<FireMode>();
        public List<FireMode> FireModes {  get { return fireModes; } }

        #endregion

    }
}
