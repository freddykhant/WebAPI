using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebAPI.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration; 

namespace WebAPI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string apiBaseUrl;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration) 
        {
            _logger = logger;
            apiBaseUrl = configuration["ApiBaseUrl"]; 
        }

        public async Task<IActionResult> Index()
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync($"{apiBaseUrl}/api/Clients/getAll");
                if (response.IsSuccessStatusCode)
                {
                    var clientsJson = await response.Content.ReadAsStringAsync();
                    var clients = JsonConvert.DeserializeObject<List<Client>>(clientsJson);
                    return View(clients);
                }
            }
            return View(new List<Client>());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

