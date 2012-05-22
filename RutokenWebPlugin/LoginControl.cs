using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RutokenWebPlugin
{
    public class Login : ControlAjaxed, INamingContainer
    {
        #region ELoginType enum

        public enum ELoginType
        {
            Login,
            Remember
        }

        #endregion


        public ELoginType LoginType { get; set; }

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

                // javascript hash
                Utils.AddScriptToPage("hash256.js", Page, GetType());


                //------------------------------ login template logic 
                // ищем контролы и проверяем что все есть
                switch (LoginType)
                {
                    case ELoginType.Login:
                        EnsureLogincontrols();
                        break;
                    case ELoginType.Remember:
                        EnsureRememberControls();
                        break;
                }


                Controls.Add(administrationData);
            }
            else // разметка по умолчанию
            {
                throw new Exception("define Template for Login controller!");
            }

            //-------------------------------------
        }


        /// <summary>
        /// проверка контролов формы логина и добавление переменных скрипта на страницу
        /// </summary>
        private void EnsureLogincontrols()
        {
            var rtwUsers = (Literal) administrationData.FindControl("rtwUsers");
            var rtwLogin = (Button) administrationData.FindControl("rtwLogin");
            var rtwErrorMessage = (Label) administrationData.FindControl("rtwErrorMessage");
            var rtwMessage = (Label) administrationData.FindControl("rtwMessage");
            var rtwAjaxImg = (Image) administrationData.FindControl("rtwAjaxImg");

            if (rtwUsers == null || rtwErrorMessage == null ||
                rtwMessage == null || rtwLogin == null || rtwAjaxImg == null)
            {
                throw new ArgumentException(
                    "Template must contain all of controls: rtwUsers, rtwLogin, rtwMessage, rtwErrorMessage, rtwAjaxImg");
            }

            rtwAjaxImg.Attributes.CssStyle["display"] = "none";
            rtwUsers.EnableViewState = false;
            rtwUsers.ClientIDMode =
                rtwLogin.ClientIDMode =
                rtwErrorMessage.ClientIDMode = rtwMessage.ClientIDMode = rtwAjaxImg.ClientIDMode = ClientIDMode.Static;
            rtwUsers.Text = "<select id=\"rtwUsers\"></select>";

            // тип контрола, для яваскрипта
            Page.ClientScript.RegisterStartupScript(typeof (Control), "controlType",
                                                    string.Format("{0}.controlType = 'Login'; {0}.texts={{}};",
                                                                  JScontrolVar)
                                                    , true);


            // селект списка логинов
            Utils.IdToJavaScript(rtwUsers, JScontrolVar, "rtwUsers", Page);

            // кнопка входа
            Utils.IdToJavaScript(rtwLogin, JScontrolVar, "rtwLogin", Page);

            // контрол для вывода текста
            Utils.IdToJavaScript(rtwMessage, JScontrolVar, "rtwMessage", Page);

            // контрол для вывода текста
            Utils.IdToJavaScript(rtwErrorMessage, JScontrolVar, "rtwErrorMessage", Page);

            // картинка аякс запроса
            Utils.IdToJavaScript(rtwAjaxImg, JScontrolVar, "rtwAjaxImg", Page);

            // элемент со всеми контролами
            Utils.IdToJavaScript(this, JScontrolVar, "rtwAll", Page);
        }

        /// <summary>
        /// проверка контролов формы восстановления и добавление переменных скрипта на страницу
        /// </summary>
        private void EnsureRememberControls()
        {
            var rtwRepairUser = (TextBox) administrationData.FindControl("rtwRepairUser");
            var rtwRepairBtn = (Button) administrationData.FindControl("rtwRepairBtn");
            var rtwRepair = (TextBox) administrationData.FindControl("rtwRepair");
            var rtwErrorMessage = (Label) administrationData.FindControl("rtwErrorMessage");
            var rtwMessage = (Label) administrationData.FindControl("rtwMessage");
            var rtwAjaxImg = (Image) administrationData.FindControl("rtwAjaxImg");

            if (rtwRepairUser == null || rtwRepair == null || rtwErrorMessage == null ||
                rtwMessage == null || rtwRepairBtn == null || rtwAjaxImg == null)
            {
                throw new ArgumentException(
                    "Template maust contain all of controls: rtwRepairUser, rtwRepair, rtwRepairBtn, rtwMessage, rtwErrorMessage, rtwAjaxImg");
            }

            rtwAjaxImg.Attributes.CssStyle["display"] = "none";
            rtwRepair.EnableViewState = false;
            rtwRepairBtn.ClientIDMode =
                rtwErrorMessage.ClientIDMode =
                rtwMessage.ClientIDMode =
                rtwRepair.ClientIDMode = rtwRepairUser.ClientIDMode = rtwAjaxImg.ClientIDMode = ClientIDMode.Static;

            rtwRepair.MaxLength = 79; // формат закрытого ключа на карточке восстановления

            // тип контрола, для яваскрипта
            Page.ClientScript.RegisterStartupScript(typeof (Control), "controlType",
                                                    string.Format(
                                                        "{0}.controlType = 'Remember';{0}.texts={{}};{0}.repair = true;",
                                                        JScontrolVar), true);


            // логин для восстановления
            Utils.IdToJavaScript(rtwRepairUser, JScontrolVar, "rtwRepairUser", Page);

            // кнопка восстановления
            Utils.IdToJavaScript(rtwRepairBtn, JScontrolVar, "rtwRepairBtn", Page);

            // ввод ключа восстановления
            Utils.IdToJavaScript(rtwRepair, JScontrolVar, "rtwRepair", Page);

            // контрол для вывода текста
            Utils.IdToJavaScript(rtwMessage, JScontrolVar, "rtwMessage", Page);

            // контрол для вывода текста
            Utils.IdToJavaScript(rtwErrorMessage, JScontrolVar, "rtwErrorMessage", Page);

            // картинка аякс запроса
            Utils.IdToJavaScript(rtwAjaxImg, JScontrolVar, "rtwAjaxImg", Page);

            // элемент со всеми контролами
            Utils.IdToJavaScript(this, JScontrolVar, "rtwAll", Page);
        }


        public override void DataBind()
        {
            CreateChildControls();
            ChildControlsCreated = true;
            base.DataBind();
        }
    }
}