using System;
using System.Globalization;
using System.Web;

namespace RutokenWebPlugin
{
    /// <summary>
    /// методы для администрирования токена
    /// !!!!!!!!!!!!!!!!!!!! нужно всегда проверять пользователя и ряд условий для работы с токенами
    /// например не позволять отключать чужие токены и т.п.
    /// </summary>
    public partial class TokenAjaxHandler
    {
        private bool CheckAuthenticated()
        {

            //if (!HttpContext.Current.User.Identity.IsAuthenticated)
            if (!TokenProcessor.IsUserAuthenticated())
            {
                _mResponse = new CMessageResponse(Utils.GetLocalizedString("rtwAjaxErrorNoAuth"),
                                                  CMessageResponse.EMessageResponseType.Error);
                return false;
            }
            return true;
        }


        /// <summary>
        /// отвязываем токен
        /// </summary>
        public void remove()
        {
            if (!CheckAuthenticated())
            {
                return;
            }


            try
            {
                if (_mRequest.Tokenid > 0)
                {
                    TokenProcessor.UnregisterToken(_mRequest.Tokenid);
                    _mResponse = new CMessageResponse(true.ToString(CultureInfo.InvariantCulture),
                                                      CMessageResponse.EMessageResponseType.Notify);
                }
                else
                {
                    _mResponse = new CMessageResponse(Utils.GetLocalizedString("rtwAjaxError"),
                                                      CMessageResponse.EMessageResponseType.Error);
                }
            }
            catch (Exception e)
            {
                _mResponse = new CMessageResponse(Utils.GetLocalizedString("rtwAjaxError") + e.Message,
                                                  CMessageResponse.EMessageResponseType.Error);
            }
        }

        /// <summary>
        /// привязываем токен
        /// </summary>
        public void attach()
        {
            if (!CheckAuthenticated())
            {
                return;
            }
            try
            {
                if (_mRequest.Tokenid > 0 && _mRequest.pkey.Length == 128 && _mRequest.rkey.Length == 128)
                {
                    TokenProcessor.RegisterToken(_mRequest.Tokenid, _mRequest.rkey, _mRequest.pkey);
                    _mResponse = new CMessageResponse(true.ToString(CultureInfo.InvariantCulture),
                                                      CMessageResponse.EMessageResponseType.Notify);
                }
                else
                {
                    _mResponse = new CMessageResponse(Utils.GetLocalizedString("rtwAjaxError") + "params",
                                                      CMessageResponse.EMessageResponseType.Error);
                }
            }
            catch (Exception e)
            {
                _mResponse = new CMessageResponse(Utils.GetLocalizedString("rtwAjaxError") + "msg" + e.Message,
                                                  CMessageResponse.EMessageResponseType.Error);
            }
        }

        /// <summary>
        /// переключаем активность токена
        /// </summary>
        public void switc()
        {
            if (!CheckAuthenticated())
            {
                return;
            }
            try
            {
                if (_mRequest.Tokenid > 0)
                {
                    TokenProcessor.SwitchToken(_mRequest.Tokenid, _mRequest.to);
                    _mResponse = new CMessageResponse(_mRequest.to.ToString(CultureInfo.InvariantCulture),
                                                      CMessageResponse.EMessageResponseType.Notify);
                }
                else
                {
                    _mResponse = new CMessageResponse(Utils.GetLocalizedString("rtwAjaxError"),
                                                      CMessageResponse.EMessageResponseType.Error);
                }
            }
            catch (Exception e)
            {
                _mResponse = new CMessageResponse(Utils.GetLocalizedString("rtwAjaxError") + e.Message,
                                                  CMessageResponse.EMessageResponseType.Error);
            }
        }
    }
}