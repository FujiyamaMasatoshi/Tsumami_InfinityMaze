using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartEvent : MonoBehaviour
{
    public GameObject explainPanel = null;

    // Start is called before the first frame update
    void Start()
    {
        if (explainPanel != null) explainPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // ボタンクリック時 -- ゲームスタートイベント
    public void GameStart()
    {
        if (GameManager.instance.isFirstExplanation == false)
        {
            if (explainPanel != null) explainPanel.SetActive(true);
            GameManager.instance.isFirstExplanation = true;
        }
        else
        {
            ClickToStart();
        }
        
        
    }

    public void ClickToStart()
    {
        // ゲーム情報をリセット
        GameManager.instance.InitGame();

        // ステージ画面に遷移
        SceneManager.LoadScene("stage");
    }


}
