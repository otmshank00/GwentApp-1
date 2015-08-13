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
    }
}