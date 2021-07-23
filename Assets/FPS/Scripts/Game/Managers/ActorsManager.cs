using System.Collections.Generic;
using UnityEngine;

namespace Unity.FPS.Game
{
    public class ActorsManager : MonoBehaviour
    {
        public List<Actor> Actors { get; private set; }
        public GameObject Player { get; private set; }

        public void SetPlayer(GameObject player) => Player = player; //sets Player game object to the object passed in as argument

        void Awake()
        {
            Actors = new List<Actor>();
        }
    }
}
