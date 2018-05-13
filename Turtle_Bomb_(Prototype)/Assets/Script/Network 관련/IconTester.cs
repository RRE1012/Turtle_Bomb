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
        float tempa = (float)(a + 1)%6/10.0f; 
        if (!Turtle_Move.instance.can_kick)
        {
            icon[0].color = new Color(0.25f, 0.25f, 0.25f, 1.0f);
        }
        else
        {
            icon[0].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }
        if (!Turtle_Move.instance.can_throw)
        {
            icon[1].color = new Color(0.25f, 0.25f, 0.25f, 1.0f);
        }
        else
            icon[1].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }
}
