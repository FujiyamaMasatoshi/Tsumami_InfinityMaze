using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayPlayerController : MonoBehaviour
{
    public GameObject playerControllerPanel = null;

    private void Start()
    {
        playerControllerPanel.SetActive(false);
    }



    public void displayPlayerController()
    {
        playerControllerPanel.SetActive(true);
    }


    public void DeletePopupDisplay()
    {
        playerControllerPanel.SetActive(false);
    }


}
