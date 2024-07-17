using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinEvent : MonoBehaviour
{
    [SerializeField] int point = 50;
    [SerializeField] float rotateSpeed = 50;


    // コインゲット
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.score += point;
            Destroy(this.gameObject);
        }
        if (other.CompareTag("lava"))
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        // 常にy軸中心二回り続ける
        transform.Rotate(new Vector3(rotateSpeed * Time.deltaTime, 0, 0));
    }
}
