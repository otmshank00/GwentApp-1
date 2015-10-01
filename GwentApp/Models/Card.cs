using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GwentApp.Models
{
    public class Card
    {
        /// <summary>
        /// The name of the person or unit represented by this card.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// The strength of this card!
        /// </summary>
        public int Power
        {
            get;
            set;
        }

        //Card Faction
        public string Faction
        {
            get;
            set;
        }

        //Card Range
        public string Range
        {
            get; set;
        }

        //Card Quote
        public string Quote
        {
            get; set;
        }

        //Is Hero?
        public bool Hero
        {
            get; set;
        }

        //Card Ability
        public string Ability
        {
            get; set;
        }

        //The picture path for the card
        public string picturePath
        {
            get; set;
        }
    }
}