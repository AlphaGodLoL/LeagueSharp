/*
██████╗ ██╗   ██╗     █████╗ ██╗     ██████╗ ██╗  ██╗ █████╗  ██████╗  ██████╗ ██████╗ 
██╔══██╗╚██╗ ██╔╝    ██╔══██╗██║     ██╔══██╗██║  ██║██╔══██╗██╔════╝ ██╔═══██╗██╔══██╗
██████╔╝ ╚████╔╝     ███████║██║     ██████╔╝███████║███████║██║  ███╗██║   ██║██║  ██║
██╔══██╗  ╚██╔╝      ██╔══██║██║     ██╔═══╝ ██╔══██║██╔══██║██║   ██║██║   ██║██║  ██║
██████╔╝   ██║       ██║  ██║███████╗██║     ██║  ██║██║  ██║╚██████╔╝╚██████╔╝██████╔╝
╚═════╝    ╚═╝       ╚═╝  ╚═╝╚══════╝╚═╝     ╚═╝  ╚═╝╚═╝  ╚═╝ ╚═════╝  ╚═════╝ ╚═════╝ 
*/

#region

using System;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using OrbwalkerAddon.Classes;
using System.Linq;
using SharpDX;

#endregion

namespace OrbwalkerAddon
{
    public class Program
    {
        public static Menu Menu;

        public static int LastAttackOrder;
        public static int LastAttack;
        public static int LastMoveOrder;
        public static int UpdateTick;
        public static bool JustAttacked;
        public static bool IssueOrder;
        public static bool AttackOrder;
        public static bool ShouldMove;
        public static bool ShouldBlock;
        public static bool AttackOnRange;
        public static bool GetWhenOnRange;
        public static bool BufferMovement;
        public static bool BufferAttack;
        public static int LastServerResponseDelay;
        public static GameObject LastTarget;
        public static Obj_AI_Hero Player = ObjectManager.Player;
        public static int NetworkId = Player.NetworkId;
        public static List<int> OnAttackList;
        public static List<int> MissileHitList;
        public static List<float[]> CustomMissileHit = new List<float[]>();
        public static List<float[]> CurrentMissileHit;
        public static string GameVersion = Game.Version.Substring(0, 4);
        public static float Sum = 0;
        public static float MediumTime = 0;
        public static bool MissileLaunched;
        public static Vector3 BufferPosition = new Vector3(0, 0, 0); 
        public static Obj_AI_Base BufferTarget;
        public static int BufferAttackTimer;
       

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void LoadMenu()
        {
            Menu = new Menu("Orbwalker Addon", "Orbwalker Addon", true);

            //Orbwalking
            var orbwalker = Menu.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            orbwalker.AddItem(new MenuItem("debug", "Debug (Console)").SetValue(false).SetTooltip("Attack delays showed on console window"));
            orbwalker.AddItem(new MenuItem("missilecheck", "Custom Missile Check").SetValue(true).SetTooltip("When the server responds that the missile is hit then you can move (slower then common Missile Check)"));
            orbwalker.AddItem(new MenuItem("ExtraWindup", "Extra windup time").SetValue(new Slider(60, 0, 200)).SetTooltip("Set the same as Orbwalker windup time"));
            orbwalker.AddItem(new MenuItem("LastHitKey", "Last hit").SetValue(new KeyBind('X', KeyBindType.Press)));
            orbwalker.AddItem(new MenuItem("LaneClearKey", "LaneClear").SetValue(new KeyBind('V', KeyBindType.Press)));
            orbwalker.AddItem(new MenuItem("MixedKey", "Mixed").SetValue(new KeyBind('C', KeyBindType.Press)));
            orbwalker.AddItem(new MenuItem("ComboKey", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));

            //Block After Attack
            var config = Menu.AddSubMenu(new Menu("Config", "Config"));
            config.AddItem(new MenuItem("BufferAttack", "Buffer attack").SetValue(true).SetTooltip("If a attack was blocked it will wait to attack when it's ready"));
            config.AddItem(new MenuItem("BufferMovement", "Buffer movement").SetValue(true).SetTooltip("If a movement was blocked it will wait to move after the attack is finished"));
            config.AddItem(new MenuItem("BlockOrder", "Block order on attack").SetValue(true).SetTooltip("This will block any movement or attack that would cancel a attack (except evade movements)"));
            config.AddItem(new MenuItem("blocktip", "This won't block Evade movements"));


            //Follow Mouse (Orbwalk)
            var movement = Menu.AddSubMenu(new Menu("Enable Movement", "Enable Movement"));
            movement.AddItem(new MenuItem("LastHitMovement", "On Last hit").SetValue(true).SetTooltip("Enable/Disable auto Orbwalk"));
            movement.AddItem(new MenuItem("LaneClearMovement", "On LaneClear").SetValue(true).SetTooltip("Enable/Disable auto Orbwalk"));
            movement.AddItem(new MenuItem("MixedMovement", "On Mixed").SetValue(true).SetTooltip("Enable/Disable auto Orbwalk"));
            movement.AddItem(new MenuItem("ComboMovement", "On Combo").SetValue(true).SetTooltip("Enable/Disable auto Orbwalk"));

            //Packets
            Menu.AddSubMenu(new Menu("Headers", "Headers"));
            Menu.SubMenu("Headers").AddItem(new MenuItem("forcefindheaders", "Force Auto-Find Headers").SetValue(false));
            Menu.SubMenu("Headers").AddItem(new MenuItem("headerOnAttack" + GameVersion, "Header OnAttack").SetValue(new Slider(0, 0, 400)).SetTooltip("This value is automatic please don't change it"));
            Menu.SubMenu("Headers").AddItem(new MenuItem("headerOnMissileHit" + GameVersion, "Header OnMissileHit").SetValue(new Slider(0, 0, 400)).SetTooltip("This value is automatic please don't change it"));
            Menu.SubMenu("Headers").AddItem(new MenuItem("patch", "Headers From Patch: " + GameVersion).SetTooltip("This assembly does not send packets to riot"));

            Menu.AddToMainMenu();
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            OnAttackList = new List<int>();
            MissileHitList = new List<int>();
            GameVersion = Game.Version.Substring(0, 4);
            IssueOrder = true;

            LoadMenu();

            #region Set Headers

            Packets.Attack.Header = Menu.Item("headerOnAttack" + GameVersion).GetValue<Slider>().Value;
            Packets.MissileHit.Header = Menu.Item("headerOnMissileHit" + GameVersion).GetValue<Slider>().Value;

            #endregion
            
            Obj_AI_Base.OnIssueOrder += Obj_AI_Base_OnIssueOrder;
            Game.OnUpdate += OnUpdate;
            Game.OnProcessPacket += OnProcessPacket;
            MissileClient.OnCreate += MissileClient_OnCreate;
        }

        private static void OnUpdate(EventArgs args)
        {
            if  (((Menu.Item("ComboMovement").GetValue<bool>() && Menu.Item("ComboKey").GetValue<KeyBind>().Active) ||
                (Menu.Item("MixedMovement").GetValue<bool>() && Menu.Item("MixedKey").GetValue<KeyBind>().Active) ||
                (Menu.Item("LaneClearMovement").GetValue<bool>() && Menu.Item("LaneClearKey").GetValue<KeyBind>().Active) ||
                (Menu.Item("LastHitMovement").GetValue<bool>() && Menu.Item("LastHitKey").GetValue<KeyBind>().Active)) && !Orbwalking.Move)
            {
                Orbwalking.Move = true;
            }
            else
            {
                if (Orbwalking.Move)
                {
                    Orbwalking.Move = false;
                }
            }

            if (GetWhenOnRange)
            {
                if (LastTarget == null || !LastTarget.IsValid || !(LastTarget is Obj_AI_Base) || LastTarget.IsDead)
                {
                    GetWhenOnRange = false;
                }
                else
                {
                    if (Orbwalking.InAutoAttackRange((AttackableUnit)LastTarget))
                    {
                        GetWhenOnRange = false;
                        AttackOnRange = true;
                        LastAttackOrder = Environment.TickCount;
                    }
                }
            }

            if (AttackOrder && Environment.TickCount - LastAttackOrder >= 100 + Game.Ping * 1.5)
            {
                AttackOrder = false;
            }

            if (BufferAttack && (Environment.TickCount - BufferAttackTimer >= 1000 || (BufferTarget == null || BufferTarget.IsDead)))
            {
                BufferAttack = false;
            }

            if (Player.Spellbook.IsCastingSpell && !Player.Spellbook.IsAutoAttacking && (!AttackOrder || JustAttacked))
            {
                AttackOrder = false;
                JustAttacked = false;
                AttackOnRange = false;
            }

            if (LastTarget == null || !LastTarget.IsValid || LastTarget.IsDead)
            {
                if (AttackOrder || JustAttacked)
                {
                    AttackOrder = false;
                    JustAttacked = false;
                    AttackOnRange = false;
                }
            }

            if (CanMove())
            {
                if (JustAttacked)
                {
                    JustAttacked = false;
                    IssueOrder = false;
                    AttackOnRange = false;
                    if (Menu.Item("debug").GetValue<bool>())
                    {
                        Console.WriteLine("OnCastDone - Delay: " + (Environment.TickCount - LastAttack) + "ms");
                    }
                }
                else
                {
                    JustAttacked = false;
                    AttackOrder = false;

                }
                if (!Player.Spellbook.IsChanneling)
                {
                    if (BufferMovement)
                    {
                        Player.IssueOrder(GameObjectOrder.MoveTo, BufferPosition, true);
                    }
                    BufferMovement = false;
                }
                
                if (MissileLaunched)
                {
                    MissileLaunched = false;
                }
            }
            else
            {
                if (Menu.Item("BlockOrder").GetValue<bool>())
                {
                    ShouldBlock = true;
                }
                else
                {
                    ShouldBlock = false;
                }
            }

            if (Orbwalking.CanAttack())
            {
                if (Orbwalking.Attack && BufferAttack && BufferTarget != null && BufferTarget.IsValid && BufferTarget.IsTargetable && Orbwalking.InAutoAttackRange((AttackableUnit)BufferTarget))
                {
                    Player.IssueOrder(GameObjectOrder.AttackUnit, BufferTarget, true);
                }
                BufferAttack = false;
            }
        }


        private static bool CanMove()
        {
            if (Player.CharData.BaseSkinName.Contains("Kalista"))
            {
                return true;
            }
            if (AttackOrder)
            {
                return false;
            }
            if (Player.IsRanged && Menu.Item("missilecheck").GetValue<bool>() && MissileLaunched)
            {
                return true;
            }
            if (LastTarget == null || !LastTarget.IsValid || LastTarget.IsDead)
            {
                return true;
            }
            if (Orbwalking.Move)
            {
                return Orbwalking.CanMove(Menu.Item("ExtraWindup").GetValue<Slider>().Value);
            }
            else
            {
                if (BufferMovement)
                {
                    return Utils.GameTimeTickCount + Game.Ping / 2 >= Orbwalking.LastAATick + Player.AttackCastDelay * 1000f + Menu.Item("ExtraWindup").GetValue<Slider>().Value;
                }
                else
                {
                    return Utils.GameTimeTickCount + Game.Ping / 2 >= Orbwalking.LastAATick + Player.AttackCastDelay * 1000f;
                }
            }
        }

        private static void OnProcessPacket(GamePacketEventArgs args)
        {
            short header = BitConverter.ToInt16(args.PacketData, 0);

            var length = BitConverter.ToString(args.PacketData, 0).Length;

            int networkID = BitConverter.ToInt32(args.PacketData, 2);

            #region AutoFind Headers

            if (Menu.Item("forcefindheaders").GetValue<bool>())
            {
                Menu.Item("headerOnAttack" + GameVersion).SetValue<Slider>(new Slider(0, 0, 400));
                Menu.Item("headerOnMissileHit" + GameVersion).SetValue<Slider>(new Slider(0, 0, 400));
                Packets.Attack.Header = 0;
                Packets.MissileHit.Header = 0;
                Menu.Item("forcefindheaders").SetValue<bool>(false);
            }

            if (Menu.Item("headerOnAttack" + GameVersion).GetValue<Slider>().Value == 0 && length == Packets.Attack.Length && networkID > 0)
            {
                foreach (Obj_AI_Minion obj in ObjectManager.Get<Obj_AI_Minion>().Where(obj => obj.NetworkId == networkID))
                {
                    OnAttackList.Add(header);
                    if (OnAttackList.Count<int>(x => x == header) == 10)
                    {
                        Menu.Item("headerOnAttack" + GameVersion).SetValue<Slider>(new Slider(header, 0, 400));
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

            if (Menu.Item("headerOnMissileHit" + GameVersion).GetValue<Slider>().Value == 0 && length == Packets.MissileHit.Length && networkID > 0)
            {
                foreach (Obj_AI_Minion obj in ObjectManager.Get<Obj_AI_Minion>().Where(obj => obj.IsRanged && obj.NetworkId == networkID))
                {
                    MissileHitList.Add(header);
                    if (MissileHitList.Count<int>(x => x == header) == 10)
                    {
                        Menu.Item("headerOnMissileHit" + GameVersion).SetValue<Slider>(new Slider(header, 0, 400));
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


            if (Menu.Item("headerOnAttack" + GameVersion).GetValue<Slider>().Value == 0 || Menu.Item("headerOnMissileHit" + GameVersion).GetValue<Slider>().Value == 0)
            {
                return;
            }

            #endregion

            if (networkID == NetworkId)
            {
                #region OnAttack

                if (length == Packets.Attack.Length && header == Packets.Attack.Header)
                {
                    if (Menu.Item("debug").GetValue<bool>())
                    {
                        Console.WriteLine("---------------------------------------------------------");
                        Console.Write("OnAttack (server) - Expected Attack Delay: " + (Player.AttackCastDelay * 1000)  + "ms");
                        Console.WriteLine(" - Delay: " + (Environment.TickCount - LastAttackOrder) + "ms");
                    }
                    LastServerResponseDelay = Environment.TickCount - LastAttackOrder;
                    LastAttack = Environment.TickCount;
                    JustAttacked = true;
                    Console.WriteLine("JustAttack Is True");
                    IssueOrder = false;
                    AttackOrder = false;
                }

                if (Player.CharData.BaseSkinName.Contains("Kalista"))
                {
                    return;
                }
                #endregion

                #region Custom Missile Check

                if (Menu.Item("missilecheck").GetValue<bool>() && Player.IsRanged &&
                    length == Packets.MissileHit.Length && header == Packets.MissileHit.Header &&
                    JustAttacked)
                {
                    float missileHitTime = (Environment.TickCount - LastAttack);

                    if (missileHitTime >= Player.AttackCastDelay * 1000 * 0.6)
                    {
                        JustAttacked = false;
                        IssueOrder = false;
                        if (Menu.Item("debug").GetValue<bool>())
                        {
                            Console.WriteLine("OnMissileHit (server) - Delay: " + (Environment.TickCount - LastAttack) + "ms");
                        }
                    
                        if (Orbwalking.Move)
                        {
                            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos, true);
                            LastMoveOrder = Environment.TickCount;
                        }
                        MissileLaunched = true;
                    }
                }
                #endregion
            }
        }

        private static void Obj_AI_Base_OnIssueOrder(Obj_AI_Base sender, GameObjectIssueOrderEventArgs args)
        {
            if (sender == null || !sender.IsValid || !sender.IsMe)
            {
                return;
            }

            if (args.Order == GameObjectOrder.AttackTo || args.Order == GameObjectOrder.AttackUnit ||
                args.Order == GameObjectOrder.AutoAttack || args.IsAttackMove)
            {
                if (Menu.Item("BufferAttack").GetValue<bool>() && !Orbwalking.CanAttack() && CanMove() && Orbwalking.Attack)
                {
                    if (args.Target != null && args.Target.IsValid && args.Target is Obj_AI_Base)
                    {
                        BufferAttack = true;
                        BufferTarget = (Obj_AI_Base)args.Target;
                        BufferAttackTimer = Environment.TickCount;
                    }

                    if (args.IsAttackMove)
                    {
                        if (Menu.Item("BufferMovement").GetValue<bool>() && args.Order == GameObjectOrder.MoveTo)
                        {
                            BufferMovement = true;
                            BufferPosition = args.TargetPosition;
                        } 
                    }
                    args.Process = false;
                    return;
                }


                if ((AttackOrder || JustAttacked) && ShouldBlock)
                {
                    args.Process = false;
                    return;
                }

                AttackOnRange = false;

                if (Orbwalking.Move)
                {
                    if (args.Target != null && args.Target.IsValid && args.Target is Obj_AI_Base && Orbwalking.InAutoAttackRange((AttackableUnit)args.Target))
                    {
                        LastAttackOrder = Environment.TickCount;
                        AttackOrder = true;
                        LastTarget = args.Target;
                        AttackOnRange = true;
                        return;
                    }
                    else
                    {
                        args.Process = false;
                        return;
                    }
                }
                else
                {
                    if (args.Target != null && args.Target.IsValid && args.Target is Obj_AI_Base)
                    {
                        if (Orbwalking.InAutoAttackRange((AttackableUnit)args.Target))
                        {
                            LastAttackOrder = Environment.TickCount;
                            AttackOrder = true;
                            LastTarget = args.Target;
                            AttackOnRange = true;
                            return;
                        }
                        else
                        {
                            LastTarget = args.Target;
                            GetWhenOnRange = true;
                        }
                    }
                }
            }



            if (args.Order == GameObjectOrder.MoveTo && ((AttackOrder && Environment.TickCount - LastAttackOrder > 0) || JustAttacked) &&
                !Player.CharData.BaseSkinName.Contains("Kalista") && ShouldBlock)
            {
                args.Process = false;
                if (Menu.Item("BufferMovement").GetValue<bool>())
                {
                    BufferMovement = true;
                    BufferPosition = args.TargetPosition;
                }
                return;
            }

            if (!IssueOrder && args.Order == GameObjectOrder.MoveTo && Menu.Item("debug").GetValue<bool>() && args.Process)
            {
                Console.WriteLine("Movement - Delay: " + (Environment.TickCount - LastAttack) + "ms");
            }
            IssueOrder = true;
        }

        private static void MissileClient_OnCreate(GameObject sender, EventArgs args)
        {
            if (Menu.Item("debug").GetValue<bool>())
            {
                var missile = sender as MissileClient;
                if (missile != null && missile.SpellCaster.IsMe && Orbwalking.IsAutoAttack(missile.SData.Name) && JustAttacked)
                {
                    Console.WriteLine("OnMissileHit (client) - Delay: " + (Environment.TickCount - LastAttack) + "ms");
                }
            }
        }
    }
}