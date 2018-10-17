using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class BoxCollider_z : MonoBehaviour {

    //public static BoxCollider_z instance;
    BoxCollider m_box_col;
    // Use this for initialization
    void Start()
    {
        m_box_col = GetComponent<BoxCollider>();

    }


    public void ChangeBoxColZ(int down, int up)
    {
        m_box_col = GetComponent<BoxCollider>();
        float centerz = ((float)((up - down) / 2.0f)) * 2;
        int sizez = (down + up + 1) * 2;
        m_box_col.center = new Vector3(0, centerz, -2);
        m_box_col.size = new Vector3(2, sizez, 4);
    }
    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter(Collider other)
    {
        if (gameObject.activeInHierarchy)
        {
            if (!Camera_Directing_Net.GetInstance().ani_is_working)
            {
                if (other.gameObject.CompareTag("Player"))
                {
                    if (SceneManager.GetActiveScene().buildIndex == 7 || SceneChange.instance.GetSceneState() == 13)
                    {
                        if (Turtle_Move.instance.alive != 0)
                        {
                            //Debug.Log("TTaGawa");
                            if (Turtle_Move.instance.glider_on)
                            {
                                Turtle_Move.instance.alive = 1;
                                Turtle_Move.instance.glider_on = false;
                                Turtle_Move.instance.overpower = true;
                            }
                            else
                            {
                                if (!Turtle_Move.instance.overpower)
                                    Turtle_Move.instance.alive = 0;

                            }
                            NetTest.instance.SetmoveTrue();
                        }
                    }
                    else
                    {
                        Turtle_Move_Coop.instance.alive = 0;
                        NetManager_Coop.instance.SetMyPos(transform.position.x, transform.rotation.y, transform.position.z);
                    }
                }
            }
        }
    }
}
