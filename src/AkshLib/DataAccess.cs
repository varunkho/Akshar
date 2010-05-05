using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Akshar.Lib;

namespace Akshar.Lib.Data
{
        /// <summary>
    ///  Enum to provide list of Providers to use
    /// </summary>
    public enum ProviderType
    {
        /// <summary>
        /// Provider for SQL Server
        /// </summary>
        SqlClient = 0,
        /// <summary>
        /// .Net Provider for Oracle 
        /// </summary>
        OracleClient = 1,
        /// <summary>
        ///OleDb provider for different databases
        /// </summary>
        OleDB = 2,
        /// <summary>
        /// ODBC provider for different databases
        /// </summary>
        Odbc = 3,
    }
    /// <summary>
    ///     This Class provides all methods for interacting with the Database.
    ///     It has implemented Genric DB Provider feature which can able to interact with any 
    ///     Database as per Provider from enum "ProviderType"
    /// </summary> 
    public sealed class DataAccess : IDisposable
    {
        private DbProviderFactory _dbPvdrFctr;
        private DbConnection _genricConnection;
        private DbCommand _genricCommand;
        private DbTransaction _genricTransaction;

        public DataAccess() : this(Common.ConnectionString, ProviderType.SqlClient)
        { }

        /// <summary>
        /// Constructor which take only Connection String which uses default provider "SqlClient"  
        /// </summary>
        /// <param name="connectionStr">Valid Connection string </param>
        /// <remarks> This constructor always calls overloaded constructor with default provider name </remarks> 
        public DataAccess(string connectionStr)
            : this(connectionStr, ProviderType.SqlClient)
        {
            //
        }
        /// <summary> 
        /// Constructor which take Connection String and provider type
        /// </summary>
        /// <param name="connectionStr">Valid Connection string </param>
        /// <param name="providerName">Value from Enum DataAccess.ProviderType</param>
        public DataAccess(string connectionStr, ProviderType providerName)
        {
            _dbPvdrFctr = DbProviderFactories.GetFactory(GetProviderInvariantName(providerName));
            try
            {
                _genricConnection = _dbPvdrFctr.CreateConnection();
                _genricConnection.ConnectionString = connectionStr;
            }
            catch (DbException crtConExp)
            {
                throw new Exception("Unable to Create Connection from '" + providerName
                                    + "' DB Provider", crtConExp);
            }
        }
        /// <summary>
        ///  This method retuns valid command object 
        /// </summary>
        /// <returns></returns>
        public DbCommand getDBCommand()
        {
            if (_genricCommand == null)
            {
                _genricCommand = _genricConnection.CreateCommand();
            }

            return _genricCommand;
        }
        private string GetProviderInvariantName(ProviderType providerType)
        {
            string providerNameSpace = null;
            switch (providerType)
            {
                case ProviderType.SqlClient:
                    providerNameSpace = "System.Data.SqlClient";
                    break;
                case ProviderType.OracleClient:
                    providerNameSpace = "System.Data.OracleClient";
                    break;
                case ProviderType.OleDB:
                    providerNameSpace = "System.Data.OleDb";
                    break;
                case ProviderType.Odbc:
                    providerNameSpace = "System.Data.Odbc";
                    break;
            }
            return providerNameSpace;
        }

        /// <summary>
        ///  Open Database Connection
        /// </summary>
        public void OpenConnection()
        {
            if (_genricConnection.State == ConnectionState.Closed)
            {
                _genricConnection.Open();

            }
        }

        /// <summary>
        ///  This method provide Data Reader (Base Object of type "DbDataReader") which can be 
        ///  type cast to any DataReader class.
        /// </summary>
        /// <param name="selectSQL"> valid SQL select query</param>
        /// <returns>Base Object of type "DbDataReader"</returns>
        /// <example> 
        ///  System.Data.SqlClient.SqlDataReader sqlRd = 
        ///     (System.Data.SqlClient.SqlDataReader)dtl.ExecuteQueryForReader("select * from authors"); 
        ///</example> 
        public Object ExecuteQueryForReader(string selectSQL)
        {
            DbDataReader dbDataRdr;
            try
            {
                _genricCommand = _genricConnection.CreateCommand();
                OpenConnection();
                _genricCommand.CommandType = CommandType.Text;
                _genricCommand.CommandText = selectSQL;
                dbDataRdr = _genricCommand.ExecuteReader(CommandBehavior.CloseConnection);
            }
            finally
            {
                _genricCommand.Dispose();
                _genricCommand = null;
            }
            return (Object)dbDataRdr;
        }
        /// <summary>
        ///  This method provide DataTable of supplied query result.
        ///  This should be used when single query data is passed. DataTable is lighter as compare to DataSet.
        /// </summary>
        /// <param name="selectSQL"></param>
        /// <returns> DataTable </returns>

        public DataTable ExecuteSelectForDataTable(string selectSQL)
        {
            DataTable dtResult = new DataTable();

            try
            {
                _genricCommand = _genricConnection.CreateCommand();
                OpenConnection();
                _genricCommand.CommandType = CommandType.Text;
                _genricCommand.CommandText = selectSQL;

                DbDataAdapter dbDataAdptr = _dbPvdrFctr.CreateDataAdapter();
                dbDataAdptr.SelectCommand = _genricCommand;
                dbDataAdptr.Fill(dtResult);
            }
            finally
            {
                CloseConnection();
            }

            return dtResult;

        }
        /// <summary>
        ///  This method provide DataSet of supplied query result.
        ///  This should only be used when query may return muiltiple DataTables.    
        /// </summary>
        /// <param name="selectSQL">valid SQL query</param>
        /// <returns> DataSet</returns>
        public DataSet ExecuteSelectForDataSet(string selectSQL)
        {
            DataSet dsResult = new DataSet();

            try
            {
                _genricCommand = _genricConnection.CreateCommand();
                OpenConnection();
                _genricCommand.CommandType = CommandType.Text;
                _genricCommand.CommandText = selectSQL;

                DbDataAdapter dbDataAdptr = _dbPvdrFctr.CreateDataAdapter();
                dbDataAdptr.SelectCommand = _genricCommand;
                dbDataAdptr.Fill(dsResult);
            }
            finally
            {
                CloseConnection();
            }

            return dsResult;

        }
        /// <summary>
        /// This method perform insert/ update / delete on database based on supplied query
        /// </summary>
        /// <param name="strSQL"> valid insert update statement</param>
        /// <returns> Numbers of Rows affected </returns>
        public int ExecuteInsertUpdate(string strSQL)
        {
            int rowsAffected = -1;
            try
            {
                _genricCommand = _genricConnection.CreateCommand();
                OpenConnection();
                _genricCommand.CommandType = CommandType.Text;
                _genricCommand.CommandText = strSQL;
                rowsAffected = _genricCommand.ExecuteNonQuery();
            }
            finally
            {
                CloseConnection();

            }
            return rowsAffected;
        }

        /// <summary>
        ///  This method return value (as object) of first column of first row from resulting query.
        /// </summary>
        /// <param name="selectSQL">Valid query string</param>
        /// <returns> object of resulting value</returns>
        public object ExecuteScalarQuery(string selectSQL)
        {
            object lResult = null;
            try
            {
                _genricCommand = _genricConnection.CreateCommand();
                OpenConnection();
                _genricCommand.CommandType = CommandType.Text;
                _genricCommand.CommandText = selectSQL;
                lResult = _genricCommand.ExecuteScalar();
            }
            finally
            {
                CloseConnection();
            }
            return lResult;
        }

       







        #region  "Transaction Methods"

        /// <summary>
        ///  Start Transaction 
        /// </summary>
        /// <exception > Required valid open connection by caller</exception> 
        public void InitTransaction()
        {
            if (_genricConnection == null || _genricConnection.State == ConnectionState.Closed)
            {
                throw new Exception(" DataLayer Exception, Connection must be open before calling this method.");

            }
            _genricTransaction = _genricConnection.BeginTransaction(IsolationLevel.ReadCommitted);
            if (_genricCommand == null)
            {
                _genricCommand = _genricConnection.CreateCommand();
            }
            _genricCommand.Transaction = _genricTransaction;

        }
        /// <summary>
        ///  Commit Transaction
        /// </summary>
        public void CommitTransaction()
        {
            try
            {
                _genricTransaction.Commit();
            }
            finally
            {
                _genricTransaction = null;
            }
        }

        /// <summary>
        ///  Rollback tranaction
        /// </summary>
        public void RollbackTransaction()
        {
            try
            {
                _genricTransaction.Rollback();
            }
            finally
            {
                _genricTransaction = null;
            }
        }
        /// <summary>
        ///  
        /// </summary>
        /// <param name="sqlQuries"> array of SQL queries</param>
        /// <param name="trnsRequired"> bool value for Transaction required or not while executing quries</param>
        /// <returns> bool value of successfull execution of quries </returns> 
        public bool BatchInsertUpdate(string[] sqlQuries, bool trnsRequired)
        {
            bool finalResult = false;
            try
            {
                if (_genricCommand == null) _genricCommand = _genricConnection.CreateCommand();
                OpenConnection();
                if (trnsRequired) InitTransaction();

                for (int i = 0; i < sqlQuries.Length; i++)
                {
                    if (sqlQuries[i].Trim() != null && sqlQuries[i].Trim().Length != 0)
                    {
                        _genricCommand.CommandType = CommandType.Text;
                        _genricCommand.CommandText = sqlQuries[i];
                        _genricCommand.ExecuteNonQuery();
                    }
                }

                if (trnsRequired) CommitTransaction();
                finalResult = true;
            }
            catch (DbException ex)
            {
                if (trnsRequired) RollbackTransaction();
                throw ex;
            }
            catch (Exception exx)
            {
                if (trnsRequired) RollbackTransaction();
                throw exx;
            }

            return finalResult;
        }

        /// <summary>
        ///  This method is execute under Transaction mode only
        /// </summary>
        /// <exception> InitTransaction() must be callled before calling this method</exception>
        /// <param name="strSQL">valid insert / update statement</param>
        /// <returns>Numbers of Rows affected </returns>
        public int ExecuteInsertUpdateWithTrans(string strSQL)
        {
            if (_genricTransaction == null)
            {
                throw new Exception("DataLayer exception. This method require to call 'InitTransaction' method first");
            }

            int rowsAffected = -1;

            _genricCommand.CommandType = CommandType.Text;
            _genricCommand.CommandText = strSQL;
            rowsAffected = _genricCommand.ExecuteNonQuery();

            return rowsAffected;
        }

        #endregion //************************************************************************

        #region Methods for accessing Stored Procedures   *******************

        /// <summary>
        ///  This method execute Stored procedure with or without parameters 
        /// </summary>
        /// <param name="spName"> valid stored procedure name</param>
        /// <param name="spParams"> arrays of sp params</param>
        /// <returns> dataset of resulting stored Procedure</returns>
        /// <example>
        /// 
        ///  Ebix.DataLayer.SP_PARAMS []spPrams = new Ebix.DataLayer.SP_PARAMS[2];
        ///  spPrams[0].ParamName        = "@id";
        ///  spPrams[0].ParamType        = (object)System.Data.SqlDbType.Int ;
        ///  spPrams[0].ParamValue       = (object) 2;
        ///  spPrams[0].ParamDirection   = System.Data.ParameterDirection.Input; // optional for input
        /// </example> 
        public DataSet ExecuteQuerySPForDataSet(string spName, SP_PARAMS[] spParams)
        {
            DataSet dsResult = new DataSet();
            try
            {
                _genricCommand = _genricConnection.CreateCommand();
                OpenConnection();
                _genricCommand.CommandType = CommandType.StoredProcedure;
                _genricCommand.CommandText = spName;
                DbDataAdapter dbDataAdptr = _dbPvdrFctr.CreateDataAdapter();

                if (spParams != null)
                {
                    foreach (SP_PARAMS spParam in spParams)
                    {
                        DbParameter dbParam = _genricCommand.CreateParameter();
                        dbParam.ParameterName = spParam.ParamName;
                        dbParam.DbType = (DbType)spParam.ParamType;
                        dbParam.Direction = spParam.ParamDirection;
                        if (spParam.ParamValue == null)
                        {
                            dbParam.Value = DBNull.Value;
                        }
                        else
                        {
                            dbParam.Value = spParam.ParamValue;
                        }
                        _genricCommand.Parameters.Add(dbParam);
                    }
                }

                dbDataAdptr.SelectCommand = _genricCommand;
                dbDataAdptr.Fill(dsResult);

            }
            finally
            {
                CloseConnection();
            }
            return dsResult;
        }
        /// <summary>
        ///  This method execute Stored procedure with or without parameters 
        /// </summary>
        /// <param name="spName"> valid stored procedure name</param>
        /// <param name="spParams"> arrays of sp params</param>
        /// <param name="CommandTime"> commandtimeout</param>
        /// <returns> dataset of resulting stored Procedure</returns>
        public DataSet ExecuteQuerySPForDataSet(string spName, SP_PARAMS[] spParams, int CommandTime)
        {
            DataSet dsResult = new DataSet();
            try
            {
                _genricCommand = _genricConnection.CreateCommand();
                OpenConnection();
                _genricCommand.CommandType = CommandType.StoredProcedure;
                _genricCommand.CommandText = spName;
                _genricCommand.CommandTimeout = CommandTime;
                DbDataAdapter dbDataAdptr = _dbPvdrFctr.CreateDataAdapter();

                if (spParams != null)
                {
                    foreach (SP_PARAMS spParam in spParams)
                    {
                        DbParameter dbParam = _genricCommand.CreateParameter();
                        dbParam.ParameterName = spParam.ParamName;
                        dbParam.DbType = (DbType)spParam.ParamType;
                        dbParam.Direction = spParam.ParamDirection;
                        if (spParam.ParamValue == null)
                        {
                            dbParam.Value = DBNull.Value;
                        }
                        else
                        {
                            dbParam.Value = spParam.ParamValue;
                        }
                        _genricCommand.Parameters.Add(dbParam);
                    }
                }

                dbDataAdptr.SelectCommand = _genricCommand;
                dbDataAdptr.Fill(dsResult);

            }
            finally
            {
                CloseConnection();
            }
            return dsResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spName">Sp Name</param>
        /// <param name="spParams">DbParameter </param>
        /// <returns></returns>
        public DataSet ExecuteQuerySPForDataSet(string spName, DbParameter[] spParams)
        {
            DataSet dsResult = new DataSet();
            try
            {
                if (_genricCommand == null)
                {
                    _genricCommand = getDBCommand();
                }

                OpenConnection();
                _genricCommand.CommandType = CommandType.StoredProcedure;
                _genricCommand.CommandText = spName;
                DbDataAdapter dbDataAdptr = _dbPvdrFctr.CreateDataAdapter();

                if (spParams != null)
                {
                    foreach (DbParameter spParam in spParams)
                    {
                        if (spParam.Value == null)
                        {
                            spParam.Value = DBNull.Value;

                        }
                        _genricCommand.Parameters.Add(spParam);
                    }
                }

                dbDataAdptr.SelectCommand = _genricCommand;
                dbDataAdptr.Fill(dsResult);

            }
            finally
            {
                CloseConnection();
            }
            return dsResult;
        }

        /// <summary>
        ///  Method to excute SP which return single value object
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="spParams"></param>
        /// <returns></returns>
        public object ExecuteSPForScalar(string spName, DbParameter[] spParams)
        {
            object resultObject = null;
            try
            {
                if (_genricCommand == null)
                {
                    _genricCommand = getDBCommand();
                }

                OpenConnection();
                _genricCommand.CommandType = CommandType.StoredProcedure;
                _genricCommand.CommandText = spName;

                if (spParams != null)
                {
                    foreach (DbParameter spParam in spParams)
                    {
                        if (spParam.Value == null)
                        {
                            spParam.Value = DBNull.Value;
                        }
                        _genricCommand.Parameters.Add(spParam);
                    }
                }

                resultObject = _genricCommand.ExecuteScalar();
            }
            finally
            {
                CloseConnection();
            }
            return resultObject;

        }


        public void ExecuteSP(string spName, DbParameter[] spParams)
        {
            object resultObject = null;
            try
            {
                if (_genricCommand == null)
                {
                    _genricCommand = getDBCommand();
                }

                OpenConnection();
                _genricCommand.CommandType = CommandType.StoredProcedure;
                _genricCommand.CommandText = spName;

                if (spParams != null)
                {
                    foreach (DbParameter spParam in spParams)
                    {
                        if (spParam == null)
                            continue;
                        if (spParam.Value == null)
                        {
                            spParam.Value = DBNull.Value;
                        }
                        _genricCommand.Parameters.Add(spParam);
                    }
                }

                _genricCommand.ExecuteNonQuery();
            }
            finally
            {
                CloseConnection();
            }
        }

/// <summary>
/// Varun:
/// Created this method for transaction.
/// Call OpenConnection, initTransaction method first and when you finish, call rollback/commit transaction method as appropriate and then closeConnection.
/// </summary>
public void ExecuteSPWithTrans(string spName, DbParameter[] spParams)
        {
                    if (_genricTransaction == null)
            {
                throw new Exception("DataLayer exception. This method require to call 'InitTransaction' method first");
            }

               _genricCommand.CommandType = CommandType.StoredProcedure;
                _genricCommand.CommandText = spName;

                _genricCommand.Parameters.Clear();
                if (spParams != null)
                {
                    foreach (DbParameter spParam in spParams)
                    {
                        if (spParam.Value == null)
                        {
                            spParam.Value = DBNull.Value;
                        }
                        _genricCommand.Parameters.Add(spParam);
                    }
                }

                _genricCommand.ExecuteNonQuery();
        }

/// <summary>
/// Varun:
/// Created this method for transaction.
/// Call OpenConnection, initTransaction method first and when you finish, call rollback/commit transaction method as appropriate and then closeConnection.
/// </summary>
public object ExecuteScalerSPWithTrans(string spName, DbParameter[] spParams)
{
    if (_genricTransaction == null)
    {
        throw new Exception("DataLayer exception. This method require to call 'InitTransaction' method first");
    }

    _genricCommand.CommandType = CommandType.StoredProcedure;
    _genricCommand.CommandText = spName;

    _genricCommand.Parameters.Clear();
    if (spParams != null)
    {
        foreach (DbParameter spParam in spParams)
        {
            if (spParam.Value == null)
            {
                spParam.Value = DBNull.Value;
            }
            _genricCommand.Parameters.Add(spParam);
        }
    }

    return _genricCommand.ExecuteScalar();
}

        /// <summary>
        ///  This method execute Stored procedure with or without parameters 
        /// </summary>
        /// <param name="spName"> valid stored procedure name</param>
        /// <param name="spParams"> arrays of sp params</param>
        /// <returns> datatable of resulting stored Procedure</returns>
        /// <example>
        /// 
        ///  Ebix.DataLayer.SP_PARAMS []spPrams = new Ebix.DataLayer.SP_PARAMS[2];
        ///  spPrams[0].ParamName        = "@id";
        ///  spPrams[0].ParamType        = (object)System.Data.SqlDbType.Int ;
        ///  spPrams[0].ParamValue       = (object) 2;
        ///  spPrams[0].ParamDirection   = System.Data.ParameterDirection.Input; // optional for input
        /// </example> 
        public DataTable ExecuteQuerySPForDataTable(string spName, DbParameter[] spParams)
        {
            DataTable dtResult = new DataTable();
            try
            {
                _genricCommand = _genricConnection.CreateCommand();
                OpenConnection();
                _genricCommand.CommandType = CommandType.StoredProcedure;
                _genricCommand.CommandText = spName;
                DbDataAdapter dbDataAdptr = _dbPvdrFctr.CreateDataAdapter();

                if (spParams != null)
                {
                    foreach (var spParam in spParams)
                    {
                        if (spParam.Value == null)
                        {
                            spParam.Value = DBNull.Value;
                        }
                        _genricCommand.Parameters.Add(spParam);
                    }
                }

                dbDataAdptr.SelectCommand = _genricCommand;
                dbDataAdptr.Fill(dtResult);

            }
            finally
            {
                CloseConnection();
            }
            return dtResult;
        }

        public  DbDataReader ExecuteSPForDataReader(string spName, DbParameter[] spParams)
        {
            try
            {
                _genricCommand = _genricConnection.CreateCommand();
                OpenConnection();
                _genricCommand.CommandType = CommandType.StoredProcedure;
                _genricCommand.CommandText = spName;
                if (spParams != null)
                {
                    foreach (var spParam in spParams)
                    {
                        if (spParam.Value == null)
                        {
                            spParam.Value = DBNull.Value;
                        }
                        _genricCommand.Parameters.Add(spParam);
                    }
                }

return _genricCommand.ExecuteReader(CommandBehavior.CloseConnection);
            }
            finally
            {
                _genricCommand.Dispose();
                _genricCommand = null;
            }
        }

        public int ExecuteSPNonQuery(string pstrSPName, params SqlParameter[] commandParameters)
        {
            SqlConnection lobjCon = new SqlConnection(_genricConnection.ConnectionString);
            SqlCommand lobjCmd = new SqlCommand();
            int lIntNoOfRowsAffected = 0;
            try
            {
                lobjCon.Open();
                lobjCmd.Connection = lobjCon;
                lobjCmd.CommandText = pstrSPName;
                lobjCmd.CommandType = CommandType.StoredProcedure;
                if (commandParameters != null)
                {
                    foreach (SqlParameter p in commandParameters)
                    {
                        if (p != null)
                        {
                            // Check for derived output value with no value assigned
                            if ((p.Direction == ParameterDirection.InputOutput ||
                                p.Direction == ParameterDirection.Input) &&
                                (p.Value == null))
                            {
                                p.Value = DBNull.Value;
                            }
                            Console.Write("<hr>p.Value " + p.Value + "<hr>");
                            lobjCmd.Parameters.Add(p);
                        }
                    }
                }
                lIntNoOfRowsAffected = lobjCmd.ExecuteNonQuery();
                lobjCon.Close();
            }
            catch (Exception lEx)
            {
                throw lEx;
            }
            finally
            {
                if (lobjCmd != null) lobjCmd.Dispose();
                if (lobjCon != null) lobjCon.Dispose();
            }
            return (lIntNoOfRowsAffected);
        }


        #endregion   Stored Procedure **********************

        /// <summary>
        ///  Close connection
        /// </summary>
        public void CloseConnection()
        {
            if (_genricConnection.State == ConnectionState.Open)
            {
                _genricConnection.Close();
            }

        }
        #region IDisposable Members
        /// <summary>
        ///  Dispose method for quicker garbage colloection
        /// </summary>
        /// <remarks> This must be called by using class</remarks> 
        public void Dispose()
        {
            CloseConnection(); // explicitly calls close 
            _genricConnection.Dispose();
            _genricConnection = null;

            if (_genricCommand != null)
            {
                _genricCommand.Dispose();
                _genricCommand = null;
            }

            if (_genricTransaction != null)
            {
                _genricTransaction.Dispose();
                _genricTransaction = null;
            }

            GC.SuppressFinalize(this);
        }
        # endregion


        #region destructor syntax for finalization code.
        /// <summary>
        /// destructor of class which is called if using class not called dispose
        /// </summary>
        ~DataAccess()
        {
            // Simply calling Dispose().
            Dispose();
        }
        # endregion

    }

    /// <summary>
    ///  Represent parameter details to execute Stored Procedure   
    /// </summary>
    public struct SP_PARAMS
    {
        /// <summary>
        /// Parameter name 
        /// </summary>
        public string ParamName;
        /// <summary>
        /// Parameter type as object of caller database type
        /// </summary>
        public object ParamType;
        /// <summary>
        /// Parameter value
        /// </summary>
        public object ParamValue;
        /// <summary>
        ///  Parameter direction
        /// </summary>
        public ParameterDirection ParamDirection;

    }

public class Parameters
{

public static SqlParameter Int(string name, int value)
{
return new SqlParameter {ParameterName = name, DbType = DbType.Int32, Value = value, Direction = ParameterDirection.Input};
}

public static SqlParameter Byte(string name, byte value)
{
    return new SqlParameter { ParameterName = name, DbType = DbType.Byte, Value = value, Direction = ParameterDirection.Input };
}
public static SqlParameter String(string name, string value)
{
    return new SqlParameter { ParameterName = name, DbType = DbType.String, Value = value, Direction = ParameterDirection.Input };
}

public static SqlParameter String(string name, object value)
{
    return new SqlParameter { ParameterName = name, DbType = DbType.String, Value = value, Direction = ParameterDirection.Input };
}
}
}
