using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.ObjectModel;
using System.Text;

namespace GwentApp.Models
{   
    public class Player
    {
        /// <summary>
        /// The player's complete uber model. Contains data on available factions and leaders. Stores selected faction and leader and shuffled deck.
        /// </summary>
        //A single faction object
        public FactionInfo Faction
        {
            get; set;
        }
        //A single leader object
        public LeaderInfo Leader
        {
            get; set;
        }
        //A list of card objects to pull starting deck from 
        public List<Card> Deck
        {
            get; set;
        }
        //A list of card objects that the player starts the game with
        public List<Card> StartDeck
        {
            get; set;
        }
        //A list of factions (pulled from db)
        public static List<FactionInfo> FactionList
        {
            get; set;
        }
        //A list of leaders (pulled from db)
        public static List<LeaderInfo> LeaderList
        {
            get; set;
        }
        //A list used to populate the drop down menu for faction choice
        public static List<SelectListItem> DdFactionList
        {
            get; set;
        }
        //A list used to populate the drop down menu for leader choice
        public static List<SelectListItem> DdLeaderList
        {
            get; set;
        }
        //The factioninfo object representing the selected faction
        public FactionInfo SelectedFaction
        {
            get; set;
        }
        //The leaderinfo object representing the selected leader
        public LeaderInfo SelectedLeader
        {
            get; set;
        }
        //Data for gameplay
        //Is it currently your turn?
        public bool MyTurn
        {
            get; set;
        }
        //Current number of wins
        public int WinCount
        {
            get; set;
        }
        //Other players current score
        public int RivalScore
        {
            get; set;
        }
    }
}