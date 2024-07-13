using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TitleEvent : MonoBehaviour
{
    // タイトルへ戻る イベント
    public void Back2Title()
    {
        // タイトルシーンへ移動
        // TitleEventでシーン切り替えの前にゲーム情報はリセットする
        SceneManager.LoadScene("title");
    }

}
