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
                SpawnObjectInSpecificRoom(Room.Get(Exiled.API.Enums.RoomType.LczArmory), "DTR-DFRB-001", new Vector3(4f, 0.5f, -1.5f));
            if (plugin.Config.SpawnHczArmory)
                SpawnObjectInSpecificRoom(Room.Get(Exiled.API.Enums.RoomType.HczArmory), "DTR-DFRB-001", new Vector3(2.25f, 1.5f, -1.3f));
            if (plugin.Config.SpawnMicroHID)
                SpawnObjectInSpecificRoom(Room.Get(Exiled.API.Enums.RoomType.HczHid), "DTR-DFRB-001", new Vector3(2.5f, 1.05f, -5f));
        }

        private void SpawnObjectInSpecificRoom(Room roomName, String itemType, Vector3 positionplus)
        {
            // Encuentra la sala por su nombre
            Room room = roomName;
            float PlusX = positionplus.x;
            float PlusY = positionplus.y;
            float PlusZ = positionplus.z;

            if (room != null)
            {
                // Obtiene una posición aleatoria dentro de la sala
                Vector3 position = new Vector3(room.Position.x + PlusX, room.Position.y + PlusY, room.Position.z + PlusZ);

                // Crea una instancia del objeto que deseas spawnear
                CustomItem item = CustomItem.Get($"{itemType}");

                // Agrega el objeto al mundo
                item.Spawn(position);

                if (item != null)
                    Log.Info($"¡Item {itemType} Successfully in {roomName}!.");
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
