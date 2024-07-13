using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using LlamaCppUnity;
using System.Threading.Tasks;

public class TextGeneration : MonoBehaviour
{
    [SerializeField] private float timeTextDisplay = 5.0f;
    [SerializeField] private uint max_token = 32;
    private TextMeshProUGUI tmpText;
    private Llama llm;
    private string userPrompt = "";
    private bool isGeneratingText = false; // テキスト生成中フラグ

    void Start()
    {
        // TMProUGUI取得
        tmpText = GetComponent<TextMeshProUGUI>();

        // LLM
        string modelPath = System.IO.Path.Combine(Application.streamingAssetsPath, "LLM_Model/Llama-3-ELYZA-JP-8B-Q3_K_L.gguf");
        

        llm = new Llama(modelPath); //If there is insufficient memory, the model will fail to load.

        string meirei = "これから<map>として、迷路の情報が与えられます。このマップは、壁(#)、道(.)、スタート地点(s)、ゴール地点(g)で構成された2次元配列として表現されています。プレイヤはsからスタートしてゴール地点を目指します。その時のルートを\n<map>\n";
        userPrompt = meirei;

    }



    // 非同期処理
    private string AsyncGenerateText()
    {
        string result = "";
        foreach (string text in llm.RunStream(userPrompt, maxTokens: max_token))
        {
            result += text;
        }
        return result;
    }


    async void SetText()
    {
        isGeneratingText = true;
        tmpText.text = await Task.Run(() => AsyncGenerateText());
        isGeneratingText = false;
    }

    private void Update()
    {
        if (isGeneratingText == false)
        {
            SetText();
        }
    }

}