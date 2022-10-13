using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    public class JetComponents : MonoBehaviour
    {
        [SerializeField]
        List<Transform> fireOrigins = new List<Transform>();

        public List<Transform> FireOrigins {  get { return fireOrigins; } }
    }
}
