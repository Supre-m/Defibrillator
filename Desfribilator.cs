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

            if (closestRagdoll != null)
            {
                if (closestRagdoll.Owner.IsDead && !closestRagdoll.Owner.IsHost && closestRagdoll.Owner != null)
                {
                    if (closestRagdoll.Role.GetTeam() != Team.SCPs)
                    {
                        Player ply = closestRagdoll.Owner;
                        ply.Role.Set(closestRagdoll.Role, RoleSpawnFlags.None);
                        ply.Health = Mathf.RoundToInt(ply.MaxHealth * 0.75f);
                        ply.EnableEffect(EffectType.Burned, 15, false);
                        ply.EnableEffect(EffectType.AmnesiaVision, 25, false);
                        ply.Position = new Vector3(closestRagdoll.Position.x, closestRagdoll.Position.y + 2, closestRagdoll.Position.z);
                        ply.ShowHint($"{Plugin.Instance.Translation.MessageWhenYouRevive}".Replace("{PlayerName}", ev.Player.DisplayNickname), 4);
                        if (ply.CurrentRoom == Room.Get(RoomType.Pocket))
                            ply.EnableEffect(EffectType.PocketCorroding, 9999, false);
                        ev.Item.Destroy();
                        closestRagdoll.Destroy();
                        Log.Info($"Player {ply.Nickname} revived successfully.");
                    }
                    else
                    {
                         
                        if (closestRagdoll.Role != RoleTypeId.Scp173 && Plugin.Instance.Config.SCPRevive == true)
                        {
                            foreach (RoleTypeId roleTypeId in Plugin.Instance.Config.SCPBlacklisted)
                            {
                                if (ev.Player.Role.Type != roleTypeId)
                                {
                                    Player ply = closestRagdoll.Owner;
                                    ply.Role.Set(closestRagdoll.Role, RoleSpawnFlags.None);
                                    ply.Health = Mathf.RoundToInt(ply.MaxHealth * 0.3f);
                                    ply.EnableEffect(EffectType.Burned, 15, false);
                                    ply.Position = new Vector3(closestRagdoll.Position.x, closestRagdoll.Position.y + 2, closestRagdoll.Position.z);
                                    ply.ShowHint($"{Plugin.Instance.Translation.MessageWhenYouReviveSCP}".Replace("{PlayerName}", ev.Player.DisplayNickname), 4);
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
                ev.Player.ShowHint($"{Plugin.Instance.Translation.hintwhenthereisnoragdollnearby}");
                ev.IsAllowed = false;
            }

        }
    }
}