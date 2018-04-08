using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScript : MonoBehaviour {
    public GameObject m_bomb;
    public GameObject m_effect;
	// Use this for initialization
	void Start () {
		
	}
    public void BombExplode()
    {
        m_bomb.SetActive(false);
        m_effect.SetActive(true);
    }
    // Update is called once per frame
    void Update () {
		
	}
}
