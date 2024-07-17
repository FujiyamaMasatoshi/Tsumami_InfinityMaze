using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    // ############
    // bgm list
    // ############
    // 0. title
    // 1. stage
    // 2. result
    // 3. credit
    // 4. ranking
    


    // インスタンス
    public static SoundManager instance = null;

    public AudioSource currentBgm = null; // 現在のbgm


    // startの前に呼び出される
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void PlaySE(AudioClip se, float vol=1.0f)
    {
        if (currentBgm != null) currentBgm.PlayOneShot(se, vol);
    }

}
