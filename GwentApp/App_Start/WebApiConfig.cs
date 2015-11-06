using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Cors;

namespace GwentApp
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Force all Web API requests to return just JSON (we hate XML).
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());
            config.Formatters.JsonFormatter.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;

            // Enable CORS.
            config.EnableCors();

            // Custom routes for Card API Controller

            config.Routes.MapHttpRoute(
                name: "GetAllCardsService",
                routeTemplate: "card/all",
                defaults: new { controller = "Card", action = "GetAllCards" }
            );

            config.Routes.MapHttpRoute(
                name: "GetFactionsService",
                routeTemplate: "card/factions",
                defaults: new { controller = "Card", action = "GetAllFactions" }
            );

            config.Routes.MapHttpRoute(
                name: "GetLeadersService",
                routeTemplate: "card/leaders",
                defaults: new { controller = "Card", action = "GetAllLeaders" }
            );

            config.Routes.MapHttpRoute(
                name: "GetByPowerService",
                routeTemplate: "card/power/{power}",
                defaults: new { controller = "Card", action = "GetByPower" }
            );
            
            config.Routes.MapHttpRoute(
                name: "GetLeadersByFactionAbbreviationService",
                routeTemplate: "card/leaders/byfaction/{factionAbbreviation}",
                defaults: new { controller = "Card", action= "GetLeadersByFactionAbbreviation" }
            );

            config.Routes.MapHttpRoute(
                name: "GetLeaderByNameService",
                routeTemplate: "card/leaders/byname/{leaderName}",
                defaults: new { controller = "Card", action = "GetLeaderByName" }
            );

            config.Routes.MapHttpRoute(
                name: "BuildDeckByFactionService",
                routeTemplate: "card/builddeck/byfaction/{factionAbbreviation}",
                defaults: new { controller = "Card", action = "BuildDeckByFaction" }
            );
            
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
