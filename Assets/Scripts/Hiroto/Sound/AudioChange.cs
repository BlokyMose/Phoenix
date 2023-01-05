using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioChange : MonoBehaviour
{
    //AudoClip�̔z��Aclips��錾���܂��B
    public AudioClip[] clips;
    //AudioSource�^�̕ϐ�audios��錾���܂��B
    AudioSource audios;

    void Start()
    {
        audios = GetComponent<AudioSource>();
    }

    //Button1���N���b�N���ꂽ���̏���
    public void Button1Click()
    {
        audios.clip = clips[0];
        audios.Play();
    }

    //Button2���N���b�N���ꂽ���̏���
    public void Button2Click()
    {
        audios.clip = clips[1];
        audios.Play();
    }

    //Button3���N���b�N���ꂽ���̏���
    public void Button3Click()
    {
        audios.clip = clips[2];
        audios.Play();
    }

}