using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace asm_final_1.Utils
{
    public static class FileExtension
    {
        public static string UploadFile(IFormFile ImageFile, IWebHostEnvironment webHostEnvironment)
        {
            string uniqueFileName = null;

            if (ImageFile != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images/products");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
    }
}
