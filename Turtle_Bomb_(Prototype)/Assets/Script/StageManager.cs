using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
public class StageManager : MonoBehaviour {
    public Text m_Text;
	
	// Update is called once per frame
	void Update () {
        if (!PlayerMove.C_PM.Get_IsAlive())
        {
            m_Text.text = "Game Over";
        }
	}
}
