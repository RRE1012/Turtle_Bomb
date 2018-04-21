using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingEffectCollider : MonoBehaviour {

    public bool m_isInRange = false;

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            m_isInRange = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            m_isInRange = false;
        }
    }
}
