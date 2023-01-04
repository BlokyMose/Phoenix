using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    public class LevelMainMenu : LevelManager
    {
        public override void Init()
        {
            base.Init();
            Player.DisplayCursorMenu(false);
        }
    }
}
