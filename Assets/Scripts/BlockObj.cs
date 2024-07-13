using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockObj : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.collider.CompareTag("lava"))
    //    {
    //        Destroy(this.gameObject);
    //    }
        
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("lava"))
        {
            Destroy(this.gameObject);
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("lava"))
        {
            Destroy(this.gameObject);
        }
    }

}
