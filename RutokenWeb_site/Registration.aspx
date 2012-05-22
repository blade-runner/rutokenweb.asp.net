<%@ Page Title="" Language="C#" MasterPageFile="~/Rutoken.Master" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="RutokenWebSite.Registration" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cpMain" runat="server">
    <div class="layout registration">
        <div class="screen">
            <div>
                <div class="head">
                    <h2>
                        Регистрация</h2>
                </div>
                <div class="data">
Логин: <asp:TextBox ID="login" runat="server"></asp:TextBox><br />
Пароль: <asp:TextBox ID="password" runat="server"></asp:TextBox><br />
<asp:Button ID="btnReg" runat="server" Text="Зарегистрироваться" OnClick="Register" /><br />
<asp:Label ID="lblResult" runat="server"></asp:Label>
</div>
  <div class="navy">
                <p>
                    <a href="Default.aspx">Авторизация</a> | <span>Регистрация</span> | <a href="Repair.aspx">Восстановление доступа</a></p>
            </div>
</div>
</div>
</div>
</asp:Content>
