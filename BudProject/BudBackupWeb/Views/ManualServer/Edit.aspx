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
    var initData = new Object();
    $(document).ready(function () {
        initData.serverIP = $('#serverIP').val();
        initData.account = $('#account').val();
        initData.password = $('#password').val();
        initData.startFile = $('#startFile').val();
        initData.drive = $('#drive').val();
    });
   //保存ファンクション
    function formSubmit() {
        if (submitCheck() == true) {
            var url = rootURL + "ManualServer/Update";
            if (!confirm('<%=CommonUtil.getMessage("Q005") %>')) {
                //更新操作をキャンセルする時
                return;
            }
            funShowLoading();
            $.ajax({
                type: "POST",
                url: url,
                cache: false,
                data: $("#bkserver_form").serialize(),
                error: function (errMsg) { alert(errMsg); funHideLoading(); },
                success: function (result) {
                    funHideLoading();
                    if (result == -99) {
                        alert('<%=CommonUtil.getMessage("W001","転送先設定") %>');
                        window.location.href = rootURL + "Account/LogOn";
                    }
                    else if (result == -10) {//エラーがある
                        alert('<%=CommonUtil.getMessage("E001") %>');
                    }
                    else if (result == 0) {
                        alert('<%=CommonUtil.getMessage("I001","更新")%>');
                        window.location.href = rootURL + "Transfer/Index";
                    }
                    else {
                        alert('<%=CommonUtil.getMessage("I002","更新")%>');
                    }
                }
            });
        }
    }
    //保存前のチェック
    function submitCheck() {
        var flag = true;
        if ($("#serverIP").val().trim().length == 0) {
            flag = false;
            $("#serverIP_error").html('<%=CommonUtil.getMessage("W003","IPアドレス")%>');
        }
        else if (!checkIP($("#serverIP").val().trim())) {
            flag = false;
            $("#serverIP_error").html('<%=CommonUtil.getMessage("W005","IPアドレス")%>');
        }
        else {
            $("#serverIP_error").html('');
        }
        if ($("#account").val().trim().length == 0) {
            flag = false;
            $("#account_error").html('<%=CommonUtil.getMessage("W003","接続アカウント名")%>');
        }
        else {
            $("#account_error").html('');
        }
        if ($("#password").val().trim().length == 0) {
            flag = false;
            $("#password_error").html('<%=CommonUtil.getMessage("W003","パスワード")%>');
        }
        else {
            $("#password_error").html('');
        }
        if ($("#startFile").val().trim().length == 0) {
            flag = false;
            $("#startFile_error").html('<%=CommonUtil.getMessage("W003","開始フォルダ")%>');
        }
        else {
            $("#startFile_error").html('');
        }
        return flag;
    }
    //キャンセル
    function funCancel() {
        if (funCheck_bkFromNoChange()) {
            window.location.href = rootURL + "ManualServer/Index";
        }
        else {
            if (confirm('<%=CommonUtil.getMessage("Q002")%>')) {
                window.location.href = rootURL + "ManualServer/Index";
            }
        }
    }
    //接続テスト
    function funNetworkTest() {
        if (submitCheck() == true) {
            var url = rootURL + "ManualServer/NetworkTest";
            $.ajax({
                type: "POST",
                url: url,
                cache: false,
                data: $("#bkserver_form").serialize(),
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
    }
</script>
<div class="tab1Wrapper">
    <p class="sbTtl">【コピーサーバー設定】</p>
    <div class="edit2-2 FClear">
        <% Model.ManualBackupServer bkserver = (Model.ManualBackupServer)ViewData["bkserver"]; %>
        <form name="bkserver_form" id="bkserver_form" action="" method="post">
            <dl class="editData2-2 OnCL FClear">
                <dt class="size1"><span style="color:Red; margin-right:5px;">※</span>IPアドレス：</dt>
                <dd>
                    <input type="hidden" name="DBServerIP" id="DBServerIP" value="<%:bkserver.DBServerIP%>" />
                    <input type="text" name="serverIP" id="serverIP" size="40" style="width:300px;"  value="<%:bkserver.serverIP%>"/>
                    <span id="serverIP_error" class ="spanPrompt"></span>
	            </dd>
            </dl>
            <dl class="editData2-2 OnCL FClear">
                <dt class="size1"><span style="color:Red; margin-right:5px;">※</span>接続アカウント名：</dt>
                <dd>
                    <input type="text" name="account" id="account" size="40" style="width:300px;" value="<%:bkserver.account%>"/>
                    <span id="account_error" class ="spanPrompt"></span>
	            </dd>
            </dl>
            <dl class="editData2-2 OnCL FClear">
                <dt class="size1"><span style="color:Red; margin-right:5px;">※</span>パスワード：</dt>
                <dd>
                    <input type="password" name="password" id="password" size="40" style="width:300px;" value="<%:bkserver.password%>"/>
                    <span id="password_error" class ="spanPrompt"></span>
	            </dd>
                <dd><a href="javascript:funNetworkTest();"><img src="../../Content/Image/connectBtn.gif" alt=""/></a></dd>
            </dl>
            <dl class="editData2-2 OnCL FClear">
                <dt class="size1">開始フォルダ：</dt>
                <dd class="PR10">
                    <input type="text" name="startFile" id="startFile" size="40" style="width:300px;" value="<%:bkserver.startFile%>"/>
	            </dd>
            </dl>
            <dl class="editData2-2 OnCL FClear">
                <dt class="size1">ドライブ名：</dt>
                <dd class="PR10">
                    <select id="drive" name="drive" style="width:100px">
                            <% for (char i = 'A'; i <= 'Z'; i++)
                                {
                                    %>
                            <option value="<%:i %>" <%if(i.ToString()==bkserver.drive) {%>selected="selected" <%} %> ><%:i.ToString()%></option>
                            <%} %>
                    </select>
                    <input type="hidden" name="drive" value="<%:bkserver.drive %>" />
	            </dd>
            </dl>
            <ul class="twoBtns FClear">
            <li><a href="javascript:formSubmit();"><img src="../../Content/Image/connBtn.gif" class="img_on" alt=""/></a></li>
            <li><a href="javascript:funCancel();"><img src="../../Content/Image/cancelBtn.gif" class="img_on" alt=""/></a></li>
            </ul>
            <input type="hidden" id="id" name="id" value="<%:bkserver.id %>" />
        </form>
    </div>
</div>
</asp:Content>
