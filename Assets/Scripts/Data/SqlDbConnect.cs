using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.IO;

public class SqlDbConnect
{
    protected SqliteConnection _sqlConn;

    public SqlDbConnect(string dbPath)
    {
        if (!File.Exists(dbPath))
        {
            CreateDbSqlite(dbPath);

        }
        ConnectDbSqlite(dbPath);
    }

    private bool CreateDbSqlite(string dbPath)
    {
        try
        {
            var dirName = new FileInfo(dbPath).Directory.FullName;
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }

            SqliteConnection.CreateFile(dbPath);
            return true;
        }
        catch(System.Exception e)
        {
            Debug.LogError($"数据库创建异常：{e.Message}");
            return false;
        }
    }

    private bool ConnectDbSqlite(string dbPath)
    {
        try
        {
            _sqlConn = new SqliteConnection(new SqliteConnectionStringBuilder() { DataSource = dbPath }.ToString());
            _sqlConn.Open();
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"数据库连接异常：{e.Message}");
            return false;
        }
    }

    public void Dispose()
    {
        _sqlConn.Dispose();
    }
}
