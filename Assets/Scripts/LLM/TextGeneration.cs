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

        string meirei = "あなたは、フィールド上を動き回る敵キャラクタです。あなたは、プレイヤーにアタックするためにプレイヤーを探し回っています。プレイヤーを見つけた時、プレイヤーに対して話す言葉を考えて、<条件>を元に出力してください。\n <条件>\n* 「ガハハ!」や「ヒャッハー」のようにいかにも的敵キャラクタが喋りそうな口調にすること\n* プレイヤーを挑発するような内容\n* * 出力形式: 「セリフ」";

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