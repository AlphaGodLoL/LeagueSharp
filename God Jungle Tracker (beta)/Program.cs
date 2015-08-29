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
using GodJungleTracker.Classes;

namespace GodJungleTracker
{
    class Program
    {
        static Menu _menu;

        #region Definitions

        /*
        camp.State == 0 Not Tracking
        camp.State == 1 Attacking
        camp.State == 2 Disengaged
        camp.State == 3 Tracking/Iddle
        camp.State == 4 Presumed Dead
        camp.State == 5 Guessed on fow with networkId
        camp.State == 7 dead on timer to respawn
        */

        public static Font MinimapText;

        public static Font MapText;

        public static Utility.Map.MapType MapType { get; set; }

        public static SoundPlayer Danger;
        public static SoundPlayer Danger10;
        public static SoundPlayer Danger25;
        public static SoundPlayer Danger50;
        public static SoundPlayer Danger75;
        public static SoundPlayer _sound = Danger;

        public static Jungle.Camp DragonCamp;
        public static Jungle.Camp BaronCamp;

        public static List<int> OnAttackList;
        public static List<int> MissileHitList;
        public static List<int[]> OnCreateGrompList;
        public static List<int[]> OnCreateCampIconList;
        public static List<int[]> PossibleBaronList;
        public static List<int> PossibleDragonList;

        public static int UpdateTick;
        public static int PossibleDragonTimer;
        public static int EnemyTeamStacks;
        public static int AllyTeamStacks;
        public static bool EnemyTeamNashor;
        public static bool AllyTeamNashor;
        public static int GuessNetworkId1 = 1;
        public static int GuessNetworkId2 = 1;
        public static int GuessDragonId = 1;
        public static int EnemyFoWTime;
        public static int AllyFoWTime;
        public static int LastPlayedDragonSound;
        public static int LastPlayedBaronSound;
        public static int DragonSoundDelay;
        public static int BaronSoundDelay;
        public static int Seed1 = 3;
        public static int Seed2 = 2;
        public static float ClockTimeAdjust;
        public static int BiggestNetworkId;
        public static int BufferDragonSound;
        public static int PlayingDragonSound;
        public static int BufferBaronSound;
        public static int PlayingBaronSound;

        public static bool timeronmap;
        public static bool timeronminimap;
        public static int circleradius;
        public static Color colorattacking;
        public static Color colortracked;
        public static Color colordisengaged;
        public static Color colordead;
        public static Color colorguessed;
        public static int circlewidth;

        public static ColorBGRA white;

        public static string GameVersion = Game.Version.Substring(0, 4);

        public static int[] HeroNetworkId = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public static string[] BlockHeroes = { "Caitlyn", "Fizz", "Jinx", "Nidalee", "MonkeyKing", "Zed", "Zyra", "Kalista", "Yasuo" };

        public static int[] SeedOrder = { 0, 1, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0, 0, 1, 0, 0, 1, 0 };

        public static int[] CreateOrder = { 14, 15, 10, 9, 8, 13, 12, 11, 4, 3, 2, 7, 6, 5, 23, 22, 21, 20, 29, 28, 27, 26, 19, 18, 17, 16, 35, 34, 33, 32, 31, 30 };

        public static int[] IdOrder = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 10, 0, 0, 0, 2, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        #endregion

        static void Main()
        {
            Console.WriteLine("[GJT] - Start");
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        public static void OnGameLoad(EventArgs args)
        {
            Console.WriteLine("[GJT] - Started OnGameLoad");
            //if (Game.MapId.ToString() != "SummonersRift")
            //{
            //    return;
            //}

            GameVersion = Game.Version.Substring(0, 4);

            Console.WriteLine("[GJT] - Started Loading Menu");
            LoadMenu();

            Console.WriteLine("[GJT] - Started Set Defin");
            #region Set Defin

            Danger = new SoundPlayer(Properties.Resources.danger);
            Danger10 = new SoundPlayer(Properties.Resources.danger10);
            Danger25 = new SoundPlayer(Properties.Resources.danger25);
            Danger50 = new SoundPlayer(Properties.Resources.danger50);
            Danger75 = new SoundPlayer(Properties.Resources.danger75);
            _sound = Danger;

            OnAttackList = new List<int>();
            MissileHitList = new List<int>();
            OnCreateGrompList = new List<int[]>();
            OnCreateCampIconList = new List<int[]>();
            PossibleBaronList = new List<int[]>();
            PossibleDragonList = new List<int>();

            white = new ColorBGRA(255, 255, 255, 255);

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

            #endregion

            Console.WriteLine("[GJT] - Started Set Headers");
            #region Set Headers

            Packets.Attack.Header = _menu.Item("headerOnAttack" + GameVersion).GetValue<Slider>().Value;
            Packets.MissileHit.Header = _menu.Item("headerOnMissileHit" + GameVersion).GetValue<Slider>().Value;
            Packets.Disengaged.Header = _menu.Item("headerOnDisengaged" + GameVersion).GetValue<Slider>().Value;
            Packets.MonsterSkill.Header = _menu.Item("headerOnMonsterSkill" + GameVersion).GetValue<Slider>().Value;
            Packets.CreateGromp.Header = _menu.Item("headerOnCreateGromp" + GameVersion).GetValue<Slider>().Value;
            Packets.CreateCampIcon.Header = _menu.Item("headerOnCreateCampIcon" + GameVersion).GetValue<Slider>().Value;

            #endregion

            Console.WriteLine("[GJT] - Started Set Dragon/Baron");
            #region Set Dragon/Baron Camp
            foreach (var camp in Jungle.Camps.Where(camp => camp.MapType.ToString() == "SummonersRift"))
            {
                if (camp.Name == "Dragon")
                {
                    DragonCamp = camp;
                }
                else if (camp.Name == "Baron")
                {
                    BaronCamp = camp;
                }
            }
            #endregion

            Console.WriteLine("[GJT] - Started Loading Minions");
            #region Load Minions

            foreach (Obj_AI_Minion minion in ObjectManager.Get<Obj_AI_Minion>().Where(x => x.Name.Contains("SRU_") || x.Name.Contains("Sru_")))
            {
                foreach (var camp in Jungle.Camps.Where(camp => camp.MapType.ToString() == Game.MapId.ToString()))
                {
                    foreach (var mob in camp.Mobs)
                    {
                        //Do Stuff for each mob in a camp

                        if (mob.Name.Contains(minion.Name) && !minion.IsDead && mob.NetworkId != minion.NetworkId)
                        {
                            mob.NetworkId = minion.NetworkId;
                            mob.State = 3;
                            mob.LastChangeOnState = Environment.TickCount;
                            mob.Unit = minion;

                            if (camp.Mobs.Count == 1)
                            {
                                camp.State = mob.State;
                                camp.LastChangeOnState = mob.LastChangeOnState;
                            }
                        }
                    }
                }
            }

            #endregion

            Console.WriteLine("[GJT] - Started Loading Static Menu");
            #region Load Static Menu

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

            #endregion

            Console.WriteLine("[GJT] - Started Loading Others");
            #region Load Others

            if (Game.ClockTime > 450f)
            {
                GuessNetworkId1 = 0;

                GuessNetworkId2 = 0;
            }

            int c = 0;
            foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                HeroNetworkId[c] = hero.NetworkId;
                c++;
                if (hero.NetworkId > BiggestNetworkId)
                {
                    BiggestNetworkId = hero.NetworkId;
                }

                if (!hero.IsAlly)
                {
                    for (int i = 0; i <= 8; i++)
                    {
                        if (hero.ChampionName.Contains(BlockHeroes[i]))
                        {
                            //Console.WriteLine("God Jungle Tracker: " + hero.ChampionName + " in enemy team so GuessDragonId is disabled ");
                            GuessDragonId = 0;
                        }
                    }
                }
            }

            #endregion

            if (_menu.Item("onendscene").GetValue<bool>())
            {
                Drawing.OnEndScene += Drawing_OnEndScene;
                Console.WriteLine("[GJT] - OnEndScene Loaded");
            }
            else
            {
                Console.WriteLine("[GJT] - OnEndScene Not Loaded");
            }
            if (_menu.Item("oncreate").GetValue<bool>())
            {
                GameObject.OnCreate += GameObjectOnCreate;
                Console.WriteLine("[GJT] - OnCreate Loaded");
            }
            else
            {
                Console.WriteLine("[GJT] - OnCreate Not Loaded");
            }
            if (_menu.Item("ondelete").GetValue<bool>())
            {
                GameObject.OnDelete += GameObjectOnDelete;
                Console.WriteLine("[GJT] - OnDelete Loaded");
            }
            else
            {
                Console.WriteLine("[GJT] - OnDelete Not Loaded");
            }
            if (_menu.Item("onupdate").GetValue<bool>())
            {
                Game.OnUpdate += OnGameUpdate;
                Console.WriteLine("[GJT] - OnUpdate Loaded");
            }
            else
            {
                Console.WriteLine("[GJT] - OnUpdate Not Loaded");
            }
            if (_menu.Item("onprocesspacket").GetValue<bool>())
            {
                Game.OnProcessPacket += OnProcessPacket;
                Console.WriteLine("[GJT] - OnProcessPacket Loaded");
            }
            else
            {
                Console.WriteLine("[GJT] - OnProcessPacket Not Loaded");
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
                catch (Exception)
                {
                    //ignored
                }
            }
        }

        public static void GameObjectOnCreate(GameObject sender, EventArgs args)
        {
            if (!(sender is Obj_AI_Minion) || sender.Team != GameObjectTeam.Neutral)
            {
                return;
            }

            var minion = (Obj_AI_Minion)sender;

            foreach (var camp in Jungle.Camps.Where(camp => camp.MapType.ToString() == Game.MapId.ToString()))
            {
                //Do Stuff for each camp

                foreach (var mob in camp.Mobs.Where(mob => mob.Name == minion.Name))
                {
                    //Do Stuff for each mob in a camp

                    mob.NetworkId = minion.NetworkId;
                    mob.LastChangeOnState = Environment.TickCount;
                    mob.Unit = minion;
                    if (!minion.IsDead)
                    {
                        mob.State = 3;
                        mob.JustDied = false;
                    }
                    else
                    {
                        mob.State = 7;
                        mob.JustDied = true;
                    }

                    if (camp.Mobs.Count == 1)
                    {
                        camp.State = mob.State;
                        camp.LastChangeOnState = mob.LastChangeOnState;
                    }

                    if (mob.Name.Contains("Baron") && PossibleBaronList.Count >= 1)
                    {
                        try
                        {
                            PossibleBaronList.Clear();
                        }
                        catch (Exception)
                        {
                            //ignored
                        }
                    }

                    if (camp.Name == "Gromp" && _menu.Item("headerOnCreateGromp" + GameVersion).GetValue<Slider>().Value == 0)
                    {
                        foreach (var item in OnCreateGrompList.Where(item => item[0] == mob.NetworkId))
                        {
                            _menu.Item("headerOnCreateGromp" + GameVersion).SetValue<Slider>(new Slider(item[1], 0, 400));
                            Packets.CreateGromp.Header = item[1];
                            break;
                        }
                    }
                    //Console.WriteLine("MobName: " + mob.Name + " Created - NetworkId: " + mob.NetworkId );
                }
            }
        }

        public static void GameObjectOnDelete(GameObject sender, EventArgs args)
        {
            if (!(sender is Obj_AI_Minion) || sender.Team != GameObjectTeam.Neutral)
            {
                return;
            }

            var minion = (Obj_AI_Minion)sender;

            foreach (var camp in Jungle.Camps.Where(camp => camp.MapType.ToString() == Game.MapId.ToString()))
            {
                //Do Stuff for each camp

                foreach (var mob in camp.Mobs.Where(mob => mob.Name == minion.Name))
                {
                    //Do Stuff for each mob in a camp

                    mob.LastChangeOnState = Environment.TickCount - 3000;
                    mob.Unit = null;
                    if (mob.State != 7)
                    {
                        mob.State = 7;
                        mob.JustDied = true;
                    }

                    if (camp.Mobs.Count == 1)
                    {
                        camp.State = mob.State;
                        camp.LastChangeOnState = mob.LastChangeOnState;
                    }
                }
            }
        }

        public static void OnGameUpdate(EventArgs args)
        {
            if (Environment.TickCount > UpdateTick + _menu.Item("updatetick").GetValue<Slider>().Value)
            {
                var enemy = HeroManager.Enemies.FirstOrDefault(x => x.IsValidTarget());
        
                foreach (var camp in Jungle.Camps.Where(camp => camp.MapType.ToString() == Game.MapId.ToString()))
                {
                    #region Update States

                    int mobCount = 0;

                    bool firstMob = true;

                    int visibleMobsCount = 0;

                    int rangedMobsCount = 0;

                    int deadRangedMobsCount = 0;

                    bool disengaged = false;

                    foreach (var mob in camp.Mobs)
                    {
                        //Do Stuff for each mob in a camp

                        try
                        {
                            if (mob.Unit != null && mob.Unit.IsVisible)
                            {
                                visibleMobsCount++;
                            }
                        }
                        catch (Exception)
                        {
                            //ignored
                        }
                    

                        if (mob.IsRanged)
                        {
                            rangedMobsCount++;

                            if (mob.JustDied)
                            {
                                deadRangedMobsCount++;
                            }
                        }

                        bool visible = false;

                        mobCount += 1;

                        int guessedTimetoDead = 3000;

                        if (camp.Name == "Dragon")
                        {
                            if (Game.ClockTime - ClockTimeAdjust < 420f) guessedTimetoDead = 60000;
                            else if (Game.ClockTime - ClockTimeAdjust < 820f) guessedTimetoDead = 40000;
                            else guessedTimetoDead = 15000;
                        }

                        if (camp.Name == "Baron")
                        {
                            guessedTimetoDead = 5000;
                        }

                        switch (mob.State)
                        {
                            case 1:
                                if ((Environment.TickCount - mob.LastChangeOnState) >= guessedTimetoDead && camp.Name != "Crab")
                                {
                                    if (camp.Name == "Dragon")
                                    {
                                        try
                                        {
                                            if (mob.Unit != null && !mob.Unit.IsVisible && enemy == null)
                                            {
                                                mob.State = 4;
                                                mob.LastChangeOnState = Environment.TickCount - 2000;
                                            }
                                        }
                                        catch (Exception)
                                        {
                                            //ignored
                                        }
                                    
                                    }
                                    else if (camp.Name == "Baron")
                                    {
                                            mob.State = 3;
                                            mob.LastChangeOnState = Environment.TickCount - 2000;
                                    }
                                    else
                                    {
                                        mob.State = 4;
                                        mob.LastChangeOnState = Environment.TickCount - 2000;
                                    }
                                }

                                if ((Environment.TickCount - mob.LastChangeOnState >= 10000 && camp.Name == "Crab"))
	                            {
		                            mob.State = 3;
                                    mob.LastChangeOnState = Environment.TickCount;
	                            }
                                break;
                            case 2:
                                if (Environment.TickCount - mob.LastChangeOnState >= 3500)
	                            {
		                            mob.State = 3;
                                    mob.LastChangeOnState = Environment.TickCount;
	                            }
                                break;
                            case 4:
                                if (Environment.TickCount - mob.LastChangeOnState >= 5000)
	                            {
		                            mob.State = 7;
                                    mob.JustDied = true;
	                            }
                                break;
                            case 5:
                                if (Environment.TickCount - mob.LastChangeOnState >= 30000)
	                            {
		                            mob.State = 0;
	                            }
                                break;
                            default:
                                break;
                        }

                        if (mob.Unit != null && mob.Unit.IsVisible && !mob.Unit.IsDead)
                        {
                            visible = true;
                        }

                        if ((mob.State == 7 || mob.State == 4) && visible) //check again
                        {
                            mob.State = 3;
                            mob.LastChangeOnState = Environment.TickCount;
                            mob.JustDied = false;
                        }

                        if (camp.Mobs.Count == 1)
                        {
                            camp.State = mob.State;
                            camp.LastChangeOnState = mob.LastChangeOnState;
                        }

                        if (camp.IsRanged && camp.Mobs.Count > 1 && mob.State > 0)
                        {
                            if (visible)
                            {
                                if (firstMob)
                                {
                                    camp.State = mob.State;
                                    camp.LastChangeOnState = mob.LastChangeOnState;
                                    firstMob = false;
                                }
                                else if (!firstMob)
                                {
                                    if (mob.State < camp.State)
                                    {
                                        camp.State = mob.State;
                                    }
                                    if (mob.LastChangeOnState > camp.LastChangeOnState)
                                    {
                                        camp.LastChangeOnState = mob.LastChangeOnState;
                                    }
                                }

                                if (!mob.IsRanged)
                                {
                                    camp.LastChangeOnState = Environment.TickCount;
                                    camp.RespawnTime = (camp.LastChangeOnState + camp.RespawnTimer * 1000);
                                }
                            }
                            else
                            {
                                if (firstMob)
                                {
                                    if (mob.IsRanged)
                                    {
                                        camp.State = mob.State;
                                        firstMob = false;
                                    }
                                    else
                                    {
                                        if (mob.State == 2)
                                        {
                                            disengaged = true;
                                        }
                                    }
                                    camp.LastChangeOnState = mob.LastChangeOnState;
                                
                                }
                                else if (!firstMob)
                                {
                                    if (mob.State < camp.State && mob.IsRanged)
                                    {
                                        camp.State = mob.State;
                                    }
                                    if (mob.LastChangeOnState > camp.LastChangeOnState)
                                    {
                                        camp.LastChangeOnState = mob.LastChangeOnState;
                                    }
                                }
                            }
                        }
                        else if (!camp.IsRanged && camp.Mobs.Count > 1 && mob.State > 0)
                        {
                            if (firstMob)
                            {
                                camp.State = mob.State;
                                camp.LastChangeOnState = mob.LastChangeOnState;
                                firstMob = false;
                            }
                            else
                            {
                                if (mob.State < camp.State)
                                {
                                    camp.State = mob.State;
                                }
                                if (mob.LastChangeOnState > camp.LastChangeOnState)
                                {
                                    camp.LastChangeOnState = mob.LastChangeOnState;
                                }
                            }
                            if (visible)
                            {
                                camp.LastChangeOnState = Environment.TickCount;
                                camp.RespawnTime = (camp.LastChangeOnState + camp.RespawnTimer * 1000);
                            }
                        }

                        if (camp.IsRanged && camp.Mobs.Count > 1 && mobCount == camp.Mobs.Count && firstMob)
                        {
                            if (disengaged)
                            {
                                camp.State = 2;
                            }
                            else
                            {
                                camp.State = 7;
                            }
                        
                        }

                        if (visible && camp.RespawnTime > Environment.TickCount)
                        {
                            camp.RespawnTime = (Environment.TickCount + camp.RespawnTimer * 1000);
                        }
                    }

                
                    //Do Stuff for each camp

                    if (camp.State == 7)
                    {
                        int mobsJustDiedCount = 0;

                        for (int i = 0; i < mobCount; i++)
                        {
                            try
                            {
                                if (camp.Mobs[i].JustDied)
                                {
                                    mobsJustDiedCount++;
                                }
                            }
                            catch (Exception)
                            {
                                //ignored
                            }
                        
                        }

                        if (mobsJustDiedCount == mobCount)
                        {
                            camp.RespawnTime = (camp.LastChangeOnState + camp.RespawnTimer * 1000);

                            for (int i = 0; i < mobCount; i++)
                            {
                                try
                                {
                                    camp.Mobs[i].JustDied = false;
                                }
                                catch (Exception)
                                {
                                    //ignored
                                }
                            }
                        }
                    }

                    if (camp.IsRanged && visibleMobsCount == 0 && rangedMobsCount == deadRangedMobsCount)
                    {
                        camp.RespawnTime = (camp.LastChangeOnState + camp.RespawnTimer * 1000);

                        for (int i = 0; i < mobCount; i++)
                        {
                            try
                            {
                                camp.Mobs[i].JustDied = false;
                            }
                            catch (Exception)
                            {
                                //ignored
                            }
                        
                        }
                    }

                    if (camp.Name == "Baron" && PossibleBaronList.Count >= 1 && camp.State >= 1 && camp.State <= 3)
                    {
                        try
                        {
                            PossibleBaronList.Clear();
                        }
                        catch (Exception)
                        {
                            //ignored
                        }
                    }

                    #endregion

                    #region Timers

                    if (camp.RespawnTime > Environment.TickCount && camp.State == 7)
                    {
                        var timespan = TimeSpan.FromSeconds((camp.RespawnTime - Environment.TickCount) / 1000f);

                        bool format;

                        if (camp.Position.IsOnScreen() && _menu.Item("timeronmap").GetValue<bool>())
                        {
                            format = !_menu.Item("timeronmapformat").GetValue<StringList>().SelectedIndex.Equals(0);

                            camp.Timer.TextOnMap = string.Format(format ? "{1}" : "{0}:{1:00}", (int)timespan.TotalMinutes,
                                format ? (int)timespan.TotalSeconds : timespan.Seconds);
                        }

                        if (_menu.Item("timeronminimap").GetValue<bool>())
                        {
                            format = !_menu.Item("timeronminimapformat").GetValue<StringList>().SelectedIndex.Equals(0);

                            camp.Timer.TextOnMinimap = string.Format(format ? "{1}" : "{0}:{1:00}",
                                (int)timespan.TotalMinutes,
                                format ? (int)timespan.TotalSeconds : timespan.Seconds);

                            var textrect = MapText.MeasureText(null, camp.Timer.TextOnMinimap, FontDrawFlags.Center);

                            camp.Timer.MinimapPosition = new Vector2((int)(camp.MinimapPosition.X - textrect.Width / 2f), (int)(camp.MinimapPosition.Y - textrect.Height / 2f));
                        }
                    }

                    if (camp.Position.IsOnScreen() && _menu.Item("timeronmap").GetValue<bool>())
                    {
                        var textrect = MapText.MeasureText(null, camp.Timer.TextOnMap, FontDrawFlags.Center);

                        camp.Timer.Position = new Vector3((int)(camp.Position.X - textrect.Width / 2f), (int)(camp.Position.Y - textrect.Height / 2f), 0);
                    }

                    #endregion

                    #region Guess Blue/Red NetworkID

                    if (GuessNetworkId1 == 1 && camp.Name == "Blue" && camp.Team.ToString().Contains("Order") && visibleMobsCount == camp.Mobs.Count &&
                        camp.Mobs[0].NetworkId != 0 && camp.Mobs[1].NetworkId != 0 && camp.Mobs[2].NetworkId != 0)
                    {
                        Seed1 = (camp.Mobs[1].NetworkId - camp.Mobs[0].NetworkId);
                        Seed2 = (camp.Mobs[2].NetworkId - camp.Mobs[1].NetworkId);

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

                        foreach (var camp2 in Jungle.Camps.Where(camp2 => camp2.MapType.ToString() == Game.MapId.ToString() && camp2.Name == "Blue" && !camp2.Team.ToString().Contains("Order")))
                        {
                            for (int j = 5; j <= 7; j++)
                            {
                                if (IdOrder[j] == 0) continue;
                                int i = 0;
                                switch (j)
                                {
                                    case 5:
                                        i = 2;
                                        break;
                                    case 6:
                                        i = 1;
                                        break;
                                    case 7:
                                        i = 0;
                                        break;
                                    default:
                                        break;
                                }

                                if (camp2.Mobs[i].NetworkId == 0)
                                {
                                    if (IdOrder[j] < IdOrder[4])
                                    {
                                        camp2.Mobs[i].NetworkId = camp.Mobs[0].NetworkId - ((IdOrder[4] - IdOrder[j]));
                                        camp2.Mobs[i].State = 5;
                                        camp2.Mobs[i].LastChangeOnState = Environment.TickCount;
                                    }
                                    else if (IdOrder[j] > IdOrder[4])
                                    {
                                        camp2.Mobs[i].NetworkId = camp.Mobs[0].NetworkId + ((IdOrder[j] - IdOrder[4]));
                                        camp2.Mobs[i].State = 5;
                                        camp2.Mobs[i].LastChangeOnState = Environment.TickCount;
                                    }
                                }
                            }
                        }
                        GuessNetworkId1 = 0;
                    
                    }
                    else if (GuessNetworkId1 == 1 && camp.Name == "Blue" && !camp.Team.ToString().Contains("Order") && visibleMobsCount == camp.Mobs.Count &&
                        camp.Mobs[0].NetworkId != 0 && camp.Mobs[1].NetworkId != 0 && camp.Mobs[2].NetworkId != 0)
                    {
                        Seed1 = (camp.Mobs[1].NetworkId - camp.Mobs[0].NetworkId);
                        Seed2 = (camp.Mobs[2].NetworkId - camp.Mobs[1].NetworkId);

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

                        foreach (var camp2 in Jungle.Camps.Where(camp2 => camp2.MapType.ToString() == Game.MapId.ToString() && camp2.Name == "Blue" && camp2.Team.ToString().Contains("Order")))
                        {
                            for (int j = 2; j <= 4; j++)
                            {
                                if (IdOrder[j] == 0) continue;
                                int i = 0;
                                switch (j)
                                {
                                    case 2:
                                        i = 2;
                                        break;
                                    case 3:
                                        i = 1;
                                        break;
                                    case 4:
                                        i = 0;
                                        break;
                                    default:
                                        break;
                                }

                                if (camp2.Mobs[i].NetworkId == 0)
                                {
                                    if (IdOrder[j] < IdOrder[7])
                                    {
                                        camp2.Mobs[i].NetworkId = camp.Mobs[0].NetworkId - ((IdOrder[7] - IdOrder[j]));
                                        camp2.Mobs[i].State = 5;
                                        camp2.Mobs[i].LastChangeOnState = Environment.TickCount;
                                    }
                                    else if (IdOrder[j] > IdOrder[7])
                                    {
                                        camp2.Mobs[i].NetworkId = camp.Mobs[0].NetworkId + ((IdOrder[j] - IdOrder[7]));
                                        camp2.Mobs[i].State = 5;
                                        camp2.Mobs[i].LastChangeOnState = Environment.TickCount;
                                    }
                                    //Console.WriteLine("NetworkID[" + j + "]:" + NetworkID[j] + " and Name: " + NameToCompare[j]);
                                }
                            }
                        }
                        GuessNetworkId1 = 0;
                    }

                    else if (GuessNetworkId1 == 1 && camp.Name == "Red" && camp.Team.ToString().Contains("Order") && visibleMobsCount == camp.Mobs.Count &&
                    camp.Mobs[0].NetworkId != 0 && camp.Mobs[1].NetworkId != 0 && camp.Mobs[2].NetworkId != 0)
                    {
                        Seed1 = (camp.Mobs[1].NetworkId - camp.Mobs[0].NetworkId);
                        Seed2 = (camp.Mobs[2].NetworkId - camp.Mobs[1].NetworkId);

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

                        foreach (var camp2 in Jungle.Camps.Where(camp2 => camp2.MapType.ToString() == Game.MapId.ToString() && camp2.Name == "Red" && !camp2.Team.ToString().Contains("Order")))
                        {
                            for (int j = 11; j <= 13; j++)
                            {
                                if (IdOrder[j] == 0) continue;
                                int i = 0;
                                switch (j)
                                {
                                    case 11:
                                        i = 2;
                                        break;
                                    case 12:
                                        i = 1;
                                        break;
                                    case 13:
                                        i = 0;
                                        break;
                                    default:
                                        break;
                                }

                                if (camp2.Mobs[i].NetworkId == 0)
                                {
                                    if (IdOrder[j] < IdOrder[10])
                                    {
                                        camp2.Mobs[i].NetworkId = camp.Mobs[0].NetworkId - ((IdOrder[10] - IdOrder[j]));
                                        camp2.Mobs[i].State = 5;
                                        camp2.Mobs[i].LastChangeOnState = Environment.TickCount;
                                    }
                                    else if (IdOrder[j] > IdOrder[10])
                                    {
                                        camp2.Mobs[i].NetworkId = camp.Mobs[0].NetworkId + ((IdOrder[j] - IdOrder[10]));
                                        camp2.Mobs[i].State = 5;
                                        camp2.Mobs[i].LastChangeOnState = Environment.TickCount;
                                    }
                                    //Console.WriteLine("NetworkID[" + j + "]:" + NetworkID[j] + " and Name: " + NameToCompare[j]);
                                }
                            }
                        }
                        GuessNetworkId1 = 0;

                    }
                    else if (GuessNetworkId1 == 1 && camp.Name == "Red" && !camp.Team.ToString().Contains("Order") && visibleMobsCount == camp.Mobs.Count &&
                        camp.Mobs[0].NetworkId != 0 && camp.Mobs[1].NetworkId != 0 && camp.Mobs[2].NetworkId != 0)
                    {
                        Seed1 = (camp.Mobs[1].NetworkId - camp.Mobs[0].NetworkId);
                        Seed2 = (camp.Mobs[2].NetworkId - camp.Mobs[1].NetworkId);

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

                        foreach (var camp2 in Jungle.Camps.Where(camp2 => camp2.MapType.ToString() == Game.MapId.ToString() && camp2.Name == "Red" && camp2.Team.ToString().Contains("Order")))
                        {
                            for (int j = 8; j <= 10; j++)
                            {
                                if (IdOrder[j] == 0) continue;
                                int i = 0;
                                switch (j)
                                {
                                    case 8:
                                        i = 2;
                                        break;
                                    case 9:
                                        i = 1;
                                        break;
                                    case 10:
                                        i = 0;
                                        break;
                                    default:
                                        break;
                                }

                                if (camp2.Mobs[i].NetworkId == 0)
                                {
                                    if (IdOrder[j] < IdOrder[13])
                                    {
                                        camp2.Mobs[i].NetworkId = camp.Mobs[0].NetworkId - ((IdOrder[13] - IdOrder[j]));
                                        camp2.Mobs[i].State = 5;
                                        camp2.Mobs[i].LastChangeOnState = Environment.TickCount;
                                    }
                                    else if (IdOrder[j] > IdOrder[13])
                                    {
                                        camp2.Mobs[i].NetworkId = camp.Mobs[0].NetworkId + ((IdOrder[j] - IdOrder[13]));
                                        camp2.Mobs[i].State = 5;
                                        camp2.Mobs[i].LastChangeOnState = Environment.TickCount;
                                    }
                                    //Console.WriteLine("NetworkID[" + j + "]:" + NetworkID[j] + " and Name: " + NameToCompare[j]);
                                }
                            }
                        }
                        GuessNetworkId1 = 0;
                    }

                    #endregion

                }
            
                #region Play Dragon/Baron Sound

                if (DragonCamp.State != 1)
                {
                    BufferDragonSound = 0;
                    PlayingDragonSound = 0;
                }

                if (BufferDragonSound > 0 && (Environment.TickCount - LastPlayedDragonSound > 500) && DragonCamp.State == 1 &&
                    (Environment.TickCount - DragonSoundDelay >= (_menu.Item("dragonsounddelay").GetValue<Slider>().Value * 1000)))
                {
                    LastPlayedDragonSound = Environment.TickCount;
                    PlaySound(_sound);
                    BufferDragonSound -= 1;
                    if (BufferDragonSound == 0)
                    {
                        PlayingDragonSound = 0;
                        DragonSoundDelay = Environment.TickCount;
                    }
                }

                if (BaronCamp.State != 1)
                {
                    BufferBaronSound = 0;
                    PlayingBaronSound = 0;
                }

                if (BufferBaronSound > 0 && (Environment.TickCount - LastPlayedBaronSound > 500) && BaronCamp.State == 1 && 
                    (Environment.TickCount - BaronSoundDelay >= (_menu.Item("baronsounddelay").GetValue<Slider>().Value * 1000)))
                {
                    LastPlayedBaronSound = Environment.TickCount;
                    PlaySound(_sound);
                    BufferBaronSound -= 1;
                    if (BufferBaronSound == 0)
                    {
                        PlayingBaronSound = 0;
                        BaronSoundDelay = Environment.TickCount;
                    }
                }

                #endregion
            
                #region Static Menu Update


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

                #endregion

                #region Get Dragon/Baron buff

                if (Game.MapId.ToString() == "SummonersRift")
                {
                    var ally = HeroManager.Allies.FirstOrDefault(x => x.IsValidTarget(50000f, false));

                    if (enemy != null)
                    {
                        if (GetDragonStacks(enemy) > EnemyTeamStacks)
                        {
                            EnemyTeamStacks = EnemyTeamStacks + 1;

                            if (Environment.TickCount - EnemyFoWTime > 100 ||
                                (Environment.TickCount - EnemyFoWTime <= 100 && (DragonCamp.Mobs[0].State != 4 && DragonCamp.Mobs[0].State != 7)))
                            {
                                DragonCamp.Mobs[0].State = 4;
                                DragonCamp.Mobs[0].LastChangeOnState = Environment.TickCount;
                                DragonCamp.LastChangeOnState = Environment.TickCount;
                                DragonCamp.RespawnTime = (DragonCamp.LastChangeOnState + DragonCamp.RespawnTimer * 1000);
                            }
                            //Console.WriteLine("Enemy Dragon Stacks: " + EnemyTeamStacks);
                        }
                        else if (GetNashorBuff(enemy) && !EnemyTeamNashor)
                        {
                            EnemyTeamNashor = true;

                            if (Environment.TickCount - EnemyFoWTime > 100 ||
                                (Environment.TickCount - EnemyFoWTime <= 100 && BaronCamp.Mobs[0].State != 4 && BaronCamp.Mobs[0].State != 7))
                            {
                                BaronCamp.Mobs[0].State = 4;
                                BaronCamp.Mobs[0].LastChangeOnState = Environment.TickCount;
                                BaronCamp.LastChangeOnState = Environment.TickCount;
                                BaronCamp.RespawnTime = (BaronCamp.LastChangeOnState + BaronCamp.RespawnTimer * 1000);
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
                                (Environment.TickCount - AllyFoWTime <= 100 && (DragonCamp.Mobs[0].State != 4 && DragonCamp.Mobs[0].State != 7)))
                            {
                                DragonCamp.Mobs[0].State = 4;
                                DragonCamp.Mobs[0].LastChangeOnState = Environment.TickCount;
                                DragonCamp.LastChangeOnState = Environment.TickCount;
                                DragonCamp.RespawnTime = (DragonCamp.LastChangeOnState + DragonCamp.RespawnTimer * 1000);
                            }
                            //Console.WriteLine("Ally Dragon Stacks: " + AllyTeamStacks);
                        }
                        else if (GetNashorBuff(ally) && !AllyTeamNashor)
                        {
                            AllyTeamNashor = true;

                            if (Environment.TickCount - AllyFoWTime > 100 ||
                                (Environment.TickCount - AllyFoWTime <= 100 && BaronCamp.Mobs[0].State != 4 && BaronCamp.Mobs[0].State != 7))
                            {
                                BaronCamp.Mobs[0].State = 4;
                                BaronCamp.Mobs[0].LastChangeOnState = Environment.TickCount;
                                BaronCamp.LastChangeOnState = Environment.TickCount;
                                BaronCamp.RespawnTime = (BaronCamp.LastChangeOnState + BaronCamp.RespawnTimer * 1000);
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
                        BaronCamp.Mobs[0].State == 4)
                    {
                        BaronCamp.Mobs[0].State = 1;
                    }
                }

                #endregion  //Credits to Inferno

                UpdateTick = Environment.TickCount;
            }
        }

        public static int GetDragonStacks(Obj_AI_Hero hero)
        {
            var baff = hero.Buffs.FirstOrDefault(buff => buff.Name == "s5test_dragonslayerbuff");
            if (baff == null)
                return 0;
            else
                return baff.Count;
        }//Credits to Inferno

        public static bool GetNashorBuff(Obj_AI_Hero hero)
        {
            return hero.Buffs.Any(buff => buff.Name == "exaltedwithbaronnashor");
        } //Credits to Inferno

        public static void OnProcessPacket(GamePacketEventArgs args)
        {
            short header = BitConverter.ToInt16(args.PacketData, 0);

            int length = BitConverter.ToString(args.PacketData, 0).Length;

            int networkID = BitConverter.ToInt32(args.PacketData, 2);

            if (header == 0)
            {
                return;
            }
            
            
            #region AutoFind Headers

            if (_menu.Item("forcefindheaders").GetValue<bool>())
            {
                _menu.Item("headerOnAttack" + GameVersion).SetValue<Slider>(new Slider(0, 0, 400));
                _menu.Item("headerOnMissileHit" + GameVersion).SetValue<Slider>(new Slider(0, 0, 400));
                _menu.Item("headerOnDisengaged" + GameVersion).SetValue<Slider>(new Slider(0, 0, 400));
                _menu.Item("headerOnMonsterSkill" + GameVersion).SetValue<Slider>(new Slider(0, 0, 400));
                _menu.Item("headerOnCreateGromp" + GameVersion).SetValue<Slider>(new Slider(0, 0, 400));
                _menu.Item("headerOnCreateCampIcon" + GameVersion).SetValue<Slider>(new Slider(0, 0, 400));
                Packets.Attack.Header = 0;
                Packets.MissileHit.Header = 0;
                Packets.Disengaged.Header = 0;
                Packets.MonsterSkill.Header = 0;
                Packets.CreateGromp.Header = 0;
                Packets.CreateCampIcon.Header = 0;
                _menu.Item("forcefindheaders").SetValue<bool>(false);
            }

            

            if (_menu.Item("headerOnAttack" + GameVersion).GetValue<Slider>().Value == 0 && length == Packets.Attack.Length && networkID > 0)
            {
                foreach (Obj_AI_Minion obj in ObjectManager.Get<Obj_AI_Minion>().Where(obj => obj.NetworkId == networkID))
                {
                    OnAttackList.Add(header);
                    if (OnAttackList.Count<int>(x => x == header) == 5)
                    {
                        _menu.Item("headerOnAttack" + GameVersion).SetValue<Slider>(new Slider(header, 0, 400));
                        Packets.Attack.Header = header;
                        try
                        {
                            OnAttackList.Clear();
                        }
                        catch (Exception)
                        {
                            //ignored
                        }
                    }
                }
            }

            if (_menu.Item("headerOnMissileHit" + GameVersion).GetValue<Slider>().Value == 0 && length == Packets.MissileHit.Length && networkID > 0)
            {
                foreach (Obj_AI_Minion obj in ObjectManager.Get<Obj_AI_Minion>().Where(obj => obj.IsRanged && obj.NetworkId == networkID))
                {
                    MissileHitList.Add(header);
                    if (MissileHitList.Count<int>(x => x == header) == 5)
                    {
                        _menu.Item("headerOnMissileHit" + GameVersion).SetValue<Slider>(new Slider(header, 0, 400));
                        Packets.MissileHit.Header = header;
                        try
                        {
                            MissileHitList.Clear();
                        }
                        catch (Exception)
                        {
                            //ignored
                        }
                    }
                }
            }

            if (_menu.Item("headerOnDisengaged" + GameVersion).GetValue<Slider>().Value == 0 && length == Packets.Disengaged.Length && networkID > 0)
            {
                foreach (Obj_AI_Minion obj in ObjectManager.Get<Obj_AI_Minion>().Where(obj => obj.Team.ToString().Contains("Neutral") && obj.NetworkId == networkID))
                {
                    _menu.Item("headerOnDisengaged" + GameVersion).SetValue<Slider>(new Slider(header, 0, 400));
                    Packets.Disengaged.Header = header;
                }
            }

            if (_menu.Item("headerOnMonsterSkill" + GameVersion).GetValue<Slider>().Value == 0 && length == Packets.MonsterSkill.Length && networkID > 0)
            {
                foreach (Obj_AI_Minion obj in ObjectManager.Get<Obj_AI_Minion>().Where(obj => obj.Name.Contains("Dragon") && obj.NetworkId == networkID))
                {
                    _menu.Item("headerOnMonsterSkill" + GameVersion).SetValue<Slider>(new Slider(header, 0, 400));
                    Packets.MonsterSkill.Header = header;
                }
            }

            if (_menu.Item("headerOnCreateGromp" + GameVersion).GetValue<Slider>().Value == 0 && (length == Packets.CreateGromp.Length || length == Packets.CreateGromp.Length2) && networkID > 0)
            {
                OnCreateGrompList.Add(new int[] { networkID, (int)header, length });
            }

            if (_menu.Item("headerOnCreateCampIcon" + GameVersion).GetValue<Slider>().Value == 0 && networkID == 0 &&
                (length == Packets.CreateCampIcon.Length || length == Packets.CreateCampIcon.Length2 || length == Packets.CreateCampIcon.Length3 || length == Packets.CreateCampIcon.Length4 || length == Packets.CreateCampIcon.Length5))
            {
                OnCreateCampIconList.Add(new int[] { (int)header, length });

                if ((OnCreateCampIconList.Count(item => item[0] == (int)header && item[1] == Packets.CreateCampIcon.Length) == 6) &&
                    (OnCreateCampIconList.Count(item => item[0] == (int)header && item[1] == Packets.CreateCampIcon.Length2) == 3) &&
                    (OnCreateCampIconList.Count(item => item[0] == (int)header && item[1] == Packets.CreateCampIcon.Length3) == 1) &&
                    (OnCreateCampIconList.Count(item => item[0] == (int)header && item[1] == Packets.CreateCampIcon.Length4) == 1) &&
                    (OnCreateCampIconList.Count(item => item[0] == (int)header && item[1] == Packets.CreateCampIcon.Length5) == 1))
                {
                    _menu.Item("headerOnCreateCampIcon" + GameVersion).SetValue<Slider>(new Slider(header, 0, 400));
                    Packets.CreateCampIcon.Header = header;
                    try
                    {
                        OnCreateCampIconList.Clear();
                    }
                    catch (Exception)
                    {
                        //ignored
                    }
                }
            }

            #endregion
            
            #region Update States

            bool isMob = false;

            foreach (var camp in Jungle.Camps.Where(camp => camp.MapType.ToString() == Game.MapId.ToString()))
            {
                //Do Stuff for each camp

                foreach (var mob in camp.Mobs.Where(mob => mob.NetworkId == networkID))
                {
                    //Do Stuff for each mob in a camp

                    isMob = true;

                    if (header == Packets.MonsterSkill.Header)
                    {
                        if (mob.Name.Contains("Crab"))
                        {
                            mob.State = 4;
                        }
                        else
                        {
                            if (mob.Name.Contains("Dragon") && mob.State != 1)
                            {
                                if (BufferDragonSound == 0 && PlayingDragonSound == 0 && ((_menu.Item("soundfow").GetValue<bool>() && (camp.Mobs[0].Unit == null || !camp.Mobs[0].Unit.IsVisible)) || !_menu.Item("soundfow").GetValue<bool>()))
                                {
                                    if ((_menu.Item("soundscreen").GetValue<bool>() && !camp.Position.IsOnScreen()) || !_menu.Item("soundscreen").GetValue<bool>())
                                    {
                                        if ((_menu.Item("dragonsound").GetValue<bool>()))
                                        {
                                            BufferDragonSound = _menu.Item("dragonsoundtimes").GetValue<Slider>().Value;
                                            PlayingDragonSound = 1;
                                            camp.State = 1;
                                        }
                                    }
                                }
                            }
                            mob.State = 1;
                        }
                        mob.LastChangeOnState = Environment.TickCount;
                    }

                    else if (header == Packets.Attack.Header)
                    {
                        //Console.WriteLine(NameToCompare[i] + " is Attacking");

                        mob.State = 1;
                        mob.LastChangeOnState = Environment.TickCount;
                    }

                    else if (header == Packets.MissileHit.Header)
                    {
                        //Console.WriteLine(NameToCompare[i] + " is Attacking (ranged)");

                        mob.State = 1;
                        mob.LastChangeOnState = Environment.TickCount;
                    }

                    else if (header == Packets.Disengaged.Header)
                    {
                        //Console.WriteLine(NameToCompare[i] + " is Disengaged");
                        if (mob.Name.Contains("Crab"))
                        {
                            if (mob.State == 0) mob.State = 5;    //check this again
                            else mob.State = 1;
                        }
                        if (!mob.Name.Contains("Crab") && !mob.Name.Contains("Spider"))
                        {
                            if (mob.State == 0) mob.State = 5;
                            else mob.State = 2;
                        }
                        mob.LastChangeOnState = Environment.TickCount;
                    }

                    if (mob.LastChangeOnState == Environment.TickCount && camp.Mobs.Count == 1)
                    {
                        camp.State = mob.State;
                        camp.LastChangeOnState = mob.LastChangeOnState;
                    }
                }
            }

            #endregion

            #region Guess Dragon/Baron NetworkID

            bool playBaronSound = false;

            if (!isMob && (BaronCamp.Mobs[0].State < 1 || BaronCamp.Mobs[0].State > 3))
            {
                bool isLoaded = false;
                foreach (Obj_AI_Base obj in ObjectManager.Get<Obj_AI_Base>().Where(obj => obj.NetworkId == networkID))
                {
                    isLoaded = true;
                }

                if (!isLoaded && Packets.MissileHit.Header == header && Packets.MissileHit.Length == length)
                {
                    PossibleBaronList.Add(new int[] { networkID, (int)header, length });

                    if ((PossibleBaronList.Count(item => item[0] == networkID && item[1] == Packets.MonsterSkill.Header && item[2] == Packets.MonsterSkill.Length) >= 1) &&
                    (PossibleBaronList.Count(item => item[0] == networkID && item[1] == Packets.MonsterSkill.Header && item[2] == Packets.MonsterSkill.Length2) >= 1))
                    {
                        playBaronSound = true;
                        BaronCamp.Mobs[0].State = 1;
                        BaronCamp.Mobs[0].LastChangeOnState = Environment.TickCount;
                        BaronCamp.Mobs[0].NetworkId = networkID;
                    }

                }
                else if (!isLoaded && Packets.MonsterSkill.Header == header && Packets.MonsterSkill.Length == length)
                {
                    PossibleBaronList.Add(new int[] { networkID, (int)header, length });

                    if ((PossibleBaronList.Count(item => item[0] == networkID && item[1] == Packets.MissileHit.Header && item[2] == Packets.MissileHit.Length) >= 1) &&
                    (PossibleBaronList.Count(item => item[0] == networkID && item[1] == Packets.MonsterSkill.Header && item[2] == Packets.MonsterSkill.Length2) >= 1))
                    {
                        playBaronSound = true;
                        BaronCamp.Mobs[0].State = 1;
                        BaronCamp.Mobs[0].LastChangeOnState = Environment.TickCount;
                        BaronCamp.Mobs[0].NetworkId = networkID;
                    }
                }
                else if (!isLoaded && Packets.MonsterSkill.Header == header && Packets.MonsterSkill.Length2 == length)
                {
                    PossibleBaronList.Add(new int[] { networkID, (int)header, length });

                    if ((PossibleBaronList.Count(item => item[0] == networkID && item[1] == Packets.MissileHit.Header && item[2] == Packets.MissileHit.Length) >= 1) &&
                    (PossibleBaronList.Count(item => item[0] == networkID && item[1] == Packets.MonsterSkill.Header && item[2] == Packets.MonsterSkill.Length) >= 1))
                    {
                        playBaronSound = true;
                        BaronCamp.Mobs[0].State = 1;
                        BaronCamp.Mobs[0].LastChangeOnState = Environment.TickCount;
                        BaronCamp.Mobs[0].NetworkId = networkID;
                    }
                }
            }

            if (Environment.TickCount <= PossibleDragonTimer + 5000)
            {
                foreach (var id in PossibleDragonList.Where(id => id == networkID))
                {
                    PossibleDragonList.RemoveAll(x => x == networkID);
                }
            }
            else
            {
                if (PossibleDragonList.Count() == 1)
                {
                    if (BufferDragonSound == 0 && PlayingDragonSound == 0 && ((_menu.Item("soundfow").GetValue<bool>() && (DragonCamp.Mobs[0].Unit == null || !DragonCamp.Mobs[0].Unit.IsVisible)) || !_menu.Item("soundfow").GetValue<bool>()))
                    {
                        if ((_menu.Item("soundscreen").GetValue<bool>() && !DragonCamp.Position.IsOnScreen()) || !_menu.Item("soundscreen").GetValue<bool>())
                        {
                            if ((_menu.Item("dragonsound").GetValue<bool>()))
                            {
                                BufferDragonSound = _menu.Item("dragonsoundtimes").GetValue<Slider>().Value;
                                PlayingDragonSound = 1;
                                DragonCamp.State = 1;
                            }
                        }
                    }

                    DragonCamp.Mobs[0].State = 1;
                    DragonCamp.Mobs[0].LastChangeOnState = Environment.TickCount;
                    DragonCamp.Mobs[0].NetworkId = PossibleDragonList[0];
                }
                try
                {
                    PossibleDragonList.Clear();
                }
                catch (Exception)
                {
                    //ignored
                }
            }


            if (header == Packets.MonsterSkill.Header &&
                Game.MapId.ToString() == "SummonersRift" &&
                BitConverter.ToInt32(args.PacketData, 2) != DragonCamp.Mobs[0].NetworkId &&
                BitConverter.ToInt32(args.PacketData, 2) != BaronCamp.Mobs[0].NetworkId &&
                BitConverter.ToInt32(args.PacketData, 2) > BiggestNetworkId &&
                BitConverter.ToString(args.PacketData, 0).Length == Packets.MonsterSkill.Length &&
                GuessDragonId == 1
                )
            {
                bool aiBaseTest = false;
                foreach (Obj_AI_Base obj in ObjectManager.Get<Obj_AI_Base>().Where(x => x.NetworkId == networkID))
                {
                    if (!obj.IsAlly)
                    {
                        if (!obj.Name.Contains("SRU_Dragon") && !obj.Name.Contains("SRU_Baron"))
                        {
                            Game.PrintChat("<font color=\"#FF0000\"> God Jungle Tracker (debug): Tell AlphaGod he forgot to consider: " + obj.Name + " - " + obj.SkinName + " - " + obj.CharData.BaseSkinName + " - Guess Dragon NetWorkID disabled</font>");
                            GuessDragonId = 0;
                        }
                    }
                    aiBaseTest = true;
                }

                if (!aiBaseTest)
                {
                    if (BaronCamp.Mobs[0].State <= 3)
                    {
                        PossibleDragonList.Add(networkID);
                        PossibleDragonTimer = Environment.TickCount;
                    }
                    else if (DragonCamp.Mobs[0].State == 3)
                    {
                        playBaronSound = true;
                        BaronCamp.Mobs[0].State = 1;
                        BaronCamp.Mobs[0].LastChangeOnState = Environment.TickCount;
                        BaronCamp.Mobs[0].NetworkId = networkID;
                    }
                }
            }

            if (playBaronSound)
            {
                if (BufferBaronSound == 0 && PlayingBaronSound == 0 && ((_menu.Item("soundfow").GetValue<bool>() && (BaronCamp.Mobs[0].Unit == null || !BaronCamp.Mobs[0].Unit.IsVisible)) || !_menu.Item("soundfow").GetValue<bool>()))
                {
                    if ((_menu.Item("soundscreen").GetValue<bool>() && !BaronCamp.Position.IsOnScreen()) || !_menu.Item("soundscreen").GetValue<bool>())
                    {
                        if ((_menu.Item("baronsound").GetValue<bool>()))
                        {
                            BufferBaronSound = _menu.Item("baronsoundtimes").GetValue<Slider>().Value;
                            PlayingBaronSound = 1;
                            BaronCamp.State = 1;
                        }
                    }
                }
            }

            #endregion  

            #region Gromp Created

            if (header == Packets.CreateGromp.Header && Game.MapId.ToString() == "SummonersRift")  //Gromp Created
            {
                if (length == Packets.CreateGromp.Length)
                {
                    foreach (var camp in Jungle.Camps.Where(camp => camp.Name == "Gromp"))
                    {
                        foreach (var mob in camp.Mobs.Where(mob => mob.Name.Contains("SRU_Gromp13.1.1")))
                        {
                            mob.NetworkId = BitConverter.ToInt32(args.PacketData, 2);
                            mob.State = 3;
                            mob.LastChangeOnState = Environment.TickCount;
                            camp.State = mob.State;
                            camp.LastChangeOnState = mob.LastChangeOnState;
                        }
                    }

                    if (Game.ClockTime - 111f < 90 && ClockTimeAdjust == 0)
                    {
                        ClockTimeAdjust = Game.ClockTime - 111f;
                        DragonCamp.Mobs[0].State = 0;
                        DragonCamp.RespawnTime = Environment.TickCount + 39000;
                        DragonCamp.State = 0;
                        BiggestNetworkId = BitConverter.ToInt32(args.PacketData, 2);
                    }
                }
                else if (length == Packets.CreateGromp.Length2)
                {
                    foreach (var camp in Jungle.Camps.Where(camp => camp.Name == "Gromp"))
                    {
                        foreach (var mob in camp.Mobs.Where(mob => mob.Name.Contains("SRU_Gromp14.1.1")))
                        {
                            mob.NetworkId = BitConverter.ToInt32(args.PacketData, 2);
                            mob.State = 3;
                            mob.LastChangeOnState = Environment.TickCount;
                            camp.State = mob.State;
                            camp.LastChangeOnState = mob.LastChangeOnState;
                        }
                    }
                }
            }
            #endregion
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            foreach (var camp in Jungle.Camps.Where(camp => camp.MapType.ToString() == Game.MapId.ToString()))
            {
                //Do Stuff for each camp

                #region Minimap Circles

                if (camp.State == 1)
                {
                    Utility.DrawCircle(camp.Position, circleradius, colorattacking, circlewidth + 1, 30, true);
                }
                else if (camp.State == 2)
                {
                    Utility.DrawCircle(camp.Position, circleradius, colordisengaged, circlewidth + 1, 30, true);
                }
                else if (camp.State == 3 && (camp.IsRanged || (camp.Name == "Dragon" || camp.Name == "Crab" || camp.Name == "Spider")))
                {
                    Utility.DrawCircle(camp.Position, circleradius, colortracked, circlewidth, 30, true);
                }
                else if (camp.State == 4)
                {
                    Utility.DrawCircle(camp.Position, circleradius, colordead, circlewidth, 30, true);
                }
                else if (camp.State == 5)
                {
                    Utility.DrawCircle(camp.Position, circleradius, colorguessed, circlewidth, 30, true);
                }

                #endregion

                #region Timers

                if (camp.RespawnTime > Environment.TickCount && camp.State == 7)
                {
                    if (camp.Position.IsOnScreen() && timeronmap)
                    {
                        try
                        {
                            var pos = Drawing.WorldToScreen(camp.Timer.Position);
                            MapText.DrawText(null, camp.Timer.TextOnMap, (int)pos.X, (int)pos.Y, white);
                        }
                        catch (Exception)
                        {
                            //ingore
                        }
                    }

                    if (timeronminimap)
                    {
                        try
                        {
                            MinimapText.DrawText(null, camp.Timer.TextOnMinimap, (int)camp.Timer.MinimapPosition.X, (int)camp.Timer.MinimapPosition.Y, white);
                        }
                        catch (Exception)
                        {
                            //ingore
                        }
                    }
                }

                #endregion
            }
        }
         
        public static void LoadMenu()
        {
            try
            {
                //Start Menu
                _menu = new Menu("God Jungle Tracker", "God Jungle Tracker", true);

                //Track List Menu
                //_menu.AddSubMenu(new Menu("Track List", "Track List"));
                //_menu.SubMenu("Track List").AddItem(new MenuItem("drawtracklist", "Draw Track List").SetValue(false));
                //_menu.SubMenu("Track List").AddItem(new MenuItem("posX", "Track List Pos X").SetValue(new Slider(Drawing.Width / 2, 0, Drawing.Width)));
                //_menu.SubMenu("Track List").AddItem(new MenuItem("posY", "Track List Pos Y").SetValue(new Slider(Drawing.Height / 2, 0, Drawing.Height)));

                //Play Danger Sound
                _menu.AddSubMenu(new Menu("Play Danger Sound", "Play Danger Sound"));
                _menu.SubMenu("Play Danger Sound").AddItem(new MenuItem("dragonsound", "On Dragon First Attack").SetValue(true));
                _menu.SubMenu("Play Danger Sound").AddItem(new MenuItem("dragonsoundtimes", "Play Sound X Times").SetValue(new Slider(2, 1, 4)));
                _menu.SubMenu("Play Danger Sound").AddItem(new MenuItem("dragonsounddelay", "Dragon Sound Delay (s)").SetValue(new Slider(10, 1, 30)));
                _menu.SubMenu("Play Danger Sound").AddItem(new MenuItem("baronsound", "On Baron Attack").SetValue(true));
                _menu.SubMenu("Play Danger Sound").AddItem(new MenuItem("baronsoundtimes", "Play Sound X Times").SetValue(new Slider(2, 1, 4)));
                _menu.SubMenu("Play Danger Sound").AddItem(new MenuItem("baronsounddelay", "Baron Sound Delay (s)").SetValue(new Slider(10, 1, 30)));
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

                //Advanced
                _menu.AddSubMenu(new Menu("Advanced", "Advanced"));
                var advanced = _menu.SubMenu("Advanced");
                advanced.SubMenu("Headers").AddItem(new MenuItem("forcefindheaders", "Force Auto-Find Headers").SetValue(false));
                advanced.SubMenu("Headers").AddItem(new MenuItem("headerOnAttack" + GameVersion, "Header OnAttack").SetValue(new Slider(0, 0, 400)));
                advanced.SubMenu("Headers").AddItem(new MenuItem("headerOnMissileHit" + GameVersion, "Header OnMissileHit").SetValue(new Slider(0, 0, 400)));
                advanced.SubMenu("Headers").AddItem(new MenuItem("headerOnDisengaged" + GameVersion, "Header OnDisengaged").SetValue(new Slider(0, 0, 400)));
                advanced.SubMenu("Headers").AddItem(new MenuItem("headerOnMonsterSkill" + GameVersion, "Header OnMonsterSkill").SetValue(new Slider(0, 0, 400)));
                advanced.SubMenu("Headers").AddItem(new MenuItem("headerOnCreateGromp" + GameVersion, "Header OnCreateGromp").SetValue(new Slider(0, 0, 400)));
                advanced.SubMenu("Headers").AddItem(new MenuItem("headerOnCreateCampIcon" + GameVersion, "Header OnCreateCampIcon").SetValue(new Slider(0, 0, 400)));
                advanced.SubMenu("Headers").AddItem(new MenuItem("patch", "Headers From Patch: " + GameVersion));

                //String[] gameversion = { "" };
                //advanced.SubMenu("Headers").AddItem(new MenuItem("headerFromPatch" + Game.Version.Substring(0, 4), "Headers From Patch: " + Game.Version.Substring(0, 4)).SetValue(new StringList(GameVersion)));

                _menu.SubMenu("Advanced").AddItem(new MenuItem("updatetick", "Update Tick").SetValue(new Slider(150,0,1000)));

                _menu.AddSubMenu(new Menu("Debug", "Debug"));
                _menu.SubMenu("Debug").AddItem(new MenuItem("onupdate", "OnUpdate").SetValue(true));
                _menu.SubMenu("Debug").AddItem(new MenuItem("onprocesspacket", "OnProcessPacket").SetValue(true));
                _menu.SubMenu("Debug").AddItem(new MenuItem("oncreate", "OnCreate").SetValue(true));
                _menu.SubMenu("Debug").AddItem(new MenuItem("ondelete", "OnDelete").SetValue(true));
                _menu.SubMenu("Debug").AddItem(new MenuItem("onendscene", "OnEndScene").SetValue(true));
            
                _menu.AddToMainMenu();

            }
            catch (Exception)
            {
                Console.WriteLine("[GJT] - Menu Init Failed");
            }
        }
    }
}
