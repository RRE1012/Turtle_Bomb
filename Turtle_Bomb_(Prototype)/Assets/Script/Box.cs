using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour {

    private Collider BoxCollider;

    // Use this for initialization
    public GameObject[] m_Object_temp;
    public GameObject m_Object_FireItem;
    public GameObject m_Object_BombItem;
    public GameObject m_Object_SpeedItem;
    public GameObject m_Object_Player;

	
	// Update is called once per frame
	void Update () {
        m_Object_temp = GameObject.FindGameObjectsWithTag("Fire");
        foreach (GameObject box in m_Object_temp)
        {
            if (transform.position.x == box.transform.position.x&& transform.position.z == box.transform.position.z)
            {
                // 박스 파괴
                Destroy(gameObject);

                // 파괴 후 아이템 생성
                if (m_Object_Player != null)
                {
                    if (Random.Range(0.0f, 10.0f) > 5.5f)
                    {

                        float temp = Random.Range(0.0f, 10.0f);
                        if (temp < 3.3f)
                        {

                            m_Object_FireItem.transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
                            GameObject Instance_Item = Instantiate(m_Object_FireItem);
                        }
                        else if (temp > 6.6f)
                        {
                            m_Object_BombItem.transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
                            GameObject Instance_Item = Instantiate(m_Object_BombItem);
                        }
                        else
                        {
                            m_Object_SpeedItem.transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
                            GameObject Instance_Item = Instantiate(m_Object_SpeedItem);
                        }

                    }
                }
            }
        }
        
	}
    private void OnCollisionEnter(Collision collision)
    {
       
          
    }
    private void OnTriggerOn(Collider other)
    {
        //불꽃과 충돌 시 파괴
        if (BoxCollider.isTrigger && other.tag == "Fire")
        {
            Destroy(gameObject);
        }
    }
}
