using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Text;
using System;
using static UnityEngine.Rendering.DebugUI;

public class SqlDbCommand : SqlDbConnect
{
    private SqliteCommand _sqlComm;
    private SqliteDataReader _sqlDR;

    public SqlDbCommand(string dbPath) : base(dbPath)
    {
        _sqlComm = new SqliteCommand(_sqlConn);
    }

    #region 表管理
    public int CreateTable<T>(string tableName) where T : BaseData
    {
        ReleaseCommand();
        if (IsTableCreate<T>(tableName))
        {
            return -1;
        }
        var type = typeof(T);

        var stringBuider = new StringBuilder();

        stringBuider.Append($"create table {tableName} (");

        var properties = type.GetProperties();

        foreach (var p in properties)
        {
            Debug.Log($"{p}");
            var attribute = p.GetCustomAttribute<ModelHelp>();
            Debug.Log($"{attribute.FieldName}");
            if (attribute.IsCreated)
            {
                stringBuider.Append($"{attribute.FieldName} {attribute.Type} ");

                if (attribute.IsPrimaryKey)
                {
                    stringBuider.Append(" primary key ");
                }
                if (attribute.IsCanBeNull)
                {
                    stringBuider.Append(" null ");
                }
                else
                {
                    stringBuider.Append(" not null ");
                }
                stringBuider.Append(",");
            }
        }
        stringBuider.Remove(stringBuider.Length - 1, 1);
        stringBuider.Append(")");
        ReleaseCommand();
        _sqlComm.CommandText = stringBuider.ToString();
        
        return _sqlComm.ExecuteNonQuery();
    }

    public int DeleteTable(string tableName)
    {
        var sql = $"drop table {tableName}";
        _sqlComm.CommandText = sql.ToString();
        return _sqlComm.ExecuteNonQuery();

    }

    public bool IsTableCreate<T>(string tableName) where T : BaseData
    {
        ReleaseCommand();
        string sql = $"SELECT count(*) FROM sqlite_master WHERE type ='table' AND name = '{tableName}'";

        _sqlComm.CommandText = sql;
        _sqlDR = _sqlComm.ExecuteReader();
        if(_sqlDR != null&& _sqlDR.Read()) 
        {
            return Convert.ToInt32(_sqlDR[_sqlDR.GetName(0)])==1;
        }
        ReleaseCommand();
        return false;
    }

    #endregion

    #region 新增
    public int Insert<T>(string tableName, T t) where T : BaseData
    {
        if (t == default(T))
        {
            Debug.LogError("参数错误！");
            return -1;
        }
        if (SelectById<T>(tableName, t.Id) == null)
        {
            var type = typeof(T);
            StringBuilder stringbuilder = new StringBuilder();
            stringbuilder.Append($"INSERT INTO {tableName} (");
            var properties = type.GetProperties();
            foreach (var p in properties)
            {
                if (p.GetCustomAttribute<ModelHelp>().IsCreated)
                {
                    stringbuilder.Append(p.GetCustomAttribute<ModelHelp>().FieldName);
                    stringbuilder.Append(',');
                }
            }
            stringbuilder.Remove(stringbuilder.Length - 1, 1);
            stringbuilder.Append(") VALUES (");

            foreach (var p in properties)
            {
                if (p.GetCustomAttribute<ModelHelp>().IsCreated)
                {

                    if (p.GetCustomAttribute<ModelHelp>().Type == "string")
                    {
                        stringbuilder.Append($"'{p.GetValue(t)}'");
                    }
                    else
                    {
                        stringbuilder.Append(p.GetValue(t));
                    }

                    stringbuilder.Append(",");
                }
            }
            stringbuilder.Remove(stringbuilder.Length - 1, 1);
            stringbuilder.Append(")");
            ReleaseCommand();
            _sqlComm.CommandText = stringbuilder.ToString();
            return _sqlComm.ExecuteNonQuery();
        }
        return 0;
    }

    public int Insert<T>(string tableName, List<T> tList) where T : BaseData
    {
        if (tList == null || tList.Count == 0)
        {
            Debug.LogError("参数错误！");
            return -1;
        }

        var type = typeof(T);
        StringBuilder stringbuilder = new StringBuilder();
        stringbuilder.Append($"INSERT INTO {tableName} (");
        var properties = type.GetProperties();
        foreach (var p in properties)
        {
            if (p.GetCustomAttribute<ModelHelp>().IsCreated)
            {
                stringbuilder.Append(p.GetCustomAttribute<ModelHelp>().FieldName);
                stringbuilder.Append(',');
            }
        }
        stringbuilder.Remove(stringbuilder.Length - 1, 1);
        stringbuilder.Append(") VALUES ");

        foreach (var t in tList)
        {
            if (SelectById<T>(tableName, t.Id) == null)
            {
                stringbuilder.Append(" ( ");
                foreach (var p in properties)
                {
                    if (p.GetCustomAttribute<ModelHelp>().IsCreated)
                    {
                        if (p.GetCustomAttribute<ModelHelp>().Type == "string")
                        {
                            stringbuilder.Append($"'{p.GetValue(t)}'");
                        }
                        else
                        {
                            stringbuilder.Append(p.GetValue(t));
                        }

                        stringbuilder.Append(",");
                    }
                }
                stringbuilder.Remove(stringbuilder.Length - 1, 1);
                stringbuilder.Append("),");
            }
           
        }

        stringbuilder.Remove(stringbuilder.Length - 1, 1);

        _sqlComm.CommandText = stringbuilder.ToString();
        return _sqlComm.ExecuteNonQuery();
    }

    #endregion

    #region 删除
    //删除

    public int DeleteById(string tableName, int id)
    {
        var sql = $"DELETE FROM {tableName} where Id = {id}";
        _sqlComm.CommandText = sql;
        return _sqlComm.ExecuteNonQuery();
    }

    public int DeleteById(string tableName, List<int> ids)
    {
        int count = 0;
        foreach (var id in ids)
        {
            count += DeleteById(tableName, id);
        }

        return count;
    }

    public int DeleteBySql(string tableName, string sql)
    {
        _sqlComm.CommandText = $"DELETE FROM {tableName} where {sql}";
        return _sqlComm.ExecuteNonQuery();
    }
    public int DeleteByType(string tableName, string type, string judgement, string info)
    {

        var sql = $"{type}{judgement}{info}";
        return DeleteBySql(tableName, sql);

    }
    #endregion
     
    #region 更新
    public int Update<T>(string tableName, T t) where T : BaseData
    {

        if (t == default(T))
        {
            Debug.LogError("参数错误！");
            return -1;
        }
        var type = typeof(T);
        StringBuilder stringbuilder = new StringBuilder();
        stringbuilder.Append($"UPDATE {tableName} set ");
        var properties = type.GetProperties();

        foreach (var p in properties)
        {
            if (p.GetCustomAttribute<ModelHelp>().IsCreated)
            {
                stringbuilder.Append($" {p.GetCustomAttribute<ModelHelp>().FieldName} = ");
                if (p.GetCustomAttribute<ModelHelp>().Type == "string")
                {
                    stringbuilder.Append($"'{p.GetValue(t)}'");
                }
                else
                {
                    stringbuilder.Append(p.GetValue(t));
                }

                stringbuilder.Append(",");
            }

        }
        stringbuilder.Remove(stringbuilder.Length - 1, 1);
        stringbuilder.Append($" where Id  = {t.Id}");
        ReleaseCommand();
        _sqlComm.CommandText = stringbuilder.ToString();
        return _sqlComm.ExecuteNonQuery();
    }

    public int Update<T>(string tableName, List<T> tList) where T : BaseData
    {
        if (tList == null || tList.Count == 0)
        {
            Debug.LogError("Update参数错误！");
            return -1;
        }

        int count = 0;
        foreach (var t in tList)
        {
            count += Update(tableName, t);
        }
        return count;
    }
    #endregion

    #region 查询
    public T SelectById<T>(string tableName,int id) where T : BaseData
    {
        ReleaseCommand();
        var type = typeof(T);
        var sql = $"SELECT * FROM {tableName} where Id = {id}";
        _sqlComm.CommandText = sql;
        _sqlDR = _sqlComm.ExecuteReader();
        if (_sqlDR.Read()&& _sqlDR != null) 
        {
            return DataReaderToData<T>();
        }
        ReleaseCommand();
        return default(T);

    }
    public List<T> SelectBySql<T>(string tableName,string sqlWhere="") where T : BaseData
    {
        ReleaseCommand();
        string sql;
        var type = typeof(T);
        if (string.IsNullOrEmpty(sqlWhere)) 
        {
            sql = $"SELECT * FROM {tableName}";
        }
        else
        {
            sql = $"SELECT * FROM {tableName} where {sqlWhere}";
        }
        
        _sqlComm.CommandText = sql;
        _sqlDR = _sqlComm.ExecuteReader();
        var ret = new List<T>();
        if (_sqlDR != null)
        {
            while (_sqlDR.Read())
            {
                ret.Add(DataReaderToData<T>());
            }

        }
        ReleaseCommand();
        return ret;

    }
    public T[] SelectBySqlToArray<T>(string tableName, string sqlWhere = "") where T : BaseData
    {
        ReleaseCommand();
        string sql;
        var type = typeof(T);
        if (string.IsNullOrEmpty(sqlWhere))
        {
            sql = $"SELECT * FROM {tableName}";
        }
        else
        {
            sql = $"SELECT * FROM {tableName} where {sqlWhere}";
        }

        _sqlComm.CommandText = sql;
        _sqlDR = _sqlComm.ExecuteReader();
        var ret = new T[_sqlDR.FieldCount];
        if (_sqlDR != null)
        {
            while (_sqlDR.Read())
            {
                T data = DataReaderToData<T>();
                ret[data.Id]=data;
            }

        }
        ReleaseCommand();
        return ret;

    }

    public Dictionary<int, T> SelectBySqlDic<T>(string tableName, string sqlWhere = "") where T : BaseData
    {
        ReleaseCommand();
        string sql;
        var type = typeof(T);
        if (string.IsNullOrEmpty(sqlWhere))
        {
            sql = $"SELECT * FROM {tableName}";
        }
        else
        {
            sql = $"SELECT * FROM {tableName} where {sqlWhere}";
        }

        _sqlComm.CommandText = sql;
        _sqlDR = _sqlComm.ExecuteReader();
        var ret = new Dictionary<int,T>();
        if (_sqlDR != null)
        {
            while (_sqlDR.Read())
            {
                T data = DataReaderToData<T>();
                ret[data.Id] = data;
            }

        }
        ReleaseCommand();
        return ret;

    }
    private T DataReaderToData<T>() where T : BaseData
    {
        try
        {
            List<string> fieldNames =new List<string>();
            for(int i=0;i< _sqlDR.FieldCount;i++)
            {
                fieldNames.Add(_sqlDR.GetName(i));
            }
            var type =  typeof(T);
            T data = Activator.CreateInstance<T>();
            var properties = type.GetProperties();

            foreach (var p in properties)
            {
                if (!p.CanWrite)
                    continue;
                var fieldName = p.GetCustomAttribute<ModelHelp>().FieldName;
                object value = _sqlDR[fieldName];
                if (fieldName.Contains(fieldName) && p.GetCustomAttribute<ModelHelp>().IsCreated)
                {
                    if (p.PropertyType.FullName == "System.Byte")//判断类型，如果是则强转
                        value = Convert.ToByte(value);
                    p.SetValue(data, value);
                }

            }
            
            return data;

        }
        catch(System.Exception e)
        {
            Debug.LogError($"{e.Message}");
            return null;
        }
    }

    private void ReleaseCommand()
    {
        if (_sqlDR != null)
        {
            _sqlDR.Close();
        }
        _sqlDR = null;

        if (_sqlDR != null)
        {
            _sqlDR.Dispose();
        }
        _sqlDR = null;
    }
    #endregion
}