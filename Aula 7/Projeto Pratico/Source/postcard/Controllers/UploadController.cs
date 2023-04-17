using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using postcard.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace postcard.Controllers
{
    public class UploadController : Controller
    {
        private readonly IConfiguration _configuration;

        public UploadController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            var model = new Home();

            var hostname = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in hostname.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    model.hostip = ip.ToString();
                }
            }

            return View(model);
        }

        public async Task<ActionResult> UploadFile(List<IFormFile> files)
        {
            try
            {
                long size = files.Sum(f => f.Length);

                var filePaths = new List<string>();
                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        var filePath = Path.GetTempFileName();
                        var filename = formFile.FileName;
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                            new StorageUtility().uploadfiletoshare(_configuration, filename, stream);
                        }
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

