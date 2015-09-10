using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Hosting;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;
using Galactic.ActiveDirectory;
using Galactic.Configuration;
using Galactic.Sql.MSSql;

namespace GwentApp
{
    public class Global : HttpApplication
    {
        // The directory containing configuration items used by the application.
        private const string CONFIG_ITEM_DIRECTORY = @"~\ConfigurationItems\";

        // The name of the configuration item that conatins the information required to connect to Active Directory.
        private const string ACTIVE_DIRECTORY_CONFIGURATION_ITEM_NAME = "ActiveDirectory";

        // The name of the configuration item that conatins the information required to connect to the Gwent database.
        private const string DB_CONFIGURATION_ITEM_NAME = "Db";

        // Global objects application objects.
        public static ActiveDirectory ad;
        public static string connectionString;

        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // Setup an AD client.
            ad = new ActiveDirectory(HostingEnvironment.MapPath(CONFIG_ITEM_DIRECTORY), ACTIVE_DIRECTORY_CONFIGURATION_ITEM_NAME);

            // Get our SQL connection string.
            ConfigurationItem sqlConfig = new ConfigurationItem(HostingEnvironment.MapPath(CONFIG_ITEM_DIRECTORY), DB_CONFIGURATION_ITEM_NAME, true);
            connectionString = sqlConfig.Value;
        }
    }
}