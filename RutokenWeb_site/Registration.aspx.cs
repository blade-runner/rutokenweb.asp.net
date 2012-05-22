using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RutokenWebSite
{
    public partial class Registration : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }



        protected void Register(object sender, EventArgs e)
        {
          
            SqlHelper sql = new SqlHelper();
            SqlDataReader rCheck = sql.GetReaderBySQL("select Name from Users where Name ='" + login.Text + "'");
            if (rCheck.HasRows)
            {
                lblResult.Text = "Пользователь с логином " + login.Text + " уже зарегистрирован.";

            }
            else
            {
                sql = new SqlHelper();
                SqlDataReader rInsert =
                    sql.GetReaderBySQL("insert into Users(Name,Password) values ('" + login.Text + "','" + password.Text +
                                       "')");
                lblResult.Text = "Вы можете войти с указанными логином и паролем и привязать токен в личном кабинете.";

            }
            sql.CloseConnection();
        }
    }
}