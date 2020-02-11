<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master"%>

<asp:Content ID="loginTitle" ContentPlaceHolderID="TitleContent" runat="server">
    BUD Backup System
</asp:Content>

<asp:Content ID="loginContent" ContentPlaceHolderID="budHeader" runat="server">
<script type="text/javascript">
    $(document).ready(function () {
        $('#LoginID').focus();
    });
        function formSubmit() {
            if (submitCheck() == true) {
                var url = $("#Vpath").val() + "Account/Login";
                $.ajax({
                    type: "POST",
                    url: url,
                    cache: false,
                    data: $("#login_form").serialize(),
                    success: function (str) {
                        if (str == "") {
                            $("#login_error").html('<%=budbackup.CommonWeb.CommonUtil.getMessage("I003","ログインIDまたはパスワード")%>');
                        }
                        else {
                            window.location.href = rootURL + "ObjectSpy/Index";
                        }
                    }
                });
            }
        }
        function submitCheck() {
            var flag = true;
            $("#login_error").html("");
            $("#LoginID_error").html("");
            $("#password_error").html("");
            var loginId = $("#LoginID").val();
            var password = $("#password").val();
            if (loginId.trim() == "") {
                $("#LoginID_error").html('<%=budbackup.CommonWeb.CommonUtil.getMessage("W007","ログインID")%>');
                flag = false;
            } else if (password.trim() == "") {
                $("#password_error").html('<%=budbackup.CommonWeb.CommonUtil.getMessage("W007","パスワード")%>');
                flag = false;
            }
            return flag;
        }
        function setInputFocus(obj) {
            if ($("#" + obj).val().trim() == "") {
                $("#" + obj).focus();
            } else {
                formSubmit();
            }
        }
    </script>
    <!--ログインここから-->
    <div class="login">
        <div id="loginmain">
            <p class="MB15"><img src="../../Content/Image/login_bbs.png" alt=" BBS BUD Backup System" /></p>
            <form name="login_form" id="login_form" action="<%=budbackup.CommonWeb.VirtualPath.getVirtualPath()%>Account/Login" method="post">
                <input type="hidden" name="Vpath" id="Vpath" value="<%=budbackup.CommonWeb.VirtualPath.getVirtualPath()%>" />
        <%--<div>
            <div class="login_title">ログオン</div>
            <fieldset>
                <div class="div_center">
                    <div class="editor-field"> 
                        <span style="color:Red;" id="login_error"></span>
                    </div>
                    <div class="editor-field"> 
                        <span class="field_label">ログインID:</span>
                        <input type="text" name="LoginID" id="LoginID" value="" onkeydown="if(event.keyCode==13) setInputFocus('password');" />
                        <span style="color:Red;" id="LoginID_error"></span>
                    </div>

                    <div class="editor-field">
                        <span class="field_label">パスワード:</span>
                        <input type="password" name="password" id="password" value="" onkeydown="if(event.keyCode==13) formSubmit();" />
                        <span style="color:Red;" id="password_error"></span>
                    </div>
                
                    <div class="input_button">
                        <input type="button" value="ログオン" onclick="formSubmit()" />
                    </div>
                </div>
            </fieldset>
        </div>--%>
                <span style="color:Red;" id="login_error"></span>
                <ul>
                <li class="logintxt"><input tabindex="1" type="text" class="sk2" value="" id="LoginID" name="LoginID" onkeydown="if(event.keyCode==13) setInputFocus('password');"/>
                <span style="color:Red;" id="LoginID_error"></span>
                </li>
                <li class="logintxt"><input tabindex="2" type="password" class="sk2" value="" id="password" name="password" onkeydown="if(event.keyCode==13) formSubmit();"/>
                <span style="color:Red;" id="password_error"></span>
                </li>
                </ul>
            </form>
            <div id="loginbtn"  style=""><a href="javascript:formSubmit()" tabindex="3"><img  src="../../Content/Image/loginBtn.gif" alt="login" class="img_on" /></a></div>
        </div>
    </div>
    <!--ログインここまで-->
</asp:Content>
