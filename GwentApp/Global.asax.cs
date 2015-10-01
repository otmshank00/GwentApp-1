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
using GwentApp.Models;
using Newtonsoft.Json;

namespace GwentApp
{
    public class Global : HttpApplication
    {
        // The directory containing configuration items used by the application.
        private const string CONFIG_ITEM_DIRECTORY = @"~\ConfigurationItems\";

        //The directory containing picture assets
        private const string PICTURE_DIRECTORY = @"~\Pictures\";

        // The name of the configuration item that conatins the information required to connect to Active Directory.
        private const string ACTIVE_DIRECTORY_CONFIGURATION_ITEM_NAME = "ActiveDirectory";

        // The name of the configuration item that conatins the information required to connect to the Gwent database.
        private const string DB_CONFIGURATION_ITEM_NAME = "Db";

        // The name of the configuration item that contains the application settings, SQL queries and game constants
        private const string GWENTAPP_CONFIGURATION_ITEM_NAME = "GwentApp";

        // Global objects application objects.
        public static ActiveDirectory ad;
        public static string connectionString;
        public static ConfigurationItem gwentAppConfig;
        public static AppOptions gAppOptions;
        public static string pictureMapPath;

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

            // Setup the application config
            gwentAppConfig = new ConfigurationItem(HostingEnvironment.MapPath(CONFIG_ITEM_DIRECTORY), GWENTAPP_CONFIGURATION_ITEM_NAME, false);

            //Map path to pictures
            pictureMapPath = HostingEnvironment.MapPath(PICTURE_DIRECTORY);

            //Initial configuration check/read in
            //If the file exists, then read it in. If not, then the controller checks will have to do.
            if (System.IO.File.Exists(gwentAppConfig.FilePath))
            {
                //Read the data here
                string gwentAppOptions = gwentAppConfig.Value;
                try
                {
                    gAppOptions = JsonConvert.DeserializeObject<AppOptions>(gwentAppOptions);
                }
                catch
                {

                }
            }

        }
    }
}