using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {
    private Collider BCollider;
    

    private float bombCountDown = 3.0f;

    void Update()
    {
        if (bombCountDown >= 0.0f)
        {
            bombCountDown -= Time.deltaTime;

            //Debug.Log("폭탄 시간: " + bombCountDown);
        }

        else
        {

            Destroy(gameObject);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (BCollider.isTrigger && other.tag == "Player")
        {
            BCollider.isTrigger = false;
        }
    }
}
