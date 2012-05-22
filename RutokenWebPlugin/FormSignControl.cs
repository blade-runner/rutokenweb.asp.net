using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RutokenWebPlugin
{
  public class FormSignControl : ControlAjaxed, INamingContainer
    {

      public int Port { get; set; }
      public string FormSign { get { return m_formSign; } }
      public bool FormIsCorrect { get { return m_formIsCorrect; } }

      private string m_formSign;
      private bool m_formIsCorrect;

      private SortedDictionary<string, string> fields = new SortedDictionary<string, string>(); 

      protected override void OnLoad(EventArgs e)
      {
          base.OnLoad(e);

          // проверяем hiddenfield на наличие подписи
          var rtwFormSignTxt = (HiddenField) administrationData.FindControl("rtwFormSignTxt");
          if (rtwFormSignTxt.Value.Length == 128)
          {
              m_formSign = rtwFormSignTxt.Value;
              m_formIsCorrect = CheckForm(m_formSign);
              rtwFormSignTxt.Value = string.Empty;
          }

          var rtwFormSign = (Button)administrationData.FindControl("rtwFormSign");
     //     rtwFormSign.Click += new EventHandler(rtwFormSign_Click);
          rtwFormSign.OnClientClick = "return $GrdToken.formSign();";
      }

      /// <summary>
      /// проверк4а формы
      /// </summary>
      /// <param name="mFormSign"></param>
      /// <returns></returns>
      private bool CheckForm(string mFormSign)
      {
          uint tokenid = 0;
          var rtwTokenId = (HiddenField) administrationData.FindControl("rtwTokenId");
          uint.TryParse(rtwTokenId.Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out tokenid);
          if (tokenid == 0)
          {
              return false;
          }
          // подпись, хэш, ключ
          return Rutoken.RutokenWeb.CheckSignature(mFormSign, GetFormHash(), m_tokenProcessor.GetPublicKey(tokenid));
      }

      private string GetFormHash()
      {
          GetAllFields(administrationData.Controls);

          var b = fields;
          var s = new StringBuilder();
          foreach (var field in fields)
          {
              s.Append(field.Value);
          }
          return Rutoken.RutokenWeb.GetHash(s.ToString());
      }

      private void GetAllFields(ControlCollection controls)
      {

     

          if (controls.Count > 0)
          {
              for (int i = 0; i < controls.Count; i++)
              {
                  if (controls[i] is TextBox)
                  {
                      fields.Add(((TextBox)controls[i]).ClientID,((TextBox)controls[i]).Text);
                  }
                  else if (controls[i] is DropDownList)
                  {
                      fields.Add(((DropDownList)controls[i]).ClientID,((DropDownList)controls[i]).SelectedValue);
                  }
                  else
                  {
                      if (controls[i].HasControls())
                      {
                          GetAllFields(controls[i].Controls);
                      }
                  }
              }
          }
     
      }

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

                if (m_tokenProcessor == null)
                {
                    throw  new ArgumentNullException("ITokenProcessor","object not defined");
                }

                administrationData = new AdministrationData();
                Template.InstantiateIn(administrationData);

                // javascript hash
                Utils.AddScriptToPage("hash256.js", Page, GetType());


       
                        EnsureFormSignControls();
               
           


                Controls.Add(administrationData);
            }
            else // разметка по умолчанию
            {
                throw new Exception("define Template for Form sign controller!");
            }
        }

        private void EnsureFormSignControls()
        {


                 HiddenField rtwTokenId = new HiddenField { ID = "rtwTokenId", ClientIDMode = ClientIDMode.Static };
           administrationData.Controls.Add(rtwTokenId);

            HiddenField rtwFormSignTxt = new HiddenField { ID = "rtwFormSignTxt", ClientIDMode = ClientIDMode.Static };
           administrationData.Controls.Add(rtwFormSignTxt);

        //   var rtwUsers = (Literal)administrationData.FindControl("rtwUsers");
           var rtwAjaxImg = (Image)administrationData.FindControl("rtwAjaxImg");
            var rtwFormFields = (Panel) administrationData.FindControl("rtwFormFields");
            var rtwFormSign = (Button) administrationData.FindControl("rtwFormSign");
            var rtwErrorMessage = (Label)administrationData.FindControl("rtwErrorMessage");
            var rtwMessage = (Label)administrationData.FindControl("rtwMessage");
            if (rtwFormFields == null || rtwFormSign == null || rtwErrorMessage == null || rtwMessage == null ||/* rtwUsers == null || */rtwAjaxImg == null)
            {
                throw new ArgumentException(
                    "Template must contain Panel with clientId = rtwFormFields and Button with clientId = rtwFormSign, Template must contain all of controls:  rtwMessage, rtwErrorMessage, rtwAjaxImg");
            }
            rtwFormFields.ClientIDMode = rtwFormSign.ClientIDMode = rtwErrorMessage.ClientIDMode = rtwMessage.ClientIDMode/*= rtwUsers.ClientIDMode */= rtwAjaxImg.ClientIDMode = ClientIDMode.Static;
          //  rtwUsers.EnableViewState = false;
         //   rtwUsers.Text = "<select id=\"rtwUsers\"></select>";
            List<TextBox> formFieldsList = GetAllTextBoxes(rtwFormFields.Controls);
            if (formFieldsList.Count == 0)
            {
                throw new ArgumentException("Template must contain at least 1 TextBox");
            }

            // тип контрола, для яваскрипта
            Page.ClientScript.RegisterStartupScript(typeof(Control), "controlType",
                                                    string.Format("{0}.controlType = 'FormSign'; {0}.texts={{}}; {0}.rtwUser = '{1}#%#{2}{3}';",
                                                                  JScontrolVar, m_tokenProcessor.GetUserName(), HttpContext.Current.Request.Url.Host,
                    HttpContext.Current.Request.Url.Port != 80 ? ":" + Port : string.Empty)
                                                    , true);

            // селект списка логинов
          //  Utils.IdToJavaScript(rtwUsers, JScontrolVar, "rtwUsers", Page);
           
            // панель с текстбоксами
            Utils.IdToJavaScript(rtwFormFields, JScontrolVar, "rtwFormFields", Page);

            // кнопка подписи
            Utils.IdToJavaScript(rtwFormSign, JScontrolVar, "rtwFormSign", Page);

            // скрытое поле с подписью формы
            Utils.IdToJavaScript(rtwFormSignTxt, JScontrolVar, "rtwFormSignTxt", Page);

            // скрытое поле tokenid
            Utils.IdToJavaScript(rtwTokenId, JScontrolVar, "rtwTokenId", Page);

            // контрол для вывода текста
            Utils.IdToJavaScript(rtwMessage, JScontrolVar, "rtwMessage", Page);

            // контрол для вывода текста
            Utils.IdToJavaScript(rtwErrorMessage, JScontrolVar, "rtwErrorMessage", Page);

            // картинка аякс запроса
            Utils.IdToJavaScript(rtwAjaxImg, JScontrolVar, "rtwAjaxImg", Page);

            // элемент со всеми контролами
            Utils.IdToJavaScript(this, JScontrolVar, "rtwAll", Page);
        }

      private void rtwFormSign_Click(object sender, EventArgs e)
      {
          var rtwFormFields = (Panel)administrationData.FindControl("rtwFormFields");
          List<TextBox> formFieldsList = GetAllTextBoxes(rtwFormFields.Controls);

      }

      /// <summary>
        /// находим все контролы типа TextBox
        /// </summary>
        /// <param name="controls"></param>
        /// <returns></returns>
        private List<TextBox> GetAllTextBoxes(ControlCollection controls)
        {
            List<TextBox> list = new List<TextBox>();

            if (controls.Count > 0)
            {
                for (int i = 0; i < controls.Count; i++)
                {
                    if (controls[i] is TextBox)
                    {
                        list.Add((TextBox)controls[i]);
                    }
                    else
                    {
                        list.AddRange(GetAllTextBoxes(controls[i].Controls));
                    }
                }
            }
            return list;
        }

        public override void DataBind()
        {
            CreateChildControls();
            ChildControlsCreated = true;
            base.DataBind();
        }
    }
}
