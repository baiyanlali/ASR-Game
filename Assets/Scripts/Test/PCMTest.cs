using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCMTest : MonoBehaviour
{
    public AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        PCMConvertor.PCM2WAV("D:/replay2.pcm", "D:/replay2.wav", 16000, 1, 8);
        //float[] clip = PCMConvertor.PCM2Floats("D:/replay.pcm");
        
        //AudioClip audioClip = AudioClip.Create("hei", clip.Length,1, 16000, false);
        //print( audioClip.SetData(clip, 0));
        //if (audioSource==null)
        //{
        //    print("no way!");
        //}
        //else
        //{
        //    print("yes");
        //}
        //audioSource.clip = audioClip ;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
