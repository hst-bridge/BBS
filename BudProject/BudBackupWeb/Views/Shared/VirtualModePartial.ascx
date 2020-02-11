<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@import Namespace="budbackup.Models"%>
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
        //TreeViewVirtualModeHelper.initFolderList = (List<budbackup.Models.MonitFolder>)ViewData["initFolderDetail"];
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
            settings.CallbackRouteValues = new { Controller = "FileSpy", Action = "VirtualModePartial" };
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
            //settings.Theme = "Aqua";

            settings.ClientSideEvents.NodeClick = "function(s, e) { DXEventMonitor.Trace(s, e, 'NodeClick'); }";
            settings.ClientSideEvents.CheckedChanged = "function(s, e) { DXEventMonitor.Trace(s, e, 'CheckedChanged'); }";
            settings.ClientSideEvents.ExpandedChanged = "function(s, e) { DXEventMonitor.Trace(s, e, 'ExpandedChanged'); }";
            //settings.Nodes[1].CheckState
        })
        .BindToVirtualData(TreeViewVirtualModeHelper.CreateChildren)
        .Render();
%>

