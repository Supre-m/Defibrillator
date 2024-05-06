using Exiled.Events.Commands.Reload;

namespace DesfribilatorPlugin
{
    using Exiled.API.Interfaces;

    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Security.Policy;
    using UnityEngine;
    using Exiled.API.Enums;
    using PlayerRoles;

    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        [Description("Is allowed revive SCPs")]
        public bool SCPRevive { get; set; } = true;

        [Description("Is allowed revive SCPs")]
        public List<RoleTypeId> SCPBlacklisted { get; set; } = new List<RoleTypeId>
        {
            RoleTypeId.Scp079,
            RoleTypeId.Scp106,
            RoleTypeId.Scp173,
        };

        [Description("If Spawn a Desfribilator in LczArmory")]
        public bool SpawnLczArmory { get; set; } = true;


        [Description("If Spawn a Desfribilator in LczArmory")]
        public bool SpawnHczArmory { get; set; } = true;


        [Description("If Spawn a Desfribilator in LczArmory")]
        public bool SpawnMicroHID { get; set; } = true;

        [Description("Desfribilador/Desfribilator Config:")]
        public Desfribilador Desfri { get; private set; } = new Desfribilador();
    }
}
