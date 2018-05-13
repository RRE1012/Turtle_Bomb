using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flame_Crash : MonoBehaviour {
	
	float m_FlameLifeTime = 1.4f;
	float m_curr_time = 0.0f;
	
	void Update () 
	{
		if (m_curr_time < m_FlameLifeTime) 
			m_curr_time += Time.deltaTime;

		else
			Destroy(gameObject); // 화염 소멸
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag ("Player"))
			PlayerMove.C_PM.Set_Dead();
	}

}
