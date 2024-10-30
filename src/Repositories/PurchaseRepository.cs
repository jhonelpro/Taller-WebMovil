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
                throw new ArgumentNullException("Purchase cannot be null.");
            }

            var shoppingCart = await _context.ShoppingCarts
                .Include(s => s.shoppingCartItems)
                .FirstOrDefaultAsync(s => s.UserId == user.Id);
            
            if (shoppingCart == null)
            {
                throw new ArgumentNullException("Shopping Cart not found.");
            }

            purchase.UserId = user.Id;
            purchase.Transaction_Date = DateTime.Now;

            await _context.Purchases.AddAsync(purchase);
            await _context.SaveChangesAsync();

            return purchase;
        }

        public async Task<Purchase> getPurchase(int id)
        {
            var purchase = await _context.Purchases.FindAsync(id);

            if (purchase == null)
            {
                throw new ArgumentNullException("Purchase not found.");
            }

            return purchase;
        }

        public async Task<byte[]> getPurchaseRecipt(int purchaseId)
        {
            if (purchaseId <= 0)
            {
                throw new ArgumentNullException("Purchase ID cannot be null.");
            }

            var purchase = await _context.Purchases
                .Include(p => p.SaleItems)
                .FirstOrDefaultAsync(p => p.Id == purchaseId);

            if (purchase == null)
            {
                throw new ArgumentNullException("Purchase not found.");
            }

            using (MemoryStream ms = new MemoryStream())
            {
                PdfDocument document = new PdfDocument();
                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);

                XFont titleFont = new XFont("Arial", 16, XFontStyle.Bold);
                XFont regularFont = new XFont("Arial", 12, XFontStyle.Regular);

                gfx.DrawString("Boleta de compra", titleFont, XBrushes.Black, 20, 40);

                gfx.DrawString($"ID de compra: {purchase.Id}", regularFont, XBrushes.Black, 20, 80);
                gfx.DrawString($"Fecha de transacción: {purchase.Transaction_Date}", regularFont, XBrushes.Black, 20, 100);
                gfx.DrawString($"País: {purchase.Country}", regularFont, XBrushes.Black, 20, 120);
                gfx.DrawString($"Ciudad: {purchase.City}", regularFont, XBrushes.Black, 20, 140);
                gfx.DrawString($"Comuna: {purchase.Commune}", regularFont, XBrushes.Black, 20, 160);
                gfx.DrawString($"Calle: {purchase.Street}", regularFont, XBrushes.Black, 20, 180);

                gfx.DrawString("\n", regularFont, XBrushes.Black, 20, 200);

                gfx.DrawString("Detalle de Productos:", titleFont, XBrushes.Black, 20, 220);
                gfx.DrawString("Producto", regularFont, XBrushes.Black, 20, 250);
                gfx.DrawString("Tipo", regularFont, XBrushes.Black, 200, 250);
                gfx.DrawString("Cantidad", regularFont, XBrushes.Black, 300, 250);
                gfx.DrawString("Precio Unitario", regularFont, XBrushes.Black, 400, 250);
                gfx.DrawString("Total", regularFont, XBrushes.Black, 500, 250);

                int yPosition = 270;

                double maxWidth = 100;
                int maxChars = 20; 

                void DrawStringWrapped(string text, XGraphics gfx, XFont font, XBrush brush, double x, double y, double maxWidth)
                {
                    var words = text.Split(' ');
                    string currentLine = "";
                    double currentY = y;

                    foreach (var word in words)
                    {
                        string testLine = currentLine.Length > 0 ? $"{currentLine} {word}" : word;
                        var size = gfx.MeasureString(testLine, font);

                        if (size.Width > maxWidth)
                        {
                            gfx.DrawString(currentLine, font, brush, x, currentY);
                            currentLine = word;
                            currentY += font.Height + 2; 
                        }
                        else
                        {
                            currentLine = testLine;
                        }
                    }

                    if (!string.IsNullOrEmpty(currentLine))
                    {
                        gfx.DrawString(currentLine, font, brush, x, currentY);
                    }
                }

                foreach (var item in purchase.SaleItems)
                {
                    var product = await _context.Products.Include(p => p.ProductType).FirstOrDefaultAsync(p => p.Id == item.ProductId);

                    if (product == null)
                    {
                        throw new ArgumentNullException("product not found.");
                    }

                    string productName = product.Name.Length > maxChars ? product.Name.Substring(0, maxChars) + "..." : product.Name;

                    DrawStringWrapped(productName, gfx, regularFont, XBrushes.Black, 20, yPosition, maxWidth);
                    gfx.DrawString(product.ProductType.Name, regularFont, XBrushes.Black, 200, yPosition);
                    gfx.DrawString(item.Quantity.ToString(), regularFont, XBrushes.Black, 300, yPosition);
                    gfx.DrawString(item.Product.Price.ToString("C"), regularFont, XBrushes.Black, 400, yPosition);
                    gfx.DrawString((item.Quantity * product.Price).ToString("C"), regularFont, XBrushes.Black, 500, yPosition);

                    yPosition += 20; 
                }

                var totalPrice = purchase.SaleItems.Sum(p => p.TotalPrice);
                gfx.DrawString($"Precio Total: {totalPrice:C}", titleFont, XBrushes.Black, 20, yPosition + 20);

                document.Save(ms, false);
                return ms.ToArray();
            }
        }
    }
}