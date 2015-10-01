using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GwentApp.Models;
using Newtonsoft.Json;
using System.IO;

namespace GwentApp.Controllers
{
    public class AdminPageController : Controller
    {
        // GET: AdminPage
        public ActionResult Index()
        {
            //Create options object
            //AppOptions appOptions = new AppOptions();
            //appOptions.adminOptions = new Dictionary<string, string>();

            //Use Global?
            Global.gAppOptions = new AppOptions();
            Global.gAppOptions.adminOptions = new Dictionary<string, string>();

            //Check for existance of config file
            //Yet another sanity check. Redundant code is redundant but this will be sure that we don't end up with no config.
            //This will also make sure something gets displayed in the admin page, even if it's just defaults.
            if (System.IO.File.Exists(Global.gwentAppConfig.FilePath))
            {
                //Read the data here
                //string gwentAppOptions = Global.gwentAppConfig.Value;
                if (Global.gwentAppConfig.Value.Length > 0)
                {
                    //appOptions = ReadAppOptions(Global.gwentAppConfig.Value);
                    //Global?
                    Global.gAppOptions = ReadAppOptions(Global.gwentAppConfig.Value);
                }
                else
                {
                    //Something went wrong reading in the file. Recreate with defaults
                    bool recreateSuccess = WriteOptions(Global.gwentAppConfig.FilePath, null, true);
                }

            }
            else
            {
                //If not exist, create with defaults
                bool recreateSuccess = WriteOptions(Global.gwentAppConfig.FilePath, null, true);
                if (recreateSuccess)
                {
                    //appOptions = ReadAppOptions(Global.gwentAppConfig.Value);
                    //Global?
                    Global.gAppOptions = ReadAppOptions(Global.gwentAppConfig.Value);
                }
            }
            //Global?
            return View(Global.gAppOptions);
        }

        /// <summary>
        /// This will write the app options to disk. Either a default set or a passed set based upon the last boolean argument
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="options"></param>
        /// <param name="isDefaults"></param>
        /// <returns></returns>
        public bool WriteOptions(string filePath, AppOptions options, bool isDefaults)
        {
            //DB Maxes for sanity check: weather 12, n-units 14, n-heroes 4, f-units 32, f-heroes 4
            
            //Define check var
            bool writeSuccess = false;
            //Define write object
            AppOptions optionsToWrite = new AppOptions();
            optionsToWrite.adminOptions = new Dictionary<string, string>();
            if (isDefaults)
            {
                //Define object then fill it
                AppOptions defaultOptions = new AppOptions();
                //Dictionary<string, string> defaultOptions = new Dictionary<string, string>();
                defaultOptions.adminOptions = new Dictionary<string, string>();
                //Final count of cards dealt to player from initialized deck
                defaultOptions.adminOptions.Add("STARTING_DECK_SIZE", "10");
                //Max size of initialized deck
                defaultOptions.adminOptions.Add("MAX_DECK_SIZE", "28");
                //Max number of weather cards to be dealt
                defaultOptions.adminOptions.Add("MAX_WEATHER_CARDS", "4");
                //Max number of neutral unit cards to be dealt
                defaultOptions.adminOptions.Add("MAX_NEUTRAL_UNITS", "3");
                //Max number of neutral hero cards to be dealt
                defaultOptions.adminOptions.Add("MAX_NEUTRAL_HEROES", "3");
                //Max number of neutral special cards to be dealt
                defaultOptions.adminOptions.Add("MAX_NEUTRAL_SPECIALS", "3");
                //Max number of faction hero cards to be dealt
                defaultOptions.adminOptions.Add("MAX_FACTION_HEROES", "3");
                //Max number of faction unit cards to be dealt
                defaultOptions.adminOptions.Add("MAX_FACTION_UNITS", "12");

                //Define SQL query constants. These work off of existing views in the database.
                //Get all weather cards
                defaultOptions.adminOptions.Add("SELECT_ALL_WEATHER_CARDS", "select * from AllWeather");
                //Get all neutral unit cards
                defaultOptions.adminOptions.Add("SELECT_ALL_NEUTRAL_UNITS", "select * from AllNeutralUnits");
                //Get all neutral hero cards
                defaultOptions.adminOptions.Add("SELECT_ALL_NEUTRAL_HEROES", "select * from AllNeutralHeros");
                //Get all neutral special cards
                defaultOptions.adminOptions.Add("SELECT_ALL_NEUTRAL_SPECIALS", "select * from AllNeutralSpecial");

                //Get all selected faction hero cards.
                defaultOptions.adminOptions.Add("SELECT_ALL_NR_HEROES", "select * from AllNRHeroes");
                defaultOptions.adminOptions.Add("SELECT_ALL_NE_HEROES", "select * from AllNEHeroes");
                defaultOptions.adminOptions.Add("SELECT_ALL_ST_HEROES", "select * from AllSTHeroes");
                defaultOptions.adminOptions.Add("SELECT_ALL_MS_HEROES", "select * from AllMSHeroes");
                //Get all selected faction unit cards.
                defaultOptions.adminOptions.Add("SELECT_ALL_NR_UNITS", "select * from AllNRUnits");
                defaultOptions.adminOptions.Add("SELECT_ALL_NE_UNITS", "select * from AllNEUnits");
                defaultOptions.adminOptions.Add("SELECT_ALL_ST_UNITS", "select * from AllSTUnits");
                defaultOptions.adminOptions.Add("SELECT_ALL_MS_UNITS", "select * from AllMSUnits");

                //Define SQL query constants
                defaultOptions.adminOptions.Add("SELECT_ALL_FACTIONS", "select * from dbo.Factions");
                defaultOptions.adminOptions.Add("SELECT_ALL_LEADERS", "select * from dbo.Leaders");

                //Set optionsToWrite
                optionsToWrite = defaultOptions;
            }
            else
            {
                //Set optionsToWrite
                optionsToWrite = options;
                //DB Maxes for sanity check: weather 12, n-units 14, n-heroes 4, f-units 32, f-heroes 4
                if (int.Parse(optionsToWrite.adminOptions["MAX_WEATHER_CARDS"]) > 12)
                {
                    optionsToWrite.adminOptions["MAX_WEATHER_CARDS"] = "12";
                }
                if (int.Parse(optionsToWrite.adminOptions["MAX_NEUTRAL_UNITS"]) > 14)
                {
                    optionsToWrite.adminOptions["MAX_NEUTRAL_UNITS"] = "14";
                }
                if (int.Parse(optionsToWrite.adminOptions["MAX_NEUTRAL_HEROES"]) > 4)
                {
                    optionsToWrite.adminOptions["MAX_NEUTRAL_HEROES"] = "4";
                }
                if (int.Parse(optionsToWrite.adminOptions["MAX_FACTION_UNITS"]) > 32)
                {
                    optionsToWrite.adminOptions["MAX_FACTION_UNITS"] = "32";
                }
                if (int.Parse(optionsToWrite.adminOptions["MAX_FACTION_HEROES"]) > 4)
                {
                    optionsToWrite.adminOptions["MAX_FACTION_UNITS"] = "4";
                }
                
            }
            try
            {
                //Init json
                string json = JsonConvert.SerializeObject(optionsToWrite);
                //Write out to file
                System.IO.File.WriteAllText(filePath, json);
                //Set success flag
                writeSuccess = true;
            }
            catch
            {
                //Set fail flag to be caught by parent.
                writeSuccess = false;
            }
            return writeSuccess;
        }

        /// <summary>
        /// This function will read the existing app options from the configuration file text and return them in the AppOptions class object
        /// </summary>
        /// <param name="fileContents"></param>
        /// <returns></returns>
        public AppOptions ReadAppOptions(string fileContents)
        {
            AppOptions currentOptions = new AppOptions();
            try
            {
                string gwentAppOptions = fileContents;
                currentOptions = JsonConvert.DeserializeObject<AppOptions>(gwentAppOptions);
            }
            catch
            {
                currentOptions = new AppOptions();
                currentOptions.adminOptions = new Dictionary<string, string>();
            }
            return currentOptions;
        }
        /// <summary>
        /// Saves the options specififed in the Admin Page
        /// </summary>
        /// <param name="appOptions"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateAppOptions(AppOptions appOptions)
        {
            //Write the new options
            bool writeSuccess = WriteOptions(Global.gwentAppConfig.FilePath, appOptions, false);
            //Immediately force a read to get instant update, otherwise you're using cached
            Global.gAppOptions = ReadAppOptions(Global.gwentAppConfig.Value);
            return Redirect("~/Setup/");
        }

        /// <summary>
        /// Resets the options to their default values incase of melting
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ResetAppOptions()
        {
            //Write the new options
            bool writeSuccess = WriteOptions(Global.gwentAppConfig.FilePath, null, true);
            //Immediately force a read to get instant update, otherwise you're using cached
            Global.gAppOptions = ReadAppOptions(Global.gwentAppConfig.Value);
            return Redirect("~/Setup/");
        }
    }
}