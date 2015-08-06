/*
██████╗ ██╗   ██╗     █████╗ ██╗     ██████╗ ██╗  ██╗ █████╗  ██████╗  ██████╗ ██████╗ 
██╔══██╗╚██╗ ██╔╝    ██╔══██╗██║     ██╔══██╗██║  ██║██╔══██╗██╔════╝ ██╔═══██╗██╔══██╗
██████╔╝ ╚████╔╝     ███████║██║     ██████╔╝███████║███████║██║  ███╗██║   ██║██║  ██║
██╔══██╗  ╚██╔╝      ██╔══██║██║     ██╔═══╝ ██╔══██║██╔══██║██║   ██║██║   ██║██║  ██║
██████╔╝   ██║       ██║  ██║███████╗██║     ██║  ██║██║  ██║╚██████╔╝╚██████╔╝██████╔╝
╚═════╝    ╚═╝       ╚═╝  ╚═╝╚══════╝╚═╝     ╚═╝  ╚═╝╚═╝  ╚═╝ ╚═════╝  ╚═════╝ ╚═════╝ 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;
using Font = SharpDX.Direct3D9.Font;
using FontDrawFlags = SharpDX.Direct3D9.FontDrawFlags;
using Vector2 = SharpDX.Vector2;

namespace GodJungleTracker
{
    class Program
    {
        static Menu _menu;

        #region Definitions

        public static Font MinimapText = new Font(Drawing.Direct3DDevice,
                        new FontDescription
                        {
                            FaceName = "Calibri",
                            Height = 13,
                            OutputPrecision = FontPrecision.Default,
                            Quality = FontQuality.Default
                        });

        public static Font MapText = new Font(Drawing.Direct3DDevice,
                    new FontDescription
                    {
                        FaceName = "Calibri",
                        Height = 13,
                        OutputPrecision = FontPrecision.Default,
                        Quality = FontQuality.Default
                    });

        public static short HeaderRangedAttack = 1000;

        public static Utility.Map.MapType MapType { get; set; }

        public static short HeaderMeleeAttack = 1000;

        public static short HeaderDisengaged = 1000;

        public static short HeaderSkill = 1000;

        public static short HeaderCreateGromp = 1000;

        public static float BaronSpawn = 1199;

        public static int EnemyTeamStacks;

        public static int AllyTeamStacks;

        public static bool EnemyTeamNashor;

        public static bool AllyTeamNashor;

        public static string TestMinionName = "";

        public static int TestMinionState = 0;

        public static int GuessNetworkId1 = 1;

        public static int GuessNetworkId2 = 1;

        public static int GuessDragonId = 1;

        public static int EnemyFoWTime;

        public static int AllyFoWTime;

        public static int LastPlayedDragonSound;

        public static int LastPlayedBaronSound;

        public static int LastPlayedBaronSound2;

        public static int BaronSoundDelay;

        public static int Seed1 = 3;

        public static int Seed2 = 2;

        public static float ClockTimeAdjust;

        public static int BiggestNetworkId;

        public static int L;

        public static int BufferDragonSound;

        public static int PlayingDragonSound;

        public static bool timeronmap;
        public static bool timeronminimap;
        public static int circleradius;
        public static Color colorattacking;
        public static Color colortracked;
        public static Color colordisengaged;
        public static Color colordead;
        public static Color colorguessed;
        public static int circlewidth;
        public static bool trackonminimap;

        public static ColorBGRA white = new ColorBGRA(255, 255, 255, 255);

        public static Vector3[] CampPosition = 
        { 
        new Vector3(9866f  , 4414f  , -71f),
        new Vector3(5007f  , 10471f , -71f),
        new Vector3(3872f  , 7900f  , 51f),
        new Vector3(10930f , 6992f  , 52f),
        new Vector3(7862f  , 4111f  , 54f),
        new Vector3(7017f  , 10775f , 56f),
        new Vector3(10508f , 5271f  , -62f),
        new Vector3(4418f  , 9664f  , -69f),
        new Vector3(7857f  , 9471f  , 52f),
        new Vector3(6954f  , 5458f  , 53f),
        new Vector3(2091f  , 8428f  , 52f),
        new Vector3(12703f , 6444f  , 52f),
        new Vector3(6449f  , 12117f , 56f),
        new Vector3(8381f  , 2711f  , 51f),
        new Vector3(10957f , 8350f  , 62f),
        new Vector3(3825f  , 6491f  , 52f),
        };

        private static readonly SoundPlayer Danger = new SoundPlayer(Properties.Resources.danger);
        private static readonly SoundPlayer Danger10 = new SoundPlayer(Properties.Resources.danger10);
        private static readonly SoundPlayer Danger25 = new SoundPlayer(Properties.Resources.danger25);
        private static readonly SoundPlayer Danger50 = new SoundPlayer(Properties.Resources.danger50);
        private static readonly SoundPlayer Danger75 = new SoundPlayer(Properties.Resources.danger75);
        private static SoundPlayer _sound = Danger;

        public static List<Obj_AI_Minion> TrackingList { get; set; }

        public static int[] OnFow = { 0, 0 };

        public static int[] OnScreen = { 0, 0 };

        public static int[] NetworkId = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public static int[] JustDied = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public static int[] LastChangeOnState = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public static int[] LastChangeOnCampState = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public static string[] TimerTextOnMap = { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };

        public static string[] TimerTextOnMinimap = { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };

        public static int[] TimerPosXOnMap = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public static int[] TimerPosYOnMap = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public static int[] TimerPosXOnMinimap = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public static int[] TimerPosYOnMinimap = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public static int[] HeroNetworkId = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public static string[] Heros = { "Caitlyn", "Fizz", "Jinx", "Nidalee", "Wukong", "Zed", "Zyra", "Kalista", "Yasuo" };

        public static string[] HeroName = { "", "", "", "", "", "", "", "", "", "" };

        public static int[] State = { 0, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public static int[] UnitToCamp = { 0, 1, 4, 7, 10, 13, 24, 25, 19, 23, 14, 15, 27, 29, 32, 35 };

        public static int[] CampState = { 0, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public static int[] SeedOrder = { 0, 1, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0, 0, 1, 0, 0, 1, 0 };

        public static int[] CreateOrder = { 14, 15, 10, 9, 8, 13, 12, 11, 4, 3, 2, 7, 6, 5, 23, 22, 21, 20, 29, 28, 27, 26, 19, 18, 17, 16, 35, 34, 33, 32, 31, 30 };

        public static int[] IdOrder = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 10, 0, 0, 0, 2, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public static int[] CampRespawnTime = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public static int[] CampRespawnTimer = { 360, 
                                                 420, 
                                                 300, 
                                                 300, 
                                                 300, 
                                                 300, 
                                                 180, 
                                                 180, 
                                                 100, 
                                                 100, 
                                                 100, 
                                                 100, 
                                                 100, 
                                                 100, 
                                                 100, 
                                                 100 };


        public static string[] CampName = { "Dragon",
                                            "Baron",
                                            "Blue W", 
                                            "Blue E", 
                                            "Red S", 
                                            "Red N", 
                                            "Crab S", 
                                            "Crab N", 
                                            "Raptor N", 
                                            "Raptor S", 
                                            "Gromp W", 
                                            "Gromp E", 
                                            "Krug N", 
                                            "Krug S", 
                                            "Wolf E", 
                                            "Wolf W" };


        public static string[] NameToCompare = {"SRU_Dragon6.1.1", 
                                                "SRU_Baron12.1.1",
                                                "SRU_BlueMini21.1.3", "SRU_BlueMini1.1.2", "SRU_Blue1.1.1", //4
                                                "SRU_BlueMini27.1.3", "SRU_BlueMini7.1.2", "SRU_Blue7.1.1", //7
                                                "SRU_RedMini4.1.3", "SRU_RedMini4.1.2", "SRU_Red4.1.1",//10
                                                "SRU_RedMini10.1.3", "SRU_RedMini10.1.2", "SRU_Red10.1.1", //13
                                                "SRU_Gromp13.1.1",
                                                "SRU_Gromp14.1.1", 
                                                "SRU_RazorbeakMini9.1.4", "SRU_RazorbeakMini9.1.3", "SRU_RazorbeakMini9.1.2", "SRU_Razorbeak9.1.1",//19
                                                "SRU_RazorbeakMini3.1.4", "SRU_RazorbeakMini3.1.3", "SRU_RazorbeakMini3.1.2", "SRU_Razorbeak3.1.1",//23 
                                                "Sru_Crab15.1.1",
                                                "Sru_Crab16.1.1",
                                                "SRU_Krug11.1.2", "SRU_KrugMini11.1.1", //27
                                                "SRU_Krug5.1.2", "SRU_KrugMini5.1.1",//29
                                                "SRU_MurkwolfMini8.1.3", "SRU_MurkwolfMini8.1.2", "SRU_Murkwolf8.1.1", //32
                                                "SRU_MurkwolfMini2.1.3", "SRU_MurkwolfMini2.1.2", "SRU_Murkwolf2.1.1", //35
                                                "SRU_BaronSpawn12.1.2", //36
                                                "TT_NGolem2.1.1", "TT_NGolem22.1.2",
                                                "TT_NWraith21.1.3", "TT_NWraith21.1.2", "TT_NWraith1.1.1", 
                                                "TT_NWolf23.1.3", "TT_NWolf23.1.2", "TT_NWolf3.1.1",
                                                "TT_NWraith24.1.3", "TT_NWraith24.1.2", "TT_NWraith4.1.1",
                                                "TT_NWolf26.1.3", "TT_NWolf26.1.2", "TT_NWolf6.1.1",
                                                "TT_NGolem5.1.1", "TT_NGolem25.1.2",
                                                "TT_Spiderboss8.1.1",
                                                "TT_Relic7.1.1"};

        #endregion

        static void Main()
        {
            Game.OnStart += OnGameStart;
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        public static void OnGameStart(EventArgs args)
        {
            ClockTimeAdjust = Game.ClockTime;

            CampRespawnTime[0] = (int)((145f + Game.ClockTime) * 1000) + Environment.TickCount;

            CampRespawnTime[1] = (int)((1195f + Game.ClockTime) * 1000) + Environment.TickCount;
        }

        public static void OnGameLoad(EventArgs args)
        {
            if (Game.MapId.ToString() != "SummonersRift")
            {
                return;
            }

            TrackingList = new List<Obj_AI_Minion>();

            GameObject.OnCreate += GameObjectOnCreate;
            GameObject.OnDelete += GameObjectOnDelete;
            GameObject.OnFloatPropertyChange += OnFloatChange;
            Game.OnProcessPacket += OnProcessPacket;
            Game.OnUpdate += OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;

            SetPacketId();
            LoadMenu();
            //Game.PrintChat("<font color=\"#00BFFF\">God Jungle Tracker</font> <font color=\"#FFFFFF\"> - Loaded</font>");
            //Notifications.AddNotification(new Notification("God Jungle Tracker - Loaded", 3000).SetTextColor(Color.FromArgb(136, 207, 240)));

            foreach (Obj_AI_Minion minion in ObjectManager.Get<Obj_AI_Minion>().Where(x => x.Name.Contains("SRU_") || x.Name.Contains("Sru_")))
            {
                for (int i = 0; i <= 35; i++)
                {
                    if (!_menu.Item("dragon").GetValue<bool>() && i == 0) continue;
                    if (!_menu.Item("baron").GetValue<bool>() && i == 1) continue;
                    if (!_menu.Item("blue").GetValue<bool>() && i >= 2 && i <= 7) continue;
                    if (!_menu.Item("red").GetValue<bool>() && i >= 8 && i <= 13) continue;
                    if (!_menu.Item("gromp").GetValue<bool>() && i >= 14 && i <= 15) continue;
                    if (!_menu.Item("raptor").GetValue<bool>() && i >= 16 && i <= 23) continue;
                    if (!_menu.Item("crab").GetValue<bool>() && i >= 24 && i <= 25) continue;

                    if (minion.Name.Contains(NameToCompare[i]))
                    {
                        if (!minion.IsDead && NetworkId[i] != minion.NetworkId)
                        {
                            NetworkId[i] = minion.NetworkId;
                            State[i] = 1;
                            LastChangeOnState[i] = Environment.TickCount;
                            //Console.WriteLine(minion.NetworkId + " Name: " + minion.Name);
                            TrackingList.Add(minion);
                            
                        }
                    }
                }
            }

            MinimapText = new Font(Drawing.Direct3DDevice,
                     new FontDescription
                     {
                         FaceName = "Calibri",
                         Height = _menu.Item("timerfontminimap").GetValue<Slider>().Value,
                         OutputPrecision = FontPrecision.Default,
                         Quality = FontQuality.Default
                     });

            MapText = new Font(Drawing.Direct3DDevice,
                    new FontDescription
                    {
                        FaceName = "Calibri",
                        Height = _menu.Item("timerfontmap").GetValue<Slider>().Value,
                        OutputPrecision = FontPrecision.Default,
                        Quality = FontQuality.Default
                    });
            

            if (Game.ClockTime > 400f)
            {
                GuessNetworkId1 = 0;

                GuessNetworkId2 = 0;
            }

            int c = 0;
            foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                HeroNetworkId[c] = hero.NetworkId;
                HeroName[c] = hero.BaseSkinName;
                //Console.WriteLine(HeroName[c] + " ; " + HeroNetworkID[c]);
                c++;
                if (hero.NetworkId > BiggestNetworkId)
                {
                    BiggestNetworkId = hero.NetworkId;
                }

                if (!hero.IsAlly)
                {
                    for (int i = 0; i <= 8; i++)
                    {
                        if (hero.ChampionName.Contains(Heros[i]))
                        {
                            GuessDragonId = 0;
                        }
                    }
                }
            }
        }

        private static void SetPacketId()
        {
            if (!Game.Region.Substring(0, 2).Equals("HN"))
            {
                if (Game.Version.StartsWith("5.15"))
                {
                    HeaderRangedAttack = 244;

                    HeaderMeleeAttack = 166;

                    HeaderDisengaged = 59;

                    HeaderSkill = 54;

                    HeaderCreateGromp = 320;
                }
                else if (Game.Version.StartsWith("5.14"))
                {
                    HeaderRangedAttack = 112;

                    HeaderMeleeAttack = 115;

                    HeaderDisengaged = 274;

                    HeaderSkill = 91;

                    HeaderCreateGromp = 59;
                }
                else if (Game.Version.StartsWith("5.13"))
                {
                    HeaderRangedAttack = 186;

                    HeaderMeleeAttack = 265;

                    HeaderDisengaged = 65;

                    HeaderSkill = 57;

                    HeaderCreateGromp = 23;
                }
                else
                {
                    Notifications.AddNotification(
                        new Notification("God Jungle Tracker", 10000).SetTextColor(Color.FromArgb(255, 0, 0)));
                    Notifications.AddNotification(
                        new Notification("is not updated", 10000).SetTextColor(Color.FromArgb(255, 0, 0)));
                    Notifications.AddNotification(
                        new Notification("for game version: " + Game.Version.Substring(0,4), 10000).SetTextColor(Color.FromArgb(255, 0, 0)));
                    Notifications.AddNotification(
                        new Notification("and region: " + Game.Region, 10000).SetTextColor(Color.FromArgb(255, 0, 0)));
                }
            }
            else
            {
                if (Game.Version.StartsWith("5.13"))
                {
                    HeaderRangedAttack = 186;

                    HeaderMeleeAttack = 265;

                    HeaderDisengaged = 65;

                    HeaderSkill = 57;

                    HeaderCreateGromp = 23;
                }
                else if (Game.Version.StartsWith("5.12"))
                {
                    HeaderRangedAttack = 186;

                    HeaderMeleeAttack = 118;

                    HeaderDisengaged = 52;

                    HeaderSkill = 159;

                    HeaderCreateGromp = 61;
                }
                else if (Game.Version.StartsWith("5.11"))
                {
                    HeaderRangedAttack = 182;

                    HeaderMeleeAttack = 290;

                    HeaderDisengaged = 165;

                    HeaderSkill = 187;

                    HeaderCreateGromp = 51;
                }
                else
                {
                    Notifications.AddNotification(
                        new Notification("God Jungle Tracker", 10000).SetTextColor(Color.FromArgb(255, 0, 0)));
                    Notifications.AddNotification(
                        new Notification("is not updated", 10000).SetTextColor(Color.FromArgb(255, 0, 0)));
                    Notifications.AddNotification(
                        new Notification("for game version: " + Game.Version.Substring(0, 4), 10000).SetTextColor(Color.FromArgb(255, 0, 0)));
                    Notifications.AddNotification(
                        new Notification("and region: " + Game.Region, 10000).SetTextColor(Color.FromArgb(255, 0, 0)));
                }
            }
        }

        private static void PlaySound(SoundPlayer sound = null)
        {
            if (sound != null)
            {
                try
                {
                    sound.Play();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        private static void GameObjectOnCreate(GameObject sender, EventArgs args)
        {
            if (!(sender is Obj_AI_Minion) || sender.Team != GameObjectTeam.Neutral)
            {
                return;
            }

            var minion = (Obj_AI_Minion)sender;
            var n = minion.Name;

            //Console.WriteLine("Added " + minion.Name + " to the Tracking List " + minion.NetworkId);

            for (int i = 0; i <= 35; i++)
            {
                if (!_menu.Item("dragon").GetValue<bool>() && i == 0) continue;
                if (!_menu.Item("baron").GetValue<bool>() && i == 1) continue;
                if (!_menu.Item("blue").GetValue<bool>() && i >= 2 && i <= 7) continue;
                if (!_menu.Item("red").GetValue<bool>() && i >= 8 && i <= 13) continue;
                if (!_menu.Item("gromp").GetValue<bool>() && i >= 14 && i <= 15) continue;
                if (!_menu.Item("raptor").GetValue<bool>() && i >= 16 && i <= 23) continue;
                if (!_menu.Item("crab").GetValue<bool>() && i >= 24 && i <= 25) continue;

                if (NameToCompare[i] == n)
                {
                    NetworkId[i] = minion.NetworkId;
                    State[i] = 1;
                    LastChangeOnState[i] = Environment.TickCount;
                    JustDied[i] = 0;
                    //Console.WriteLine("Added " + minion.Name + " to the Tracking List " + minion.NetworkId);
                    TrackingList.Add(minion);

                    if (i == 0 && Game.ClockTime < (BaronSpawn + ClockTimeAdjust))
                    {
                        ClockTimeAdjust = 0;
                        BaronSpawn = Game.ClockTime-1;
                    }
                    return;
                }
            }
        }

        public static void OnFloatChange(GameObject sender, GameObjectFloatPropertyChangeEventArgs args)
        {
            if (sender.Team != GameObjectTeam.Neutral || !sender.IsDead || !(sender is Obj_AI_Minion) || args.Property != "mHP" || (int)args.NewValue != 0)
            {
                return;
            }

            var minion = (Obj_AI_Minion)sender;
            var n = minion.Name;

            for (int i = 0; i <= 35; i++)
            {
                if (!_menu.Item("dragon").GetValue<bool>() && i == 0) continue;
                if (!_menu.Item("baron").GetValue<bool>() && i == 1) continue;
                if (!_menu.Item("blue").GetValue<bool>() && i >= 2 && i <= 7) continue;
                if (!_menu.Item("red").GetValue<bool>() && i >= 8 && i <= 13) continue;
                if (!_menu.Item("gromp").GetValue<bool>() && i >= 14 && i <= 15) continue;
                if (!_menu.Item("raptor").GetValue<bool>() && i >= 16 && i <= 23) continue;
                if (!_menu.Item("crab").GetValue<bool>() && i >= 24 && i <= 25) continue;

                if (NameToCompare[i] == n)
                {
                    State[i] = 7;
                    LastChangeOnState[i] = Environment.TickCount;
                    JustDied[i] = 1;
                    //Console.WriteLine("Removed " + minion.Name + " of the Tracking List");
                    try
                    {
                        TrackingList.RemoveAll(x => x.Name == minion.Name);
                    }
                    catch (Exception)
                    {

                        //ignored
                    }
                    return;
                }
            }

            //Console.WriteLine("Object: " + sender.Name + " Property changed: " + args.Property + " Value: " + args.NewValue + " IsDead: " + sender.IsDead);
        }

        private static void GameObjectOnDelete(GameObject sender, EventArgs args)
        {
            if (!(sender is Obj_AI_Minion) || sender.Team != GameObjectTeam.Neutral)
            {
                return;
            }

            var minion = (Obj_AI_Minion)sender;
            var n = minion.Name;

            for (int i = 0; i <= 35; i++)
            {
                if (!_menu.Item("dragon").GetValue<bool>() && i == 0) continue;
                if (!_menu.Item("baron").GetValue<bool>() && i == 1) continue;
                if (!_menu.Item("blue").GetValue<bool>() && i >= 2 && i <= 7) continue;
                if (!_menu.Item("red").GetValue<bool>() && i >= 8 && i <= 13) continue;
                if (!_menu.Item("gromp").GetValue<bool>() && i >= 14 && i <= 15) continue;
                if (!_menu.Item("raptor").GetValue<bool>() && i >= 16 && i <= 23) continue;
                if (!_menu.Item("crab").GetValue<bool>() && i >= 24 && i <= 25) continue;

                if (NameToCompare[i] == n)
                {
                    if (LastChangeOnState[i] < Environment.TickCount + 10000)
                    {
                        State[i] = 7;
                        LastChangeOnState[i] = Environment.TickCount - 3000;
                        JustDied[i] = 1;
                        //Console.WriteLine("Removed " + minion.Name + " of the Tracking List");
                        try
                        {
                            TrackingList.RemoveAll(x => x.Name == minion.Name);
                        }
                        catch (Exception)
                        {
                            
                            //ignored
                        }
                        
                    }
                    
                    return;
                }
            }
        }

        public static void OnGameUpdate(EventArgs args)
        {
            #region Update CampState

            if (L <= 11 && (L < 2 || L > 5) && L != 8 && L != 9)
            {

                var visible = 0;

                CampState[L] = State[UnitToCamp[L]];
                LastChangeOnCampState[L] = LastChangeOnState[UnitToCamp[L]];

                try
                {
                    foreach (Obj_AI_Minion minion in TrackingList.Where(x => x.IsVisible && x.Name.Contains(NameToCompare[UnitToCamp[L]]) && !x.IsDead))
                    {
                        visible = 1;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(@"Here ------" + ex);
                }

                if ((CampState[L] == 7 || CampState[L] == 4) && visible == 1)
                {
                    State[UnitToCamp[L]] = 1;
                    CampState[L] = 1;
                    JustDied[UnitToCamp[L]] = 0;
                }

                if (JustDied[UnitToCamp[L]] == 1)
                {
                    CampRespawnTime[L] = (LastChangeOnCampState[L] + CampRespawnTimer[L] * 1000);
                    JustDied[UnitToCamp[L]] = 0;
                }
            }

            if (GuessDragonId == 1)
            {

                if (CampState[0] == 6 && ((CampState[1] == 7 || CampState[1] == 0) && CampRespawnTime[1] <= Environment.TickCount + 1000))
                {
                    CampState[0] = 0;
                    State[0] = 0;
                }

                if (CampState[1] == 6 && ((CampState[0] == 7 || CampState[0] == 0) && CampRespawnTime[0] <= Environment.TickCount + 1000))
                {
                    CampState[1] = 0;
                    State[1] = 0;
                }

                if (CampState[0] == 1 || (CampState[0] == 7 && CampRespawnTime[0] > Environment.TickCount + 1000))
                {
                    if ((CampState[1] == 7 || CampState[1] == 0) && CampRespawnTime[1] <= Environment.TickCount + 1000)
                    {
                        State[1] = 6;
                        CampState[1] = 6;
                    }
                }

                if (CampState[1] == 1 || (CampState[1] == 7 && CampRespawnTime[1] > Environment.TickCount + 1000) || CampState[1] == 8)
                {
                    if ((CampState[0] == 7 || CampState[0] == 0) && CampRespawnTime[0] <= Environment.TickCount + 1000)
                    {
                        State[0] = 6;
                        CampState[0] = 6;
                    }
                }
            }


            if ((L >= 2 && L <= 5)) //Red and Blue
            {

                var visible = 0;

                foreach (Obj_AI_Minion minion in TrackingList.Where(x => x.IsVisible && x.Name.Contains(NameToCompare[UnitToCamp[L]]) && !x.IsDead))
                {
                    visible = 1;
                }


                CampState[L] = Math.Min(State[UnitToCamp[L] - 1], State[UnitToCamp[L] - 2]);
                LastChangeOnCampState[L] = Math.Max(LastChangeOnState[UnitToCamp[L]], Math.Max(LastChangeOnState[UnitToCamp[L] - 1], LastChangeOnState[UnitToCamp[L] - 2]));

                if ((State[UnitToCamp[L]] == 2 || State[UnitToCamp[L] - 1] == 2 || State[UnitToCamp[L] - 2] == 2))
                {
                    CampState[L] = 2;
                    LastChangeOnCampState[L] = Math.Max(LastChangeOnState[UnitToCamp[L]], Math.Max(LastChangeOnState[UnitToCamp[L] - 1], LastChangeOnState[UnitToCamp[L] - 2]));
                }

                if (visible == 1)
                {
                    if (State[UnitToCamp[L]] == 1 && (State[UnitToCamp[L] - 1] == 4 || State[UnitToCamp[L] - 1] == 7) && (State[UnitToCamp[L] - 2] == 4 || State[UnitToCamp[L] - 2] == 7))
                    {
                        CampState[L] = 0;
                    }


                    LastChangeOnCampState[L] = Math.Max(LastChangeOnState[UnitToCamp[L]], Math.Max(LastChangeOnState[UnitToCamp[L] - 1], LastChangeOnState[UnitToCamp[L] - 2]));

                    if (CampRespawnTime[L] > Environment.TickCount) CampRespawnTime[L] = (Environment.TickCount + CampRespawnTimer[L] * 1000);
                }

                if (CampState[L] == 7 && JustDied[UnitToCamp[L] - 1] == 1 && JustDied[UnitToCamp[L] - 2] == 1)
                {
                    CampRespawnTime[L] = (LastChangeOnCampState[L] + CampRespawnTimer[L] * 1000);
                    JustDied[UnitToCamp[L]] = 0;
                    JustDied[UnitToCamp[L] - 1] = 0;
                    JustDied[UnitToCamp[L] - 2] = 0;
                }
            }

            if ((L >= 8 && L <= 9)) //Razor
            {

                var visible = 0;

                foreach (Obj_AI_Minion minion in TrackingList.Where(x => x.IsVisible &&
                    (x.Name.Contains(NameToCompare[UnitToCamp[L] - 1]) || x.Name.Contains(NameToCompare[UnitToCamp[L] - 2]) || x.Name.Contains(NameToCompare[UnitToCamp[L] - 3]))
                    && !x.IsDead))
                {
                    visible = 1;
                }

                if (visible == 0)
                {
                    CampState[L] = State[UnitToCamp[L]];
                    LastChangeOnCampState[L] = Math.Max(LastChangeOnState[UnitToCamp[L]], Math.Max(LastChangeOnState[UnitToCamp[L] - 1], Math.Max(LastChangeOnState[UnitToCamp[L] - 2], LastChangeOnState[UnitToCamp[L] - 3])));
                }

                if ((State[UnitToCamp[L]] == 2 || State[UnitToCamp[L] - 1] == 2 || State[UnitToCamp[L] - 2] == 2 || State[UnitToCamp[L] - 3] == 2))
                {
                    CampState[L] = 2;
                    LastChangeOnCampState[L] = Math.Max(LastChangeOnState[UnitToCamp[L]], Math.Max(LastChangeOnState[UnitToCamp[L] - 1], Math.Max(LastChangeOnState[UnitToCamp[L] - 2], LastChangeOnState[UnitToCamp[L] - 3])));
                }

                if (visible == 1)
                {
                    CampState[L] = Math.Min(State[UnitToCamp[L]], Math.Min(State[UnitToCamp[L] - 1], Math.Min(State[UnitToCamp[L] - 2], State[UnitToCamp[L] - 3])));
                    LastChangeOnCampState[L] = Math.Max(LastChangeOnState[UnitToCamp[L]], Math.Max(LastChangeOnState[UnitToCamp[L] - 1], Math.Max(LastChangeOnState[UnitToCamp[L] - 2], LastChangeOnState[UnitToCamp[L] - 3])));

                    if (CampRespawnTime[L] > Environment.TickCount) CampRespawnTime[L] = (Environment.TickCount + CampRespawnTimer[L] * 1000);
                }

                if (CampState[L] == 7 && JustDied[UnitToCamp[L]] == 1)
                {
                    CampRespawnTime[L] = (LastChangeOnCampState[L] + CampRespawnTimer[L] * 1000);
                    JustDied[UnitToCamp[L]] = 0;
                    JustDied[UnitToCamp[L] - 1] = 0;
                    JustDied[UnitToCamp[L] - 2] = 0;
                    JustDied[UnitToCamp[L] - 3] = 0;
                }
            }

            if (L == 12 || L == 13) //Krug
            {
                var visible = 0;

                foreach (Obj_AI_Minion minion in TrackingList.Where(x => x.IsVisible &&
                    (x.Name.Contains(NameToCompare[UnitToCamp[L]]) || x.Name.Contains(NameToCompare[UnitToCamp[L] - 1]))
                    && !x.IsDead))
                {
                    visible = 1;
                }

                if (visible == 0)
                {
                    CampState[L] = Math.Min(State[UnitToCamp[L]], State[UnitToCamp[L] - 1]);
                    LastChangeOnCampState[L] = Math.Max(LastChangeOnState[UnitToCamp[L]], LastChangeOnState[UnitToCamp[L] - 1]);
                }

                if (State[UnitToCamp[L]] == 2 || State[UnitToCamp[L] - 1] == 2)
                {
                    CampState[L] = 2;
                    LastChangeOnCampState[L] = Math.Max(LastChangeOnState[UnitToCamp[L]], LastChangeOnState[UnitToCamp[L] - 1]);
                }

                if (visible == 1)
                {
                    CampState[L] = Math.Min(State[UnitToCamp[L]], State[UnitToCamp[L] - 1]);
                    LastChangeOnCampState[L] = Math.Max(LastChangeOnState[UnitToCamp[L]], LastChangeOnState[UnitToCamp[L] - 1]);

                    if (CampRespawnTime[L] > Environment.TickCount) CampRespawnTime[L] = (Environment.TickCount + CampRespawnTimer[L] * 1000);
                }

                if (CampState[L] == 7 && JustDied[UnitToCamp[L]] == 1 && JustDied[UnitToCamp[L] - 1] == 1)
                {
                    CampRespawnTime[L] = (LastChangeOnCampState[L] + CampRespawnTimer[L] * 1000);
                    JustDied[UnitToCamp[L]] = 0;
                    JustDied[UnitToCamp[L] - 1] = 0;
                }
            }

            if (L == 14 || L == 15) //Wolf
            {
                var visible = 0;

                foreach (Obj_AI_Minion minion in TrackingList.Where(x => x.IsVisible &&
                    (x.Name.Contains(NameToCompare[UnitToCamp[L]]) || x.Name.Contains(NameToCompare[UnitToCamp[L] - 1]) || x.Name.Contains(NameToCompare[UnitToCamp[L] - 2]))
                    && !x.IsDead))
                {
                    visible = 1;
                }

                if (visible == 0)
                {
                    CampState[L] = Math.Min(State[UnitToCamp[L]], Math.Min(State[UnitToCamp[L] - 1], State[UnitToCamp[L] - 2]));
                    LastChangeOnCampState[L] = Math.Max(LastChangeOnState[UnitToCamp[L]], Math.Max(LastChangeOnState[UnitToCamp[L] - 1], LastChangeOnState[UnitToCamp[L] - 2]));
                }

                if ((State[UnitToCamp[L]] == 2 || State[UnitToCamp[L] - 1] == 2 || State[UnitToCamp[L] - 2] == 2))
                {
                    CampState[L] = 2;
                    LastChangeOnCampState[L] = Math.Max(LastChangeOnState[UnitToCamp[L]], Math.Max(LastChangeOnState[UnitToCamp[L] - 1], LastChangeOnState[UnitToCamp[L] - 2]));
                }

                if (visible == 1)
                {
                    CampState[L] = Math.Min(State[UnitToCamp[L]], Math.Min(State[UnitToCamp[L] - 1], State[UnitToCamp[L] - 2]));
                    LastChangeOnCampState[L] = Math.Max(LastChangeOnState[UnitToCamp[L]], Math.Max(LastChangeOnState[UnitToCamp[L] - 1], LastChangeOnState[UnitToCamp[L] - 2]));

                    if (CampRespawnTime[L] > Environment.TickCount) CampRespawnTime[L] = (Environment.TickCount + CampRespawnTimer[L] * 1000);
                }

                if (CampState[L] == 7 && JustDied[UnitToCamp[L]] == 1 && JustDied[UnitToCamp[L] - 1] == 1 && JustDied[UnitToCamp[L] - 2] == 1)
                {
                    CampRespawnTime[L] = (LastChangeOnCampState[L] + CampRespawnTimer[L] * 1000);
                    JustDied[UnitToCamp[L]] = 0;
                    JustDied[UnitToCamp[L] - 1] = 0;
                    JustDied[UnitToCamp[L] - 2] = 0;
                }
            }



            #endregion

            #region Play Dragon/Baron Sound

            if (CampState[0] != 2)
            {
                BufferDragonSound = 0;
                PlayingDragonSound = 0;
            }

            if (BufferDragonSound > 0 && (Environment.TickCount - LastPlayedDragonSound > 500) && CampState[0] == 2)
            {
                LastPlayedDragonSound = Environment.TickCount;
                PlaySound(_sound);
                BufferDragonSound -= 1;
                if (BufferDragonSound == 0) PlayingDragonSound = 0;
            }

            if ((_menu.Item("soundfow").GetValue<bool>() && OnFow[1] == 0) || !_menu.Item("soundfow").GetValue<bool>())
            {
                if ((_menu.Item("soundscreen").GetValue<bool>() && OnScreen[1] == 0) || !_menu.Item("soundscreen").GetValue<bool>())
                {
                    if ((CampState[1] == 2 && _menu.Item("baronsound").GetValue<bool>()) && (Environment.TickCount - BaronSoundDelay >= (_menu.Item("sounddelay").GetValue<Slider>().Value * 1000)))
                    {
                        for (int i = 1; i <= _menu.Item("baronsoundtimes").GetValue<Slider>().Value; i++)
                        {
                            if (i == 1 && (LastPlayedBaronSound == 0 || Environment.TickCount - LastPlayedBaronSound >= (_menu.Item("sounddelay").GetValue<Slider>().Value * 1000)) && Environment.TickCount - LastChangeOnCampState[1] < 500)
                            {
                                LastPlayedBaronSound = Environment.TickCount;
                                LastPlayedBaronSound2 = Environment.TickCount;
                                PlaySound(_sound);
                            }
                            else if (i > 1 && Environment.TickCount - LastPlayedBaronSound > 550 * (i - 1) && Environment.TickCount - LastPlayedBaronSound < 600 * (i - 1) && Environment.TickCount - LastPlayedBaronSound2 > 500)
                            {
                                LastPlayedBaronSound += (50 * (i - 1));

                                LastPlayedBaronSound2 = Environment.TickCount;


                                if (i == _menu.Item("baronsoundtimes").GetValue<Slider>().Value)
                                {
                                    BaronSoundDelay = Environment.TickCount;
                                }
                                PlaySound(_sound);
                            }
                        }
                    }
                }
            }

            #endregion

            #region Static Menu Update

            if (L == 0)
            {
                if (_menu.Item("soundvolume").GetValue<StringList>().SelectedIndex.Equals(0))
                {
                    _sound = Danger10;
                }
                else if (_menu.Item("soundvolume").GetValue<StringList>().SelectedIndex.Equals(1))
                {
                    _sound = Danger25;
                }
                else if (_menu.Item("soundvolume").GetValue<StringList>().SelectedIndex.Equals(2))
                {
                    _sound = Danger50;
                }
                else if (_menu.Item("soundvolume").GetValue<StringList>().SelectedIndex.Equals(3))
                {
                    _sound = Danger75;
                }
                else if (_menu.Item("soundvolume").GetValue<StringList>().SelectedIndex.Equals(4))
                {
                    _sound = Danger;
                }

                timeronmap = _menu.Item("timeronmap").GetValue<bool>();
                timeronminimap = _menu.Item("timeronminimap").GetValue<bool>();
                circleradius = _menu.Item("circleradius").GetValue<Slider>().Value;
                colorattacking = _menu.Item("colorattacking").GetValue<Color>();
                colortracked = _menu.Item("colortracked").GetValue<Color>();
                colordisengaged = _menu.Item("colordisengaged").GetValue<Color>();
                colordead = _menu.Item("colordead").GetValue<Color>();
                colorguessed = _menu.Item("colorguessed").GetValue<Color>();
                circlewidth = _menu.Item("circlewidth").GetValue<Slider>().Value;
                trackonminimap = _menu.Item("TrackonMinimap").GetValue<bool>();
            }

            #endregion

            #region Guess Blue/Red NetworkID

            if (GuessNetworkId1 == 1 && NetworkId[4] != 0 && NetworkId[3] != 0 && NetworkId[2] != 0)
            {
                Seed1 = (NetworkId[3] - NetworkId[4]);
                Seed2 = (NetworkId[2] - NetworkId[3]);

                //Console.WriteLine("Seed1:" + Seed1 + "  Seed2:" + Seed2);

                int id = 0;

                for (int c = 0; c <= 31; c++)
                {
                    int order = CreateOrder[c];

                    if (c == 2)
                    {
                        id += Seed1;
                        id += Seed2;
                    }
                    else
                    {
                        if (SeedOrder[c] == 1) id += Seed1;
                        else id += Seed2;
                    }

                    IdOrder[order] = id;
                }


                for (int j = 5; j <= 7; j++)
                {
                    if (j == 4 || IdOrder[j] == 0) continue;

                    if (NetworkId[j] == 0)
                    {
                        if (IdOrder[j] < IdOrder[4])
                        {
                            NetworkId[j] = NetworkId[4] - ((IdOrder[4] - IdOrder[j]));
                            State[j] = 5;
                            LastChangeOnState[j] = Environment.TickCount;
                        }
                        else if (IdOrder[j] > IdOrder[4])
                        {
                            NetworkId[j] = NetworkId[4] + ((IdOrder[j] - IdOrder[4]));
                            State[j] = 5;
                            LastChangeOnState[j] = Environment.TickCount;
                        }
                        //Console.WriteLine("NetworkID[" + j + "]:" + NetworkID[j] + " and Name: " + NameToCompare[j]);
                    }
                }
                GuessNetworkId1 = 0;
            }
            else if (GuessNetworkId1 == 1 && NetworkId[7] != 0 && NetworkId[6] != 0 && NetworkId[5] != 0 && NetworkId[7] < NetworkId[6])
            {
                Seed1 = (NetworkId[6] - NetworkId[7]);
                Seed2 = (NetworkId[5] - NetworkId[6]);

                //Console.WriteLine("Seed1:" + Seed1 + "  Seed2:" + Seed2);

                int id = 0;

                for (int c = 0; c <= 31; c++)
                {
                    int order = CreateOrder[c];

                    if (c == 2)
                    {
                        id += Seed1;
                        id += Seed2;
                    }
                    else
                    {
                        if (SeedOrder[c] == 1) id += Seed1;
                        else id += Seed2;
                    }

                    IdOrder[order] = id;
                }

                for (int j = 2; j <= 4; j++)
                {
                    if (j == 7 || IdOrder[j] == 0) continue;

                    if (NetworkId[j] == 0)
                    {
                        if (IdOrder[j] < IdOrder[7])
                        {
                            NetworkId[j] = NetworkId[7] - ((IdOrder[7] - IdOrder[j]));
                            State[j] = 5;
                            LastChangeOnState[j] = Environment.TickCount;
                        }
                        else if (IdOrder[j] > IdOrder[7])
                        {
                            NetworkId[j] = NetworkId[7] + ((IdOrder[j] - IdOrder[7]));
                            State[j] = 5;
                            LastChangeOnState[j] = Environment.TickCount;
                        }
                        //Console.WriteLine("NetworkID[" + j + "]:" + NetworkID[j] + " and Name: " + NameToCompare[j]);
                    }
                }
                GuessNetworkId1 = 0;
            }

            else if (GuessNetworkId2 == 1 && NetworkId[10] != 0 && NetworkId[9] != 0 && NetworkId[8] != 0 && NetworkId[10] < NetworkId[9])
            {
                Seed1 = (NetworkId[9] - NetworkId[10]);
                Seed2 = (NetworkId[8] - NetworkId[9]);

                //Console.WriteLine("Seed1:" + Seed1 + "  Seed2:" + Seed2);

                int id = 0;

                for (int c = 0; c <= 31; c++)
                {
                    int order = CreateOrder[c];

                    if (c == 2)
                    {
                        id += Seed1;
                        id += Seed2;
                    }
                    else
                    {
                        if (SeedOrder[c] == 1) id += Seed1;
                        else id += Seed2;
                    }

                    IdOrder[order] = id;
                }

                for (int j = 11; j <= 13; j++)
                {
                    if (j == 10 || IdOrder[j] == 0) continue;

                    if (NetworkId[j] == 0)
                    {
                        if (IdOrder[j] < IdOrder[10])
                        {
                            NetworkId[j] = NetworkId[10] - ((IdOrder[10] - IdOrder[j]));
                            State[j] = 5;
                            LastChangeOnState[j] = Environment.TickCount;
                        }
                        else if (IdOrder[j] > IdOrder[10])
                        {
                            NetworkId[j] = NetworkId[10] + ((IdOrder[j] - IdOrder[10]));
                            State[j] = 5;
                            LastChangeOnState[j] = Environment.TickCount;
                        }
                        //Console.WriteLine("NetworkID[" + j + "]:" + NetworkID[j] + " and Name: " + NameToCompare[j]);
                    }
                }
                GuessNetworkId2 = 0;
            }

            else if (GuessNetworkId2 == 1 && NetworkId[13] != 0 && NetworkId[12] != 0 && NetworkId[11] != 0 && NetworkId[13] < NetworkId[12])
            {
                Seed1 = (NetworkId[12] - NetworkId[13]);
                Seed2 = (NetworkId[11] - NetworkId[12]);

                //Console.WriteLine("Seed1:" + Seed1 + "  Seed2:" + Seed2);

                int id = 0;

                for (int c = 0; c <= 31; c++)
                {
                    int order = CreateOrder[c];

                    if (c == 2)
                    {
                        id += Seed1;
                        id += Seed2;
                    }
                    else
                    {
                        if (SeedOrder[c] == 1) id += Seed1;
                        else id += Seed2;
                    }

                    IdOrder[order] = id;
                }

                for (int j = 8; j <= 10; j++)
                {
                    if (j == 13 || IdOrder[j] == 0) continue;

                    if (NetworkId[j] == 0)
                    {
                        if (IdOrder[j] < IdOrder[13])
                        {
                            NetworkId[j] = NetworkId[13] - ((IdOrder[13] - IdOrder[j]));
                            State[j] = 5;
                            LastChangeOnState[j] = Environment.TickCount;
                        }
                        else if (IdOrder[j] > IdOrder[13])
                        {
                            NetworkId[j] = NetworkId[13] + ((IdOrder[j] - IdOrder[13]));
                            State[j] = 5;
                            LastChangeOnState[j] = Environment.TickCount;
                        }
                        //Console.WriteLine("NetworkID[" + j + "]:" + NetworkID[j] + " and Name: " + NameToCompare[j]);
                    }
                }
                GuessNetworkId2 = 0;
            }

            #endregion

            #region Dragon/Baron vision and OnScreen

            foreach (Obj_AI_Minion minion in TrackingList.Where(x => x.Name.Contains("Baron") || x.Name.Contains("Dragon")))
            {
                for (int i = 0; i <= 1; i++)
                {

                    if (!_menu.Item("dragon").GetValue<bool>() && i == 0) continue;
                    if (!_menu.Item("baron").GetValue<bool>() && i == 1) continue;


                    if (minion.Name.Contains(NameToCompare[i]))
                    {
                        if (CampPosition[i].IsOnScreen()) OnScreen[i] = 1;

                        else OnScreen[i] = 0;

                        if (minion.IsVisible)
                        {
                            OnFow[i] = 1;
                        }
                        else if (!minion.IsVisible && i < 2)
                        {
                            OnFow[i] = 0;
                            OnScreen[i] = 0;
                        }
                    }
                }
            }

            #endregion

            #region Get Dragon/Baron buff

            var enemy = HeroManager.Enemies.FirstOrDefault(x => x.IsValidTarget());

            if (L == 0)
            {
                var ally = HeroManager.Allies.FirstOrDefault(x => x.IsValidTarget(50000f, false));

                if (enemy != null)
                {
                    if (GetDragonStacks(enemy) > EnemyTeamStacks)
                    {
                        EnemyTeamStacks = EnemyTeamStacks + 1;

                        if (Environment.TickCount - EnemyFoWTime > 100 ||
                            (Environment.TickCount - EnemyFoWTime <= 100 && (State[0] != 4 && State[0] != 7)))
                        {
                            State[0] = 4;
                            LastChangeOnState[0] = Environment.TickCount;
                            LastChangeOnCampState[0] = Environment.TickCount;
                            CampRespawnTime[0] = (LastChangeOnCampState[0] + CampRespawnTimer[0]*1000);
                        }
                        //Console.WriteLine("Enemy Dragon Stacks: " + EnemyTeamStacks);
                    }
                    else if (GetNashorBuff(enemy) && !EnemyTeamNashor)
                    {
                        EnemyTeamNashor = true;

                        if (Environment.TickCount - EnemyFoWTime > 100 ||
                            (Environment.TickCount - EnemyFoWTime <= 100 && State[1] != 4 && State[1] != 7))
                        {
                            State[1] = 4;
                            LastChangeOnState[1] = Environment.TickCount;
                            LastChangeOnCampState[1] = Environment.TickCount;
                            CampRespawnTime[1] = (LastChangeOnCampState[1] + CampRespawnTimer[1]*1000);
                        }
                        //Console.WriteLine("Enemy Baron: " + EnemyTeamNashor);
                    }
                    else if (GetDragonStacks(enemy) < EnemyTeamStacks)
                    {
                        EnemyTeamStacks--;
                        //Console.WriteLine("Enemy Dragon Stacks: " + EnemyTeamStacks);
                    }
                    else if (!GetNashorBuff(enemy) && EnemyTeamNashor)
                    {
                        EnemyTeamNashor = false;
                        //Console.WriteLine("Enemy Baron: " + EnemyTeamNashor);
                    }
                }
                else
                {
                    EnemyFoWTime = Environment.TickCount;
                }

                if (ally != null)
                {
                    if (GetDragonStacks(ally) > AllyTeamStacks)
                    {
                        AllyTeamStacks = AllyTeamStacks + 1;

                        if (Environment.TickCount - AllyFoWTime > 100 ||
                            (Environment.TickCount - AllyFoWTime <= 100 && (State[0] != 4 && State[0] != 7)))
                        {
                            State[0] = 4;
                            LastChangeOnState[0] = Environment.TickCount;
                            LastChangeOnCampState[0] = Environment.TickCount;
                            CampRespawnTime[0] = (LastChangeOnCampState[0] + CampRespawnTimer[0]*1000);
                        }
                        //Console.WriteLine("Ally Dragon Stacks: " + AllyTeamStacks);
                    }
                    else if (GetNashorBuff(ally) && !AllyTeamNashor)
                    {
                        AllyTeamNashor = true;

                        if (Environment.TickCount - AllyFoWTime > 100 ||
                            (Environment.TickCount - AllyFoWTime <= 100 && State[1] != 4 && State[1] != 7))
                        {
                            State[1] = 4;
                            LastChangeOnState[1] = Environment.TickCount;
                            LastChangeOnCampState[1] = Environment.TickCount;
                            CampRespawnTime[1] = (LastChangeOnCampState[1] + CampRespawnTimer[1]*1000);
                        }
                        //Console.WriteLine("Ally Baron: " + AllyTeamNashor);
                    }
                    else if (GetDragonStacks(ally) < AllyTeamStacks)
                    {
                        AllyTeamStacks--;
                        //Console.WriteLine("Ally Dragon Stacks: " + AllyTeamStacks);
                    }
                    else if (!GetNashorBuff(ally) && AllyTeamNashor)
                    {
                        AllyTeamNashor = false;
                        //Console.WriteLine("Ally Baron: " + AllyTeamNashor);
                    }
                }
                else
                {
                    AllyFoWTime = Environment.TickCount;
                }

                if (ally != null && enemy != null && AllyTeamNashor == false && EnemyTeamNashor == false &&
                    State[1] == 4)
                {
                    State[1] = 1;
                }
            }

            #endregion  //Credits to Inferno

            #region Update States

            int t = 3000;

            if (L == 0)
            {
                if (Game.ClockTime - ClockTimeAdjust < 420f) t = 60000;
                else if (Game.ClockTime - ClockTimeAdjust < 820f) t = 40000;
                else t = 15000;
            }

            if (L == 1 && State[L] == 8 && CampRespawnTime[L] <= Environment.TickCount)
            {
                State[L] = 0;
            }

            if (State[L] == 2 && (Environment.TickCount - LastChangeOnState[L]) >= t && !(NameToCompare[L].Contains("Crab")))    //presumed dead
            {
                if (L <= 1)
                {
                    if (OnFow[L] == 0 && enemy == null)
                    {
                        State[L] = 4;
                        LastChangeOnState[L] = Environment.TickCount - 2000;
                    }
                }
                else
                {
                    State[L] = 4;
                    LastChangeOnState[L] = Environment.TickCount - 2000;
                }
            }
            else if (State[L] == 2 && (Environment.TickCount - LastChangeOnState[L]) >= 10000 && (NameToCompare[L].Contains("Crab")))
            {
                State[L] = 1;
                LastChangeOnState[L] = Environment.TickCount;
            }

            else if (State[L] == 3 && (Environment.TickCount - LastChangeOnState[L]) >= 2000)    //after desingaged wait 2 sec's
            {
                State[L] = 1;
                LastChangeOnState[L] = Environment.TickCount;
            }

            else if (State[L] == 4 && (Environment.TickCount - LastChangeOnState[L]) >= 5000)
            {
                State[L] = 7;
                JustDied[L] = 1;
            }

            else if (State[L] == 5 && (Environment.TickCount - LastChangeOnState[L]) >= 30000)
            {
                State[L] = 0;
            }


            #endregion

            #region Timers

            if (L >= 0 && L <= 15)
            {
                if (CampRespawnTime[L] > Environment.TickCount && CampState[L] == 7)
                {
                    var timespan = TimeSpan.FromSeconds((CampRespawnTime[L] - Environment.TickCount)/1000f);

                    bool format;

                    if (CampPosition[L].IsOnScreen() && _menu.Item("timeronmap").GetValue<bool>())
                    {
                        format = !_menu.Item("timeronmapformat").GetValue<StringList>().SelectedIndex.Equals(0);

                        TimerTextOnMap[L] = string.Format(format ? "{1}" : "{0}:{1:00}", (int) timespan.TotalMinutes,
                            format ? (int) timespan.TotalSeconds : timespan.Seconds);
                    }

                    
                    var pos2 = Drawing.WorldToMinimap(CampPosition[L]);

                    if (_menu.Item("timeronminimap").GetValue<bool>())
                    {
                        format = !_menu.Item("timeronminimapformat").GetValue<StringList>().SelectedIndex.Equals(0);

                        TimerTextOnMinimap[L] = string.Format(format ? "{1}" : "{0}:{1:00}",
                            (int) timespan.TotalMinutes,
                            format ? (int) timespan.TotalSeconds : timespan.Seconds);

                        var textrect = MapText.MeasureText(null, TimerTextOnMinimap[L], FontDrawFlags.Center);

                        TimerPosXOnMinimap[L] = (int) (pos2.X - textrect.Width/2f);

                        TimerPosYOnMinimap[L] = (int) (pos2.Y - textrect.Height/2f);
                    }
                    
                }
            }

            for (int i = 0; i <= 15; i++)
            {
                if (CampPosition[i].IsOnScreen() && _menu.Item("timeronmap").GetValue<bool>())
                {
                    var pos = Drawing.WorldToScreen(CampPosition[i]);

                    var textrect = MapText.MeasureText(null, TimerTextOnMap[i], FontDrawFlags.Center);

                    TimerPosXOnMap[i] = (int)(pos.X - textrect.Width / 2f);

                    TimerPosYOnMap[i] = (int)(pos.Y - textrect.Height / 2f);
                }
            }

            #endregion

            L++;
            if (L > 35) L = 0;
        }

        static int GetDragonStacks(Obj_AI_Hero hero)
        {
            var baff = hero.Buffs.FirstOrDefault(buff => buff.Name == "s5test_dragonslayerbuff");
            if (baff == null)
                return 0;
            else
                return baff.Count;
        }//Credits to Inferno

        static bool GetNashorBuff(Obj_AI_Hero hero)
        {
            return hero.Buffs.Any(buff => buff.Name == "exaltedwithbaronnashor");
        } //Credits to Inferno

        private static void OnProcessPacket(GamePacketEventArgs args)
        {
            short header = BitConverter.ToInt16(args.PacketData, 0);

            #region Update States

            for (int i = 0; i <= 25; i++)
            {
                if (NetworkId[i] == 0) continue;

                if (BitConverter.ToInt32(args.PacketData, 2) == NetworkId[i])
                {
                    if (header == HeaderSkill)
                    {
                        //Console.WriteLine(NameToCompare[i] + " is Attacking");   //"using skill" or crab dead

                        if (NameToCompare[i].Contains("Crab"))
                        {
                            State[i] = 4;
                        }
                        else
                        {
                            if (NameToCompare[i].Contains("Dragon") && State[i] != 2)
                            {
                                if (BufferDragonSound == 0 && PlayingDragonSound == 0 && ((_menu.Item("soundfow").GetValue<bool>() && OnFow[0] == 0) || !_menu.Item("soundfow").GetValue<bool>()))
                                {
                                    if ((_menu.Item("soundscreen").GetValue<bool>() && OnScreen[0] == 0) || !_menu.Item("soundscreen").GetValue<bool>())
                                    {
                                        if ((_menu.Item("dragonsound").GetValue<bool>()))
                                        {
                                            BufferDragonSound = _menu.Item("dragonsoundtimes").GetValue<Slider>().Value;
                                            PlayingDragonSound = 1;
                                            CampState[0] = 2;
                                        }
                                    }
                                }
                            }
                            State[i] = 2;
                        }
                        LastChangeOnState[i] = Environment.TickCount;
                    }

                    else if (header == HeaderMeleeAttack)
                    {
                        //Console.WriteLine(NameToCompare[i] + " is Attacking");

                        State[i] = 2;
                        LastChangeOnState[i] = Environment.TickCount;
                    }

                    else if (header == HeaderRangedAttack)
                    {
                        //Console.WriteLine(NameToCompare[i] + " is Attacking (ranged)");

                        State[i] = 2;
                        LastChangeOnState[i] = Environment.TickCount;
                    }

                    else if (header == HeaderDisengaged)
                    {
                        //Console.WriteLine(NameToCompare[i] + " is Disengaged");
                        if (NameToCompare[i].Contains("Crab"))
                        {
                            if (State[i] == 0) State[i] = 5;
                            else State[i] = 2;
                        }
                        else
                        {
                            if (State[i] == 0) State[i] = 5;
                            else State[i] = 3;
                        }
                        LastChangeOnState[i] = Environment.TickCount;
                    }
                }
            }

            #endregion

            #region Guess Dragon/Baron NetworkID

            if (header == HeaderSkill &&
                BitConverter.ToInt32(args.PacketData, 2) != NetworkId[0] &&
                BitConverter.ToInt32(args.PacketData, 2) != NetworkId[1] &&
                BitConverter.ToInt32(args.PacketData, 2) > BiggestNetworkId &&
                BitConverter.ToString(args.PacketData, 0).Length == 47 &&
                GuessDragonId == 1
                )
            {
                bool aiBaseTest = false;
                foreach (Obj_AI_Base obj in ObjectManager.Get<Obj_AI_Base>().Where(x => x.NetworkId == BitConverter.ToInt32(args.PacketData, 2)))
                {
                    if (!obj.IsAlly)
                    {
                        if (!obj.Name.Contains("SRU_Dragon") && !obj.Name.Contains("SRU_Baron"))//&& !obj.Name.Contains("TestCube"))
                        {
                            Game.PrintChat("<font color=\"#FF0000\"> God Jungle Tracker (debug): Tell AlphaGod he forgot to consider: " + obj.Name + " - " + obj.SkinName + " - " + obj.BaseSkinName + " - Guess Dragon NetWorkID disabled</font>");
                            GuessDragonId = 0;
                            aiBaseTest = true;
                        }
                    }
                }

                if (!aiBaseTest)
                {
                    if (State[1] == 1 || (State[1] == 7 && CampRespawnTime[1] > Environment.TickCount + 1000) || State[1] == 8)
                    {
                        if (BufferDragonSound == 0 && PlayingDragonSound == 0 && ((_menu.Item("soundfow").GetValue<bool>() && OnFow[0] == 0) || !_menu.Item("soundfow").GetValue<bool>()))
                        {
                            if ((_menu.Item("soundscreen").GetValue<bool>() && OnScreen[0] == 0) || !_menu.Item("soundscreen").GetValue<bool>())
                            {
                                if ((_menu.Item("dragonsound").GetValue<bool>()))
                                {
                                    BufferDragonSound = _menu.Item("dragonsoundtimes").GetValue<Slider>().Value;
                                    PlayingDragonSound = 1;
                                    CampState[0] = 2;
                                }
                            }
                        }

                        State[0] = 2;
                        LastChangeOnState[0] = Environment.TickCount;
                        NetworkId[0] = BitConverter.ToInt32(args.PacketData, 2);
                    }
                    else if (State[0] == 1 || (State[0] == 7 && CampRespawnTime[0] > Environment.TickCount + 1000))
                    {
                        State[1] = 2;
                        LastChangeOnState[1] = Environment.TickCount;
                        NetworkId[1] = BitConverter.ToInt32(args.PacketData, 2);
                    }
                }
            }

            #endregion

            #region Gromp Created

            if (header == HeaderCreateGromp)  //Gromp Created
            {
                if (BitConverter.ToString(args.PacketData, 0).Length == 302 || BitConverter.ToString(args.PacketData, 0).Length == 284)
                {
                    NetworkId[14] = BitConverter.ToInt32(args.PacketData, 2);
                    State[14] = 1;
                    LastChangeOnState[14] = Environment.TickCount;

                    if (Game.ClockTime - 111f < 90 && ClockTimeAdjust <= 0)
                    {
                        ClockTimeAdjust = Game.ClockTime - 111f;
                        State[0] = 0;
                        CampRespawnTime[0] = Environment.TickCount + 39000;
                        CampState[0] = 0;
                        BiggestNetworkId = BitConverter.ToInt32(args.PacketData, 2);
                    }
                }
                else if (BitConverter.ToString(args.PacketData, 0).Length == 311 || BitConverter.ToString(args.PacketData, 0).Length == 293)
                {
                    NetworkId[15] = BitConverter.ToInt32(args.PacketData, 2);
                    State[15] = 1;
                    LastChangeOnState[15] = Environment.TickCount;
                }
            }
            #endregion
        }

        public static void Drawing_OnDraw(EventArgs args)
        {
            Color cor = Color.FromArgb(255, 0, 255, 0);

            int c = 0;

            /*
            CampState == 0 Not Tracking
            CampState == 1 Tracking/Iddle
            CampState == 2 Attacking
            CampState == 3 Disengaged
            CampState == 4 Presumed Dead
            CampState == 5 Guessed on fow
            CampState == 6 Guessed on fow
            CampState == 7 on timer to respawn
            */

            for (int i = 0; i <= 11; i++)
            {

                if (!_menu.Item("drawtracklist").GetValue<bool>()) break;

                if (CampState[i] > 0 && CampState[i] != 7)
                {
                    int x = _menu.Item("posX").GetValue<Slider>().Value;
                    int y = _menu.Item("posY").GetValue<Slider>().Value - c * 30;

                    c += 1;

                    if (CampName[i].Contains("Dragon"))
                    {
                        Drawing.DrawText(x, y, Color.FromArgb(255, 255, 165, 15), CampName[i]);
                    }
                    else if (CampName[i].Contains("Baron"))
                    {
                        Drawing.DrawText(x, y, Color.FromArgb(255, 153, 50, 204), CampName[i]);
                    }
                    else if (CampName[i].Contains("Blue"))
                    {
                        Drawing.DrawText(x, y, Color.FromArgb(255, 0, 255, 255), CampName[i]);
                    }
                    else if (CampName[i].Contains("Red"))
                    {
                        Drawing.DrawText(x, y, Color.FromArgb(255, 255, 0, 0), CampName[i]);
                    }
                    else if (CampName[i].Contains("Crab"))
                    {
                        Drawing.DrawText(x, y, Color.FromArgb(255, 152, 251, 152), CampName[i]);
                    }
                    else if (CampName[i].Contains("Raptor"))
                    {
                        Drawing.DrawText(x, y, Color.FromArgb(255, 255, 127, 80), CampName[i]);
                    }
                    else if (CampName[i].Contains("Gromp"))
                    {
                        Drawing.DrawText(x, y, Color.FromArgb(255, 0, 139, 45), CampName[i]);
                    }
                    if (CampState[i] == 1)
                    {
                        cor = colortracked;
                    }
                    else if (CampState[i] == 2)
                    {
                        cor = colorattacking;
                    }
                    else if (CampState[i] == 3)
                    {
                        cor = colordisengaged;
                    }
                    else if (CampState[i] == 4)
                    {
                        cor = colordead;
                    }
                    else if (CampState[i] == 5 || CampState[i] == 6)
                    {
                        cor = colorguessed;
                    }


                    Drawing.DrawLine(
                        new Vector2(x - 4.5f, y - 5),
                        new Vector2(x + 70, y - 5), 3, cor);

                    Drawing.DrawLine(
                        new Vector2(x + 70, y - 4.5f),
                        new Vector2(x + 70, y + 21), 3, cor);

                    Drawing.DrawLine(
                        new Vector2(x + 70, y + 21),
                        new Vector2(x - 3, y + 21), 3, cor);

                    Drawing.DrawLine(
                        new Vector2(x - 5, y + 21),
                        new Vector2(x - 5, y - 4.5f), 3, cor);
                }
            }

            if (c == 0 && _menu.Item("drawtracklist").GetValue<bool>() )
            {
                cor = Color.FromArgb(255, 255, 255, 255);

                int x = _menu.Item("posX").GetValue<Slider>().Value;
                int y = _menu.Item("posY").GetValue<Slider>().Value - c * 30;

                Drawing.DrawText(x, y, cor, "TrackList");

                Drawing.DrawLine(
                        new Vector2(x - 4.5f, y - 5),
                        new Vector2(x + 70, y - 5), 3, cor);

                Drawing.DrawLine(
                    new Vector2(x + 70, y - 4.5f),
                    new Vector2(x + 70, y + 21), 3, cor);

                Drawing.DrawLine(
                    new Vector2(x + 70, y + 21),
                    new Vector2(x - 3, y + 21), 3, cor);

                Drawing.DrawLine(
                    new Vector2(x - 5, y + 21),
                    new Vector2(x - 5, y - 4.5f), 3, cor);

            }
        }

        public static void Drawing_OnEndScene(EventArgs args)
        {
            if (Drawing.Direct3DDevice == null || Drawing.Direct3DDevice.IsDisposed)
            {
                return;
            }

            for (int i = 0; i <= 15; i++)
            {
                #region Timers

                if (CampRespawnTime[i] > Environment.TickCount && CampState[i] == 7)
                {
                    if (CampPosition[i].IsOnScreen() && timeronmap)
                    {
                        MapText.DrawText(null, TimerTextOnMap[i], TimerPosXOnMap[i],TimerPosYOnMap[i], white);
                    }

                    if (timeronminimap)
                    {
                        MinimapText.DrawText(null, TimerTextOnMinimap[i], TimerPosXOnMinimap[i], TimerPosYOnMinimap[i], white);
                    }
                }

                #endregion

                #region Minimap Circles

                if (i > 11) continue;

                if (!trackonminimap) continue;

                if ((i == 0 || i == 6 || i == 7) && CampState[i] == 2 && (Environment.TickCount - LastChangeOnCampState[i]) < 60000)
                {
                    Utility.DrawCircle(CampPosition[i], circleradius, colorattacking, circlewidth + 1, 30, true);
                }
                else if (i != 0 && CampState[i] == 2 && (Environment.TickCount - LastChangeOnCampState[i]) < 10000)
                {
                    Utility.DrawCircle(CampPosition[i], circleradius, colorattacking, circlewidth + 1, 30, true);
                }
                else if (CampState[i] == 1)
                {
                    Utility.DrawCircle(CampPosition[i], circleradius, colortracked, circlewidth, 30, true);
                }
                else if (CampState[i] == 3)
                {
                    Utility.DrawCircle(CampPosition[i], circleradius, colordisengaged, circlewidth + 1, 30, true);
                }
                else if (CampState[i] == 4)
                {
                    Utility.DrawCircle(CampPosition[i], circleradius, colordead, circlewidth, 30, true);
                }
                else if (CampState[i] == 5 || CampState[i] == 6)
                {
                    Utility.DrawCircle(CampPosition[i], circleradius, colorguessed, circlewidth, 30, true);
                }
                #endregion
            }
        }

        static void LoadMenu()
        {
            //Start Menu
            _menu = new Menu("God Jungle Tracker", "God Jungle Tracker", true);

            //Camp to Track Menu
            _menu.AddSubMenu(new Menu("Camp to Track", "Camp to Track"));
            _menu.SubMenu("Camp to Track").AddItem(new MenuItem("baron", "Baron").SetValue(true));
            _menu.SubMenu("Camp to Track").AddItem(new MenuItem("dragon", "Dragon").SetValue(true));
            _menu.SubMenu("Camp to Track").AddItem(new MenuItem("blue", "Blue").SetValue(true));
            _menu.SubMenu("Camp to Track").AddItem(new MenuItem("red", "Red").SetValue(true));
            _menu.SubMenu("Camp to Track").AddItem(new MenuItem("crab", "Crab").SetValue(true));
            _menu.SubMenu("Camp to Track").AddItem(new MenuItem("raptor", "Raptor").SetValue(true));
            _menu.SubMenu("Camp to Track").AddItem(new MenuItem("gromp", "Gromp").SetValue(true));

            //Track List Menu
            _menu.AddSubMenu(new Menu("Track List", "Track List"));
            _menu.SubMenu("Track List").AddItem(new MenuItem("drawtracklist", "Draw Track List").SetValue(false));
            _menu.SubMenu("Track List").AddItem(new MenuItem("posX", "Track List Pos X").SetValue(new Slider(Drawing.Width / 2, 0, Drawing.Width)));
            _menu.SubMenu("Track List").AddItem(new MenuItem("posY", "Track List Pos Y").SetValue(new Slider(Drawing.Height / 2, 0, Drawing.Height)));

            //Play Danger Sound
            _menu.AddSubMenu(new Menu("Play Danger Sound", "Play Danger Sound"));
            _menu.SubMenu("Play Danger Sound").AddItem(new MenuItem("dragonsound", "On Dragon First Attack").SetValue(true));
            _menu.SubMenu("Play Danger Sound").AddItem(new MenuItem("dragonsoundtimes", "Play Sound X Times").SetValue(new Slider(2, 1, 4)));
            _menu.SubMenu("Play Danger Sound").AddItem(new MenuItem("baronsound", "On Baron Attack").SetValue(true));
            _menu.SubMenu("Play Danger Sound").AddItem(new MenuItem("baronsoundtimes", "Play Sound X Times").SetValue(new Slider(2, 1, 4)));
            _menu.SubMenu("Play Danger Sound").AddItem(new MenuItem("sounddelay", "Baron Sound Delay (s)").SetValue(new Slider(20, 1, 60)));
            _menu.SubMenu("Play Danger Sound").AddItem(new MenuItem("soundfow", "Only On Fog of War").SetValue(false));
            _menu.SubMenu("Play Danger Sound").AddItem(new MenuItem("soundscreen", "Only If Camp Not On Screen").SetValue(true));
            String [] volume = {"10%","25%","50%","75%","100%"};
            _menu.SubMenu("Play Danger Sound").AddItem(new MenuItem("soundvolume", "Sound Volume").SetValue(new StringList(volume, 2)));

            //Timers
            _menu.AddSubMenu(new Menu("Timers", "Timers"));
            String[] format = { "mm:ss", "ss" };
            var timers = _menu.SubMenu("Timers");
            timers.SubMenu("On Map").AddItem(new MenuItem("timeronmapformat", "Format ").SetValue(new StringList(format)));
            timers.SubMenu("On Map").AddItem(new MenuItem("timerfontmap", "Font Size").SetValue(new Slider(20, 3, 30)));
            timers.SubMenu("On Map").AddItem(new MenuItem("timeronmap", "Enabled").SetValue(true));
            timers.SubMenu("On Minimap").AddItem(new MenuItem("timeronminimapformat", "Format ").SetValue(new StringList(format)));
            timers.SubMenu("On Minimap").AddItem(new MenuItem("timerfontminimap", "Font Height").SetValue(new Slider(13, 3, 30)));
            timers.SubMenu("On Minimap").AddItem(new MenuItem("timeronminimap", "Enabled").SetValue(true));
            
            //Drawing
            _menu.AddSubMenu(new Menu("Drawing", "Drawing"));
            var draw = _menu.SubMenu("Drawing");
            draw.SubMenu("Color").AddItem(new MenuItem("colortracked", "Camp is Idle - Tracked").SetValue(Color.FromArgb(255, 0, 255, 0)));
            draw.SubMenu("Color").AddItem(new MenuItem("colorguessed", "Camp is Idle - Guessed").SetValue(Color.FromArgb(255, 0, 255, 255)));
            draw.SubMenu("Color").AddItem(new MenuItem("colorattacking", "Camp is Attacking").SetValue(Color.FromArgb(255, 255, 0, 0)));
            draw.SubMenu("Color").AddItem(new MenuItem("colordisengaged", "Camp is Disengaged").SetValue(Color.FromArgb(255, 255, 210, 0)));
            draw.SubMenu("Color").AddItem(new MenuItem("colordead", "Camp is Dead").SetValue(Color.FromArgb(255, 200, 200, 200)));
            _menu.SubMenu("Drawing").AddItem(new MenuItem("circleradius", "Circle Radius").SetValue(new Slider(300, 1, 500)));
            _menu.SubMenu("Drawing").AddItem(new MenuItem("circlewidth", "Circle Width").SetValue(new Slider(1, 1, 4)));
            _menu.SubMenu("Drawing").AddItem(new MenuItem("TrackonMinimap", "Draw on Minimap").SetValue(true));

            _menu.AddToMainMenu();
        }
    }
}
