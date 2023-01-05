using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioChange : MonoBehaviour
{
    //AudoClipの配列、clipsを宣言します。
    public AudioClip[] clips;
    //AudioSource型の変数audiosを宣言します。
    AudioSource audios;

    void Start()
    {
        audios = GetComponent<AudioSource>();
    }

    //Button1がクリックされた時の処理
    public void Button1Click()
    {
        audios.clip = clips[0];
        audios.Play();
    }

    //Button2がクリックされた時の処理
    public void Button2Click()
    {
        audios.clip = clips[1];
        audios.Play();
    }

    //Button3がクリックされた時の処理
    public void Button3Click()
    {
        audios.clip = clips[2];
        audios.Play();
    }

}