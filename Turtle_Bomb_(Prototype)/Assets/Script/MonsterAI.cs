using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAI : MonoBehaviour {

    int m_My_MCL_Index = 0;
    bool m_is_Find_My_Coord;
    bool m_is_Done_Camera_Walking;

    float m_Monster_Basic_Speed;
    float m_RotationX;
    float m_RotateSensX;
    bool m_isStuck;


    void Start ()
    {
        StageManager.m_Left_Monster += 1;

        m_RotationX = 0.0f;
        m_RotateSensX = 100.0f;

        m_Monster_Basic_Speed = 5.0f;
        m_isStuck = false;

        m_is_Find_My_Coord = false;
        m_is_Done_Camera_Walking = false;
        Invoke("ReadyToCameraWalking", 6.0f);
    }
	
	void Update ()
    {
        if(!m_is_Find_My_Coord)
            Find_My_Coord();

        if (m_is_Done_Camera_Walking)
        {
            MonsterMove();
        }
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Box") || collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Rock"))
        {
            transform.Rotate(new Vector3 (0.0f, 150.0f, 0.0f));
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Flame" && !StageManager.m_is_Stage_Clear)
        {
            StageManager.m_Left_Monster -= 1;
            Destroy(gameObject);
        }
    }

    void MonsterMove()
    {
        transform.Translate(new Vector3(0.0f, 0.0f, (m_Monster_Basic_Speed * Time.deltaTime)));
    }

    void ReadyToCameraWalking()
    {
        m_is_Done_Camera_Walking = true;
    }

    void Find_My_Coord()
    {
        if (StageManager.m_is_init_MCL)
        {
            m_My_MCL_Index = StageManager.Find_Own_MCL_Index(transform.position.x, transform.position.z);
            m_is_Find_My_Coord = true;
        } 
    }
}
