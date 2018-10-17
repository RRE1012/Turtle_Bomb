using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector_Box : MonoBehaviour {

    public bool m_isInRange = false;
    GameObject m_Target;

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            m_isInRange = true;
            m_Target = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            m_isInRange = false;
            m_Target = null;
        }
    }

    public GameObject Get_Target()
    {
        if (m_Target != null)
            return m_Target;
        else return null;
    }

    public void Set_Target(GameObject t)
    {
        m_Target = t;
    }
}
