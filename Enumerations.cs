using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MtgLifeCounter
{
    public enum CustomCounterType
    {
        Poison,
        Experience,
        Energy,
        LifeHistory,
    }

    public enum BackGroundColors
    {
        Red,
        Blue,
        Green,
        Purple,
        Yellow,
        White,
        Pink,
        Cyan,
    }

    public enum Gametypes
    {
        Current,
        MultiPlayer,
        Commander,
        Brawl,

    }

    public enum PlayerID
    {
        Unknown,
        Player1,
        Player2,
        Player3,
        Player4,
    }


    public class CounterTypeHelper
    {

        public static string CounterTypeImage(CustomCounterType type)
        {
            switch (type)
            {
                case CustomCounterType.Poison:
                    return "assets/countersymbols/poison.png";
                case CustomCounterType.Experience:
                    return "assets/countersymbols/xp.png";
                default:
                case CustomCounterType.Energy:
                    return "assets/countersymbols/energy.png";
            }
        }
    }
}
