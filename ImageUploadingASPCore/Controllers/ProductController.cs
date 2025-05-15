using System.Security.Policy;
using ImageUploadingASPCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using NuGet.Packaging;

namespace ImageUploadingASPCore.Controllers
{

    public class ProductController : Controller
    {
        ImageDbContext context;
        IWebHostEnvironment env;
        private HttpClient client = new HttpClient();

        public ProductController(ImageDbContext imageDbContext, IWebHostEnvironment env)
        {
            this.context = imageDbContext;
            this.env = env;
        }

        public IActionResult Index()
        {
            return View(context.Products.ToList());
        }

        public IActionResult AddProduct()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddProduct(ProductVewModel prod)
        {
            string fileName = "";
            if (prod.Photo != null)
            {
                var ext = Path.GetExtension(prod.Photo.FileName);
                var size = prod.Photo.Length;
                if (ext.Equals(".png") || ext.Equals(".jpg") || ext.Equals(".jpeg"))
                {
                    if (size <= 1000000) // 1000000 Bytes = 1Mb
                    {

                    
                    string folder = Path.Combine(env.WebRootPath, "images");
                    fileName = Guid.NewGuid().ToString() + "_" + prod.Photo.FileName;
                    string filePath = Path.Combine(folder, fileName);
                    prod.Photo.CopyTo(new FileStream(filePath, FileMode.Create));

                    Product p = new Product()
                    {
                        Name = prod.Name,
                        Price = prod.Price,
                        ImagePath = fileName
                    };
                    context.Products.Add(p);
                    context.SaveChanges();
                    TempData["Success"] = "Product Added.. ";
                    return RedirectToAction("Index"); // kyuki all product mai retreive karooga in Index page pe
                    } else
                    {
                        TempData["Size_Error"] = "Image must be less than 1Mb";

                    }
                }
            } else
            {
                TempData["Ext_Error"] = "Only PNG, JGP, JPEG images are allowed.";
            }
                return View();
        }


        public async Task<IActionResult> Details(int? id) // int ki Default value 0 hoti hai isliye
                                                          // Default value null set karne keliye int ke aage ? ye mark lagana padta
        {
            if (id == null || context.Products == null) // Validation lagaya gaya hai.
            {
                return NotFound();
            }
            var proData = await context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (proData == null)  // Validation lagaya gaya hai
            {
                return NotFound();
            }
            return View(proData);
        }
    }
}
