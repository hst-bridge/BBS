DXEventMonitorFolder = {
    TimerId: -1,
    PendingUpdates: [],

    Trace: function (sender, e, eventName) {
        var self = DXEventMonitorFolder;
        var currentNodeId = e["node"].contentElementID;
        var nodeCheckState = e["node"].GetChecked();
        var parentNodeId = "";
        var parentFolderPath = "";
        if (e["node"].parent != null) {
            parentNodeId = e["node"].parent.contentElementID;
            parentFolderPath = e["node"].parent.name;
        }
        if (eventName == 'NodeClick') {
            //单击事件
            var chkClass = $('#' + currentNodeId).children().attr('class');
            var chkState = false;
            if (chkClass.indexOf('dxWeb_edtCheckBoxChecked') >= 0) {
                chkState = true;
            }
            if (currentNodeId != $('#currentNodeId').val()) {
                funGetDetail(parentNodeId, currentNodeId, chkState, e["node"].name, 0, parentFolderPath);
            }

        }
        if (eventName == 'CheckedChanged') {
            var chkClass = $('#' + currentNodeId).children().attr('class');
            var chkState = false;
            if (chkClass.indexOf('dxWeb_edtCheckBoxChecked') >= 0) {
                chkState = true;
            }
            funCheckChanged(parentNodeId, currentNodeId, chkState, e["node"].name, e["node"].text, parentFolderPath);
        }
        if (eventName == 'ExpandedChanged') {//展開の時
            var currentObj = $('#' + e["node"].contentElementID);
            //ファイル子ノードを隠します
            currentObj.parent().children('ul').children('li').children('span.dxtv-elbNoLn:empty').parent().hide();
            if (e["node"].GetExpanded()) {
                funShowLoading();
                for (var i = 0; i < e["node"].nodes.length; i++) {
                    e["node"].nodes[i].SetExpanded(false);
                    if (!e["node"].nodes[i].GetChecked()) {
                        //未チェックの場合
                        var checkFlg = funIsChecked(e["node"].nodes[i].text, e["node"].name);
                        var chkObj = '#' + e["node"].nodes[i].contentElementID + '_D';
                        if (checkFlg == 1) {
                            e["node"].nodes[i].SetCheckState("Checked");
                            //$(chkObj).attr('class', spanCheckedClass);
                        }
                        else if (checkFlg == 2) {
                            //$(chkObj).attr('class', spanIndeterminateClass);
                            e["node"].nodes[i].SetCheckState("Indeterminate");
                        }
                        else {
                            e["node"].nodes[i].SetCheckState("Unchecked");
                        }
                    }
                }
                funHideLoading();
            }
        }
    },

    GetDetailElement: function () {
        return document.getElementById("folderDetail");
    },
    Update: function () {
        self.TimerId = -1;
        self.PendingUpdates = [];
    },
    showDetail: function (result) {
        var self = DXEventMonitorFolder;
        var element = self.GetDetailElement();
        element.innerHTML = result;
    }
};
