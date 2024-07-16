using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class RestaratEvent : MonoBehaviour
{
    // restartイベント
    public void Restart()
    {
        // シーン遷移
        SceneManager.LoadScene("stage");
    }
}
