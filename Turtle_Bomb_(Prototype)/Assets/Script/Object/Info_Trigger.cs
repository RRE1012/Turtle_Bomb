using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Info_Trigger : MonoBehaviour {

    public int m_Script_Num;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Script_Box.c_Script_Box.Set_TextList(CSV_Manager.GetInstance().Get_Script(m_Script_Num)); // 대사 받아오기
            Script_Box.c_Script_Box.gameObject.SetActive(true); // 박스 활성화

            // 트리거 제거
            GameObject[] triggers = GameObject.FindGameObjectsWithTag(gameObject.tag);
            foreach (GameObject t in triggers)
                Destroy(t);
        }
    }
}
