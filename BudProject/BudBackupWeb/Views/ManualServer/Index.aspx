<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="budbackup.CommonWeb" %>
<asp:Content ID="TransferTitle" ContentPlaceHolderID="TitleContent" runat="server">
	BUD Backup System
</asp:Content>
<asp:Content ID="header" ContentPlaceHolderID="budHeader" runat="server">
<% Html.RenderPartial("LogonHeader"); %>
</asp:Content>

<asp:Content ID="TransferContent" ContentPlaceHolderID="MainContent" runat="server">
<% Html.RenderPartial("TabControlList7"); %>
<script type="text/javascript">
    //削除ファンクション
    function funDelete(id) {
        var url = rootURL + "ManualServer/Delete";
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
                    window.location.href = rootURL + "ManualServer/Index";
                }
                else {
                    alert('<%=CommonUtil.getMessage("I002","削除")%>');
                }
            }
        });
    }
    //接続テスト
    function funNetworkTest(id) {
        var url = rootURL + "ManualServer/NetworkConnect";
        var data = {};
        data.id = id;
        $.ajax({
            type: "POST",
            url: url,
            cache: false,
            data: data,
            error: function (errMsg) { alert(errMsg); },
            success: function (result) {
                if (result == 1) {
                    alert('<%=CommonUtil.getMessage("I001","接続テスト")%>');
                }
                else {
                    alert('<%=CommonUtil.getMessage("I002","接続テスト")%>');
                }
            }
        });
    }
</script>
<div class="tab1Wrapper">
    <p class="sbTtl">【コピーサーバー設定】</p>
    <% List<Model.ManualBackupServer> bkList = (List<Model.ManualBackupServer>)ViewData["bkList"]; %>
    <table border="1" cellspacing="0" cellpadding="0" class="table2-5">
        <tr>
            <th scope="col" width="55">No.</th>
            <th scope="col" width="130">IPアドレス</th>
            <th scope="col" width="100">ドライブ名</th>
            <th scope="col" width="174">操作</th>
        </tr>
        <% for (int i = 0; bkList != null && i < bkList.Count; i++)
           {%>
        <tr>
            <td class="cel1"><%: i +1 %></td>
            <td class="cel2"><%:bkList[i].serverIP %></td>
            <td class="cel3"><%:bkList[i].drive %></td>
            <td align="center">
			  <ul class="ctrlBtn FClear" style=" text-align:center;">
                	<li class="PR10"><a href="javascript:funNetworkTest('<%:bkList[i].id %>');"><img src="../../Content/Image/connBtn.gif" alt=""/></a></li>
                	<li style=" float:none"><a href="javascript:funDelete('<%:bkList[i].id %>');"><img src="../../Content/Image/deleteBtn.gif" alt=""/></a></li>
                </ul>
			</td>
          </tr>
        <%} %>
    </table>
    <!--転送先新規作成-->
    <p><a href="javascript:funGotoPage('ManualServer/Insert');"><img src="../../Content/Image/copyBtn.gif" class="img_on" alt=""/></a></p>
</div>
</asp:Content>
