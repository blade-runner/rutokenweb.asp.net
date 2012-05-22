using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace RutokenWebPlugin
{
    public class Utils

    {
        public const string StrRequestType = "X-Requested-With";
        public const string StrAjaxRequest = "XhrRutoken";
        public const string STR_URL_JAVA_PARAM = "getRutokenJavaLocal";

        private static readonly Regex REGEX = new Regex(@"LOCALIZE\(([^\))]*)\)",
                                                        RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// референс на webresource
        /// </summary>
        /// <param name="scriptname"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static void AddScriptToPage(string scriptname, Page page, Type t)
        {
            string s = page.ClientScript.GetWebResourceUrl(t,
                                                           "RutokenWebPlugin.javascript." + scriptname);
            page.ClientScript.RegisterClientScriptInclude(scriptname,
                                                          s);
        }

        /// <summary>
        /// idtojavascript
        /// </summary>
        /// <param name="ctrl">контрол(для типа)</param>
        /// <param name="jsvar">основная переменная</param>
        /// <param name="field">поле с id контрола</param>
        /// <param name="page">страница со скриптом</param>
        public static void IdToJavaScript(Control ctrl, string jsvar, string field, Page page)
        {
            page.ClientScript.RegisterStartupScript(typeof (Control), field,
                                                    jsvar + "." + field + " = rtwGID('" + ctrl.ClientID +
                                                    "');  ", true);
        }


        /// <summary>
        /// файл javascript в header
        /// </summary>
        public static void AddJavaScriptInclude(string url, Page page)
        {
            var script = new HtmlGenericControl("script");
            script.Attributes["type"] = "text/javascript";
            script.Attributes["src"] = url;
            page.Header.Controls.Add(script);
        }

        /// <summary>
        /// текст javascript в header
        /// </summary>
        /// <param name="jsText"></param>
        /// <param name="?"></param>
        public static void AddJavaScriptText(string jsText, Page page)
        {
            var script = new HtmlGenericControl("script");
            script.Attributes["type"] = "text/javascript";
            script.InnerText = jsText;
            page.Header.Controls.Add(script);
        }


        public static string GetLocalizedString(String strResourceStringID)
        {
            return (string) HttpContext.GetGlobalResourceObject("RutokenLocalText", strResourceStringID) ??
                   strResourceStringID;
        }

        /// <summary>
        /// Локализатор скрипта. Заменяет LOCALIZE(*) не ресурс
        /// </summary>
        /// <param name="text">текст для обработки</param>
        /// <returns>Текст локализованный</returns>
        public static string LocalizeScript(string text)
        {
            MatchCollection matches = REGEX.Matches(text);

            foreach (Match match in matches)
            {
                string val = match.Groups[1].Value;
                string str = GetLocalizedString(val);
                text = str != val
                           ? text.Replace(match.Value, MakeValidString(str))
                           : text.Replace(match.Value, string.Format("'LOCALIZE.{0}'", str));
            }

            return text;
        }

        /// <summary>
        /// Экранируем спецсимволы
        /// </summary>
        private static string MakeValidString(string text)
        {
            return "\"" + text.Replace("'", "\\'").Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
        }
    }
}