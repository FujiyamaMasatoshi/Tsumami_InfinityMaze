using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    //private GameObject player;  //プレイヤー情報格納用
    private Vector3 offset; // 相対距離取得用

    void Start()
    {
        //　Playerの情報を取得
        //this.player = GameObject.Find("tsumamiObj");

        // メインカメラ（自分自身）とPlayerとの相対距離を求める
        offset = transform.position - player.transform.position;
    }

    void Update()
    {
        //　新しいトランスフォームの値を代入する
        transform.position = player.transform.position + offset;
    }
}
