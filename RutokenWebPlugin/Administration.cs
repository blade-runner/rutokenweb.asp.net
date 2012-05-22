using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RutokenWebPlugin
{
    public class Administration : ControlAjaxed, INamingContainer
    {
        #region .ctor

        public Administration(ITokenProcessor processor)
        {
            m_tokenProcessor = processor;
        }

        public Administration()
        {
        }

        #endregion

        public int Port { get; set; }

        public override ControlCollection Controls
        {
            get
            {
                EnsureChildControls();
                return base.Controls;
            }
        }


        protected override void CreateChildControls()
        {
            if (Template != null) // задан темплэйт
            {
                Controls.Clear();


                administrationData = new AdministrationData();
                Template.InstantiateIn(administrationData);


                // ищем контролы и проверяем что все есть
                var rtwEnable = (GridView) administrationData.FindControl("rtwEnable");
                var rtwConnect = (Button) administrationData.FindControl("rtwConnect");
                var rtwErrorMessage = (Label) administrationData.FindControl("rtwErrorMessage");
                var rtwMessage = (Label) administrationData.FindControl("rtwMessage");
                var rtwAjaxImg = (Image) administrationData.FindControl("rtwAjaxImg");

                if (rtwEnable == null || rtwConnect == null || rtwMessage == null ||
                    rtwErrorMessage == null || rtwAjaxImg == null)
                {
                    throw new ArgumentException(
                        "Template must contain all of controls with ids: rtwEnable, trwConnect, rtwReconnect, rtwRemove, rtwMessage, rtwErrorMessage, rtwCheck, rtwAjaxImg");
                }

                // устанавливаем видимость контролов
                rtwAjaxImg.Attributes.CssStyle["display"] = "none";

                rtwConnect.ClientIDMode =
                    rtwEnable.ClientIDMode =
                    rtwMessage.ClientIDMode =
                    rtwErrorMessage.ClientIDMode = rtwAjaxImg.ClientIDMode = ClientIDMode.Static;
                rtwConnect.OnClientClick = "return false;";

                rtwEnable.RowDataBound += TokenRowDataBound;
                // список токенов
                rtwEnable.DataSource = m_tokenProcessor.GetUserTokens(m_tokenProcessor.GetUserName());
                rtwEnable.DataBind();

                // здесь добавляем на страницу имя пользователя, для создания контейнера
                Page.ClientScript.RegisterStartupScript(typeof (Control), "rtwEnable", string.Format(
                    "if ( rtwGID('{1}') != null) {{ {0}.rtwEnable = rtwGID('{1}'); /* {0}.rtwEnable.b = document.getElementsByName(rtwGID('{1}').getElementsByTagName('input')[0].getAttribute('name')); */ {0}.rtwUser = '{2}#%#{3}{4}';{0}.controlType = '{5}';}}",
                    JScontrolVar, rtwEnable.ClientID, m_tokenProcessor.GetUserName(),
                    HttpContext.Current.Request.Url.Host,
                    HttpContext.Current.Request.Url.Port != 80 ? ":" + Port : string.Empty, GetType().Name), true);


                // кнопка привязать токен
                Utils.IdToJavaScript(rtwConnect, JScontrolVar, "rtwConnect", Page);

                // контрол для вывода текста
                Utils.IdToJavaScript(rtwMessage, JScontrolVar, "rtwMessage", Page);

                // контрол для вывода текста
                Utils.IdToJavaScript(rtwErrorMessage, JScontrolVar, "rtwErrorMessage", Page);

                // картинка аякс запроса
                Utils.IdToJavaScript(rtwAjaxImg, JScontrolVar, "rtwAjaxImg", Page);

                // элемент со всеми контролами
                Utils.IdToJavaScript(this, JScontrolVar, "rtwAll", Page);
                
                Controls.Add(administrationData);
            }
            else // нет разметки по умолчанию
            {
                throw new Exception("define Template for Admin controller!");
            }
        }

        private void TokenRowDataBound(object sender, GridViewRowEventArgs e)
        {
            uint tokenid = Convert.ToUInt32(e.Row.DataItem);
            bool bSwitch = m_tokenProcessor.IsTokenSwitchedOn(tokenid);


            Label rtwEnabledToken = (Label) e.Row.FindControl("rtwEnabledToken") ?? new Label();
            rtwEnabledToken.Text = bSwitch ? "✓" : "-";

            Button rtwEnableSwitch = (Button) e.Row.FindControl("rtwEnableSwitch") ?? new Button();
            rtwEnableSwitch.Attributes["token"] = tokenid.ToString("X");
            rtwEnableSwitch.Attributes["to"] = (!bSwitch).ToString();
            rtwEnableSwitch.Attributes["act"] = "switc";
            rtwEnableSwitch.Text = bSwitch ? Utils.GetLocalizedString("rtwSetAuthOff") : Utils.GetLocalizedString("rtwSetAuthOn");

            Button rtwRemove = (Button) e.Row.FindControl("rtwRemove") ?? new Button();
            rtwRemove.Attributes["token"] = tokenid.ToString("X");
            rtwRemove.Attributes["act"] = "remove";
            rtwRemove.Text = Utils.GetLocalizedString("rtwRemoveToken");
        }


        public override void DataBind()
        {
            CreateChildControls();
            ChildControlsCreated = true;
            base.DataBind();
        }
    }
}