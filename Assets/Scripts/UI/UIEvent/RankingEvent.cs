using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RankingEvent : MonoBehaviour
{
    public void Go2Ranking()
    {
        // ランキングシーンへ遷移
        SceneManager.LoadScene("ranking");
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
