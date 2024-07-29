using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LlamaCppUnity;
using System.Threading.Tasks;


public class LlamaWrapper
{
    //
    private Llama llm = null;
    public LlamaWrapper(string modelPath)
    {
        llm = new Llama(modelPath);
    }

    public void Run(string prompt)
    {
        llm.Run(prompt);
    }
}

public class LlamaSample : MonoBehaviour
{
    private Llama llm = null;

    public bool isGenerating = false;
    void Start()
    {
        string modelPath = System.IO.Path.Combine(Application.streamingAssetsPath, "LLM_Model/Llama-3-ELYZA-JP-8B-Q3_K_L.gguf");
        llm = new Llama(modelPath); //If there is insufficient memory, the model will fail to load.
                                    //string result = Task.Run(() => llm.Run("Q: x軸方向はsin波で、z軸方向は直線的に変化するtransform.positionを更新するC#プログラムを生成して A: ", maxTokens: 512));


        ////Stream Mode
        //foreach (string text in llm.RunStream("Q: 相手を挑発する言葉を発してください A: ", maxTokens: 512))
        //{
        //    Debug.Log(text);
        //}

        estimate();
    }

    private string LlmRun()
    {
        isGenerating = true;
        string llmOut = llm.Run("Q: x軸方向はsin波で、z軸方向は直線的に変化するtransform.positionを更新するC#プログラムを生成して A: ", maxTokens: 512, temperature: 0.8f);
        isGenerating = false;
        return llmOut;
    }

    async void estimate()
    {
        string llmOut = await Task.Run(() => LlmRun());
        Debug.Log(llmOut);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGenerating == false) estimate();
        }
    }
}
