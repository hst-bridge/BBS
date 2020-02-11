<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="budbackup.CommonWeb" %>
<asp:Content ID="ObjectSpyTitle" ContentPlaceHolderID="TitleContent" runat="server">
	BUD Backup System
</asp:Content>
<asp:Content ID="header" ContentPlaceHolderID="budHeader" runat="server">
<% Html.RenderPartial("LogonHeader"); %>
</asp:Content>
<asp:Content ID="ObjectSpyContent" ContentPlaceHolderID="MainContent" runat="server">
<% Html.RenderPartial("TabControlList1"); %>
<script type="text/javascript">
    //削除ファンクション
    function funDelete(id) {
        if (!confirm('<%=CommonUtil.getMessage("Q004","転送元") %>')) {
            //削除操作をキャンセルする時
            return;
        }
        var url = rootURL + "ObjectSpy/Delete";
        var data = {};
        data.id = id;
        $.ajax({
            type: "POST",
            url: url,
            cache: false,
            data: data,
            error: function (errMsg) { alert(errMsg); },
            success: function (result) {
                if (result == 99) {//未登録
                    alert('<%=CommonUtil.getMessage("W001","転送元設定") %>');
                    window.location.href = rootURL + "Account/LogOn";
                }
                else if (result == -10) {//エラーがある
                    alert('<%=CommonUtil.getMessage("E001") %>');
                }
                else if (result == 0) {//成功
                    alert('<%=CommonUtil.getMessage("I001","削除")%>');
                    window.location.href = rootURL + "ObjectSpy/Index";
                }
                else {//失敗
                    alert('<%=CommonUtil.getMessage("I002","削除")%>');
                }
            }
        });
    }
</script>
    <div class="tab1Wrapper">
        <p class="sbTtl">転送元一覧</p>
        <% List<Model.MonitorServer> msDataList = (List<Model.MonitorServer>)ViewData["msDataList"]; %>
        <table border="1" cellspacing="0" cellpadding="0" class="table2-1">
         <tr>
            <th scope="col" width="55">No.</th>
            <th scope="col" width="170">転送元名称</th>
            <th scope="col" width="120">IPアドレス</th>
            <th scope="col" width="100">転送環境</th>
            <th scope="col" width="207">メモ</th>
            <th scope="col" width="90">状況</th>
            <th scope="col" width="174">操作</th>
          </tr>
           <% for (int i = 0; i < msDataList.Count; i++)
            { %>
            <tr>
                <td class="cel1"><%:i+1 %></td>
                <td class="cel2"><%:msDataList[i].monitorServerName %></td>
                <td class="cel3"><%:msDataList[i].monitorServerIP %></td>
                <td class="cel3"><% if (msDataList[i].monitorSystem == 1)
                                    {%>MAC環境<%} %>
                                <%else if (msDataList[i].monitorSystem == 2)
                                    { %>WINDOWS環境<%} %>
                </td>
                <td class="cel4"><%:msDataList[i].memo%></td>
                <td class="cel5">
                    <% if (msDataList[i].deleteFlg == 1)
                    { Response.Write("削除済"); }
                    else
                    { Response.Write("有効"); }
                        %>
                </td>
                <td>
                    <ul class="ctrlBtn FClear" style=" text-align:center;">
                	    <li class="PR10"><a href="javascript:funGoEditPage('ObjectSpy/Edit','<%:msDataList[i].id %>');"><img src="../../Content/Image/editBtn.gif" alt=""/></a></li>
                	    <li style=" float:none;"><a href="javascript:funDelete('<%:msDataList[i].id %>');"><img src="../../Content/Image/deleteBtn.gif" alt=""/></a></li>
                    </ul>
                </td>
            </tr>
            <%}
                %>
        </table>
        <!--転送元新規作成-->
        <p><a href="javascript:funGotoPage('ObjectSpy/Insert');"><img src="../../Content/Image/newfileBtn.gif" class="img_on" alt=""/></a></p>
    </div>
</asp:Content>
