using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box_Particle_Manager : MonoBehaviour {
    
	void Update ()
    {
        if (gameObject.GetComponentInChildren<ParticleSystem>().time >= gameObject.GetComponentInChildren<ParticleSystem>().main.duration)
            Destroy(gameObject);
	}
}
