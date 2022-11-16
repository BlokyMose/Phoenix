using Encore.Utility;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using static Encore.Utility.MathUtility;

namespace Phoenix
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class BorderController : MonoBehaviour
    {
        #region [Classes]

        public enum BorderOrientation { Auto, Vertical, Horizontal }


        #endregion

        [SerializeField]
        bool scaleAffectsCollider = true;


        [SerializeField]
        BorderOrientation orientation = BorderOrientation.Auto;
        public BorderOrientation Orientation => orientation;


        BoxCollider2D col;
        public BoxCollider2D Col => col;

        public Vector2 GetNearestBorderPosition(Vector2 otherPos)
        {
            switch (orientation)
            {
                case BorderOrientation.Auto:
                    if (otherPos.x > col.GetLeftPos().x && otherPos.x < col.GetRightPos().x)
                        return GetNearestBorderPositionHorizontal();
                    else if (otherPos.y > col.GetBottomPos().y && otherPos.y < col.GetTopPos().y)
                        return GetNearestBorderPositionVertical();
                    else
                        return GetNearestCornerPoint();
                case BorderOrientation.Vertical:
                    return GetNearestBorderPositionVertical();
                case BorderOrientation.Horizontal:
                    return GetNearestBorderPositionHorizontal();
                default:
                    return transform.position;
            }

            Vector2 GetNearestBorderPositionVertical()
            {
                if (otherPos.x > transform.position.x)
                {
                    return new Vector2(transform.position.x + col.GetScaledSize().x / 2, otherPos.y);
                }
                else
                {
                    return new Vector2(transform.position.x - col.GetScaledSize().x / 2, otherPos.y);
                }
            }

            Vector2 GetNearestBorderPositionHorizontal()
            {
                if (otherPos.y > transform.position.y)
                {
                    return new Vector2(otherPos.x, transform.position.y + col.GetScaledSize().y / 2);
                }
                else
                {
                    return new Vector2(otherPos.x, transform.position.y - col.GetScaledSize().y / 2);
                }
            }

            Vector2 GetNearestCornerPoint()
            {
                Vector2 nearestPos = col.GetTopLeftPos();
                var nearestMag = Mathf.Abs((otherPos - nearestPos).magnitude);

                var topRightMag = Mathf.Abs((otherPos - col.GetTopRightPos()).magnitude);
                if (nearestMag > topRightMag)
                {
                    nearestMag = topRightMag;
                    nearestPos = col.GetTopRightPos();
                }

                var bottomLeftMag = Mathf.Abs((otherPos - col.GetBottomLeftPos()).magnitude);
                if (nearestMag > bottomLeftMag)
                {
                    nearestMag = bottomLeftMag;
                    nearestPos = col.GetBottomLeftPos();
                }

                var bottomRightMag = Mathf.Abs((otherPos - col.GetBottomRightPos()).magnitude);
                if (nearestMag > bottomRightMag)
                {
                    nearestMag = bottomRightMag;
                    nearestPos = col.GetBottomRightPos();
                }

                return nearestPos;
            }
        }

        private void Awake()
        {
            col = GetComponent<BoxCollider2D>();
        }

        [Button]
        void Test(Line a, Line b)
        {
            Debug.Log(GetIntersection(a,b));
        }
    }
}
