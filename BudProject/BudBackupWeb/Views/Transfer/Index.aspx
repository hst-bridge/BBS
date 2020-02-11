<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="budbackup.CommonWeb" %>
<asp:Content ID="TransferTitle" ContentPlaceHolderID="TitleContent" runat="server">
	BUD Backup System
</asp:Content>
<asp:Content ID="header" ContentPlaceHolderID="budHeader" runat="server">
<% Html.RenderPartial("LogonHeader"); %>
</asp:Content>

<asp:Content ID="TransferContent" ContentPlaceHolderID="MainContent" runat="server">
<% Html.RenderPartial("TabControlList3"); %>
<script type="text/javascript">
    //削除ファンクション
    function funDelete(id) {
        var url = rootURL + "Transfer/Delete";
        if (!confirm('<%=CommonUtil.getMessage("Q004","転送先") %>')) {
            //削除操作をキャンセルする時
            return;
        }
        var data ={};
        data.id = id;
        $.ajax({
            type: "POST",
            url: url,
            cache: false,
            data: data,
            error: function (errMsg) { alert(errMsg); },
            success: function (result) {
                if (result == -99) {
                    alert('<%=CommonUtil.getMessage("W001","転送先設定") %>');
                    window.location.href = rootURL + "Account/LogOn";
                }
                else if (result == -10) {//エラーがある
                    alert('<%=CommonUtil.getMessage("E001") %>');
                }
                else if (result == 0) {
                    alert('<%=CommonUtil.getMessage("I001","削除")%>');
                    window.location.href = rootURL + "Transfer/Index";
                }
                else {
                    alert('<%=CommonUtil.getMessage("I002","削除")%>');
                }
            }
        });
    }
</script>
<div class="tab1Wrapper">
    <p class="sbTtl">【転送先一覧】</p>
    <% List<Model.BackupServer> bkList= (List<Model.BackupServer>)ViewData["bkList"]; %>
    <table border="1" cellspacing="0" cellpadding="0" class="table2-5">
        <tr>
            <th scope="col" width="55">No.</th>
            <th scope="col" width="170">転送先対象名</th>
            <th scope="col" width="130">IPアドレス</th>
            <th scope="col" width="297">メモ</th>
            <th scope="col" width="90">状況</th>
            <th scope="col" width="174">操作</th>
        </tr>
        <% for (int i = 0; i < bkList.Count; i++){%>
        <tr>
            <td class="cel1"><%: i +1 %></td>
            <td class="cel2"><%:bkList[i].backupServerName %></td>
            <td class="cel3"><%:bkList[i].backupServerIP %></td>
            <td class="cel4"><%:bkList[i].memo %></td>
            <td class="cel5">有効</td>
            <td>
			  <ul class="ctrlBtn FClear" style=" text-align:center; padding:4px 10px;">
                	<li class="PR10"><a href="javascript:funGoEditPage('Transfer/Edit','<%:bkList[i].id %>');"><img src="../../Content/Image/editBtn.gif" alt=""/></a></li>
                	<li style=" float:none"><a href="javascript:funDelete('<%:bkList[i].id %>');"><img src="../../Content/Image/deleteBtn.gif" alt=""/></a></li>
                </ul>
			</td>
          </tr>
        <%} %>
    </table>
    <!--転送先新規作成-->
    <p><a href="javascript:funGotoPage('Transfer/Insert');"><img src="../../Content/Image/sendBtn.gif" class="img_on" alt=""/></a></p>
</div>
</asp:Content>
