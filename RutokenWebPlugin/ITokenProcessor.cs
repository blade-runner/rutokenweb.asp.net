using System;
using System.Collections.Generic;

namespace RutokenWebPlugin
{
    /// <summary>
    /// работы с токеном
    /// </summary>
    public interface ITokenProcessor
    {
        /// <summary>
        /// аутентифицирован ли пользователь
        /// </summary>
        /// <returns></returns>
        bool IsUserAuthenticated();

        /// <summary>
        /// имя пользователя (логин), для подгрузки на страницу и использования при привязке
        /// </summary>
        /// <returns></returns>
        string GetUserName();

        /// <summary>
        /// регистрация токена для пользователя
        /// </summary>
        /// <param name="tokenId">id токена</param>
        /// <param name="repairKey">ключ восстановления</param>
        /// <param name="publicKey">открытый ключ</param>
        /// <returns></returns>
        void RegisterToken(uint tokenId, string repairKey, string publicKey);

        /// <summary>
        /// отвязываем токен от юзера
        /// </summary>
        void UnregisterToken(uint tokenId);

        /// <summary>
        /// Проверяем что токен уже привязан
        /// </summary>
        /// <returns></returns>
        bool IsTokenRegistered(uint tokenId);

        /// <summary>
        /// вкл выкл возможности работы с токеном
        /// </summary>
        /// <param name="tokenId"> </param>
        /// <param name="enabled"></param>
        void SwitchToken(uint tokenId, bool enabled);

        /// <summary>
        /// проверка включена ли аутентификация по токену
        /// </summary>
        /// <returns></returns>
        bool IsTokenSwitchedOn(uint tokenId);

        /// <summary>
        /// Существует ли такой пользователь и можно ли его залогинить?
        /// проверять валидность, включенность токена и наличие ключей.
        /// тоесть все условия для авторизации юзера
        /// </summary>
        /// <param name="tokenid"></param>
        /// <returns></returns>
        bool UserCanBeAuthenticated(uint tokenid);

        /// <summary>
        /// выдает пару открытого ключа пользователя (userXkey,userYkey)
        /// </summary>
        /// <returns></returns>
        string GetPublicKey(uint tokenid);


        /// <summary>
        /// выдает пару ключа восстановления (repairXkey,repairYkey)
        /// </summary>
        /// <returns></returns>
        string GetRepairKey(string login);

        /// <summary>
        /// Установка аутентификации пользователя с передачей методу всех входных данных
        /// </summary>
        /// <param name="tokenid"></param>
        /// <param name="strSignature"></param>
        /// <param name="strSource"></param>
        /// <returns></returns>
        bool SetUserAuthenticated(uint tokenid, String strSignature, String strSource);


        /// <summary>
        /// получаем список id токенов пользователя
        /// </summary>
        /// <returns></returns>
        List<uint> GetUserTokens(string login);
    }
}