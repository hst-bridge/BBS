
function ajax_main(url, data, info) {
    $.ajax({
        type: "POST",                                   
        url: url,                                       
        data: data,                                     
        datatype: "json",                               
        ifModified: false,                                                                
        global: true,
        error: function (json, err, e) { error(json, err, e); }, 
        success: function (json) { success(json); },        
        complete: function (json, suc) { },                
        contentType: "application/x-www-form-urlencoded", 
        processData: true,                               
        async: true,                                     
        beforeSend: function (json) { before(info); } 
    }
	);
}

function before(info) {
}

function error(json, err, e) {
    funHideLoading();
}

function success(json) {
    funHideLoading();
    if (json.length > 0) {
        if (json.substr(0, 8) == "<script>") {
            eval(json.substr(0, json.length - 9).substr(8));
        }
        else {
            var jsontext = json;
            UpdatePage(jsontext);
        }
    } else {
        var tableTR = "";
        var thead = $('.logTableWrapper table tr:first').html();

        $('.logTableWrapper table').html('<tr class="tdn">' + thead + '</tr>');
        $('#PageTab').html("");
    }
}

function getElementsByClassName(n) {
    var el = [],
        _el = document.getElementsByTagName('a');
    for (var i = 0; i < _el.length; i++) {
        if (_el[i].className == n) {
            el[el.length] = _el[i];
        }
    }
    return el;
}

function UpdateData(jsontext) {
    var flag =  $("#hidden_url").val();
    if (flag.indexOf("FileDownload") > -1) {
        searchResultShowNew(jsontext);
    } else {
        showLogNew(jsontext);
    }
}

function UpdatePage(jsontext) {
    jsontext = eval("(" + jsontext + ")"); 
    UpdateData(jsontext.logList);
    
    //total count
    var userCount = jsontext.totalCount;
    //total page
    var pagecount = jsontext.pageCount;
    $("#pcount").val(pagecount);
    //current page
    var pindex = jsontext.pindex;
    if (pagecount > 1) {
        sailstr = "<ul class=\"table_page_ul\">";
        if (pindex > 1) {
            sailstr += "<li class=\"prev\"><a href=\"javascript:void(0)\" onclick=\"GetPage(" + (pindex - 1) + ")\"><< 前のページ</a></li>";
        }

        if (Number(pagecount) >= 5) {
            if (pindex == 1) {
                for (var i = 1; i < 6; i++) {
                    if (i == pindex) {
                        sailstr += "<li class=\"numon\"><a href=\"javascript:void(0)\">" + i + "</a></li>";
                    }
                    else {
                        sailstr += "<li class=\"num\"><a href=\"javascript:void(0)\" onclick=\"GetPage(" + i + ")\">" + i + "</a></li>";
                    }
                }
            } else if ((Number(pindex) + 4) - Number(pagecount) > 0) {
                for (var i = Number(pagecount) - 4; i < Number(pagecount) + 1; i++) {
                    if (i == pindex) {
                        sailstr += "<li class=\"numon\"><a href=\"javascript:void(0)\">" + i + "</a></li>";
                    }
                    else {
                        sailstr += "<li class=\"num\"><a href=\"javascript:void(0)\" onclick=\"GetPage(" + i + ")\">" + i + "</a></li>";
                    }
                }
            } else {
                for (var i = (Number(pindex) - 1); i < (Number(pindex) + 4); i++) {
                    if (i == pindex) {
                        sailstr += "<li class=\"numon\"><a href=\"javascript:void(0)\">" + i + "</a></li>";
                    }
                    else {
                        sailstr += "<li class=\"num\"><a href=\"javascript:void(0)\" onclick=\"GetPage(" + i + ")\">" + i + "</a></li>";
                    }
                }
            }
        } else if(pagecount>1) {
            for (var i = 1; i < Number(pagecount) + 1; i++) {
                if (i == pindex) {
                    sailstr += "<li class=\"numon\"><a href=\"javascript:void(0)\">" + i + "</a></li>";
                }
                else {
                    sailstr += "<li class=\"num\"><a href=\"javascript:void(0)\" onclick=\"GetPage(" + i + ")\">" + i + "</a></li>";
                }
            }
        }

        if (Number(pindex) < Number(pagecount)) {
            sailstr += "<li class=\"next\"><a href=\"javascript:void(0)\" onclick=\"GetPage(" + (Number(pindex) + 1) + ")\">次のページ >></a></li></ul>";
        }

    } else {
        sailstr = "";
    }
    $("#PageTab").html(sailstr);
}

function GetPage(pval) {
    $("#pindex").val(pval);
    var flag =  $("#hidden_url").val();
    if (flag.indexOf("FileDownload") > -1) {
        funShowLoading();
        formSubmit('page');
    } else {
        funShowLoading();
        GetListContent();
    }
}


function PageGoto() {
    var gopage = $("#txtsailpage").val();    
    if (!isNaN(gopage)) {
        if (($("#pcount").val()-gopage)< 0 ) { gopage = $("#pcount").val(); }
        $("#pindex").val(gopage);
        GetListContent();
    }
}

function GetListContent(flag) {
    var pindex = $("#pindex").val();
    var pagesize = $("#pagesize").val();
    var url_control = $("#hidden_url").val();

    //search condition    
    var dtStart = $('#startDate').val();
    var dtEnd = $('#endDate').val();
    var tmStart = $('#startTime').val();
    var tmEnd = $('#endTime').val();
    var name = $('#name').val();
    var displayFlg = 0;
    var transferFlg = 0;
    var stateFlg = 2;
    var logFlg = 0;
    var groupId = $("#groupSelect").val();
    if ($('input:radio[name="rdDisplay"]:checked').val() == 'file') {
        displayFlg = 1;
    }
    if ($('input:radio[name="rdTransfer"]:checked').val() == 'transfering') {
        transferFlg = 1;
    }
    if ($('input:radio[name="rdState"]:checked').val() == 'stateNG') {
        stateFlg = 0;
    }
    if ($('input:radio[name="rdState"]:checked').val() == 'stateOK') {
        stateFlg = 1;
    }
    if ($('input:radio[name="rdLog"]:checked').val() == 'transferLog') {
        logFlg = 1;
    } 
    var data = new Object();
    
    if (flag) {
        if (flag == 1) {
            data.pindex = 1;
        }
    }
    data.pindex = pindex;
    data.pagesize = pagesize;
    data.dtStart = dtStart;
    data.dtEnd = dtEnd;
    data.tmStart = tmStart;
    data.tmEnd = tmEnd;
    data.displayFlg = displayFlg;
    data.transferFlg = transferFlg;
    data.stateFlg = stateFlg;
    data.logFlg = logFlg;
    data.groupId = groupId;
    data.name = name;
    
    var url = "/" + url_control;

    ajax_main(url, data, 'pindex');
}
