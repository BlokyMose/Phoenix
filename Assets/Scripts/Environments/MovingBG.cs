using Encore.Utility;
using Sirenix.OdinInspector;
using System;
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

        [Serializable]
        public class BG
        {
            [SerializeField]
            Sprite sprite;
            public Sprite Sprite => sprite;

            [SerializeField]
            int sortingOrder = 0;
            public int SortingOrder => sortingOrder;

            [HorizontalGroup("Color")]
            [SerializeField, LabelText("New Color")]
            bool isOverrideColor = false;
            public bool IsOverrideColor => isOverrideColor;

            [HorizontalGroup("Color")]
            [SerializeField, LabelWidth(0.1f), ShowIf(nameof(isOverrideColor))]
            Color color;

            public Color Color => isOverrideColor ? color : Color.white;

            [HorizontalGroup("RectSize")]
            [SerializeField, LabelText("New Rect Size")]
            bool isOverrideSpriteSize = false;
            public bool IsOverrideSpriteSize => isOverrideSpriteSize;

            [HorizontalGroup("RectSize")]
            [SerializeField, LabelWidth(0.1f), ShowIf(nameof(isOverrideSpriteSize))]
            Vector2 spriteSize;

            public Vector2 SpriteSize => isOverrideSpriteSize ? spriteSize : Sprite.bounds.size;


        }

        [SerializeField, LabelText("BGs")]
        List<BG> bgs = new();

        [SerializeField]
        MovementProperties unitProperties = new();

        [SerializeField]
        Vector2 spawnOffset;

        [SerializeField]
        int spawnCount = 4;

        List<MovingBGUnit> units = new List<MovingBGUnit>();

        int currentBGIndex = 0;
        BG currentBG => bgs[currentBGIndex];

        void Awake()
        {
            Init();
        }

        void Init()
        {
            var allUnits = new List<MovingBGUnit>();
            for (int i = 0; i < spawnCount; i++)
            {
                var newUnit = InstantiateUnit();
                newUnit.StartMoving(GetStartPos(newUnit), unitProperties, true);
                units.Add(newUnit);
                currentBGIndex = (currentBGIndex + 1) % bgs.Count;
            }


            foreach (var unit in units)
                unit.isPaused = false;

            MovingBGUnit InstantiateUnit()
            {
                var go = new GameObject(units.Count.ToString());
                var unit = go.AddComponent<MovingBGUnit>();
                unit.Init(transform, currentBG, OnUnitPassedDistance);
                return unit;


                void OnUnitPassedDistance()
                {
                    unit.StartMoving(GetStartPos(unit), unitProperties);
                    units.MoveToLast(unit);
                }
            }

            Vector2 GetStartPos(MovingBGUnit unit)
            {
                if (units.IsEmpty())
                    return Vector2.zero + spawnOffset;

                var previousUnitPos = units.GetLast().transform.position;
                var previousSpriteSize = units.GetLast().bg.SpriteSize;
                var currentSpriteSize = unit.bg.SpriteSize;

                return unitProperties.moveTo switch
                {
                    Direction4.Right => new Vector2(previousUnitPos.x - previousSpriteSize.x / 2 - currentSpriteSize.x / 2, previousUnitPos.y),
                    Direction4.Left => new Vector2(previousUnitPos.x + previousSpriteSize.x / 2 + currentSpriteSize.x / 2, previousUnitPos.y),
                    Direction4.Up => new Vector2(previousUnitPos.x, previousUnitPos.y - previousSpriteSize.y / 2 - currentSpriteSize.x / 2),
                    Direction4.Down => new Vector2(previousUnitPos.x, previousUnitPos.y + previousSpriteSize.y / 2 + currentSpriteSize.x / 2),
                    _ => Vector2.zero,
                };
            }

        }
            




    }
}
