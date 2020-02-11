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
        var connectflag = false;
        var initData = new Object();
        $(document).ready(function () {
            initData.monitorServerName = $('#monitorServerName').val();
            initData.monitorServerIP = $('#monitorServerIP').val();
            initData.memo = $('#memo').val();
            initData.account = $('#account').val();
            initData.password = $('#password').val();
            initData.startFile = $('#startFile').val();
            initData.monitorDriveP = $('#monitorDriveP').val();
            initData.monitorLocalPath = $('#monitorLocalPath').val();
        });
        function formSubmit() {
            if (submitCheck()) {
                if (!connectflag) {
                    alert('<%=CommonUtil.getMessage("I002","接続テスト")%>' + '確認してください。');
                    return;
                }
                //$("#ms_form").submit();
                if (!confirm('<%=CommonUtil.getMessage("Q001") %>')) {
                    //保存操作をキャンセルする時
                    return;
                }
                var url = rootURL + "ObjectSpy/Add";
                funShowLoading();
                $.ajax({
                    type: "POST",
                    url: url,
                    cache: false,
                    data: $("#ms_form").serialize(),
                    error: function (errMsg) { alert(errMsg); funHideLoading(); },
                    success: function (result) {
                        funHideLoading();
                        if (result == -99) {//未登録
                            alert('<%=CommonUtil.getMessage("W001","転送元設定") %>');
                            window.location.href = rootURL + "Account/LogOn";
                        }
                        else if (result == -10) {//エラーがある
                            alert('<%=CommonUtil.getMessage("E001") %>');
                        } 
                        else if (result == -20) 
                        {
                            alert('<%=CommonUtil.getMessage("I002","接続テスト")%>');
                        }
                        else if (result >= 0) {//成功
                            alert('<%=CommonUtil.getMessage("I001","保存")%>');
                            window.location.href = rootURL + "ObjectSpy/Index";
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
            if ($("#monitorServerName").val().trim().length == 0) {
                flag = false;
                $("#monitorServerName_error").html('<%=CommonUtil.getMessage("W003","転送元名称")%>');
            }
            else if (funCheckIsExists($("#monitorServerName").val())) {
                flag = false;
                //$("#monitorServerName_error").html('<%=CommonUtil.getMessage("W008","転送元名称")%>');
            }
            else {
                $("#monitorServerName_error").html('');
            }
            
            if ($("#monitorServerIP").val().trim().length == 0) {
                flag = false;
                $("#monitorServerIP_error").html('<%=CommonUtil.getMessage("W003","IPアドレス")%>');
            }
            else if (!checkIP($("#monitorServerIP").val().trim())) {
                flag = false;
                $("#monitorServerIP_error").html('<%=CommonUtil.getMessage("W005","IPアドレス")%>');
            }
            else {
                $("#monitorServerIP_error").html("");
            }
            if ($('input:radio[name="monitorSystem"]:checked ').length <= 0) {
                flag = false;
                $("#monitorSystem_error").html('<%=CommonUtil.getMessage("W003","転送環境")%>');
            }
            else {
                $("#monitorSystem_error").html("");
            }
            if ($("#account").val().trim().length == 0) {
                flag = false;
                $("#account_error").html('<%=CommonUtil.getMessage("W003","接続アカウント名")%>');
            }
            else {
                $("#account_error").html("");
            }
            if ($("#password").val().trim().length == 0) {
                flag = false;
                $("#password_error").html('<%=CommonUtil.getMessage("W003","パスワード")%>');
            }
            else {
                $("#password_error").html("");
            }
            if ($("#startFile").val().trim().length == 0) {
                flag = false;
                $("#startFile_error").html('<%=CommonUtil.getMessage("W003","共有ポイント")%>');
            }
            else {
                $("#startFile_error").html("");
            }
            if ($("#monitorMacPath").val().trim().length == 0) {
                flag = false;
                $("#monitorMacPath_error").html('<%=CommonUtil.getMessage("W003","MAC環境パス")%>');
            }
            else {
                $("#monitorMacPath_error").html("");
            }
            if ($("#monitorLocalPath").val().trim().length == 0) {
                flag = false;
                $("#monitorLocalPath_error").html('<%=CommonUtil.getMessage("W003","ローカルパス")%>');
            }
            else if (!checkDirPath($("#monitorLocalPath").val().trim())) {
                flag = false;
                $("#monitorLocalPath_error").html('<%=CommonUtil.getMessage("W005","ローカルパス")%>');
            }
            else {
                $("#monitorLocalPath_error").html("");
            }
            
            return flag;
        }
        function clearText() { 

        }
        //接続テストファンクション
        function funNetworkTest() {
            var url = rootURL + "ObjectSpy/NetworkTest";
            if (!fun_NetworkTestCheck()) {
                return;
            }
            $.ajax({
                type: "POST",
                url: url,
                cache: false,
                data: $("#ms_form").serialize(),
                error: function (errMsg) { alert(errMsg); },
                success: function (result) {
                    if (result == 1) {
                        alert('<%=CommonUtil.getMessage("I001","接続テスト")%>');
                        connectflag = true;
                    }
                    else {
                        alert('<%=CommonUtil.getMessage("I002","接続テスト")%>');
                        connectflag = false;
                    }
                }
            });
        }
        function fun_NetworkTestCheck() {
            var flag = true;
            if ($("#monitorServerIP").val().trim().length == 0) {
                flag = false;
                $("#monitorServerIP_error").html('<%=CommonUtil.getMessage("W003","IPアドレス")%>');
            }
            else if (!checkIP($("#monitorServerIP").val().trim())) {
                flag = false;
                $("#monitorServerIP_error").html('<%=CommonUtil.getMessage("W005","IPアドレス")%>');
            }
            else {
                $("#monitorServerIP_error").html("");
            }
            if ($('input:radio[name="monitorSystem"]:checked ').length <= 0) {
                flag = false;
                $("#monitorSystem_error").html('<%=CommonUtil.getMessage("W003","転送環境")%>');
            }
            else {
                $("#monitorSystem_error").html("");
            }
            if ($("#account").val().trim().length == 0) {
                flag = false;
                $("#account_error").html('<%=CommonUtil.getMessage("W003","接続アカウント名")%>');
            }
            else {
                $("#account_error").html("");
            }
            if ($("#password").val().trim().length == 0) {
                flag = false;
                $("#password_error").html('<%=CommonUtil.getMessage("W003","パスワード")%>');
            }
            else {
                $("#password_error").html("");
            }
            if ($("#startFile").val().trim().length == 0) {
                flag = false;
                $("#startFile_error").html('<%=CommonUtil.getMessage("W003","共有ポイント")%>');
            }
            else {
                $("#startFile_error").html("");
            }
            return flag;
        }
        //キャンセル
        function funCancel() {
            if (funCheck_msFormNoChange()) {
                window.location.href = rootURL + "ObjectSpy/Index";
            }
            else {
                if (confirm('<%=CommonUtil.getMessage("Q002")%>')) {
                    window.location.href = rootURL + "ObjectSpy/Index";
                 }
            }
        }
        function funCheckIsExists(objName) {
            var url = rootURL + "ObjectSpy/CheckNameIsExists";
            var data = new Object();
            data.msName = objName;
            var flg = false;
            $.ajax({
                async: false,
                type: "POST",
                url: url,
                cache: false,
                data: data,
                error: function (errMsg) {alert(errMsg); },
                success: function (result) {
                    if (result >= 1) {
                        $("#monitorServerName_error").html('<%=CommonUtil.getMessage("W008","転送元名称")%>');
                        flg = true;
                    }
                    else if (result == 0) {
                        $("#monitorServerName_error").html('');
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
        /*
        path 要显示值的对象id
        ***
        function browseFolder(path) {
            try {
                var Message = "コンピューター"; //选择框提示信息
                var Shell = new ActiveXObject("Shell.Application");
                var Folder = Shell.BrowseForFolder(0, Message, 64, 17); //起始目录为：我的电脑
                //var Folder = Shell.BrowseForFolder(0,Message,0); //起始目录为：桌面
                if (Folder != null) {
                    Folder = Folder.items(); // 返回 FolderItems 对象
                    Folder = Folder.item(); // 返回 Folderitem 对象
                    Folder = Folder.Path; // 返回路径
                    if (Folder.charAt(Folder.length - 1) != "") {
                        Folder = Folder + "";
                    }
                    document.getElementById(path).value = Folder;
                    return;
                }
            }
            catch (e) {
                alert(e.message);
            }
        }*/
    </script>
    <div class="tab1Wrapper">
        <p class="sbTtl">【転送元設定】</p>
        <div class="edit2-2 FClear">
            <form name="ms_form" id="ms_form" action="" method="post">
                <dl class="editData2-2 OnCL FClear">
            	    <dt class="size1"><span style="color:Red; margin-right:5px;">※</span>転送元名称：</dt>
            	    <dd>
              		    <input type="text" name="monitorServerName" id="monitorServerName" size="40" style="width:300px;" />
                        <span id="monitorServerName_error" class ="spanPrompt"></span>
				    </dd>
			    </dl>
                <dl class="editData2-2 OnCL FClear">
            	    <dt class="size1"><span style="color:Red; margin-right:5px;">※</span>IPアドレス：</dt>
            	    <dd>
              		    <input type="text" name="monitorServerIP" id="monitorServerIP" size="40" style="width:300px;" />
                        <span id="monitorServerIP_error" class="spanPrompt"></span>
				    </dd>
			    </dl>
                <dl class="editData2-2 OnCL FClear" style="display:none">
                    <dt class="size1">転送環境：</dt>
                    <dd style="padding-top:3px">
                        <div style="width:310px; float:left;">
                            <input type="radio" name="monitorSystem" value="1" id="monitorSystem1" checked="checked"/><label for="monitorSystem1">MAC環境</label>
                            <input type="radio" name="monitorSystem" value="2" id="monitorSystem2" /><label for="monitorSystem2">WINDOWS環境</label>
                        </div>
                        <span id="monitorSystem_error" class="spanPrompt"></span>
                    </dd>
                </dl>
                <dl class="editData2-2 OnCL FClear">
            	    <dt class="size1">メモ：</dt>
            	    <dd>
              		    <input type="text" name="memo" id="memo" size="40" style="width:300px;" />
                        <span id="memo_error" class="spanPrompt"></span>
				    </dd>
			    </dl>
                <dl class="editData2-2 OnCL FClear">
            	    <dt class="size1"><span style="color:Red; margin-right:5px;">※</span>接続アカウント名：</dt>
            	    <dd>
              		    <input type="text" name="account" id="account" size="40" style="width:300px;" />
                        <span id="account_error" class="spanPrompt"></span>
				    </dd>
			    </dl>
                <dl class="editData2-2 OnCL FClear">
            	    <dt class="size1"><span style="color:Red; margin-right:5px;">※</span>パスワード：</dt>
            	    <dd class="PR10">
              		    <input type="password"  name="password" id="password" size="40" style="width:300px;" />
                        <span id="password_error" class="spanPrompt"></span>
				    </dd>
                    <dd><a href="javascript:funNetworkTest();"><img src="../../Content/Image/connectBtn.gif"alt="" /></a></dd>
			    </dl>
                <dl class="editData2-2 OnCL FClear">
            	    <dt class="size1"><span style="color:Red; margin-right:5px;">※</span>共有ポイント：</dt>
            	    <dd class="PR10">
              		    <input type="text" name="startFile" id="startFile" size="40" style="width:300px;" />
                        <span id="startFile_error" class="spanPrompt"></span>
				    </dd>
                    <%--<dd><a href="javascript:browseFolder('startFile');"><img src="../../Content/Image/settingBtn.gif"alt="" /></a></dd>--%>
			    </dl>
                <dl class="editData2-2 OnCL FClear" style="display:none">
                    <dt class="size1">ドライブ名：</dt>
                    <dd>
                        <select id="monitorDriveP" name="monitorDriveP" style="width:100px">
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
                <dl class="editData2-2 OnCL FClear">
                    <dt class="size1"><span style="color:Red; margin-right:5px;">※</span>MAC環境パス：</dt>
                    <dd class="PR10">
                        <input type="text" id="monitorMacPath" name="monitorMacPath" size="40" style="width:300px;" value="" /> 
                        <span class="spanPrompt" id="monitorMacPath_error"></span>
                    </dd>
                </dl>
                <dl class="editData2-2 OnCL FClear">
                    <dt class="size1"><span style="color:Red; margin-right:5px;">※</span>コピー先：</dt>
                    <dd class="PR10">
                        <input type="text" id="monitorLocalPath" name="monitorLocalPath" size="40" style="width:300px;" value="<%=ViewData["StartPath"] %>" /> 
                        <span class="spanPrompt" id="monitorLocalPath_error"></span>
                    </dd>
                    <dd><a href="javascript:browseFolder();"><img src="../../Content/Image/refBtn.gif"alt="" /></a></dd>
                </dl>
                <dl class="editData2-2 OnCL FClear">
                    <dt class="size1"><input type="checkbox" name="copyInit" id="copyInit" value="1"/></dt>
                    <dd class="PR10" style="line-height:21px;">
                        共有ポイント直下のファイルをコピーする
                    </dd>
                </dl>
                <ul class="twoBtns FClear">
       	            <li><a href="javascript:formSubmit(); "><img src="../../Content/Image/saveBtn.gif" class="img_on" alt=""/></a></li>
                    <li><a href="javascript:funCancel();"><img src="../../Content/Image/cancelBtn.gif" class="img_on" alt=""/></a></li>
                </ul>
            </form>
        </div>
    </div>
    <%  Html.RenderPartial("FolderBrowserPartial");  %>

    <script type="text/javascript">
        $(function () {
            //共有ポイント Text Changed Event
            $("#startFile").change(function () {
                $("#monitorLocalPath").val('<%=ViewData["StartPath"].ToString().Replace("\\", "\\\\") %>' + $("#startFile").val());
            });
        });
    </script>
</asp:Content>
