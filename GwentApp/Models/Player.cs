using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GwentApp.Models
{

    public class Player
    {
        /// <summary>
        /// The player's Faction choice within the game.
        /// </summary>
        public string Faction
        {
            get;
            set;
        }

        /// <summary>
        /// A list of Faction SelectListItems that can be displayed in the View.
        /// </summary>
        public static List<SelectListItem> FactionSelectListItems
        {
            get
            {
                // Create a list of SelectListItems to populate.
                List<SelectListItem> listItems = new List<SelectListItem>();

                // Create list items called "Faction#" for the number of factions in the game.
                for (int i = 1; i < 5; i++)
                {
                    // Create a list item.
                    SelectListItem item = new SelectListItem();

                    // Give it a name and populate it's value and text representation.
                    string name = "Faction" + i;
                    item.Value = name;
                    item.Text = name;

                    // Add the item to the list.
                    listItems.Add(item);
                }

                // Return the populated list.
                return listItems;
            }
        }

        /// <summary>
        /// This this the name that the player goes by within the game.
        /// </summary>
        public string LeaderName
        {
            get;
            set;
        }
    }
}