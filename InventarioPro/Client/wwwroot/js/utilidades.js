function descargarExcel(excel) {
    var a = document.createElement("a");
    a.href = excel;
    a.download = "ReportExcel.xls";
    a.click();
}
