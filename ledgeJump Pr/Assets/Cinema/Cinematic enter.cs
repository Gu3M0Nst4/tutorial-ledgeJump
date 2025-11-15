using Unity.VisualScripting;
using UnityEngine;

public class Cinematicenter : MonoBehaviour
{
    public GameObject thePlayer;
    public GameObject cutsceneCam;
    public AnimationClip animator;

    void OnTriggerEnter(Collider other)
    {
        cutsceneCam.SetActive(true);
        thePlayer.SetActive(false);
        
    }

}
