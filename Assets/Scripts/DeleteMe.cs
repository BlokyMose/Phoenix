using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    public class DeleteMe : MonoBehaviour
    {
        List<BorderController> borders;
        // Start is called before the first frame update
        void Start()
        {
            borders = new List<BorderController>(FindObjectsOfType<BorderController>());

        }

        // Update is called once per frame
        void Update()
        {
            var margin = Mathf.Abs((borders[0].GetNearestBorderPosition(transform.position) - (Vector2)transform.position).magnitude);
        }
    }
}
