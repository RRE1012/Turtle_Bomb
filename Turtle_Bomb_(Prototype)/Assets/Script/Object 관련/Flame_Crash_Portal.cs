using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class Flame_Crash_Portal : MonoBehaviour {

	float m_FlameSpawnTime = 2.0f;
	float m_curr_time = 0.0f;
    bool is_adventure;
	public GameObject m_Flame_Crash;

    void Start()
    {
        is_adventure = true;
        if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            is_adventure = true;
        }
        else if (SceneManager.GetActiveScene().buildIndex == 12)
        {
            is_adventure = false;
        }


    }
    void Update () 
	{
        if (is_adventure)
        {
            if (m_curr_time < m_FlameSpawnTime)
                m_curr_time += Time.deltaTime;

            else
            {
                Vector3 pos = transform.position;
                pos.y = m_Flame_Crash.transform.position.y;
                Instantiate(m_Flame_Crash).transform.position = pos; // 화염생성.

                Destroy(gameObject); // 나는 사라진다.
            }
        }
        else
        {
            if (m_curr_time < m_FlameSpawnTime)
            {
                m_Flame_Crash.SetActive(false);
                m_curr_time += Time.deltaTime;
            }
            else
            {
                Vector3 pos = transform.position;
                pos.y = m_Flame_Crash.transform.position.y;
                m_Flame_Crash.SetActive(true);

                m_Flame_Crash.transform.position = pos; // 화염생성.
                m_curr_time = 0.0f;
                gameObject.SetActive(false); // 나는 사라진다.
            }
        }
	}
}
