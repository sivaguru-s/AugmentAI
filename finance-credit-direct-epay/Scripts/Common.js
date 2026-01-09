function ViewEpayPayment(iReferenceNumber, sType) {
    var left = (screen.width / 2) - 200;
    var top = (screen.height / 2) - 225;
    var windowfeatures = "width=550, height=425, top=" + top + ", left=" + left;
    var URL = "ViewPayment.aspx?RefNo=" + iReferenceNumber + "&Type=" + sType;
    window.open(URL, "ViewPayment" + iReferenceNumber, windowfeatures);
}