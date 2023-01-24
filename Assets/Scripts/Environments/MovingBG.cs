using Encore.Utility;
using Sirenix.OdinInspector;
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

        [Serializable]
        public class Unit
        {
            Transform Transform { get; set; }
            public SpriteRenderer SR { get; private set; }
            public BG BG { get; private set; }

            public Unit(Transform transform, SpriteRenderer sr, BG bg)
            {
                this.Transform = transform;
                this.SR = sr;
                this.BG = bg;
            }

            public void DestroySelf()
            {
                Destroy(Transform);
            }

            public Vector2 Pos
            {
                get => Transform.position;
                set => Transform.position = value;
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

        List<Unit> units = new();

        void Awake()
        {
            Init();
        }

        void Init()
        {
            foreach (var unit in units)
                unit.DestroySelf();
            units.Clear();

            var finishPos = GetFinishPos(unitProperties);
            var initialUnits = new List<Unit>();
            for (int i = 0; i < spawnCount; i++)
                initialUnits.Add(InstantiateUnit(i, bgs.GetAt(i, 0)));

            for (int i = 0; i < initialUnits.Count; i++)
            {
                initialUnits[i].Pos = GetStartPos(units, initialUnits[i]);
                units.Add(initialUnits[i]);
            }

            StartCoroutine(Update());
            IEnumerator Update()
            {
                while (true)
                {
                    foreach (var unit in units)
                    {
                        if (Vector2.Distance(unit.Pos, finishPos) < 1f)
                        {
                            unit.Pos = GetStartPos(units, unit);
                            units.MoveToLast(unit);
                            break;
                        }

                        unit.Pos += Time.deltaTime * Time.timeScale * unitProperties.speed * unitProperties.direction ;
                    }

                    yield return null;
                }
            }


            Unit InstantiateUnit(int index, BG bg)
            {
                var go = new GameObject(index.ToString());
                go.transform.parent = transform;
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = bg.Sprite;
                sr.sortingOrder = bg.SortingOrder;
                sr.color = bg.Color;
                sr.sortingLayerName = "BG";
                var unit = new Unit(go.transform, sr, bg);

                return unit;

            }

            Vector2 GetStartPos(List<Unit> units, Unit unit)
            {
                if (units.IsEmpty())
                    return Vector2.zero + spawnOffset;

                var previousUnitPos = units.GetLast().Pos;
                var previousSpriteSize = units.GetLast().BG.SpriteSize;
                var currentSpriteSize = unit.BG.SpriteSize;

                return unitProperties.moveTo switch
                {
                    Direction4.Right => new Vector2(previousUnitPos.x - previousSpriteSize.x / 2 - currentSpriteSize.x / 2, previousUnitPos.y),
                    Direction4.Left => new Vector2(previousUnitPos.x + previousSpriteSize.x / 2 + currentSpriteSize.x / 2, previousUnitPos.y),
                    Direction4.Up => new Vector2(previousUnitPos.x, previousUnitPos.y - previousSpriteSize.y / 2 - currentSpriteSize.x / 2),
                    Direction4.Down => new Vector2(previousUnitPos.x, previousUnitPos.y + previousSpriteSize.y / 2 + currentSpriteSize.x / 2),
                    _ => Vector2.zero,
                };
            }

            Vector2 GetFinishPos(MovementProperties properties)
            {
                return properties.moveTo switch
                {
                    Direction4.Right => (Vector2)transform.position + new Vector2(properties.distance, 0),
                    Direction4.Left => (Vector2)transform.position + new Vector2(-properties.distance, 0),
                    Direction4.Up => (Vector2)transform.position + new Vector2(0, properties.distance),
                    Direction4.Down => (Vector2)transform.position + new Vector2(0, -properties.distance),
                    _ => transform.position,
                };
            }

        }
            
    }
}
