using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ZR.CodeExample.SecureMVC.Helpers;
using ZR.CodeExample.SecureMVC.Models;

namespace ZR.CodeExample.SecureMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Define the data you want to secure
            string plainText = "I am Ziggy Rafiq from United Kingdom";

            // Encrypt the data using the EncryptionHelper
            string cipherText = EncryptionHelper.Encrypt(plainText);

            // Decrypt the data to retrieve the original content
            string decryptedText = EncryptionHelper.Decrypt(cipherText);

            // Store the encrypted and decrypted data in ViewData for use in your view
            ViewData["CipherText"] = cipherText;
            ViewData["DecryptedText"] = decryptedText;

            return View();
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