using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Phoenix
{
    public class CursorDisplayerMenu : CursorDisplayer
    {
        [SerializeField]
        AudioSourceRandom clickAudioSource;

        CursorPack cursorPack;
        Image cursorImage;

        public void Init(ref Action<Vector2> onPointerPos, ref Action<bool> onFiring, float speed, CursorPack cursorPack)
        {
            this.cursorPack = cursorPack;
            var emptyGO = new GameObject("Cursor");
            base.Init(ref onPointerPos, ref onFiring, speed, emptyGO);

            onFiring += PlayClickSFX;

            cursorImage = cursor.AddComponent<Image>();
            cursorImage.rectTransform.pivot = new Vector2(
                cursorPack.Normal.pivot.x/cursorPack.Normal.rect.width, 
                cursorPack.Normal.pivot.y/cursorPack.Normal.rect.height
                );

            cursorImage.sprite = cursorPack.Normal;
        }

        public override void Exit(ref Action<Vector2> onPointerPos, ref Action<bool> onFiring)
        {
            base.Exit(ref onPointerPos, ref onFiring);
            onFiring -= PlayClickSFX;
        }


        void PlayClickSFX(bool isFiring)
        {
            if (isShow && isFiring)
                clickAudioSource.Play();
        }

        protected override void OnFiring(bool isFiring)
        {
            base.OnFiring(isFiring);
            cursorImage.sprite = isFiring ? cursorPack.Click : cursorPack.Normal;
        }
    }
}
