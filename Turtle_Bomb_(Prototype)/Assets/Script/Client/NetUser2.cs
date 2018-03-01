using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;
public class NetUser2 : MonoBehaviour
{
    public static NetUser2 instance;
    public Text m_TurtleTXT;
    public TextMesh m_TM;
    public int id;
    void Awake()
    {
        instance = this;
    }
    // Use this for initialization
    void Start()
    {
        if (Turtle_Move.instance.GetId() != 1)
            StartCoroutine("NetworkCheck",2.0f);

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
    // Update is called once per frame
    void Update()
    {
        //m_TurtleTXT.text = "X:" + transform.position.x+"\nZ:"+ transform.position.z;
        //m_TM.text = "X:" + transform.position.x;
    }
    IEnumerator NetworkCheck()
    {
        for (; ; )
        {

            SetPos(NetworkTest2.instance.GetNetPosx(1), NetworkTest2.instance.GetNetRoty(1), NetworkTest2.instance.GetNetPosz(1));

            yield return new WaitForSeconds(0.05f);
        }
    }
}
