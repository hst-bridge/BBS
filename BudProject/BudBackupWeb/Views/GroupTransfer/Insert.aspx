<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master"  Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
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
    var initData = new Object();
    $(document).ready(function () {
        initData.backupServerGroupName = $('#backupServerGroupName').val();
        initData.monitorServerID = $('#monitorServerID').val();
        initData.memo = $('#memo').val();
    });
    //保存ファンクション
    function formSubmit() {
        var url = rootURL + "GroupTransfer/Add";
        if (!submitCheck()) {
            return;
        }
        if (!confirm('<%=CommonUtil.getMessage("Q001") %>')) {
            //保存操作をキャンセルする時
            return;
        }
        funShowLoading();
        $.ajax({
            type: "POST",
            url: url,
            cache: false,
            data: $("#group_form").serialize(),
            error: function (errMsg) { alert(errMsg); funHideLoading(); },
            success: function (result) {
                funHideLoading();
                if (result == -99) {
                    alert('<%=CommonUtil.getMessage("W001","転送先グループ設定") %>');
                    window.location.href = rootURL + "Account/LogOn";
                }
                else if (result == -10) {//エラーがある
                    alert('<%=CommonUtil.getMessage("E001") %>');
                }
                else if (result >= 0) {
                    alert('<%=CommonUtil.getMessage("I001","保存")%>');
                    window.location.href = rootURL + "GroupTransfer/Index";
                }
                else {
                    alert('<%=CommonUtil.getMessage("I002","保存")%>');
                }
            }
        });
    }
     //保存前のチェック
    function submitCheck() {
        var flag = true;
        if ($("#backupServerGroupName").val().trim().length == 0) {
            flag = false;
            $("#backupServerGroupName_error").html('<%=CommonUtil.getMessage("W003","転送先グループ名称")%>');
        }
        else if (funCheckIsExists($("#backupServerGroupName").val())) {
            flag = false;
        }
        else {
            $("#backupServerGroupName_error").html('');
        }
        return flag;
    }
    //キャンセル
    function funCancel() {
        if (funCheck_groupFromNoChange()) {
            window.location.href = rootURL + "GroupTransfer/Index";
        }
        else {
            if (confirm('<%=CommonUtil.getMessage("Q002")%>')) {
                window.location.href = rootURL + "GroupTransfer/Index";
            }
        }
    }
    function funCheckIsExists(objName) {
        var url = rootURL + "GroupTransfer/CheckNameIsExists";
        var data = new Object();
        data.groupName = objName;
        var flg = false;
        $.ajax({
            async: false,
            type: "POST",
            url: url,
            cache: false,
            data: data,
            error: function (errMsg) { ajaxDoing = false; alert(errMsg); },
            success: function (result) {
                if (result >= 1) {
                    $("#backupServerGroupName_error").html('<%=CommonUtil.getMessage("W008","転送先グループ名称")%>');
                    flg = true;
                }
                else if (result == 0) {
                    $("#backupServerGroupName_error").html('');
                    flg = false;
                }
                else if (result == -10) {
                    alert('<%=CommonUtil.getMessage("E001") %>');
                    flg = true;
                }
            }
        });
        return flg;
    }
</script>
<div class="tab1Wrapper">
    <p class="sbTtl">【転送先グループ設定】</p>
    <div class="edit2-2 FClear">
    <% List<Model.MonitorServer> msList = (List<Model.MonitorServer>)ViewData["msList"]; %>
        <form id="group_form" name="group_form" action="">
            <dl class="editData2-2 OnCL FClear">
            	<dt class="size1" style=" width:210px"><span style="color:Red; margin-right:5px;">※</span>転送先グループ名称：</dt>
            	<dd>
              		<input type="text" name="backupServerGroupName" id="backupServerGroupName" size="40" style="width:300px;" />
                    <span id="backupServerGroupName_error" class ="spanPrompt"></span>
				</dd>
			</dl>
            <dl class="editData2-2 OnCL FClear">
            	<dt class="size1" style=" width:210px">メモ：</dt>
            	<dd>
              		<input type="text" name="memo" id="memo" size="40" style="width:300px;" />
                    <span id="memo_error" class ="spanPrompt"></span>
				</dd>
			</dl>
            <dl class="editData2-2 OnCL FClear">
                <dt class="size1" style=" width:210px">転送元：</dt>
                <dd>
                    <select id="monitorServerID" name="monitorServerID">
                    <% for (int i = 0; i < msList.Count; i++)
                        {%>
                        <option value="<%:msList[i].id %>"><%:msList[i].monitorServerName %></option>
                        <%} %>
                    </select>
                </dd>
            </dl>
            <ul class="twoBtns FClear">
       	        <li><a href="javascript:formSubmit();"><img src="../../Content/Image/saveBtn.gif" class="img_on" alt=""/></a></li>
                <li><a href="javascript:funCancel();"><img src="../../Content/Image/cancelBtn.gif" class="img_on" alt=""/></a></li>
            </ul>
        </form>
    </div>
</div>

</asp:Content>