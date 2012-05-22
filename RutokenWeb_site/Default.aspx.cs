using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RutokenWebSite
{
    public partial class Default : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            // передаем объект с интерфейсом ITopkenProcessor и урл для пернаправления после аутентификации
            tokenlogin.SetRequired(new CustomTokenProcessor(), "/Admin/");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
     
            if (User.Identity.IsAuthenticated)
            {
                Response.Redirect(  Request.ApplicationPath + "/Admin/");
            }
        }

        protected void OnAuth(object sender, AuthenticateEventArgs e)
        {
            bool Authenticated = false;
            Authenticated = Authenticate(login.UserName, login.Password);
            e.Authenticated = Authenticated;
            
        }

        private bool Authenticate(string userName, string password)
        {
            SqlHelper sql = new SqlHelper();
            SqlDataReader r = sql.GetReaderBySQL("select Name from Users where Name='"+ userName +"' and Password='"+password+"' ");
            return r.HasRows;
        }
    }
}