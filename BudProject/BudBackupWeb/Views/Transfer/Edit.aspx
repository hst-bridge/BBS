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
    var initData = new Object();
    $(document).ready(function () {
        initData.backupServerName = $('#backupServerName').val();
        initData.backupServerIP = $('#backupServerIP').val();
        initData.memo = $('#memo').val();
        initData.account = $('#account').val();
        initData.password = $('#password').val();
        initData.startFile = $('#startFile').val();
    });
   //保存ファンクション
    function formSubmit() {
        if (submitCheck() == true) {
            var url = rootURL + "Transfer/Update";
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
        var IP = $("#backupServerIP").val().trim();
        var startFile = $("#startFile").val().trim();

        if ($("#backupServerName").val().trim().length == 0) {
            flag = false;
            $("#backupServerName_error").html('<%=CommonUtil.getMessage("W003","転送先名称")%>');
        }
        else if (funCheckIsExists($("#id").val(), $("#backupServerName").val())) {
            flag = false;
        }
        else {
            $("#backupServerName_error").html('');
        }
        if (IP.length == 0) {
            flag = false;
            $("#backupServerIP_error").html('<%=CommonUtil.getMessage("W003","IPアドレス")%>');
        }
        else if (!checkIP(IP)) {
            flag = false;
            $("#backupServerIP_error").html('<%=CommonUtil.getMessage("W005","IPアドレス")%>');
        }
        else {
            $("#backupServerIP_error").html('');
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
        if (startFile.length == 0) {
            flag = false;
            $("#startFile_error").html('<%=CommonUtil.getMessage("W003","開始フォルダ")%>');
        }
        else if (flag && (funCheckIPAndStartfolder($("#id").val(), IP, startFile) || funCheckDeletedBackupServer(IP, startFile))) {
            flag = false;
        }
        else {
            $("#startFile_error").html('');
        }
        return flag;
    }
    //キャンセル
    function funCancel() {
        if (funCheck_bkFromNoChange()) {
            window.location.href = rootURL + "Transfer/Index";
        }
        else {
            if (confirm('<%=CommonUtil.getMessage("Q002")%>')) {
                window.location.href = rootURL + "Transfer/Index";
            }
        }
    }
    //接続テスト
    function funNetworkTest() {
        if (submitCheck() == true) {
            var url = rootURL + "Transfer/NetworkTest";
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

    //Check that whether the name exists but this id
    function funCheckIsExists(objId, objName) {
        var url = rootURL + "Transfer/CheckNameIsExistsByIdName";
        var data = new Object();
        data.id = objId;
        data.bkName = objName;
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
                    $("#backupServerName_error").html('<%=CommonUtil.getMessage("W008","転送先名称")%>');
                    flg = true;
                }
                else if (result == 0) {
                    $("#backupServerName_error").html('');
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

    //Check IP And Start folder
    function funCheckIPAndStartfolder(objId, objIP, objStartfolder) {
        var url = rootURL + "Transfer/CheckIPAndStartfolder";
        var data = new Object();
        data.id = objId;
        data.bkIP = objIP;
        data.startFolder = objStartfolder;
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
                    $("#startFile_error").html('<%=CommonUtil.getMessage("W009")%>');
                    flg = true;
                }
                else if (result == 0) {
                    $("#startFile_error").html('');
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

    //Check Deleted Backup Server
    function funCheckDeletedBackupServer(objIP, objStartfolder) {
        var url = rootURL + "Transfer/CheckDeletedBackupServer";
        var data = new Object();
        data.bkIP = objIP;
        data.startFolder = objStartfolder;
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
                    flg = !confirm('<%=CommonUtil.getMessage("W010")%>');
                }
                else if (result == 0) {
                    $("#startFile_error").html('');
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
    ****/
    function browseFolder(path) {
        try {
            var Message = "/u8bf7/u9009/u62e9/u6587/u4ef6/u5939"; //选择框提示信息
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
    }
</script>
<div class="tab1Wrapper">
    <p class="sbTtl">【転送先設定·編集】</p>
    <div class="edit2-2 FClear">
        <% Model.BackupServer bkserver = (Model.BackupServer)ViewData["bkserver"]; %>
        <% List<Model.MonitorServer> msList = (List<Model.MonitorServer>)ViewData["msList"]; %>
        <% Model.BackupServerGroup model = (Model.BackupServerGroup)ViewData["model"]; %>
        <form name="bkserver_form" id="bkserver_form" action="" method="post">
            <input type="hidden" name="backupServerGroupName" id="backupServerGroupName" value="<%:model.backupServerGroupName %>"/>
            <dl class="editData2-2 OnCL FClear">
                <dt class="size1">転送元：</dt>
                <dd>
                    <select id="monitorServerID" name="monitorServerID" style="height: 21px; width: 306px;">
                    <% for (int i = 0; i < msList.Count; i++)
                        {%>
                        <option value="<%:msList[i].id %>" <% if(msList[i].id==model.monitorServerID) {%> selected="selected" <%} %>><%:msList[i].monitorServerName %></option>
                        <%} %>
                    </select>
                </dd>
            </dl>
            <dl class="editData2-2 OnCL FClear" style="display: none;">
                <dt class="size1"><span style="color:Red; margin-right:5px;">※</span>転送先名称：</dt>
                <dd>
                    <input type="text" name="backupServerName" id="backupServerName" size="40" style="width:300px;" value="<%:bkserver.backupServerName%>"/>
                    <span id="backupServerName_error" class ="spanPrompt"></span>
	            </dd>
            </dl>
            <dl class="editData2-2 OnCL FClear">
                <dt class="size1"><span style="color:Red; margin-right:5px;">※</span>IPアドレス：</dt>
                <dd>
                    <input type="text" name="backupServerIP" id="backupServerIP" size="40" style="width:300px;"  value="<%:bkserver.backupServerIP%>"/>
                    <span id="backupServerIP_error" class ="spanPrompt"></span>
	            </dd>
            </dl>
            <dl class="editData2-2 OnCL FClear">
                <dt class="size1">メモ：</dt>
                <dd>
                    <input type="text"  name="memo" id="memo" size="40" style="width:300px;" value="<%:bkserver.memo%>"/>
                    <span id="memo_error" class ="spanPrompt"></span>
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
                <dd class="PR10">
                    <input type="password" name="password" id="password" size="40" style="width:300px;" value="<%:bkserver.password%>"/>
                    <span id="password_error" class ="spanPrompt"></span>
	            </dd>
                <dd><a href="javascript:funNetworkTest();"><img src="../../Content/Image/connectBtn.gif" alt=""/></a></dd>
            </dl>
            <dl class="editData2-2 OnCL FClear">
                <dt class="size1"><span style="color:Red; margin-right:5px;">※</span>開始フォルダ：</dt>
                <dd class="PR10">
                    <input type="text" name="startFile" id="startFile" size="40" style="width:300px;" value="<%:bkserver.startFile%>"/>
                    <span id="startFile_error" class ="spanPrompt"></span>
	            </dd>
                <%--<dd><a href="javascript:browseFolder('startFile');"><img src="../../Content/Image/settingBtn.gif"alt="" /></a></dd>--%>
            </dl>
            <dl class="editData2-2 OnCL FClear" style="display: none;">
                <dt class="size1">SSB転送先のパス：</dt>
                <dd class="PR10">
                    <input type="text" name="ssbpath" id="ssbpath" size="40" style="width:300px;" value="" />
                    <span id="ssbpath_error" class ="spanPrompt"></span>
	            </dd>
                <dd>入力例：C:\Test</dd>
            </dl>
            <ul class="twoBtns FClear">
            <li><a href="javascript:formSubmit();"><img src="../../Content/Image/saveBtn.gif" class="img_on" alt=""/></a></li>
            <li><a href="javascript:funCancel();"><img src="../../Content/Image/cancelBtn.gif" class="img_on" alt=""/></a></li>
            </ul>
            <input type="hidden" id="id" name="id" value="<%:bkserver.id %>" />
            <input type="hidden" id="group_id" name="group_id" value="<%:model.id %>" />
        </form>
    </div>
</div>
<script type="text/javascript">
    //set backupServerName
    function set_backupServerName() {
        $("#backupServerName").val($("#monitorServerID option:selected").text());
    }
    $(function () {
        $("#monitorServerID").change(function () {
            set_backupServerName();
        });
    });
</script>
</asp:Content>
