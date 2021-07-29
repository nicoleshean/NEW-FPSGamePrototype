using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.FPS.Gameplay
{
    public class PressurePlate : MonoBehaviour
    {
        // Start is called before the first frame update
        [SerializeField] private GameObject Door;
        [SerializeField] private GameObject Wall;

        //public AudioSource AudioSource;

        bool CloseDoor;
        Collider m_Collider;

        //[Tooltip("Sound that plays when door closes")]
        //public AudioClip DoorSFX;

        MoveDoor m_MoveDoor;

        void Start()
        {
            CloseDoor = false;
            m_MoveDoor = FindObjectOfType<MoveDoor>();
            m_Collider = Door.GetComponent<BoxCollider>();
            m_Collider.enabled = false;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !CloseDoor)
            {
                CloseDoor = true;
                Debug.Log("Entered");
                m_Collider.enabled = true;

                m_MoveDoor.CloseDoor();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Destroy(gameObject);
            }
        }
    }

}
