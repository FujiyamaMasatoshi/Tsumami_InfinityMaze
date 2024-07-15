using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class RestaratEvent : MonoBehaviour
{
    // restartイベント
    public void Restart()
    {
        // ゲームマネージャーの情報を初期化する
        GameManager.instance.InitGame();

        // ゲーム時間を進める
        Time.timeScale = 1.0f;

        // シーン遷移
        SceneManager.LoadScene("stage");
    }
}
