<%@ Page Title="" Language="C#" MasterPageFile="~/Rutoken.Master" AutoEventWireup="true" EnableEventValidation="false"
    CodeBehind="Default.aspx.cs" Inherits="RutokenWebSite.Default" %>
<%@ Register TagPrefix="aktivlogin" Namespace="RutokenWebPlugin" Assembly="RutokenWebPlugin" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cpMain" runat="server">
    <div class="screen">
    <div class="data">Пример интеграции РутокенWeb</div>
        </div>
    <div class="layout login">
        <div class="screen">
            <div>
                <div class="head">
                <asp:Label ID="lblLoginStatus" runat="server"></asp:Label>
                    <h2>
                        Вход по логину и паролю</h2>
                </div>
                <div class="data">
                    <asp:Login ID="login" runat="server" OnAuthenticate="OnAuth" DisplayRememberMe="false"
                        DestinationPageUrl="Admin/">
                    </asp:Login>
                </div>
            </div>
            <div>
                <div class="head">
                    <h2>
                        Вход по токену</h2>
                </div>
                <div class="data">
                <table>
                 <tr >
                <th style="vertical-align: top;">
                    Логин:
                </th>
                <td>
                    <aktivlogin:Login ID="tokenlogin" runat="server" LoginType="Login">
                    <Template>
                        <asp:Literal ID="rtwUsers" runat="server" />
                        <asp:Label ID="rtwErrorMessage" runat="server" CssClass="rutoken error" style="display: block; color: #c00;" />
                        <asp:Label ID="rtwMessage" runat="server" CssClass="rutoken message" style="display: block; color: green;" />
                        <asp:Button ID="rtwLogin" runat="server" OnClientClick="return false;" Text="Войти" style="margin-top:12px;" />
                          <asp:Image ID="rtwAjaxImg" runat="server" ImageUrl="/ajax_loader.gif" />
                    </Template>                    
                    </aktivlogin:Login> 
                </td>
            </tr>
                </table>
                </div>
            </div>
            <div class="navy">
                <p>
                    <span>Авторизация</span> | <a href="Registration.aspx">Регистрация</a> | <a href="Repair.aspx">Восстановление доступа</a></p>
            </div>
        </div>
    </div>
</asp:Content>
