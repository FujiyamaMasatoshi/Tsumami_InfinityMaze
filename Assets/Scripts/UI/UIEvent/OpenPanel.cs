using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenPanel : MonoBehaviour
{
    [SerializeField, Header("初めのactiveがfalseかどうか")] private bool isFirstFalse = true;
    public GameObject panel = null;

    // Start is called before the first frame update
    void Start()
    {
        if (panel != null && isFirstFalse) panel.SetActive(false);
        else if (panel != null && !isFirstFalse) panel.SetActive(true);
    }

    // panelをアクティブにする
    public void ActivePanel()
    {
        if (panel != null) panel.SetActive(true);
    }

    public void ExitPanel()
    {
        if (panel != null) panel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
