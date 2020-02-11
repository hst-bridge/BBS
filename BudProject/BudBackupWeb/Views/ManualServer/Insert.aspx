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
            var url = rootURL + "ManualServer/Add";
            if (!confirm('<%=CommonUtil.getMessage("Q001") %>')) {
                //保存操作をキャンセルする時
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
                    else if (result >= 0) {
                        alert('<%=CommonUtil.getMessage("I001","保存")%>');
                        window.location.href = rootURL + "ManualServer/Index";
                    }
                    else {
                        alert('<%=CommonUtil.getMessage("I002","保存")%>');
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
    function clearText() {
        $("#serverIP").val("");
        $("#account").val("");
        $("#password").val("");
        $("#startFile").val("");
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
    function funCheck_bkFromNoChange2() {
        if ($("#serverIP").val().trim() != initData.serverIP) {
            return false;
        }
        if ($("#account").val().trim() != initData.account) {
            return false;
        }
        if ($("#password").val().trim() != initData.password) {
            return false;
        }
        if ($("#startFile").val().trim() != initData.startFile) {
            return false;
        }
        return true;
    }
    //キャンセル
    function funCancel() {
        if (funCheck_bkFromNoChange2()) {
            window.location.href = rootURL + "ManualServer/Index";
        }
        else {
            if (confirm('<%=CommonUtil.getMessage("Q002")%>')) {
                window.location.href = rootURL + "ManualServer/Index";
            }
        }
    }
</script>
<div class="tab1Wrapper">
    <p class="sbTtl">【コピーサーバー設定】</p>
    <div class="edit2-2 FClear">
        <form name="bkserver_form" id="bkserver_form" action="" method="post">
            <dl class="editData2-2 OnCL FClear">
                <dt class="size1"><span style="color:Red; margin-right:5px;">※</span>IPアドレス：</dt>
                <dd>
                    <input type="text" name="serverIP" id="serverIP" size="40" style="width:300px;" />
                    <span id="serverIP_error" class ="spanPrompt"></span>
	            </dd>
            </dl>
            <dl class="editData2-2 OnCL FClear">
                <dt class="size1"><span style="color:Red; margin-right:5px;">※</span>接続アカウント名：</dt>
                <dd>
                    <input type="text" name="account" id="account" size="40" style="width:300px;" />
                    <span id="account_error" class ="spanPrompt"></span>
	            </dd>
            </dl>
            <dl class="editData2-2 OnCL FClear">
                <dt class="size1"><span style="color:Red; margin-right:5px;">※</span>パスワード：</dt>
                <dd class="PR10">
                    <input type="password" name="password" id="password" size="40" style="width:300px;" />
                    <span id="password_error" class ="spanPrompt"></span>
	            </dd>
                <dd><a href="javascript:funNetworkTest();"><img src="../../Content/Image/connectBtn.gif" alt=""/></a></dd>
            </dl>
            <dl class="editData2-2 OnCL FClear">
                <dt class="size1"><span style="color:Red; margin-right:5px;">※</span>開始フォルダ：</dt>
                <dd class="PR10">
                    <input type="text" name="startFile" id="startFile" size="40" style="width:300px;" />
                    <span id="startFile_error" class ="spanPrompt"></span>
	            </dd>
            </dl>
            <dl class="editData2-2 OnCL FClear">
                <dt class="size1">ドライブ名：</dt>
                <dd>
                    <select id="drive" name="drive" style="width:100px">
                        <% for (char i = 'A'; i <= 'Z'; i++)
                            {
                                if (!System.IO.Directory.Exists(i.ToString() + ":\\"))
                                {
                                %>
                        <option value="<%:i %>" ><%:i.ToString()%></option>
                        <%}
                            } %>
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
