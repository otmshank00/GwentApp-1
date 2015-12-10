using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace GwentApp.Models
{
    [DataContract]
    public class Card
    {
        /// <summary>
        /// The name of the person or unit represented by this card.
        /// </summary>
        [DataMember]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// The strength of this card!
        /// </summary>
        [DataMember]
        public int Power
        {
            get;
            set;
        }

        //Card Faction
        [DataMember]
        public string Faction
        {
            get;
            set;
        }

        //Card Range
        [DataMember]
        public string Range
        {
            get; set;
        }

        //Card Quote
        [DataMember]
        public string Quote
        {
            get; set;
        }

        //Is Hero?
        [DataMember]
        public bool Hero
        {
            get; set;
        }

        //Card Ability
        [DataMember]
        public string Ability
        {
            get; set;
        }

        //The picture path for the card
        [DataMember]
        public string ImageFilePath
        {
            get; set;
        }

        //The card ID from the database
        [DataMember]
        public int CardId
        {
            get; set;
        }
    }
}