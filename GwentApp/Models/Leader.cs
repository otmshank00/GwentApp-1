using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GwentApp.Models
{
    public class LeaderInfo
    {
        /// <summary>
        /// This object contains the properties comprising a single leader
        /// </summary>
        //The leaders name
        public string LeaderName
        {
            get; set;
        }
        //The leaders special ability
        public string LeaderAbility
        {
            get; set;
        }
        //The faction that the leader belongs to
        public string LeaderFaction
        {
            get; set;
        }
        //The abbreviation of the faction that the leader belongs to
        public string LeaderFactionAbbr
        {
            get; set;
        }
        //The database id for the leader
        public int LeaderId
        {
            get; set;
        }
    }
}
