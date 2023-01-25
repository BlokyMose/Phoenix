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

        public void Init(Brain brain, float speed, CursorPack cursorPack)
        {
            this.cursorPack = cursorPack;
            var emptyGO = new GameObject("Cursor");
            base.Init(brain, speed, emptyGO);
            
            cursorImage = cursor.AddComponent<Image>();
            cursorImage.rectTransform.pivot = new Vector2(
                cursorPack.Normal.pivot.x/cursorPack.Normal.rect.width, 
                cursorPack.Normal.pivot.y/cursorPack.Normal.rect.height
                );

            cursorImage.sprite = cursorPack.Normal;
        }

        public override void Exit(Brain brain)
        {
            base.Exit(brain);
        }


        void PlayClickSFX(bool isFiring)
        {
            if (isShow && isFiring)
                clickAudioSource.Play();
        }

        protected override void SetFiring(bool isFiring)
        {
            base.SetFiring(isFiring);
            cursorImage.sprite = isFiring ? cursorPack.Click : cursorPack.Normal;
            PlayClickSFX(isFiring);
        }
    }
}
