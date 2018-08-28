using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Network_Alarm : MonoBehaviour {
    public static Network_Alarm instance = null;
    // Use this for initialization
    public bool m_awake;
    void Awake()
    {
        instance = this;
    }
    void Start () {
        m_awake = false;

    }
	
	// Update is called once per frame
	void Update () {
        if (m_awake)
        {
            gameObject.SetActive(true);
            m_awake = false;
        }
	}
}
