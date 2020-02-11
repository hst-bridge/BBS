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
    //保存
    function formSubmit() {
        if (submitCheck() == true) {
            var url = rootURL + "ObjectSpy/Update";
            if (!confirm('<%=CommonUtil.getMessage("Q005") %>')) {
                //更新操作をキャンセルする時
                return;
            }
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
                        window.location.href = rootUrl + "Account/LogOn";
                    }
                    else if (result == -10) {//エラーがある
                        alert('<%=CommonUtil.getMessage("E001") %>');
                    }
                    else if (result == 0) {//成功
                        alert('<%=CommonUtil.getMessage("I001","更新")%>');
                        window.location.href = rootURL + "ObjectSpy/Index";
                    }
                    else {//失敗
                        alert('<%=CommonUtil.getMessage("I002","更新")%>');
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
        //$("#memo_error").html("");
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
            $("#startFile_error").html('<%=CommonUtil.getMessage("W003","転送フォルダ")%>');
        }
        else {
            $("#startFile_error").html("");
        }
        if ($("#monitorLocalPath").val().trim().length == 0) {
            flag = false;
            $("#monitorLocalPath_error").html('<%=CommonUtil.getMessage("W003","コピー先")%>');
        }
        else if (!checkDirPath($("#monitorLocalPath").val().trim())) {
            flag = false;
            $("#monitorLocalPath_error").html('<%=CommonUtil.getMessage("W005","コピー先")%>');
        }
        else {
            $("#monitorLocalPath_error").html("");
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
    //接続テストファンクション
    function funNetworkTest() {
        var url = rootURL + "ObjectSpy/NetworkTest";
        $.ajax({
            type: "POST",
            url: url,
            cache: false,
            data: $("#ms_form").serialize(),
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
        <p class="sbTtl">【転送元設定】</p>
        <div class="edit2-2 FClear">
        <% Model.MonitorServer msData = (Model.MonitorServer)ViewData["msData"]; %>
            <form name="ms_form" id="ms_form" action="" method="post">
                <dl class="editData2-2 OnCL FClear">
            	    <dt class="size1"><span style="color:Red; margin-right:5px;">※</span>転送元名称：</dt>
            	    <dd>
              		    <input type="text" name="monitorServerName" id="monitorServerName" size="40" style="width:300px;" value="<%:msData.monitorServerName %>" />
                        <span id="monitorServerName_error" class ="spanPrompt"></span>
				    </dd>
			    </dl>
                <dl class="editData2-2 OnCL FClear">
            	    <dt class="size1"><span style="color:Red; margin-right:5px;">※</span>IPアドレス：</dt>
            	    <dd>
              		    <input type="text" name="monitorServerIP" id="monitorServerIP" size="40" style="width:300px;" value="<%:msData.monitorServerIP %>"/>
                        <span id="monitorServerIP_error" class="spanPrompt"></span>
				    </dd>
			    </dl>
                <dl class="editData2-2 OnCL FClear" style="display:none">
                    <dt class="size1"><span style="color:Red; margin-right:5px;">※</span>転送環境：</dt>
                    <dd style="padding-top:3px">
                        <div style="width:310px; float:left;">
                            <input type="radio" name="monitorSystem" value="1" id="monitorSystem1"  checked="checked"/><label for="monitorSystem1">MAC環境</label>
                            <input type="radio" name="monitorSystem" value="2" id="monitorSystem2" /><label for="monitorSystem2">WINDOWS環境</label>
                        </div>
                        <span id="monitorSystem_error" class="spanPrompt"></span>
                    </dd>
                </dl>
                <dl class="editData2-2 OnCL FClear">
            	    <dt class="size1">メモ：</dt>
            	    <dd>
              		    <input type="text" name="memo" id="memo" size="40" style="width:300px;" value="<%:msData.memo %>"/>
                        <span id="memo_error" class="spanPrompt"></span>
				    </dd>
			    </dl>
                <dl class="editData2-2 OnCL FClear">
            	    <dt class="size1"><span style="color:Red; margin-right:5px;">※</span>接続アカウント名：</dt>
            	    <dd>
              		    <input type="text" name="account" id="account" size="40" style="width:300px;" value="<%:msData.account %>"/>
                        <span id="account_error" class="spanPrompt"></span>
				    </dd>
			    </dl>
                <dl class="editData2-2 OnCL FClear">
            	    <dt class="size1"><span style="color:Red; margin-right:5px;">※</span>パスワード：</dt>
            	    <dd class="PR10">
              		    <input type="password"  name="password" id="password" size="40" style="width:300px;" value="<%:msData.password %>"/>
                        <span id="password_error" class="spanPrompt"></span>
				    </dd>
                    <dd><a href="javascript:funNetworkTest();"><img src="../../Content/Image/connectBtn.gif"alt="" /></a></dd>
			    </dl>
                <dl class="editData2-2 OnCL FClear">
            	    <dt class="size1"><span style="color:Red; margin-right:5px;">※</span>共有ポイント：</dt>
            	    <dd class="PR10">
              		    <input type="text" id="startFile" name="startFile" size="40" style="width:300px;" value="<%:msData.startFile %>"/>
                        <span id="startFile_error" class="spanPrompt"></span>
				    </dd>
			    </dl>
                <dl class="editData2-2 OnCL FClear">
                    <dt class="size1"><span style="color:Red; margin-right:5px;">※</span>MAC環境パス：</dt>
                    <dd class="PR10">
                        <input type="text" id="monitorMacPath" name="monitorMacPath" size="40" style="width:300px;" value="<%:msData.monitorMacPath %>" /> 
                        <span class="spanPrompt" id="monitorMacPath_error"></span>
                    </dd>
                </dl>
                <dl class="editData2-2 OnCL FClear" style="display:none">
                    <dt class="size1">ドライブ名：</dt>
                    <dd>
                        <select id="monitorDriveP" name="monitorDriveP" style="width:100px" disabled="disabled">
                            <% for (char i = 'A'; i <= 'Z'; i++)
                                {
                                    %>
                            <option value="<%:i %>" <%if(i.ToString()==msData.monitorDriveP.TrimEnd(':')) {%>selected="selected" <%} %> ><%:i.ToString()%></option>
                            <%} %>
                        </select>
                        <input type="hidden" name="monitorDriveP" value="<%:msData.monitorDriveP %>" />
                    </dd>
                </dl>
                <dl class="editData2-2 OnCL FClear">
                    <dt class="size1"><span style="color:Red; margin-right:5px;">※</span>コピー先：</dt>
                    <dd class="PR10">
                        <input type="text" id="monitorLocalPath" name="monitorLocalPath" size="40" style="width:300px;" value="<%:msData.monitorLocalPath %>" /> 
                        <span class="spanPrompt" id="monitorLocalPath_error"></span>
                    </dd>
                    <dd><a href="javascript:browseFolder();"><img src="../../Content/Image/refBtn.gif"alt="" /></a></dd>
                </dl>
                <dl class="editData2-2 OnCL FClear">
                    <dt class="size1"><input type="checkbox" name="copyInit" <%if(msData.copyInit==1) {%>checked="checked"<%} %> id="copyInit" value="1" /></dt>
                    <dd class="PR10" style="line-height:21px;">
                        共有ポイント直下のファイルをコピーする
                    </dd>
                </dl>
                <ul class="twoBtns FClear">
       	            <li><a href="javascript:formSubmit(); "><img src="../../Content/Image/saveBtn.gif" class="img_on" alt=""/></a></li>
                    <li><a href="javascript:funCancel();"><img src="../../Content/Image/cancelBtn.gif" class="img_on" alt=""/></a></li>
                </ul>
                <input type="hidden" id="msId" name="msId" value="<%:msData.id %>" />
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
