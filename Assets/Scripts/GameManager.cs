
using UnityEngine;

namespace AcidRain
{
    public class GameManager : MonoBehaviour
    {
        private GameObject Drone;
        private GameObject Player;

        private void Awake()
        {
            Drone = Resources.Load<GameObject>("Prefabs/Drone");
            Player = Resources.Load<GameObject>("Prefabs/Body");
        }

        private void Start()
        {
            GameObject player = Instantiate(Player);
            GameObject drone1 = Instantiate(Drone, new Vector3(1f, 1f, -10f), Quaternion.identity);
            player.GetComponent<Entities.Player.Controller>().DroneConnector.Connect(drone1.GetComponent<Entities.Drone.Controller>());
            GameObject drone2 = Instantiate(Drone, new Vector3(1f, 2f, -10f), Quaternion.identity);
            player.GetComponent<Entities.Player.Controller>().DroneConnector.Connect(drone2.GetComponent<Entities.Drone.Controller>());
        }
    }
}