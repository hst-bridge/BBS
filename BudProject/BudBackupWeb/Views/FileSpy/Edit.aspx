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
<script type="text/javascript">
    var initData = new Object();
    $(document).ready(function () {
        initData.exceptAttributeFlg1 = $('#exceptAttributeFlg1').val();
        initData.exceptAttributeFlg2 = $('#exceptAttributeFlg2').val();
        initData.exceptAttributeFlg3 = $('#exceptAttributeFlg3').val();
        initData.exceptAttribute1 = $('#exceptAttribute1').val();
        initData.exceptAttribute2 = $('#exceptAttribute2').val();
        initData.exceptAttribute3 = $('#exceptAttribute3').val();
        initData.systemFileFlg = $('#systemFileFlg').val();
        initData.hiddenFileFlg = $('#hiddenFileFlg').val();
    });
　　//保存ファンクション
    function formSubmit() {
        if (!submitCheck()) {
            return;
        }
        if (!confirm('<%=CommonUtil.getMessage("Q001") %>')) {
            //更新操作をキャンセルする時
            return;
        }
        var url = rootURL + "FileSpy/AddFileTypeSet";
//        var data = new Object();
//        var model = new Object();
//        if ($('#filename1').attr('checked')) {
//            model.exceptAttributeFlg1 = $('#exceptAttributeFlg1').val();
//        }
//        if ($('#filename2').attr('checked')) {
//            model.exceptAttributeFlg2 = $('#exceptAttributeFlg2').val();
//        }
//        if ($('#filename3').attr('checked')) {
//            model.exceptAttributeFlg3 = $('#exceptAttributeFlg3').val();
//        }
//        if ($('#filename4').attr('checked')) {
//            model.exceptAttribute1 = $('#exceptAttribute1').val();
//        }
//        if ($('#filename5').attr('checked')) {
//            model.exceptAttribute2 = $('#exceptAttribute2').val();
//        }
//        if ($('#filename6').attr('checked')) {
//            model.exceptAttribute3 = $('#exceptAttribute3').val();
//        }
//        model.systemFileFlg = $('#systemFileFlg').val();
//        model.hiddenFileFlg = $('#hiddenFileFlg').val();
//        model.monitorServerID = $('#monitorServerID').val();
//        model.id = $('#id').val();
//        model.monitorServerFolderName = $('#monitorServerFolderName').val();
        //        data.jsonModel = $.toJSON(model);
        funShowLoading();
        $.ajax({
            type: "POST",
            url: url,
            cache: false,
            data: $("#set_from").serialize(),
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
                else if (result >= 0) {//成功
                    alert('<%=CommonUtil.getMessage("I001","保存")%>');
                    //return current extended path.——2014-06-17 wjd add
                    var url = "FileSpy/Index?monistorId=" + $('#monitorServerID').val() + "&expandedPath=" + $("#spanFolderName").text();
                    funGotoPage(url);
                    //window.location.href = rootURL + "FileSpy/Index";
                }
                else {//失敗
                    alert('<%=CommonUtil.getMessage("I002","保存")%>');
                }
            }
        });
    }
    //キャンセルファンクション
    function funCancel() {
        var url = "FileSpy/Index?monistorId=" + $('#monitorServerID').val();
        if (funCheck_setFromNoChange()) {
            funGotoPage(url);
        }
        else {
            if (confirm('<%=CommonUtil.getMessage("Q002")%>')) {
                funGotoPage(url);
            }
        }
    }
    //チェック変更のとき
    function fun_onclick(obj) {
        if ($(obj).attr('checked')) {
            $(obj).val('1');
            //$(obj).parent().next().children('input').attr('disabled', false);
        }
        else {
            $(obj).val('0');
            //$(obj).parent().next().children('input').val('');
            //$(obj).parent().next().children('input').attr('disabled', true);
        }
    }
    function fun_setSystemORHiddenFile(obj) {
        if ($(obj).attr('checked')) {
            $(obj).val('1');
        }
        else {
            $(obj).val('0');
        }
    }

    function submitCheck() {
        var flg = true;
        if ($('#exceptAttributeFlg1').attr('checked') && $("#exceptAttribute1").val().trim().length == 0) {
            flg = false;
            $('#exceptAttribute1_error').html('<%=CommonUtil.getMessage("W007","拡張子")%>');
        }
        else {
            $('#exceptAttribute1_error').html('');
        }
        if ($('#exceptAttributeFlg2').attr('checked') && $("#exceptAttribute2").val().trim().length == 0) {
            flg = false;
            $('#exceptAttribute2_error').html('<%=CommonUtil.getMessage("W007","拡張子")%>');
        }
        else {
            $('#exceptAttribute2_error').html(''); 
        }
        if ($('#exceptAttributeFlg3').attr('checked') && $("#exceptAttribute3").val().trim().length == 0) {
            flg = false;
            $('#exceptAttribute3_error').html('<%=CommonUtil.getMessage("W007","拡張子")%>');
        }
        else {
            $('#exceptAttribute3_error').html(''); 
        }
        return flg;
    }
</script>
<div class="subwinWrapper">
    <p class="sbTtl">フォルダ別フォルダ指定</p>
    <% Model.FileTypeSet setFile = (Model.FileTypeSet)ViewData["setFile"]; %>
    <form name="set_from" id="set_from" action="">
        <div class="subwinBox FClear">
            <p class="sbTtl MB15">フォルダ名：<span class="pass" id="spanFolderName"><%=ViewData["msrFolderName"]%></span></p>
            <!--
            <dl class="OnCl FCLear">
		        <dt class="sbTtl PL10">《選択条件》</dt>
            </dl>
            <dl class=" OnCL FClear">
           	<dt><input type="checkbox" name="" value="" id="filename1" onclick="fun_onclick(this)" /> <label for="filename1">拡張子</label></dt>
            	<dd>
              		<input type="text" id="includeAttribute1" name="includeAttribute1" size="40" style="width:300px;" value="" />
				</dd>
			</dl>
            <dl class=" OnCL FClear">
           	<dt><input type="checkbox" name="" value="" id="filename2" onclick="fun_onclick(this)" /> <label for="filename2">拡張子</label></dt>
            	<dd>
              		<input type="text" name="includeAttribute2" id="includeAttribute2" size="40" style="width:300px;" value=""/>
				</dd>
			</dl>
            <dl class=" OnCL FClear">
           	  <dt><input type="checkbox" name="" value="" id="filename3" onclick="fun_onclick(this)" /> <label for="filename3">拡張子</label></dt>
            	<dd>
              		<input type="text" name="includeAttribute3" id="includeAttribute3" size="40" style="width:300px;" value="" />
				</dd>
			</dl>
            -->
            <p class="sbTtl OnCL PL10">《除外条件》</p>
            <dl class=" OnCL FClear">
           	  <dt><input type="checkbox" name="exceptAttributeFlg1" value="<%:setFile.exceptAttributeFlg1 %>" id="exceptAttributeFlg1" onclick="fun_onclick(this)" <%if(setFile.exceptAttributeFlg1!=null && setFile.exceptAttributeFlg1 =="1"){ %>checked="true" <%} %>/> <label for="exceptAttributeFlg1">拡張子</label></dt>
            	<dd>
              		<input type="text" name="exceptAttribute1" id="exceptAttribute1" size="40" style="width:300px;" value="<%:setFile.exceptAttribute1 %>" />
                    <span id="exceptAttribute1_error" class ="spanPrompt"></span>
				</dd>
			</dl>
            <dl class=" OnCL FClear">
       	    <dt><input type="checkbox" name="exceptAttributeFlg2" value="<%:setFile.exceptAttributeFlg2 %>" id="exceptAttributeFlg2" onclick="fun_onclick(this)" <%if(setFile.exceptAttributeFlg2!=null && setFile.exceptAttributeFlg2 =="1"){ %>checked="true" <%} %>/> <label for="exceptAttributeFlg2">拡張子</label></dt>
            	<dd>
              		<input type="text" name="exceptAttribute2" id="exceptAttribute2" size="40" style="width:300px;" value="<%:setFile.exceptAttribute2 %>" />
                    <span id="exceptAttribute2_error" class ="spanPrompt"></span>
				</dd>
			</dl>
            <dl class=" OnCL FClear">
            	<dt><input type="checkbox" name="exceptAttributeFlg3" value="<%:setFile.exceptAttributeFlg3 %>" id="exceptAttributeFlg3" onclick="fun_onclick(this)" <%if(setFile.exceptAttributeFlg3!=null && setFile.exceptAttributeFlg3 =="1"){ %>checked="true" <%} %>/> <label for="exceptAttributeFlg3">拡張子</label></dt>
            	<dd class="PR10">
              		<input type="text" name="exceptAttribute3" id="exceptAttribute3" size="40" style="width:300px;" value="<%:setFile.exceptAttribute3 %>" />
                    <span id="exceptAttribute3_error" class ="spanPrompt"></span>
				</dd>
			</dl>
            <ul class="fileCheckBox OnCL FClear" style="display: none;">
            	<li><input type="checkbox" id="systemFileFlg" name="systemFileFlg" value="0"  onclick="fun_setSystemORHiddenFile(this)" /> <label for="systemfile">システムファイル</label></li>
                <li><input type="checkbox" id="hiddenFileFlg" name="hiddenFileFlg" value="0"  onclick="fun_setSystemORHiddenFile(this)" /> <label for="hiddenfile">隠しファイル</label></li>
            </ul>
            <ul class="twoBtns FClear" style="margin-top: 17px;">
       		    <li><a href="javascript:formSubmit();"><img src="../../Content/Image/saveBtn.gif" class="img_on" alt=""/></a></li>
                <li><a href="javascript:funCancel();"><img src="../../Content/Image/cancelBtn.gif" class="img_on" alt=""/></a></li>
            </ul>
        </div>
        <input type="hidden" id="monitorServerID" name="monitorServerID" value='<%=ViewData["msID"] %>'/>
        <input type="hidden" id="monitorServerFolderName" name="monitorServerFolderName" value='<%=ViewData["msrFolderName"]%>'/>
        <input type="hidden" id="id" name ="id" value="<%:setFile.id %>"/>
    </form>
</div>

</asp:Content>
