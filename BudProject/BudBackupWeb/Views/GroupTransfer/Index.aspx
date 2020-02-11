<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="budbackup.CommonWeb" %>
<asp:Content ID="GroupTransferTitle" ContentPlaceHolderID="TitleContent" runat="server">
	BUD Backup System
</asp:Content>
<asp:Content ID="header" ContentPlaceHolderID="budHeader" runat="server">
<% Html.RenderPartial("LogonHeader"); %>
</asp:Content>

<asp:Content ID="GroupTransferContent" ContentPlaceHolderID="MainContent" runat="server">
<% Html.RenderPartial("TabControlList4"); %>
<script type="text/javascript">
   //削除ファンクション
    function funDelete(id) {
        if (!confirm('<%=CommonUtil.getMessage("Q004","転送先グループ") %>')) {
            //削除操作をキャンセルする時
            return;
        }
        var url = rootURL + "GroupTransfer/Delete";
        var data = {};
        data.id = id;
        $.ajax({
            type: "POST",
            url: url,
            cache: false,
            data: data,
            error: function (errMsg) { alert(errMsg); },
            success: function (result) {
                if (result == -99) {
                    alert('<%=CommonUtil.getMessage("W001","転送先グループ設定") %>');
                    window.location.href = rootURL + "Account/LogOn";
                }
                else if (result == -10) {//エラーがある
                    alert('<%=CommonUtil.getMessage("E001") %>');
                }
                else if (result == 0) {
                    alert('<%=CommonUtil.getMessage("I001","削除")%>');
                    window.location.href = rootURL + "GroupTransfer/Index";
                }
                else {
                    alert('<%=CommonUtil.getMessage("I002","削除")%>');
                }
            }
        });
    }
</script>
<div class="tab1Wrapper">
    <p class="sbTtl">【転送先グループ名称】</p>
    <% List<Model.BackupServerGroup> groupList = (List<Model.BackupServerGroup>)ViewData["modelList"]; %>
    <table border="1" cellspacing="0" cellpadding="0" class="table2-5">
        <tr>
            <th scope="col" width="55">No.</th>
            <th scope="col" width="170">転送先グループ名称</th>
            <th scope="col" width="344">メモ</th>
            <th scope="col" width="90">状況</th>
            <th scope="col" width="248">操作</th>
        </tr>
        <% for (int i = 0; i < groupList.Count; i++)
        { %>
            <tr>
                <td class="cel1"><%: i+1 %></td>
                <td class="cel2"><%:groupList[i].backupServerGroupName %></td>
                <td class="cel3"><%:groupList[i].memo %></td>
                <td class="cel4">有効</td>
                <td>
			        <ul class="ctrlBtn FClear" style="width:208px;">
                	    <li class="PR10"><a href="javascript:funGoEditPage('GroupTransfer/Edit','<%:groupList[i].id %>');"><img src="../../Content/Image/editBtn.gif" alt=""/></a></li>
                	    <li class="PR10"><a href="javascript:funDelete('<%:groupList[i].id %>');"><img src="../../Content/Image/deleteBtn.gif" alt=""/></a></li>
                        <li><a href="javascript:funGoEditPage('GroupTransfer/DetailEdit','<%:groupList[i].id %>');"><img src="../../Content/Image/settingBtn.gif" alt=""/></a></li>
                        
                    </ul>
			    </td>
              </tr>
        <%} %>
    </table>
    <p><a href="javascript:funGotoPage('GroupTransfer/Insert');"><img src="../../Content/Image/sendGroupBtn.gif" class="img_on" alt=""/></a></p>
</div>
</asp:Content>
