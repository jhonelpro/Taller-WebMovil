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
    /// <summary>
    /// Repositorio para gestionar las operaciones relacionadas con compras, como la creación, consulta de compras y generación de recibos en PDF.
    /// </summary>
    public class PurchaseRepository : IPurchase
    {
        /// <summary>
        /// Atributo de tipo ApplicationDBContext que representa el contexto de la base de datos.
        /// </summary>
        private readonly ApplicationDBContext _context;

        /// <summary>
        /// Constructor que inicializa el contexto de base de datos.
        /// </summary>
        /// <param name="context">Contexto de la base de datos para la aplicación.</param>
        public PurchaseRepository(ApplicationDBContext context)
        {
            //Inicializa el atributo _context con el objeto de tipo ApplicationDBContext recibido.
            _context = context;
        }

        /// <summary>
        /// Crea una nueva compra y la guarda en la base de datos.
        /// </summary>
        /// <param name="purchase">Objeto Purchase que representa la compra a realizar.</param>
        /// <param name="user">Usuario de tipo AppUser asociado a la compra.</param>
        /// <returns>La compra creada de tipo Purchase.</returns>
        /// <exception cref="ArgumentNullException">Lanzado si la compra o el carrito de compras no existen.</exception>
        public async Task<Purchase> createPurchase(Purchase purchase, AppUser user)
        {
            if (purchase == null)
            {
                throw new ArgumentNullException(nameof(purchase), "La compra no puede ser nula.");
            }

            // Se busca el carrito de compra y sus items, asociados al usuario.
            var shoppingCart = await _context.ShoppingCarts
                .Include(s => s.shoppingCartItems)
                .ThenInclude(item => item.Product)
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (shoppingCart == null)
            {
                throw new InvalidOperationException("Carrito de compras no encontrado.");
            }

            if (shoppingCart.shoppingCartItems == null || !shoppingCart.shoppingCartItems.Any())
            {
                throw new InvalidOperationException("El carrito de compras está vacío.");
            }

            // Se actualiza los datos de la compra.
            purchase.UserId = user.Id;
            purchase.User = user;
            purchase.Transaction_Date = DateTime.UtcNow; 

            // Se guarda la compra en la base de datos.
            await _context.Purchases.AddAsync(purchase);
            await _context.SaveChangesAsync();

            // Se retorna la compra.
            return purchase;
        }

        /// <summary>
        /// Obtiene una compra específica basada en su ID y el ID del usuario.
        /// </summary>
        /// <param name="purchaseId">ID de la compra a buscar.</param>
        /// <param name="userId">ID del usuario propietario de la compra.</param>
        /// <returns>El objeto Purchase encontrado.</returns>
        /// <exception cref="ArgumentNullException">Lanzado si la compra no se encuentra.</exception>
        public async Task<Purchase> getPurchase(int purchaseId, string userId)
        {
            // Se obtiene la compra incluyendo los items, a traves del id del usuario y el id de la compra.
            var purchase = await _context.Purchases.Where(p => p.UserId == userId)
                .Include(p => p.SaleItems)
                .FirstOrDefaultAsync(p => p.Id == purchaseId);
            
            if (purchase == null)
            {
                throw new ArgumentNullException("Compra no encontrada.");
            }
            
            // Se retorna la compra.
            return purchase;
        }

        /// <summary>
        /// Genera un recibo en PDF para una compra específica y lo retorna como un arreglo de bytes.
        /// </summary>
        /// <param name="purchaseId">ID de la compra para la cual se genera el recibo.</param>
        /// <param name="userId">ID del usuario propietario de la compra.</param>
        /// <returns>Un arreglo de bytes que representa el recibo en PDF.</returns>
        /// <exception cref="ArgumentNullException">Lanzado si el ID de compra o la compra no existen.</exception>
        public async Task<byte[]> getPurchaseRecipt(int purchaseId, string userId)
        {
            // Verifica si el ID de compra es válido.
            if (purchaseId <= 0)
            {
                throw new ArgumentNullException("La ID de compra no es válida.");
            }

            // Obtiene la compra incluyendo sus items asociados mediante el ID de la compra.
            var purchase = await _context.Purchases
                .Include(p => p.SaleItems)
                .FirstOrDefaultAsync(p => p.Id == purchaseId);

            // Verifica si la compra existe en la base de datos.
            if (purchase == null)
            {
                throw new ArgumentNullException("La compra no existe.");
            }

            // Configuración del archivo PDF y escritura de los datos.
            using (MemoryStream ms = new MemoryStream())
            {
                // Crea un nuevo documento PDF y una página.
                PdfDocument document = new PdfDocument();
                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);

                // Configura las fuentes para el título y el texto regular.
                XFont titleFont = new XFont("Arial", 16, XFontStyle.Bold);
                XFont regularFont = new XFont("Arial", 12, XFontStyle.Regular);

                // Escribe el encabezado de la boleta de compra.
                gfx.DrawString("Boleta de compra", titleFont, XBrushes.Black, 20, 40);

                // Escribe detalles de la compra: ID, fecha y dirección.
                gfx.DrawString($"ID de compra: {purchase.Id}", regularFont, XBrushes.Black, 20, 80);
                gfx.DrawString($"Fecha de transacción: {purchase.Transaction_Date}", regularFont, XBrushes.Black, 20, 100);
                gfx.DrawString($"País: {purchase.Country}", regularFont, XBrushes.Black, 20, 120);
                gfx.DrawString($"Ciudad: {purchase.City}", regularFont, XBrushes.Black, 20, 140);
                gfx.DrawString($"Comuna: {purchase.Commune}", regularFont, XBrushes.Black, 20, 160);
                gfx.DrawString($"Calle: {purchase.Street}", regularFont, XBrushes.Black, 20, 180);

                // Espacio en blanco entre secciones.
                gfx.DrawString("\n", regularFont, XBrushes.Black, 20, 200);

                // Encabezados de la sección de detalle de productos.
                gfx.DrawString("Detalle de Productos:", titleFont, XBrushes.Black, 20, 220);
                gfx.DrawString("Producto", regularFont, XBrushes.Black, 20, 250);
                gfx.DrawString("Tipo", regularFont, XBrushes.Black, 200, 250);
                gfx.DrawString("Cantidad", regularFont, XBrushes.Black, 300, 250);
                gfx.DrawString("Precio Unitario", regularFont, XBrushes.Black, 400, 250);
                gfx.DrawString("Total", regularFont, XBrushes.Black, 500, 250);

                int yPosition = 270; // Posición vertical inicial para los items.
                double maxWidth = 100; // Ancho máximo para ajustar texto.
                int maxChars = 20; // Cantidad máxima de caracteres para mostrar.

                // Método auxiliar para ajustar el texto si excede el ancho máximo.
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
                            currentY += font.Height + 2; // Incrementa el Y para la siguiente línea.
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

                // Itera sobre los items de la compra y los imprime en el recibo.
                foreach (var item in purchase.SaleItems)
                {
                    // Obtiene el producto y su tipo para cada item.
                    var product = await _context.Products.Include(p => p.ProductType).FirstOrDefaultAsync(p => p.Id == item.ProductId);

                    // Verifica si el producto existe.
                    if (product == null)
                    {
                        throw new ArgumentNullException("Producto no encontrado.");
                    }

                    // Limita el nombre del producto a maxChars caracteres si es necesario.
                    string productName = product.Name.Length > maxChars ? product.Name.Substring(0, maxChars) + "..." : product.Name;

                    // Imprime el nombre del producto, tipo, cantidad, precio unitario y total.
                    DrawStringWrapped(productName, gfx, regularFont, XBrushes.Black, 20, yPosition, maxWidth);
                    gfx.DrawString(product.ProductType.Name, regularFont, XBrushes.Black, 200, yPosition);
                    gfx.DrawString(item.Quantity.ToString(), regularFont, XBrushes.Black, 300, yPosition);
                    gfx.DrawString(item.Product.Price.ToString("C"), regularFont, XBrushes.Black, 400, yPosition);
                    gfx.DrawString((item.Quantity * product.Price).ToString("C"), regularFont, XBrushes.Black, 500, yPosition);

                    yPosition += 20; // Incrementa la posición vertical para el siguiente item.
                }

                // Calcula el precio total de la compra e imprime el total en el recibo.
                var totalPrice = purchase.SaleItems.Sum(p => p.TotalPrice);
                gfx.DrawString($"Precio Total: {totalPrice:C}", titleFont, XBrushes.Black, 20, yPosition + 20);

                // Guarda el documento en el MemoryStream y retorna el arreglo de bytes.
                document.Save(ms, false);
                return ms.ToArray();
            }
        }
    }
}