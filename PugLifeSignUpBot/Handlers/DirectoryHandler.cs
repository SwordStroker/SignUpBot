using Newtonsoft.Json;
using PugLifeSignUpBot.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PugLifeSignUpBot.Handlers
{
    public static class DirectoryHandler
    {
        public static string raidFolderPathString = Directory.GetCurrentDirectory() + "\\Raids";
        public static string raidTextPathString = raidFolderPathString + "\\Raids.txt";

        public static void DirectoryCheck()
        {
            if (!Directory.Exists(raidFolderPathString))
            {
                Directory.CreateDirectory(raidFolderPathString);
                File.CreateText(raidTextPathString);
                RaidBusiness.raidList = new List<Raid>();
            }
            else
            {
                string[] files = Directory.GetFiles(raidFolderPathString);
                string raids = File.ReadAllText(raidTextPathString);
                RaidBusiness.raidList = JsonConvert.DeserializeObject<List<Raid>>(raids);
            }
        }

        public static void SaveRaids()
        {
            File.WriteAllText(raidTextPathString, JsonConvert.SerializeObject(RaidBusiness.raidList));
        }
    }
}
