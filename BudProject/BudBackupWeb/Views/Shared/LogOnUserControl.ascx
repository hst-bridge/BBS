<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Model" %>
<%if (Session["Usr"] != null)
              {
                  UserInfo user = (UserInfo)Session["Usr"];
      %>
        ようこそ <b><%=user.name%></b> さん
<%
    }
    else {
%> 
        [ <%: Html.ActionLink("ログオン", "LogOn", "Account") %> ]
<%
    }
%>
