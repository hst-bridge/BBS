<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="budbackup.CommonWeb" %>
<%@ Import Namespace="Model" %>
<asp:Content ID="GroupTransferTitle" ContentPlaceHolderID="TitleContent" runat="server">
	BUD Backup System
</asp:Content>
<asp:Content ID="header" ContentPlaceHolderID="budHeader" runat="server">
<% Html.RenderPartial("LogonHeader"); %>
</asp:Content>

<asp:Content ID="GroupTransferContent" ContentPlaceHolderID="MainContent" runat="server">
<% Html.RenderPartial("TabControlList4"); %>
<script type="text/javascript">
    var AddList = new Array();
    var DeleteList = new Array();
//    var AddList = new Object();
//    var DeleteList = new Object();
    

    
    $(function () {
        //選択した転送先対象データ

    });
////    function funLi_onclick(liId, obj) {
////        //onclick = "funLi_onclick('<:unSelectList[i].id %>',this)"
////        var liObj = $('#li' + liId);
//////        console.info(obj);
//////        console.info();
////        $(obj).toggleClass("chosedLi");
    ////    }
    //対象転送先サーバを選択したとき
    function funLeftSelect_onclick(obj, serverId) {
        $('.leftTableBox dd').attr('class','');
        $(obj).attr('class', 'leftSelectedState');
    }
    //転送先サーバを選択したとき
    function funRightSelect_onclick(obj, serverId) {
        $('.rightTableBox dd').attr('class', '');
        $(obj).attr('class', 'rightSelecedState');
    }
    /*mouseover style funtion*/
    function lion(obj, color) {
        var cColor = "";
        if (color == 1) {
            cColor = "rgb(251, 215, 187)";
        } else if (color == 2) {
            cColor = "rgb(219, 238, 243)";
        }
       // $(obj).css("background-color", cColor);
        $(obj).find("a").css("visibility","visible");
    }
    /*mouseout style function*/
    function liout(obj) {
       // $(obj).css("background-color", "#fff");
        $(obj).find("a").css("visibility", "hidden");
    }
    /*addRemove function, add from r->l, remove from l->r*/

    //グループに入れる
    function funAddServer() {
        var leftDdHtml = '<dd id="@id" onclick="funLeftSelect_onclick(this,\'@serverId\')">@serverName</dd>';
        //var groupObj = $("#groupSelect");
        var addCount = 0;
        $('.rightTableBox dd.rightSelecedState').each(function () {
            addCount = addCount + 1;
            var arrObj = new Object();
            var id = $(this).attr('id');
            var name = $(this).html();
            var addHtml = leftDdHtml.replace('@id', id);
            addHtml = addHtml.replace('@serverId', id);
            addHtml = addHtml.replace('@serverName', name);
            $(this).remove();
            arrObj.backupServerId = id;
            funAddOrDeletArray(arrObj, 1);
            //funAddOrDeletArray(arrObj, 1);
            $('.leftTableBox dl').append(addHtml);
        });
        if (addCount <= 0) {
            alert('<%=CommonUtil.getMessage("W004","入れるサーバー") %>');
        }

    }
    //グループから外す
    function funRemoveServer() {
        //var groupObj = $("#groupSelect");
        var RightDdHtml = '<dd id="@id" onclick="funRightSelect_onclick(this,\'@serverId\')">@serverName</dd>';
        var removeCount = 0;
        $('.leftTableBox dd.leftSelectedState').each(function () {
            removeCount = removeCount + 1;
            var arrObj = new Object();
            var id = $(this).attr('id');
            var name = $(this).html();
            var addHtml = RightDdHtml.replace('@id', id);
            addHtml = addHtml.replace('@serverId', id);
            addHtml = addHtml.replace('@serverName', name);
            $(this).remove();
            //funAddOrDeletArray(arrObj, -1);
            arrObj.backupServerId = id;
            funAddOrDeletArray(arrObj, -1);
            $('.rightTableBox dl').append(addHtml);
        });
        if (removeCount <= 0) {
            alert('<%=CommonUtil.getMessage("W004","外すサーバー") %>');
        }
        
    }
    //保存
    function funSave() {
        var url = rootURL + "GroupTransfer/AddDetail?groupId=" + $("#groupSelect").val();
        var data = {};
        funShowLoading();
        if (AddList.length > 0) {
            $.ajax({
                type: "POST",
                url: url,
                cache: false,
                data: funArrayToObject(AddList),
                error: function (errMsg) { alert(errMsg); funHideLoading(); },
                success: function (result) {
                    if (result == -99) {
                        alert('<%=CommonUtil.getMessage("W001","転送先グループ") %>');
                        window.location.href = rootURL + "Account/LogOn";
                    }
                    else if (result == -10) {
                        alert('<%=CommonUtil.getMessage("E001") %>');
                    }
                    else if (result >= 0) {
                        if (DeleteList.length > 0) {
                            funDelete();
                        }
                        else {
                            alert('<%=CommonUtil.getMessage("I001","保存")%>');
                        }
                    }
                    else {
                        alert('<%=CommonUtil.getMessage("I002","保存")%>');
                    }
                    funHideLoading();
                }
            });
        }
        else if (DeleteList.length > 0) {
        funDelete();
        }
    }
    //削除
    function funDelete() {
        var url = rootURL + "GroupTransfer/DeleteDetail?groupId=" + $("#groupSelect").val();
        if (DeleteList.length > 0) {
            $.ajax({
                type: "POST",
                url: url,
                cache: false,
                data: funArrayToObject(DeleteList),
                error: function (errMsg) { alert(errMsg); funHideLoading(); },
                success: function (result) {
                    if (result == -99) {
                        alert('<%=CommonUtil.getMessage("W001","転送先グループ") %>');
                        window.location.href = rootURL + "Account/LogOn";
                    }
                    else if (result == -10) {
                        alert('<%=CommonUtil.getMessage("E001") %>');
                    }
                    else if (result == 0) {
                        DeleteList = new Array();
                        AddList = new Array();
                        alert('<%=CommonUtil.getMessage("I001","保存")%>');
                        // window.location.href = rootURL + "GroupTransfer/Index";
                    }
                    else {
                        alert('<%=CommonUtil.getMessage("I002","保存")%>');
                    }
                    funHideLoading();
                }
            });
        }
    }
    //転送先グループ選択変更のとき、使うファンクション
    function funOnchange(groupId) {
        window.location.href = rootURL + "GroupTransfer/DetailEdit?id=" + groupId;
    }
    function funArrayToObject(arrObj) {
        var o = new Object();
        for (var i = 0; i < arrObj.length; i++) {
            o[i] = arrObj[i];
        }
        return o;
    }
    //移動データの格納
    function funAddOrDeletArray(arrObj,flg) {
        var addArr = new Array();
        var delArr = new Array();
        var existsFlg = false;
        var i;
        if (flg == 1) {//追加
            for (i = 0; i < DeleteList.length; i++) {
                if (DeleteList[i].backupServerId == arrObj.backupServerId) {
                    existsFlg = true;
                    DeleteList.splice(i, 1);
                    break;
                }
            }
            if (!existsFlg) {
                AddList[AddList.length] = arrObj;
            }
            
        }
        else { //削除
            for (i = 0; i < AddList.length; i++) {
                if (AddList[i].backupServerId == arrObj.backupServerId) {
                    existsFlg = true;
                    AddList.splice(i, 1);
                    break;
                }
            }
            if (!existsFlg) {
                DeleteList[DeleteList.length] = arrObj;
            }
        }
    }
</script>
<div class="tab1Wrapper">
    <div class="edit2-9 FClear">
        <% List<BackupServer> groupDetail = (List<BackupServer>)ViewData["selectList"]; %>
        <% List<BackupServer> unSelectList = (List<BackupServer>)ViewData["unSelectList"]; %>
        <% List<BackupServerGroup> groupList = (List<BackupServerGroup>)ViewData["groupList"]; %>
        <% string groupId = (string)ViewData["groupId"]; %>
        <form action="">
            <dl class="editData2-9 OnCL FClear">
                <dt class="size1" style=" width:190px">転送先グループ名称：</dt>
                <dd>
                    <select class="select1" name="groupSelect" id="groupSelect" onchange ="funOnchange(this.value)" style="width:180px;">
                        <% for (int i = 0; i < groupList.Count; i++)
                       {%>
                            <option  value="<%:groupList[i].id %>" <% if(groupList[i].id==groupId){%> selected="selected"<%} %> >
                            <%:groupList[i].backupServerGroupName %>
                            </option>
                       <%} %>
                    </select>
                </dd>
            </dl>
            <!--対象転送先サーバーここから-->
            <div class="leftTableBox OnCL">
                <dl>
                	<dt>転送先（対象）</dt>
                    <% for (int i = 0; i < groupDetail.Count; i++)
                    {%>
                    <dd id="<%: groupDetail[i].id%>" onclick="funLeftSelect_onclick(this,'<%:groupDetail[i].id %>')"><%:groupDetail[i].backupServerName%>
                    </dd>
                    <%} %>
                </dl>
            <%--<p style="width:99px;margin:0 auto"><a href="javascript:funSave();"><img src="../../Content/Image/saveBtn.gif" class="img_on" alt=""/></a></p>--%>
            <p style="width:99px;float:left;"><a href="javascript:funGotoPage('GroupTransfer/Index');"><img src="../../Content/Image/cancelBtn.gif" class="img_on" alt=""/></a></p>
            <p style="width:99px;float:right;"><a href="javascript:funSave();"><img src="../../Content/Image/saveBtn.gif" class="img_on" alt=""/></a></p>
            </div>
            <!--対象転送先サーバーここまで-->
            <div class="centerBox">
                <ul class="centerBtn FClear">
                    <li><a href="javascript:funAddServer();"><img src="../../Content/Image/addBtn.gif" class="img_on" alt=""/></a></li>
                    <li><a href="javascript:funRemoveServer();"><img src="../../Content/Image/removeBtn.gif" class="img_on" alt=""/></a></li>
                </ul>
            </div>
            <!--転送先サーバーここから-->
            <div class="rightTableBox">
            	<dl>
                    <dt>転送先（全て）</dt>
                    <% for (int i = 0; i < unSelectList.Count; i++)
                    { %>
                        <dd id="<%: unSelectList[i].id%>" onclick="funRightSelect_onclick(this,'<%: unSelectList[i].id%>')"><%:unSelectList[i].backupServerName %>
                        </dd>
                    <%} %>
                </dl>
            <%--<p style="width:179px;margin:0 auto"><a href="javascript:funGotoPage('Transfer/Index');"><img src="../../Content/Image/Btn1.gif" class="img_on" alt=""/></a></p>--%>
            </div>
            <!--転送先サーバーここから-->

        </form>
    </div>
</div>
</asp:Content>
