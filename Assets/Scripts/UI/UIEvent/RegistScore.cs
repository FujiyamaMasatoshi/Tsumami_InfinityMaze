using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;




public class RegistScore : MonoBehaviour
{
    // 登録名を入力
    public TMP_InputField inputField; // Reference to the InputField

    // スコア登録パネル
    public GameObject scoreRegistPanel = null;

    // データ管理obj
    public DataManager dataManager = null;


    public void AddPlayerData()
    {
        // 名前を登録
        GameManager.instance.rank_name = inputField.text;
        if (GameManager.instance.rank_name != "")
        {
            // input fieldを空にする
            inputField.text = "";

            // 登録後、scoreRegistPanelをactive falseにする
            if (scoreRegistPanel != null) scoreRegistPanel.SetActive(false);

            // ####################
            // jsonファイルに書き込む
            // ####################

            // 追加するデータ
            PlayerData playerData = new PlayerData(GameManager.instance.rank_name, GameManager.instance.score);

            // データの追加保存
            dataManager.Save(playerData);
        }
    }
}
