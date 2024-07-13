using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetStageNum : MonoBehaviour
{
    private TextMeshProUGUI textMeshPro = null;
    private string initText = "";
    // Start is called before the first frame update
    void Start()
    {
        // get component
        textMeshPro = GetComponent<TextMeshProUGUI>();

        // initTextの設定
        initText = textMeshPro.text;
    }

    // Update is called once per frame
    void Update()
    {
        textMeshPro.text = initText + $"{GameManager.instance.n_now_stage}";
    }
}
