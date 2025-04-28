using System;
using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using UserSettings.ServerSpecific;
using Exiled.API.Features.Core.UserSettings;
using Exiled.Loader.Models;
using PlayerRoles;


namespace ZeitvertreibQOL
{
    public class ZeitvertreibQol : Plugin<Config>
    {
        public override string Prefix => "zeitvertreib-qol";
        public override string Name => "zeitvertreib-qol";
        public override string Author => "AlexInABox";
        public override Version Version => new Version(1, 0, 0);

        private static ZeitvertreibQol _singleton;
        public static ZeitvertreibQol Instance => _singleton;

        public override PluginPriority Priority { get; } = PluginPriority.Last;

        public override void OnEnabled()
        {
            _singleton = this;
            Log.Info("zeitvertreib-qol has been enabled!");

            IEnumerable<string> listOfHumanRoles = new List<string> { "ClassD", "Scientist", "FacilityGuard" };
            HeaderSetting header = new HeaderSetting("Zeitvertreib \"Quality of life\" settings :3");
            IEnumerable<SettingBase> settingBases = new SettingBase[]
            {
                header,
                new TwoButtonsSetting(Config.KeybindId, "Mute most C.A.S.S.I.E. announcements?", "Mute", "Don't mute!",
                    true),
                new DropdownSetting(Config.KeybindId, "What's your favourite non-scp role to play?", listOfHumanRoles,
                    0),
            };

            SettingBase.Register(settingBases);
            SettingBase.SendToAll();

            ServerSpecificSettingsSync.ServerOnSettingValueReceived += OnSettingValueReceived;
            Exiled.Events.Handlers.Player.Verified += OnVerified;

            Exiled.Events.Handlers.Cassie.SendingCassieMessage += SendingCassieMessage;
            Exiled.Events.Handlers.Server.ChoosingStartTeamQueue += ChoosingStartTeamQueue;
            Exiled.Events.Handlers.Player.Spawning += Spawning;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Log.Info("zeitvertreib-qol has been disabled!");

            ServerSpecificSettingsSync.ServerOnSettingValueReceived -= OnSettingValueReceived;
            Exiled.Events.Handlers.Player.Verified -= OnVerified;
            Exiled.Events.Handlers.Cassie.SendingCassieMessage -= SendingCassieMessage;

            base.OnDisabled();
        }

        public readonly HashSet<Player> _cassieMuteList = new();

        private void OnSettingValueReceived(ReferenceHub hub, ServerSpecificSettingBase settingBase)
        {
            Log.Debug("Received hotkey!");
            if (!Player.TryGet(hub, out Player player))
                return;

            if (settingBase is SSTwoButtonsSetting twoButtonsSetting &&
                twoButtonsSetting.SettingId == ZeitvertreibQol.Instance.Config.KeybindId)
            {
                if (twoButtonsSetting.SyncIsA)
                {
                    _cassieMuteList.Add(player);
                }
                else
                {
                    _cassieMuteList.Remove(player);
                }
            }

            if (settingBase is SSDropdownSetting dropdownSetting &&
                dropdownSetting.SettingId == ZeitvertreibQol.Instance.Config.KeybindId)
            {
                Log.Info(dropdownSetting.SyncSelectionText);
            }
        }

        private static void OnVerified(Exiled.Events.EventArgs.Player.VerifiedEventArgs ev)
        {
            ServerSpecificSettingsSync.SendToPlayer(ev.Player.ReferenceHub);
        }

        private void SendingCassieMessage(Exiled.Events.EventArgs.Cassie.SendingCassieMessageEventArgs ev)
        {
            ev.IsAllowed = false;
            foreach (Player player in Player.List)
            {
                if (!_cassieMuteList.Contains(player))
                {
                    player.PlayCassieAnnouncement(ev.Words, ev.MakeHold, ev.MakeNoise, true);
                }
            }
        }


        private void ChoosingStartTeamQueue(Exiled.Events.EventArgs.Server.ChoosingStartTeamQueueEventArgs ev)
        {
        }

        private void Spawning(Exiled.Events.EventArgs.Player.SpawningEventArgs ev)
        {
        }
    }
}