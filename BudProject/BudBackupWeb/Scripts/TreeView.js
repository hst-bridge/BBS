﻿/*
DXDemo = {};

DXDemo.focusSearch = function() {
    searchBox.Focus();
};

DXDemo.CurrentDemoGroupKey = "";
DXDemo.CurrentDemoKey = "";
DXDemo.CurrentThemeCookieKey = "DXCurrentTheme";
DXDemo.CodeLoaded = false;
DXDemo.ShowCodeInNewWindow = function(demoTitle, codeTitle){
    var codeWnd = window.open("", "_blank", "status=0,toolbar=0,scrollbars=1,resizable=1,top=74,left=74,width=" + (screen.availWidth - 150) + ",height=" + (screen.availHeight - 150));
    codeWnd.document.open();
    codeWnd.document.write("<html><head><title>");
    codeWnd.document.write(demoTitle + " - " + codeTitle);
    codeWnd.document.write("</title>");
    var links = document.getElementsByTagName("link");
    for(var i = 0; i < links.length; i++){
        if(links[i].href && links[i].href.indexOf("CodeFormatter.css") > -1){
            codeWnd.document.write("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + links[i].href + "\" />");
            break;
        }
    }
    codeWnd.document.write("</head><body><code>");
    var codeTab = DemoPageControl.GetTabByName(codeTitle);
    codeWnd.document.write(document.getElementById("CodeBlock" + (codeTab.index - 1)).innerHTML);
    codeWnd.document.write("</code></body></html>");
    codeWnd.document.close();
}
DXDemo.SetCurrentTheme = function(theme){
    ThemeSelectorPopup.Hide();
    ASPxClientUtils.SetCookie(DXDemo.CurrentThemeCookieKey, theme);
    if(document.forms[0] && (!document.forms[0].onsubmit || document.forms[0].onsubmit.toString().indexOf("Sys.Mvc.AsyncForm") == -1)) {
        // for export purposes
        var eventTarget = document.getElementById("__EVENTTARGET");
        if(eventTarget)
            eventTarget.value = "";
        var eventArgument = document.getElementById("__EVENTARGUMENT");
        if(eventArgument)
            eventArgument.value = "";

        document.forms[0].submit();
    } else {
        window.location.reload();
    }
}
DXDemo.CodeNavBar_HeaderClick = function(s, e) {
    var source = _aspxGetEventSource(e.htmlEvent);
    var tag = source.tagName;
    e.cancel = tag != "A" && tag != "IMG";
}
DXDemo.ShowScreenshotWindow = function(evt, link){
    DXDemo.ShowScreenshot(link.href); 
    evt.cancelBubble = true; 
    return false;
}
DXDemo.ShowScreenshot = function(src){
    var screenLeft = document.all && !document.opera ? window.screenLeft : window.screenX;
	var screenWidth = screen.availWidth;
	var screenHeight = screen.availHeight;
    var zeroX = Math.floor((screenLeft < 0 ? 0 : screenLeft) / screenWidth) * screenWidth;
    
	var windowWidth = 475;
	var windowHeight = 325;
	var windowX = parseInt((screenWidth - windowWidth) / 2);
	var windowY = parseInt((screenHeight - windowHeight) / 2);
	if(windowX + windowWidth > screenWidth)
		windowX = 0;
	if(windowY + windowHeight > screenHeight)
		windowY = 0;

    windowX += zeroX;

	var popupwnd = window.open(src,'_blank',"left=" + windowX + ",top=" + windowY + ",width=" + windowWidth + ",height=" + windowHeight + ", scrollbars=no, resizable=no", true);
	if (popupwnd != null && popupwnd.document != null && popupwnd.document.body != null) {
	    popupwnd.document.body.style.margin = "0px"; 
    }
}
DXDemo.treeViewNodeClick = function(s, e) {
    var node = e.node;
    if(node.GetNavigateUrl() == "javascript:;") {
        node.SetExpanded(!node.GetExpanded());
    }
}
DXDemo.treeViewExpandedChanging = function(s, e) {
    var node = e.node;
    if(!node.parent && s.GetNodeCount() == 1)
        e.cancel = true;
}


DXDemo.Search = {
    searchTimeout: null,
    lastSearch: null,
    selectedItem: null,

    onSearchBoxKeyPress: function(s, e) {
        var text = SearchBox.GetValue();
        if(text != DXDemo.Search.lastSearch) {
            if(DXDemo.Search.searchTimeout != null)
                clearTimeout(DXDemo.Search.searchTimeout);
            DXDemo.Search.searchTimeout = setTimeout(function() {
                DXDemo.Search.doSearch(text);
            }, 500);
        }
        DXDemo.Search.lastSearch = text;
    },
    onSearchBoxKeyDown: function(s, e) {
        var keyCode = ASPxClientUtils.GetKeyCode(e.htmlEvent);
        var prevent = false;
        if(keyCode == 13) {
            ASPxClientUtils.PreventEventAndBubble(e.htmlEvent);
            if(DXDemo.Search.selectedItem)
                document.location = DXDemo.Search.selectedItem.GetNavigateUrl();
        }
        if(keyCode == 40) {
            DXDemo.Search.moveSelectedItem(1);
            prevent = true;
        }
        if(keyCode == 38) {
            DXDemo.Search.moveSelectedItem(-1);
            prevent = true;
        }
        if(prevent)
            ASPxClientUtils.PreventEventAndBubble(e.htmlEvent);
    },
    onSearchBoxGotFocus: function() {
        var text = SearchBox.GetValue();
        if(text && text.length > 0)
            DXDemo.Search.showPopup();
    },

    doSearch: function(text) {
        DXDemo.Search.selectedItem = null;
        if(text && text.length > 2) {
            DXDemo.Search.showPopup();
            var isMvc = window.MVCxClientPopupControl !== undefined && SearchPopup instanceof window.MVCxClientPopupControl;
            SearchPopup.PerformCallback(isMvc ? { text: text} : text);
        }
        else {
            SearchPopup.Hide();
        }
    },
    moveSelectedItem: function(direction) {
        var groupCount = SearchResultsNavBar.GetGroupCount();
        var group, item;
        var newItem = null;
        if(groupCount == 0 || !SearchPopup.IsVisible()) {
            DXDemo.Search.selectedItem = null;
            return;
        }
        if(DXDemo.Search.selectedItem == null) {
            group = SearchResultsNavBar.GetGroup(direction > 0 ? 0 : groupCount - 1);
            item = group.GetItem(direction > 0 ? 0 : group.GetItemCount() - 1);
            newItem = item;
        }
        else {
            group = DXDemo.Search.selectedItem.group;
            if(direction > 0) {
                if(DXDemo.Search.selectedItem.index < group.GetItemCount() - 1)
                    newItem = group.GetItem(DXDemo.Search.selectedItem.index + 1);
                else {
                    group = SearchResultsNavBar.GetGroup(group.index < groupCount - 1 ? group.index + 1 : 0);
                    newItem = group.GetItem(0);
                }
            }
            else {
                if(DXDemo.Search.selectedItem.index > 0)
                    newItem = group.GetItem(DXDemo.Search.selectedItem.index - 1);
                else {
                    group = SearchResultsNavBar.GetGroup(group.index > 0 ? group.index - 1 : groupCount - 1);
                    newItem = group.GetItem(group.GetItemCount() - 1);
                }
            }
        }
        DXDemo.Search.selectNavBarItem(newItem);
    },
    showPopup: function() {
        DXDemo.Search.selectNavBarItem(null);
        SearchPopup.ShowAtElementByID("SearchPanel");
    },
    selectNavBarItem: function(item) {
        if(window.SearchResultsNavBar) {
            SearchResultsNavBar.SetSelectedItem(item);
            DXDemo.Search.selectedItem = item;
            if(item) {
                var scrollContainer = SearchResultsNavBar.GetMainElement().parentNode;
                var itemElement = document.getElementById("sr_" + item.group.index + "_" + item.index);
                var scrollContainerHeight = scrollContainer.offsetHeight;
                var itemElementOffsetTop = itemElement.offsetTop;
                if(itemElementOffsetTop >= scrollContainer.scrollTop + scrollContainerHeight || itemElementOffsetTop < scrollContainer.scrollTop)
                    scrollContainer.scrollTop = itemElementOffsetTop < scrollContainerHeight ? 0 : itemElementOffsetTop;
            }
        }
    }
}
*/
DXEventMonitor = {
    TimerId: -1,
    PendingUpdates: [],

    Trace: function (sender, e, eventName) {
        //        var checkbox = document.getElementById("EventCheck" + eventName);

        //        if (!checkbox.checked) return;

        var self = DXEventMonitor;
        //        var name = sender.name;
        //        var lastSeparator = name.lastIndexOf("_");
        //        if (lastSeparator > -1)
        //            name = name.substr(lastSeparator + 1);

        //        var builder = ["<table>"];
        //        self.BuildTraceRowHtml(builder, "Sender", name, 100);
        //        self.BuildTraceRowHtml(builder, "EventType", eventName.replace(/_/g, " "));
        //        var count = 0;

        //        for (var child in e) {
        //            if (typeof e[child] == "function") continue;
        //            var childValue = e[child];
        //            if (typeof e[child] == "object" && e[child] && e[child].name)
        //                childValue = "[name: '" + e[child].name + "']";
        //            self.BuildTraceRowHtml(builder, count ? "" : "Arguments", child + " = " + childValue);
        //            count++;
        //        }
        //        builder.push("</table><br />");
        var currentNodeId = e["node"].contentElementID;
        var nodeCheckState = e["node"].GetCheckState();
        var parentNodeId = "";
        var parentFolderPath = "";
        if (e["node"].parent != null) {
            parentNodeId = e["node"].parent.contentElementID;
            parentFolderPath = e["node"].parent.name;
        }
        if (eventName == 'NodeClick') {
            //单击事件
            //            var chkClass = $('#' + currentNodeId).children().attr('class');
            //            var chkState = false;
            //            if (chkClass.indexOf('dxWeb_edtCheckBoxChecked') >= 0) {
            //                chkState = true;
            //            }
            if (currentNodeId != $('#currentNodeId').val()) {
                funGetDetail(parentNodeId, currentNodeId, nodeCheckState, e["node"].name, 0, parentFolderPath);
                //funGetDetail(parentNodeId, currentNodeId, chkState, e["node"].name, 0, parentFolderPath);
            }

            //console.info(e["node"]);
            //alert(e["node"].parent.GetCheckState());
            //e["node"].SetChecked(true);
            //alert(result);
            //            alert(e["node"].name);
            //            var fso = new ActiveXObject("Scripting.FileSystemObject");
            //            var f = fso.GetFolder(e["node"].name);
            //            var fc = new Enumerator(f.files);
            //            var s = "";
            //            for (; !fc.atEnd(); fc.moveNext()) {
            //                s += fc.item();
            //                s += "<br/>";
            //            }
            //self.PendingUpdates.unshift(e[child].name + "");
            //self.showDetail(e["node"].name);
        }
        if (eventName == 'CheckedChanged') {
            //选中时
            //console.info(e["node"]);
            //            e["node"].SetFocus() ;
            //console.info(e["node"].GetNode());
            //console.info(e);
            //console.info(parentNodeId);
            //            var chkClass = $('#' + currentNodeId).children().attr('class');
            //            var chkState = false;
            //            if (chkClass.indexOf('dxWeb_edtCheckBoxChecked') >= 0) {
            //                chkState = true;
            //            }
            funCheckChanged(parentNodeId, currentNodeId, nodeCheckState, e["node"].name, e["node"].text, parentFolderPath);
            //funCheckChanged(parentNodeId, currentNodeId, chkState, e["node"].name, e["node"].text, parentFolderPath);
        }
        if (eventName == 'ExpandedChanged') {//展開の時
            var e_node = e["node"];
            // alert("ExpandedChanged");
            //            console.info(e);
            //            console.info(e["node"]);
            //e["node"].treeView.SetNodeState("Unchecked");
            var currentObj = $('#' + e_node.contentElementID);
            //ファイル子ノードを隠します
            currentObj.parent().children('ul').children('li').children('span.dxtv-elbNoLn:empty').parent().hide();
            if (e_node.GetExpanded()) {
                funShowLoading();
                for (var i = 0; i < e_node.nodes.length; i++) {
                    var child_node = e_node.nodes[i];
                    //e["node"].nodes[i].SetExpanded(false);
                    if (!child_node.GetChecked()) {
                        //未チェックの場合
                        var checkFlg = funIsChecked(child_node.text, e_node.name);
                        var chkObj = '#' + child_node.contentElementID + '_D';
                        if (checkFlg == 1) {
                            child_node.SetCheckState("Checked");
                            //$(chkObj).attr('class', spanCheckedClass);
                        }
                        else if (checkFlg == 2) {
                            //$(chkObj).attr('class', spanIndeterminateClass);
                            child_node.SetCheckState("Indeterminate");
                        }
                        else {
                            child_node.SetCheckState("Unchecked");
                        }
                        //console.info("e_node.name: " + e_node.name + ", child_node.text: " + child_node.text + ", checkFlg: " + checkFlg + ", ExpandedChanged");
                    }
                }
                funHideLoading();
                //console.info(checkFlg);
                //e["node"].nodes[i].SetCheckState('<%DevExpress.Web.ASPxClasses.CheckState.Indeterminate%>');
                //e["node"].nodes[i].SetChecked(true);
            }
            funGetDetail(parentNodeId, currentNodeId, nodeCheckState, e_node.name, 0, parentFolderPath);
            funExpandPath();

            funGetBranchFileTypeSet(currentNodeId, e["node"].name);
        }
        //        self.PendingUpdates.unshift(builder.join(""));
        //        if (self.TimerId < 0)
        //            self.TimerId = window.setTimeout(self.Update, 0);
    },

    //    BuildTraceRowHtml: function (builder, name, text, width) {
    //        builder.push("<tr><td valign=top");
    //        if (width)
    //            builder.push(" style='width: ", width, "px'");
    //        builder.push(">");
    //        if (name)
    //            builder.push("<b>", name, ":</b>");
    //        builder.push("</td><td valign=top>", text, "</td></tr>");
    //    },

    //    GetLogElement: function () {
    //        return document.getElementById("EventLog")
    //    },
    GetDetailElement: function () {
        return document.getElementById("folderDetail");
    },
    Update: function () {
        //        var self = DXEventMonitor;
        //        var element = self.GetLogElement();

        //        element.innerHTML = self.PendingUpdates.join("") + element.innerHTML;
        self.TimerId = -1;
        self.PendingUpdates = [];
    },

    //    Clear: function () {
    //        DXEventMonitor.GetLogElement().innerHTML = "";
    //    },
    showDetail: function (result) {
        var self = DXEventMonitor;
        var element = self.GetDetailElement();
        element.innerHTML = result;
    }
};

