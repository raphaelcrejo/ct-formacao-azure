using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using postcard.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Queues.Models;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Hosting;
using System.Reflection;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace postcard.Controllers
{
    public class CardsController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public CardsController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index(int _iduf, string _estado)
        {
            var model = new Estado();
            var modelcards = new List<PostCards>();

            var hostname = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in hostname.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    model.hostip = ip.ToString();
                }
            }

            modelcards = await new SQLUtility().getcards("select postcards.id, postcards.id_uf, capas.uf, postcards.cidade, postcards.card, postcards.descricao from postcards INNER JOIN capas on capas.id = postcards.id_uf WHERE capas.Id = " + _iduf.ToString(), _configuration);

            model.estado = _estado;
            model.listpostcards = modelcards;

            return View(model);
        }

        public async Task<ActionResult> DownloadFile(string cardname, string filename, string URLimg, string hostip)
        {
            Stream stream = await new StorageUtility().getstreamtodownload(_configuration, cardname);

            string jsonqueueMessage = @"{""host"" : """ + hostip + @""",""arquivo"" : """ + filename + @""",""url"" : """ + URLimg +@"""}";

            SendMessageDownload(jsonqueueMessage);
            return File(stream, "application /jpeg", filename);
        }

        public ActionResult SendMessageDownload(string queueMessage)
        {
            new StorageUtility().SendMessageToQueue(_configuration, queueMessage.ToString());
            return Ok();
        }
    }
}

