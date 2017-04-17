using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WowDotNetAPI;
using WowDotNetAPI.Models;

namespace PugLifeSignUpBot.Classes
{
    public static class WowHandler
    {
        private static WowExplorer _wowClient;
        private static string tokenTextPathString = Directory.GetCurrentDirectory() + "\\WowToken.txt";

        public static void Setup()
        {
            //ggruf2rsnfw3vd57nkatxcypat6wm8ux
            string[] token = File.ReadAllLines(tokenTextPathString);
            _wowClient = new WowExplorer(WowDotNetAPI.Region.EU, Locale.en_US, token[0]);
        }

        public static Character GetCharacter(string name,string realm,CharacterOptions options)
        {
            return _wowClient.GetCharacter(realm, name, options);
        }

        public static bool CheckGuldanCurveAchievement(string name,string realm)
        {
            Character character = GetCharacter(name, realm, CharacterOptions.GetAchievements);
            int result = character.Achievements.AchievementsCompleted.FirstOrDefault(r => r == 11195);
            return result != 0 ? true : false;

        }
    }
}
