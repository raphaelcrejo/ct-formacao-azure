using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using postcard.Models;
using System.Net.Sockets;

namespace postcard.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
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

        //model.hostip = hostname;//Dns.GetHostByName(hostname).AddressList[0].ToString();

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

