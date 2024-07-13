using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LlamaCppUnity;
using System.Threading.Tasks;
using TMPro;

public class LLMAgent : MonoBehaviour
{
    // LLMの最大トークン
    [SerializeField] private uint max_token = 32;

    // LLM Agent
    private Llama llm; // llmモジュール
    private string model_path; // モデルパス
    private string initPrompt = "";
    private string userPrompt = ""; // Userプロンプト
    private bool isGenerating = false; // 生成中か
    private string textCommand = ""; // 生成された行動

    //　文字入力
    public TMP_InputField inputField; // Reference to the InputField
    private bool onePressedRtnKey = false;

    // 出力文字列
    public TextMeshProUGUI outputText = null;
    

    // Start is called before the first frame update
    void Start()
    {
        // UserPromptの設定
        initPrompt = "あなたはこれから与えられる指示に忠実なアシスタントです。これから与える<指示>の内容を<条件>に従って出力をしてください。<指示>には、あなたが出力すべき方向を教えてくれます。指示の内容に合う方向を<出力形式>の中から適切なものを1つ選び、出力してください。\n<出力形式>: {right}, {left}, {forward}, {back}\n<指示>: ";
        userPrompt += initPrompt;
        // モデルパス (Build for Mac)
        model_path = System.IO.Path.Combine(Application.streamingAssetsPath, "LLM_Model/Llama-3-ELYZA-JP-8B-Q3_K_L.gguf");

        // LLM
        llm = new Llama(model_path); //If there is insufficient memory, the model will fail to load.
    }

    // テキスト生成
    private string GenerateText()
    {
        string result = "";
        foreach (string text in llm.RunStream(userPrompt, maxTokens: max_token))
        {
            result += text;
        }
        return result;
    }

    // 生成したテキストをcommandにセット
    async void SetCommand()
    {
        isGenerating = true;
        outputText.text = await Task.Run(() => GenerateText());
        isGenerating = false;
    }

    // Update is called once per frame
    void Update()
    {
        // テキストボックスに文字入力を行い、決定する
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if ((inputField.text != "") && onePressedRtnKey)
            {
                textCommand = inputField.text;
                userPrompt = initPrompt + textCommand;
                onePressedRtnKey = false;
                if (isGenerating == false)
                {
                    SetCommand();
                }
            }
            else if (onePressedRtnKey)
            {
                onePressedRtnKey = false;
            }
            else if(onePressedRtnKey == false)
            {
                onePressedRtnKey = true;
            }
        }
        Debug.Log($"textCommand: {textCommand}");
    }



}
