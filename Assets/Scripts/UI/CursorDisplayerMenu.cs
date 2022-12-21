using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Phoenix
{
    public class CursorDisplayerMenu : CursorDisplayer
    {
        CursorPack cursorPack;
        Image cursorImage;

        public void Init(ref Action<Vector2> onPointerPos, ref Action<bool> onFiring, float speed, CursorPack cursorPack)
        {
            this.cursorPack = cursorPack;
            var emptyGO = new GameObject("Cursor");
            base.Init(ref onPointerPos, ref onFiring, speed, emptyGO);

            cursorImage = cursor.AddComponent<Image>();
            cursorImage.rectTransform.pivot = new Vector2(
                cursorPack.Normal.pivot.x/cursorPack.Normal.rect.width, 
                cursorPack.Normal.pivot.y/cursorPack.Normal.rect.height
                );

            cursorImage.sprite = cursorPack.Normal;
        }

        protected override void OnFiring(bool isFiring)
        {
            base.OnFiring(isFiring);
            cursorImage.sprite = isFiring ? cursorPack.Click : cursorPack.Normal;
        }
    }
}
