using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;
public class NetUser3 : MonoBehaviour
{
    public static NetUser3 instance;
    public Text m_TurtleTXT;
    public TextMesh m_TM;
    public GameObject p;
    float m_GliderfloatSpeed = 1.0f;
    bool textmesh_On = false;
    byte id = 254;
    byte color = 0;
    bool dead_ani = false;
    bool throw_ani = false;
    bool push_ani = false;
    bool walk_ani = false;
    bool kick_ani = false;
    bool get_glider = false;
    bool glider_on = false;
    public GameObject glider;
    public GameObject parts;
    public Material[] icon_material;
    public GameObject m_plane;
    public Renderer m_Rplane;
    Animator m_animator;
    void Awake()
    {
        instance = this;
        transform.position = new Vector3(0.0f, transform.position.y, 28.0f);
    }
    // Use this for initialization
    void Start()
    {
        transform.position = new Vector3(0.0f, transform.position.y, 28.0f);
        dead_ani = false;
        //p = GetComponent<GameObject>();
        //Invoke("IDCheck", 2.0f);
        m_animator = GetComponent<Animator>();

        throw_ani = false;
        walk_ani = false;
        push_ani = false;
        kick_ani = false;
        get_glider = false;
        glider_on = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bush"))
        {
            parts.SetActive(false);
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Bush"))
        {
            parts.SetActive(false);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bush"))
        {
            parts.SetActive(true);
        }
        if (other.CompareTag("Bomb"))
        {
            other.isTrigger = false;
        }
    }
    public void GetGlider()
    {
        get_glider = true;
    }
    void SetFalse()
    {
        gameObject.SetActive(false);
    }
    public void SetKickMotion()
    {
        kick_ani = true;
    }
    public void SetMoveMotion()
    {
        walk_ani = true;
    }
    public void SetDeadMotion()
    {
        dead_ani = true;
    }
    public void SetThrowMotion()
    {
        throw_ani = true;
    }
    public void SetPushMotion()
    {
        push_ani = true;
    }
    void Push_Ani_False()
    {
        m_animator.SetBool("TurtleMan_isPush", false);
    }
    void Throw_Ani_False()
    {
        m_animator.SetBool("TurtleMan_isThrow", false);
    }
    void Walk_Ani_False()
    {
        m_animator.SetBool("TurtleMan_isWalk", false);
        m_animator.SetBool("TurtleMan_GliderMove", false);
    }
    void Kick_Ani_False()
    {
        m_animator.SetBool("TurtleMan_isKick", false);
    }
    void Glider_False()
    {
        m_animator.SetBool("TurtleMan_GetGlider", false);
    }
    void IDCheck()
    {
        if (p != null)
        {
            if (Turtle_Move.instance.GetId() == 2 || Turtle_Move.instance.GetId() > 3)
            {

            }
            // else
            //  StartCoroutine("NetworkCheck");
        }
    }
    public void SetText(byte itemtype)
    {
        textmesh_On = true;

        switch (itemtype)
        {
            case 11:
                color = 0;
                break;
            case 12:
                color = 2;
                break;
            case 13:
                color = 1;
                break;
            case 14:
                color = 3;
                break;
            case 15:
                color = 4;
                break;
        }
        color = itemtype;

    }
    public void SetTextOff()
    {
        m_TM.gameObject.SetActive(false);
    }
    public void SetPos(float x, float y, float z)
    {
        float tempx = x - transform.position.x;
        float tempz = z - transform.position.z;
        float tempy = y - transform.localEulerAngles.y;


        //transform.position = new Vector3(transform.position.x+(tempx/4), transform.position.y, transform.position.z+(tempz/4));
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(x, transform.position.y, z), 0.5f);
        transform.eulerAngles = new Vector3(0, y, 0);
        //Thread.Sleep(125);
        //new WaitForSeconds(0.125f);
        //yield WaitForSeconds(0.125f);

    }
    public void SetIconOff()
    {
        m_plane.gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (NetTest.instance.GetNetAlive(2) == 2)
        {
            glider_on = true;
            m_animator.SetBool("TurtleMan_GetGlider", true);

        }
        else if (NetTest.instance.GetNetAlive(2) == 1)
        {
            if (glider_on)
            {
                m_animator.SetBool("TurtleMan_GetGlider", false);
                glider_on = false;

            }
        }
        if (kick_ani)
        {
            m_animator.SetBool("TurtleMan_isKick", true);
            Invoke("Kick_Ani_False", 1.0f);
            kick_ani = false;
        }
        if (push_ani)
        {
            m_animator.SetBool("TurtleMan_isPush", true);
            Invoke("Push_Ani_False", 1.0f);
            push_ani = false;
        }
        if (walk_ani)
        {
            if (get_glider)
            {
                m_animator.SetBool("TurtleMan_GliderMove", true);
            }
            else
                m_animator.SetBool("TurtleMan_isWalk", true);
            Invoke("Walk_Ani_False", 1.0f);
            walk_ani = false;
        }
        //m_TM.transform.position = new Vector3(gameObject.transform.position.x, m_TM.transform.position.y, gameObject.transform.position.z);
        if (dead_ani)
        {
            m_TM.gameObject.SetActive(true);

            m_TM.text = "1P - Dead!!!";
            m_animator.SetBool("TurtleMan_isDead", true);
            Invoke("SetTextOff", 2.0f);
            Invoke("SetFalse", 5.0f);
            dead_ani = false;
        }
        if (throw_ani)
        {
            m_animator.SetBool("TurtleMan_isThrow", true);
            Invoke("Throw_Ani_False", 1.0f);
            throw_ani = false;
        }
        if (get_glider)
        {
            m_animator.SetBool("TurtleMan_GetGlider", true);
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, 2.0f, gameObject.transform.position.z);
            get_glider = false;
            glider_on = true;
        }
        if (glider_on)
        {
            m_animator.SetBool("TurtleMan_GetGlider", true);
            glider.SetActive(true);
            if (gameObject.transform.position.y >= 3.0f)
                m_GliderfloatSpeed = -0.01f;
            else if (gameObject.transform.position.y <= 2.0f)
                m_GliderfloatSpeed = 0.01f;
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + (m_GliderfloatSpeed), gameObject.transform.position.z);
        }
        else
        {
            glider.SetActive(false);
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, -0.5f, gameObject.transform.position.z);
        }
        if (VariableManager.instance.people_inRoom[2] == 0)
            p.SetActive(false);
        else if (VariableManager.instance.pos_inRoom - 1 == 2)
        {
            p.SetActive(false);
        }
        else
        {
            p.SetActive(true);
        }
        SetPos(NetTest.instance.GetNetPosx(2), NetTest.instance.GetNetRoty(2), NetTest.instance.GetNetPosz(2));
        if (textmesh_On)
        {
            m_plane.gameObject.SetActive(true);
            switch (color)
            {
                case 0:
                    m_Rplane.sharedMaterial = icon_material[0];
                    //m_TM.color = new Color(0, 0, 1);
                    //m_TM.text = "Bomb Up~";
                    break;
                case 1:
                    m_Rplane.sharedMaterial = icon_material[1];
                    //m_TM.color = new Color(1, 0, 0);
                    //m_TM.text = "Fire Up~";
                    break;
                case 2:
                    m_Rplane.sharedMaterial = icon_material[2];
                    //m_TM.color = new Color(1, 1, 0);
                    //m_TM.text = "Speed Up~";
                    break;
                case 3:
                    m_Rplane.sharedMaterial = icon_material[3];
                    break;
                case 4:
                    m_Rplane.sharedMaterial = icon_material[4];
                    break;
                case 5:
                   
                    get_glider = true;
                    m_Rplane.sharedMaterial = icon_material[5];
                    color = 100;
                    break;
                default:
                    break;
            }
            Invoke("SetIconOff", 2.0f);
            textmesh_On = false;
        }
        //m_TurtleTXT.text = "X:" + transform.position.x+"\nZ:"+ transform.position.z;
        //m_TM.text = "X:" + transform.position.x;
    }
    IEnumerator NetworkCheck()
    {
        WaitForSeconds delay = new WaitForSeconds(0.05f);
        for (; ; )
        {
            
            SetPos(NetTest.instance.GetNetPosx(2), NetTest.instance.GetNetRoty(2), NetTest.instance.GetNetPosz(2));

            yield return delay;
        }
    }
}
