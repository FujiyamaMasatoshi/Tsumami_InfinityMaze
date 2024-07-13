using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuEvent : MonoBehaviour
{
    private bool isOpenMenu = false;
    // Start is called before the first frame update
    void Start()
    {
        isOpenMenu = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            // menuが開かれている時
            if (isOpenMenu)
            {
                // menuを閉じる
                isOpenMenu = false;
                GameManager.instance.isEventDoing = false;
                Time.timeScale = 1.0f;

                
            }
            // menuが開かれていない
            else
            {
                // menuスタート
                // menu画面を開いている
                isOpenMenu = true;

                // イベント中フラグを立てる
                GameManager.instance.isEventDoing = true;

                // ゲーム時間を止める
                Time.timeScale = 0.0f;

                // menu画面での処理
                Menu();
            }
        }
    }

    // menu画面での処理
    public void Menu()
    {
        Debug.Log("menu open");
    }
}
