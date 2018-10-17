using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Start_Point : MonoBehaviour {
    
	void Start ()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().Player_Set_Start_Point(transform.position);
        Destroy(gameObject);
    }
}
