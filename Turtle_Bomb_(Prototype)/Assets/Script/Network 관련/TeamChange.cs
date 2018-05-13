using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TeamChange : MonoBehaviour {

    public Material[] team_material;
    //public GameObject[] team_turtle;
    Renderer rend;
	// Use this for initialization
	void Start () {
        rend = GetComponent<Renderer>();
        
        rend.sharedMaterial = team_material[0];
	}
	
	// Update is called once per frame
	void Update () {
        rend.sharedMaterial = team_material[VariableManager.instance.myteam];
	}
}
