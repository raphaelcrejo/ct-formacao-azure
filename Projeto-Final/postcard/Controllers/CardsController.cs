using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using postcard.Models;

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
        public IActionResult Index(int _iduf, string _estado)
        {
            var model = new Estado();
            var modelcards = new List<PostCards>();

            modelcards = new SQLUtility().getcards("select postcards.id, postcards.id_uf, capas.uf, postcards.cidade, postcards.card, postcards.descricao from postcards INNER JOIN capas on capas.id = postcards.id_uf WHERE capas.Id = " + _iduf.ToString(), _configuration);

            model.estado = _estado;
            model.listpostcards = modelcards;

            return View(model);
        }

        public FileResult DownloadFile(string _path, string _card)
        {
            string contentType = "application/jpeg";

            return File(_path, contentType, _card);
        }
    }
}

