<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@import Namespace="budbackup.Models"%>

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
<% 
    if (ViewData["StartPath"] != null)
    {
        TreeViewVirtualModeHelper.strStartPath = ViewData["StartPath"].ToString();
    }
    else
    {
        TreeViewVirtualModeHelper.strStartPath = string.Empty;
    }
    if (ViewData["initFolderDetail"] != null)
    {
    }
    if (ViewData["msId"] != null)
    {
        TreeViewVirtualModeHelper.msID = ViewData["msId"].ToString();
    }
    TreeViewVirtualModeHelper.bInit = true;
    %>
<% 
    Html.DevExpress().TreeView(
        settings =>
        {
            settings.Name = "tvVirtualMode";
            settings.CallbackRouteValues = new { Controller = "FileSpy", Action = "PathSelectModePartial" };
            settings.Images.NodeImage.Width = 13;
            settings.Styles.NodeImage.Paddings.PaddingTop = 3;
            settings.AllowCheckNodes = true;
            settings.AllowSelectNode = true;
            settings.CheckNodesRecursive = true;
            settings.EnableAnimation = false;
            settings.EnableHotTrack = false;
            settings.ShowTreeLines = false;
            settings.ShowExpandButtons = true;
            settings.Width = 300;
            settings.ClientSideEvents.NodeClick = "function(s, e) { DXEventMonitor.Trace(s, e, 'NodeClick'); }";
            settings.ClientSideEvents.CheckedChanged = "function(s, e) { DXEventMonitor.Trace(s, e, 'CheckedChanged'); }";
            settings.ClientSideEvents.ExpandedChanged = "function(s, e) { DXEventMonitor.Trace(s, e, 'ExpandedChanged'); }";
        })
        .BindToVirtualData(TreeViewPathSelector.CreateChildren)
        .Render();
%>

