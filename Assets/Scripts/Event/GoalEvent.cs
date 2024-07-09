using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalEvent : MonoBehaviour
{
    [SerializeField] float upSpeed = 5.0f;


    private bool isExecuted = false; // 1回しか実行しないので、実行したかどうかを判定

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private IEnumerator GoNextStage(Collision collision)
    {
        if (!isExecuted)
        {
            // playerのGM.now_stageを+1する
            GameManager.instance.n_now_stage += 1;

            // ゲーム時間を止める
            Time.timeScale = 0.0f;

            GameManager.instance.isEventDoing = true;
            Vector3 startPos = transform.position;
            //Vector3 currentPos = transform.position;
            while (transform.position.y < startPos.y + 5/*blockSize*/* 3/*wallHeight*/*2.5/*block scale*/)
            {
                transform.position += Vector3.up * upSpeed; // blockを上昇させる
                collision.transform.position += Vector3.up * upSpeed; //プレイヤも上昇させる
                Debug.Log($"{transform.position.y}");
                yield return new WaitForSecondsRealtime(0.5f); // 1秒待つ（リアルタイム）

            }
            // 終了したら、イベント終了
            GameManager.instance.isEventDoing = false;

            isExecuted = true;

            // 時間を動かす
            Time.timeScale = 1.0f;

            
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(GoNextStage(collision));
    }


}
