using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Player_Detector : MonoBehaviour {

    public bool m_isInRange = false;
    public bool m_isRotated = false;

    Transform m_MainBody;

    void Start()
    {
        m_MainBody = transform.parent;
    }
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            m_isInRange = true;

            Vector3 dir = other.transform.position - m_MainBody.transform.position;
            Vector3 dirXZ = new Vector3(dir.x, 0f, dir.z);

            if (dirXZ != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(dirXZ);

                m_MainBody.transform.rotation = targetRot;
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


}
