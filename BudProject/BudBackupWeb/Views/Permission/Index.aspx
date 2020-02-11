<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="budbackup.CommonWeb" %>
<asp:Content ID="PermissionTitle" ContentPlaceHolderID="TitleContent" runat="server">
	BUD Backup System
</asp:Content>

<asp:Content ID="PermissionContent" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript">
    var rootURL = "<%=VirtualPath.getVirtualPath()%>";
    function funDelete(id) {
        if (!confirm('<%=CommonUtil.getMessage("Q004","権限設定") %>')) {
            //削除操作をキャンセルする時
            return;
        }
        var url = rootURL + "Permission/Delete";
        var data = {};
        data.id = id;
        $.ajax({
            type: "POST",
            url: url,
            cache: false,
            data: data,
            error: function (errMsg) { alert(errMsg); },
            success: function (result) {
                if (result == 99) {//未登録
                    alert('<%=CommonUtil.getMessage("W001","権限設定管理") %>');
                    window.location.href = rootURL + "Account/LogOn";
                }
                else if (result == -10) {//エラーがある
                    alert('<%=CommonUtil.getMessage("E001") %>');
                }
                else if (result == 0) {//成功
                    alert('<%=CommonUtil.getMessage("I001","削除")%>');
                    window.location.href = rootURL + "Permission/Index";
                }
                else {//失敗
                    alert('<%=CommonUtil.getMessage("I002","削除")%>');
                }
            }
        });
    }
</script>
    <div>
    <% List<Model.UserInfo> userList = (List<Model.UserInfo>)ViewData["userList"]; %>
         <div class="osMain">
            <div class="osTitle">
                <ul>
                    <li><%: Html.ActionLink("転送元設定", "Index", "ObjectSpy")%></li>
                    <li><%: Html.ActionLink("転送ファルダ設定", "Index", "FileSpy")%></li>
                    <li><%: Html.ActionLink("転送先設定", "Index", "Transfer")%></li>
                    <li><%: Html.ActionLink("転送先グループ設定", "Index", "GroupTransfer")%></li>
                    <li><%: Html.ActionLink("ログ表示", "Index", "Log")%></li>
                    <li class="on"><%: Html.ActionLink("権限設定管理", "Index", "Permission")%></li>
                    <li class="right"><a href="<%=VirtualPath.getVirtualPath()%>Account/Logon">ログアウト</a></li>
                </ul>
            </div>
            <div class="clear"></div>
            <div class="osContent">
                <div class="contentInner">
                    <h4>【転送元設定】</h4>
                    <table cellpadding="0" cellspacing="0">
                        <tbody>
                            <tr>
                                <th>ログインID</th>
                                <th>名称</th>
                                <th>メール</th>
                                <th colspan="2">操作</th>
                            </tr>
                            <% for (int i = 0; i < userList.Count; i++)
                               { %>
                                <tr>
                                    <td class="right"><%:i+1 %></td>
                                    <td><%:userList[i].name %></td>
                                    <td class="center"><%:userList[i].mail %></td>
                                    <td class="center">
                                    <% if (userList[i].deleteFlg == 1)
                                       {%>
                                        <a href="javascript:void(0);">編集</a>
                                        <a href="javascript:void(0);">削除</a>
                                        <%}
                                       else
                                       { %>
                                       <%: Html.ActionLink("編集", "Edit", "Permission", new { userList[i].id }, null)%>
                                       <a href="javascript:funDelete('<%:userList[i].id %>');">削除</a>
                                        <%} %>
                                    </td>
                                </tr>
                               <%} %>
                        </tbody>
                    </table>
                    <div class="left_btn">
                        <%: Html.ActionLink("権限新規作成", "Insert", "Permission")%>
                    </div>
                    <!--
                    <div class="right_desc"><p class="attention">※[削除]ボタンで権限対象削除<br />使用中の場合は[削除]ボタン使用禁止</p></div>
                    -->
                </div>
            </div>
         </div>
    </div>
    
</asp:Content>
