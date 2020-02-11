<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="Model" %>
<%@ Import Namespace="budbackup.CommonWeb" %>
<asp:Content ID="LogTitle" ContentPlaceHolderID="TitleContent" runat="server">
	BUD Backup System
</asp:Content>
<asp:Content ID="header" ContentPlaceHolderID="budHeader" runat="server">
<% Html.RenderPartial("LogonHeader"); %>
</asp:Content>

<asp:Content ID="LogContent" ContentPlaceHolderID="MainContent" runat="server">
<% Html.RenderPartial("TabControlList5"); %>
<script type="text/javascript">
    var timeClock;
    $(document).ready(function () {
        $("#tabProduct").jFixedtable({
            fixedCols: 0,
            width: "864",
            height: "300",
            headerRows: 1
        });
        //funReadLog();
        if ($('input:radio[name="rdDisplay"]:checked').val() == 'real') {
            timeClock = setInterval("funReadLogPage()", 30000);
        } else {
            clearInterval(timeClock);
        }
    });
    function timeClearSet(flag) {
        funReadLogPage();
        if (flag == 2) {
            timeClock = setInterval("funReadLogPage()", 30000);
        } else {
            clearInterval(timeClock);
        }
    }
    function funReadLogPage() {
        if (!funCheck()) {
            return;
        }
        $("#pindex").val(1);      
        var url = rootURL + "Log/ReadLog";
        var dtStart = $('#startDate').val();
        var dtEnd = $('#endDate').val();
        var tmStart = $('#startTime').val();
        var tmEnd = $('#endTime').val();
        var name = $('#name').val();
        var displayFlg = 0;
        var transferFlg = 0;
        var stateFlg = 2;
        var logFlg = 0;
        var groupId = $("#groupSelect").val();
        if ($('input:radio[name="rdDisplay"]:checked').val() == 'file') {
            displayFlg = 1;
        }
        if ($('input:radio[name="rdTransfer"]:checked').val() == 'transfering') {
            transferFlg = 1;
        }
        if ($('input:radio[name="rdState"]:checked').val() == 'stateNG') {
            stateFlg = 0;
        }
        if ($('input:radio[name="rdState"]:checked').val() == 'stateOK') {
            stateFlg = 1;
        }
        if ($('input:radio[name="rdLog"]:checked').val() == 'transferLog') {
            logFlg = 1;
        }
        var data = new Object();
        data.dtStart = dtStart;
        data.dtEnd = dtEnd;
        data.tmStart = tmStart;
        data.tmEnd = tmEnd;
        data.displayFlg = displayFlg;
        data.transferFlg = transferFlg;
        data.stateFlg = stateFlg;
        data.logFlg = logFlg;
        data.groupId = groupId;
        data.name = name;
        funShowLoading();
        if (logFlg == 0) {
            $('.logBox').show();
            $('.sizeBox').hide();
            GetListContent(1);
        } else {
            $.ajax({
                type: "POST",
                url: url,
                cache: false,
                data: data,
                error: function (errMsg) { funHideLoading(); },
                success: function (result) {
                    $('#PageTab').html("");
                    funHideLoading();
                    $('.sizeBox').show();
                    $('.logBox').hide();
                    showTransferLog(result);
                    if (result.length == 0) {
                        //alert('<%=CommonUtil.getMessage("W002")%>');
                    }
                }
            });
        }
    }
    function funReadLog() {
        if (!funCheck()) {
            return;
        }
        var url = rootURL + "Log/ReadLog";
        var dtStart = $('#startDate').val();
        var dtEnd = $('#endDate').val();
        var tmStart = $('#startTime').val();
        var tmEnd = $('#endTime').val();
        var name = $('#name').val();
        var displayFlg = 0;
        var transferFlg = 0;
        var stateFlg = 2;
        var logFlg = 0;
        var groupId = $("#groupSelect").val();
        if ($('input:radio[name="rdDisplay"]:checked').val() == 'file') {
            displayFlg = 1;
        }
        if ($('input:radio[name="rdTransfer"]:checked').val() == 'transfering') {
            transferFlg = 1;
        }
        if ($('input:radio[name="rdState"]:checked').val() == 'stateNG') {
            stateFlg = 0;
        }
        if ($('input:radio[name="rdState"]:checked').val() == 'stateOK') {
            stateFlg = 1;
        }
        if ($('input:radio[name="rdLog"]:checked').val() == 'transferLog') {
            logFlg = 1;
        }
        var data = new Object();
        data.dtStart = dtStart;
        data.dtEnd = dtEnd;
        data.tmStart = tmStart;
        data.tmEnd = tmEnd;
        data.displayFlg = displayFlg;
        data.transferFlg = transferFlg;
        data.stateFlg = stateFlg;
        data.logFlg = logFlg;
        data.groupId = groupId;
        data.name = name;
        funShowLoading();
        $.ajax({
            type: "POST",
            url: url,
            cache: false,
            data: data,
            error: function (errMsg) { funHideLoading(); },
            success: function (result) {
                funHideLoading();
                if (logFlg == 0) {
                    $('.logBox').show();
                    $('.sizeBox').hide();
                    showLog(result);
                    funLoadLog(1, data, result.length);
                }
                else {
                    $('.sizeBox').show();
                    $('.logBox').hide();
                    showTransferLog(result);
                    funLoadLog(0, data, result.length);
                }
                if (result.length == 0) {
                    //alert('<%=CommonUtil.getMessage("W002")%>');
                }
            }
        });
    }
    function funLoadLog(logFlg, data, resultLength) {
        if (resultLength == 0) {
        //検索データがない場合
            return;
        }
        data.logFlg = logFlg;
        var url = rootURL + "Log/ReadLog";
        $.ajax({
            type: "POST",
            url: url,
            cache: false,
            data: data,
            error: function (errMsg) { alert(errMsg); },
            success: function (result) {
                if (logFlg == 0) {
                    showLog(result);
                }
                else {
                    showTransferLog(result);
                }
            }
        });
    }
    //絞り込みの入力条件のチェック
    function funCheck() {
        var flag = true;
        var dtStart = $('#startDate').val();
        var dtEnd = $('#endDate').val();
        var tmStart = $('#startTime').val();
        var tmEnd = $('#endTime').val();
        //if (dtStart != "" && dtEnd != "") {
            //if (dtStart > dtEnd) {
                //alert('<%=CommonUtil.getMessage("W005","日付")%>');
                //return false;
            //}
        //}
        //else if (dtStart != "" || dtEnd != "") {
            //if (dtStart == "") {
                //alert('<%=CommonUtil.getMessage("W004","開始日付")%>');
                //return false;
            //}
            //else if (dtEnd == "") {
                //alert('<%=CommonUtil.getMessage("W004","終了日付")%>');
                //return false;
            //}
        //}
        //else {
            //alert('<%=CommonUtil.getMessage("W004","日付")%>');
            //return false;
        //}
        if(tmStart !="" && tmEnd !="")
        {
            if (!newFunCheckTime(tmStart)) {
                alert('<%=CommonUtil.getMessage("W005","開始時間")%>');
                return false;
            }
            if (!newFunCheckTime(tmEnd)) {
                alert('<%=CommonUtil.getMessage("W005","終了時間")%>');
                return false;
            }
            else if (parseInt(tmStart) > parseInt(tmEnd)) {
                alert('<%=CommonUtil.getMessage("W005","時間")%>');
                return false;
            }
        }
        else if (tmStart.trim() != "" || tmEnd.trim() != "") {
            if (tmStart.trim() == "") {
                alert('<%=CommonUtil.getMessage("W003","開始時間")%>');
                return false;
            }
            else if (tmEnd.trim() == "") {
                alert('<%=CommonUtil.getMessage("W003","終了時間")%>');
                return false;
            }
        }
        return true;
    }
    function checkIsDate(obj) {
        try {
            var newObj = new Date(obj);
            return true;
        }
        catch (e) {
            alert(e.Message);
            return false;
        }
    }
    //ログ・転送容量表示の変換
    function funShowLog_div(obj) {
        if ($(obj).val() == 'log') {
            //ログ表示
            $('.logBox').show();
            $('.sizeBox').hide();
        }
        else {
            //転送容量表示
            $('.sizeBox').show();
            $('.logBox').hide();
            
        }
    }
</script>
<div class="tab1Wrapper">
    <div class="edit3-1 FClear">
        <div class="MB20 FClear">
            <% List<Log> logList = (List<Log>)ViewData["Log"]; %>
            <% List<BackupServerGroup> groupList = (List<BackupServerGroup>)ViewData["groupList"]; %>
            <form id="log_form" action="">
                <dl class="editData3-1 OnCL FClear">
                    <dt class="size1" style="width: 104px;">転送先：</dt>
                    <dd>
                        <select id="groupSelect" class="select1" style="width:206px;">
                            <option value="0">--すべて</option>
                            <% for (int i = 0; groupList != null && i < groupList.Count; i++)
                                { %>
                                <option value="<%:groupList[i].id %>"><%:groupList[i].backupServerGroupName%></option>
                                <%} %>
                        </select>
                    </dd>
                    <dt>日付：</dt>
                    <% string today = string.Format("{0:yyyy-MM-dd}", DateTime.Now); %>
                    <dd><input id="startDate" name="startDate" type="text" size="40" style="width:90px;" onclick="WdatePicker()"value="<%:today %>" />〜<input id="endDate" name="endDate" onclick="WdatePicker()" size="40" style="width:90px;" type="text" value="<%:today %>" />
                    </dd>
                    <dt>時間：</dt>
                    <dd><input id="startTime" name="startTime" size="40" style="width:90px;" type="text" />〜<input id="endTime" name="endTime" size="40" style="width:90px;" type="text" />
                    </dd>
                </dl>
                <dl class="editData3-1 OnCL FClear OnFLeft">
                    <dt class="size1" style="width: 104px;">ログファイル名：</dt>
           	        <dd><input type="text" name="name" id="name" size="40" style="width:200px;" /></dd>
                </dl>
                <p class="OnFRight"><a href="javascript:funReadLogPage();"><img src="../../Content/Image/refineBtn.gif" class="img_on" alt=""/></a></p>
            </form>
        </div>
        <div class="displayOption OnCL">
            <ul class="typeCheck FClear" style="display:none">
                <li class="PR20"><input type="radio" name="rdDisplay" value="real" id="1a" onclick="timeClearSet(2)" /> <label for="1a">リアルタイム表示</label></li>
                <li><input type="radio" name="rdDisplay" value="file" id="1b" checked="checked" onclick="timeClearSet(1)"/> <label for="1b">ファイル表示</label></li>
        	</ul>
            <ul class="typeCheck FClear" style="display:none">
                <li class="PR20"><input type="radio" name="rdTransfer" value="transfered" id="2a" checked="checked" onclick="funReadLogPage(1)"/> <label for="2a">転送中・完了両方</label></li>
                <li><input type="radio" name="rdTransfer" value="transfering" id="2b" onclick="funReadLogPage(1)"/> <label for="2b">転送中のみ</label></li>
        	</ul>
            <ul class="typeCheck FClear">
               
                <li class="PR20"><input type="radio" name="rdState"value="stateOK" checked="checked" id="3c" onclick="funReadLogPage(1)" /> <label for="3c">OKのみ</label></li>
                <li><input type="radio" name="rdState"value="stateNG" id="3b" onclick="funReadLogPage(1)" /> <label for="3b">NGのみ</label></li>
        	</ul>
            <ul class="typeCheck FClear">
                <li class="PR20"><input type="radio" name="rdLog" value="log" id="4a" checked="checked" onclick="funReadLogPage()"/> <label for="4a">ログ</label></li>
                <li><input type="radio" name="rdLog" value ="transferLog" id="4b" onclick="funReadLogPage()"/> <label for="4b">転送容量</label></li>
        	</ul>
        </div>
        <!--ログ表示ここから-->
        <div class="logBox">
            <p class="sbTtl">ログ表示</p>
            <div class="logTableWrapper">
                <div class="list-c" style="width:964px;margin:20px auto;">
                <table border="0" cellspacing="0" cellpadding="0" class="logTable" id="tabProduct">
                    <tr>
                        <th scope="col" width="30" data-resizable-column-id="cel1">NO.</th>
                        <th scope="col" width="200" data-resizable-column-id="cel2">ファイル名</th>
                        <th scope="col" width="400" data-resizable-column-id="cel3">ファイルパス</th>
                        <th scope="col" width="80" data-resizable-column-id="cel4">ファイル容量</th>
                        <%--<th scope="col" width="120" data-resizable-column-id="cel5">コビー開始時間</th>
                        <th scope="col" width="120" data-resizable-column-id="cel6">コビー完了時間</th>--%>
                        <th scope="col" width="120" data-resizable-column-id="cel7">転送開始時間</th>
                        <%--<th scope="col" width="120" data-resizable-column-id="cel8">転送完了時間</th>--%>
                        <th scope="col" width="80" data-resizable-column-id="cel9" style="display: none;">転送時間</th>
                        <th scope="col" width="50" data-resizable-column-id="cel10">結果</th>
                        <th scope="col" width="100" data-resizable-column-id="cel11">ファイル状態</th>
                    </tr>
                </table>
                </div>
            </div>
        </div>
        <!--ログ表示ここまで-->

        <!--転送容量表示ここから-->
        <div class="sizeBox" style="display:none">
            <p class="sbTtl">転送容量表示</p>
            <div class="sizeTableWrapper">
                <table border="0" cellspacing="0" cellpadding="0" class="sizeTable">
                    <tr>
                        <th scope="col" width="100">日付</th>
                        <th scope="col" width="100">時間帯</th>
                        <th scope="col">処理ファイル数</th>
                        <th scope="col">転送容量</th>
                    </tr>
                </table>
            </div>
        </div>
        <!--転送容量表示ここまで-->
        <div class="table_page" id="PageTab">                
            
        </div>
        <input type="hidden" value="Log/ReadConLog" id ="hidden_url" />  
        <input type="hidden" id="pagesize"  name="pagesize"  value="20" />                        
        
        <%Html.RenderAction("CommonPaging", "Common");%>
    </div>
</div>
</asp:Content>
