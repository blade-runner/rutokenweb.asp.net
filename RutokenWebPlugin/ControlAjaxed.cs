using System;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace RutokenWebPlugin
{
    public class ControlAjaxed : WebControl
    {
        protected AdministrationData administrationData;

        /// <summary>
        /// параметры аякс запроса
        /// </summary>
        /// <summary>
        /// Объект с интерфейсом для работы с токеном. Исключения от методов объекта обрабатываются в try catch только при аякс запросах!
        /// </summary>
        protected ITokenProcessor m_tokenProcessor;

        [TemplateContainer(typeof (AdministrationData)), TemplateInstance(TemplateInstance.Single)]
        public virtual ITemplate Template { get; set; }



        /// <summary>
        /// id объекта плагина на странице
        /// </summary>
        public string JStokenObjectID { get; set; }



        #region Дефолтные значения

        protected const string JScontrolVar = "$grd_ctrls";
        private const string STR_TOKEN_OBJECT_ID = "cryptoPlugin";


        #endregion

        /// <summary>
        /// процессор для работы с токеном
        /// </summary>
        public void SetRequired(ITokenProcessor processor, string successurl)
        {
            m_tokenProcessor = processor;

            HttpSessionState session = HttpContext.Current.Session;
            if (session != null)
            {
                if (session["TokenProcessor"] == null)
                {
                    session["TokenProcessor"] = processor;
                }

                session["SuccessUrl"] = successurl;
            }
        }

        public void SetOnSuccessAuth(EventHandler onauth)
        {
            HttpSessionState session = HttpContext.Current.Session;
            if (session != null)
            {
                session["OnSuccessAuth"] = onauth;
            }
        }

        /// <summary>
        /// устанавливаем свойства
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);


            if (string.IsNullOrEmpty(JStokenObjectID))
            {
                JStokenObjectID = STR_TOKEN_OBJECT_ID;
            }



            Page.ClientScript.RegisterStartupScript(typeof (Control), string.Empty,
                                                    string.Format(
                                                        "function rtwGID (id) {{ return document.getElementById(id); }}  var {0} = {{}}; /* {1} */",
                                                        JScontrolVar, Visible), true);


            Page.ClientScript.RegisterClientScriptInclude("jslocal",
                                                          string.Format("{1}/rutokenweb/ajax.rtw?{0}=1",
                                                                        Utils.STR_URL_JAVA_PARAM,
                                                                        HttpContext.Current.Request.ApplicationPath == "/" ? string.Empty : HttpContext.Current.Request.ApplicationPath));
        }





        /// <summary>
        /// Если аякс запрос - перехватываем здесь
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {


            base.OnLoad(e);
            EnsureChildControls();


            // плагин
            EnsureRutokenPlugin();
        }


        /// <summary>
        /// добавления разметки плагина на страницу
        /// </summary>
        private void EnsureRutokenPlugin()
        {
            Utils.AddScriptToPage("rutokenweb.js", Page, GetType());

            //var rtObject = new HtmlGenericControl("object") {ClientIDMode = ClientIDMode.Static, ID = JStokenObjectID};
            //rtObject.Attributes.Add("type", "application/x-rutoken");
            //rtObject.Attributes.Add("width", "0");
            //rtObject.Attributes.Add("height", "0");

            //var rtParam = new HtmlGenericControl("param") {TagName = "onload"};
            //rtParam.Attributes.Add("value", "pluginit");
            //rtObject.Controls.Add(rtParam);

         


            // ищем контрол с возможностью добавить и кидаем объект туда
            bool bControlAdded = false;
            if (Page.Form == null)
            {
                throw new Exception("define 'Form' tag on page!");
            }
            foreach (PlaceHolder control in Page.Form.Controls.OfType<PlaceHolder>())
            {
               // (control).Controls.Add(rtObject);
                bControlAdded = true;
                break;
            }
            if (!bControlAdded)
            {
                throw new Exception("define an empty 'PlaceHolder' tag after the tag 'Form'");
            }

            // объект токена
         //   Utils.IdToJavaScript(rtObject, JScontrolVar, "token", Page);

            // объект с настройками
            Page.ClientScript.RegisterStartupScript(typeof (Control), "settings",
                                                    string.Format(
                                                        "{0}.settings = {{}}; {0}.settings.mainurl = '{1}/rutokenweb/ajax.rtw';",
                                                        JScontrolVar, HttpContext.Current.Request.ApplicationPath == "/" ? string.Empty : HttpContext.Current.Request.ApplicationPath)
                                                    , true);
        }
    }

    /// <summary>
    /// данные для темплэйта
    /// можно байндить данные
    /// </summary>
    public class AdministrationData : WebControl, INamingContainer
    {
    }
}