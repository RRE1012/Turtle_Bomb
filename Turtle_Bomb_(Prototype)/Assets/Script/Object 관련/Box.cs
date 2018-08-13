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

    GameObject m_PlayerCollider = null; // 플레이어의 밀기용 감지기
    GameObject m_IcicleCollider = null; // 고드름 바닥 충돌체

    public bool m_is_Destroyed = false;
    protected int index;

    void Start()
    {
        // 최초 시작 시 박스 자신의 위치의 isBlocked를 true로 갱신
        index = StageManager.GetInstance().Find_Own_MCL_Index(transform.position.x, transform.position.z);
        StageManager.GetInstance().Update_MCL_isBlocked(index, true);
    }

    void OnDestroy()
    {
        if (m_PlayerCollider) // 충돌중인 감지기가 있으면
        {
            m_PlayerCollider.GetComponent<Front_Collider>().TriggerExit_Ver2(); // 트리거아웃을 직접 수행해준다.
        }
        if (m_IcicleCollider) // 충돌중인 고드름바닥이 있으면
        {
            m_IcicleCollider.GetComponent<Icicle_Bottom>().TriggerExit_Ver2(); // 트리거아웃을 직접 수행해준다.
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (!m_is_Destroyed && other.gameObject.CompareTag("Flame_Remains"))
        {
            m_is_Destroyed = true;

            // MCL 갱신
            index = StageManager.GetInstance().Find_Own_MCL_Index(transform.position.x, transform.position.z);
            StageManager.GetInstance().Update_MCL_isBlocked(index, false);
            
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

    public void Save_Player_Front_Collider(GameObject fc)
    {
        m_PlayerCollider = fc;
    }
    public void Clear_Player_Front_Collider()
    {
        m_PlayerCollider = null;
    }

    public void Save_Icicle_Bottom_Collider(GameObject ic)
    {
        m_IcicleCollider = ic;
    }
    public void Clear_Icicle_Bottom_Collider()
    {
        m_IcicleCollider = null;
    }
}
