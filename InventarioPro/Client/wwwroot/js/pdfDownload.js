function descargarPDF(pdfBase64) {
    const link = document.createElement('a');
    link.href = pdfBase64;
    link.download = 'inventario_productos.pdf';
    link.click();
}