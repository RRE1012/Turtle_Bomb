using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitRoom : MonoBehaviour {
    int page = 1; //현재 내가 몇페이지에 있는지.
    public static WaitRoom instance;
    byte m_id;
    public GameObject forceouted;
    public byte[] people_inRoom = new byte[4];
    public Text m_text;
    Touch touch2;
    public GameObject pop;
    public GameObject[] uiBox;
    byte[] roomIDarray = new byte[20]; //0이면 안만들어짐 
   
    List<TB_Room> room_List = new List<TB_Room>();
    public Text[] text_array;
    // Use this for initialization
    private void Awake()
    {
        instance = this;
    }
    void Start () {
        Debug.Log("Started");
        StartCoroutine("RoomCheck");
    }
   
    public void PopMenu()
    {
        
        pop.transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
       
        if (pop.activeInHierarchy)
        {
            
        }

        pop.SetActive(true);

    }
    public void CancelPopMenu()
    {
        //Debug.Log("Clicked");
        pop.SetActive(false);
    }
    
   
    public void ToLeftRoomPage()
    {
        Debug.Log("Left Page");
        if (page > 1)
        {
            page -= 1;
            
        }
        if (page <= 1)
        {
            page = 1;
        }
    }
    public void ToRightRoomPage()
    {
        if (page < 3)
        {
            page += 1;

        }
        if (page >= 3)
        {
            page = 3;
        }
    }
    
    public void SetRoomNum()
    {

        VariableManager.instance.m_roomid = roomIDarray[0+((page-1)*8)];
        //Debug.Log(m_roomid);
    }
    public void SetRoomNum1()
    {
        VariableManager.instance.m_roomid = roomIDarray[1 + ((page - 1) * 8)];
        //Debug.Log(m_roomid);
    }
    public void SetRoomNum2()
    {
        VariableManager.instance.m_roomid = roomIDarray[2 + ((page-1) * 8)];
    }
    public void SetRoomNum3()
    {
        VariableManager.instance.m_roomid = roomIDarray[3 + ((page-1) * 8)];
    }
    public void SetRoomNum4()
    {
        VariableManager.instance.m_roomid = roomIDarray[4 + ((page-1) * 8)];
    }
    public void SetRoomNum5()
    {
        VariableManager.instance.m_roomid = roomIDarray[5 + ((page-1) * 8)];
    }
    public void SetRoomNum6()
    {
        VariableManager.instance.m_roomid = roomIDarray[6 + ((page-1) * 8)];
    }
    public void SetRoomNum7()
    {
        VariableManager.instance.m_roomid = roomIDarray[7 + ((page-1) * 8)];
    }
    
    public void SendJoinRoom()
    {
        NetTest.instance.SendJoinPacket();
    }

    public void SendCreateRoom()
    {
        NetTest.instance.SendCreatePacket();
    }




    // Update is called once per frame
    void Update () {
        if(m_text!=null)
            m_text.text = "" + VariableManager.instance.m_roomid;
        //m_text.text = "Hi";
        for (var i = 0; i < Input.touchCount; ++i)
        {
            Touch touch = Input.GetTouch(i);
            Debug.Log("Hi3");
            
                // Need to put .x
                //if (touch.position.x > (Screen.width / 2))
                //{
                    Vector2 touchDeltaPosition = Input.GetTouch(i).deltaPosition;
                    pop.transform.position = new Vector2(touchDeltaPosition.x,touchDeltaPosition.y);
                //}
            
            m_text.text = "Touch Position : " + touch.position;
        }
        if (Input.touchCount > 0)
        {
            Debug.Log("Hi");
            touch2 = Input.GetTouch(0);
            m_text.text = "Touch Position : " + touch2.position;
        }
        Vector3 nv= Input.mousePosition;
        if (VariableManager.instance.forceout)
        {
            forceouted.SetActive(true);
            VariableManager.instance.forceout = false;
        }
    }

    IEnumerator RoomCheck()
    {
        for(; ; )
        {
            room_List.Clear();
            
            for (int i = 0; i < 20; ++i)
            {
                
                if (VariableManager.instance.roominfo[i].made == 1 || VariableManager.instance.roominfo[i].made == 2)
                {
                    TB_Room temp = VariableManager.instance.roominfo[i];
                    room_List.Add(temp);
                }
            }
            int current_room = 0;
            foreach(TB_Room t in room_List)
            {
                if (current_room < 8*page)
                {
                    if (text_array[current_room] != null)
                    {
                        text_array[current_room].text = "Room No." + t.roomID + "\nRoom Max : " + t.people_max;
                        roomIDarray[current_room] = t.roomID;
                        current_room++;
                    }
                }
                
            }
            for(int i= current_room;i<page*8;++i)
            {
                if (text_array[i] != null)
                    text_array[i].text = "Empty";
            }
            //for(int i = current_room; i < 8 * page; ++i)
            //{
            //    roomIDarray[current_room] = 0;
            //}
            
            yield return new WaitForSeconds(0.2f);
        }


    }
}
