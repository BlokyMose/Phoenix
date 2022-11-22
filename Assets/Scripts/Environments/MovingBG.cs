using Encore.Utility;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    public class MovingBG : MonoBehaviour
    {
        [Serializable]
        public struct MovementProperties
        {
            public Direction4 moveTo;
            public Vector2 direction
            {
                get  
                {
                    return moveTo switch
                    {
                        Direction4.Right => Vector2.right,
                        Direction4.Left => Vector2.left,
                        Direction4.Up => Vector2.up,
                        Direction4.Down => Vector2.down,
                        _ => Vector2.zero,
                    };
                }
            }
            public float speed;
            public float distance;

            public MovementProperties(Direction4 moveTo, float speed, float distance)
            {
                this.moveTo = moveTo;
                this.speed = speed;
                this.distance = distance;
            }
        }

        [SerializeField]
        List<Sprite> sprites = new List<Sprite>();

        [SerializeField]
        MovementProperties unitProperties = new MovementProperties();

        [SerializeField]
        Vector2 spawnOffset;

        [SerializeField]
        int spawnCount = 4;

        List<MovingBGUnit> units = new List<MovingBGUnit>();

        int currentSpriteIndex = 0;

        void Awake()
        {
            Init();
        }

        void Init()
        {
            for (int i = 0; i < spawnCount; i++)
                InstantiateUnit();
        }
            
        void InstantiateUnit()
        {
            var go = new GameObject(units.Count.ToString());
            var unit = go.AddComponent<MovingBGUnit>();
            unit.Init(transform, sprites[currentSpriteIndex], OnUnitPassedDistance);
            unit.StartMoving(GetStartPos(), unitProperties);
            units.Add(unit);
            currentSpriteIndex = (currentSpriteIndex + 1) % sprites.Count;

            Vector2 GetStartPos()
            {
                if (units.IsEmpty())
                    return Vector2.zero + spawnOffset;

                var previousUnitPos = units.GetLast().transform.position;
                var previousSpriteSize = units.GetLast().SR.sprite.bounds.size;
                var currentSpriteSize = unit.SR.bounds.size;
                return unitProperties.moveTo switch
                {
                    Direction4.Right => new Vector2(previousUnitPos.x - previousSpriteSize.x/2 - currentSpriteSize.x/2, previousUnitPos.y),
                    Direction4.Left => new Vector2(previousUnitPos.x + previousSpriteSize.x/2 + currentSpriteSize.x/2, previousUnitPos.y),
                    Direction4.Up => new Vector2(previousUnitPos.x, previousUnitPos.y - previousSpriteSize.y/2 - currentSpriteSize.x/2),
                    Direction4.Down => new Vector2(previousUnitPos.x, previousUnitPos.y + previousSpriteSize.y/2 + currentSpriteSize.x/2),
                    _ => Vector2.zero,
                };
            }

            void OnUnitPassedDistance()
            {
                var position = GetStartPos();
                unit.StartMoving(position, unitProperties);
                units.MoveToLast(unit);
            }
        }



    }
}
