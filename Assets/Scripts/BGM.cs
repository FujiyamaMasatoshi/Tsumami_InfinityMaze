using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    private AudioSource bgmSource = null;
    // Start is called before the first frame update
    void Start()
    {
        // bgmを取得
        bgmSource = GetComponent<AudioSource>();

        // SoundManagerのcurrentBgmにセットする
        SoundManager.instance.currentBgmSource = bgmSource;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
