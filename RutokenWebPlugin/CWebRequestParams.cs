using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;

namespace RutokenWebPlugin
{
    /// <summary>
    /// инкапсулирует запрос ajax. в свойствах все параметры
    /// </summary>
    public class CMessageRequest
    {
        private const string ChrAjaxNoCache = "_";

        /// <summary>
        /// разбираем что прислали
        /// </summary>
        /// <param name="context"></param>
        public CMessageRequest(HttpContext context)
        {
            NameValueCollection q = context.Request.QueryString;
            foreach (string key in q.AllKeys.Where(key => key != ChrAjaxNoCache))
            {
                try
                {
                    PropertyInfo das = GetType().GetProperty(key);
                    Type t = das.PropertyType;
                    das.SetValue(this, Convert.ChangeType(q[key], t), null);
                }
                catch (Exception)
                {
                    notValid = true;
                    break;
                }
            }
        }

        public string act { get; private set; } // метод для выполнения
        public string user { get; private set; } // id токена текст
        public string sign { get; private set; } // подпись
        public string urnd { get; private set; } // хэш сгенеренный пользователем
        public string login { get; private set; } // login
        public bool repair { get; private set; } // восттановление или обычный логин
        public string rkey { get; private set; } // repair key
        public string pkey { get; private set; } // public key
        public bool to { get; private set; } // вкл/выкл токена

        public bool notValid { get; private set; } // запрос не валиден, возможно ошибка в параметрах
        public uint Tokenid
        {
            get
            {
                uint tokenid;
                return uint.TryParse(user, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out tokenid)
                           ? tokenid
                           : tokenid;
            }
        }

        public string err { get; set; }
    }
}