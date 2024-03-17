# Overview

In the realm of ASP.NET Core MVC, harnessing the power of C# to fortify our data security through encryption and decryption is a versatile and essential capability. This security enhancement is made possible through the utilization of a range of encryption algorithms, including but not limited to AES (Advanced Encryption Standard), RSA (Rivest-Shamir-Adleman), DES (Data Encryption Standard), and many more. These cryptographic techniques empower developers to safeguard sensitive information, ensuring it remains confidential and tamper-resistant.

Let's delve into a practical illustration of how the AES algorithm, known for its robust encryption capabilities, can be seamlessly integrated into our ASP.NET Core MVC application to both encrypt and decrypt data. This example serves as a foundational understanding of encryption processes within the ASP.NET Core MVC framework, providing a starting point for developers to explore and implement advanced security measures in their web applications.

By the end of this demonstration, we'll have a clearer grasp of how to employ the AES algorithm within our ASP.NET Core MVC projects, bolstering our ability to secure and protect critical data assets. This knowledge empowers us to make informed decisions about which encryption techniques best suit our application's unique security requirements, ensuring the utmost protection for our users' sensitive information.

## Step 1: Implement an Encryption and Decryption Helper Class

Generally, it is recommended to create a dedicated helper class to carry out encryption and decryption operations within our application. Below is a well-structured example of such a class that can be used to enhance the security of our application:

```csharp
using System.Security.Cryptography;

namespace ZR.CodeExample.SecureMVC.Helpers
{
    public static class EncryptionHelper
    {
        private static readonly string EncryptionKey = GenerateRandomKey(256);
        
        public static string Encrypt(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Convert.FromBase64String(EncryptionKey);
                aesAlg.IV = GenerateRandomIV(); // Generate a random IV for each encryption

                aesAlg.Padding = PaddingMode.PKCS7; // Set the padding mode to PKCS7

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(aesAlg.IV.Concat(msEncrypt.ToArray()).ToArray());
                }
            }
        }


        public static string Decrypt(string cipherText)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Convert.FromBase64String(EncryptionKey);
                aesAlg.IV = cipherBytes.Take(16).ToArray();

                aesAlg.Padding = PaddingMode.PKCS7; // Set the padding mode to PKCS7

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherBytes, 16, cipherBytes.Length - 16))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

        private static byte[] GenerateRandomIV()
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.GenerateIV();
                return aesAlg.IV;
            }
        }

        private static string GenerateRandomKey(int keySizeInBits)
        {
            // Convert the key size to bytes
            int keySizeInBytes = keySizeInBits / 8;

            // Create a byte array to hold the random key
            byte[] keyBytes = new byte[keySizeInBytes];

            // Use a cryptographic random number generator to fill the byte array
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(keyBytes);
            }

            // Convert the byte array to a base64-encoded string for storage
            return Convert.ToBase64String(keyBytes);
        }

    }
}

```


As the name implies, this helper class encapsulates the encryption and decryption logic, making it easier to secure sensitive data in our ASP.NET Core MVC application. We generate the encryption key dynamically as part of the best practice. GenerateRandomKey(256)

## Step 2: Utilise Encryption and Decryption in Our Controller or Service
We will be required to call the Encrypt and Decrypt methods within our controller or service class in order to take advantage of the encryption and decryption capabilities we've integrated. Here's an example of how to do this in detail.

```csharp
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

```

During the development of this example, we've assumed that we have created a separate namespace which contains the EncryptionHelper class, which we should replace with a namespace that contains our actual code.

We should never hardcode our encryption key directly into our code, as shown in the EncryptionHelper class. Instead of hardcoding it directly into the code, we should consider more secure methods like storing it in an environment variable or a protected configuration file. As a result of this practice, sensitive information is kept out of our source code, making it less prone to being accessed by unauthorized parties.

## Step 3: Display Encrypted and Decrypted Data in the View
A Razor view can be used to display encrypted and decrypted data that we have stored in the ViewData dictionary in our controller. Here is an example of how we can do this in a Razor view.

```csharp
@{
    ViewData["Title"] = "Security ASP.net Core MVC (C#) Encryption and Decryption";
}

<div class="text-center">
    <h1 class="display-4">@ViewData["Title"]</h1>
    <p>By Ziggy Rafiq, delve into the intricacies of security in ASP.NET Core MVC (C#) through our comprehensive article, focusing on the vital aspects of encryption and decryption techniques. Learn how to safeguard your web applications effectively.</p>
</div>
<div>
    <h2>Encrypted Text:</h2>
    <p>@ViewData["CipherText"]</p>
</div>

<div>
    <h2>Decrypted Text:</h2>
    <p>@ViewData["DecryptedText"]</p>
</div>
<div>
    <h3>Who is Ziggy Rafiq?</h3>
    <p>I am Ziggy Rafiq, a seasoned Technical Lead Developer, recognized as a C# Corner MVP and VIP. With over 19 years of extensive experience, I am passionate about sharing my knowledge as a speaker, mentor, and trainer. Currently, I am proud to be a part of the Capgemini team, contributing my expertise to drive innovation and excellence in the field of software development.</p>
</div>
```

This Razor view code assumes that we have a view associated with our controller action (e.g., Index.cshtml). It displays the encrypted text and the decrypted text on the web page. We can customize the HTML structure and styling to fit our application's design and requirements.

## Summary
In the ever-evolving landscape of ASP.NET Core MVC, where data security is paramount, the art of encryption and decryption becomes an indispensable skill. Throughout this journey into the world of ASP.NET Core MVC, empowered by the versatility of C#, we have explored the means to fortify our data's security.

Encryption, a formidable shield against unauthorized access and tampering, is achieved through the adept utilization of encryption algorithms such as AES, RSA, and DES. These cryptographic techniques equip developers with the tools to safeguard sensitive information, ensuring its confidentiality and integrity.

Our practical exploration centered on the formidable AES algorithm, renowned for its robust encryption capabilities. We seamlessly integrated it into the ASP.NET Core MVC framework, demonstrating how to both encrypt and decrypt data. This exercise served as a foundational understanding of encryption processes within the framework, providing developers with a springboard to implement advanced security measures in their web applications.

As we conclude this demonstration, we now possess a clearer grasp of how to wield the AES algorithm within our ASP.NET Core MVC projects. This newfound knowledge empowers us to make informed decisions about encryption techniques that align with our application's unique security requirements. By doing so, we guarantee the utmost protection for our users' sensitive information.

In wrapping up our journey, we've not only fortified our data's security but also equipped us with the wisdom to navigate the ever-evolving landscape of web application security. As we continue to evolve as a developer, may these insights serve as a steadfast guide in our quest to protect and secure critical data assets.

## About The Author Ziggy Rafiq
- **Technical Lead Developer, C# Corner (MVP 🏅, VIP⭐️, Public Speaker🎤), Mentor, and Trainer**
- **C# Corner MVP, VIP, Speaker, Chapter Lead UK**
- Mentor and Trainer with solid experience in System Architecture for over 19 years
- Link to [**Ziggy Rafiq Blog**](https://blog.ziggyrafiq.com)
- Link to [**Ziggy Rafiq Website**](https://ziggyrafiq.com)
* [**Please remember to subscribe to My YouTube channel**](https://www.youtube.com/)
* [**Please remember to follow me on LinkedIn**](https://www.linkedin.com/in/ziggyrafiq/)
* [**Please remember to connect with me on C# Corner**](https://www.c-sharpcorner.com/members/ziggy-rafiq)
* [**Please remember to follow  me on Twitter/X**](https://twitter.com/ziggyrafiq)
* [**Please remember to follow  me on Instagram**](https://www.instagram.com/ziggyrafiq/)
* [**Please remember to follow  me on Facebook**](https://www.facebook.com/ziggyrafiq)

Ziggy Rafiq is the author and creator of this repository. He is a C# Corner MVP,VIP,Speaker and Chapter Lead Award recipient in 2023, with over 20 years of technical experience using Microsoft technologies and tools. Ziggy has earned various other awards in the past for his contributions to the tech community.

Please note that these examples are simplified for demonstration purposes. In real-world applications, additional security measures and best practices may be required. It is essential to adapt these practices to the specific requirements and security needs of your application.

We hope these code examples serve as a useful reference for maintaining security in your C# applications. Should you have any questions or feedback, please feel free to reach out.

Happy and Secure Coding!

For a deeper dive into related topics and to stay updated with my work, I invite you to explore my websites: [Ziggy Rafiq](https://ziggyrafiq.com) and [Ziggy Rafiq's Blog](https://blog.ziggyrafiq.com). These platforms provide additional insights and valuable information to further enhance our understanding of the subject matter.

The full article can be found at [Security ASP.net Core MVC (C#) Encryption and Decryption](https://www.c-sharpcorner.com/article/the-encryption-and-decryption-of-asp-net-core-mvc-c-sharp/.)

