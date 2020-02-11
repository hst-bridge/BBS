<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="budbackup.Models" %>
<% 
    if (ViewData["StartPath"] != null)
    {
        TreeViewFolderHelper.strStartPath = ViewData["StartPath"].ToString();
    }
    else
    {
        TreeViewFolderHelper.strStartPath = string.Empty;
    }
    if (ViewData["initFolderDetail"] != null)
    {
        //TreeViewVirtualModeHelper.initFolderList = (List<budbackup.Models.MonitFolder>)ViewData["initFolderDetail"];
    }
    TreeViewFolderHelper.bInit = true;
%>
<% 
    Html.DevExpress().TreeView(
        settings =>
        {
            settings.Name = "tvFolderMode";
            settings.CallbackRouteValues = new { Controller = "ObjectSpy", Action = "FolderSelectPartial" };
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

            settings.ClientSideEvents.NodeClick = "function(s, e) { DXEventMonitorFolder.Trace(s, e, 'NodeClick'); }";
            settings.ClientSideEvents.CheckedChanged = "function(s, e) { DXEventMonitorFolder.Trace(s, e, 'CheckedChanged'); }";
            settings.ClientSideEvents.ExpandedChanged = "function(s, e) { DXEventMonitorFolder.Trace(s, e, 'ExpandedChanged'); }";
        })
        .BindToVirtualData(TreeViewFolderHelper.CreateChildren)
        .Render();
%>
