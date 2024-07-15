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



    }

    private void CreatePrompt()
    {
        userPrompt = "入力された単語に関するクイズを答えと一緒に、json形式で生成してください。\n";
        string ex1 = "(入力1)\nりんご\n(出力)\n{\"quiz\": \"林檎のロゴを代表するテック企業は?\", \"answer\": \"Apple\"}";
        string ex3 = "(入力1)\nRPG\n(出力)\n{\"quiz\": \"堀井裕二が生みの親の国民的RPGは何?\", \"answer\": \"ドラゴンクエスト\"}";
        string ex4 = "(入力1)\nりんご\n(出力)\n{\"quiz\": \"林檎のロゴを代表するテック企業は?\", \"answer\": \"Apple\"}";

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