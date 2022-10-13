using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    public class QuitGame : MonoBehaviour
    {
        public void Quit()
        {

            Debug.Log("Quitted");
            Application.Quit();

        }
    }
}
