using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Front_Collider : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bomb"))
        {
            transform.parent.GetComponent<PlayerMove>().m_isAbleToKick = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Bomb"))
        {
            transform.parent.GetComponent<PlayerMove>().m_isAbleToKick = false;
        }
    }
}
