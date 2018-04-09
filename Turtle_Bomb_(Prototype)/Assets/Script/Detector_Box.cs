using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector_Box : MonoBehaviour {

    public bool m_isInRange = false;

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
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
