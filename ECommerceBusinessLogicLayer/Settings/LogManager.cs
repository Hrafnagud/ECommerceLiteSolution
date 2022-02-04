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
                string format = "{0}\t\t{1}\t\t{2}\t\t{3}";
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat(
                    format, DateTime.Now.ToString("dd/MM/yyyy"),
                    pageInfo, userInfo, message
                );
                writer.WriteLine(stringBuilder.ToString());
                writer.Close();
                writer = null;
            }
        }
    }
}
