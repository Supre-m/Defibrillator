using System;
using Exiled.API.Features;
using System.Collections.Generic;
using System.Linq;
using MEC;
using DesfribilatorPlugin;
using UnityEngine;
using Exiled.CustomItems.API.Features;

namespace DesfribilatorPlugin
{

    public class EventHandler
    {
        private readonly Plugin _plugin;
        private Plugin plugin = Plugin.Instance;

        public void OnStart()
        {
            if (plugin.Config.SpawnLczArmory)
                SpawnObjectInSpecificRoom(Room.Get(Exiled.API.Enums.RoomType.LczArmory), "DTR-DFRB-001", new Vector3(0f, 0.5f, 0f));
            if (plugin.Config.SpawnHczArmory)
                SpawnObjectInSpecificRoom(Room.Get(Exiled.API.Enums.RoomType.HczArmory), "DTR-DFRB-001", new Vector3(0f, 5f, 0f));
            if (plugin.Config.SpawnMicroHID)
                SpawnObjectInSpecificRoom(Room.Get(Exiled.API.Enums.RoomType.HczHid), "DTR-DFRB-001", new Vector3(0f, 2f, 0f));
        }

        private void SpawnObjectInSpecificRoom(Room roomName, String itemType, Vector3 positionplus)
        {
            // Encuentra la sala por su nombre
            Room room = roomName;
            float PlusX = positionplus.x;
            float PlusY = positionplus.y;
            float PlusZ = positionplus.z;
            Vector3 Plus = new Vector3(PlusX, PlusY, PlusZ);
            if (room != null)
            {
                // Obtiene una posición aleatoria dentro de la sala
                Vector3 position = new Vector3(room.Position.x + PlusX, room.Position.y + PlusY, room.Position.z + PlusZ);

                // Crea una instancia del objeto que deseas spawnear
                CustomItem item = CustomItem.Get($"{itemType}");
                Timing.CallDelayed(5, () => { CustomItem.Get($"{itemType}").Spawn(position); });
                if (item != null)
                    Log.Info($"¡Item {itemType} Successfully in {roomName}({roomName.Position + Plus}!.");
                else
                    Log.Error("Oops... Something Is Wrong.");
            }
            else
            {
                Log.Error($"Not Found Room with the name {roomName}. Please, check the name carefully.");
            }
        }
    }
}
