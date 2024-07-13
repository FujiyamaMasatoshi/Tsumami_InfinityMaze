using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetTime : MonoBehaviour
{
    private TextMeshProUGUI textMeshPro = null;
    private string initText = "";
    // Start is called before the first frame update
    void Start()
    {
        // get component
        textMeshPro = GetComponent<TextMeshProUGUI>();

        // initTextの設定
        //initText = textMeshPro.text;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimerText(GameManager.instance.lavaTime);
        //textMeshPro.text = initText + $"{GameManager.instance.lavaTime}";
    }

    void UpdateTimerText(float time)
    {
        // テキストを更新
        textMeshPro.text = $"ステージ{GameManager.instance.n_lava_stage}消滅まで\nあと" + time.ToString("F1")+"秒";
        Debug.Log("time: "+time);
    }
}
