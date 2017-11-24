using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManagerR : MonoBehaviour {
    public AudioSource source;
    public AudioClip a;
    public AudioClip click;
    
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Asound()
    {
        source.PlayOneShot(a);
    }
    public void clickSound()
    {
        source.PlayOneShot(click,0.4f);
    }
}
