/*
██████╗ ██╗   ██╗     █████╗ ██╗     ██████╗ ██╗  ██╗ █████╗  ██████╗  ██████╗ ██████╗ 
██╔══██╗╚██╗ ██╔╝    ██╔══██╗██║     ██╔══██╗██║  ██║██╔══██╗██╔════╝ ██╔═══██╗██╔══██╗
██████╔╝ ╚████╔╝     ███████║██║     ██████╔╝███████║███████║██║  ███╗██║   ██║██║  ██║
██╔══██╗  ╚██╔╝      ██╔══██║██║     ██╔═══╝ ██╔══██║██╔══██║██║   ██║██║   ██║██║  ██║
██████╔╝   ██║       ██║  ██║███████╗██║     ██║  ██║██║  ██║╚██████╔╝╚██████╔╝██████╔╝
╚═════╝    ╚═╝       ╚═╝  ╚═╝╚══════╝╚═╝     ╚═╝  ╚═╝╚═╝  ╚═╝ ╚═════╝  ╚═════╝ ╚═════╝ 
*/

using System;
using System.Linq;


namespace OrbwalkerAddon.Classes
{
    public class Packets
    {
        public static OnAttack Attack;
        public static OnMissileHit MissileHit;

        static Packets()
        {
            try
            {
                Attack = new OnAttack();
                MissileHit = new OnMissileHit();
            }
            catch (Exception)
            {
                //ignored
            }
        }

        public class OnAttack
        {
            public OnAttack(int header = 0, int length = 71)
            {
                Length = length;
                Header = header;
            }
            public int Header { get; set; }
            public int Length { get; set; }
        }

        public class OnMissileHit
        {
            public OnMissileHit(int header = 0, int length = 35)
            {
                Length = length;
                Header = header;
            }
            public int Header { get; set; }
            public int Length { get; set; }
        }
    }
}
