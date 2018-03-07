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
    void OnTriggerEnter(Collider other)
    {
        if (!m_isRotated && other.gameObject.CompareTag("Player"))
        {
            m_isInRange = true;

            var heading = other.transform.position - m_MainBody.transform.position;
            var distance = heading.magnitude;
            var direction = heading / distance;

            float Dot = Vector3.Dot(m_MainBody.transform.forward, direction);

            float Angle = Mathf.Acos(Dot);
            Angle *= Mathf.Rad2Deg;

            m_MainBody.transform.Rotate(m_MainBody.transform.up, Angle);

            float AngleY = 0.0f;
            if (m_MainBody.transform.localEulerAngles.y >= 315.0f && m_MainBody.transform.localEulerAngles.y < 45.0f)
                AngleY = 0.0f;
            else if (m_MainBody.transform.localEulerAngles.y >= 45.0f && m_MainBody.transform.localEulerAngles.y < 135.0f)
                AngleY = 90.0f;
            else if (m_MainBody.transform.localEulerAngles.y >= 135.0f && m_MainBody.transform.localEulerAngles.y < 225.0f)
                AngleY = 180.0f;
            else if (m_MainBody.transform.localEulerAngles.y >= 225.0f && m_MainBody.transform.localEulerAngles.y < 315.0f)
                AngleY = 270.0f;
            m_MainBody.transform.localEulerAngles = new Vector3(0.0f, AngleY, 0.0f);

            m_isRotated = true;
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
