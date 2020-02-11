<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<script type="text/javascript">
    //* Cut to Index.aspx 2014-06-02 wjd
    //チェックステータスのクラス
    //var spanCheckedClass = 'dxtv-ndChk dxICheckBox  dxWeb_edtCheckBoxChecked ';
    //未チェックステータスのクラス
    //var spanUnCheckedClass = 'dxtv-ndChk dxICheckBox dxWeb_edtCheckBoxUnchecked';
    //不確定ステータスのクラス
    //var spanIndeterminateClass = 'dxtv-ndChk dxICheckBox dxWeb_edtCheckBoxGrayed'
    //var rootURL = "<=budbackup.Common.VirtualPath.getVirtualPath()%>";
    /*
    * Cut to Index.aspx 2014-06-02 wjd
    //ファイル詳細明細を取得するファンクション
    * parentNodeId：父ノード
    * currentNodeId：カレントのノード
    * checkState:チェックステータス
    * folderPath：ファイルパース
    * eventFlg:イベントフラグ　0:NodeClickイベントから;　1:CheckedChanged（Treeviewイベント）のfunCheckChangedファンクションから
    */
    /*
    function funGetDetail(parentNodeId, currentNodeId, checkState, folderPath,eventFlg,parentFolderPath) {
        var url = rootURL + "FileSpy/GetFolderDetail"
        var data = new Object;
        data.folderPath = folderPath;
        data.checkstate = checkState;
        data.msID = $('#selMonitor').val();
        funShowLoading();
        $.ajax({
            type: "POST",
            url: url,
            cache: false,
            data: data,
            error: function (errMsg) { alert(errMsg); funHideLoading(); },
            success: function (result) {
                //                if (result != 'nodata') {
                //                    alert(result);
                //                    return;
                //                }
                funHideLoading();
                if (result.length > 0) {

                    //$('#folderDetail').html(result);
                    $('#parentNodeId').val(parentNodeId);
                    $('#currentNodeId').val(currentNodeId);
                    $('#currentFolderPath').val(folderPath);
                    $('#parentFolderPath').val(parentFolderPath);
                    funShowFolderDetail(result, checkState, eventFlg);
                    funGetFileTypeSet();
                }
                else if (result == 0) {
                    $('#dirTreeView').html(result);
                    var thead = $('#folderDetail table tr:first').html();
                    $('#folderDetail table').html('<tr>' + thead + '</tr>');
                }
                else if (result == -10) {
                    alert('<%=budbackup.CommonWeb.CommonUtil.getMessage("E001") %>');
                }
            }
        });
        return "detail";
    }
    */
    /*
    * Cut to Index.aspx 2014-06-02 wjd
    *TreeViewチェック変更のファンクション
    *
    * parentNodeId：父ノード
    * currentNodeId：カレントのノード
    * checkState:チェックステータス
    * nodePath：ファイルパース
    * nodeName:ファイル名
    */
    /*——comment start——
    function funCheckChanged(parentNodeId, currentNodeId, checkState, nodePath, nodeName, parentFolderPath) 
    {
    if (currentNodeId == $('#currentNodeId').val()) {//カレントのノードを操作する時
    //$("#folderDetail :checkbox").attr("checked", checkState);
    if (checkState) {
    //$("#folderDetail span").attr('class', spanCheckedClass);
    $("#folderDetail table tr td.cel1 span").attr('class', spanCheckedClass);
    }
    else {
    //$("#folderDetail span").attr('class', spanUnCheckedClass);
    $("#folderDetail table tr td.cel1 span").attr('class', spanUnCheckedClass);
    }
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
    dataObj.monitorFileType = '99';
    if (checkState) {
    funAddDataToArray(dataObj,1);
    }
    else {
    funDeleteDataFromArray(dataObj,1);
    }
    //            $('#folderDetail table .trFileName').each(function () {
    //                var dataObj = new Object();

    //               // dataObj.monitorServerID = $('#selMonitor').val();
    //                dataObj.monitorFileName = $(this).html();
    //                dataObj.monitorFilePath = $('#currentFolderPath').val();
    //                dataObj.monitorFileType = $(this).parent().children().children('span').children('input').val();
    //                //dataObj.monitorFlg = '1';
    //                if (checkState) {
    //                    funAddDataToArray(dataObj);
    //                }
    //                else {
    //                    funDeleteDataFromArray(dataObj);
    //                }
    //            });
    }
    else {
    $('#parentNodeId').val(parentNodeId);
    $('#currentNodeId').val(currentNodeId);
    $('#currentFolderPath').val(nodePath);
    $('#parentFolderPath').val(parentFolderPath);
    var dataObj = new Object();
    if (parentFolderPath == null || parentFolderPath == "") {
    dataObj.monitorFileName = "";
    dataObj.monitorFilePath = nodePath;
    }
    else {
    dataObj.monitorFileName = nodeName;
    dataObj.monitorFilePath = parentFolderPath;
    }
    dataObj.monitorFileType = '99';
    if (checkState) {
    funAddDataToArray(dataObj, 1);
    }
    else {
    funDeleteDataFromArray(dataObj, 1);
    }
    funGetDetail(parentNodeId, currentNodeId, checkState, nodePath, 1, parentFolderPath);
           
    }
    //        else if (parentNodeId == $('#currentNodeId').val()) {

    //            var obj = "#floderDetail :checkbox[name='" + nodeName + "']"
    //            $(obj).attr("checked", checkState);
    //        }
    }
    *——comment end——
    */

    
//    function funInputCheckChanged(ckObj, ExtensionType) {
////        var obj = "#folderDetail :checkbox[name='" + name + "']"
//        var name = $(ckObj).parent().parent().children('.trFileName').html();
//        var nodeId = '#' + $('#currentNodeId').val();
//        var checkSpanId = nodeId+'_D'
//        var nodeObj = $(nodeId);
//        var parentCheckSpanObj = $(checkSpanId);
//        //console.info($(nodeId + '_0').html());
//        if (($(nodeId + '_0').html() != null)) {
//            //ノード展開済
//            //console.info(nodeObj.parent().html());
////            console.info(nodeObj.parent().children("span .dxtv-ndTxt"));
//            nodeObj.parent().children("ul").children('li').children('div').children('span.dxtv-ndTxt').each(function () {
//                if (($(this).html() == name)) {
//                    var nodeCheckSpanObj = $('#'+$(this).parent().attr('id') + '_D');
//                    if ($(ckObj).attr('checked')) {
//                        //チェック
//                        nodeCheckSpanObj.removeClass('dxWeb_edtCheckBoxUnchecked');
//                        nodeCheckSpanObj.removeClass('dxWeb_edtCheckBoxGrayed');
//                        nodeCheckSpanObj.addClass('dxWeb_edtCheckBoxChecked');
//                    }
//                    else { //チェックのキャンセル
//                        nodeCheckSpanObj.removeClass('dxWeb_edtCheckBoxChecked');
//                        nodeCheckSpanObj.removeClass('dxWeb_edtCheckBoxGrayed');
//                        nodeCheckSpanObj.addClass('dxWeb_edtCheckBoxUnchecked');
//                    }
//                    return false;
//                }
//            });
//        }
//        else {
//            //ノード未展開
//        }

//        var dataObj = new Object();

//        dataObj.monitorServerID = $('#selMonitor').val();
//        dataObj.monitorFileName = name;
//        dataObj.monitorFilePath = $('#currentFolderPath').val();
//        dataObj.monitorFileType = ExtensionType;
//        dataObj.monitorFlg = '1';
//        if ($(ckObj).attr('checked')) {
//            //チェックの場合
////            alert('選択');
//            if (funIsALlChecked()) {
//                //全部チェックの場合
//                parentCheckSpanObj.removeClass('dxWeb_edtCheckBoxUnchecked');
//                parentCheckSpanObj.removeClass('dxWeb_edtCheckBoxGrayed');
//                parentCheckSpanObj.addClass('dxWeb_edtCheckBoxChecked');
//            }
//            else {
//            //未全部チェックの場合
//                parentCheckSpanObj.removeClass('dxWeb_edtCheckBoxUnchecked');
//                parentCheckSpanObj.removeClass('dxWeb_edtCheckBoxChecked');
//                parentCheckSpanObj.addClass('dxWeb_edtCheckBoxGrayed');
//            }
////            console.info('add');
////            console.info(dataObj);
//            funAddDataToArray(dataObj);
//        }
//        else {
//            //チェックをキャンセルする場合
//            //            alert('未選択');
//            if (funIsALlUnChecked()) {
//                //全部未チェックの場合
//                parentCheckSpanObj.removeClass('dxWeb_edtCheckBoxChecked');
//                parentCheckSpanObj.removeClass('dxWeb_edtCheckBoxGrayed');
//                parentCheckSpanObj.addClass('dxWeb_edtCheckBoxUnchecked');

//            }
//            else {
//                //部分チェックの場合
//                parentCheckSpanObj.removeClass('dxWeb_edtCheckBoxUnchecked');
//                parentCheckSpanObj.removeClass('dxWeb_edtCheckBoxChecked');
//                parentCheckSpanObj.addClass('dxWeb_edtCheckBoxGrayed');
//            }
////            console.info('delete');
////            console.info(dataObj);
//            funDeleteDataFromArray(dataObj);
//        }
//       
//        //console.info(nodeObj);
//    }
    //全部チェックステータスを判断
    function funIsALLChecked() {
        var flg = true;
        $("#folderDetail span.dxtv-ndChk").each(function () {
            if (!funGetSpanCheckState($(this))) {
                flg = false;
                return flg;
            }
        });
        return flg;
    }

    //全部未チェックステータスを判断
    function funIsALLUnChecked() {
        //alert('funIsALlChecked');
        var flg = true;
        $("#folderDetail span.dxtv-ndChk").each(function () {
            if (funGetSpanCheckState($(this))) {
                flg = false;
                return flg;
            }
        });
        return flg;
    }
//    function funChecked() {
//        alert(funGetSpanCheckState('#spanTest'));
//    }
//  
  
</script>
<div id="folderDetail" class="right">
    <table border="1" cellspacing="0" cellpadding="0" class="table2-3">
        <tr>
            <th scope="col" width="23">&nbsp;</th>
            <th scope="col" width="274">ファイル名</th>
            <th scope="col" width="90">サイズ</th>
            <th scope="col" width="107">更新日時</th>
        </tr>
    </table>
</div>
<input type="hidden" id="parentNodeId" name="parentNodeId" value="" />
<input type="hidden" id="currentNodeId" name="currentNodeId" value="" />
<input type="hidden" id="currentFolderPath" name="currentFolderPath" value="" />
<input type="hidden" id="parentFolderPath" name="parentFolderPath" value="" />
<div class="EventLogPanel">
    <div id="EventLog" style="width:520px; border:0px solid blue">
   <%-- <table>
        <tr>
        <td></td>
        <td>test</td>
        </tr>
        <tr>
        <td><span class="dxtv-ndChk dxICheckBox dxWeb_edtCheckBoxChecked" id="spanTest" onclick="funSpanCheckedChanged(this)"></span></td>
        <td><input type="button" value="选中" onclick="funChecked()" /></td>
        </tr>
    </table>--%>
    </div>
    <!-- <input type="button" value="Clear" onclick="DXEventMonitor.Clear()" /> -->
</div>


<div class="EventListPanel">

</div>
