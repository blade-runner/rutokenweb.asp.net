using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.Security;
using System.Web.SessionState;
using Rutoken;

namespace RutokenWebPlugin
{
    public partial class TokenAjaxHandler : IHttpHandler, IRequiresSessionState
    {
        private const string STR_RND = "rndRutokenValue";
        private const string CACHE_LOGINS = "___rtwLogins";
        private const int CACHE_EXPIRES = 3; // кэш в секундах
        private static readonly object _lock = new object();

        private static readonly Regex REGEX_KEYS = new Regex(@"[\dA-F]{128}",
                                                             RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Regex REGEX_URND = new Regex(@"[\da-f]{64}",
                                                             RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Regex REGEX_LOGIN = new Regex(@"[<>]+", RegexOptions.Singleline | RegexOptions.Compiled);
        private ITokenProcessor TokenProcessor;
        private HttpContext _mContext;
        private CMessageRequest _mRequest;
        private CMessageResponse _mResponse;
        private string _successUrl;

        #region IHttpHandler Members

        public void ProcessRequest(HttpContext context)
        {
            // если запрос скриптов
            if (context.Request.QueryString[Utils.STR_URL_JAVA_PARAM] != null)
            {
                context.Response.ContentType = "text/javascript";
                SendLocalizeScript();
                context.Response.End();
                return;
            }


            if (context.Request.Headers[Utils.StrRequestType] != Utils.StrAjaxRequest) return;
            // парсим строку запроса, создаем строго наш экземпляр
            _mRequest = new CMessageRequest(context);
            if (_mRequest.notValid)
            {
                _mResponse = new CMessageResponse(
                    _mRequest.err,
                    CMessageResponse.EMessageResponseType.Error);
            }
            if (context.Session == null)
            {
                _mResponse = new CMessageResponse("Session needs to be activated",
                                                  CMessageResponse.EMessageResponseType.Error);
            }


            // берем TokenProcessor и урл из сессии
            if (_mResponse == null &&
                ((TokenProcessor = (ITokenProcessor) context.Session["TokenProcessor"]) == null ||
                 (_successUrl = (string) context.Session["SuccessUrl"]) == null))
            {
                _mResponse =
                    new CMessageResponse(
                        "No ITokenProcessor or SuccessUrl",
                        CMessageResponse.EMessageResponseType.Error);
            }
            if (_mResponse == null)
            {
                _mContext = context;
                try
                {
                    // используем рефлексию для вызова методов по имени, переданному в запросе
                    GetType().InvokeMember(_mRequest.act, BindingFlags.InvokeMethod, null, this,
                                           new object[] {});
                }
                catch (Exception)
                {
                    _mResponse =
                        new CMessageResponse(
                            "Method error. Check request",
                            CMessageResponse.EMessageResponseType.Error);
                }
            }


            if (_mResponse == null)
                _mResponse = new CMessageResponse("Not valid request",
                                                  CMessageResponse.EMessageResponseType.Error);
            context.Response.ContentType = "application/json";
            context.Response.Write(_mResponse.ToJson());
            context.Response.End();
        }

        public bool IsReusable
        {
            get { return false; }
        }

        #endregion

        public event EventHandler OnSuccessAuth;


        /// <summary>
        /// получение случайного значения
        /// </summary>
        public void rnd()
        {
            try
            {
                if (((_mRequest.Tokenid > 0) && CheckCachedLogin(_mRequest.user) && TokenProcessor.UserCanBeAuthenticated(_mRequest.Tokenid))
                    ||
                    (_mRequest.repair)) // если восстановление не проверяем условия, отдаем число так
                {
                    string randomText = RutokenWeb.GetRandomHash();
                    _mContext.Session[STR_RND] = randomText;
                    _mResponse = new CMessageResponse(randomText, CMessageResponse.EMessageResponseType.Notify);
                }
                else
                {
                    AddLoginToCache(_mRequest.user);
                    _mResponse = new CMessageResponse(Utils.GetLocalizedString("rtwErrCantLoginByToken") + " " + _mRequest.Tokenid + " " + TokenProcessor.UserCanBeAuthenticated(_mRequest.Tokenid),
                                                      CMessageResponse.EMessageResponseType.Error);
                }
            }
            catch (Exception e)
            {
                _mResponse = new CMessageResponse(e.Message, CMessageResponse.EMessageResponseType.Error);
            }
        }


        /// <summary>
        /// Аутентификация
        /// </summary>
        public void login()
        {
            if (!CheckLoginParams())
            {
                _mResponse = new CMessageResponse(Utils.GetLocalizedString("rtwErrLogin"),
                                                  CMessageResponse.EMessageResponseType.Error);
            }
            else
            {
                try
                {
                    if ((CheckCachedLogin(_mRequest.user) && TokenProcessor.UserCanBeAuthenticated(_mRequest.Tokenid)) || _mRequest.repair)
                    {
                        // склеиваем и считаем хэш
                        string hash = RutokenWeb.GetHash(_mRequest.urnd + ":" + _mContext.Session[STR_RND]);

                        string key = _mRequest.repair
                                         ? TokenProcessor.GetRepairKey(_mRequest.login)
                                         : TokenProcessor.GetPublicKey(_mRequest.Tokenid);


                        bool iscorrect = RutokenWeb.CheckSignature(_mRequest.sign, hash, key);

                        if (iscorrect)
                        {
                            try
                            {

                                if ( _mRequest.repair)
                                {
                                    if (!string.IsNullOrEmpty(_mRequest.login))
                                    {
                                        // todo: новый метод в интерфейсе
                                        FormsAuthentication.SetAuthCookie(_mRequest.login, true);
                                    }
                                }
                                else
                                {
                                    if (TokenProcessor.SetUserAuthenticated(_mRequest.Tokenid, _mRequest.sign, _mRequest.urnd))
                                    {
                                        //TokenProcessor.UnregisterTokenByLogin(_mRequest.login);
                                    }
                                // если было восстановление  - удаляем записи о токене
                                }


                                _mContext.Session["currentTokenId"] = _mRequest.Tokenid;


                                if ((OnSuccessAuth = (EventHandler) _mContext.Session["OnSuccessAuth"]) != null)
                                {
                                    OnSuccessAuth(_mContext.Session, new EventArgs());
                                    _mContext.Session["OnSuccessAuth"] = null;
                                }

                                _successUrl = _mContext.Session["SuccessUrl"] != null
                                                  ? _mContext.Session["SuccessUrl"].ToString()
                                                  : "/";


                                _mResponse = new CMessageResponse(true.ToString(CultureInfo.InvariantCulture),
                                                                  CMessageResponse.EMessageResponseType.Notify,
                                                                  _successUrl);
                            }
                            catch (Exception e)
                            {
                                _mResponse =
                                    new CMessageResponse(
                                        Utils.GetLocalizedString("rtwErrLogin") + Utils.GetLocalizedString(e.Message),
                                        CMessageResponse.EMessageResponseType.Error);
                            }
                        }
                        else
                        {
                            _mResponse = new CMessageResponse(Utils.GetLocalizedString(_mRequest.repair ? "rtwRepairError" : "rtwErrLogin"),
                                                              CMessageResponse.EMessageResponseType.Error);
                        }
                    }
                    else
                    {
                        AddLoginToCache(_mRequest.user);
                        _mResponse = new CMessageResponse(Utils.GetLocalizedString("rtwErrLoginNoUser") + _mRequest.user,
                                                          CMessageResponse.EMessageResponseType.Error);
                    }
                }
                catch (Exception e)
                {
                    _mResponse = new CMessageResponse(e.Message + "   " + e.StackTrace, CMessageResponse.EMessageResponseType.Error);
                }
            }
        }


        /// <summary>
        /// добавляем логин в список не прошедших проверку
        /// </summary>
        /// <param name="login"></param>
        private void AddLoginToCache(string login)
        {
            lock (_lock)
            {
                List<string> badLogins = (List<string>) _mContext.Cache[CACHE_LOGINS] ?? new List<string>();
                if (!badLogins.Contains(login))
                {
                    badLogins.Add(login);
                }

                _mContext.Cache.Insert(CACHE_LOGINS, badLogins, null, DateTime.UtcNow.AddSeconds(CACHE_EXPIRES),
                                       Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
            }
        }

        /// <summary>
        /// проверка что логин не в списке не прошедших проверку в кэше
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        private bool CheckCachedLogin(string login)
        {
            return (_mContext.Cache[CACHE_LOGINS] == null ||
                    !((List<string>) _mContext.Cache[CACHE_LOGINS]).Contains(login));
        }

        /// <summary>
        /// проверяем параметры логина
        /// </summary>
        private bool CheckLoginParams()
        {
            return
                !string.IsNullOrEmpty(_mRequest.login) && !string.IsNullOrEmpty(_mRequest.urnd) &&
                !string.IsNullOrEmpty(_mRequest.sign) &&
                !REGEX_LOGIN.IsMatch(_mRequest.login) &&
                REGEX_URND.IsMatch(_mRequest.urnd) &&
                _mRequest.Tokenid > 0 &&
                _mRequest.sign.Length == 128 &&
                REGEX_KEYS.IsMatch(_mRequest.sign);
        }


        private void SendLocalizeScript()
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "RutokenWebPlugin.javascript.tokenadmin.js"))
            {
                if (stream != null)
                {
                    var reader = new StreamReader(stream);
                    HttpContext.Current.Response.Write(Utils.LocalizeScript(reader.ReadToEnd()));
                }
            }
        }
    }
}