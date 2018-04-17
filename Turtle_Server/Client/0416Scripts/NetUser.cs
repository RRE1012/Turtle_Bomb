using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;
public class NetUser : MonoBehaviour
{
    public static NetUser instance;
    public Text m_TurtleTXT;
    public TextMesh m_TM;
    public GameObject p;
    bool textmesh_On=false;
    byte color = 0;
    byte id = 254;
    bool dead_ani = false;
    Animator m_animator;
    void Awake()
    {
        instance = this;
    }
    // Use this for initialization
    void Start()
    {
        dead_ani = false;
        //Invoke("IDCheck", 2.0f);
        m_animator = GetComponent<Animator>();
    }
    void SetFalse()
    {
        gameObject.SetActive(false);
    }
    public void SetDeadMotion()
    {
        dead_ani = true;
    }
    public void SetPos(float x, float y, float z)
    {
        float tempx = x - transform.position.x;
        float tempz = z - transform.position.z;
        float tempy = y - transform.rotation.y;

        for (int i = 0; i < 4; ++i)
        {
            //transform.position = new Vector3(transform.position.x+(tempx/4), transform.position.y, transform.position.z+(tempz/4));
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x + (tempx / 4), transform.position.y, transform.position.z + (tempz / 4)), 0.5f);
            transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y + (tempy / 4), transform.rotation.z, transform.rotation.w);
            //Thread.Sleep(125);
            //new WaitForSeconds(0.125f);
            //yield WaitForSeconds(0.125f);
        }

    }

    public void SetText(byte itemtype)
    {
        textmesh_On = true;

        color = itemtype;

    }

    public void SetTextOff()
    {
        m_TM.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (dead_ani)
        {
            m_TM.gameObject.SetActive(true);

            m_TM.text = "1P - Dead!!!";
            m_animator.SetBool("death", true);
            Invoke("SetTextOff", 2.0f);
            
            Invoke("SetFalse", 1.1f);
            dead_ani = false;
        }
        SetPos(NetTest.instance.GetNetPosx(0), NetTest.instance.GetNetRoty(0), NetTest.instance.GetNetPosz(0));
        if (VariableManager.instance.pos_inRoom-1 == 0)
        {
            p.SetActive(false);
        }
        else
        {
            p.SetActive(true);
        }
        m_TurtleTXT.text = "ID: " + Turtle_Move.instance.GetId();
        //m_TurtleTXT.text = "X:" + transform.position.x+"\nZ:"+ transform.position.z;
        //m_TM.text = "X:" + transform.position.x;
        if (textmesh_On)
        {
            m_TM.gameObject.SetActive(true);
            switch (color)
            {
                case 0:
                    m_TM.color = new Color(0, 0, 1);
                    m_TM.text = "Bomb Up~";
                    break;
                case 1:
                    m_TM.color = new Color(1, 0, 0);
                    m_TM.text = "Fire Up~";
                    break;
                case 2:
                    m_TM.color = new Color(1, 1, 0);
                    m_TM.text = "Speed Up~";
                    break;
                default:
                    break;
            }
            Invoke("SetTextOff", 2.0f);
            textmesh_On = false;
        }
    }

    void IDCheck()
    {
        if (p != null)
        {
            if (Turtle_Move.instance.GetId() == 0 || Turtle_Move.instance.GetId() > 3)
            {

            }
            else
                StartCoroutine("NetworkCheck");
        }
    }

    IEnumerator NetworkCheck()
    {
        for (; ; )
        {

            SetPos(NetTest.instance.GetNetPosx(0), NetTest.instance.GetNetRoty(0), NetTest.instance.GetNetPosz(0));

            yield return new WaitForSeconds(0.05f);
        }
    }
}
