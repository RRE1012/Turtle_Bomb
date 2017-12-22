using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour {
    
    // Use this for initialization
    public GameObject[] m_Object_temp;
    public GameObject m_Object_FireItem;
    public GameObject m_Object_BombItem;
    public GameObject m_Object_SpeedItem;
    public GameObject m_Object_Player;
    

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("파괴");
        print("Destroy");
        if (other.gameObject.CompareTag("Flame_Remains"))
        {
            Debug.Log("파괴");
            print("Destroy");
            // 화염 잔해 파괴
            Destroy(other.gameObject);
            // 박스 파괴
            Destroy(gameObject);
            // 파괴 후 아이템 생성
            SetItem();
        }
    }

    void SetItem()
    {
        if (m_Object_Player != null)
        {
            if (Random.Range(0.0f, 10.0f) > 5.5f)
            {

                float temp = Random.Range(0.0f, 10.0f);
                if (temp < 3.3f)
                {
                    m_Object_FireItem.transform.position = new Vector3(transform.position.x, 0.6f, transform.position.z);
                    GameObject Instance_Item = Instantiate(m_Object_FireItem);
                }
                else if (temp > 6.6f)
                {
                    m_Object_BombItem.transform.position = new Vector3(transform.position.x, 0.6f, transform.position.z);
                    GameObject Instance_Item = Instantiate(m_Object_BombItem);
                }
                else
                {
                    m_Object_SpeedItem.transform.position = new Vector3(transform.position.x, 0.6f, transform.position.z);
                    GameObject Instance_Item = Instantiate(m_Object_SpeedItem);
                }
            }
        }
    }
}
