using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceBusinessLogicLayer.Settings
{
    public static class Constants
    {
        public static string EmailAddress {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["MAILADDRESS"].ToString();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message + " There is no such key as 'MAIL ADDRESS' in webconfig file");
                }
            }
        }

        public static string EmailPassword
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["MAILPASSWORD"].ToString();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message + " There is no such key as 'MAIL PASSWORD' in webconfig file");
                }
            }
        }
    }
}
