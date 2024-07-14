using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LlamaCppUnity;
using System.Threading.Tasks;
using TMPro;
using System.Text.RegularExpressions;

public class LLMAgent : MonoBehaviour
{
    // LLMの最大トークン
    [SerializeField] private uint max_token = 32;
    [SerializeField] private float temperature = 0.2f;
    [SerializeField] private float moveStep = 5.0f;


    // LLM Agent
    private Llama llm; // llmモジュール
    private string model_path; // モデルパス
    private string initPrompt = "";
    private string userPrompt = ""; // Userプロンプト
    public bool isGenerating = false; // 生成中か
    private string action = ""; // アクション

    // 動いているかどうか
    private float dist = 0.0f; // 1回の行動で進んだ距離
    


    //　文字入力
    public TMP_InputField inputField; // Reference to the InputField

    // 出力文字列
    public TextMeshProUGUI outputText = null;
    

    // Start is called before the first frame update
    void Start()
    {
        // UserPromptの設定
        //initPrompt = "あなたはこれから与えられる指示に忠実なアシスタントです。<指示>の内容に会う行動方向を出力してください。その時、<出力形式>に従って出力してください。なお、行動方向は次の4つから選択してください。{forward, back, right, left}\n<出力形式>: JSON形式({\"move\": \"行動方向\"})\n(例){move: forward}\n(例)\n前に進め -> {move:forward}\n前に進んでください -> {move:forward}\n後ろに下がれ ->{move:back}\n戻れ -> {move:back}\n右に進みなさい -> {move:right}\n右に進め -> {move:right}\nもうちょっと左に行って -> {move:left}\n左に行け -> {move:left}\n以上のように、指示の内容に沿った形式で出力してください。\n<指示>: \n";

        initPrompt = "あなたはこれから与えられる指示に忠実なアシスタントです。JSON形式で、forward, back, right, leftを出力してください。";


        // モデルパス (Build for Mac)
        model_path = System.IO.Path.Combine(Application.streamingAssetsPath, "LLM_Model/Llama-3-ELYZA-JP-8B-Q3_K_L.gguf");

        // LLM
        llm = new Llama(model_path, chatFormat:"chatml"); //If there is insufficient memory, the model will fail to load.
    }

    // テキスト生成
    private string GenerateText()
    {
        // llm出力
        
        string result = llm.Run(userPrompt, maxTokens: max_token, temperature: temperature, topP: 0.95f, minP: 0.5f /* , repeatPenalty: 2.0f */);
        
        Debug.Log($"LLM Outputs: {result}");

        return result;
    }

    // 生成したテキストをcommandにセット
    async void SetCommand()
    {
        isGenerating = true;
        string llmOutputs = await Task.Run(() => GenerateText());
        outputText.text = GetActionFromLLMOutput(llmOutputs);

        // action設定
        action = outputText.text;

        Debug.Log($"outputText.text: {outputText.text}");

        

        isGenerating = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (action != "")
        {
            select_action(action);
        }
        
    }

    private void CreatePrompt(string userInput)
    {
        
        string ex = "(入力1)\n前に進め\n(出力1)\n{\"move\":\"forward\"\n}" + "(入力2)\n後ろにもどれ\n(出力2)\n{\"move\":\"back\"}" + "(入力3)\n右に進め\n(出力3)\n{\"move\":\"right\"}\n" + "(入力4)\n左に進んでください。\n(出力4)\n{\"move\":\"left\"}\n(入力5)\n"+userInput+"\n(出力5)\n";

        userPrompt = initPrompt + ex;

    }

    // ボタンクリック時に呼び出す
    public void SendMessage()
    {
        if (isGenerating == false && inputField.text != "")
        {
            //textCommand = inputField.text;
            //userPrompt = initPrompt + textCommand;
            CreatePrompt(inputField.text);
            SetCommand();
        }
        else
        {
            Debug.Log("you cannot send message because of now generating");
        }
    }


    // LLMの出力から、行動を取得する
    private string GetActionFromLLMOutput(string llmOut)
    {
        string pattern = @"\b(forward|back|right|left)\b";

        // マッチした文字列をリストに格納
        List<string> matchedStrings = new List<string>();
        foreach (Match match in Regex.Matches(llmOut, pattern))
        {
            matchedStrings.Add(match.Value);
        }

        // 結果を表示
        foreach (string str in matchedStrings)
        {
            Debug.Log($"抽出された文字列: {str}");
        }

        if (matchedStrings.Count == 0) return "not_matched";
        else return matchedStrings[0];
    }


    // ##############
    // 移動メソッド
    // ##############
    private void moveForward()
    {
        
        if (dist < moveStep)
        {
            transform.position += Vector3.forward * moveStep * Time.deltaTime;
            dist += moveStep * Time.deltaTime;
        }
        else
        {
            dist = 0.0f;
            action = "";
        }
    }

    private void moveBack()
    {
        if (dist < moveStep)
        {
            transform.position += Vector3.back * moveStep * Time.deltaTime;
            dist += moveStep * Time.deltaTime;
        }
        else
        {
            dist = 0.0f;
            action = "";
        }
    }

    private void moveRight()
    {

        if (dist < moveStep)
        {
            transform.position += Vector3.right * moveStep * Time.deltaTime;
            dist += moveStep * Time.deltaTime;
        }
        else
        {
            dist = 0.0f;
            action = "";
        }
    }

    private void moveLeft()
    {

        if (dist < moveStep)
        {
            transform.position += Vector3.left * moveStep * Time.deltaTime;
            dist += moveStep * Time.deltaTime;
        }
        else
        {
            dist = 0.0f;
            action = "";
        }
    }

    private void select_action(string action)
    {
        if (action == "forward")
        {
            moveForward();
        }
        else if(action == "back")
        {
            moveBack();
        }
        else if(action == "right")
        {
            moveRight();
        }
        else if(action == "left")
        {
            moveLeft();
        }
        else
        {
            Debug.Log("no action");
        }
    }

    

}
