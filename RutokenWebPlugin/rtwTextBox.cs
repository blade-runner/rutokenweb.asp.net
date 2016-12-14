using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace RutokenWebPlugin
{
   public class rtwTextBox :TextBox 
    {
      //  public int OrderNumber { get; set; }
        public string PinPadField
        {
            get { return string.IsNullOrEmpty(pinpadfield) ? this.ClientID : pinpadfield; }
            set { pinpadfield = value; }
        }

        private string pinpadfield;


        protected override void AddAttributesToRender(System.Web.UI.HtmlTextWriter writer)
        {
            base.AddAttributesToRender(writer);



            if (!string.IsNullOrEmpty(PinPadField))
            {
                writer.AddAttribute("PinPadField", PinPadField);
            }
          //  writer.AddAttribute("OrderNumber",OrderNumber.ToString());
        }
    }
}
