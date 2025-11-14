using UnityEngine;

public class Cinematicenter : MonoBehaviour
{
    public GameObject thePlayer;
    public GameObject cutsceneCam;

    void OnTriggerEnter(Collider other)
    {
        cutsceneCam.SetActive(true);
        thePlayer.SetActive(false);
    }

}
