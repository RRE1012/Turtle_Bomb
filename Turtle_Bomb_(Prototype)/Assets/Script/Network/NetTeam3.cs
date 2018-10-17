using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetTeam3 : MonoBehaviour {

    public Material[] team_material;
    //public GameObject[] team_turtle;
    Renderer rend;
    // Use this for initialization
    void Start()
    {
        rend = GetComponent<Renderer>();
        //rend.sharedMaterial = team_material[VariableManager.instance.team_Turtle[2]];
        rend.sharedMaterial = team_material[VariableManager.instance.roominfo[VariableManager.instance.m_roomid - 1].team3];
    }

}
