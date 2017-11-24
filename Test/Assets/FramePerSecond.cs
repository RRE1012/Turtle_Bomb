using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FramePerSecond : MonoBehaviour {
    float deltaTime = 0.0f;
    
    int stage_select;
    // Use this for initialization
    void Start () {

        //instance = GetComponent<StageSelectLoader>();
        if(null!=StageSelectLoader.instance)
            stage_select = StageSelectLoader.instance.GetStage_Num();
    }
	
	// Update is called once per frame
	void Update () {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

    }
    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        Rect rect2 = new Rect(0, h * 10 / 100, w, h * 20 / 100);

        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        string text2 = string.Format("Stage{0:0.}", stage_select);

        GUI.Label(rect, text, style);
        //GUI.Label(rect, text2, style);

    }
}
