using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flame_Crash_Portal : MonoBehaviour {

	float m_FlameSpawnTime = 2.0f;
	float m_curr_time = 0.0f;
	public GameObject m_Flame_Crash;

	void Update () 
	{
		if (m_curr_time < m_FlameSpawnTime) 
			m_curr_time += Time.deltaTime;

		else
		{
			Vector3 pos = transform.position;
			pos.y = m_Flame_Crash.transform.position.y;
			Instantiate (m_Flame_Crash).transform.position = pos; // 화염생성.

			Destroy(gameObject); // 나는 사라진다.
		}
	}
}
