<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="budbackup.Models" %>
<script type="text/javascript" src="../../Scripts/TreeViewFolder.js"></script>
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
<div class="div_folder" id="mask_folder" style="z-index: 100; display: none; background-color: White;
    top: 0; left: 0; opacity: 0.5; width: 100%; height: 100%; position: absolute;">
</div>
<div class="browse OnCL FClear div_folder" style="display: none; position: absolute;
    background-color: White; z-index: 101; width: auto; border: 1px solid rgb(95, 151, 216);
    border-radius: 5px; left: 400px; top: 200px; padding-top: 0px;">
    <div style="padding: 10px 0px 10px 20px;">
        フォルダを選択してください。</div>
    <div class="left" style="border: 0px none; height: 300px; width: 450px; float: none;">
        <form action="" autocomplete="off" id="form_folder">
        <%  Html.RenderPartial("FolderSelectPartial");  %>
        </form>
    </div>
    <div id="div_btn">
        <div onclick="btnOK()" id="div_OK" onmouseover="btnokOver()" onmouseout="btnokOut()">
            <div style="width: 99px;">
                <input type="button" id="btnOK" value="確定" /></div>
            <div id="div_ok_bottom">
            </div>
        </div>
        <div onclick="btnCancel()" id="div_Cancel" onmouseover="btncancelOver()" onmouseout="btncancelOut()">
            <div style="width: 99px;">
                <input type="button" id="btnCancel" value="キャンセル" /></div>
            <div id="div_cancel_bottom">
            </div>
        </div>
    </div>
</div>
<input type="hidden" id="parentNodeId" name="parentNodeId" value="" />
<input type="hidden" id="currentNodeId" name="currentNodeId" value="" />
<input type="hidden" id="currentFolderPath" name="currentFolderPath" value="<%=ViewData["StartPath"] %>" />
<input type="hidden" id="parentFolderPath" name="parentFolderPath" value="" />
<style type="text/css">
    .dxICheckBox
    {
        display: none;
    }
    #div_btn
    {
        float: right;
        margin: 5px;
    }
    #div_btn input
    {
        border: 0 none;
        color: #FFFFFF;
        cursor: pointer;
        font-family: "hiragino maru gothic pro", メイリオ;
        font-size: 14px;
        font-weight: bold;
        height: 30px;
        margin: 0;
        width: 99px;
    }
    
    #div_OK
    {
        float: left;
        margin-right: 16px;
    }
    #div_OK #btnOK
    {
        background-color: rgb(51, 152, 219);
    }
    #div_OK #div_ok_bottom
    {
        background-color: rgb(40, 117, 168);
        cursor: pointer;
        height: 5px;
        width: 99px;
    }
    
    #div_Cancel
    {
        float: left;
    }
    #div_Cancel #btnCancel
    {
        background-color: rgb(196, 196, 196);
    }
    #div_Cancel #div_cancel_bottom
    {
        background-color: rgb(144, 144, 144);
        cursor: pointer;
        height: 5px;
        width: 99px;
    }
    
</style>
<script type="text/javascript">
    //mouse over and out effects
    function btnokOver() {
        $("#btnOK").css("margin-top", "5px");
        $("#div_ok_bottom").css("height", "0");
    }
    function btnokOut() {
        $("#btnOK").css("margin-top", "0");
        $("#div_ok_bottom").css("height", "5px");
    }
    function btncancelOver() {
        $("#btnCancel").css("margin-top", "5px");
        $("#div_cancel_bottom").css("height", "0");
    }
    function btncancelOut() {
        $("#btnCancel").css("margin-top", "0");
        $("#div_cancel_bottom").css("height", "5px");
    }

    function browseFolder() {
        $("#mask_folder").height($(document.body).height());
        $(".div_folder").show();
    }
    function btnOK() {
        $("#monitorLocalPath").val($("#currentFolderPath").val());
        $(".div_folder").hide();
    }
    function btnCancel() {
        $(".div_folder").hide();
    }

    var saveDataArray = new Array();

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
        $('#parentNodeId').val(parentNodeId);
        $('#currentNodeId').val(currentNodeId);
        $('#currentFolderPath').val(folderPath);
        $('#parentFolderPath').val(parentFolderPath);
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
            //add extension of the file——2014-06-03 wjd modified
            var suffix = $("#" + currentNodeId + "I").attr("alt");
            dataObj.monitorFileType = suffix == "" ? '99' : suffix;
            if (checkState) {
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
            if (checkState) {
                funAddDataToArray(dataObj, 1);
            }
            else {
                funDeleteDataFromArray(dataObj, 1);
            }
            funGetDetail(parentNodeId, currentNodeId, checkState, nodePath, 1, parentFolderPath);

        }
    }
</script>
