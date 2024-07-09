using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static int MAZE_VERTICAL = 15;
    public static int MAZE_WIDTH = 15;

    public int blockSize = 5;

    [Header("プレイヤ")] public GameObject player = null;
    [SerializeField] private int wallHeight = 3; // 1ステージあたりの壁の高さ
    [SerializeField] private int floorHeight = 1; //床の高さ
    


    // 迷路要素
    private char WALL = '#';
    private char PATH = ' ';
    private char START = 's';
    private char KEY = 'k';
    private char TREASURE = 't';
    private char GOAL = 'g';
    private char ENEMY = 'e';


    
    private int[] startPoint = { 0, 0}; // スタート地点
    private int[] goalPoint = { 0, 0 }; // ゴール地点

    private char[,] map = new char[MAZE_VERTICAL, MAZE_VERTICAL];


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
                    if (map[j,i] != START)
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




                //// 壁を生成
                //if (map[j, i] == WALL)
                //{
                //    for (int h = 1; h <= wallHeight + 1; h++)
                //    {
                //        Instantiate(block, new Vector3(
                //            j * blockSize * block.transform.localScale.x,
                //            (GameManager.instance.n_stage * (floorHeight + wallHeight) + h) * blockSize * block.transform.localScale.y,
                //            /*GameManager.instance.n_stage * (h+floorHeight+1) * blockSize * block.transform.localScale.y,*/
                //            i * blockSize * block.transform.localScale.z), Quaternion.identity);
                //    }
                //}

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



    // Start is called before the first frame update
    void Start()
    {
        // 初期マップを生成
        for (int i=0; i<GameManager.instance.n_first_stage; i++)
        {
            // 迷路を生成
            GenerateMap();

            // 迷路を表示
            PrintMaze();

            //// プレイヤを召喚
            //SpawnPlayer();


            // 迷路Objを生成
            GenerateMazeInScene();
        }
        
        
    }

    // Update is called once per frame
    void Update()
    {
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
    }
}
