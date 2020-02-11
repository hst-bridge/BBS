<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="budbackup.CommonWeb" %>
<asp:Content ID="PermissionTitle" ContentPlaceHolderID="TitleContent" runat="server">
	BUD Backup System
</asp:Content>

<asp:Content ID="PermissionContent" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript">
    var rootURL = "<%=VirtualPath.getVirtualPath()%>";
    function formSubmit() {
        if (submitCheck() == true) {
            var url = rootURL + "Permission/Update";
            $.ajax({
                type: "POST",
                url: url,
                cache: false,
                data: $("#ms_form").serialize(),
                error: function (errMsg) { alert(errMsg); },
                success: function (result) {
                    if (result == -99) {//未登録
                        alert('<%=CommonUtil.getMessage("W001","権限設定管理") %>');
                        window.location.href = rootURL + "Account/LogOn";
                    }
                    else if (result == -10) {//エラーがある
                        alert('<%=CommonUtil.getMessage("E001") %>');
                    }
                    else if (result == 0) {//成功
                        alert('<%=CommonUtil.getMessage("I001","保存")%>');
                        window.location.href = rootURL + "Permission/Index";
                    }
                    else {//失敗
                        alert('<%=CommonUtil.getMessage("I002","保存")%>');
                    }
                }
            });
        }
    }
    function submitCheck() {
        var flag = true;
        var mail = $("#mail").val();
        if ($("#name").val().trim().length == 0) {
            flag = false;
            $("#name_error").html('<%=CommonUtil.getMessage("W003","ユーザー名前")%>');
        }
        else {
            $("name_error").html('');
        }
        if ($("#loginID").val().trim().length == 0) {
            flag = false;
            $("#loginID_error").html('<%=CommonUtil.getMessage("W003","ログインID")%>');
        }
        else {
            $("#loginID_error").html("");
        }
        if ($("#password").val().trim().length == 0) {
            flag = false;
            $("#password_error").html('<%=CommonUtil.getMessage("W003","パスワード")%>');
        }
        else {
            $("#password_error").html("");
        }
        if (mail.trim() == "") {
            $("#mail_error").html('<%=CommonUtil.getMessage("W003","ユーザーメール")%>');
            flag = false;
        } else if (mail.trim() != "") {
            var regex = /^([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+@([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+\.[a-zA-Z]{2,3}$/;
            if (!regex.test(mail)) {
                $("#mail_error").html('<%=CommonUtil.getMessage("W003","ユーザーメール")%>');
                flag = false;
            }
        }
        return flag;
    }
</script>
    <div>
         <div class="osMain">
            <div class="osTitle">
                <ul>
                    <li><%: Html.ActionLink("転送元設定", "Index", "ObjectSpy")%></li>
                    <li><%: Html.ActionLink("転送ファルダ設定", "Index", "FileSpy")%></li>
                    <li><%: Html.ActionLink("転送先設定", "Index", "Transfer")%></li>
                    <li><%: Html.ActionLink("転送先グループ設定", "Index", "GroupTransfer")%></li>
                    <li><%: Html.ActionLink("ログ表示", "Index", "Log")%></li>
                    <li class="on"><%: Html.ActionLink("権限設定管理", "Index", "Permission")%></li>
                    <li class="right"><a href="#">ログアウト</a></li>
                </ul>
            </div>
            <div class="clear"></div>
            <div class="osContent">
                <div class="contentInner">
                    <h4>【権限設定·編集】</h4>
                    <div class="div_border">
                    <form id="ms_form" name="ms_from" action="">
                    <table cellpadding="0" cellspacing="0" class="editTable">
                    <% Model.UserInfo userList = (Model.UserInfo)ViewData["userList"]; %>
                        <tbody>
                            <tr>
                                <td class="right" width="20%">名称</td>
                                <td width="30%"><input type="text" name="name" id="name" value="<%:userList.name %>" /></td>
                                <td><span id="name_error" class ="spanPrompt"></span></td>
                            </tr>
                            <tr>
                                <td class="right">ログインID</td>
                                <td><input type="text" id="loginID" name="loginID" value="<%:userList.loginID %>" /></td>
                                <td><span id="loginID_error" class="spanPrompt"></span></td>
                            </tr>
                            <tr>
                                <td class="right">パスワード</td>
                                <td><input type="password" id="password" name="password" value="<%:userList.password %>" /></td>
                                <td><span id="Span1" class="spanPrompt"></span></td>
                            </tr>
                            <tr>
                                <td class="right">メール</td>
                                <td><input type="text" id="mail" name="mail" value="<%:userList.mail %>" /></td>
                                <td><span id="mail_error" class="spanPrompt"></span></td>
                            </tr>
                            <tr>
                                <td colspan="2" class="right">
                                    <a href="javascript:history.go(0);">キャンセル</a>
                                    <a href="javascript:formSubmit();">保存</a>
                                </td>
                                
                            </tr>
                        </tbody>
                    </table>
                    <input type="hidden" id="id" name="id" value="<%:userList.id %>" />
                    </form>
                    </div>
                </div>
            </div>
         </div>
            
    </div>

</asp:Content>
