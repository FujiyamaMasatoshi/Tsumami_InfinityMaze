using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    private AudioSource bgm = null;
    // Start is called before the first frame update
    void Start()
    {
        // bgmを取得
        bgm = GetComponent<AudioSource>();

        // SoundManagerのcurrentBgmにセットする
        SoundManager.instance.currentBgm = bgm;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
