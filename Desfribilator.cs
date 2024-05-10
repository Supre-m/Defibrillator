namespace DesfribilatorPlugin
{
    using System.Collections.Generic;
    using CustomPlayerEffects;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Roles;
    using Exiled.API.Features.Spawn;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs.Player;
    using EPlayer = Exiled.API.Features.Player;
    using MEC;
    using PlayerRoles;
    using UnityEngine;
    using static PlayerList;
    using System.Linq;
    using System.Diagnostics.Eventing.Reader;

    /// <inheritdoc />
    [CustomItem(ItemType.Medkit)]
    public class Desfribilador : CustomItem
    {
        /// <inheritdoc/>
        public override uint Id { get; set; } = 222;

        /// <inheritdoc/>
        public override string Name { get; set; } = "DTR-DFRB-001";

        public override ItemType Type { get; set; } = ItemType.Medkit;

        public override Vector3 Scale { get; set; } = new Vector3(0.6f, 0.6f, 0.6f);

        /// <inheritdoc/>
        public override string Description { get; set; } =
            "Defibrillator, it's just a normal defibrillator, it doesn't have anything different, what you do think is?";

        /// <inheritdoc/>
        public override float Weight { get; set; } = 1f;

        public static readonly Vector3 DefibrillatorRange = new Vector3(3f, 1.5f, 3f);

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; }


        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.UsingItem += OnUsing;
            Exiled.Events.Handlers.Player.ThrowingRequest += OnThrown;
            base.SubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.UsingItem -= OnUsing;
            Exiled.Events.Handlers.Player.ThrowingRequest -= OnThrown;
            base.UnsubscribeEvents();
        }

        private void OnThrown(ThrowingRequestEventArgs ev)
        {
            if (Check(ev.Item) || ev.Item != null)
                return;
            ev.RequestType = ThrowRequest.CancelThrow;
        }

        private void OnUsing(UsingItemEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem) || ev.Item == null)
                return;
            Ragdoll closestRagdoll = null;
            float closestDistance = float.MaxValue;
            foreach (Ragdoll ragdoll in Ragdoll.List)
            {
                float distance = Vector3.Distance(ev.Player.Position, ragdoll.Position);

                if (distance <= DefibrillatorRange.magnitude && distance < closestDistance)
                {
                    closestRagdoll = ragdoll;
                    closestDistance = distance;
                }
            }
            if (Plugin.Instance.EventHandlers.TimeGrace == 0)
            {
                if (Plugin.Instance.EventHandlers.Cooldown == 0)
                {
                    if (closestRagdoll != null)
                    {

                        if (closestRagdoll.Owner.IsDead && !closestRagdoll.Owner.IsHost && closestRagdoll.Owner != null && Player.List.Contains(ev.Player))
                        {
                            if (closestRagdoll.Role.GetTeam() != Team.SCPs)
                            {
                                Player ply = closestRagdoll.Owner;
                                ply.Role.Set(closestRagdoll.Role, RoleSpawnFlags.None);
                                ply.Health = Mathf.RoundToInt(Plugin.Instance.Config.PercentageOfHPWhenRevived / 100 * ply.MaxHealth);
                                ply.EnableEffect(EffectType.Burned, 15, false);
                                ply.EnableEffect(EffectType.AmnesiaVision, 25, false);
                                var revivingEffects = Plugin.Instance.Config.RevivingEffects;
                                foreach (var Effects in revivingEffects)
                                {
                                    var EffectType = Effects.Key;
                                    var TimeEffect = Effects.Value;
                                    ply.EnableEffect(EffectType, TimeEffect);
                                }
                                ply.Position = new Vector3(closestRagdoll.Position.x, closestRagdoll.Position.y + 2, closestRagdoll.Position.z);
                                ply.ShowHint($"{Plugin.Instance.Translation.MessageWhenYouRevive}".Replace("{PlayerName}", ev.Player.DisplayNickname), 4);
                                if (ply.CurrentRoom == Room.Get(RoomType.Pocket))
                                    ply.EnableEffect(EffectType.PocketCorroding, 9999, false);
                                Plugin.Instance.EventHandlers.Cooldown = Plugin.Instance.Config.CooldownTime;
                                Plugin.Instance.Coroutines.Add(Timing.RunCoroutine(Plugin.Instance.EventHandlers.TimerCooldown()));
                                ev.Item.Destroy();
                                closestRagdoll.Destroy();
                                Log.Info($"Player {ply.Nickname} revived successfully.");
                            }
                            else
                            {

                                if (Plugin.Instance.Config.SCPRevive == true)
                                {
                                    foreach (RoleTypeId roleTypeId in Plugin.Instance.Config.SCPBlacklisted)
                                    {
                                        if (ev.Player.Role.Type != roleTypeId)
                                        {
                                            Player ply = closestRagdoll.Owner;
                                            ply.Role.Set(closestRagdoll.Role, RoleSpawnFlags.None);
                                            ply.Health = Mathf.RoundToInt(Plugin.Instance.Config.PercentageOfHPWhenSCPRevived / 100 * ply.MaxHealth);
                                            var revivingEffectsSCPs = Plugin.Instance.Config.RevivingEffectsSCPs;
                                            foreach (var Effects in revivingEffectsSCPs)
                                            {
                                                var EffectType = Effects.Key;
                                                var TimeEffect = Effects.Value;
                                                ply.EnableEffect(EffectType, TimeEffect);
                                            }
                                            ply.Position = new Vector3(closestRagdoll.Position.x, closestRagdoll.Position.y + 2, closestRagdoll.Position.z);
                                            ply.ShowHint($"{Plugin.Instance.Translation.MessageWhenYouReviveSCP}".Replace("{PlayerName}", ev.Player.DisplayNickname), 4);

                                            Plugin.Instance.EventHandlers.Cooldown = Plugin.Instance.Config.CooldownTimeSCP;
                                            Plugin.Instance.Coroutines.Add(Timing.RunCoroutine(Plugin.Instance.EventHandlers.TimerCooldown()));
                                            ev.Item.Destroy();
                                            closestRagdoll.Destroy();
                                            Log.Info($"The SCP {ply.Nickname} has revived.");
                                        }
                                    }
                                }
                                else
                                {
                                    ev.IsAllowed = false;
                                    ev.Player.ShowHint($"{Plugin.Instance.Translation.BlacklistedSCPMessage}", 3);
                                }
                            }
                        }
                        else
                        {
                            ev.IsAllowed = false;
                            ev.Player.ShowHint($"{Plugin.Instance.Translation.MessageWhenARagdollnotavailable}", 3);
                        }
                    }
                    else
                    {
                        ev.IsAllowed = false;
                        ev.Player.ShowHint($"{Plugin.Instance.Translation.hintwhenthereisnoragdollnearby}");
                    }

                }
                else
                {
                    ev.IsAllowed = false;
                    ev.Player.ShowHint($"{Plugin.Instance.Translation.CooldownHint.Replace("{Cooldown}", Plugin.Instance.EventHandlers.Cooldown.ToString())}", 3);
                }
            }
            else
            {
                ev.IsAllowed = false;
                ev.Player.ShowHint($"{Plugin.Instance.Translation.TimeOfGrace.Replace("{Time}", Plugin.Instance.EventHandlers.TimeGrace.ToString())}", 3);
            }
        }
    }
}