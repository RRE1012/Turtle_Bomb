using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump_Checker : MonoBehaviour
{
    Bomb_Remaster m_Parent;

    void Awake()
    {
        m_Parent = transform.parent.GetComponent<Bomb_Remaster>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Box") ||
           other.gameObject.CompareTag("Rock") ||
           (other.gameObject.CompareTag("Bomb") && other.gameObject != m_Parent.gameObject) ||
           other.gameObject.CompareTag("icicle_Body")
           )
        {
            if (m_Parent.transform.position.y >= other.transform.position.y + 0.3f)
                m_Parent.Set_Rising(THROWN_BOMB_VALUES.RERISING_MAX_Y);
            else m_Parent.Set_Bomb();
        }

        if (other.gameObject.CompareTag("Land")) m_Parent.Set_Bomb();


        if (other.gameObject.CompareTag("Wall"))
        {
            if (m_Parent.transform.position.y >= other.transform.position.y + 0.3f)
                m_Parent.Return_To_Pool();
            else m_Parent.Set_Bomb();
        }

    }

    /*
    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Box") ||
            collision.gameObject.CompareTag("Rock") ||
            collision.gameObject.CompareTag("Bomb")
            )
        {
            if (m_Parent.transform.position.y >= collision.transform.position.y + 0.5f)
                m_Parent.Set_Rising();
        }

        if (collision.gameObject.CompareTag("Land")) m_Parent.Set_Bomb();

        
        if (collision.gameObject.CompareTag("Wall")) m_Parent.Return_To_Pool();
    }
    */
}
