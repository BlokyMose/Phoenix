using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Phoenix
{
    public class OerationButtun : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void OnClickOerationButtun()
        {
            SceneManager.LoadScene("Oeration");
        }
    }
}
