using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // インスタンス
    public static GameManager instance = null;

    public float blockSize = 5.0f;
    
    public int n_first_stage = 2; // 初めにいくつのステージを生成するか
    public int n_stage = 0; // 生成したステージの数
    public int n_now_stage = 0; // プレイヤの現在のステージ数 stageManagerで更新

    public bool isEventDoing = false; //イベント中かどうか


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


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
