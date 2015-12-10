using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using GwentApp.Models;

namespace GwentApp.Models
{
    /// <summary>
    /// This class represents the board
    /// </summary>
    [DataContract]
    public class BoardState
    {
        /// <summary>
        /// Cards in this list are added to the close combat row for Player
        /// </summary>
        [DataMember]
        public List<Card> CloseCombat
        {
            get; set;
        }
        /// <summary>
        /// Cards in this list are added to the ranged combat row for Player
        /// </summary>
        [DataMember]
        public List<Card> RangedCombat
        {
            get; set;
        }
        /// <summary>
        /// Cards in this list are added to the siege combat row for Player
        /// </summary>
        [DataMember]
        public List<Card> SiegeCombat
        {
            get; set;
        }
        /// <summary>
        /// The total strength of the close combat row for Player
        /// </summary>
        [DataMember]
        public int CloseCombatTotal
        {
            get; set;
        }
        /// <summary>
        /// The total strength of the ranged combat row for Player 
        /// </summary>
        [DataMember]
        public int RangedCombatTotal
        {
            get; set;
        }
        /// <summary>
        /// The total strength of the siege combat row for Player
        /// </summary>
        [DataMember]
        public int SiegeCombatTotal
        {
            get; set;
        }
        /// <summary>
        /// The combined total of all rows in the round for Player
        /// </summary>
        [DataMember]
        public int RoundTotal
        {
            get; set;
        }
        /// <summary>
        /// Boolean value indicating if Player has passed
        /// </summary>
        [DataMember]
        public bool Passed
        {
            get; set;
        }
        /// <summary>
        /// This is the leader card for Player . Filled if card is on the board and active
        /// </summary>
        [DataMember]
        public Card LeaderCard
        {
            get; set;
        }
        /// <summary>
        /// Boolean value indicating if Player has used their leader ability
        /// </summary>
        [DataMember]
        public bool LeaderAbilityUsed
        {
            get; set;
        }
        /// <summary>
        /// A storage place to set what the last played card was
        /// </summary>
        [DataMember]
        public string PlayedCardId
        {
            get; set;
        }
        /// <summary>
        /// The round number we are currently on. 3 maximum
        /// </summary>
        [DataMember]
        public int RoundNumber
        {
            get; set;
        }
        /// <summary>
        /// If a biting frost card is played then this becomes true. Sets ALL close combat card power to 1 (except hero)
        /// </summary>
        [DataMember]
        public bool IsSnowing
        {
            get; set;
        }
        /// <summary>
        /// If a torrential rain card is played then this becomes true. Sets ALL siege combat card power to 1 (except hero)
        /// </summary>
        [DataMember]
        public bool IsRaining
        {
            get; set;
        }
        /// <summary>
        /// If an impenatrable fog card is played then this becomes true. Sets ALL ranged combat card power to 1 (except hero)
        /// </summary>
        [DataMember]
        public bool IsFoggy
        {
            get; set;
        }
        /// <summary>
        /// Is there a commanders horn in the close combat row?
        /// </summary>
        [DataMember]
        public bool HornInCloseRow
        {
            get; set;
        }
        /// <summary>
        /// Is there a commanders horn in the ranged combat row?
        /// </summary>
        [DataMember]
        public bool HornInRangedRow
        {
            get; set;
        }
        /// <summary>
        /// Is there a commanders horn in the siege combat row?
        /// </summary>
        [DataMember]
        public bool HornInSiegeRow
        {
            get; set;
        }
    }
}