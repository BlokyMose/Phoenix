using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Phoenix
{
    public class UIManager : MonoBehaviour
    {
        //３つのPanelを格納する変数
        //インスペクターウィンドウからゲームオブジェクトを設定する
  
        [SerializeField] GameObject stageSelectPanel;
        [SerializeField] GameObject reportPanel;
        [SerializeField] GameObject operationPanel;

        // Start is called before the first frame update
        void Start()
        {
            //BackToMenuメソッドを呼び出す
            BackToMenu();
        }


        //MenuPanelでXR-HubButtonが押されたときの処理
        //XR-HubPanelをアクティブにする
        public void StageSelectDescription()
        {
            stageSelectPanel.SetActive(true);
        }


        //MenuPanelでUnityButtonが押されたときの処理
        //UnityPanelをアクティブにする
        public void ReportDescription()
        {
            reportPanel.SetActive(true);
        } 
        
        public void OperationDescription()
        {
            operationPanel.SetActive(true);
        }


        //2つのDescriptionPanelでBackButtonが押されたときの処理
        //MenuPanelをアクティブにする
        public void BackToMenu()
        {
            stageSelectPanel.SetActive(false);
            reportPanel.SetActive(false);
            operationPanel.SetActive(false);
        }
    }
}
