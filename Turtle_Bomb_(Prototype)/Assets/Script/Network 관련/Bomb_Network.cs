using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb_Network : MonoBehaviour {

    public GameObject m_flame_effect;
    byte firepower;
    Animation m_Animations;
    
    //방에서 포인트 설정 
    void Awake()
    {
        m_Animations = GetComponent<Animation>();

    }
	// Use this for initialization
	void Start () {
       
        m_Animations.Play("Bomb_Pumpin");
       
    }
	
	// Update is called once per frame
	void Update () {
        m_Animations.PlayQueued("Bomb_Pumpin");
    }

    void ReLoad()
    {
        this.gameObject.SetActive(true);

    }
    void Explode(byte fire_power)
    {
        GameObject Instance_FlameDir_N;
        GameObject Instance_FlameDir_S;
        GameObject Instance_FlameDir_W;
        GameObject Instance_FlameDir_E;
        for (byte i = 0; i < firepower; ++i)
        {
            Instance_FlameDir_N = Instantiate(m_flame_effect);
            Instance_FlameDir_N.transform.position = new Vector3(gameObject.transform.position.x, 0.0f, gameObject.transform.position.z + (2.0f * (i + 1)));
            Instance_FlameDir_S = Instantiate(m_flame_effect);
            Instance_FlameDir_S.transform.position = new Vector3(gameObject.transform.position.x, 0.0f, gameObject.transform.position.z - (2.0f * (i + 1)));
            Instance_FlameDir_W = Instantiate(m_flame_effect);
            Instance_FlameDir_W.transform.position = new Vector3(gameObject.transform.position.x - (2.0f * (i + 1)), 0.0f, gameObject.transform.position.z);
            Instance_FlameDir_E = Instantiate(m_flame_effect);
            Instance_FlameDir_E.transform.position = new Vector3(gameObject.transform.position.x + (2.0f * (i + 1)), 0.0f, gameObject.transform.position.z);
            
        }
        this.gameObject.SetActive(false);
    }

}
