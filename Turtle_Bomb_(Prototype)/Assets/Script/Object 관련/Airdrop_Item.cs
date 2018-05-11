using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airdrop_Item : MonoBehaviour {

    float m_Move_Speed = 0.3f;
    float m_Rotate_Speed = 50.0f;
    bool m_Rising = false;
    float m_Rising_Speed = 0.5f;

    void Update()
    {
        if (!StageManager.c_Stage_Manager.Get_is_Pause())
        {
            Rising();
            floating();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Flame"))
            Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Land"))
        {
            gameObject.GetComponent<Rigidbody>().useGravity = false;
            gameObject.GetComponent<BoxCollider>().isTrigger = true;
            m_Rising = true;
        }
    }

    void Rising()
    {
        if (m_Rising)
        {
            transform.Translate(new Vector3(0.0f, m_Rising_Speed * Time.deltaTime, 0.0f));

            if (transform.position.y > 1.0f)
            {
                transform.Translate(new Vector3(0.0f, -m_Rising_Speed * Time.deltaTime, 0.0f));
                m_Rising = false;
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
