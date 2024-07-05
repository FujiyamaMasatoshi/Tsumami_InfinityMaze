using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] const int MAZE_VERTICAL = 41;
    [SerializeField] const int MAZE_WIDTH = 41;

    public int blockSize = 5;



    // 迷路要素
    private char WALL = '#';
    private char PATH = ' ';
    private char START = 's';
    private char KEY = 'k';
    private char TREASURE = 't';
    private char GOAL = 'g';
    private char ENEMY = 'e';

    // プレイヤ
    [Header("プレイヤ")] public GameObject player = null;

    // プレイヤーのスポーン位置
    private Vector3 spawnPoint;

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
        int[] startPos = GetStartPosition();

        // 道を生成
        CarvePassage(startPos[0], startPos[1]);

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
    private int[] GetStartPosition()
    {
        System.Random random = new System.Random();

        int start_x = random.Next(MAZE_WIDTH / 2) * 2 + 1;
        int start_y = random.Next(MAZE_VERTICAL / 2) * 2 + 1;

        // プレイヤのspawnPOintを設定する
        spawnPoint = new Vector3(start_x, 0, start_y);
        // プレイヤの位置を更新
        player.transform.position = spawnPoint;

        // スタート位置を設定
        map[start_x, start_y] = START;


        // スタートポイントをreturnする
        return new int[] { start_x, start_y };
    }

    // ゴールを設定する
    private void SetGoal()
    {
        System.Random random = new System.Random();
        List<int[]> goalPositions = new List<int[]>();

        for (int x = 1; x < MAZE_WIDTH - 1; x++)
        {
            if (map[1,x] == PATH)
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
            if (map[y,1] == PATH)
            {
                goalPositions.Add(new int[] { 0, y });
            }
            if (map[y,MAZE_WIDTH - 2] == PATH)
            {
                goalPositions.Add(new int[] { MAZE_WIDTH - 1, y });
            }
        }

        int[] goal = goalPositions[random.Next(goalPositions.Count)];
        map[goal[1],goal[0]] = GOAL;
    }


    // Scene上に迷路を生成させる
    private void GenerateMazeInScene()
    {
        for (int i=0; i<MAZE_VERTICAL; i++)
        {
            for (int j=0; j<MAZE_WIDTH; j++)
            {
                
                if (map[j, i] == WALL)
                {
                    GameObject block = (GameObject)Resources.Load("block0");
                    Instantiate(block, new Vector3(i * blockSize, 0 * blockSize, j * blockSize), Quaternion.identity);
                    Instantiate(block, new Vector3(i * blockSize, 1 * blockSize, j * blockSize), Quaternion.identity);
                    Instantiate(block, new Vector3(i * blockSize, 2 * blockSize, j * blockSize), Quaternion.identity);
                    Instantiate(block, new Vector3(i * blockSize, 3 * blockSize, j * blockSize), Quaternion.identity);
                }
                
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        // 迷路を生成
        GenerateMap();

        // 迷路を表示
        PrintMaze();

        GenerateMazeInScene();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
