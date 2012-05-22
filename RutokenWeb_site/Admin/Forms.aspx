<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true"
    CodeBehind="Forms.aspx.cs" Inherits="RutokenWebSite.Admin.Forms" %>
    
<%@ Register TagPrefix="token" Namespace="RutokenWebPlugin" Assembly="RutokenWebPlugin" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cpAdmin" runat="server">
    <div class="layout admin">
        <div class="screen">
            <asp:Label runat="server" ID="lblResult"></asp:Label>
           <h3>Подпись форм</h3>
       <token:FormSignControl runat="server" ID="formSignControl" Port="321">
           <Template>
               <asp:Panel runat="server" ID="rtwFormFields" CssClass="round_3px">
                   <table class="formtable">
                        <tr>
                            <th>ФИО плательщика:</th>
                            <td><asp:TextBox runat="server" ID="TextBox0"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <th>Перевод со счета:</th>
                            <td><asp:TextBox runat="server" ID="AAox1"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <th>Сумма перевода и валюта:</th>
                            <td><asp:TextBox runat="server" ID="ddl2" CssClass="preselect"></asp:TextBox>
                            <asp:DropDownList runat="server" ID="ddl1">
                               <Items>
                                   <asp:ListItem Text="RUR" Value="RUR"></asp:ListItem>
                                   <asp:ListItem Text="USD" Value="USD"></asp:ListItem>
                                   <asp:ListItem Text="EUR" Value="EUR"></asp:ListItem>
                               </Items>
                           </asp:DropDownList></td>
                        </tr>
                        <tr>
                            <th>Наименование получателя:</th>
                            <td><asp:TextBox runat="server" ID="TextBox35"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <th>Примечание:</th>
                            <td><asp:TextBox runat="server" ID="ATextBox4" TextMode="MultiLine" Rows="5"></asp:TextBox></td>
                        </tr>
                    </table>
                   <br />
                     <%--контрол со списком контейнеров подписи--%>
                   Выберите ключ цифровой подписи: <asp:Literal ID="rtwUsers" runat="server" />
                   <br />
                   <asp:Button runat="server" ID="rtwFormSign" Text="подписать форму" OnClick="CheckFormSign" />
                    <asp:Image ID="rtwAjaxImg" runat="server" ImageUrl="/ajax_loader.gif" />
                   <br />
                     <asp:Label ID="rtwErrorMessage" runat="server" CssClass="errLabel" />
                                <asp:Label ID="rtwMessage" runat="server" CssClass="status ok" />
               </asp:Panel>
           </Template>
       </token:FormSignControl>
        </div>
    </div>
</asp:Content>
