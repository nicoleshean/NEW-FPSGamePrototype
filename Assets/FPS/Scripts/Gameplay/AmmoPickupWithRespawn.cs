﻿using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.Gameplay
{ 

    public class AmmoPickupWithRespawn : Pickup
    {

        [Tooltip("Weapon those bullets are for")]
        public WeaponController Weapon;

        [Tooltip("Number of bullets the player gets")]
        public int BulletCount = 30;

        public GameObject Pickup;

        public Vector3 RespawnPoint;

        protected override void OnPicked(PlayerCharacterController byPlayer)
        {
            PlayerWeaponsManager playerWeaponsManager = byPlayer.GetComponent<PlayerWeaponsManager>();
            if (playerWeaponsManager)
            {
                WeaponController weapon = playerWeaponsManager.HasWeapon(Weapon);
                if (weapon != null && !weapon.IsAmmoFull) //checks if player is carrying the same weapon and isn't already full on ammo
                {
                    //weapon.AddCarriablePhysicalBullets(BulletCount);

                    weapon.AddAmmo(BulletCount); //adds BulletCount to current ammo

                    AmmoPickupEvent evt = Events.AmmoPickupEvent;
                    evt.Weapon = weapon;
                    EventManager.Broadcast(evt);

                    PlayPickupFeedback();

                    StartCoroutine("RespawnPickup");
                }
            }
        }

        IEnumerator RespawnPickup()
        {
            
            Destroy(gameObject);
            Pickup = (GameObject)Instantiate(Pickup, RespawnPoint, Quaternion.identity);
            yield return null;
        }
    }
}
