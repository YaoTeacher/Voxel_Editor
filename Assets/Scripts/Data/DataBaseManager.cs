using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using UnityEditor.MemoryProfiler;

public class DataBaseManager : MonoBehaviour
{

    private static DataBaseManager _dbInstance = null;
    public static DataBaseManager DBInstance()
    {
        return _dbInstance;
    }

    //�������ݿ�����
    SqliteConnection connection;
    //���ݿ�����
    SqliteCommand command;
    //���ݿ��Ķ���
    SqliteDataReader reader;


    private string dbName = "data";  
    // Start is called before the first frame update
    void Start()
    {
        connection = new SqliteConnection(Application.streamingAssetsPath + "/TestDatabase.db");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
