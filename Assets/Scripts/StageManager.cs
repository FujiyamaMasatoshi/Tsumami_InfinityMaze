using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using LlamaCppUnity;
using System.Threading.Tasks;

public class StageManager : MonoBehaviour
{
    public static int MAZE_VERTICAL = 13;
    public static int MAZE_WIDTH = 13;

    public float blockSize = 5f;
    public float blockScale = 2f;

    public TextMeshProUGUI textMeshPro = null; // LLMで生成したメッセージを出力する

    [Header("プレイヤ")] public GameObject player = null;
    [Header("マグマ")] public GameObject lavaObj = null;
    
    
    //[SerializeField] private float waitLavaMovingTime = 10.0f; // マグマが動き出すまでの時間
    [SerializeField] private float lavaMovingSpeed = 10.0f;

    [SerializeField] private int wallHeight = 3; // 1ステージあたりの壁の高さ
    [SerializeField] private int floorHeight = 1; //床の高さ
    [SerializeField] private float startLavaHeight = -10;
    [SerializeField] private int n_enemy = 5;
    [SerializeField] private uint max_token = 32;
    [SerializeField] private float temperature = 0.5f;


    // LLM
    private Llama llm;
    private string initPrompt = "";
    private string userPrompt = "";
    private bool isGenerating = false;

    // 迷路要素
    private char WALL = '#';
    private char PATH = '.';
    private char START = 's';
    private char KEY = 'k';
    private char TREASURE = 't';
    private char GOAL = 'g';
    private char ENEMY = 'e';
    
    private int[] startPoint = { 0, 0}; // スタート地点
    private int[] goalPoint = { 0, 0 }; // ゴール地点

    private char[,] map = new char[MAZE_VERTICAL, MAZE_VERTICAL];


    // Start is called before the first frame update
    void Start()
    {
        // ######
        // LLM
        // ######
        string modelPath = System.IO.Path.Combine(Application.streamingAssetsPath, "LLM_Model/Llama-3-ELYZA-JP-8B-Q3_K_L.gguf");
        llm = new Llama(modelPath); //If there is insufficient memory, the model will fail to load.

        //initPrompt = "あなたは、フィールド上を動き回る敵キャラクタです。あなたは、プレイヤーにアタックするためにプレイヤーを探し回っています。プレイヤーを見つけた時、プレイヤーに対して話す言葉を考えて、<条件>を元に出力してください。\n <条件>\n* 「ガハハ!」や「ヒャッハー」のようにいかにも的敵キャラクタが喋りそうな口調にすること\n* プレイヤーを挑発するような内容\n* 発する言葉のみ";
        //initPrompt = "あなたは「エビシー」という名前のキャラクターです。\nその特徴は以下のとおり。\n-語尾は「〜だシ！」です。\n-プレイヤーからすると敵なので、挑発します。\n-一人称は「オイラ」です。\n-敬語を使いません。\n台詞の例は以下のとおり。\n-君、ゲームへたっぴだシ\n-そんなんじゃ良いスコアは出せないだシ！\n-言ってることの意味がわからないシ！";
        initPrompt = "ゲームに関するキーワードを3つ教えてください";
        

        userPrompt += initPrompt;

        // GMのタイマーをリセット (waitTime + lavaHeight)に設定
        GameManager.instance.lavaTime = Mathf.Abs(startLavaHeight)/lavaMovingSpeed;
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

    // クイズ生成
    private string GenerateQuizKeywords()
    {
        string QuizKeywords = llm.Run(initPrompt, maxTokens: max_token, temperature: temperature);
        return QuizKeywords;
    }

    async void GenerateQuiz()
    {
        isGenerating = true;
        string QuizKeywords = await Task.Run(() => GenerateQuizKeywords());
        Debug.Log($"クイズキーワード: {QuizKeywords}");
        userPrompt = initPrompt + $"\n以下は、あなたが生成したゲームに関するキーワードです。「{QuizKeywords}」" + "これに基づいて、クイズを生成してください。";
        textMeshPro.text = await Task.Run(() => llm.Run(userPrompt, maxTokens: max_token, temperature: temperature));
        isGenerating = false;
    }

    // 非同期処理
    private string AsyncGenerateText()
    {
        string result = "";
        foreach (string text in llm.RunStream(userPrompt, maxTokens: max_token, temperature: temperature))
        {
            result += text;
        }
        return result;
    }


    async void SetText()
    {
        isGenerating = true;
        // userPromptを設定
        userPrompt = initPrompt;
        textMeshPro.text = await Task.Run(() => AsyncGenerateText());
        isGenerating = false;
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
        //if (GameManager.instance.lavaTime > waitLavaMovingTime)
        //{
        //    // 何もしない
        //}
        //else if (GameManager.instance.lavaTime <= waitLavaMovingTime)
        //{
        //    lavaObj.transform.position += Vector3.up * Time.deltaTime * lavaMovingSpeed;
        //}
        //if (GameManager.instance.lavaTime <= 0)
        //{
        //    GameManager.instance.lavaTime = waitLavaMovingTime + Mathf.Abs(startLavaHeight)/lavaMovingSpeed + GameManager.instance.blockSize; // リセット
        //}
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

    // map情報を文字列型で取得
    private string GetMap()
    {
        string result = "";
        for (int i=0; i<MAZE_VERTICAL; i++)
        {
            for (int j=0; j<MAZE_WIDTH; j++)
            {
                result += map[i, j];
            }
            result += "\n";
        }
        return result;
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
        SetEnemy();
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
        Debug.Log($"startPoint: {startPoint[0]}, {startPoint[1]}");
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
        Debug.Log($"goalPoint:{goalPoint[0]}, {goalPoint[1]}");

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
                    else
                    {
                        Instantiate(block, new Vector3(
                    j * blockSize * block.transform.localScale.x,
                    (GameManager.instance.n_stage * (floorHeight + wallHeight) + floorHeight) * blockSize * block.transform.localScale.y,
                    i * blockSize * block.transform.localScale.z), Quaternion.identity);
                    }
                    
                }
                else
                {
                    // start以外の床だけ生成する
                    if (map[j,i] == START)
                    {
                        //GameObject emptyBlock = (GameObject)Resources.Load("emptyBlock");
                        //Instantiate(emptyBlock, new Vector3(
                        //j * blockSize * emptyBlock.transform.localScale.x,
                        //(GameManager.instance.n_stage * (floorHeight + wallHeight) + floorHeight) * blockSize * emptyBlock.transform.localScale.y,
                        //i * blockSize * emptyBlock.transform.localScale.z), Quaternion.identity);
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
                        }
                        // 他を生成
                        else
                        {
                            Instantiate(block, new Vector3(
                        j * blockSize * block.transform.localScale.x,
                        (GameManager.instance.n_stage * (floorHeight + wallHeight) + floorHeight) * blockSize * block.transform.localScale.y,
                        i * blockSize * block.transform.localScale.z), Quaternion.identity);
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
            //SetText();
            GenerateQuiz();
        }
    }
}
