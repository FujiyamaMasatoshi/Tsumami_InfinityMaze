using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditEvent : MonoBehaviour
{
    public void Go2Credit()
    {
        // クレジットシーンへ遷移
        SceneManager.LoadScene("credit");
    }

    public void Back2Title()
    {
        // タイトルシーンへ移動
        // TitleEventでシーン切り替えの前にゲーム情報はリセットする
        SceneManager.LoadScene("title");
    }


    // 一つ前のシーンに戻る
    public void Back2BeforScene()
    {
        SceneManager.LoadScene(GameManager.instance.beforeScene);
    }
}
