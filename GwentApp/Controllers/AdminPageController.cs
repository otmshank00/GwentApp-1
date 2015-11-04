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
            Global.gAppOptions = new AppOptions();

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
                    Global.gAppOptions = ReadAppOptions(Global.gwentAppConfig.Value);
                }
            }
            return View(Global.gAppOptions);
        }

        /// <summary>
        /// This will write the app options to disk. Either a default set or a passed set based upon the last boolean argument
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="options"></param>
        /// <param name="isDefaults"></param>
        /// <returns></returns>
        public static bool WriteOptions(string filePath, AppOptions options, bool isDefaults)
        {
            //DB Maxes for sanity check: weather 12, n-units 14, n-heroes 4, f-units 32, f-heroes 4
            
            //Define check var
            bool writeSuccess = false;
            //Define write object
            AppOptions optionsToWrite = new AppOptions();
            if (isDefaults)
            {
                //Define object then fill it
                AppOptions defaultOptions = new AppOptions();
                //Final count of cards dealt to player from initialized deck
                defaultOptions.StartingDeckSize = 10;
                //Max size of initialized deck
                defaultOptions.MaxDeckSize = 28;
                //Max number of weather cards to be dealt
                defaultOptions.MaxWeatherCards = 4;
                //Max number of neutral unit cards to be dealt
                defaultOptions.MaxNeutralUnits = 3;
                //Max number of neutral hero cards to be dealt
                defaultOptions.MaxNeutralHeroes = 3;
                //Max number of neutral special cards to be dealt
                defaultOptions.MaxNeutralSpecials = 3;
                //Max number of faction hero cards to be dealt
                defaultOptions.MaxFactionHeroes = 3;
                //Max number of faction unit cards to be dealt
                defaultOptions.MaxFactionUnits = 12;
                //Min number of faction units to be included in deck.
                defaultOptions.MinFactionUnits = 2;

                //Define SQL query constants. These work off of existing views in the database.
                //Get all weather cards
                defaultOptions.SelectAllWeatherCards = "select * from AllWeather";
                //Get all neutral unit cards
                defaultOptions.SelectAllNeutralUnits = "select * from AllNeutralUnits";
                //Get all neutral hero cards
                defaultOptions.SelectAllNeutralHeroes = "select * from AllNeutralHeros";
                //Get all neutral special cards
                defaultOptions.SelectAllNeutralSpecials = "select * from AllNeutralSpecials";

                //Get all selected faction hero cards.
                defaultOptions.SelectAllNRHeroes = "select * from AllNRHeroes";
                defaultOptions.SelectAllNEHeroes = "select * from AllNEHeroes";
                defaultOptions.SelectAllSTHeroes = "select * from AllSTHeroes";
                defaultOptions.SelectAllMSHeroes = "select * from AllMSHeroes";
                //Get all selected faction unit cards.
                defaultOptions.SelectAllNRUnits = "select * from AllNRUnits";
                defaultOptions.SelectAllNEUnits = "select * from AllNEUnits";
                defaultOptions.SelectAllSTUnits = "select * from AllSTUnits";
                defaultOptions.SelectAllMSUnits = "select * from AllMSUnits";
                //Define SQL query constants
                defaultOptions.SelectAllFactions = "select * from dbo.factions";
                defaultOptions.SelectAllLeaders = "select * from dbo.leaders";
                defaultOptions.SelectAllPlayerCards = "select * from dbo.cards";

                //Define modal search values
                defaultOptions.CloseIdentifier = "Close";
                defaultOptions.SiegeIdentifier = "Siege";
                defaultOptions.RangedIdentifier = "Ranged";
                defaultOptions.WeatherIdentifier = "Weather";
                defaultOptions.NeutralIdentifier = "Neutral";
                

                //Set optionsToWrite
                optionsToWrite = defaultOptions;
            }
            else
            {
                //Set optionsToWrite
                optionsToWrite = options;
                //DB Maxes for sanity check: weather 12, n-units 14, n-heroes 4, f-units 32, f-heroes 4
                if (optionsToWrite.MaxWeatherCards > 12)
                {
                    optionsToWrite.MaxWeatherCards = 12;
                }
                if (optionsToWrite.MaxNeutralUnits > 14)
                {
                    optionsToWrite.MaxNeutralUnits = 14;
                }
                if (optionsToWrite.MaxNeutralHeroes > 4)
                {
                    optionsToWrite.MaxNeutralHeroes = 4;
                }
                if (optionsToWrite.MaxFactionUnits > 32)
                {
                    optionsToWrite.MaxFactionUnits = 32;
                }
                if (optionsToWrite.MaxFactionHeroes > 4)
                {
                    optionsToWrite.MaxFactionHeroes = 4;
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
        public static AppOptions ReadAppOptions(string fileContents)
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
                //currentOptions.adminOptions = new Dictionary<string, string>();
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