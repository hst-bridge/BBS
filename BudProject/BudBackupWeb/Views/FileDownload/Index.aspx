<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="budbackup.CommonWeb" %>
<asp:Content ID="FileDownloadTitle" ContentPlaceHolderID="TitleContent" runat="server">
	BUD Backup System
</asp:Content>
<asp:Content ID="header" ContentPlaceHolderID="budHeader" runat="server">
<% Html.RenderPartial("LogonHeader"); %>
</asp:Content>

<asp:Content ID="FileDownloadContent" ContentPlaceHolderID="MainContent" runat="server">
<% Html.RenderPartial("TabControlList6"); %>
    <script type="text/javascript">
        var connectflag = false;
        var saveDataArray = new Array();
        var checkDataArray = new Object();
        var initData = new Object();
        var searchFileName = "";
        function formSubmit_old() {
            if (submitCheck()) {
                var url = rootURL + "FileDownload/Search";
                funShowLoading();
                $.ajax({
                    type: "POST",
                    url: url,
                    cache: false,
                    data: $("#ms_form").serialize(),
                    error: function (errMsg) { alert(errMsg); funHideLoading(); },
                    success: function (result) {
                        funHideLoading();
                        if (result == "-99") {//未登録
                            alert('<%=CommonUtil.getMessage("W001","転送元設定") %>');
                            window.location.href = rootURL + "Account/LogOn";
                        }
                        else if (result == "-10") {//エラーがある
                            alert('<%=CommonUtil.getMessage("E001") %>');
                        }
                        else {
                            searchResultShow(result);
                        }
                    }
                });
            }
        }
        function formSubmit(flag) {
            var pindex = $("#pindex").val();
            if (flag == "search") {
                if ($("#monitorFileName").val().trim().length == 0) {
                    $("#monitorFileName_error").html('<%=CommonUtil.getMessage("W003","ファイル名/フォルダ名")%>');
                    return false;
                } else {
                    searchFileName = $("#monitorFileName").val();
                }
                pindex = 1;
            }
            if (submitCheck()) {   
                var url = "/" + $("#hidden_url").val();
                var monitorFileName = searchFileName;
                var monitorServerID = $("#monitorServerID").val();
                var dirname = "0";
                if ($("input[name='dirname']").attr("checked")) {
                    dirname = "1";
                }
                var data = new Object();
                data.monitorFileName = monitorFileName;
                data.monitorServerID = monitorServerID;
                data.pindex = pindex;
                funShowLoading();
                $.ajax({
                    type: "POST",
                    url: url,
                    cache: false,
                    data: data,
                    error: function (errMsg) { alert(errMsg); funHideLoading(); },
                    success: function (result) {
                        funHideLoading();
                        if (result == "-99") {//未登録
                            alert('<%=CommonUtil.getMessage("W001","転送元設定") %>');
                            window.location.href = rootURL + "Account/LogOn";
                        }
                        else if (result == "-10") {//エラーがある
                            alert('<%=CommonUtil.getMessage("E001") %>');
                        }
                        else {
                            UpdatePage(result);
                            $("input[name='selectAll']").attr("checked", false);
                            $("input[name='monitorFileLocalPath[]']").click(function () {
                                $("input[name='monitorFileLocalPath[]']").each(function () {
                                    if ($(this).attr("checked")) {
                                        checkDataArray[$(this).attr("title")] = $(this).val();
                                    }
                                    else {
                                        delete checkDataArray[$(this).attr("title")];
                                    }
                                });
                            });
                            if (flag == "search") {
                                checkDataArray = new Object();
                            }
                            $("input[name='monitorFileLocalPath[]']").each(function () {
                                for (var key in checkDataArray) {
                                    if (key == $(this).attr("title")) {
                                        $(this).attr("checked", true);
                                    }
                                }
                            });
                        }
                    }
                });
            }
        }
        function submitCheck() {
            var flag = true;
            if ($("#monitorFileName").val().trim().length == 0 && searchFileName =="") {
                flag = false;
                $("#monitorFileName_error").html('<%=CommonUtil.getMessage("W003","ファイル名/フォルダ名")%>');
            }
            else {
                $("#monitorFileName_error").html("");
            }
            

            return flag;
        }
        function searchResultShow(result) {
            var tableTR = "";
            var thead = $('#searchTable tr:first').html();
            if (result.length > 0) {
                //検索データがある場合
                var fileList = eval(result);
                var fileTR = "";
                fileTR = '<tr> ' +
                 '           <td class="cel1" style="text-align:center;line-height:28px;"><input type="checkbox" value=@monitorFileLocalPath class="filePath" name="monitorFileLocalPath[]"/></td>' +
                 '           <td class="cel2">@monitorFileName</td>' +
                 '           <td class="cel3">@monitorFileFullPath</td>' +
                
                 '       </tr>';
                var tr;
                for (var i = 0; i < fileList.length; i++) {
                    tr = fileTR;
                    
                    tr = tr.replace('@trId', fileList[i].id);
                    tr = tr.replace('@monitorFileFullPath', fileList[i].monitorFileFullPath);
                    tr = tr.replace('@monitorFileName', fileList[i].monitorFileName);
                    tr = tr.replace('@monitorFileLocalPath', fileList[i].monitorFileLocalPath);
                    tr = tr.replace('@monitorServerID', fileList[i].monitorServerID);
                    tableTR = tableTR + tr;
                }
            }
            $('#searchTable').html("");
            $('#searchTable').html('<tr>' + thead + '</tr>' + tableTR);
        }

        function getCookie(name) {
            var parts = document.cookie.split(name + "=");
            if (parts.length == 2) return parts.pop().split(";").shift();
        }

        function expireCookie(cName) {
            document.cookie =
          encodeURIComponent(cName) +
          "=deleted; expires=" +
          new Date(0).toUTCString();
        }

        function setFormToken() {
            var downloadToken = new Date().getTime();
            document.getElementById("downloadToken").value = downloadToken;
            return downloadToken;
        }

        var downloadTimer;
        function blockResubmit() {
            funShowLoading();
            var downloadToken = setFormToken();
            //test the cookie to check whether the file is sending
            downloadTimer = window.setInterval(function () {
                var token = getCookie("downloadToken");

                if (token == downloadToken) {
                    unblockSubmit();
                }

            }, 1000);
        }

        function unblockSubmit() {
            funHideLoading();
            window.clearInterval(downloadTimer);
            expireCookie("downloadToken");
        }
        function checkFileDownload() {
            var str = "";
            //var filePaths = $(".filePath");
            //for (var i = 0; i < filePaths.length; i++) {
            //    if (filePaths[i].checked) {
            //        str += filePaths[i].value + ",";
            //    }
            //}
            for (var key in checkDataArray) {
                str += checkDataArray[key] + ",";
            }
            if (str == "") {
                alert('<%=CommonUtil.getMessage("W004","ファイル名/フォルダ名")%>');
                return false;
            }
            else {
                $("#checkFilePath").val(str);
                blockResubmit();
                return true;
            }
        }
        function funSelectAll() {
            var obj = $("input[name='monitorFileLocalPath[]']");
            var all = $("input[name='selectAll']");
            if (typeof (obj) != "undefined" && $("input[name='selectAll']").attr("checked")) {
                obj.each(function () {
                    $(this).attr("checked", true);
                    checkDataArray[$(this).attr("title")] = $(this).val();
                })
                //$("input[name='monitorFileLocalPath[]']").attr("checked", true);
            } else {
                obj.each(function () {
                    $(this).attr("checked", false);
                    delete checkDataArray[$(this).attr("title")];
                })
                //$("input[name='monitorFileLocalPath[]']").attr("checked", false);
            }
        }
    </script>
    <div class="tab1Wrapper">
            <form name="ms_form" id="ms_form" action="" method="post">
            <% List<budbackup.Models.MonitorServer> msList = (List<budbackup.Models.MonitorServer>)ViewData["msList"]; %>
                <dl class="editData2-2 OnCL FClear">
            	    <dt class="size1" style="width: 180px;"><span style="color:Red; margin-right:5px;">※</span>ファイル名/フォルダ名：</dt>
            	    <dd class="PR10">
              		    <input type="text" name="monitorFileName" id="monitorFileName" size="40" style="width:260px;"/>
                         <!--<input type="checkbox" name="dirname" id="dirname" value="1"/><span style="font-weight:bold;">フォルダー名</span>-->
                        <span id="monitorFileName_error" class ="spanPrompt"></span>
				    </dd>
			    </dl>
                <!--<dl class="editData2-2 OnCL FClear">
            	    <dt class="size1" style="text-align:left;">転送日：</dt>
                    <% string today = string.Format("{0:yyyy-MM-dd}", DateTime.Now); %>
            	    <dd class="PR10">
              		    <input id="updateDate" name="updateDate" type="text" size="40" style="width:260px;" onclick="WdatePicker()"value="<%:today %>" readonly="readonly"/>
				    </dd>
                    <dd>
                        <input type="button" onclick="formSubmit()" name="search" id="search" value="検索" />
                    </dd>
			    </dl>-->
                <dl class="editData2-2 OnCL FClear">
            	    <dt class="size1" style="width: 180px;">転送元：</dt>
            	    <dd class="PR10">
              		    <select id="monitorServerID" name="monitorServerID" style="width:266px;height:20px;" >
                            <option value="-1">--すべて</option>
                            <% for (int i = 0; msList != null && i < msList.Count; i++){%>
                            <option value="<%:msList[i].DBServerIP + "|" + msList[i].ID %>"><%:msList[i].MonitorServerName %></option>
                            <%} %>
                        </select>
				    </dd>
                    <dd>
                        <input type="button" onclick="formSubmit('search')" name="search" id="search" value="検索" />
                    </dd>
			    </dl>
            </form>
            <form name="ms_form2" id="ms_form2" action="<%=budbackup.CommonWeb.VirtualPath.getVirtualPath()%>FileDownload/Download" method="post">
            <input type="hidden" id="downloadToken" name="downloadToken" value="" />
            <input  type="hidden" name="checkFilePath" id="checkFilePath" value=""/>
                <table border="1" cellspacing="0" cellpadding="0" class="table2-1" id="searchTable">
                    <tr>
                        <th scope="col" width="55">選択</th>
                        <th scope="col" width="55">区分</th>
                        <th scope="col" width="120">ファイル名/フォルダ名</th>
                        <th scope="col" width="170">ファイルパス</th>
                       
                    </tr>
                </table>
                <dl class="editData2-2 OnCL FClear">
            	    <dt class="size1" style="text-align:center;">
                        <input type="checkbox"  onclick="funSelectAll()" name="selectAll" id="Checkbox1"/><span style="font-weight:bold;">すべて選択</span>
                    </dt>
            	    <dd class="PR10" style="float:right;">
              		    <div class="table_page" id="PageTab" style="width:100%; margin-right:350px">                
                        </div>
                        <input type="hidden" value="FileDownload/Search_new" id ="hidden_url" />  
                        <input type="hidden" id="pagesize"  name="pagesize"  value="20" />                        
                        <%Html.RenderAction("CommonPaging", "Common");%>
				    </dd>
			    </dl>
                <%--<dl class="editData2-2 OnCL FClear">
            	    <dt class="size1"></dt>
            	    <dd class="PR10">
              		    <input type="checkbox"  onclick="funSelectAll()" name="selectAll" id="selectAll"/><span style="font-weight:bold;">すべて選択</span>
				    </dd>
			    </dl>--%>
                <dl class="editData2-2 OnCL FClear">
            	    <dt class="size1"></dt>
            	    <dd class="PR10" style="float:right;">
              		    <input type="submit" onclick="return checkFileDownload();" name="download" id="download" value="ダウンロード" style="width:100px;height:30px;"/>
				    </dd>
			    </dl>
                <!--転送容量表示ここまで-->
           </form>
    </div>
</asp:Content>
