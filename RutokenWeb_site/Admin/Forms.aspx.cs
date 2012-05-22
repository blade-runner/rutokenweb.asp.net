using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RutokenWebSite.Admin
{
    public partial class Forms : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            var processor = new CustomTokenProcessor();
          formSignControl.SetRequired(processor, "/");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// обработчик кнопки проверки подписи формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CheckFormSign(object sender, EventArgs e)
        {
            /*
             Проверка происходит в контроле, здесь в методе можем помотреть уже результат
             */

            // это признак что форма подписана нужным ключем
            lblResult.Text = "это признак что форма подписана нужным ключем: formSignControl.FormIsCorrect = <b>" + formSignControl.FormIsCorrect + "</b></br>";

            // это текст с подписью
            lblResult.Text += "это текст с подписью: formSignControl.FormSign = <b>" + formSignControl.FormSign + "</b>";
        }
    }
}