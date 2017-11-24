using System;
using System.Collections;
using System.Collections.Generic;
//using System.Data;
//using System.Data.Odbc;

using UnityEngine;

//using Excel = Microsoft.Office.Interop.Excel;



public class XLSReader : MonoBehaviour {
   // string a = "Map1.xlsx";
    // Use this for initialization

    public GameObject boxBreakable;
    public GameObject boxUnbreakable;
    public GameObject Bomb;
    public GameObject boxB;
    public GameObject boxU;
    
    public int bombAmount;
    string FileName ="Map3";

    string[] stringList1;

    string[] stringList2;
    string[][] stringList3;


    string fileFullPath;





    void Start()

    {
        if (null != StageSelectLoader.instance)
        {
            switch (StageSelectLoader.instance.GetStage_Num())
            {
                case 1:
                    FileName = "Map1";
                    break;
                case 2:
                    FileName = "Map2";
                    break;
                case 3:
                    FileName = "Map3";
                    break;
                default:
                    FileName = "Map3";
                    break;
            }
        }
        TextAsset txtFile = Resources.Load(FileName) as TextAsset;

        fileFullPath = txtFile.text;



        stringList1 = fileFullPath.Split('\n');

       


        stringList2 = stringList1[0].Split(',');
        string[] s = stringList1[2].Split(',');
        string[] s2 = stringList1[1].Split(',');

        bombAmount = Int32.Parse(s[1]);
        for(int z = 1; z < Int32.Parse(s2[1]); ++z)
        {
            stringList2 = stringList1[2+z].Split(',');
            for (int x=1; x < Int32.Parse(s2[2]); ++x)
            {
                if (stringList2[x] == "B")
                {
                    Vector3 temp = new Vector3(-9.0f+(float)(x*3),1.25f,-8.0f+(float)(z*3));
                    GameObject obj2 = Object_Pool.obj_pool.GetPoolObject_Box();
                    if (obj2 == null)
                        return;
                    obj2.transform.position = new Vector3(temp.x, temp.y, temp.z);
                    obj2.transform.rotation = transform.rotation;
                    obj2.SetActive(true);
                    
                }
                else if (stringList2[x] == "U")
                {
                    Vector3 temp = new Vector3(-9.0f + (float)(x * 3), 1.25f, -8.0f + (float)(z * 3));
                    GameObject obj3 = Object_Pool.obj_pool.GetPoolObject_BoxU();
                    if (obj3 == null)
                        return;
                    obj3.transform.position = new Vector3(temp.x, temp.y, temp.z);
                    obj3.transform.rotation = transform.rotation;
                    obj3.SetActive(true);
                    //boxU = (GameObject)Instantiate(boxUnbreakable, temp, UnityEngine.Quaternion.identity);
                }
                else if (stringList2[x] == "1")
                {
                    Vector3 temp = new Vector3(-9.0f + (float)(x * 3), 1.25f, -8.0f + (float)(z * 3));
                    GameObject obj3 = Object_Pool.obj_pool.GetPoolObject_Enemy01();
                    if (obj3 == null)
                        return;
                    obj3.transform.position = new Vector3(temp.x, temp.y, temp.z);
                    obj3.transform.rotation = transform.rotation;
                    obj3.SetActive(true);

                }

            }

        }
        foreach (string b in stringList2)

        {

            Debug.Log(b);

        }
        Debug.Log(bombAmount);
        //Invoke("ChangeText", 1.5f);

    }

    void ChangeText()

    {



        for (int i = 0; ; i++)

        {
            // 여기서 출력!
          

        }

    }
    public int GetPoolAmount()
    {
        int a = bombAmount;
        return a;
        /*
        GameObject obj = Object_Pool.obj_pool.GetPoolObject();
        if (obj == null)
            return;
        obj.transform.position = new Vector3(bombLocX, 0.5f, bombLocZ);
        obj.transform.rotation = transform.rotation;
        obj.SetActive(true);
        */
    }
}
/*
private DataTable ReadXLS(string _filePath, string _tableName)
    {
        // Must be saved as excel 2003 workbook, not 2007, mono issue really 
        //string con = "Driver={Microsoft Excel Driver (*.xls)}; DriverId=790; Dbq=" + _filePath + ";"; 
        string con = "Driver={Microsoft Excel Driver (*.xls, *.xlsx, *.xlsm, *.xlsb)}; Dbq=" + _filePath + ";";
        //Debug.Log(con); 

        //string yourQuery = "SELECT * FROM [Sheet1$]"; 
        string yourQuery = string.Format(_tableName);
        // our odbc connector 
        OdbcConnection oCon = new OdbcConnection(con);
        // our command object 
        OdbcCommand oCmd = new OdbcCommand(yourQuery, oCon);
        // table to hold the data 
        DataTable dtYourData = new DataTable("YourData");
        // open the connection 
        oCon.Open();
        // lets use a datareader to fill that table! 
        OdbcDataReader rData = oCmd.ExecuteReader();
        // now lets blast that into the table by sheer man power! 
        dtYourData.Load(rData);
        Debug.Log(rData);
        // close that reader! 
        rData.Close();
        // close your connection to the spreadsheet! 
        oCon.Close();
        // wow look at us go now! we are on a roll!!!!! 
        // lets now see if our table has the spreadsheet data in it, shall we? 

        return dtYourData;
    }
}
*/