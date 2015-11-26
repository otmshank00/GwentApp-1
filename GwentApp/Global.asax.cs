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
using Galactic.Sql;
using GwentApp.Models;
using Newtonsoft.Json;
using GwentApp.Controllers;

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
        //The application options (AppOptions.cs) class
        public static AppOptions gAppOptions;
        public static string pictureMapPath;
        //List of all cards in the database
        public static List<Card> gAllCards = new List<Card>();
        //List of all factions in the database
        public static List<FactionInfo> gAllFactions = new List<FactionInfo>();
        //List of all leaders in the database
        public static List<LeaderInfo> gAllLeaders = new List<LeaderInfo>();
        //Maybe don't create lists for each faction. Dynamically build the player deck using in mem queries from the gAllCards. Don't work twice

        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            try { 
                // Setup an AD client.
                ad = new ActiveDirectory(HostingEnvironment.MapPath(CONFIG_ITEM_DIRECTORY), ACTIVE_DIRECTORY_CONFIGURATION_ITEM_NAME);
            }
            catch (System.ArgumentNullException ex)
            {
                //If the file doesn't exist, or if it's 0 bytes. Note that as of this writing, a 0-byte file will be created if it does not already exist.
                //System.IO.FileNotFoundException might be a better fit...
                Console.WriteLine("The file {0}{1}, does not exist, please create it in Astrolabe from the template template.{1}, exception is: {2}", CONFIG_ITEM_DIRECTORY, ACTIVE_DIRECTORY_CONFIGURATION_ITEM_NAME, ex.Source);
            }
            catch (System.ArgumentException ex)
            {
                //Hits here if the info in ACTIVE_DIRECTORY_CONFIGURATION_ITEM_NAME was not valid.
                Console.WriteLine("There was a problem with your AD config file {0}{1}, please create it in Astrolabe from the template template.{1}, exception is: {2}", CONFIG_ITEM_DIRECTORY, ACTIVE_DIRECTORY_CONFIGURATION_ITEM_NAME, ex.Source);
            }

            // Get our SQL connection string.
            ConfigurationItem sqlConfig = new ConfigurationItem(HostingEnvironment.MapPath(CONFIG_ITEM_DIRECTORY), DB_CONFIGURATION_ITEM_NAME, true);
            connectionString = sqlConfig.Value;

            // Setup the application config
            gwentAppConfig = new ConfigurationItem(HostingEnvironment.MapPath(CONFIG_ITEM_DIRECTORY), GWENTAPP_CONFIGURATION_ITEM_NAME, false);
            if (System.IO.File.Exists(gwentAppConfig.FilePath))
            {
                if (gwentAppConfig.Value.Length > 1)
                {
                    
                    try
                    {
                        //Read the options
                        gAppOptions = (AdminPageController.ReadAppOptions(gwentAppConfig.Value));
                        //Check for valid data
                        if (gAppOptions.MaxDeckSize < 1)
                        {
                            //If unable to, recreate as defaults
                            bool writesuccess = AdminPageController.WriteOptions(gwentAppConfig.FilePath, new AppOptions(), true);
                            gAppOptions = (AdminPageController.ReadAppOptions(gwentAppConfig.Value));
                        }
                    }
                    catch
                    {
                        
                    }
                }
                else
                {
                    //If unable to, recreate as defaults
                    bool writesuccess = AdminPageController.WriteOptions(gwentAppConfig.FilePath, new AppOptions(), true);
                    gAppOptions = (AdminPageController.ReadAppOptions(gwentAppConfig.Value));
                }
            }
            else
            {
                //If unable to, recreate as defaults
                bool writesuccess = AdminPageController.WriteOptions(gwentAppConfig.FilePath, new AppOptions(), true);
                gAppOptions = (AdminPageController.ReadAppOptions(gwentAppConfig.Value));
            }


            //Map path to pictures
            pictureMapPath = HostingEnvironment.MapPath(PICTURE_DIRECTORY);

            //Fill the lists of cards, leaders and factions here
            DatabaseRead();

        }

        void DatabaseRead()
        {
            // Create a utility to handle the SQL calls for this action.
            MSSqlUtility sqlUtil = new MSSqlUtility();

            //Create an empty row list to be used as the all cards holder
            List<SqlRow> allCardsRows = new List<SqlRow>();

            //Create an empty row list to be used as the all factions holder
            List<SqlRow> allFactionsRows = new List<SqlRow>();

            //Create an empty row list to be used as the all leaders holder
            List<SqlRow> allLeadersRows = new List<SqlRow>();
            try
            {

                allCardsRows = sqlUtil.ExecuteQuery(gAppOptions.SelectAllPlayerCards, connectionString, null);

                //READ LEADERS AND FACTIONS
                allFactionsRows = sqlUtil.ExecuteQuery(gAppOptions.SelectAllFactions, connectionString, null);
                allLeadersRows = sqlUtil.ExecuteQuery(gAppOptions.SelectAllLeaders, connectionString, null);
                //Build the Factions and build the list
                if (allFactionsRows != null)
                {
                    foreach (SqlRow row in allFactionsRows)
                    {
                        //Assign the returned database values into the object
                        FactionInfo faction = new FactionInfo();
                        faction.FactionName = ((string)row["factionName"]).Trim();
                        faction.FactionAbbr = ((string)row["factionAbbreviation"]).Trim();
                        faction.FactionPerk = ((string)row["factionPerk"]).Trim();
                        gAllFactions.Add(faction);
                    }
                }
                //Build the Leaders and build the list
                if (allLeadersRows != null)
                {
                    foreach (SqlRow row in allLeadersRows)
                    {
                        //Assign the returned database values into the object
                        LeaderInfo leader = new LeaderInfo();
                        leader.LeaderName = ((string)row["leaderName"]).Trim();
                        leader.LeaderAbility = ((string)row["leaderAbility"]).Trim();
                        leader.LeaderFaction = ((string)row["leaderFaction"]).Trim();
                        leader.LeaderFactionAbbr = ((string)row["leaderFactionAbbreviation"]);
                        gAllLeaders.Add(leader);
                    }
                }
                //Build the motherlode deck by assigning the attributes from the database into the object attributes.
                //The TRY/CATCH is there to handle potential null values because C# null is not the same as DBNULL 
                if (allCardsRows != null)
                {
                    foreach (SqlRow row in allCardsRows)
                    {
                        Card card = new Card();
                        try
                        {
                            card.Name = ((string)row["cardName"]).Trim();
                        }
                        catch
                        { }
                        try
                        {
                            card.Power = (int)row["cardPower"];
                        }
                        catch { }
                        try
                        {
                            card.Faction = ((string)row["cardFaction"]).Trim();
                        }
                        catch { }
                        try
                        {
                            card.Quote = ((string)row["cardQuote"]).Trim();
                        }
                        catch { }
                        try
                        {
                            card.Hero = (bool)(row["cardHero"]);
                        }
                        catch { }
                        try
                        {
                            card.Range = ((string)row["cardRange"]).Trim();
                        }
                        catch { }
                        try
                        {
                            card.Ability = ((string)row["cardAbilities"]).Trim();
                        }
                        catch { }
                        try
                        {
                            card.Quote = ((string)row["cardQuote"]).Trim();
                        }
                        catch { }
                        //New logic to read image file from DB and build path. ImageFilePath
                        try
                        {
                            string cardImageFileName = (string)row["cardImageFileName"];
                            card.ImageFilePath = "~/Content/Images/" + (cardImageFileName.Trim().Replace(" ", "%20"));

                        }
                        catch { }
                        gAllCards.Add(card);
                    }
                }


            } // end of Try-No Touchy
            catch
            {
                //Do we need to perform an action on catch?
            }
        }
    }
}