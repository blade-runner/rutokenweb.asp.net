using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml.Serialization;

namespace RutokenWebPlugin
{
    [DataContract]
    public class CMessageResponse
    {
        // ��� ��������� �� �������
        #region EMessageResponseType enum

        public enum EMessageResponseType
        {
            Error, // ������
            Notify, // �����������
        }

        #endregion


        private readonly EMessageResponseType m_messageType = EMessageResponseType.Error;


        // ������ ����� � ��� 
        public CMessageResponse(string html, EMessageResponseType typ, string Url = null)
        {
            m_messageType = typ;
            InnerHtml = html;
            SuccessUrl = Url;
        }


        public CMessageResponse()
        {
        }

        /// <summary>
        /// ��� ���������
        /// </summary>
        [DataMember(Name = "type")]
        public string MessageType
        {
            get { return m_messageType.ToString(); }
            set { }
        }

        /// <summary>
        /// ����� ��� �������
        /// </summary>
        [DataMember(Name = "text")]
        public string InnerHtml { get; set; }

        /// <summary>
        /// SuccessUrl
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "url")]
        public string SuccessUrl { get; set; }




        public string ToJson()
        {
            var serializer = new DataContractJsonSerializer(GetType());
            var ms = new MemoryStream();
            serializer.WriteObject(ms, this);
            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}