/*
██████╗ ██╗   ██╗     █████╗ ██╗     ██████╗ ██╗  ██╗ █████╗  ██████╗  ██████╗ ██████╗ 
██╔══██╗╚██╗ ██╔╝    ██╔══██╗██║     ██╔══██╗██║  ██║██╔══██╗██╔════╝ ██╔═══██╗██╔══██╗
██████╔╝ ╚████╔╝     ███████║██║     ██████╔╝███████║███████║██║  ███╗██║   ██║██║  ██║
██╔══██╗  ╚██╔╝      ██╔══██║██║     ██╔═══╝ ██╔══██║██╔══██║██║   ██║██║   ██║██║  ██║
██████╔╝   ██║       ██║  ██║███████╗██║     ██║  ██║██║  ██║╚██████╔╝╚██████╔╝██████╔╝
╚═════╝    ╚═╝       ╚═╝  ╚═╝╚══════╝╚═╝     ╚═╝  ╚═╝╚═╝  ╚═╝ ╚═════╝  ╚═════╝ ╚═════╝ 
*/

                                                         
using System;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System.Linq;



namespace Scripter
{
    class Program
    {
        static Menu menu;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        public static void OnGameLoad(EventArgs args)
        {
            LoadMenu();
            Game.OnGameUpdate += OnGameUpdate;
            Game.PrintChat("<font color=\"#00BFFF\">Scripter</font> <font color=\"#FFFFFF\"> - Loaded!</font>");
        }

        public static Vector2[] LastWaypointPosition = { Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero };
        public static int[] Waypoint = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] Cheater = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] Timer = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static Vector2 WaypointPosition;

        public static void OnGameUpdate(EventArgs args)
        {
            int i = -1;

            int waypointCount = menu.Item("waypointCount").GetValue<Slider>().Value;

            foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                if (!menu.Item("enable").GetValue<bool>()) break;

                i += 1;

                if (hero.IsMe) continue;

                if (Cheater[i] == 1) continue;

                if (!hero.IsVisible) continue;

                WaypointPosition = hero.GetWaypoints().Last();

                if (LastWaypointPosition[i] != WaypointPosition)// && Vector2.Distance(WaypointPosition, (Vector2)hero.Position) <= 550 && Vector2.Distance(WaypointPosition, (Vector2)hero.Position) >= 300)
                {
                    Waypoint[i] += 1;
                    LastWaypointPosition[i] = hero.GetWaypoints().Last();
                }

                if (Environment.TickCount - Timer[i] >= 2000)
                {
                    Console.WriteLine(Waypoint[i]);

                    if (Waypoint[i] >= (waypointCount * 2))
                    {
                        Game.PrintChat("<font color=\"#FF0000\">" + hero.ChampionName + " is using a Script</font>");
                        Cheater[i] = 1;
                    }

                    Waypoint[i] = 0;
                    Timer[i] = Environment.TickCount;

                }
            }
        }

        static void LoadMenu()
        {
            menu = new Menu("Scripter", "Scripter", true);
            menu.AddItem(new MenuItem("enable", "Enable / Disable").SetValue(true));
            menu.AddItem(new MenuItem("waypointCount", "Waypoints per second to flag as Scripter").SetValue(new Slider(9, 8, 15)));
            menu.AddToMainMenu();
        }
    }
}
