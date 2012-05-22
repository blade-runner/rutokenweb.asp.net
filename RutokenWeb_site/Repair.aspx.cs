using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RutokenWebSite
{
    public partial class Repair : System.Web.UI.Page
    {

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            tokenlogin.SetRequired(new CustomTokenProcessor(), "/Admin/");
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}