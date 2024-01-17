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

    //建立数据库连接
    SqliteConnection connection;
    //数据库命令
    SqliteCommand command;
    //数据库阅读器
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
