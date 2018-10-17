using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Flame_Crash : MonoBehaviour {
	
	float m_FlameLifeTime = 1.2f;
	float m_curr_time = 0.0f;
    bool m_is_adventure;

    // ParticleSystem m_ParticleSystem;
    /*
    void Start()
    {
        m_ParticleSystem = GetComponentInChildren<ParticleSystem>();
    }
    */

    void Update () 
	{
        if (m_curr_time < m_FlameLifeTime)
            m_curr_time += Time.deltaTime;

        else
        {
            if(SceneManager.GetActiveScene().buildIndex==3)
                Destroy(gameObject); // 화염 소멸
            if (SceneManager.GetActiveScene().buildIndex == 12)
            {
                m_curr_time = 0.0f;
                gameObject.SetActive(false);

            }
        }
	}

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (SceneManager.GetActiveScene().buildIndex == 3)
            {
                other.gameObject.GetComponent<Player>().Set_Dead();
            }
            if (SceneManager.GetActiveScene().buildIndex == 12)
            {
                Turtle_Move_Coop.instance.alive = 0;
                NetManager_Coop.instance.SetMyPos(transform.position.x, transform.rotation.y, transform.position.z);

            }
        }

    }
}
