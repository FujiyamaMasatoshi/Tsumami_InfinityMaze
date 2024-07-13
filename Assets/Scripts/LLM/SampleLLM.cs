using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LlamaCppUnity;

public class LlamaSample : MonoBehaviour
{
    void Start()
    {
        Llama test = new Llama("Assets/LLM_Model/ELYZA-japanese-Llama-2-7b-fast-q2_K.gguf"); //If there is insufficient memory, the model will fail to load.
        string result = test.Run("Q: Name the planets in the solar system? A: ", maxTokens: 16);
        //Output example: "1. Venus, 2. Mercury, 3. Mars,"

        //Stream Mode
        foreach (string text in test.RunStream("Q: 相手を挑発する言葉を発してください A: ", maxTokens: 512))
        {
            Debug.Log(text);
        }
    }
}
