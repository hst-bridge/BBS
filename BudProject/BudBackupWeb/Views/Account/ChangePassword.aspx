<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<budbackup.Models.ChangePasswordModel>" %>

<asp:Content ID="changePasswordTitle" ContentPlaceHolderID="TitleContent" runat="server">
    パスワードの変更
</asp:Content>

<asp:Content ID="changePasswordContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>パスワードの変更</h2>
    <p>
        パスワードを変更するには、次のフォームを使用します。 
    </p>
    <p>
        新しいパスワードは、<%: ViewData["PasswordLength"] %> 文字以上であることが必要です。
    </p>

    <% using (Html.BeginForm()) { %>
        <%: Html.ValidationSummary(true, "パスワードの変更に失敗しました。エラーを修正し、再試行してください。") %>
        <div>
            <fieldset>
                <legend>アカウント情報</legend>
                
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.OldPassword) %>
                </div>
                <div class="editor-field">
                    <%: Html.PasswordFor(m => m.OldPassword) %>
                    <%: Html.ValidationMessageFor(m => m.OldPassword) %>
                </div>
                
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.NewPassword) %>
                </div>
                <div class="editor-field">
                    <%: Html.PasswordFor(m => m.NewPassword) %>
                    <%: Html.ValidationMessageFor(m => m.NewPassword) %>
                </div>
                
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.ConfirmPassword) %>
                </div>
                <div class="editor-field">
                    <%: Html.PasswordFor(m => m.ConfirmPassword) %>
                    <%: Html.ValidationMessageFor(m => m.ConfirmPassword) %>
                </div>
                
                <p>
                    <input type="submit" value="パスワードの変更" />
                </p>
            </fieldset>
        </div>
    <% } %>
</asp:Content>
