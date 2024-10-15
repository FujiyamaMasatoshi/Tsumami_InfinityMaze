//MIT License

//Copyright (c) 2024 DefiosLab

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LlamaCppUnity;
using System;

public class LLMWrapper : MonoBehaviour
{

    private Llama llm = null;
    private string modelPath = System.IO.Path.Combine(Application.streamingAssetsPath, "Model/stabilityai-japanese-stablelm-3b-4e1t-instruct-Q4_0.gguf");

    private bool isSuccessed = false;

    public void InitLLM()
    {
        isSuccessed = false;
        while (!isSuccessed)
        {
            // LLMをロード
            LoadLLM();

            // ロードに成功したらisSuccessed=trueとなりロード完了となる
            if (isSuccessed)
            {
                Debug.Log("ロードに成功!!");
                return;
            }
        }
    }

    public void LoadLLM()
    {
        Debug.Log("modelPath: " + modelPath);
        try
        {
            llm = new Llama(modelPath, nCtx:1024);
            isSuccessed = true;
        }
        catch (ArgumentException e)
        {
            Debug.Log("ロードに失敗しました");
            isSuccessed = false;
        }

    }

    public string Run(string userPrompt, uint maxTokens, float temperature)
    {
        string result = llm.Run(userPrompt, maxTokens: maxTokens, temperature: temperature);
        return result;
    }


}
