
$(function () {
    $('#txtFromDate').datepicker({
        buttonImage: '/images/downArrow.gif', buttonImageOnly: true, changeMonth: true, changeYear: true,
        showOn: 'both', defaultDate: '0D',
        changeMonth: true, changeYear: true, showOtherMonths: true, selectOtherMonths: true, showButtonPanel: true,
        onSelect: function (dateText, inst) { $('#txtFromDate').val(dateText); $(this).blur(); }
    }).keyup(function (e) { datePicker_keyUp(this, e); })
});

$(function () {
    $('#txtToDate').datepicker({
        buttonImage: '/images/downArrow.gif', buttonImageOnly: true, changeMonth: true, changeYear: true,
        showOn: 'both', defaultDate: '0D',
        changeMonth: true, changeYear: true, showOtherMonths: true, selectOtherMonths: true, showButtonPanel: true,
        onSelect: function (dateText, inst) { $('#txtToDate').val(dateText); $(this).blur(); }
    }).keyup(function (e) { datePicker_keyUp(this, e); });
});

function Invoice(Cusno, Shpno, Invno, Invdate, Ponum, Ordno, Ordate) {
    var URL = "../../DealerProfilesNET/InvoiceDetail.aspx?MenuReadOnly=1&txtshpno=" + Shpno + "&txtinvno=" + Invno + "&txtinvdate=" + Invdate + "&txtordno=" + Ordno + "&txtorddt=" + Ordate
    window.open(URL, 'InvoiceDetail', 'toolbar=no,titlebar=no,menubar=no,directories=no,location=no,status=no,resizable=yes,copyhistory=no,scrollbars=yes,top=50,left=1,width=700,height=500,fullscreen=no');
}

function gridviewScroll() {
    $('#gvInvoices').gridviewScroll({
        width: 1120,
        height: 300,
        headerrowcount: 1,
        arrowsize: 30,
        railsize: 15,
        barsize: 10,
        varrowtopimg: "/Images/arrowvt.png",
        varrowbottomimg: "/Images/arrowvb.png",
        harrowleftimg: "/Images/arrowhl.png",
        harrowrightimg: "/Images/arrowhr.png"

    });

    gvStyles();
}

function gvStyles() {

    var Timg = document.getElementById('gvInvoicesVertical_TIMG');
    var Bimg = document.getElementById('gvInvoicesVertical_BIMG');
    var HBar = document.getElementById('gvInvoicesVerticalRail');
    var VBar = document.getElementById('gvInvoicesHorizontalRail');
    var Limg = document.getElementById('gvInvoicesHorizontal_LIMG');
    var Rimg = document.getElementById('gvInvoicesHorizontal_RIMG');

    if (HBar) {
        var BarHeight = parseInt(HBar.clientHeight) + parseInt(30);
        var Himgtop = parseInt(HBar.clientHeight) + parseInt(60);


        if (HBar.style.display != "none") {
            if (Timg) {
                Timg.style.cssText = "height: 30px; position: absolute; z-index: 0; top: 0px; right: 0px;background-color: #F0F0F0";
            }



            if (Bimg) {
                Bimg.style.cssText = "height: 30px; position: absolute; z-index: 0; right: 0px; top:" + parseInt(BarHeight) + "px;background-color: #F0F0F0";


            }
        }
    }
    if (VBar) {
        if (VBar.style.display != "none") {
            if (Limg) {
                Limg.style.cssText = "width: 30px; position: absolute; top: " + parseInt(Rimg.style.top) + "px; z-index: 0; left: 0px;";
            }
        }
    }
}

$(document).ready(function () {
    var totalRows = $('#gvInvoices tr').length;
    if (totalRows > 5)
        gridviewScroll();
});