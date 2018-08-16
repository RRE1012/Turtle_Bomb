using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class Boss_AI_Coop : MonoBehaviour {

    public static Boss_AI_Coop instance;
    public RawImage m_Boss_HP_Bar; // 보스 HP바 이미지
    public RawImage m_Boss_HP_Character; // 보스 HP바 캐릭터

    public int hp;
    float m_Attack_Speed = 1.5f;
    int target;
    byte[] copy_info = new byte[16];
    public byte bossani;
    public byte boss_damaged;
    public byte boss_alive;
    public byte boss_target = 1;
    public float bossX = 14.0f;
    public float bossZ = 14.0f;
    public float boss_rotY = 1.0f;
    public GameObject m_Attack_Collider;
    public GameObject m_Attack_Collider_UI;
    public GameObject[] m_Flame_Crash;
    void Awake()
    {
        instance =this;
    }
	// Use this for initialization
	void Start () {

        for(int i = 0; i < 5; ++i)
        {
            m_Flame_Crash[i].SetActive(false);
        }
        m_Attack_Collider.SetActive(false);
        hp = 30;
        Boss_HP_Gauge.GetInstance().Set_Max_HP(hp);

        boss_damaged = 0;
        transform.position = new Vector3(14.0f, transform.position.y, 14.0f);
        transform.rotation = new Quaternion(transform.rotation.x, boss_rotY, transform.rotation.z, transform.rotation.w);
    }
    public void SetBossState(byte[] a)
    {

        bossani = a[2];
        hp = a[3];
        boss_damaged = a[4];
    }
    public void SetBossPoss(byte[] a)
    {
        Buffer.BlockCopy(a, 2, copy_info, 0, 16);
        // Debug.Log("Copy PosSet_Boss");

        // Debug.Log("End PosSet_Boss");
        bossani = copy_info[0];
        boss_alive = copy_info[1];
        if (boss_alive == 2) { boss_target = copy_info[2]; }
        else
        {
            boss_target = copy_info[2];

            bossX = BitConverter.ToSingle(copy_info, 4);
            bossZ = BitConverter.ToSingle(copy_info, 8);
            boss_rotY = BitConverter.ToSingle(copy_info, 12);
        }
        //boss_rotY = (boss_rotY * 2.0f) - 1.0f;
    }

    void SetAniState(byte anistate)
    {
        switch (anistate)
        {
            case (byte)0:

                break;
            case (byte)1:
                GetComponent<Animator>().SetBool("OrkBoss_isHurt", false);
                GetComponent<Animator>().SetBool("OrkBoss_isAttack", false);
                GetComponent<Animator>().SetBool("OrkBoss_isSummon", false);
                GetComponent<Animator>().SetBool("OrkBoss_isWalk", true);
                break;
            case (byte)2:
                if (!GetComponent<Animator>().GetBool("OrkBoss_isSummon"))
                {
                    GetComponent<Animator>().SetBool("OrkBoss_isHurt", false);
                    GetComponent<Animator>().SetBool("OrkBoss_isWalk", false);
                    GetComponent<Animator>().SetBool("OrkBoss_isSummon", false);
                    GetComponent<Animator>().SetBool("OrkBoss_isAttack", true);
                }
                bossani = 0;

                break;
            case (byte)3:
                GetComponent<Animator>().SetBool("OrkBoss_isWalk", false);
                GetComponent<Animator>().SetBool("OrkBoss_isAttack", false);
                GetComponent<Animator>().SetBool("OrkBoss_isSummon", true);
                Skill_Fire_Crash();
                bossani = 0;
                break;
            case (byte)4:
                GetComponent<Animator>().SetBool("OrkBoss_isWalk", false);
                GetComponent<Animator>().SetBool("OrkBoss_isAttack", false);
                GetComponent<Animator>().SetBool("OrkBoss_isHurt", true);
                bossani = 0;
                break;
            case (byte)5:
                GetComponent<Animator>().SetBool("OrkBoss_isDead", true);
                break;
        }

    }

    void Skill_Fire_Crash()
    {
        for (int i = 0; i < 5; ++i)
        {
            m_Flame_Crash[i].SetActive(true);
        }
    }
    // Update is called once per frame
    void Update () {
        int hp_current = hp;
        Vector3 newPos;
        newPos.x = 0 - (hp_current * 10);
        newPos.y = 0.0f;
        newPos.z = 0.0f;
        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f && GetComponent<Animator>().GetBool("OrkBoss_isAttack")) // 초기 부분
        {
            GetComponent<Animator>().SetFloat("Attack_Speed", m_Attack_Speed); // 슬로우모션 후 뒷부분은 빠르게하기 위해..
            m_Attack_Collider.SetActive(true); // 공격용 충돌체를 꺼낸다.
            m_Attack_Collider_UI.SetActive(true); // 공격용 충돌체를 꺼낸다.
        }
        else
        {
            GetComponent<Animator>().SetFloat("Attack_Speed", 0.2f);
            m_Attack_Collider.SetActive(false);
            m_Attack_Collider_UI.SetActive(false); // 공격용 충돌체를 꺼낸다.
        }
        SetAniState(bossani);

        if (boss_damaged == 1)
        {
            Boss_HP_Gauge.GetInstance().Set_Curr_HP(hp_current);
            //m_Boss_HP_Character.GetComponent<Animator>().SetTrigger("Damaged");
            boss_damaged = 0;
        }
        Vector3 newPos2;
        newPos2.x = 260.0f - (hp_current * 17);
        newPos2.y = 60.0f;
        newPos2.z = 0.0f;

        //m_Boss_HP_Bar.rectTransform.sizeDelta = new Vector2(585.0f - (hp_current * 19), 30);
        //m_Boss_HP_Bar.rectTransform.localPosition = newPos;
        //m_Boss_HP_Character.rectTransform.localPosition = newPos2;

        
        target = boss_target;
        //Debug.Log(bossX+","+bossZ);
        if (!GetComponent<Animator>().GetBool("OrkBoss_isSummon"))
        {
            Vector3 targetPos = new Vector3(bossX, -0.76f, bossZ);

            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 1.5f);
            if (target > 0)
            {
                if (target == VariableManager_Coop.instance.pos_id)
                {
                    // Debug.Log("Look at Me!!");
                    transform.LookAt(CoOpManager.instance.myturtle.transform);
                }
                else
                {
                    //Debug.Log(target);
                    if (CoOpManager.instance.m_net_user[target - 1].activeInHierarchy)
                        transform.LookAt(CoOpManager.instance.m_net_user[target - 1].transform);
                }
                boss_rotY = transform.eulerAngles.y;
            }
        }
    }
}
