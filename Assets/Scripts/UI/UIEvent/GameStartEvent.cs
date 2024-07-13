using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartEvent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ボタンクリック時 -- ゲームスタートイベント
    public void GameStart()
    {
        // ゲーム情報をリセット
        GameManager.instance.InitGame();

        // ステージ画面に遷移
        SceneManager.LoadScene("stage");
        
    }
}
