using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;


namespace Unity.FPS.Gameplay
{

    public class RespawnPickup : MonoBehaviour
    {
        public GameObject PickupPrefab;

        //public GameObject SpawnParent;

        public Transform PickupSpawnPoint;

        bool firstPickup;
        private int m_SpawnedPickup;

        // Start is called before the first frame update
        void Start()
        {
            firstPickup = true;
        }

        // Update is called once per frame
        void Update()
        {
            m_SpawnedPickup = PickupSpawnPoint.childCount;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                firstPickup = false;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && !firstPickup && m_SpawnedPickup == 0)
            {
                Instantiate(PickupPrefab, PickupSpawnPoint);
                firstPickup = false;
            }
        }
    }

}