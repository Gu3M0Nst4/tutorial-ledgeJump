using UnityEngine;

public class Cinematicenter : MonoBehaviour
{
    public GameObject thePlayer;
    public GameObject cutsceneCam;
    public Animator animator;

    void OnTriggerEnter(Collider other)
    {
        cutsceneCam.SetActive(true);
        thePlayer.SetActive(false);
        animator.Play("Camara Animation");
        
    }

}
