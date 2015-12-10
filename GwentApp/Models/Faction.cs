using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GwentApp.Models
{
    public class FactionInfo
    {
        /// <summary>
        /// The object representing a single faction
        /// </summary>
        //The factions name
        public string FactionName
        {
            get; set;
        }
        //The factions abbreviation
        public string FactionAbbr
        {
            get; set;
        }
        //The factions ability perk
        public string FactionPerk
        {
            get; set;
        }
        //The faction id from database
        public int FactionId
        {
            get; set;
        }
    }
}
