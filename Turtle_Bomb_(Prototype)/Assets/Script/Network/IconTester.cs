using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class IconTester : MonoBehaviour {
    public RawImage[] icon;
    int a = 5;
    float color_a = 0.5f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
       
        if (!Turtle_Move.instance.can_kick)
        {
            icon[0].gameObject.SetActive(true);
        }
        else
        {
            icon[0].gameObject.SetActive(false);
        }
        if (!Turtle_Move.instance.can_throw)
        {
            icon[1].gameObject.SetActive(true);
        }
        else
            icon[1].gameObject.SetActive(false);
    }
}
