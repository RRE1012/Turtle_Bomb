using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

	float m_Move_Speed = 0.3f;
	float m_Rotate_Speed = 50.0f;

	// Update is called once per frame
	void Update () {
		floating ();
	}

	void floating()
	{
		if (this.transform.position.y > 1.0f || this.transform.position.y < 0.6f)
            m_Move_Speed *= -1;
		this.transform.Translate(new Vector3(0.0f, m_Move_Speed * Time.deltaTime, 0.0f));
		transform.Rotate (Vector3.up * m_Rotate_Speed * Time.deltaTime);
	}
}
