using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RankingManager : MonoBehaviour
{
    static int n_rankList = 10;
    public TextMeshProUGUI[] rankList = new TextMeshProUGUI[n_rankList];

    // dataマネジャー
    public DataManager dataManager = null;

    //private List<PlayerData> pData = new List<PlayerData>();
    private PlayerDataList pData = new PlayerDataList();

    // Start is called before the first frame update
    void Start()
    {
        // プレイヤーデータをload
        pData.playersData = dataManager.Load();
        

        // スコア順にソート
        SortByScore();

        // 名前を表示させていく
        SetPlayerNameByScore();
    }

    public void SortByScore()
    {
        pData.playersData.Sort((a, b) => b.score.CompareTo(a.score));
    }

    // スコア順にソート後、名前をセットしていく
    public void SetPlayerNameByScore()
    {
        int iter = n_rankList;
        if (n_rankList > pData.playersData.Count) iter = pData.playersData.Count;
        for (int i=0; i<iter; i++)
        {
            //if (rankList[i] != null && pData[i] != null) rankList[i].text = $"{pData[i].name} : {pData[i].score}";
            if (rankList[i] != null && pData.playersData[i] != null) rankList[i].text = $"{pData.playersData[i].name} : {pData.playersData[i].score}";


        }
        // ランキングよりもデータが少ない場合は、ランキングのリストを消す
        if (iter < n_rankList)
        {
            for (int i=iter; i<n_rankList; i++)
            {
                rankList[i].transform.parent.gameObject.SetActive(false);
            }
        }

        // debug
        //rankList[0].text = "json読み込めないよ ("+pData.playersData.Count+") (0だったら本当に読み込めていない)";
        //rankList[0].transform.parent.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
