using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GwentApp.Models
{
    /// <summary>
    /// This is the class to store the admin page options. Right now it's a simple dictionary
    /// </summary>
    public class AppOptions
    {
        /// <summary>
        /// Final count of cards dealt to player from initialized deck
        /// </summary>
        public int StartingDeckSize
        {
            get; set;
        }
        /// <summary>
        /// Max size of initialized deck
        /// </summary>
        public int MaxDeckSize
        {
            get; set;
        }
        /// <summary>
        /// Max number of weather cards to be dealt
        /// </summary>
        public int MaxWeatherCards
        {
            get; set;
        }
        /// <summary>
        /// Max number of neutral unit cards to be dealt
        /// </summary>
        public int MaxNeutralUnits
        {
            get; set;
        }
        /// <summary>
        /// Max number of neutral hero cards to be dealt
        /// </summary>
        public int MaxNeutralHeroes
        {
            get; set;
        }
        /// <summary>
        /// Max number of neutral special cards to be dealt
        /// </summary>
        public int MaxNeutralSpecials
        {
            get; set;
        }
        /// <summary>
        /// Max number of faction hero cards to be dealt
        /// </summary>
        public int MaxFactionHeroes
        {
            get; set;
        }
        /// <summary>
        /// Min number of faction unit cards that must be in the starting deck
        /// Solves a really rare case where you can get a deck comprised solely of neutrals. I hate those filthy neutrals, Kif!
        /// </summary>
        public int MinFactionUnits
        {
            get; set;
        }

        /// <summary>
        /// Max number of faction unit cards to be dealt
        /// </summary>
        public int MaxFactionUnits
        {
            get; set;
        }
        //Define SQL query constants. These work off of existing views in the database.
        /// <summary>
        /// Get all weather cards
        /// </summary>
        public string SelectAllWeatherCards
        {
            get; set;
        }
        /// <summary>
        /// Get all neutral unit cards
        /// </summary>
        public string SelectAllNeutralUnits
        {
            get; set;
        }
        /// <summary>
        /// Get all neutral hero cards
        /// </summary>
        public string SelectAllNeutralHeroes
        {
            get; set;
        }
        /// <summary>
        /// Get all neutral special cards
        /// </summary>
        public string SelectAllNeutralSpecials
        {
            get; set;
        }
        /// <summary>
        /// Get all NR faction hero cards.
        /// </summary>
        public string SelectAllNRHeroes
        {
            get; set;
        }
        /// <summary>
        /// Get all NE faction hero cards.
        /// </summary>
        public string SelectAllNEHeroes
        {
            get; set;
        }
        /// <summary>
        /// Get all ST faction hero cards.
        /// </summary>
        public string SelectAllSTHeroes
        {
            get; set;
        }
        /// <summary>
        /// Get all MS faction hero cards.
        /// </summary>
        public string SelectAllMSHeroes
        {
            get; set;
        }
        /// <summary>
        /// Get all NR faction unit cards.
        /// </summary>
        public string SelectAllNRUnits
        {
            get; set;
        }
        /// <summary>
        /// Get all NE faction hero cards.
        /// </summary>
        public string SelectAllNEUnits
        {
            get; set;
        }
        /// <summary>
        /// Get all ST faction hero cards.
        /// </summary>
        public string SelectAllSTUnits
        {
            get; set;
        }
        /// <summary>
        /// Get all MS faction hero cards.
        /// </summary>
        public string SelectAllMSUnits
        {
            get; set;
        }
        /// <summary>
        /// Get all factions
        /// </summary>
        public string SelectAllFactions
        {
            get; set;
        }
        /// <summary>
        /// Get all leaders
        /// </summary>
        public string SelectAllLeaders
        {
            get; set;
        }
        /// <summary>
        /// Get all player cards, across all factions and hero status. The motherload.
        /// </summary>
        public string SelectAllPlayerCards
        {
            get; set;
        }
        /// <summary>
        /// The string to search that identifies a Weather card
        /// </summary>
        public string WeatherIdentifier
        {
            get; set;
        }
        /// <summary>
        /// The string to search that identifies a Neutral card
        /// </summary>
        public string NeutralIdentifier
        {
            get; set;
        }
        /// <summary>
        /// The string to search that identifies a Ranged card
        /// </summary>
        public string RangedIdentifier
        {
            get; set;
        }
        /// <summary>
        /// The string to search that identifies a Siege card
        /// </summary>
        public string SiegeIdentifier
        {
            get; set;
        }
        /// <summary>
        /// The string to search that identifies a Close card
        /// </summary>
        public string CloseIdentifier
        {
            get; set;
        }
    }
}
