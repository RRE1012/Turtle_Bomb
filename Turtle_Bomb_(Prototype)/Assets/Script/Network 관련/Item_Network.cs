using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Network : MonoBehaviour {

    // Use this for initialization
    float m_Move_Speed = 0.3f;
    float m_Rotate_Speed = 50.0f;

    // Update is called once per frame
    void Update()
    {
        floating();
    }

    void OnTriggerEnter(Collider other)
    {
        byte item_type = 0;
        if (gameObject.activeInHierarchy)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                
                // 아이템 먹었다고 패킷을 보낸다
                // 아이템 포지션 값을 보낸다.
                // 아이템 종류를 보낸다.
                int x = (int)transform.position.x;
                int z = (int)transform.position.z;
                if (gameObject.CompareTag("Bomb_Item"))
                {
                    //Debug.Log("Get Bomb Item");
                    item_type = 0;
                }
                if (gameObject.CompareTag("Fire_Item"))
                {
                    //Debug.Log("Get Fire Item");
                    item_type = 1;
                }
                if (gameObject.CompareTag("Speed_Item"))
                {
                    //Debug.Log("Get Speed Item");
                    item_type = 2;
                }
                if (gameObject.CompareTag("Kick_Item"))
                {
                    item_type = 3;
                }
                if (gameObject.CompareTag("Throw_Item"))
                {
                    item_type = 4;
                }

                //Debug.Log("Send Item Log");
                NetTest.instance.SendItemPacket(x, z, item_type);
                gameObject.SetActive(false);
            }
            
        }
    }

    void floating()
    {
        if (this.transform.position.y > 1.0f || this.transform.position.y < 0.6f)
            m_Move_Speed *= -1;
        this.transform.Translate(new Vector3(0.0f, m_Move_Speed * Time.deltaTime, 0.0f));
        transform.Rotate(Vector3.up * m_Rotate_Speed * Time.deltaTime);
    }
}
