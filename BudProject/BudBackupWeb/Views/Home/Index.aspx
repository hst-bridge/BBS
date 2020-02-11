<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    ホーム ページ
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: ViewData["Message"] %></h2>
    <p>
        ASP.NET MVC の詳細については、<a href="http://asp.net/mvc" title="ASP.NET MVC Website">http://asp.net/mvc</a> を参照してください。
    </p>
</asp:Content>
