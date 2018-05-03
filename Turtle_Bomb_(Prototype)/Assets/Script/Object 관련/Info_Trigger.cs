using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Info_Trigger : MonoBehaviour {
    
	void Start ()
    {
		

	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (gameObject.tag == "Info_Trigger_Bomb")
            {
                // 대사 띄우기

            }
            else if (gameObject.tag == "Info_Trigger_Speed")
            {
                // 대사 띄우기

            }
            else if (gameObject.tag == "Info_Trigger_Fire")
            {
                // 대사 띄우기

            }
            else if (gameObject.tag == "Info_Trigger_Kick")
            {
                // 대사 띄우기

            }
            else if (gameObject.tag == "Info_Trigger_Throw")
            {
                // 대사 띄우기

            }

            // 트리거 제거
            GameObject[] triggers = GameObject.FindGameObjectsWithTag(gameObject.tag);
            foreach (GameObject t in triggers)
                Destroy(t);
        }
    }
}
