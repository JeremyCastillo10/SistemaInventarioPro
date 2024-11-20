function descargarInventario(pdfBase64) {
    const link = document.createElement('a');
    link.href = pdfBase64;
    link.download = 'reporte_inventario.pdf';
    link.click();
}
function descargarVentas(pdfBase64) {
    const link = document.createElement('a');
    link.href = pdfBase64;
    link.download = 'reporte_ventas.pdf';
    link.click();
}
function descargarFactura(pdfBase64) {
    const link = document.createElement('a');
    link.href = pdfBase64;
    link.download = 'reporte_factura.pdf';
    link.click();
}
function descargarEntradas(pdfBase64) {
    const link = document.createElement('a');
    link.href = pdfBase64;
    link.download = 'reporte_entrada.pdf';
    link.click();
}