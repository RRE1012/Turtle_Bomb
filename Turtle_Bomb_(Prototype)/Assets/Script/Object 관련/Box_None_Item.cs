using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box_None_Item : MonoBehaviour {

    public bool m_is_Destroyed = false;
    int index;

    void Start()
    {
        // 최초 시작 시 박스 자신의 위치의 isBlocked를 true로 갱신
        index = StageManager.c_Stage_Manager.Find_Own_MCL_Index(transform.position.x, transform.position.z);
        StageManager.c_Stage_Manager.Update_MCL_isBlocked(index, true);
    }


    void OnTriggerEnter(Collider other)
    {
        if (!m_is_Destroyed && other.gameObject.CompareTag("Flame_Remains"))
        {
            m_is_Destroyed = true;

            // MCL 갱신
            index = StageManager.c_Stage_Manager.Find_Own_MCL_Index(transform.position.x, transform.position.z);
            StageManager.c_Stage_Manager.Update_MCL_isBlocked(index, false);

            // 화염 잔해 파괴
            Destroy(other.gameObject);
            // 박스 파괴
            Destroy(gameObject);
        }
    }
}
