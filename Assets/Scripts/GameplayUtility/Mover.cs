using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    public class Mover : MonoBehaviour
    {
        [SerializeField]
        float duration = 180;

        [SerializeField, InlineButton(nameof(TeleportToDestination),"Teleport")]
        Transform destination;

        Vector2 direction;
        float time = 0f;
        Vector2 initialPos;

        void Start()
        {
            initialPos = transform.position;
            direction = destination.position - transform.position;
        }

        void Update()
        {
            if (time > duration) return;
            transform.position = direction * time / duration + initialPos;
            time += Time.deltaTime * Time.timeScale;
        }

        void TeleportToDestination() => transform.position = destination.position;
    }
}
