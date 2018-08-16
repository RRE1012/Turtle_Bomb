using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
public class Matching_Room : MonoBehaviour {
    public static Matching_Room instance;
    public byte difficulty;
    public byte boss;
    public byte map;
    public byte how_many;
    public Text myinfo_text;
    public Text tier_text;

    public Text difficult_text;
    public Text nop_text;
    public GameObject matching_image;
    public GameObject matching_image_success;
    public GameObject matching_image_fail;
    public GameObject matching_image_vs_status;
    public GameObject matching_particles;
    public GameObject[] match_button;
    public RawImage[] player_Tier;
    string id_no1;
    string id_no2;
    string id_no3;
    string id_no4;
    public Text[] partnerIDtext;
    public RawImage tierimage_cur;
    public RawImage map_text;
    public GameObject MyInfo;
    public GameObject MatchingInfo;

    public Texture[] tierImage;
    public Texture[] mapimage;
    byte[] tier_num = new byte[4];
    byte[] copy_info = new byte[87];

    public byte success_Match;
    void Awake()
    {
        instance = this;
    }
    // Use this for initialization
    void Start () {
        tier_num[0] = 0;
        tier_num[1] = 0;
        tier_num[2] = 0;
        tier_num[3] = 0;

        boss = 0;
        map = 0;
        how_many = 2;
        difficulty = 0;
        success_Match = 0;
        difficult_text.text = "Easy";
        difficult_text.color = new Color(1.0f, 1.0f, 1.0f);
        nop_text.text = "2";
        nop_text.color = new Color(1.0f, 0.0f, 0.0f);
        map_text.texture = mapimage[0];
        MyInfo.SetActive(false);
        MatchingInfo.SetActive(true);
        match_button[0].SetActive(true);
        match_button[1].SetActive(false);

    }
    public string GetID(int i)
    {
        switch (i)
        {
            case 1:
                return id_no1;
                
            case 2:
                return id_no2;
                
            case 3:
                return id_no3;
                
            case 4:
                return id_no4;
               
            default:
                return null;
        }
        
    }
    public void OutMatchingRoom()
    {

        NetManager_Coop.instance.Disconnect();
        SceneChange.instance.GoTo_Select_Scene();

    }
    public void MyPage(bool open)
    {
        if (open)
        {
            MyInfo.SetActive(true);
            MatchingInfo.SetActive(false);
        }
        else
        {
            MyInfo.SetActive(false);
            MatchingInfo.SetActive(true);
        }
    }
    public void SetMatchingInfo(byte[] a)
    {

        Buffer.BlockCopy(a, 2, copy_info, 0, 87);
        success_Match = copy_info[0];

        //Debug.Log("Data Load"+success_Match);

    }

    public void MathcingCorrect(byte a)
    {
        success_Match = a;
    }
    public void SendMatching()
    {
        match_button[0].SetActive(false);
        match_button[1].SetActive(true);
        MatchingInfo.SetActive(false);
        NetManager_Coop.instance.SendMatchPacket(difficulty, boss, map, how_many);
        matching_image.SetActive(true);
    }
    public void CancelMatching()
    {
        match_button[0].SetActive(true);
        match_button[1].SetActive(false);
        MatchingInfo.SetActive(true);
        NetManager_Coop.instance.SendMatchPacket(difficulty, boss, map, 0);
        matching_image.SetActive(false);
    }
    void GoToPlay() {
        SceneChange.instance.GoTo_CoopBoss_Scene();
    }
    public void ChangeNOP(bool downup)
    {
        if (downup)
        {
            if (how_many != 4)
                how_many++;
        }
        else
        {
            if (how_many != 2)
                how_many--;
        }
        switch (how_many)
        {
            case 4:
                nop_text.text = "4";
                nop_text.color = new Color(1.0f, 1.0f, 0.0f);
                break;
            case 3:
                nop_text.text = "3";
                nop_text.color = new Color(1.0f, 0.5f, 0.0f);
                break;
            case 2:
                nop_text.text = "2";
                nop_text.color = new Color(1.0f, 0.0f, 0.0f);
                break;

        }

    }
    public void ChangeMap(bool downup)
    {
        if (downup)
        {
            if (map != 2)
                map++;
        }
        else
        {
            if (map != 0)
                map--;
        }
        switch (map)
        {
            case 0:
                map_text.texture = mapimage[0];
               
                break;
            case 1:
                map_text.texture = mapimage[1];
               
                break;
            case 2:
                map_text.texture = mapimage[2];
               
                break;

        }

    }
    public void ChangeDifficulty(bool downup)
    {
        if (downup)
        {
            if (difficulty != 2)
                difficulty++;
        }
        else
        {
            if (difficulty != 0)
                difficulty--;
        }
        switch (difficulty)
        {
            case 0:
                difficult_text.text = "Easy";
                difficult_text.color = new Color(1.0f, 1.0f, 1.0f);
                break;
            case 1:
                difficult_text.text = "Normal";
                difficult_text.color = new Color(1.0f, 1.0f, 1.0f);
                break;
            case 2:
                difficult_text.text = "Hard";
                difficult_text.color = new Color(1.0f, 0.0f, 0.0f);
                break;

        }

    }

    void VsImageOn()
    {
       
        VariableManager_Coop.instance.howmany = copy_info[1];
        VariableManager_Coop.instance.m_roomid = copy_info[2];
        for(int i = 0; i < 4; ++i)
        {
            tier_num[i] = copy_info[3+i];

        }
        id_no1 = BitConverter.ToString(copy_info, 7, 20);
        id_no2 = BitConverter.ToString(copy_info, 27, 20);
        id_no3 = BitConverter.ToString(copy_info, 47, 20);
        id_no4 = BitConverter.ToString(copy_info, 67, 20);
        id_no1 = System.Text.Encoding.UTF8.GetString(copy_info, 7, 20);
        id_no2 = System.Text.Encoding.UTF8.GetString(copy_info, 27, 20);
        id_no3 = System.Text.Encoding.UTF8.GetString(copy_info, 47, 20);
        id_no4 = System.Text.Encoding.UTF8.GetString(copy_info, 67, 20);
        VariableManager_Coop.instance.id_list[0] = id_no1;
        VariableManager_Coop.instance.id_list[1] = id_no2;
        VariableManager_Coop.instance.id_list[2] = id_no3;
        VariableManager_Coop.instance.id_list[3] = id_no4;

        if (String.Compare(id_no1, VariableManager_Coop.instance.mystringID) == 0)
        {
            Debug.Log("1");
            VariableManager_Coop.instance.SetMyPos(1);
        }
        if (String.Compare(id_no2, VariableManager_Coop.instance.mystringID) == 0)
        {
            Debug.Log("2");
            VariableManager_Coop.instance.SetMyPos(2);
        }
        if (String.Compare(id_no3, VariableManager_Coop.instance.mystringID) == 0)
        {
            Debug.Log("3");
            VariableManager_Coop.instance.SetMyPos(3);
        }
        if (String.Compare(id_no4, VariableManager_Coop.instance.mystringID) == 0)
        {
            Debug.Log("4");
            VariableManager_Coop.instance.SetMyPos(4);
        }
        partnerIDtext[0].text = id_no1;
        partnerIDtext[1].text = id_no2;
        partnerIDtext[2].text = id_no3;
        partnerIDtext[3].text = id_no4;
        matching_image_success.SetActive(false);
        matching_image_vs_status.SetActive(true);
        matching_particles.SetActive(true);
        Invoke("GoToPlay", 3.0f);
    }
	// Update is called once per frame
	void Update () {
        myinfo_text.text = VariableManager_Coop.instance.GetStringID();
        switch (VariableManager_Coop.instance.tier)
        {
            case 0:
                tier_text.text = "Bronze";
                break;
            case 1:
                tier_text.text = "Silver";
                break;
            case 2:
                tier_text.text = "Gold";
                break;
            case 3:
                tier_text.text = "Platinum";
                break;
            case 4:
                tier_text.text = "Master";
                break;

        }
        for (int i = 0; i < 4; ++i) {
            if (tier_num[i] <= 4)
            {
                player_Tier[i].gameObject.SetActive(true);
                player_Tier[i].texture = tierImage[tier_num[i]];
            }
            else
                player_Tier[i].gameObject.SetActive(false);
        }
        tierimage_cur.texture = tierImage[VariableManager_Coop.instance.tier];
        

        if (success_Match==1)
        {

            Debug.Log("Match Success!!");
            matching_image.SetActive(false);
            matching_image_success.SetActive(true);
            success_Match = 0;
            Invoke("VsImageOn", 2.0f);
        }
        else if (success_Match == 2)
        {
            matching_image.SetActive(false);
            matching_image_fail.SetActive(true);
        }
    }
}
