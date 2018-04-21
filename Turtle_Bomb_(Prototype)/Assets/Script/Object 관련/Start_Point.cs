using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Start_Point : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        PlayerMove.C_PM.Player_Set_Start_Point(transform.position);
        Destroy(gameObject);
    }
}
