using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.ObjectModel;
using System.Text;
using System.Runtime.Serialization;

namespace GwentApp.Models
{
    [DataContract]
    public class Player
    {
        /// <summary>
        /// The player's complete uber model. Contains data on available factions and leaders. Stores selected faction and leader and shuffled deck.
        /// </summary>
        //A single faction object
        [DataMember]
        public FactionInfo Faction
        {
            get; set;
        }
        //A single leader object
        [DataMember]
        public LeaderInfo Leader
        {
            get; set;
        }
        //A list of card objects to pull starting deck from 
        [DataMember]
        public List<Card> Deck
        {
            get; set;
        }
        //A list of card objects that the player starts the game with
        [DataMember]
        public List<Card> Hand
        {
            get; set;
        }
        /// <summary>
        /// A list of cards that have been played. Tallied at rounds end
        /// </summary>
        [DataMember]
        public List<Card> Discard
        {
            get; set;
        }
        //A list of factions (pulled from db)
        [DataMember]
        public static List<FactionInfo> FactionList
        {
            get; set;
        }
        //A list of leaders (pulled from db)
        [DataMember]
        public static List<LeaderInfo> LeaderList
        {
            get; set;
        }
        //A list used to populate the drop down menu for faction choice
        [DataMember]
        public static List<SelectListItem> DdFactionList
        {
            get; set;
        }
        //A list used to populate the drop down menu for leader choice
        [DataMember]
        public static List<SelectListItem> DdLeaderList
        {
            get; set;
        }
        //The factioninfo object representing the selected faction
        [DataMember]
        public FactionInfo SelectedFaction
        {
            get; set;
        }
        //The leaderinfo object representing the selected leader
        [DataMember]
        public LeaderInfo SelectedLeader
        {
            get; set;
        }
        //Data for gameplay
        //Is it currently your turn?
        [DataMember]
        public bool MyTurn
        {
            get; set;
        }
        //Current number of wins
        [DataMember]
        public int WinCount
        {
            get; set;
        }
        //Other players current score
        [DataMember]
        public int RivalScore
        {
            get; set;
        }
        //The board state object reflecting what's going on in the game
        [DataMember]
        public BoardState PlayerBoardState
        {
            get; set;
        }
        /// <summary>
        /// The players guid, used in session variables and db operations (future)
        /// </summary>
        [DataMember]
        public Guid PlayerGuid
        {
            get; set;
        }
    }
}