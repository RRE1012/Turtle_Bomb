using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Network_Airdrop : Item_Network{
    bool m_Droping = false;
    float m_Droping_Speed = 6.0f;
    // Use this for initialization
   
    void Droping()
    {
        if (m_Droping)
        {
            transform.Translate(new Vector3(0.0f, -m_Droping_Speed * Time.deltaTime, 0.0f));

            if (transform.position.y < 1.0f)
            {
                //transform.Translate(new Vector3(0.0f, m_Droping_Speed * Time.deltaTime, 0.0f));
                m_Droping = false;
            }
        }
    }
    public void IsGen()
    {
        m_Droping = true;
    }
    void OnTriggerEnter(Collider other)
    {
        byte item_type = 0;
        if (gameObject.activeInHierarchy)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                int x = (int)transform.position.x;
                int z = (int)transform.position.z;
                item_type = 6;
                NetTest.instance.SendItemPacket(x, z, item_type);
                IsGen();
                gameObject.SetActive(false);
            }
        }
    }
    // Update is called once per frame
    void Update () {
        if (!m_Droping)
            floating();
        else
            Droping();
    }
}
