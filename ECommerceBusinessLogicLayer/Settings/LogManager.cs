using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceBusinessLogicLayer.Settings
{
    public class LogManager
    {
        public static void LogMessage(string message, string userInfo="", string pageInfo = "")
        {
            string fileName = "ECommerceLite" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);

            using (FileStream stream = new FileStream(filePath, FileMode.Append, FileAccess.Write))
            {
                StreamWriter writer = new StreamWriter(stream);
            }
        }
    }
}
