<%@ Page Title="" Language="C#" MasterPageFile="~/Rutoken.Master" AutoEventWireup="true" CodeBehind="Repair.aspx.cs" Inherits="RutokenWebSite.Repair" %>
<%@ Register TagPrefix="aktivlogin" Namespace="RutokenWebPlugin" Assembly="RutokenWebPlugin" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cpMain" runat="server">
    <div class="layout login">
        <div class="screen">
            <div>
                <div class="head">
                    <h2>
                        Восстановление доступа</h2>
                </div>
                <div class="data">
                 <aktivlogin:Login ID="tokenlogin" runat="server" SuccessUrl="Admin/" LoginType="Remember">
                    <Template>
                        Логин: <asp:TextBox ID="rtwRepairUser" runat="server" /><br />
                        Код восстановления: <asp:TextBox ID="rtwRepair" runat="server" /><br />
                        <asp:Label ID="rtwErrorMessage" runat="server" style="display: block; color: #c00;" />
                        <asp:Label ID="rtwMessage" runat="server" style="display: block; color: green;" />
                        <asp:Button ID="rtwRepairBtn" runat="server" OnClientClick="return false;" Text="Войти" style="margin-top:12px;" />
                          <asp:Image ID="rtwAjaxImg" runat="server" ImageUrl="/ajax_loader.gif" />
                    </Template>                    
                    </aktivlogin:Login> 
                </div>
            </div>
            <div class="navy">
                <p>
                    <a href="Default.aspx">Авторизация</a> | <a href="Registration.aspx">Регистрация</a> | <span>Восстановление доступа</span></p>
            </div>
        </div>
    </div>
</asp:Content>
