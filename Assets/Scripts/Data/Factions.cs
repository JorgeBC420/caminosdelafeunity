using UnityEngine;
using System.Collections.Generic;

namespace CaminoDeLaFe.Data
{
    /// <summary>
    /// Represents a faction in the game
    /// </summary>
    [System.Serializable]
    public class Faction
    {
        public string name;
        public Color color;
        public string description;
        public Vector3 startingPosition;
        public string[] allies;
        public string[] enemies;
        
        public Faction(string name, Color color, string description, Vector3 startingPosition, string[] allies = null, string[] enemies = null)
        {
            this.name = name;
            this.color = color;
            this.description = description;
            this.startingPosition = startingPosition;
            this.allies = allies ?? new string[0];
            this.enemies = enemies ?? new string[0];
        }
    }

    /// <summary>
    /// Static data for all factions in the game
    /// </summary>
    public static class Factions
    {
        private static Dictionary<string, Faction> _factions;
        
        public static Dictionary<string, Faction> All
        {
            get
            {
                if (_factions == null)
                {
                    InitializeFactions();
                }
                return _factions;
            }
        }
        
        private static void InitializeFactions()
        {
            _factions = new Dictionary<string, Faction>
            {
                {
                    "Cruzados",
                    new Faction(
                        "Cruzados",
                        new Color(1f, 0.84f, 0f), // Gold
                        "Guerreros santos de la cristiandad, luchando por la fe y la conquista de Tierra Santa.",
                        new Vector3(0, 0, 0),
                        allies: new string[] { },
                        enemies: new string[] { "Sarracenos" }
                    )
                },
                {
                    "Sarracenos", 
                    new Faction(
                        "Sarracenos",
                        new Color(0f, 0.8f, 0f), // Green
                        "Defensores del Islam, protegiendo sus tierras sagradas de los invasores.",
                        new Vector3(20, 0, 20),
                        allies: new string[] { },
                        enemies: new string[] { "Cruzados" }
                    )
                },
                {
                    "Antiguos",
                    new Faction(
                        "Antiguos",
                        new Color(0.5f, 0f, 0.5f), // Purple
                        "Guardianes de antiguos secretos y conocimientos perdidos.",
                        new Vector3(-20, 0, -20),
                        allies: new string[] { },
                        enemies: new string[] { }
                    )
                }
            };
        }
        
        public static Faction GetFaction(string factionName)
        {
            All.TryGetValue(factionName, out Faction faction);
            return faction;
        }
        
        public static string[] GetFactionNames()
        {
            return new string[] { "Cruzados", "Sarracenos", "Antiguos" };
        }
    }
}
