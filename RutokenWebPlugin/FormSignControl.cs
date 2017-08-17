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
      private const string STR_RND = "rndRutokenValue";
      private string m_formSign;
      private bool m_formIsCorrect;

      private SortedDictionary<string, string> fields = new SortedDictionary<string, string>();

      private bool bUsePimPad = false;

      protected override void OnLoad(EventArgs e)
      {
          base.OnLoad(e);

        //  var hash = GetFormHash();


          // проверяем hiddenfield на наличие подписи
          var rtwFormSignTxt = (HiddenField) administrationData.FindControl("rtwFormSignTxt");
          if (rtwFormSignTxt.Value.Length == 128 || rtwFormSignTxt.Value.Length == 64)
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
      /*    var tokensession = HttpContext.Current.Session["currentTokenId"];
          if (tokensession== null)
          {
              return false;
          }
          else
          {
              tokenid = (uint) tokensession;
          }*/
          // подпись, хэш, ключ
          var key = m_tokenProcessor.GetPublicKey(tokenid);

          return Rutoken.RutokenWeb.CheckSignature(mFormSign, GetFormHash(), m_tokenProcessor.GetPublicKey(tokenid));
      }

      


      private string GetFormHash()
      {
          GetAllFields(administrationData.Controls);

          var b = fields;
          var s = new StringBuilder();
          foreach (var field in fields)
          {
              s.Append("<N>").Append(field.Key).Append("<V>").Append(field.Value);
          }
          s.Append("<!>").Append(HttpContext.Current.Session[STR_RND]);

          HttpContext.Current.Session[STR_RND] = null;


         // var rtwPinPadUse = (CheckBox)administrationData.FindControl("rtwPinPadUse");

          bUsePimPad = true;// rtwPinPadUse.Checked;

  string hx = bUsePimPad ? "<!PINPADFILE RU>" + s.ToString() : s.ToString();
        /*  if (bUsePimPad)
          {
              

          }
          else
          {*/
              var hxh = string.Join(string.Empty, hx.ToCharArray().Select(ch => ((int)ch).ToString("X")).ToArray());
              //sb.Append(string.Join(" ", sIn.ToCharArray().Select(b => ((int)b).ToString("X")).ToArray()));

              var ba = Encoding.GetEncoding(1251).GetBytes(hx).ToArray();
              StringBuilder sb = new StringBuilder(ba.Length * 2);
              foreach (byte bt in ba)
              {
                  sb.AppendFormat("{0:x2}", bt);
              }
              //-------------------------------------------------------------------------------------


              string ret = string.Empty;
          if (bUsePimPad)
          {
              var hfSignData = ((HiddenField) administrationData.FindControl("rtwFormSignData")).Value;
              var pinpadtext = GetPinPadText(hfSignData).Where(s1 => !string.IsNullOrEmpty(s1)).ToArray();

              byte[] strBytes = new byte[pinpadtext.Count()];

              for (int i = 0; i < pinpadtext.Count(); i++)
              {
                  strBytes[i] = Byte.Parse(pinpadtext[i], NumberStyles.HexNumber);
              }

              string res = Encoding.GetEncoding(1251).GetString(strBytes);

          

        //      string test_data =
         //         "<!PINPADFILE RU><N>Валюта<V>RUR<N>Получ<V>Алиса<N>Счет<V>Общий<N>Сумма<V>10<N>Плат<V>Боб";

           //   var ret1 = Rutoken.RutokenWeb.GetHash(test_data, true);

              ret = Rutoken.RutokenWeb.GetHash(hx, true);
              return ret;

          }

              //var ret =  Rutoken.RutokenWeb.GetHash(sb.ToString());
               ret = Rutoken.RutokenWeb.GetHash(hx,true);

              return ret;
          //}

        

         
       //   return sb.ToString();
      }

      private IEnumerable<string> GetPinPadText(string str)
      {
          int index = 0;
          int maxLength = 2;
          while (index + maxLength <= str.Length)
          {
              yield return str.Substring(index, maxLength);
              index += maxLength;
          }

          yield return str.Substring(index);
      }

      private void GetAllFields(ControlCollection controls)
      {

     

          if (controls.Count > 0)
          {
              for (int i = 0; i < controls.Count; i++)
              {
                  if (controls[i] is rtwTextBox)
                  {
                      fields.Add(((rtwTextBox)controls[i]).PinPadField,((rtwTextBox)controls[i]).Text);
                  }
                  else if (controls[i] is rtwDropDownList)
                  {
                      fields.Add(((rtwDropDownList)controls[i]).PinPadField,((rtwDropDownList)controls[i]).SelectedValue);
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
                    throw new ArgumentNullException("ITokenProcessor", "ITokenProcessor object not defined");
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
          


         
          //  throw new Exception(sss.ToString());


                 HiddenField rtwTokenId = new HiddenField { ID = "rtwTokenId", ClientIDMode = ClientIDMode.Static };
           administrationData.Controls.Add(rtwTokenId);

            HiddenField rtwFormSignTxt = new HiddenField { ID = "rtwFormSignTxt", ClientIDMode = ClientIDMode.Static };
           administrationData.Controls.Add(rtwFormSignTxt);


           HiddenField rtwFormSignData = new HiddenField { ID = "rtwFormSignData", ClientIDMode = ClientIDMode.Static };
           administrationData.Controls.Add(rtwFormSignData);


        //   var rtwUsers = (Literal)administrationData.FindControl("rtwUsers");
           var rtwAjaxImg = (Image)administrationData.FindControl("rtwAjaxImg");
            var rtwFormFields = (Panel) administrationData.FindControl("rtwFormFields");
            var rtwFormSign = (Button) administrationData.FindControl("rtwFormSign");
            var rtwFormSend = (Button)administrationData.FindControl("rtwFormSend");
            var rtwErrorMessage = (Label)administrationData.FindControl("rtwErrorMessage");
            var rtwMessage = (Label)administrationData.FindControl("rtwMessage");
          //  var rtwPinPadUse = (CheckBox)administrationData.FindControl("rtwPinPadUse");
            if (rtwFormFields == null || rtwFormSign == null || rtwFormSend == null || rtwErrorMessage == null || rtwMessage == null /*|| rtwPinPadUse == null*/ || rtwAjaxImg == null)
            {
                throw new ArgumentException(
                    "Template must contain Panel with clientId = rtwFormFields, Buttons with clientId = rtwFormSign and rtwFormSend, Template must contain all of controls:  rtwMessage, rtwErrorMessage, rtwAjaxImg");
            }
            rtwFormFields.ClientIDMode = rtwFormSign.ClientIDMode = rtwFormSend.ClientIDMode/* = rtwPinPadUse.ClientIDMode*/ = rtwErrorMessage.ClientIDMode = rtwMessage.ClientIDMode/*= rtwUsers.ClientIDMode */= rtwAjaxImg.ClientIDMode = ClientIDMode.Static;
          //  rtwUsers.EnableViewState = false;
         //   rtwUsers.Text = "<select id=\"rtwUsers\"></select>";
            List<TextBox> formFieldsList = GetAllTextBoxes(rtwFormFields.Controls);
            if (formFieldsList.Count == 0)
            {
                throw new ArgumentException("Template must contain at least 1 TextBox");
            }

            string postBackJavascript = Page.ClientScript.GetPostBackEventReference(rtwFormSign, "");

            rtwFormSend.Style["display"] = "none";

            // тип контрола, для яваскрипта
            Page.ClientScript.RegisterStartupScript(typeof(Control), "controlType",
                                                    string.Format("{0}.controlType = 'FormSign'; {0}.texts={{}}; {0}.rtwUser = '{1}#%#{2}{3}'; {0}.mainform = document.getElementById('{4}'); function rtwSendSignedFrom(){{ {5}}};",
                                                                  JScontrolVar, m_tokenProcessor.GetUserName(), HttpContext.Current.Request.Url.Host,
                    HttpContext.Current.Request.Url.Port != 80 ? ":" + Port : string.Empty,Page.Form.ClientID,postBackJavascript)
                                                    , true);


         //   rtwPinPadUse.Checked = true;
            // селект списка логинов
          //  Utils.IdToJavaScript(rtwUsers, JScontrolVar, "rtwUsers", Page);

            // чекбокс использовать пинпад
          //  Utils.IdToJavaScript(rtwPinPadUse, JScontrolVar, "rtwPinPadUse", Page);

            // панель с текстбоксами
            Utils.IdToJavaScript(rtwFormFields, JScontrolVar, "rtwFormFields", Page);

            // кнопка подписи
            Utils.IdToJavaScript(rtwFormSign, JScontrolVar, "rtwFormSign", Page);

            // кнопка отправки платежки
            Utils.IdToJavaScript(rtwFormSend, JScontrolVar, "rtwFormSend", Page);

            // скрытое поле с подписью формы
            Utils.IdToJavaScript(rtwFormSignTxt, JScontrolVar, "rtwFormSignTxt", Page);

            // скрытое поле с данными на подпись
            Utils.IdToJavaScript(rtwFormSignData, JScontrolVar, "rtwFormSignData", Page);

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
