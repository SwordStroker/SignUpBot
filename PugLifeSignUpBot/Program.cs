using Discord;
using Discord.Commands;
using PugLifeSignUpBot.Classes;
using PugLifeSignUpBot.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WowDotNetAPI;
using WowDotNetAPI.Models;

namespace PugLifeSignUpBot
{
    class Program
    {
        private DiscordClient _client;

        static void Main(string[] args)
        {
            new Program().Start();
        }

        public void Start()
        {
            var token = "Mjk3MzEzMTE0OTg1MzMyNzM2.C8Bo9A.CqlPQoVxJATcFs-wvdeQfGHWskg";


            WowHandler.Setup();
            DirectoryHandler.DirectoryCheck();

            _client = new DiscordClient(x =>
            {
                x.AppName = "PugLifeSignBot";
                x.AppUrl = "";
                x.LogLevel = LogSeverity.Verbose;
                x.LogHandler = Log;
            });

            _client.UsingCommands(x =>
            {
                x.PrefixChar = '!';
                x.HelpMode = HelpMode.Public;
                x.AllowMentionPrefix = true;
                x.ErrorHandler = ErrorLog;
            });

            CreateCommands();

            _client.ExecuteAndWait(async () =>
            {
                await _client.Connect(token, TokenType.Bot);
            });


        }

        private void ErrorLog(object sender, CommandErrorEventArgs e)
        {
            Console.WriteLine(string.Format("[{0}] [{1}]", "ERROR", e.ErrorType.ToString()));

            e.Channel.SendMessage(e.ErrorType.ToString());
        }

        public void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(string.Format("[{0}] [{1}] {2}", e.Severity, e.Source, e.Message));
        }

        private void CreateCommands()
        {
            var cService = _client.GetService<CommandService>();

            cService.CreateCommand("addraid")
                .Parameter("raidName", ParameterType.Required)
                .Parameter("date", ParameterType.Required)
                .Parameter("time", ParameterType.Required)
                .Parameter("name", ParameterType.Required)
                .Parameter("realm", ParameterType.Required)
                .Parameter("spec", ParameterType.Required)
                .Parameter("minimumilvl", ParameterType.Required)
                .Parameter("description", ParameterType.Required)
                .Do(async (e) =>
                {
                    RaidBusiness.AddRaid(e.GetArg("raidName"), e.GetArg("date"), e.GetArg("time"), e.GetArg("name"), e.GetArg("realm"), e.GetArg("spec"), int.Parse(e.GetArg("minimumilvl")), e.GetArg("description"), e.User.Id);
                    await e.Channel.SendMessage(string.Format("`Raid {0} has been created successfully by {1}`", e.GetArg("raidName"), e.User.Name));
                    Channel newc = await e.Channel.Server.CreateChannel(e.GetArg("raidName"), ChannelType.Text);
                    await newc.SendMessage("`" + e.GetArg("description") + "`");
                });

            cService.CreateCommand("showallraids")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("`" + RaidBusiness.ShowAllRaids() + "`");
                });

            cService.CreateCommand("signtoraid")
                .Parameter("raidName", ParameterType.Required)
                .Parameter("characterName", ParameterType.Required)
                .Parameter("realm", ParameterType.Required)
                .Parameter("spec", ParameterType.Required)
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage(RaidBusiness.AddRaidMember(e.GetArg("raidName"), e.GetArg("characterName"), e.GetArg("realm"), e.GetArg("spec"), e.User.Id));
                });

            cService.CreateCommand("showraid")
                .Parameter("raidName")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("`" + RaidBusiness.ShowRaid(e.GetArg("raidName")) + "`");
                });

            cService.CreateCommand("cancelsignup")
                .Parameter("raidName", ParameterType.Required)
                .Parameter("name", ParameterType.Required)
                .Parameter("realm", ParameterType.Required)
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage(RaidBusiness.DeleteRaidMember(e.GetArg("raidName"), e.GetArg("name"), e.GetArg("realm"), e.User.Id));
                });

            cService.CreateCommand("changeraiddate")
                .Parameter("raidName", ParameterType.Required)
                .Parameter("newDate", ParameterType.Required)
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage(RaidBusiness.ChangeDateOfRaid(e.GetArg("raidName"), e.GetArg("newDate"), e.User.Id));
                });

            cService.CreateCommand("changeraidtime")
                .Parameter("raidName", ParameterType.Required)
                .Parameter("newTime", ParameterType.Required)
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage(RaidBusiness.ChangeTimeOfRaid(e.GetArg("raidName"), e.GetArg("newTime"), e.User.Id));
                });

            cService.CreateCommand("changeraideqilvl")
                .Parameter("raidName", ParameterType.Required)
                .Parameter("newILvl", ParameterType.Required)
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage(RaidBusiness.ChangeMinimumILvlOfRaid(e.GetArg("raidName"), int.Parse(e.GetArg("newILvl")), e.User.Id));
                });

            cService.CreateCommand("checkguldan")
                .Parameter("characterName", ParameterType.Required)
                .Parameter("realm", ParameterType.Required)
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage(RaidBusiness.CheckGuldanAchi(e.GetArg("characterName"), e.GetArg("realm")));
                });

            cService.CreateCommand("cancelraid")
                .Parameter("raidName", ParameterType.Required)
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage(RaidBusiness.CancelRaid(e.GetArg("raidName"), e.User.Id, e.User.Name));
                });
        }
    }
}
