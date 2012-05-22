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
            if (Template != null) // ����� ��������
            {
                Controls.Clear();


                administrationData = new AdministrationData();
                Template.InstantiateIn(administrationData);

                // javascript hash
                Utils.AddScriptToPage("hash256.js", Page, GetType());


                //------------------------------ login template logic 
                // ���� �������� � ��������� ��� ��� ����
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
            else // �������� �� ���������
            {
                throw new Exception("define Template for Login controller!");
            }

            //-------------------------------------
        }


        /// <summary>
        /// �������� ��������� ����� ������ � ���������� ���������� ������� �� ��������
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

            // ��� ��������, ��� ����������
            Page.ClientScript.RegisterStartupScript(typeof (Control), "controlType",
                                                    string.Format("{0}.controlType = 'Login'; {0}.texts={{}};",
                                                                  JScontrolVar)
                                                    , true);


            // ������ ������ �������
            Utils.IdToJavaScript(rtwUsers, JScontrolVar, "rtwUsers", Page);

            // ������ �����
            Utils.IdToJavaScript(rtwLogin, JScontrolVar, "rtwLogin", Page);

            // ������� ��� ������ ������
            Utils.IdToJavaScript(rtwMessage, JScontrolVar, "rtwMessage", Page);

            // ������� ��� ������ ������
            Utils.IdToJavaScript(rtwErrorMessage, JScontrolVar, "rtwErrorMessage", Page);

            // �������� ���� �������
            Utils.IdToJavaScript(rtwAjaxImg, JScontrolVar, "rtwAjaxImg", Page);

            // ������� �� ����� ����������
            Utils.IdToJavaScript(this, JScontrolVar, "rtwAll", Page);
        }

        /// <summary>
        /// �������� ��������� ����� �������������� � ���������� ���������� ������� �� ��������
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

            rtwRepair.MaxLength = 79; // ������ ��������� ����� �� �������� ��������������

            // ��� ��������, ��� ����������
            Page.ClientScript.RegisterStartupScript(typeof (Control), "controlType",
                                                    string.Format(
                                                        "{0}.controlType = 'Remember';{0}.texts={{}};{0}.repair = true;",
                                                        JScontrolVar), true);


            // ����� ��� ��������������
            Utils.IdToJavaScript(rtwRepairUser, JScontrolVar, "rtwRepairUser", Page);

            // ������ ��������������
            Utils.IdToJavaScript(rtwRepairBtn, JScontrolVar, "rtwRepairBtn", Page);

            // ���� ����� ��������������
            Utils.IdToJavaScript(rtwRepair, JScontrolVar, "rtwRepair", Page);

            // ������� ��� ������ ������
            Utils.IdToJavaScript(rtwMessage, JScontrolVar, "rtwMessage", Page);

            // ������� ��� ������ ������
            Utils.IdToJavaScript(rtwErrorMessage, JScontrolVar, "rtwErrorMessage", Page);

            // �������� ���� �������
            Utils.IdToJavaScript(rtwAjaxImg, JScontrolVar, "rtwAjaxImg", Page);

            // ������� �� ����� ����������
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