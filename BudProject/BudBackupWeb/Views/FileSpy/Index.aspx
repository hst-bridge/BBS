<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="budbackup.CommonWeb" %>

<asp:Content ID="FileSpyTitle" ContentPlaceHolderID="TitleContent" runat="server">
	BUD Backup System
</asp:Content>
<asp:Content ID="header" ContentPlaceHolderID="budHeader" runat="server">
<% Html.RenderPartial("LogonHeader"); %>
</asp:Content>

<asp:Content ID="FileSpyContent" ContentPlaceHolderID="MainContent" runat="server">
<% Html.RenderPartial("TabControlList2"); %>
    <%Html.DevExpress().RenderStyleSheets(Page, 
          new StyleSheet { ExtensionSuite = ExtensionSuite.GridView },
          //new StyleSheet { ExtensionSuite = ExtensionSuite.HtmlEditor },
          //new StyleSheet { ExtensionSuite = ExtensionSuite.Editors },
          new StyleSheet { ExtensionSuite = ExtensionSuite.TreeList }
          
          ); %>
 <%Html.DevExpress().RenderScripts(Page,
          new Script { ExtensionSuite = ExtensionSuite.GridView },
          new Script { ExtensionSuite = ExtensionSuite.TreeList }
          ); %>
<script type="text/javascript">
    var saveDataArray = new Array();
    var savingFlg = false;
    $(document).ready(function () {
        var Obj = $('input#tvVirtualMode_NSHF');
        if (Obj.val() != null) {
            var objValue = eval(Obj.val());
            var path = "";
            if (objValue.length > 0) {
                path = objValue[objValue.length - 1].N0;
            }
            funShowLoading();
            $('#tvVirtualMode_CD').children('ul').children('li').children('span.dxtv-elbNoLn').children('img').click();
            //初期化選択済ファイルリスト取得
            funGetInitFolderDetail();
            funHideLoading();
        }

    });
    //保存
    function funSave() {
        var url = rootURL + "FileSpy/Add";
        var data = new Object();
        if (savingFlg) {
            alert('<%=CommonUtil.getMessage("I004") %>');
            return;
        }
        //saveDataArray[saveDataArray.length] = dataObj
        if (!confirm('<%=CommonUtil.getMessage("Q001") %>')) {
            //保存操作をキャンセルする時
            return;
        }
        data.dataJson = $.toJSON(saveDataArray);
        data.intMonitorServerID = $('#selMonitor').val();
//        alert(data1.join('-'));
        //        alert($.toJSON(data1));
        $('a#save').attr("disabled", false);

        savingFlg = true;
        funShowLoading();
        $.ajax({
            type: "POST",
            url: url,
            cache: false,
            data: data,
            error: function (errMsg) { alert(errMsg); funHideLoading(); },
            success: function (result) {
                savingFlg = false;
                funHideLoading();
                //saveDataArray = new Array();
                $('a#save').attr("disabled", true);
                if (result == -99) {
                    alert('<%=CommonUtil.getMessage("W001","転送ファルダ設定") %>');
                    window.location.href = rootURL + "Account/LogOn";
                }
                else if (result == -10) {
                    alert('<%=CommonUtil.getMessage("E001") %>');
                }
                else if (result >= 0) {
                    alert('<%=CommonUtil.getMessage("I001","保存")%>');
                }
                else {
                    alert('<%=CommonUtil.getMessage("I002","保存")%>');
                }
            }
        });
    }
   
    //転送元変更のとき
    function funMointorChange() {
        var url = rootURL + "FileSpy/Index?monistorId="+$('#selMonitor').val();
        window.location.href = url;
    }
    //初期化フォルダデータを取得する
    function funGetInitFolderDetail() {
        var url = rootURL + "FileSpy/GetInitMonitorFolderDatail"
        var data = new Object();
        var flg = false;
        data.monistorId = $('#selMonitor').val();
        $.ajax({
            type: "POST",
            async: false,
            url: url,
            cache: false,
            data: data,
            error: function (errMsg) { alert(errMsg); funHideLoading(); },
            success: function (result) {
                saveDataArray = funJsonToArray(result)

                //dxo.InlineInitialize();
                var initRootNode = dxo.rootNode.nodes[0];
                var rootNodeState = false;
                if (initRootNode.name != null) {
                    var checkFlg = funIsChecked("", initRootNode.name);
                    if (checkFlg == 1) {
                        initRootNode.SetCheckState("Checked");
                        rootNodeState = true;
                    }
                    else if (checkFlg == 2) {
                        initRootNode.SetCheckState("Indeterminate");
                    }
                    setTimeout(funExpandPath, 500);
                }
                //hide folder and file details of right side——2014-06-02 wjd commented
//                var Obj = $('input#tvVirtualMode_NSHF');
//                if (Obj.val() != null) {
//                    var objValue = eval(Obj.val());
//                    var path = objValue[objValue.length - 1].N0;
//                    funGetDetail("", "tvVirtualMode_N0", rootNodeState, path, 0, "");
//                }
                flg = true;
            }
        });
        return flg;
    }
    function funEdit() {
        var url = 'FileSpy/Edit?msID=' + $('#selMonitor').val() + '&msrFolderName=' + $('#currentFolderPath').val();
        funGotoPage(url);
    }
    function funGetFileTypeSet() {
        var url = rootURL + "FileSpy/GetFileTypeSet";
        var data = new Object;
        data.msID = $('#selMonitor').val();
        data.folderName = $('#currentFolderPath').val();
        $.ajax({
            type: "POST",
            url: url,
            cache: false,
            async: false,
            data: data,
            error: function (errMsg) { alert(errMsg); },
            success: function (result) {
                if (result.length >= 0) {
                    funShowFileTypeSet(result);
                }
                else if (result == 0) {
                  
                }
                else if (result == -10) {
                    alert('<%=budbackup.CommonWeb.CommonUtil.getMessage("E001") %>');
                }
            }
        });
    }
    //获取所有父节点的除外条件——2014-8-30 wjd add
    function funGetBranchFileTypeSet(currentNodeId, nodePath) {
        var url = rootURL + "FileSpy/GetBranchFileTypeSet";
        var data = new Object;
        data.msID = $('#selMonitor').val();
        data.folderName = $('#currentFolderPath').val();
        $.ajax({
            type: "POST",
            url: url,
            cache: false,
            async: false,
            data: data,
            error: function (errMsg) { alert(errMsg); },
            success: function (result) {
                if (result.length >= 0) {
                    funRunBranchFileTypeSet(result, currentNodeId, nodePath);
                }
                else if (result == 0) {

                }
                else if (result == -10) {
                    alert('<%=budbackup.CommonWeb.CommonUtil.getMessage("E001") %>');
                }
            }
        });
    }

    function funRunBranchFileTypeSet(jsonObj, currentNodeId, nodePath) {
        var exts = eval(jsonObj);
        if (exts.length > 0) {
            $("#" + currentNodeId).parent().find(">ul>li").each(function () {

                var imgAlt = $(this).find(">div>img").attr("alt");
                var fileName = $(this).find(">div>span[class='dxtv-ndTxt']").text();

                for (var i = 0; i < exts.length; i++) {

                    //var spanObj = $(this).find(">div>span:first");
                    if (imgAlt != "" && exts[i] == imgAlt) {

                        var node = dxo.GetNodeByName(nodePath + "\\" + fileName);
                        if (node != undefined && node.GetChecked()) {

                            node.SetChecked(false);

                            var c_dataObj = new Object();
                            c_dataObj.monitorFileName = fileName;
                            c_dataObj.monitorFilePath = nodePath;
                            c_dataObj.monitorFileType = imgAlt;
                            funDeleteDataFromArray(c_dataObj, 1);
                        }
                    }
                }
            });
        }
    }

</script>
<div class="tab1Wrapper">
    <div class="box2-3LeftTop">
        <% List<Model.MonitorServer> monitorList = (List<Model.MonitorServer>)ViewData["monitorList"]; %>
        <% string initMonistorId = ViewData["initMonistorId"].ToString(); %>
        <form action="">
            <%--<dl class="editData2-3 OnCL FClear">
			    <dt class="size1" style=" width:160px; text-align:left;">転送フォルダ名：</dt>
			    <dd class="PR5" style=" line-height:27px;">
            	    <input type="text" name="monitorFolderName" id="monitorFolderName" size="40" style="width:180px;" />

			    </dd>
                <dd class="OnFRight"><a id="save" name="save" href="javascript:funSave();"><img src="../../Content/Image/saveBtn2.gif" class="img_on" alt=""/></a></dd>
		    </dl>--%>
            <dl class="editData2-3 OnCL FClear">
		    <dt class="size1" style=" text-align: center;">転送元：</dt>
                <dd class="PR5" style=" line-height:27px;">
                    <select class="select1" name="selMonitor"  id="selMonitor" onchange="funMointorChange()" style="width:186px;">
                       <% for (int i = 0; i < monitorList.Count; i++)
                            {%>
                            <option  value="<%:monitorList[i].id %>" <% if(monitorList[i].id==initMonistorId) {%> selected="selected" <%} %> >
                            <%:monitorList[i].monitorServerName%>
                            </option>
                            <%} %>
                    </select>
                </dd>
                <%--<dd class="OnFRight"><a href="javascript:funMointorChange()"> <img src="../../Content/Image/andoBtn.gif" class="img_on" alt=""/></a></dd>--%>
		    </dl>
        </form>
    </div>

    <div class="box2-3RightTop">
       <!--
        <p>選択条件：<span id="spanIncludeAttribute"></span></p>
        <form action="">
            <ul class="checkBox2-3 OnCL FClear">
                <!--<li><input type="checkbox" value="" name="allfile" id="allfile" /> <label for="systemfile">全て選択</label></li>
            </ul>
        </form>
        -->
        <p>除外条件：<span id="spanExceptAttribute"></span> </p>
        <form action="">
            <div class="checkBox2-3 OnCL FClear">
                <div class="div_fixed">
                    <div style="display: none;" id="li_sysfile">
                        <input type="checkbox" disabled="disabled" id="systemfile" value="systemfile" name="exceptAttribute" />
                        <label for="systemfile">システムファイル</label></div>
                </div>
                <div class="div_fixed">
                    <div style="display: none;" id="li_hidefile">
                        <input type="checkbox" disabled="disabled" id="hiddenfile" value="hiddenfile" name="exceptAttribute" />
                        <label for="hiddenfile">隠しファイル</label></div>
                </div>
                <div class="PR0" style="float: left;">
                    <a href="javascript:funEdit()"><img src="../../Content/Image/changeBtn.gif" class="img_on" alt="" /></a></div>
            </div>
        </form>
    </div>
    <div class="browse OnCL FClear">
        <div class="left" style="width:1006px;">
        <form action="" autocomplete="off">
            <%  Html.RenderPartial("VirtualModePartial");  %>
        </form>
        </div>
        <%--hide folder and file details of right side——2014-06-02 wjd commented--%>
        <!--右側-->
        <%--<%  Html.RenderPartial("EventMonitorPartial");  %>--%>
    </div>
    <div class="editInitData" style="width:auto;">
        <div class="btnLeft"><a id="save" name="save" href="javascript:funSave();"><img src="../../Content/Image/saveBtn2.gif" class="img_on" alt=""/></a></div>
        <div class="btnRight"><a href="javascript:funMointorChange()"> <img src="../../Content/Image/andoBtn.gif" class="img_on" alt=""/></a></div>
	</div>
</div>
<input type="hidden" id="parentNodeId" name="parentNodeId" value="" />
<input type="hidden" id="currentNodeId" name="currentNodeId" value="" />
<input type="hidden" id="currentFolderPath" name="currentFolderPath" value="" />
<input type="hidden" id="parentFolderPath" name="parentFolderPath" value="" />
<input type="hidden" id="expandedPath" name="expandedPath" value="<%=ViewData["expandedPath"] %>" />
<script type="text/javascript">
    //Pasted from EventMonitorPartial.ascx 2014-06-02 wjd
    //チェックステータスのクラス
    var spanCheckedClass = 'dxtv-ndChk dxICheckBox  dxWeb_edtCheckBoxChecked ';
    //未チェックステータスのクラス
    var spanUnCheckedClass = 'dxtv-ndChk dxICheckBox dxWeb_edtCheckBoxUnchecked';
    //不確定ステータスのクラス
    var spanIndeterminateClass = 'dxtv-ndChk dxICheckBox dxWeb_edtCheckBoxGrayed'

    /*
    //ファイル詳細明細を取得するファンクション
    * parentNodeId：父ノード
    * currentNodeId：カレントのノード
    * checkState:チェックステータス
    * folderPath：ファイルパース
    * eventFlg:イベントフラグ　0:NodeClickイベントから;　1:CheckedChanged（Treeviewイベント）のfunCheckChangedファンクションから
    */
    function funGetDetail(parentNodeId, currentNodeId, checkState, folderPath, eventFlg, parentFolderPath) {
//        var url = rootURL + "FileSpy/GetFolderDetail"
//        var data = new Object;
//        data.folderPath = folderPath;
//        data.checkstate = checkState;
//        data.msID = $('#selMonitor').val();
//        funShowLoading();
//        $.ajax({
//            type: "POST",
//            url: url,
//            cache: false,
//            data: data,
//            error: function (errMsg) { alert(errMsg); funHideLoading(); },
//            success: function (result) {
//                //                if (result != 'nodata') {
//                //                    alert(result);
//                //                    return;
//                //                }
//                funHideLoading();
//                if (result.length > 0) {

//                    //$('#folderDetail').html(result);
                    $('#parentNodeId').val(parentNodeId);
                    $('#currentNodeId').val(currentNodeId);
                    $('#currentFolderPath').val(folderPath);
                    $('#parentFolderPath').val(parentFolderPath);
//                    funShowFolderDetail(result, checkState, eventFlg);
                    funGetFileTypeSet();
                    
//                }
//                else if (result == 0) {
//                    $('#dirTreeView').html(result);
//                    var thead = $('#folderDetail table tr:first').html();
//                    $('#folderDetail table').html('<tr>' + thead + '</tr>');
//                }
//                else if (result == -10) {
//                    alert('<%=budbackup.CommonWeb.CommonUtil.getMessage("E001") %>');
//                }
//            }
//        });
//        return "detail";
    }

    /*
    *TreeViewチェック変更のファンクション
    *
    * parentNodeId：父ノード
    * currentNodeId：カレントのノード
    * checkState:チェックステータス
    * nodePath：ファイルパース
    * nodeName:ファイル名
    */
    function funCheckChanged(parentNodeId, currentNodeId, checkState, nodePath, nodeName, parentFolderPath) {
        if (currentNodeId == $('#currentNodeId').val()) {//カレントのノードを操作する時
            //$("#folderDetail :checkbox").attr("checked", checkState);
//            if (checkState) {
//                //$("#folderDetail span").attr('class', spanCheckedClass);
//                $("#folderDetail table tr td.cel1 span").attr('class', spanCheckedClass);
//            }
//            else {
//                //$("#folderDetail span").attr('class', spanUnCheckedClass);
//                $("#folderDetail table tr td.cel1 span").attr('class', spanUnCheckedClass);
            //            }

            funGetDetail(parentNodeId, currentNodeId, checkState, nodePath, 1, parentFolderPath);

            //saveDataArrayのデータを更新する
            var dataObj = new Object();
            if (parentFolderPath == null || parentFolderPath == "") {
                dataObj.monitorFileName = "";
                dataObj.monitorFilePath = nodePath;
            }
            else {
                dataObj.monitorFileName = nodeName;
                dataObj.monitorFilePath = parentFolderPath;
            }
            //add extension of the file——2014-06-03 wjd modified
            var suffix = $("#" + currentNodeId + "I").attr("alt");
            dataObj.monitorFileType = suffix == "" ? '99' : suffix;
            if (checkState != "Unchecked") {
                funAddDataToArray(dataObj, 1);
            }
            else {
                funDeleteDataFromArray(dataObj, 1);
            }
        }
        else {
            $('#parentNodeId').val(parentNodeId);
            $('#currentNodeId').val(currentNodeId);
            $('#currentFolderPath').val(nodePath);
            $('#parentFolderPath').val(parentFolderPath);

            funGetDetail(parentNodeId, currentNodeId, checkState, nodePath, 1, parentFolderPath);

            var dataObj = new Object();
            if (parentFolderPath == null || parentFolderPath == "") {
                dataObj.monitorFileName = "";
                dataObj.monitorFilePath = nodePath;
            }
            else {
                dataObj.monitorFileName = nodeName;
                dataObj.monitorFilePath = parentFolderPath;
            }
            //add extension of the file——2014-06-03 wjd modified
            var suffix = $("#" + currentNodeId + "I").attr("alt");
            dataObj.monitorFileType = suffix == "" ? '99' : suffix;
            if (checkState != "Unchecked") {
                funAddDataToArray(dataObj, 1);
            }
            else {
                funDeleteDataFromArray(dataObj, 1);
            }

        }

        removeExceptedFile(currentNodeId, checkState, nodePath);
        //        else if (parentNodeId == $('#currentNodeId').val()) {

        //            var obj = "#floderDetail :checkbox[name='" + nodeName + "']"
        //            $(obj).attr("checked", checkState);
        //        }
    }

    //remove files that excepted.——2014-06-11 wjd add
    var lastClickNode = "";
    function removeExceptedFile(currentNodeId, checkState, nodePath) {

        var suffix = $('#spanExceptAttribute').text().split(" ");
        if (checkState != "Unchecked" && suffix.length > 0 && lastClickNode != currentNodeId) {
            lastClickNode = currentNodeId;
            //$('#parentFolderPath').val(nodePath);

            $("#" + currentNodeId).parent().find(">ul>li").each(function () {

                var imgAlt = $(this).find(">div>img").attr("alt");
                var fileName = $(this).find(">div>span[class='dxtv-ndTxt']").text();

                for (var i = 0; i < suffix.length; i++) {

                    //var spanObj = $(this).find(">div>span:first");
                    if (imgAlt != "" && suffix[i] == imgAlt) {

                        var node = dxo.GetNodeByName(nodePath + "\\" + fileName);
                        if (node != undefined && node.GetChecked()) {

                            node.SetChecked(false);

                            var c_dataObj = new Object();
                            c_dataObj.monitorFileName = fileName;
                            c_dataObj.monitorFilePath = nodePath;
                            c_dataObj.monitorFileType = imgAlt;
                            funDeleteDataFromArray(c_dataObj, 1);
                        }
                    }
                }
            });
        }
        else {
            lastClickNode = "";
        }
    }

    //To expand the path that added the conditions of exceptions just now.——2014-06-17 wjd add
    function funExpandPath(){
    
        var path = $("#expandedPath").val();
        if(path != "")
        {
            var p1 = path.substr(path.indexOf("\\\\") + 2);
            var ps = p1.split("\\");
            var pss = new Array();
            var j = 0;
            for (var i = 0; i < ps.length; i++) {
                j = i;
                if(ps[i] == "") {
                    j -= 1;
                    continue;
                }
                if (i == 0) {
                    pss[j] = "\\\\" + ps[i];
                }
                else
                {
                    pss[j] = pss[j-1] + "\\" + ps[i];
                }
            }
            for (var i = 0; i < pss.length; i++) {
                var node = dxo.GetNodeByName(pss[i]);
                if (node != undefined && !node.GetExpanded()) {
                    //expand the treelist
                    var imgObj = $(node.GetHtmlElement()).parent().find(">span>img");
                    if (imgObj != null) {
                        imgObj.click();
                    } else {
                        node.SetExpanded(true);
                    }
                }
                if(i == pss.length - 1 && node != undefined)
                {
                    var spanObj = $(node.GetHtmlElement()).find("span:first");
                    spanObj.click();
                    if(node.GetChecked())
                    {
                        spanObj.click();
                    }
                    spanObj.parent().click();
                    node.SetChecked(true);
                    //clear
                    $("#expandedPath").val("");
                }
            }
        }

    }
</script>
</asp:Content>
