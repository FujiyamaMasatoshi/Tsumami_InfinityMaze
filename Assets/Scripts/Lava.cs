using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    //if (other.CompareTag("block"))
    //    //{
    //    //    Debug.Log(other.name);
    //    //    Destroy(other);
    //    //}

    //    if (other.CompareTag("Player"))
    //    {
    //        Debug.Log("game over");
    //    }
    //    //else
    //    //{
    //    //    Debug.Log(other.name);
    //    //    Destroy(other);
    //    //}
    //}


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("game over");
        }
        //else
        //{
        //    Debug.Log(collision.gameObject.name);
        //    Destroy(collision.gameObject);
        //}
    }

}
