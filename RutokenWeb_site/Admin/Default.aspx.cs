using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RutokenWebSite.Admin
{
    public partial class Default : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            var processor = new CustomTokenProcessor();
            backoffice.SetRequired(processor, "/");
        }

        protected void Page_Load(object sender, EventArgs e)
        {


        }


    }
}