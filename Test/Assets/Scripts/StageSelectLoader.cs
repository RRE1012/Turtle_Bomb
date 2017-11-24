using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectLoader : MonoBehaviour {

    public static StageSelectLoader instance;

    int stage_Select_Num;
    void Awake()
    {
        if (null!= instance)
        {
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public static StageSelectLoader GetInstance()
    {
        if (!instance)
        {
            //instance = GameObject.FindObjectOfType(typeof(StageSelectLoader));
            if (!instance)
                Debug.LogError("There needs to be one active MyClass script on a GameObject in your scene.");
        }

        return instance;
    }


    
	// Use this for initialization
	void Start () {
	    	
	}
	public void StageSelect(int i)
    {
        stage_Select_Num = i;
    }
    public int GetStage_Num()
    {
        return stage_Select_Num;
    }
	// Update is called once per frame
	void Update () {
		
	}
}
