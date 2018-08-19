using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box_None_Item : Box {

    void OnTriggerEnter(Collider other)
    {
        if (!m_is_Destroyed && other.gameObject.CompareTag("Flame_Remains"))
        {
            m_is_Destroyed = true;

            // MCL 갱신
            index = StageManager.GetInstance().Find_Own_MCL_Index(transform.position.x, transform.position.z);
            StageManager.GetInstance().Update_MCL_isBlocked(index, false);
            
            // 박스 파괴
            Destroy(gameObject);
        }
    }
}
