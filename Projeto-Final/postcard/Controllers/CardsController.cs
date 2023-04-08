using System;
using System.Collections.Generic;
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
        public IActionResult Index(int _iduf)
        {
            var model = new List<PostCards>();

            model = new SQLUtility().getcards("select postcards.id, postcards.id_uf, capas.uf, capas.estado, postcards.cidade, postcards.card, postcards.descricao from postcards INNER JOIN capas on capas.id = postcards.id_uf WHERE capas.Id = " + _iduf.ToString(), _configuration);

            return View(model);
        }
    }
}

