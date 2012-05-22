using System;
using System.Collections.Generic;

namespace RutokenWebPlugin
{
    /// <summary>
    /// ������ � �������
    /// </summary>
    public interface ITokenProcessor
    {
        /// <summary>
        /// ���������������� �� ������������
        /// </summary>
        /// <returns></returns>
        bool IsUserAuthenticated();

        /// <summary>
        /// ��� ������������ (�����), ��� ��������� �� �������� � ������������� ��� ��������
        /// </summary>
        /// <returns></returns>
        string GetUserName();

        /// <summary>
        /// ����������� ������ ��� ������������
        /// </summary>
        /// <param name="tokenId">id ������</param>
        /// <param name="repairKey">���� ��������������</param>
        /// <param name="publicKey">�������� ����</param>
        /// <returns></returns>
        void RegisterToken(uint tokenId, string repairKey, string publicKey);

        /// <summary>
        /// ���������� ����� �� �����
        /// </summary>
        void UnregisterToken(uint tokenId);

        /// <summary>
        /// ��������� ��� ����� ��� ��������
        /// </summary>
        /// <returns></returns>
        bool IsTokenRegistered(uint tokenId);

        /// <summary>
        /// ��� ���� ����������� ������ � �������
        /// </summary>
        /// <param name="tokenId"> </param>
        /// <param name="enabled"></param>
        void SwitchToken(uint tokenId, bool enabled);

        /// <summary>
        /// �������� �������� �� �������������� �� ������
        /// </summary>
        /// <returns></returns>
        bool IsTokenSwitchedOn(uint tokenId);

        /// <summary>
        /// ���������� �� ����� ������������ � ����� �� ��� ����������?
        /// ��������� ����������, ������������ ������ � ������� ������.
        /// ������ ��� ������� ��� ����������� �����
        /// </summary>
        /// <param name="tokenid"></param>
        /// <returns></returns>
        bool UserCanBeAuthenticated(uint tokenid);

        /// <summary>
        /// ������ ���� ��������� ����� ������������ (userXkey,userYkey)
        /// </summary>
        /// <returns></returns>
        string GetPublicKey(uint tokenid);


        /// <summary>
        /// ������ ���� ����� �������������� (repairXkey,repairYkey)
        /// </summary>
        /// <returns></returns>
        string GetRepairKey(string login);

        /// <summary>
        /// ��������� �������������� ������������ � ��������� ������ ���� ������� ������
        /// </summary>
        /// <param name="tokenid"></param>
        /// <param name="strSignature"></param>
        /// <param name="strSource"></param>
        /// <returns></returns>
        bool SetUserAuthenticated(uint tokenid, String strSignature, String strSource);


        /// <summary>
        /// �������� ������ id ������� ������������
        /// </summary>
        /// <returns></returns>
        List<uint> GetUserTokens(string login);
    }
}