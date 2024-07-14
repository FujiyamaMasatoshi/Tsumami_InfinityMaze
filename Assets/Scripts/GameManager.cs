using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // インスタンス
    public static GameManager instance = null;

    public float blockSize = 5.0f;
    
    public int n_first_stage = 2; // 初めにいくつのステージを生成するか
    public int n_stage = 0; // 生成したステージの数
    public int n_now_stage = 0; // プレイヤの現在のステージ数 stageManagerで更新
    public int n_lava_stage = 0;

    public bool isEventDoing = false; //イベント中かどうか

    public string beforeScene = "";

    public float lavaTime = 0.0f; // マグマが動き出すまでのカウントダウン(seconds)


    // スコア
    public int score = 0;
    public int stagePoint = 500; // ステージ攻略につき500ポイント
    public int enemyPoint = 50; // エネミーを倒すにつき50ポイント

    // startの前に呼び出される
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        
    }


    // ゲーム情報リセット
    public void InitGame()
    {
        // 各種変数の初期化
        blockSize = 5.0f;
        //n_first_stage = 2; // 初めにいくつのステージを生成するか
        n_stage = 0; // 生成したステージの数
        n_now_stage = 0; // プレイヤの現在のステージ数 stageManagerで更新
        isEventDoing = false; //イベント中かどうか
        beforeScene = "title";
        /* lavaTimeはStageMangerでリセットする */
        n_lava_stage = 0;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    

    // Update is called once per frame
    void Update()
    {
        // 現在のシーン
        string currentScene = SceneManager.GetActiveScene().name;

        // 現在のシーンがtitle or resultの時beforeSceneにnameをセットする
        // titleとresultは、ranking, credit sceneへの遷移と戻るを実装するため
        if (currentScene == "title" || currentScene == "result")
        {
            beforeScene = currentScene;
        }
    }
}
