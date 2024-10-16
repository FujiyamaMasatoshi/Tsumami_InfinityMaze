using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using LlamaCppUnity;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

public class StageManager : MonoBehaviour
{
    public static int MAZE_VERTICAL = 13;
    public static int MAZE_WIDTH = 13;

    public float blockSize = 5f;
    public float blockScale = 2f;

    public TextMeshProUGUI textMeshPro = null; // LLMで生成したメッセージを出力する

    [Header("プレイヤ")] public GameObject player = null;
    [Header("マグマ")] public GameObject lavaObj = null;

    [SerializeField] private LLMWrapper llm;
    
    
    //[SerializeField] private float waitLavaMovingTime = 10.0f; // マグマが動き出すまでの時間
    [SerializeField] private float lavaMovingSpeed = 10.0f;

    [SerializeField] private int wallHeight = 3; // 1ステージあたりの壁の高さ
    [SerializeField] private int floorHeight = 1; //床の高さ
    [SerializeField] private float startLavaHeight = -10;
    [SerializeField] private int n_enemy = 5;
    [SerializeField] private int n_coins = 10;
    [SerializeField] private uint max_token = 32;
    [SerializeField] private float temperature = 0.5f;
    [SerializeField] private float nonFloorRate = 0.3f; // 床を消す割合


    // LLM
    //private Llama llm;
    private string initPrompt = "";
    private string userPrompt = "";
    public bool isGenerating = false;
    [SerializeField] private int n_playerPos = 50; // 履歴に保持するプレイヤの行動履歴数
    [SerializeField] private float posAddTime = 0.2f;
    private float posTimer = 0.0f; //ポジション追加時に使用するタイマー
    public List<Vector2> playerPosSeq = new List<Vector2>(); // ポジション履歴
    public List<Vector2> goalPos = new List<Vector2>();
    
    
    private float cosSimilarity = 0.0f; // 進んでいる方向とゴールまでのcos類似度
    private float remaingTime = 0.0f; // ゲームオーバーまでの残り時間計算


    // 迷路要素
    private char WALL = '#';
    private char PATH = '.';
    private char START = 's';
    private char KEY = 'k';
    private char TREASURE = 't';
    private char GOAL = 'g';
    private char ENEMY = 'e';
    private char COIN = 'c';
    
    private int[] startPoint = { 0, 0}; // スタート地点
    private int[] goalPoint = { 0, 0 }; // ゴール地点

    private char[,] map = new char[MAZE_VERTICAL, MAZE_VERTICAL];


    // Start is called before the first frame update
    void Start()
    {
        // ゲームスタート
        GameStart();

    }

    private void GameStart()
    {
        // ゲーム初期化
        GameManager.instance.InitGame();
        Time.timeScale = 1.0f;

        // ######
        // LLM
        // ######
        llm.InitLLM();
        //string modelPath = System.IO.Path.Combine(Application.streamingAssetsPath, "Model/stabilityai-japanese-stablelm-3b-4e1t-instruct-Q4_0.gguf");
        //llm = new Llama(modelPath); //If there is insufficient memory, the model will fail to load.


        initPrompt = "あなたはこれから与えられる指示に忠実なアシスタントです。あなたには(1)プレイヤの進んでいるベクトルとプレイヤとゴールまでのベクトルのcos類似度と、(2)ゲームオーバーまでの時間が与えられます。プレイヤに適切な言葉をかけてください。ただし、残り時間について具体的な言及はしないでください。\n";


        // GMのタイマーをリセット (waitTime + lavaHeight)に設定
        GameManager.instance.lavaTime = Mathf.Abs(startLavaHeight) / lavaMovingSpeed;
        GameManager.instance.n_lava_stage += 1;

        // マグマ生成
        for (int i = 0; i < MAZE_WIDTH; i++)
        {
            for (int j = 0; j < MAZE_VERTICAL; j++)
            {
                GenerateLava(i, j);
            }
        }


        // 初期マップを生成
        for (int i = 0; i < GameManager.instance.n_first_stage; i++)
        {
            // 迷路を生成
            GenerateMap();

            // 迷路を表示
            PrintMaze();


            // 迷路Objを生成
            GenerateMazeInScene();
        }
    }



    private void CreatePrompt()
    {
        // cos類似度計算
        compute_CosSimilarity();

        // 残り時間計測
        compute_RemainingTime();
        userPrompt = "";
        userPrompt += initPrompt;
        string ex1 = "(入力1)\n{\"cos\":1.0, \"time\":10.0}\n(出力1)「時間が残り少ないけど、向かっている方向にゴールがあるよ!!」\n";
        string ex2 = "(入力2)\n{\"cos\":-1.0, \"time\":30.0}\n(出力2)「ゴールと逆方向に進んでいるよ!!」\n";
        string ex3 = "(入力3)\n{\"cos\":-0.7, \"time\":35.0}\n(出力3)「ゴールと逆方向に進んでいるよ!!。時間はたっぷりあるから焦らないで!!」\n";
        string ex4 = "(入力4)\n{\"cos\":0.6, \"time\":15.0}\n(出力4)「残り時間が残り少なくなってきているよ。その調子でゴールまで急いで！！」\n";

        string input = "(入力5)\n{\"cos\":"+cosSimilarity+", \"time\":"+remaingTime+"}\n(出力5)\n";

        // few shot prompt
        userPrompt += ex1 + ex2 + ex3 + ex4 + input;
        //userPrompt += ex1 + ex2 + ex3 + input;
    }


    private void compute_CosSimilarity()
    {
        // プレイヤの行動方向
        if (playerPosSeq.Count > 10)
        {
            Vector2 pDirection = playerPosSeq[playerPosSeq.Count - 1] - playerPosSeq[0];

            // プレイヤとゴールまでのベクトル
            Vector2 player2goal = goalPos[GameManager.instance.n_now_stage] - playerPosSeq[0];


            // cos類似度を計算
            if (pDirection.magnitude != 0 && player2goal.magnitude != 0)
            {
                cosSimilarity = Vector2.Dot(pDirection, player2goal) / (pDirection.magnitude * player2goal.magnitude);
                Debug.Log($"cos similarity: {cosSimilarity}");
            }
            else
            {
                Debug.Log("not compute cos similarity");
            }
        }
        

        
        
    }


    // 残り時間計算
    private void compute_RemainingTime()
    {
        if (lavaMovingSpeed != 0)
        {
            // 1層あたりの残り時間

            float remaingTimePerFloor = (4 * floorHeight * blockSize * blockScale) / lavaMovingSpeed;

            // 残り時間をセット
            remaingTime = remaingTimePerFloor * (GameManager.instance.n_now_stage - GameManager.instance.n_lava_stage) + GameManager.instance.lavaTime;
        }else
        {
            float remaingTimePerFloor = (4 * floorHeight * blockSize * blockScale) / 1;
            // 残り時間をセット
            remaingTime = remaingTimePerFloor * (GameManager.instance.n_now_stage - GameManager.instance.n_lava_stage) + GameManager.instance.lavaTime;
        }
        
    }


    private static string ExtractQuotedParts(string input)
    {
        // 正規表現を使って「」で囲まれた部分を抽出
        List<string> quotedParts = new List<string>();
        string pattern = "「(.*?)」";
        MatchCollection matches = Regex.Matches(input, pattern);

        foreach (Match match in matches)
        {
            quotedParts.Add(match.Groups[1].Value);
        }

        if (quotedParts.Count != 0) return quotedParts[0];
        else return "cannot get pattern"+input;
    }


    // 非同期処理
    private string AsyncGenerateText()
    {
        string result = llm.Run(userPrompt, max_token, temperature);
        return result;
    }


    async void SetText()
    {
        CreatePrompt(); // userPromptの設定
        if (cosSimilarity != float.NaN)
        {
            isGenerating = true;

            string llmOut = await Task.Run(() => AsyncGenerateText());
            textMeshPro.text = ExtractQuotedParts(llmOut);
            isGenerating = false;
        }
        
    }



    /// <summary>
    /// map[j, -startLavaPos, i]にマグマ生成
    /// </summary>
    private void GenerateLava(int i, int j)
    {

        // lavaObjの子オブジェクトとしてlavaをInstantiateする
        GameObject lava = Instantiate((GameObject)Resources.Load("lava"), Vector3.zero, Quaternion.identity);
        lava.transform.SetParent(lavaObj.transform);

        // 位置調整
        lava.transform.position = new Vector3(
                        j * blockSize * lava.transform.localScale.x,
                        startLavaHeight,
                        i * blockSize * lava.transform.localScale.z);
   
    }

    /// <summary>
    /// マグマを動かす, updateで呼び出す
    /// </summary>
    private void MoveLava()
    {
        lavaObj.transform.position += Vector3.up * Time.deltaTime * lavaMovingSpeed;
        if (GameManager.instance.lavaTime <= 0)
        {
            GameManager.instance.lavaTime = (4*floorHeight * blockSize * blockScale) / lavaMovingSpeed; // リセット
            GameManager.instance.n_lava_stage += 1;
        }
    }


    // mapをconsoleに表示
    private void PrintMaze()
    {
        string map_string = "";
        for (int i=0; i<MAZE_VERTICAL; i++)
        {
            for (int j=0; j<MAZE_WIDTH; j++)
            {
                map_string += map[i, j];
            }
            map_string += "\n";
        }
        Debug.Log(map_string);
    }


    private void GenerateMap()
    {
        // mapの初期化
        InitializeMaze();

        // スタートを設定
        SetStartPosition();

        // 道を生成
        CarvePassage(startPoint[0], startPoint[1]);

        // ゴールを設定
        SetGoal();


        // Enemyを配置
        //SetEnemy();

        // Coinを設置
        SetCoins();
    }

    // 迷路マップの初期化
    private void InitializeMaze()
    {
        for (int i=0; i<MAZE_VERTICAL; i++)
        {
            for (int j=0; j<MAZE_WIDTH; j++)
            {
                map[i, j] = WALL;
            }
        }
    }


    // int[]を要素にとるリストをシャッフルするメソッド
    private void Shuffle(List<int[]> intArrayList)
    {
        System.Random rng = new System.Random();
        int n = intArrayList.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            int[] value = intArrayList[k];
            intArrayList[k] = intArrayList[n];
            intArrayList[n] = value;
        }
    }

    // 道を生成していく
    // 深さ優先探索によって実装する
    private void CarvePassage(int start_x, int start_y)
    {
        List<int[]> directions = new List<int[]>
        {
            new int[] { 0, 2},
            new int[] { 2, 0 },
            new int[] {0, -2},
            new int[] {-2, 0}
        };
        // directionsをシャッフルする
        Shuffle(directions);

        // 道を生成していく
        foreach (int[] direction in directions)
        {
            int nx = start_x + direction[0];
            int ny = start_y + direction[1];

            if(nx > 0 && ny > 0 && nx < MAZE_WIDTH - 1 && ny < MAZE_VERTICAL - 1 && map[ny,nx] == WALL)
            {
                map[ny, nx] = PATH;
                map[start_y + direction[1] / 2, start_x + direction[0] / 2] = PATH;
                CarvePassage(nx, ny);
            }
        }
    }

    // スタートを設定する
    private void SetStartPosition()
    {
        // 初期生成飲み自動生成させる
        // 2回目以降は、goalPointをstartPointにする
        if (GameManager.instance.n_stage == 0)
        {
            System.Random random = new System.Random();

            int start_x = random.Next(MAZE_WIDTH / 2) * 2 + 1;
            int start_y = random.Next(MAZE_VERTICAL / 2) * 2 + 1;

            // プレイヤのspawnPOintを設定する
            startPoint[0] = start_x;
            startPoint[1] = start_y;



            // スタート位置を設定
            map[startPoint[1], startPoint[0]] = START;
        }
        else
        {
            startPoint = goalPoint;
            // スタート位置を設定
            map[startPoint[1], startPoint[0]] = START;

        }
        //Debug.Log($"startPoint: {startPoint[0]}, {startPoint[1]}");
    }

    // ゴールを設定する
    private void SetGoal()
    {
        System.Random random = new System.Random();
        List<int[]> goalPositions = new List<int[]>();

        for (int x = 1; x < MAZE_WIDTH - 1; x++)
        {
            if (map[1, x] == PATH)
            {
                goalPositions.Add(new int[] { x, 0 });
            }
            if (map[MAZE_VERTICAL - 2, x] == PATH)
            {
                goalPositions.Add(new int[] { x, MAZE_VERTICAL - 1 });
            }
        }

        for (int y = 1; y < MAZE_VERTICAL - 1; y++)
        {
            if (map[y, 1] == PATH)
            {
                goalPositions.Add(new int[] { 0, y });
            }
            if (map[y, MAZE_WIDTH - 2] == PATH)
            {
                goalPositions.Add(new int[] { MAZE_WIDTH - 1, y });
            }
        }

        // goalPointの設定
        goalPoint = goalPositions[random.Next(goalPositions.Count)];

        //goalPointの調整
        if (goalPoint[0] == 0)
        {
            goalPoint[0] = 1;
        } else if (goalPoint[0] == MAZE_WIDTH - 1)
        {
            goalPoint[0] = MAZE_WIDTH - 2;
        } else if (goalPoint[1] == 0)
        {
            goalPoint[1] = 1;
        } else if (goalPoint[1] == MAZE_VERTICAL - 1)
        {
            goalPoint[1] = MAZE_VERTICAL - 2;
        }

        map[goalPoint[1], goalPoint[0]] = GOAL;
        //Debug.Log($"goalPoint:{goalPoint[0]}, {goalPoint[1]}");

        // startPointからの調整
        if (startPoint[0] == MAZE_WIDTH-2)
        {
            map[startPoint[0] - 1, startPoint[1]] = PATH;
        }
        else if (startPoint[0] == 1)
        {
            map[startPoint[0] + 1, startPoint[1]] = PATH;
        }
        else if (startPoint[1] == MAZE_WIDTH - 2)
        {
            map[startPoint[0], startPoint[1] - 1] = PATH;
        }
        else if (startPoint[1] == 1)
        {
            map[startPoint[0], startPoint[1] + 1] = PATH;
        }
        else
        {
            map[startPoint[0] - 1, startPoint[1]] = PATH;
        }


    }


    private void SetEnemy()
    {
        // n体のEnemyをセットする
        int now_enemy = 0;
        while (now_enemy < n_enemy)
        {
            int x = Random.Range(0, MAZE_WIDTH);
            int y = Random.Range(0, MAZE_VERTICAL);

            if (map[y, x] == PATH)
            {
                map[y, x] = ENEMY;
                now_enemy += 1;
            }
        }
    }

    private void SetCoins()
    {
        int now_coin = 0;
        while (now_coin < n_coins)
        {
            int x = Random.Range(0, MAZE_WIDTH);
            int y = Random.Range(0, MAZE_VERTICAL);
            if (map[y, x] == PATH)
            {
                map[y, x] = COIN;
                now_coin += 1;
            }
        }
    }

    // Scene上に迷路を生成させる
    private void GenerateMazeInScene()
    {

        for (int i=0; i<MAZE_VERTICAL; i++)
        {
            for (int j=0; j<MAZE_WIDTH; j++)
            {
                // 現在のマップポイント

                GameObject block = (GameObject)Resources.Load("block0");
                // ###################
                // 床を生成
                // ###################

                // 最初の床は全て生成する
                if (GameManager.instance.n_stage == 0)
                {
                    if (map[j, i] == START)
                    {
                        GameObject startBlock = (GameObject)Resources.Load("startPoint");
                        Instantiate(startBlock, new Vector3(
                    j * blockSize * startBlock.transform.localScale.x,
                    (GameManager.instance.n_stage * (floorHeight + wallHeight) + floorHeight) * blockSize * startBlock.transform.localScale.y,
                    i * blockSize * startBlock.transform.localScale.z), Quaternion.identity);
                    }
                    else if(map[j, i] == GOAL)
                    {
                        GameObject goalBlock = (GameObject)Resources.Load("goalPoint");
                        Instantiate(goalBlock, new Vector3(
                    j * blockSize * goalBlock.transform.localScale.x,
                    (GameManager.instance.n_stage * (floorHeight + wallHeight) + floorHeight) * blockSize * goalBlock.transform.localScale.y,
                    i * blockSize * goalBlock.transform.localScale.z), Quaternion.identity);
                    }
                    // 床をランダムで生成
                    else
                    {
                        float p = Random.value;
                        if (p < nonFloorRate)
                        {
                            // 何もしない
                        }
                        else
                        {
                            Instantiate(block, new Vector3(
                        j * blockSize * block.transform.localScale.x,
                        (GameManager.instance.n_stage * (floorHeight + wallHeight) + floorHeight) * blockSize * block.transform.localScale.y,
                        i * blockSize * block.transform.localScale.z), Quaternion.identity);
                        }
                    }
                    
                }
                else
                {
                    // start以外の床だけ生成する
                    if (map[j,i] == START)
                    {
                        // 何も生成しない
                    }
                    else
                    {
                        // GOALを生成する時
                        if (map[j, i] == GOAL)
                        {
                            GameObject goalBlock = (GameObject)Resources.Load("goalPoint");
                            Instantiate(goalBlock, new Vector3(
                        j * blockSize * goalBlock.transform.localScale.x,
                        (GameManager.instance.n_stage * (floorHeight + wallHeight) + floorHeight) * blockSize * goalBlock.transform.localScale.y,
                        i * blockSize * goalBlock.transform.localScale.z), Quaternion.identity);

                            // goalPosに追加
                            goalPos.Add(new Vector2(
                                j * blockSize * blockScale,
                                i * blockSize * blockScale));
                        }
                        // 他を生成
                        else
                        {

                            //    Instantiate(block, new Vector3(
                            //j * blockSize * block.transform.localScale.x,
                            //(GameManager.instance.n_stage * (floorHeight + wallHeight) + floorHeight) * blockSize * block.transform.localScale.y,
                            //i * blockSize * block.transform.localScale.z), Quaternion.identity);

                            float p = Random.value;
                            if (p < nonFloorRate)
                            {
                                // 何もしない
                            }
                            else
                            {
                                Instantiate(block, new Vector3(
                            j * blockSize * block.transform.localScale.x,
                            (GameManager.instance.n_stage * (floorHeight + wallHeight) + floorHeight) * blockSize * block.transform.localScale.y,
                            i * blockSize * block.transform.localScale.z), Quaternion.identity);
                            }

                        }
                    }
                }




                // 壁を生成
                if (map[j, i] == WALL)
                {
                    for (int h = 1; h <= wallHeight + 1; h++)
                    {
                        block = (GameObject)Resources.Load("wall");
                        Instantiate(block, new Vector3(
                            j * blockSize * block.transform.localScale.x,
                            (GameManager.instance.n_stage * (floorHeight + wallHeight) + h) * blockSize * block.transform.localScale.y,
                            /*GameManager.instance.n_stage * (h+floorHeight+1) * blockSize * block.transform.localScale.y,*/
                            i * blockSize * block.transform.localScale.z), Quaternion.identity);


                    }
                    
                }


                // 敵をInstantiate
                if (map[j, i] == ENEMY)
                {
                    GameObject enemy = (GameObject)Resources.Load("enemy1");
                    Instantiate(enemy, new Vector3(j * blockSize * block.transform.localScale.x,
                        (GameManager.instance.n_stage * (floorHeight + wallHeight) + floorHeight + 1) * blockSize * block.transform.localScale.y,
                        i * blockSize * block.transform.localScale.z
                        ), Quaternion.identity);
                }

                // coinを生成
                if (map[j, i] == COIN)
                {
                    GameObject coin = (GameObject)Resources.Load("Coin");
                    Instantiate(coin, new Vector3(j * blockSize * block.transform.localScale.x,
                        (GameManager.instance.n_stage * (floorHeight + wallHeight) + floorHeight + 1) * blockSize * block.transform.localScale.y,
                        i * blockSize * block.transform.localScale.z
                        ), Quaternion.Euler(0, 0, 90));
                }

                // プレイヤを設置
                // 初期生成のみプレイヤを設置する
                if (GameManager.instance.n_stage == 0)
                {
                    if (map[j, i] == START)
                    {
                        // プレイヤ設置
                        player.transform.position = new Vector3(
                            j * blockSize * block.transform.localScale.x,
                            (GameManager.instance.n_stage * (floorHeight + wallHeight) + floorHeight+1) * blockSize * block.transform.localScale.y,
                            i * blockSize * block.transform.localScale.z);

                        // GM.n_stageを+1する
                        GameManager.instance.n_now_stage += 1;
                    }
                }
                
            }
        }

        // GameaManagerの値を更新
        GameManager.instance.n_stage += 1;

    }




    // Update is called once per frame
    void Update()
    {

        // ゲーム時間を進める
        GameManager.instance.lavaTime -= Time.deltaTime;

        if (GameManager.instance.n_stage - GameManager.instance.n_now_stage == 2)
        {
            

            // ###################
            // 迷路を生成する
            // ###################
            // 迷路を生成
            GenerateMap();

            // 迷路を表示
            PrintMaze();

            //// プレイヤを召喚
            //SpawnPlayer();


            // 迷路Objを生成
            GenerateMazeInScene();

        }
        //Debug.Log($"player.transform.position in Update: {player.transform.position}");

        // #############
        // マグマを動かす
        // #############
        MoveLava();


        // テキスト生成
        if (isGenerating == false)
        {
            SetText();
            //GenerateQuiz();
        }


        // プレイヤのPosを保持していく
        posTimer += Time.deltaTime;
        if (playerPosSeq.Count < n_playerPos)
        {
            Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.z);
            if (posTimer > posAddTime)
            {
                playerPosSeq.Add(playerPos);
                posTimer = 0.0f;
            }
        }
        else
        {
            playerPosSeq.RemoveAt(0);
            Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.z);
            if (posTimer > posAddTime)
            {
                playerPosSeq.Add(playerPos);
                posTimer = 0.0f;
            }
        }


        // イベント中は、テキストを表示させない
        if (GameManager.instance.isEventDoing)
        {
            textMeshPro.gameObject.SetActive(false);
        }
        else
        {
            textMeshPro.gameObject.SetActive(true);
        }
    }
}
