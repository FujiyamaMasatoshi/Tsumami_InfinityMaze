using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{



    // プレイヤーデータをJSONファイルに保存する
    public void Save(PlayerData playerData)
    {

        // 既存のデータをjson形式で呼び出す
        List<PlayerData> allPlayerData = Load();
        allPlayerData.Add(playerData);
        // シリアライズ化してPlayerPrefsで書き込む
        string json = JsonUtility.ToJson(new PlayerDataList(allPlayerData), true);
        PlayerPrefs.SetString("playersData", json);

    }


    // JSONファイルから全てのプレイヤーデータを読み込む
    public List<PlayerData> Load()
    {

        // PlayerPrefs ver
        string json = PlayerPrefs.GetString("playersData", "");
        PlayerDataList playerDataList = JsonUtility.FromJson<PlayerDataList>(json);

        // json == "" => return empty list
        if (playerDataList == null) return new List<PlayerData>();
        else return playerDataList.playersData;
    }

    public void DeleteAllData()
    {
        // 既存のデータをjson形式で呼び出す
        List<PlayerData> allPlayerData = Load();
        allPlayerData.Clear();
        // シリアライズ化してPlayerPrefsで書き込む
        string json = JsonUtility.ToJson(new PlayerDataList(allPlayerData), true);
        PlayerPrefs.SetString("playersData", json);

        // ランキングシーンを再度呼び出す
        SceneManager.LoadScene("ranking");
    }
}

// ####################
// PlayerDataの保存
// ####################

// 保管するデータ構造
[System.Serializable]
public class PlayerData
{
    public string name;
    public int score;

    public PlayerData(string name, int score)
    {
        this.name = name;
        this.score = score;
    }
}

// PlayerDataを保持するリスト
[System.Serializable]
public class PlayerDataList
{
    public List<PlayerData> playersData;

    public PlayerDataList()
    {
        playersData = new List<PlayerData>();
    }

    public PlayerDataList(List<PlayerData> playersData)
    {
        this.playersData = playersData;
    }
}
