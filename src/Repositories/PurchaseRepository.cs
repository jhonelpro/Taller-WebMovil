using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using api.src.Data;
using api.src.Interfaces;
using api.src.Models;
using api.src.Models.User;
using Microsoft.EntityFrameworkCore;
using System.IO; // Para MemoryStream
using PdfSharpCore.Drawing; // Para XGraphics y XFont
using PdfSharpCore.Pdf;
using PdfSharpCore; // Para PdfDocument


namespace api.src.Repositories
{
    public class PurchaseRepository : IPurchase
    {
        private readonly ApplicationDBContext _context;

        public PurchaseRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Purchase> createPurchase(Purchase purchase, AppUser user)
        {
            if (purchase == null)
            {
                throw new ArgumentNullException(nameof(purchase));
            }

            purchase.UserId = user.Id;
            purchase.Transaction_Date = DateTime.Now;

            await _context.Purchases.AddAsync(purchase);
            await _context.SaveChangesAsync();

            return purchase;
        }

        public async Task<Purchase> getPurchase(int id)
        {
            if (id == 0)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var purchase = await _context.Purchases.FindAsync(id);

            if (purchase == null)
            {
                throw new ArgumentNullException(nameof(purchase));
            }

            return purchase;
        }

        public async Task<byte[]> getPurchaseRecipt(int purchaseId)
        {
            if (purchaseId <= 0)
            {
                throw new ArgumentNullException(nameof(purchaseId), "El ID de compra debe ser mayor que cero.");
            }

            var purchase = await _context.Purchases
                .Include(p => p.SaleItems)
                .FirstOrDefaultAsync(p => p.Id == purchaseId);
            
            var saleItems = await _context.SaleItems
                .Include(s => s.Product)
                .Where(s => s.PurchaseId == purchaseId)
                .ToListAsync();
            
            if (purchase == null)
            {
                throw new ArgumentNullException(nameof(purchase), "No se encontró la compra con el ID especificado.");
            }

            using (var ms = new MemoryStream())
            {
                var document = new PdfDocument();
                document.Info.Title = "Boleta de compra";

                var page = document.AddPage();
                page.Size = PageSize.A4;
                page.Orientation = PageOrientation.Portrait;

                XGraphics gfx = XGraphics.FromPdfPage(page);

                XFont titleFont = new XFont("Verdana", 16, XFontStyle.Bold);
                XFont regularFont = new XFont("Verdana", 12, XFontStyle.Regular);

                gfx.DrawString("Boleta de compra", titleFont, XBrushes.Black, 20, 40);

                gfx.DrawString($"ID de compra: {purchase.Id}", regularFont, XBrushes.Black, 20, 80);
                gfx.DrawString($"Fecha de transacción: {purchase.Transaction_Date}", regularFont, XBrushes.Black, 20, 100);
                gfx.DrawString($"País: {purchase.Country}", regularFont, XBrushes.Black, 20, 120);
                gfx.DrawString($"Ciudad: {purchase.City}", regularFont, XBrushes.Black, 20, 140);
                gfx.DrawString($"Comuna: {purchase.Commune}", regularFont, XBrushes.Black, 20, 160);
                gfx.DrawString($"Calle: {purchase.Street}", regularFont, XBrushes.Black, 20, 180);

                gfx.DrawString("\n", regularFont, XBrushes.Black, 20, 200);

                // Agregar detalle de productos
                gfx.DrawString("Detalle de Productos:", titleFont, XBrushes.Black, 20, 220);
                gfx.DrawString("Producto", regularFont, XBrushes.Black, 20, 250);
                gfx.DrawString("Cantidad", regularFont, XBrushes.Black, 200, 250);
                gfx.DrawString("Precio Unitario", regularFont, XBrushes.Black, 300, 250);
                gfx.DrawString("Total", regularFont, XBrushes.Black, 400, 250);

                int yPosition = 270; // Ajusta la posición vertical inicial para los productos

                foreach (var item in saleItems)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);

                    if (product == null)
                    {
                        throw new ArgumentNullException(nameof(product), "No se encontró el producto asociado al item de venta.");
                    }

                    gfx.DrawString(product.Name, regularFont, XBrushes.Black, 20, yPosition);
                    gfx.DrawString(item.Quantity.ToString(), regularFont, XBrushes.Black, 200, yPosition);
                    gfx.DrawString(item.Product.Price.ToString("C"), regularFont, XBrushes.Black, 300, yPosition);
                    gfx.DrawString((item.Quantity * product.Price).ToString("C"), regularFont, XBrushes.Black, 400, yPosition);
                    yPosition += 20; // Espacio entre filas
                }

                // Calcular y agregar precio total
                var totalPrice = purchase.SaleItems.Sum(p => p.TotalPrice);
                gfx.DrawString($"Precio Total: {totalPrice:C}", titleFont, XBrushes.Black, 20, yPosition + 20);

                // Guardar el documento en el MemoryStream
                document.Save(ms, false);
                
                return ms.ToArray();
            }
        }
    }
}