using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bush : MonoBehaviour
{

    // Use this for initialization
    public GameObject[] m_Object_temp;
    
    public GameObject m_Object_Player;
    public GameObject fire_Effect;
    GameObject Instance_Item;
    Color color =new Color(1.0f,0.0f,0.0f,1);
    bool m_is_Destroyed = false;
    MeshRenderer meshrenderer;
    public Material m;
    void Start()
    {
        
        StartCoroutine("BushCheck");
        meshrenderer = gameObject.GetComponent<MeshRenderer>();
    }

    //부쉬가 화염과 충돌하면 상태가 변경되는 함수 - 불타는 수풀이 되며, 화염이펙트가 추가된다.
    IEnumerator BushCheck()
    {
        int i = 0;
        for (;;)
        {
            m_Object_temp = GameObject.FindGameObjectsWithTag("Flame_Remains");
            foreach(GameObject flame in m_Object_temp)
            {
                if(transform.position.x==flame.transform.position.x && transform.position.z == flame.transform.position.z&&gameObject.tag=="Bush")
                {
                    GameObject Instance_Flame_Remains = Instantiate(fire_Effect);
                    Instance_Flame_Remains.transform.position = transform.position;
                    gameObject.tag = "Flame_Bush";
                    meshrenderer.material = m;
                }
            }
            if (gameObject.CompareTag("Flame_Bush"))
            {
                i = (i + 1) % 11;
                meshrenderer.material.color = new Color(0.0f + (i * 0.1f), 1.0f - (i * 0.1f), 0, 1);
            }
            yield return new WaitForSeconds(0.05f);
        }
    }
    

    
}
