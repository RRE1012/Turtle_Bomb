using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake_Checker : MonoBehaviour {

    public bool m_isInRange = false;

    GameObject m_Player; public GameObject Get_Target() { return m_Player; }

    void OnTriggerEnter(Collider other)
    {
        if(!m_isInRange && other.gameObject.CompareTag("Player"))
        {
            m_isInRange = true;
            m_Player = other.gameObject;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) Set_Out_of_Range();
    }

    public void Set_Out_of_Range()
    {
        m_isInRange = false;
        m_Player = null;
    }
}
