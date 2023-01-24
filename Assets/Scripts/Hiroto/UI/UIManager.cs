using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Phoenix
{
    public class UIManager : MonoBehaviour
    {
        //�R��Panel���i�[����ϐ�
        //�C���X�y�N�^�[�E�B���h�E����Q�[���I�u�W�F�N�g��ݒ肷��
  
        [SerializeField] GameObject stageSelectPanel;
        [SerializeField] GameObject reportPanel;
        [SerializeField] GameObject operationPanel;

        // Start is called before the first frame update
        void Start()
        {
            //BackToMenu���\�b�h���Ăяo��
            BackToMenu();
        }


        //MenuPanel��XR-HubButton�������ꂽ�Ƃ��̏���
        //XR-HubPanel���A�N�e�B�u�ɂ���
        public void StageSelectDescription()
        {
            stageSelectPanel.SetActive(true);
        }


        //MenuPanel��UnityButton�������ꂽ�Ƃ��̏���
        //UnityPanel���A�N�e�B�u�ɂ���
        public void ReportDescription()
        {
            reportPanel.SetActive(true);
        } 
        
        public void OperationDescription()
        {
            operationPanel.SetActive(true);
        }


        //2��DescriptionPanel��BackButton�������ꂽ�Ƃ��̏���
        //MenuPanel���A�N�e�B�u�ɂ���
        public void BackToMenu()
        {
            stageSelectPanel.SetActive(false);
            reportPanel.SetActive(false);
            operationPanel.SetActive(false);
        }
    }
}
