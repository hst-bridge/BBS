String.prototype.trim = function () { return this.replace(/^\s+|\s+$/g, "") }

//日付フォーマット
Date.prototype.format = function (format) {
    var o = {
        "M+": this.getMonth() + 1, //month
        "d+": this.getDate(), //day
        "h+": this.getHours(), //hour
        "m+": this.getMinutes(), //minute
        "s+": this.getSeconds(), //second
        "q+": Math.floor((this.getMonth() + 3) / 3), //quarter
        "S": this.getMilliseconds() //millisecond
    }
    if (/(y+)/.test(format)) format = format.replace(RegExp.$1,
(this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o) if (new RegExp("(" + k + ")").test(format))
        format = format.replace(RegExp.$1,
RegExp.$1.length == 1 ? o[k] :
("00" + o[k]).substr(("" + o[k]).length));
    return format;
}
function funFormat(fdate) {
var fTime, fStr = 'ymdhis';
var result = fdate;
var formatStr= "yyyy/mm/dd hh:ii:ss";
if (fdate) {
    fTime = new Date(fdate);
}
else {
    var formatArr = [fTime.getFullYear().toString(),
    (fTime.getMonth() + 1).toString(),
    fTime.getDate().toString(),
    fTime.getHours().toString(),
    fTime.getMinutes().toString(),
    fTime.getSeconds().toString()]
    for (var i = 0; i < formatArr.length; i++) {
        result = formatStr.replace(fStr.charAt(i), formatArr[i]);
    }
}
return result;
}
//IPアドレスチェック
function checkIP(strIP) {
    var patt = new RegExp("([1-9]|[1-9]\\d|1\\d{2}|2[0-4]\\d|25[0-5])(\\.(\\d|[1-9]\\d|1\\d{2}|2[0-4]\\d|25[0-5])){3}");
    if (patt.test(strIP)) {//正確の場合
        return true;
    }
    else {//不正確の場合
        return false;
    }
}
//転送元設定 編集と新規ページ 未保存データがあるかをチェックする
function funCheck_msFormNoChange() {
    if ($("#monitorServerName").val().trim() != initData.monitorServerName) {
        return false;
    }
    if ($("#monitorServerIP").val().trim() != initData.monitorServerIP) {
        return false;
    }
    if ($("#memo").val().trim() != initData.memo) {
        return false;
    }
    if ($("#account").val().trim() !=  initData.account) {
        return false;
    }
    if ($("#password").val().trim() != initData.password) {
        return false;
    }
    if ($("#startFile").val().trim() != initData.startFile) {
        return false;
    }
//    if ($("#monitorDrive").val().trim() != initData.monitorDrive) {
//        return false;
//    }
//    if ($("#monitorLocalPath").val().trim() != initData.monitorLocalPath) {
//        return false;
//    }
    return true;
}
//転送先設定 編集と新規ページ 未保存データがあるかをチェックする
function funCheck_bkFromNoChange() {
    if ($("#backupServerName").val().trim() != initData.backupServerName) {
        return false;
    }
    if ($("#backupServerIP").val().trim() != initData.backupServerIP) {
        return false;
    }
    if ($("#memo").val().trim() != initData.memo) {
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
//    if ($("#ssbpath").val().trim() != initData.ssbpath) {
//        return false;
//    }
    return true;
}

//転送先設定 編集と新規ページ 未保存データがあるかをチェックする
function funCheck_groupFromNoChange() {
    if ($("#backupServerGroupName").val().trim() != initData.backupServerGroupName) {
        return false;
    }
    if ($("#memo").val().trim() != initData.memo) {
        return false;
    }
    if ($("#monitorServerID").val().trim() != initData.monitorServerID) {
        return false;
    }
    return true;
}
//転送フォルダ設定　選択・除外条件変更ページ
function funCheck_setFromNoChange()
{
    if ($("#exceptAttributeFlg1").val().trim() != initData.exceptAttributeFlg1) {
        return false;
    }
    if ($("#exceptAttributeFlg2").val().trim() != initData.exceptAttributeFlg2) {
        return false;
    }
    if ($("#exceptAttributeFlg3").val().trim() != initData.exceptAttributeFlg3) {
        return false;
    }
    if ($("#exceptAttribute1").val().trim() != initData.exceptAttribute1) {
        return false;
    }
    if ($("#exceptAttribute2").val().trim() != initData.exceptAttribute2) {
        return false;
    }
    if ($("#exceptAttribute3").val().trim() != initData.exceptAttribute3) {
        return false;
    }
    if ($("#systemFileFlg").val().trim() != initData.systemFileFlg) {
        return false;
    }
    if ($("#hiddenFileFlg").val().trim() != initData.hiddenFileFlg) {
        return false;
    }
    return true;
}
//ログページ　ログ表示の内容
function showLog(logListJson) {
    var tableTR = "";
    var thead = $('.logTableWrapper table tr:first').html();
    if (logListJson.length > 0) {
        //検索データがある場合
        var logList = eval(logListJson);
        var logTR = "";
        logTR = '<tr> ' +
                 '           <td class="cel1">@trId</td>' +
                 '           <td class="cel2">@backupServerFileName</td>' +
                 '           <td class="cel3">@backupServerFilePath</td>' +
                 '           <td class="cel4">@backupServerFileSize Byte</td>' +
                 '           <td class="cel5">@copyStartTime</td>' +
                 '           <td class="cel6">@copyEndTime</td>' +
                 '           <td class="cel7">@backupStartTime</td>' +
                 '           <td class="cel8">@backupEndTime</td>' +
                 '           <td class="cel9">@backupTime秒</td>' +
                 '           <td class="cel10">@state</td>' +
                 '       </tr>';
        var tr;
        for (var i = 0; i < logList.length; i++) {
            tr = logTR;
            tr = tr.replace('@backupServerFileSize', funCommafy(logList[i].backupServerFileSize));
            tr = tr.replace('@copyStartTime', funDateFromat(logList[i].copyStartTime));
            tr = tr.replace('@copyEndTime', funDateFromat(logList[i].copyEndTime));
            tr = tr.replace('@backupStartTime', funDateFromat(logList[i].backupStartTime));
            tr = tr.replace('@backupEndTime', funDateFromat(logList[i].backupEndTime));
            tr = tr.replace('@backupTime', funCommafy(logList[i].backupTime));
            tr = tr.replace('@trId', i + 1);
            tr = tr.replace('@state', logList[i].backupFlg == 1 ? 'OK' : 'NG');
            tr = tr.replace('@backupServerFileName', logList[i].backupServerFileName);
            tr = tr.replace('@backupServerFilePath', logList[i].backupServerFilePath);
            tableTR = tableTR + tr;
        }
    }
    $('.logTableWrapper table').html("");
    $('.logTableWrapper table').html('<tr class="tdn">' + thead + '</tr>' + tableTR);

}
function showLogNew(logListJson) {
    var tableTR = "";
    var thead = $('.logTableWrapper table tr:first').html();
    if(typeof(logListJson)!="undefined"){
        if (logListJson.length > 0) {
            tableTR = logListJson;
        }
        $('.logTableWrapper table').html("");
        $('.logTableWrapper table').html('<tr class="tdn">' + thead + '</tr>' + tableTR);
    }

}
function searchResultShowNew(result) {
    var tableTR = "";
    var thead = $('#searchTable tr:first').html();
    if (result.length > 0) {
        tableTR = result
    }
    $('#searchTable').html("");
    $('#searchTable').html('<tr>' + thead + '</tr>' + tableTR);
}

//ログページ　転送容量表示の内容
function showTransferLog(transferLogListJson) {
    var tableTR = "";
    var thead = $('.sizeTableWrapper table tr:first').html();
    var transferLogTR = '<tr> ' +
                ' <td class="cel1">@transferDate</td>' +
                ' <td class="cel2">@transferTime時</td> ' +
                ' <td class="cel3">@transferFileCount</td>' +
                ' <td class="cel4">@transferFileSize Byte</td>' +
                '</tr>';

    //日期时间
    var startDate = $("#startDate").val().replace(/-/g, "/");
    var endDate = $("#endDate").val().replace(/-/g, "/");
    var startTime = $("#startTime").val();
    var endTime = $("#endTime").val();
    if(startTime == "") {
        startTime = "00:00";
    }
    else if(startTime.indexOf(":") < 0) {
        startTime += ":00";
    }
    if(endTime == "") {
        endTime = "23:59";
    }
    else if(endTime.indexOf(":") < 0) {
        endTime += ":59";
    }
    var sd = Date.parse(startDate);
    var ed = Date.parse(endDate);
    var dateIntv = 24*3600*1000;
    var startHour = Number(startTime.substr(0, startTime.indexOf(":")));
    var endHour = Number(endTime.substr(0, endTime.indexOf(":")));
    //从开始日期到结束日期，依时间段列出容量
    if (transferLogListJson.length > 0) {
        var transferLogList = eval(transferLogListJson);

        for (var i = sd; i <= ed; i += dateIntv) {

            tableTR = tableTR + '<tr class="tdn">' + thead + '</tr>';
            var dt = new Date(i);
            var d = dt.format("yyyy/MM/dd");

            for (var j = startHour; j <= endHour; j++) {
                var t = j;
                var tr = transferLogTR;
                tr = tr.replace('@transferDate', d);
                tr = tr.replace('@transferTime', t);

                for (var k = 0; k < transferLogList.length; k++) {
                    if(transferLogList[k].transferDate == d && transferLogList[k].transferTime == t) {
                        tr = tr.replace('@transferFileCount', funCommafy(transferLogList[k].transferFileCount));
                        tr = tr.replace('@transferFileSize', funCommafy(transferLogList[k].transferFileSize));
                        break;
                    }
                }

                tr = tr.replace('@transferFileCount', 0);
                tr = tr.replace('@transferFileSize', 0);
                tableTR = tableTR + tr;
            }
        }
    }
    else {
        for (var i = sd; i <= ed; i += dateIntv) {

            tableTR = tableTR + '<tr class="tdn">' + thead + '</tr>';
            var dt = new Date(i);
            var d = dt.format("yyyy/MM/dd");

            for (var j = startHour; j <= endHour; j++) {
                var t = j;
                var tr = transferLogTR;
                tr = tr.replace('@transferDate', d);
                tr = tr.replace('@transferTime', t);
                tr = tr.replace('@transferFileCount', 0);
                tr = tr.replace('@transferFileSize', 0);
                tableTR = tableTR + tr;
            }
        }
    }
    $('.sizeTableWrapper table').html(tableTR);
}

//時間フォーマットのチェック
function funCheckTime(objTime) {
    var patt = new RegExp("^\[0-2]{1}\[0-6]{1}:\[0-5]{1}\[0-9]{1}:\[0-5]{1}\[0-9]{1}");
    if (patt.test(objTime)) {//正確の場合
        return true;
    }
    else {//不正確の場合
        return false;
    }
}
//時間フォーマットのチェック
function newFunCheckTime(objTime) {
    var patt = new RegExp("^\[0-2]{1}\[0-6]{1}:\[0-5]{1}\[0-9]{1}:\[0-5]{1}\[0-9]{1}");
    var patt2 = new RegExp("^\[0-2]{1}\[0-6]{1}:\[0-5]{1}\[0-9]{1}");
    var patt3 = new RegExp("^\[0-2]{1}\[0-6]{1}");
    var patt4 = new RegExp("^\[0-9]{1}");
    if (patt.test(objTime) || patt2.test(objTime) || patt3.test(objTime) || patt4.test(objTime)) {//正確の場合
        return true;
    }
    else {//不正確の場合
        return false;
    }
}
//日付フォーマット転換
function funDateFromat(obj) {
    try {
        var newObj = new Date(obj).format("yyyy/MM/dd hh:mm:ss");
        return newObj;
    }
    catch (e) {
        return obj;
    }
}

//千分位フォーマット転換
function funCommafy(s) {
    var type = 0;
    if (/[^0-9\.]/.test(s)) return "0";
    if (s == null || s == "") return "0";
    s = s.toString().replace(/^(\d*)$/, "$1.");
    s = (s + "00").replace(/(\d*\.\d\d)\d*/, "$1");
    s = s.replace(".", ",");
    var re = /(\d)(\d{3},)/;
    while (re.test(s))
        s = s.replace(re, "$1,$2");
    s = s.replace(/,(\d\d)$/, ".$1");
    if (type == 0) {// 小数がない(デフォールト)
        var a = s.split(".");
        if (a[1] == "00") {
            s = a[0];
        }
    }
    return s;
}

//転送ファルダ設定ページ 右側フォルダー詳細の表示
/*
* eventFlg :イベントフラグ 0:NodeClickイベント; 1:CheckedChangedイベント
*/
function funShowFolderDetail(detailJson,checkState,eventFlg) {
    var tableTR = "";
    var thead = $('#folderDetail table tr:first').html();
    if (detailJson.length > 0) {
        var detailList = eval(detailJson);
        var detailTR = '';
        var trFirsrTd = "";
        var tr = '';
        //20140227
        tr = tr + 
                '<tr>' +
//                ' <td class="cel1"><span class="@checkboxValue@" onclick="funSpanCheckedChanged(this)"> <input type="hidden" value="@fileExtensionType@"/></span></td>' +
                ' <td class="cel1"><span class="@checkboxValue@"> <input type="hidden" value="@fileExtensionType@"/></span></td>' +
                ' <td class="cel2">@fileName@</td> ' +
                ' <td class="cel3">@fileSize@</td>' +
                ' <td class="cel4">@update@</td>' +
                '</tr>';
        for (var i = 0; i < detailList.length; i++) {

            detailTR = tr;
            if (detailList[i].fileExtensionType == "99") {
            //フォルダーの場合
                detailTR = detailTR.replace('@fileName@', '<span><img src="../../Content/Image/iconFolder.gif" alt="" />&nbsp;</span><span class="trFileName">' + detailList[i].fileName + '</span>');
            }
            else {
                //ファイルの場合
                detailTR = detailTR.replace('@fileName@', '<span class="trFileName">' + detailList[i].fileName + '</span>');
            }
            detailTR = detailTR.replace('@fileSize@', detailList[i].fileSize);
            detailTR = detailTR.replace('@update@', detailList[i].fileLastUpdateTime);
            detailTR = detailTR.replace('@fileExtensionType@', detailList[i].fileExtensionType);
            if (eventFlg == 1) {
            //CheckedChanged
                if (checkState) {
                //チェックステータス
                    //                    detailTR = tr + ' <td><input type="checkbox" value="@checkboxValue@" onclick="funInputCheckChanged(this,\'@fileExtensionType@\')" checked="true"/></td>';
                    detailTR = detailTR.replace('@checkboxValue@', spanCheckedClass);
                }
                else {
                //未チェックステータス
                    //                    detailTR = tr + ' <td><input type="checkbox" value="@checkboxValue@" onclick="funInputCheckChanged(this,\'@fileExtensionType@\')"/></td>';
                    detailTR = detailTR.replace('@checkboxValue@', spanUnCheckedClass);
                }
            }
            else 
            {//nodeclick
                var checkFlg = funIsChecked(detailList[i].fileName, detailList[i].filePath);
                if (checkFlg == 1 || checkState) {
                //チェックステータス
                    detailTR = detailTR.replace('@checkboxValue@', spanCheckedClass);
                    //detailTR = tr + ' <td><span class="@checkboxValue@" onclick="funSpanCheckedChanged(this)"> <input type="hidden" value="@fileExtensionType@"/></span></td>';
                }
                else if (checkFlg == 0) {
                //未チェックステータス
                    detailTR = detailTR.replace('@checkboxValue@', spanUnCheckedClass);
                    //detailTR = tr + ' <td><input type="checkbox" value="@checkboxValue@" onclick="funInputCheckChanged(this,\'@fileExtensionType@\')"/></td>';
                }
                else if (checkFlg == 2) {
                    //不確定ステータス
                    detailTR = detailTR.replace('@checkboxValue@', spanIndeterminateClass);
                }

            }     
            tableTR = tableTR + detailTR;
        }
    }
    $('#folderDetail table').html("");
    $('#folderDetail table').html('<tr>' + thead + '</tr>' + tableTR);
}

function funShowFileTypeSet(jsonObj)
{
    var fileTypeSet = eval('[' + jsonObj + ']');
    if(fileTypeSet[0]==null)
    {
        $('#spanIncludeAttribute').html('');
        $('#spanExceptAttribute').html('');
        $('#systemfile').attr('checked',false);
        $('#hiddenfile').attr('checked',false);
    }
    else
    {
        //var includeAttribute = fileTypeSet[0].includeAttribute1 + ' ' + fileTypeSet[0].includeAttribute2 + ' ' + fileTypeSet[0].includeAttribute3;
        var exceptAttribute = "";
        var separator = " ";
        if(fileTypeSet[0].exceptAttributeFlg1 =="1")
        {
            exceptAttribute = exceptAttribute + fileTypeSet[0].exceptAttribute1 + separator;
        }
        if(fileTypeSet[0].exceptAttributeFlg2 =="1")
        {
            exceptAttribute = exceptAttribute + fileTypeSet[0].exceptAttribute2 + separator;
        }
        if(fileTypeSet[0].exceptAttributeFlg3 =="1")
        {
            exceptAttribute = exceptAttribute + fileTypeSet[0].exceptAttribute3 + separator;
        }
        //$('#spanIncludeAttribute').html(includeAttribute);
        $('#spanExceptAttribute').html(exceptAttribute.trim());
        if(fileTypeSet[0].systemFileFlg == '1')
        {
            $("#li_sysfile").css("display", "inherit");
            $('#systemfile').attr('checked',true);
        }
        else
        {
            $("#li_sysfile").css("display", "none");
            $('#systemfile').attr('checked',false);
        }
        if(fileTypeSet[0].hiddenFileFlg == '1')
        {
            $("#li_hidefile").css("display", "inherit");
            $('#hiddenfile').attr('checked',true);
        }
        else
        {
            $("#li_hidefile").css("display", "none");
            $('#hiddenfile').attr('checked',false);
        }
    }
}

//json⇒array変換
function funJsonToArray(jsonObj) {
    var newArray = new Array();
    if (jsonObj.length == 0) {
        return newArray;
    }
    var objList = eval(jsonObj);
    newArray = objList;
//    for(var obj in objList) {
    //        newArray[newArray.length] = objList[obj];
    //    }
    return newArray;
}
//チェックするか判断
function funIsChecked(monitorFileName,monitorFilePath)
{//flg 0:未チェック 1:チェック 2:不確定
    var flg = 0;
    if(saveDataArray.length > 0)
    {
        for (var i = 0; i < saveDataArray.length; i++) {
            if ((saveDataArray[i].monitorFileName == monitorFileName && saveDataArray[i].monitorFilePath == monitorFilePath)||(saveDataArray[i].monitorFileName == monitorFileName &&saveDataArray[i].monitorFilePath+ '\\' == monitorFilePath)) {
                flg = 1;
                break;
            }
            else if ((saveDataArray[i].monitorFilePath + '\\').indexOf(monitorFilePath + '\\' + monitorFileName + (monitorFileName == "" ? '' : '\\')) >= 0)
            {
                flg = 2;
            }
        }
     }
    return flg;
} 
//転送フォルダ設定ページ　使用
function funGetSpanCheckState(obj) {
    var checkedFlg = false;
    var checkClass = obj.attr("class");
    if (checkClass.indexOf('dxWeb_edtCheckBoxChecked') > 0) {
        checkedFlg = true;
    }
    else {
        checkedFlg = false;
    }
    return checkedFlg;
}
//転送フォルダ設定ページ　使用
function funSetSpanCheckState(obj, state) {
    if (state) {
        obj.attr("class", spanCheckedClass);
    }
    else {
        obj.attr("class", spanUnCheckedClass);
    }
}
//転送フォルダ設定ページ　右側フォルダ明細チェックステータス変更の時、使うファンクション
function funSpanCheckedChanged(obj) {
    var currentCheckedState = !funGetSpanCheckState($(obj));
    funSetSpanCheckState($(obj), currentCheckedState);
    var name = $(obj).parent().parent().children('td.cel2').children('span.trFileName').html()
    var nodeId = '#' + $('#currentNodeId').val();
    var checkSpanId = nodeId + '_D'
    var parentCheckSpanObj = $(checkSpanId);
    
    var nodeChildrenObj = funGetTreeViewNodeByText(dxo.rootNode, name, $('#currentFolderPath').val());
    if (nodeChildrenObj != null) {
    //ノードが存在する場合
        if (currentCheckedState) {
            nodeChildrenObj.SetCheckState("Checked");
        }
        else {
            nodeChildrenObj.SetCheckState("Unchecked");
        }
        var dataObj = new Object();
        dataObj.monitorFileName = name;
        dataObj.monitorFilePath = $('#currentFolderPath').val();
        dataObj.monitorFileType = $(obj).children('input').val();
        funDeleteDataFromArray(dataObj, 1);
    }
    else {
        //ノードが存在しない場合
        var nodeObj = funGetTreeViewNodeById(dxo.rootNode,$('#currentNodeId').val());
        if (nodeObj != null) {
            if (currentCheckedState) {
                //チェックの場合
                var dataObj = new Object();
                if (funIsALLChecked()) {
                    //全部チェックの場合
                    
                    nodeObj.SetCheckState("Checked");

                    //父のノードをチェックステータスをチェック
                    var parentObj = nodeObj;
                    while (parentObj.parent != null && parentObj.parent.GetChecked()) {
                        parentObj = parentObj.parent;
                    }
                    dataObj.monitorFileName = parentObj.text;
                    dataObj.monitorFilePath = parentObj.parent.name;
                    dataObj.monitorFileType = '99';
                }
                else {
                    //未全部チェックの場合
                    nodeObj.SetCheckState("Indeterminate");
                    dataObj.monitorFileName = name;
                    dataObj.monitorFilePath = $('#currentFolderPath').val();
                    dataObj.monitorFileType = $(obj).children('input').val();
                }
                funAddDataToArray(dataObj, 1);
            }
            else {
                //チェックをキャンセルする場合
                var dataObj = new Object();
                if (funIsALLUnChecked()) {
                    //全部未チェックの場合
                    nodeObj.SetCheckState("Unchecked");
                }
                else {
                    //部分チェックの場合
                    nodeObj.SetCheckState("Indeterminate");
                }
                dataObj.monitorFileName = name;
                dataObj.monitorFilePath = $('#currentFolderPath').val();
                dataObj.monitorFileType = $(obj).children('input').val();
                funDeleteDataFromArray(dataObj, 1);
            }
        }
    }
}
function funChangeParentNodeState(currentNodeId) {
    var parentNodeId = funGetparentNodeId(currentNodeId)
    if (parentNodeId != "") {
        var chkObj=$('#' + parentNodeId).children('span:first');
        chkObj.attr('class',chkObj.attr('class').replace('dxWeb_edtCheckBoxChecked','dxWeb_edtCheckBoxGrayed'));
        funChangeParentNodeState(parentNodeId);
    }
}

//カレントのパースとノードによって、父のパスと父のノードを取得する。
function getParentPathAndFileName(objPath,nodeId) {
    var strPath = "";
    var strFileName = "";
    var obj = new Object();
    var index = -1;
    var lastIndex = 0;
    index = objPath.indexOf("\\");
    var subObjPath = objPath;
    while (index >= 0) {
        if (lastIndex > 0) {
            lastIndex = lastIndex + index + 1;
        }
        else {
            lastIndex = lastIndex + index;
        }
        subObjPath = subObjPath.substr(index + 1);
        index = subObjPath.indexOf("\\");
    }
    if (lastIndex > 0) {
        obj.path = objPath.substr(0, lastIndex);
        obj.fileName = objPath.substr(lastIndex + 1);
    }
    obj.parendNodeId = funGetparentNodeId(nodeId); 
    return obj;
}

//カレントノードによって、父のノードを取得する。
function funGetparentNodeId(nodeId) {
    var parendNodeId = "";
    if (nodeId == 'tvVirtualMode_N0') {
        return parendNodeId;
    }
    var index = nodeId.indexOf("_"); ;
    var lastIndex = 0;
    var subNodeId = nodeId;
    while (index >= 0) {
        if (lastIndex > 0) {
            lastIndex = lastIndex + index + 1;
        }
        else {
            lastIndex = lastIndex + index;
        }
        subNodeId = subNodeId.substr(index + 1);
        index = subNodeId.indexOf("_");
    }
    if (lastIndex > 0) {
        parendNodeId = nodeId.substr(0, lastIndex);
    }
    return parendNodeId;
}

//ファイル、フォルダのチェックのとき
function funAddDataToArray(obj, delFlg) {

    if (delFlg != 0) {
        funDeleteDataFromArray(obj, 0);
    }
    saveDataArray[saveDataArray.length] = obj
}
//チェックのファイル、フォルダをキャンセルするとき
function funDeleteDataFromArray(obj, addFlg) {
    var count = saveDataArray.length;
    var delFlg = false;
    var delParentPathIndex = -1;
    var parentFileName = $('#' + $('#currentNodeId').val()).children('span.dxtv-ndTxt').html();
    var parentFilePath = $('#parentFolderPath').val();
    var nodeObj = funGetTreeViewNodeById(dxo.rootNode, $('#currentNodeId').val());
    var delIndexArr = new Array();
    for (var i = 0; i < count; i++) {
        if ((saveDataArray[i].monitorFileName == obj.monitorFileName && saveDataArray[i].monitorFilePath == obj.monitorFilePath)||(saveDataArray[i].monitorFileName == obj.monitorFileName &&saveDataArray[i].monitorFilePath+ '\\' == obj.monitorFilePath)) {
            saveDataArray.splice(i, 1);
            delFlg = true;
            break;
        }

        if (obj.monitorFileType == '99') {
            if ((saveDataArray[i].monitorFilePath + '\\').indexOf(obj.monitorFilePath + '\\' + obj.monitorFileName + (obj.monitorFileName == "" ? '' : '\\')) == 0) {
                //子フォルダ或いは子ファイルのindex
                delIndexArr.push(i);
            }
        }
        if (nodeObj != null && saveDataArray[i].monitorFileName == nodeObj.text && saveDataArray[i].monitorFilePath == nodeObj.parent.name) {
            delParentPathIndex = i;
        }
    }
    while (delIndexArr.length > 0) {
        //子フォルダ或いは子ファイルを削除。
        saveDataArray.splice(delIndexArr.pop(), 1);
    }

    if (nodeObj != null && addFlg != 0 && !delFlg) {
        funDeleteParentFolder(nodeObj);
    }
}
//父ノードの処理
function funDeleteParentFolder(parentNode) {
    var findFlg = false;
    var obj = new Object();
    for (var i = 0; i < saveDataArray.length; i++) {
        if (parentNode.parent!=null && saveDataArray[i].monitorFilePath == parentNode.parent.name && saveDataArray[i].monitorFileName == parentNode.text) {
            saveDataArray.splice(i, 1);
            findFlg = true;
            break;
        }
        else if(parentNode.parent==null && saveDataArray[i].monitorFilePath == parentNode.name)
        {//ルートノード
            saveDataArray.splice(i, 1);
            findFlg = true;
            break;
        }
    }
    if (parentNode.GetCheckState() != "Unchecked" && parentNode.name == $('#currentFolderPath').val() && !parentNode.GetExpanded()) {
        //カレントノード
        $("#folderDetail table tr td.cel1 span").each(function () {
            var dataObj = new Object();
            dataObj.monitorFileName = $(this).parent().parent().children('td.cel2').children('span.trFileName').html();
            dataObj.monitorFilePath = parentNode.name; // $('#currentFolderPath').val();
            dataObj.monitorFileType = $(this).children('input').val();
            var checkState = funGetSpanCheckState($(this));
            if (checkState) {
                funAddDataToArray(dataObj, 0);
            }
        });
    }
    else {
        for (i = 0; i < parentNode.nodes.length; i++) {
            if (parentNode.nodes[i].GetChecked()) {
                var dataObj = new Object();
                //2014-06-11 wjd modified
                var nodeText = parentNode.nodes[i].text;
                dataObj.monitorFileName = nodeText;
                dataObj.monitorFilePath = parentNode.name; // $('#currentFolderPath').val();
                var nodeTarget = parentNode.nodes[i].target;
                dataObj.monitorFileType = nodeTarget == "" ? nodeText.substr(nodeText.lastIndexOf(".")) : nodeTarget;
                funAddDataToArray(dataObj, 0);
            }
        }
    }
    if (!findFlg && parentNode.parent != null) {
        funDeleteParentFolder(parentNode.parent);
    }
}
/*
//父ノードの処理---(廃棄)
function funSearchParentPath(parentPath, parentFileName, parentNodeId) {
    var findFlg = false;
    var obj = new Object();
    for (var i = 0; i < saveDataArray.length; i++) {
        if (saveDataArray[i].monitorFilePath == parentPath && saveDataArray[i].monitorFileName == parentFileName) {
            saveDataArray.splice(i, 1);
            findFlg = true;
            break;
        }
    }
    if (parentPath +"\\"+ parentFileName == $('#currentFolderPath').val()) {
        $('#folderDetail table .trFileName').each(function () {
            var dataObj = new Object();
            dataObj.monitorFileName = $(this).html();
            dataObj.monitorFilePath = parentPath; // $('#currentFolderPath').val();
            dataObj.monitorFileType = $(this).parent().children().children('span').children('input').val();
            var checkState = funGetSpanCheckState($(this).parent().children().children('span'));
            if (checkState) {
                funAddDataToArray(dataObj, 0);
            }
        });
    }
    else if(!findFlg){
        $('#' + parentNodeId).parent().children('ul').children('li').each(function () {

            var nodeObj = $(this).children('.dxtv-nd').children('span:first');
            var chkClass = nodeObj.attr('class');
            var chkState = false;
            if (chkClass.indexOf('dxWeb_edtCheckBoxChecked') > 0) {
                chkState = true;
            }
            if (chkState) {
                var dataObj = new Object();
                dataObj.monitorFileName = nodeObj.parent().children('span.dxtv-ndTxt').html();
                dataObj.monitorFilePath = parentPath; // $('#currentFolderPath').val();
                dataObj.monitorFileType = '99';
                funAddDataToArray(dataObj, 1);
            }
        });
        var objParent = getParentPathAndFileName(parentPath, parentNodeId);
        if (objParent.path != null && objParent.path != "") {
            funSearchParentPath(objParent.path, objParent.fileName, objParent.parendNodeId);
        }
    }
//    if (!findFlg) { //存在しない場合
//        var objParent = getParentPathAndFileName(parentPath, parentNodeId);
//        if (objParent.path != null && objParent.path != "") {
//            funSearchParentPath(objParent.path, objParent.fileName, objParent.parendNodeId);
//        }
//    }
    return findFlg;
}
*/
//ノードのDIV IDによって、ノードの対象を取得する。
function funGetTreeViewNodeById(rootNode,nodeId) {
//    var rootNode = dxo.rootNode.nodes;
    var findNode = null;
    var flg = false;
    if (rootNode.nodes.length == 'undefined') {
        return findNode;
    }
    for (var i = 0; i < rootNode.nodes.length; i++) {
        if (rootNode.nodes[i].contentElementID == nodeId) {
            flg = true;
            findNode = rootNode.nodes[i];
            break;
        }
        else {
            findNode = funGetTreeViewNodeById(rootNode.nodes[i], nodeId);
            if (findNode != null) {
                break;
            }
        }
    }
    return findNode;
}
//ノードのテキストとnameによって、ノードの対象を取得する。
function funGetTreeViewNodeByText(rootNode,nodeText, nodeName) {
    var findNode = null;
    var flg = false;
    if (rootNode.nodes.length == 'undefined') {
        return findNode;
    }
    for (var i = 0; i < rootNode.nodes.length; i++) {
        if (rootNode.nodes[i].text == nodeText && rootNode.nodes[i].name == nodeName + '\\' + nodeText) {
            flg = true;
            findNode = rootNode.nodes[i];
            break;
        }
        else {
            findNode = funGetTreeViewNodeByText(rootNode.nodes[i], nodeText, nodeName);
            if (findNode != null) {
                break;
            }
        }
    }
    return findNode;
}
//指定ページへ
function funGotoPage(goUrl) {
    window.location.href = rootURL + goUrl;
}
//編集ページへ
function funGoEditPage(goUrl, id) {
    var url = rootURL + goUrl + "?id=" + id;
    window.location.href = url;
}
//パースのチェック
function checkDirPath(strDir) {
    //var regEx = /^[a-zA-Z]:[\\]((?! )(?![^\\/]*\s+[\\/])[\w -]+[\\/])*(?! )(?![^.]*\s+\.)[\w -]+$/;
    //var regEx = /^[a-zA-Z]:[\\]((?! )(?![^\\/]*\s+[\\/])[\w -]+[\\/])*(?! )(?![^.]*\s+\.)[\w -]+$/;
    var regEx = /^[a-zA-Z]:(((\\(?! )[^\\/:*?"<>|]+)+\\?)|(\\))\s*$/;
    if (regEx.test(strDir)) {//正確の場合
        return true;
    }
    else {//不正確の場合
        return false;
    }
}

//ロードDIV`を表示する
function funShowLoading() {
    $('#divLoading').height($(document.body).height());
        $('#divLoading').show();
    }
//ロードDIV`を隠します
function funHideLoading() {
    $('#divLoading').hide();
}