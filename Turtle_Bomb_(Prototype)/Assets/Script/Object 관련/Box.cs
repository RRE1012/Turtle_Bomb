using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour {
    
    // Use this for initialization
    public GameObject m_Object_FireItem;
    public GameObject m_Object_BombItem;
    public GameObject m_Object_SpeedItem;
    public GameObject m_Object_KickItem;
    public GameObject m_Object_ThrowItem;


    public GameObject m_Particle;

    

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
            
            SetItem(); // 아이템 생성

            Instantiate(m_Particle).transform.position = transform.position; // 파티클 발생

            Destroy(other.gameObject); // 화염 잔해 파괴
            
            Destroy(gameObject); // 박스 파괴
        }
    }

    void SetItem()
    {
        if (Random.Range(0, 100) > 51)
        {
            GameObject Instance_Item;
            int dropRate = Random.Range(0, 100);
            if (dropRate < 25)
            {
                m_Object_FireItem.transform.position = new Vector3(transform.position.x, 0.6f, transform.position.z);
                Instance_Item = Instantiate(m_Object_FireItem);
            }
            else if (dropRate >= 25 && dropRate < 55)
            {
                m_Object_BombItem.transform.position = new Vector3(transform.position.x, 0.6f, transform.position.z);
                Instance_Item = Instantiate(m_Object_BombItem);
            }
            else if (dropRate >= 55 && dropRate < 80)
            {
                m_Object_SpeedItem.transform.position = new Vector3(transform.position.x, 0.6f, transform.position.z);
                Instance_Item = Instantiate(m_Object_SpeedItem);
            }
            else if (dropRate >= 80 && dropRate < 90)
            {
                m_Object_KickItem.transform.position = new Vector3(transform.position.x, 0.6f, transform.position.z);
                Instance_Item = Instantiate(m_Object_KickItem);
            }
            else
            {
                m_Object_ThrowItem.transform.position = new Vector3(transform.position.x, 0.6f, transform.position.z);
                Instance_Item = Instantiate(m_Object_ThrowItem);
            }
        }
    }
}
