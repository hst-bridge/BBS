<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="budbackup.CommonWeb" %>
<asp:Content ID="GroupTransferTitle" ContentPlaceHolderID="TitleContent" runat="server">
	BUD Backup System
</asp:Content>
<asp:Content ID="header" ContentPlaceHolderID="budHeader" runat="server">
<% Html.RenderPartial("LogonHeader"); %>
</asp:Content>


<asp:Content ID="GroupTransferContent" ContentPlaceHolderID="MainContent" runat="server">
<% Html.RenderPartial("TabControlList4"); %>
<script type="text/javascript">
    var initData = new Object();
    $(document).ready(function () {
        initData.backupServerGroupName = $('#backupServerGroupName').val();
        initData.monitorServerID = $('#monitorServerID').val();
        initData.memo = $('#memo').val();
    });
    //保存ファンクション
    function formSubmit() {
        if (!confirm('<%=CommonUtil.getMessage("Q005") %>')) {
            //更新操作をキャンセルする時
            return;
        }
        var url = rootURL + "GroupTransfer/Update";
        funShowLoading();
        $.ajax({
            type: "POST",
            url: url,
            cache: false,
            data: $("#group_form").serialize(),
            error: function (errMsg) { alert(errMsg); funHideLoading(); },
            success: function (result) {
                funHideLoading();
                if (result == -99) {
                    alert('<%=CommonUtil.getMessage("W001","転送先グループ設定") %>');
                    window.location.href = rootURL + "Account/LogOn";
                }
                else if (result == -10) {//エラーがある
                    alert('<%=CommonUtil.getMessage("E001") %>');
                }
                else if (result == 0) {
                    alert('<%=CommonUtil.getMessage("I001","更新")%>');
                    window.location.href = rootURL + "GroupTransfer/Index";
                }
                else {
                    alert('<%=CommonUtil.getMessage("I002","更新")%>');
                }
            }
        });
    }
    //キャンセル
    function funCancel() {
        if (funCheck_groupFromNoChange()) {
            window.location.href = rootURL + "GroupTransfer/Index";
        }
        else {
            if (confirm('<%=CommonUtil.getMessage("Q002")%>')) {
                window.location.href = rootURL + "GroupTransfer/Index";
            }
        }
    }
</script>
<div class="tab1Wrapper">
    <p class="sbTtl">【転送先グループ設定】</p>
    <div class="edit2-2 FClear">
        <% Model.BackupServerGroup model = (Model.BackupServerGroup)ViewData["model"]; %>
        <% List<Model.MonitorServer> msList = (List<Model.MonitorServer>)ViewData["msList"]; %>

         <form id="group_form" name="group_form" action="">
            <dl class="editData2-2 OnCL FClear">
            	<dt class="size1" style=" width:210px"><span style="color:Red; margin-right:5px;">※</span>転送先グループ名称：</dt>
            	<dd>
              		<input type="text" name="backupServerGroupName" id="backupServerGroupName" size="40" style="width:300px;" value="<%:model.backupServerGroupName %>"/>
                    <span id="backupServerGroupName_error" class ="spanPrompt"></span>
				</dd>
			</dl>
            <dl class="editData2-2 OnCL FClear">
            	<dt class="size1" style=" width:210px">メモ：</dt>
            	<dd>
              		<input type="text" name="memo" id="memo" size="40" style="width:300px;" value="<%:model.memo %>"/>
                    <span id="memo_error" class ="spanPrompt"></span>
				</dd>
			</dl>
            <dl class="editData2-2 OnCL FClear">
                <dt class="size1" style=" width:210px">転送元：</dt>
                <dd>
                    <select id="monitorServerID" name="monitorServerID">
                    <% for (int i = 0; i < msList.Count; i++)
                        {%>
                        <option value="<%:msList[i].id %>" <% if(msList[i].id==model.monitorServerID) {%> selected="selected" <%} %>><%:msList[i].monitorServerName %></option>
                        <%} %>
                    </select>
                </dd>
            </dl>
            <ul class="twoBtns FClear">
       	        <li><a href="javascript:formSubmit();"><img src="../../Content/Image/saveBtn.gif" class="img_on" alt=""/></a></li>
                <li><a href="javascript:funCancel();"><img src="../../Content/Image/cancelBtn.gif" class="img_on" alt=""/></a></li>
            </ul>
             <input type="hidden" id="id" name="id" value="<%:model.id %>" />
        </form>
    </div>
</div>
</asp:Content>
