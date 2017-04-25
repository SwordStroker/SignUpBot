using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PugLifeSignUpBot.Utility
{
    public class Const
    {
        public const string SPEC_TANK = "tank";
        public const string SPEC_MDPS = "mdps";
        public const string SPEC_RDPS = "rdps";
        public const string SPEC_HEALER = "healer";

        public static List<string> Specs = new List<string> { SPEC_HEALER, SPEC_MDPS, SPEC_RDPS, SPEC_TANK };
    }
}
