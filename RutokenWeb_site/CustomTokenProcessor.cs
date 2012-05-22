using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using RutokenWebPlugin;


namespace RutokenWebSite
{
    public class CustomTokenProcessor : ITokenProcessor
    {
        public bool IsUserAuthenticated()
        {
            return HttpContext.Current.User.Identity.IsAuthenticated;
        }

        public string GetUserName()
        {
            return HttpContext.Current.User.Identity.Name;
        }

        public void RegisterToken(uint tokenid, string repairKey, string publicKey)
        {
            // позволяем привязку только новых или своих токенов
            SqlHelper sql = new SqlHelper();
            SqlDataReader r = sql.GetReaderBySQL("declare @ClientId int select @ClientId = id from Users where Name = '" + HttpContext.Current.User.Identity.Name + "' " +
"if not exists (select Id from UserMetaData where TokenId = " + tokenid + " and ClientId != @ClientId)" +
" begin delete from UserMetaData where TokenId =  " + tokenid + " insert into UserMetaData (ClientId,TokenId,PublicKey,RepairKey,TokenActive) values " +
   "   (@ClientId," + tokenid + ",'" + publicKey + "','" + repairKey + "',1) select 1 end else RAISERROR ('Чужой токен',16,1)");
            r.Read();
        }

        public void UnregisterToken(uint tokenid)
        {
            // позволяем отвязку только своих токенов

            SqlHelper sql = new SqlHelper();

            SqlDataReader r = sql.GetReaderBySQL("declare @ClientId int select @ClientId = id from Users where Name = '" + HttpContext.Current.User.Identity.Name + "' " +
"if not exists (select Id from UserMetaData where TokenId = " + tokenid + " and ClientId != @ClientId)" +
" begin delete from UserMetaData where TokenId =  " + tokenid + "  select 1 end else RAISERROR ('Чужой токен',16,1)");
            r.Read();
        }

        public bool IsTokenRegistered(uint tokenid)
        {
            SqlHelper sql = new SqlHelper();

            SqlDataReader r = sql.GetReaderBySQL("select PublicKey,RepairKey from UserMetaData where TokenId = " + tokenid);
            bool b = false;
            if (r.Read())
            {
                b = !r.IsDBNull(0) && !r.IsDBNull(1);
            }
            return b;
        }

        public void SwitchToken(uint tokenid, bool enabled)
        {
            SqlHelper sql = new SqlHelper();
            SqlDataReader r = sql.GetReaderBySQL("if exists (select Id from UserMetaData join Users on UserMetaData.ClientId = Users.id where Users.Name != '" + HttpContext.Current.User.Identity.Name +
                "' and TokenId = " + tokenid + ") RAISERROR ('Чужой токен',16,1) else "
                + "update UserMetaData set TokenActive = '" + enabled + "' where TokenId = " + tokenid);
            r.Read();
        }

        public bool IsTokenSwitchedOn(uint tokenid)
        {
            SqlHelper sql = new SqlHelper();
            SqlDataReader r = sql.GetReaderBySQL("select TokenActive from UserMetaData where TokenId = " + tokenid + "");

            bool b = false;
            while (r.Read())
            {
                b = !r.IsDBNull(0) && r.GetBoolean(0);
            }
            return b;
        }

        public bool UserCanBeAuthenticated(uint tokenid)
        {
            SqlHelper sql = new SqlHelper();
            SqlDataReader r = sql.GetReaderBySQL("select a.id,TokenId from Users a join UserMetaData b on a.id = b.ClientId  where TokenId =" + tokenid + " and TokenActive = 1");

            bool b = false;
            if (r.Read())
            {
                b = !r.IsDBNull(0) && !r.IsDBNull(1);
            }
            return b;
        }

        public string GetPublicKey(uint nTokenID)
        {
            SqlHelper sql = new SqlHelper();
            SqlDataReader r = sql.GetReaderBySQL("select PublicKey from UserMetaData where TokenId =" + nTokenID);

            string key = string.Empty;
            while (r.Read())
            {
                key = r.GetString(0);
            }
            return key;
        }

        public string GetRepairKey(string login)
        {
            SqlHelper sql = new SqlHelper();
            SqlDataReader r = sql.GetReaderBySQL("select RepairKey from UserMetaData a join Users b on a.ClientId = b.id   where Name = '" + login + "'");

            string key = string.Empty;
            while (r.Read())
            {
                key = r.GetString(0);
            }
            return key;
        }


        public bool SetUserAuthenticated(uint tokenid, string strSignature, string strSource)
        {
            SqlHelper sql = new SqlHelper();
            SqlDataReader r = sql.GetReaderBySQL("select Name from UserMetaData a join Users b on a.ClientId = b.id   where TokenId = " + tokenid);

            string login = string.Empty;
            while (r.Read())
            {
                login = r.GetString(0);
            }


            FormsAuthentication.SetAuthCookie(login, false);

            return true;
        }

        public List<uint> GetUserTokens(string login)
        {
            var tokens = new List<uint>();
            SqlHelper sql = new SqlHelper();
            SqlDataReader r = sql.GetReaderBySQL("select TokenId from UserMetaData a join Users b on a.ClientId = b.id   where Name = '" + login + "'");


            while (r.Read())
            {
                tokens.Add(Convert.ToUInt32(r.GetValue(0)));
            }
            return tokens;
        }
    }
}