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
        initData.ssbpath = $('#ssbpath').val();
    });
    //保存ファンクション
    function formSubmit() {
        if (submitCheck() == true) {
            var url = rootURL + "Transfer/Add";
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
                        window.location.href = rootURL + "Transfer/Index";
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
        var IP = $("#backupServerIP").val().trim();
        var startFile = $("#startFile").val().trim();

        if ($("#backupServerName").val().trim().length == 0) {
            flag = false;
            $("#backupServerName_error").html('<%=CommonUtil.getMessage("W003","転送先名称")%>');
        }
        else if (funCheckIsExists($("#backupServerName").val())) {
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
        else if (flag && (funCheckIPAndStartfolder(0, IP, startFile) || funCheckDeletedBackupServer(IP, startFile))) {
            flag = false;
        }
        else {
            $("#startFile_error").html('');
         }
//        if ($("#ssbpath").val().trim().length == 0) {
//            flag = false;
//            $("#ssbpath_error").html('<%=CommonUtil.getMessage("W003","SSB転送先のパス")%>');
//        }
//        else {
//            $("#ssbpath_error").html('');
//        }
        return flag;
    }
    function clearText() {
        $("#backupServerName").val("");
        $("#backupServerIP").val("");
        $("#memo").val("");
        $("#account").val("");
        $("#password").val("");
        $("#startFile").val("");
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
    function funCheckIsExists(objName) {
        var url = rootURL + "Transfer/CheckNameIsExists";
        var data = new Object();
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
    }
</script>
<div class="tab1Wrapper">
    <p class="sbTtl">【転送先設定·編集】</p>
    <div class="edit2-2 FClear">
    <% List<Model.MonitorServer> msList = (List<Model.MonitorServer>)ViewData["msList"]; %>
        <form name="bkserver_form" id="bkserver_form" action="" method="post">
            <dl class="editData2-2 OnCL FClear">
                <dt class="size1">転送元：</dt>
                <dd>
                    <select id="monitorServerID" name="monitorServerID" style="height: 21px; width: 306px;">
                    <% for (int i = 0; i < msList.Count; i++)
                        {%>
                        <option value="<%:msList[i].id %>"><%:msList[i].monitorServerName %></option>
                        <%} %>
                    </select>
                </dd>
            </dl>
            <dl class="editData2-2 OnCL FClear" style="display: none;">
                <dt class="size1"><span style="color:Red; margin-right:5px;">※</span>転送先名称：</dt>
                <dd>
                    <input type="text" name="backupServerName" id="backupServerName" size="40" style="width:300px;" />
                    <span id="backupServerName_error" class ="spanPrompt"></span>
	            </dd>
            </dl>
            <dl class="editData2-2 OnCL FClear">
                <dt class="size1"><span style="color:Red; margin-right:5px;">※</span>IPアドレス：</dt>
                <dd>
                    <input type="text" name="backupServerIP" id="backupServerIP" size="40" style="width:300px;" />
                    <span id="backupServerIP_error" class ="spanPrompt"></span>
	            </dd>
            </dl>
            <dl class="editData2-2 OnCL FClear">
                <dt class="size1">メモ：</dt>
                <dd>
                    <input type="text"  name="memo" id="memo" size="40" style="width:300px;" />
                    <span id="memo_error" class ="spanPrompt"></span>
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
                <!--<dd><a href="javascript:browseFolder('startFile');"><img src="../../Content/Image/settingBtn.gif"alt="" /></a></dd>-->
            </dl>
            <dl class="editData2-2 OnCL FClear" style="display: none;">
                <dt class="size1">SSB転送先のパス：</dt>
                <dd class="PR10">
                    <input type="text" name="ssbpath" id="ssbpath" size="40" style="width:300px;" />
                    <span id="ssbpath_error" class ="spanPrompt"></span>
	            </dd>
                <dd>入力例：C:\Test</dd>
            </dl>
            <dl class="editData2-2 OnCL FClear">
                <dd class="PR10">
                    <span style="color:Red; margin-left: 46px;">※</span>
                    <span id="startFile_info" class ="spanPrompt">転送先サーバーで上記「開始フォルダ」を共有設定してください。</span>
	            </dd>
            </dl>
            <ul class="twoBtns FClear">
            <li><a href="javascript:formSubmit();"><img src="../../Content/Image/saveBtn.gif" class="img_on" alt=""/></a></li>
            <li><a href="javascript:funCancel();"><img src="../../Content/Image/cancelBtn.gif" class="img_on" alt=""/></a></li>
            </ul>
        </form>
    </div>
</div>
<%
    object ip_startfolder = ViewData["IP_StartFolder"];
    Dictionary<string, string> ralate = null;
    string pairStr = "[]";
    if (ip_startfolder != null)
    {
        ralate = (Dictionary<string, string>)ip_startfolder;
        pairStr = JsonHelper.GetJson(ralate);
    }
%>
<script type="text/javascript">
    //set backupServerName
    function set_backupServerName() {
        $("#backupServerName").val($("#monitorServerID option:selected").text());
    }
    $(function () {
        set_backupServerName();
        $("#monitorServerID").change(function () {
            set_backupServerName();
        });

        //IP输入离开后，自动填充“開始フォルダ”
        var pairStr = '<%=pairStr %>';
        var jsontext = JSON.parse(pairStr);
        $("#backupServerIP").blur(function () {
            var backupName = $("#backupServerName").val();
            var ip = $("#backupServerIP").val();
            $("#startFile").val("");
            if (backupName != "" && ip != "") {
                for (var i = 0; i < jsontext.length; i++) {
                    if (jsontext[i].Key == ip) {
                        $("#startFile").val(jsontext[i].Value + "/" + backupName);
                        break;
                    }
                }
            }
        });
    });
</script>
</asp:Content>
