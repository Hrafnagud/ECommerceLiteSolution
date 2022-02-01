using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECommerceLiteUI.Controllers
{
    public class BaseController : Controller
    {
        //Cleaned Index method that comes with BaseController.
        [NonAction]
        public string GenerateNewRandomPassword()
        {
            //abcd1234
            Random rnd = new Random();
            int number = rnd.Next(1000, 5000);
            char[] line = Guid.NewGuid().ToString().Replace("-", "").ToArray();
            string password = string.Empty;
            for (int i = 0; i < line.Length; i++)
            {
                if (password.Length == 4) break;
                if (char.IsLetter(line[i]))
                    password += line[i].ToString();
            }
            password += number;
            return password;
        }

        [NonAction]
        public static byte[] BitmapToByteArray(Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }
    }
}