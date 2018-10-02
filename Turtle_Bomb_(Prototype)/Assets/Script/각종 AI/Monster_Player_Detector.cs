using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Player_Detector : MonoBehaviour {

    public bool m_isInRange = false;

    Transform m_MainBody;

    GameObject m_Target;

    void Start()
    {
        m_MainBody = transform.parent;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!other.gameObject.GetComponent<Player>().Get_is_Hiden())
            {
                m_isInRange = true;

                m_Target = other.gameObject;
            }
            else
            {
                m_isInRange = false;
                m_Target = null;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            m_isInRange = false;
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
