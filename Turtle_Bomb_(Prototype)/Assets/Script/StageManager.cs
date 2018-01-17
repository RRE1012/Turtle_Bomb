using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct Coordinate
{
    public float x, z;
    public bool isBlocked;
    public Coordinate(float x, float z, bool isBlocked)
    {
        this.x = x;
        this.z = z;
        this.isBlocked = isBlocked;
    }
}

public class StageManager : MonoBehaviour {

    public int m_Map_Size_X = 16;
    public int m_Map_Size_Z = 16;

    public Text m_Text;

    public static bool m_is_Stage_Clear = false;
    public static int m_Stars = 0;
    public static int m_Left_Monster = 0;

    public static Coordinate m_tmpCoordinate;
    public static Coordinate m_tmpCoord_For_Objects;

    public static List<Coordinate> m_Map_Coordinate_List;

    public static StageManager c_Stage_Manager;
    public static bool m_is_init_MCL;


    void Awake()
    {
        m_is_Stage_Clear = false;
        m_Stars = 0;
        m_Left_Monster = 0;

        m_is_init_MCL = false;
        m_Map_Coordinate_List = new List<Coordinate>();
        
        c_Stage_Manager = this;
        MCL_init();
    }

    void Update()
    {
        if (!PlayerMove.C_PM.Get_IsAlive())
        {
            m_Text.text = "Game Over";
        }
	}
    
    public static void StageClear()
    {
        m_is_Stage_Clear = true;
        UI.Draw_StageClearPage();
    }



    // 좌표 초기화
    void MCL_init()
    {
        for (int i = -1; i < m_Map_Size_X; ++i)
        {
            for (int j = -1; j < m_Map_Size_Z; ++j)
            {
                m_tmpCoordinate.x = j * 2;
                m_tmpCoordinate.z = i * 2;
                m_tmpCoordinate.isBlocked = false;
                m_Map_Coordinate_List.Add(m_tmpCoordinate);
            }
        }
        m_is_init_MCL = true;
    }


    // 오브젝트들이 MCL의 isBlocked를 갱신할 수 있도록 해주는 메소드
    public static void Update_MCL_isBlocked(int index, bool isBlocked)
    {
        m_tmpCoordinate.x = m_Map_Coordinate_List[index].x;
        m_tmpCoordinate.z = m_Map_Coordinate_List[index].z;
        m_tmpCoordinate.isBlocked = isBlocked;
        m_Map_Coordinate_List.Insert(index, m_tmpCoordinate);
    }

    // 받아온 좌표에 대한 MCL의 인덱스 반환
    public static int Find_Own_MCL_Index(float x, float z)
    {

        // 받아온 좌표에서 가장 가까운 좌표로 설정한다.
        int index_X = (int)x;
        int index_Z = (int)z;

        if (index_X % 2 == 1)
        {
            m_tmpCoord_For_Objects.x = index_X + 1;
        }
        else if (index_X % 2 == -1)
        {
            m_tmpCoord_For_Objects.x = index_X - 1;
        }
        else
            m_tmpCoord_For_Objects.x = index_X;


        if (index_Z % 2 == 1)
        {
            m_tmpCoord_For_Objects.z = index_Z + 1;
        }
        else if (index_Z % 2 == -1)
        {
            m_tmpCoord_For_Objects.z = index_Z - 1;
        }
        else
            m_tmpCoord_For_Objects.z = index_Z;

        //=========================


        // 속한 좌표의 인덱스를 뽑아서 반환한다.
        return m_Map_Coordinate_List.IndexOf(m_tmpCoord_For_Objects);
    }
}
