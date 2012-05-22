using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;


namespace RutokenWebSite
{
    public class SqlHelper
    {
        private string mstr_ConnectionString;
        private SqlConnection mobj_SqlConnection;
        private SqlCommand mobj_SqlCommand;
        private int mint_CommandTimeout = 30;

        public enum ExpectedType
        {
            StringType = 0,
            NumberType = 1,
            DateType = 2,
            BooleanType = 3,
            ImageType = 4
        }

        public SqlHelper()
        {
            try
            {

                mstr_ConnectionString = ConfigurationManager.ConnectionStrings["SqlServerConnection"].ToString();
                mobj_SqlConnection = new SqlConnection(mstr_ConnectionString);
                mobj_SqlCommand = new SqlCommand { CommandTimeout = mint_CommandTimeout, Connection = mobj_SqlConnection };

            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка инициализации базы " + Environment.NewLine + ex.Message);
            }
        }

        public void Dispose()
        {
            try
            {

                if (mobj_SqlConnection != null)
                {
                    if (mobj_SqlConnection.State != ConnectionState.Closed)
                    {
                        mobj_SqlConnection.Close();
                    }
                    mobj_SqlConnection.Dispose();
                }


                if (mobj_SqlCommand != null)
                {
                    mobj_SqlCommand.Dispose();
                }

            }

            catch (Exception ex)
            {
                throw new Exception( Environment.NewLine + ex.Message);
            }

        }

        public void CloseConnection()
        {
            if (mobj_SqlConnection.State != ConnectionState.Closed) mobj_SqlConnection.Close();
        }



        public SqlDataReader GetReaderBySQL(string strSQL)
        {
            mobj_SqlConnection.Open();
            try
            {
                SqlCommand myCommand = new SqlCommand(strSQL, mobj_SqlConnection);
                return myCommand.ExecuteReader();
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }
        }


    }
}