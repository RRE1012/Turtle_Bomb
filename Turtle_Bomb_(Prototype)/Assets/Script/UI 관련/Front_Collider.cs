using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Front_Collider : MonoBehaviour {
    PlayerMove m_Player;
    void Start()
    {
        m_Player = transform.parent.GetComponent<PlayerMove>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bomb"))
        {
            m_Player.m_isAbleToKick = true;
        }

        
    }

    void OnTriggerStay(Collider other)
    {
        if (!m_Player.m_isBoxSelected && other.gameObject.CompareTag("Box"))
        {
            Vector3 tmpPosition;
            tmpPosition = other.gameObject.transform.position + transform.forward * 1.2f;
            int index = StageManager.c_Stage_Manager.Find_Own_MCL_Index(tmpPosition.x, tmpPosition.z);
            if (index != -1)
            {
                if (StageManager.c_Stage_Manager.Get_MCL_index_is_Blocked(index) == false)
                {
                    m_Player.m_Front_Box = other.gameObject;
                    m_Player.m_isBoxSelected = true;
                    m_Player.m_isAbleToPush = true;
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Bomb"))
        {
            m_Player.m_isAbleToKick = false;
        }

        // 박스와 접촉 해제시 밀기 비활성화

        if (m_Player.m_isBoxSelected && other.gameObject.CompareTag("Box"))
        {
            m_Player.m_isBoxSelected = false;
            m_Player.m_isAbleToPush = false;
        }
    }
}
