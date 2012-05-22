<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="RutokenWebSite.Admin.Default" %>

<%@ Register TagPrefix="token" Namespace="RutokenWebPlugin" Assembly="RutokenWebPlugin" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cpAdmin" runat="server">

    <div class="layout admin">
        <div class="screen">
            <table border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td colspan="2">
                        <h3>
                            Управление токеном</h3>
                        <p>
                            Включите возможность авторизации по токену и привяжите токен к аккаунту. После этого
                            можно входить в кабинет по токену.</p>
                    </td>
                </tr>
                <tr>
                    <td>
                        <token:Administration ID="backoffice" runat="server" Port="321">
                            <template>
                                      
                            <label>Список токенов:</label>
                        
                               <%-- <asp:RadioButtonList ID="rtwEnable" runat="server" />--%>
                               <asp:GridView runat="server" ID="rtwEnable" CssClass="OrdersGr" AutoGenerateColumns="False" GridLines="None" ShowHeaderWhenEmpty="False">
                                   <EmptyDataTemplate>
                                       Нет привязанных токенов
                                   </EmptyDataTemplate>
                               <Columns>
                                   <asp:TemplateField HeaderText="Token Id">
                                     <ItemTemplate>
                                         <%# ((uint)Container.DataItem) %>
                                     </ItemTemplate>
                                   </asp:TemplateField>
                               </Columns>
                               <Columns>
                                   <asp:TemplateField HeaderText="Активен">
                                     <ItemTemplate>
                                       <asp:Label ID="rtwEnabledToken" runat="server"></asp:Label>
                                     </ItemTemplate>
                                   </asp:TemplateField>
                               </Columns>
                                <Columns>
                                   <asp:TemplateField HeaderText="Управление">
                                     <ItemTemplate>
                                        <asp:Button runat="server" ID="rtwEnableSwitch" OnClientClick="return false;"/>
                                        <asp:Button ID="rtwRemove" runat="server" Text="Отвязать токен" OnClientClick="return false;" ClientIDMode="Predictable"/> 
                                     </ItemTemplate>
                                   </asp:TemplateField>
                               </Columns>
                               </asp:GridView>
                                            <br />
                                <label>Связка с Рутокен Web:</label>
                            
                                <asp:Button ID="rtwConnect" runat="server" Text="Привязать токен"/>                         
                                            <br />
                                <asp:Image ID="rtwAjaxImg" runat="server" ImageUrl="~/ajax_loader.gif" />
                                            <br />
                                <asp:Label ID="rtwErrorMessage" runat="server" CssClass="errLabel" />
                                <asp:Label ID="rtwMessage" runat="server" CssClass="status ok" />
                          
                          </template>
                        </token:Administration>
                    </td>
                    <td>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>
