using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box_None_Item : Box {

    void OnTriggerEnter(Collider other)
    {
        if (!m_is_Destroyed && other.gameObject.CompareTag("Flame_Remains") || other.gameObject.CompareTag("Flame"))
        {
            m_is_Destroyed = true;

            // MCL 갱신
            StageManager.GetInstance().Update_MCL_isBlocked(m_MCL_index, false);

            Instantiate(m_Particle).transform.position = transform.position; // 파티클 발생

            // 박스 파괴
            Destroy(gameObject);
        }
    }
}
