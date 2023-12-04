using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTileCollision : MonoBehaviour
{
    
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name != "second" && collision.gameObject.name != "Base" && enabled == true)
        {
            collision.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            enabled = false;
        }
    }
}
