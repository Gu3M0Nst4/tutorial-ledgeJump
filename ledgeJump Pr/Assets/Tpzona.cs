using UnityEngine;
using System.Collections;

public class Tpzona : MonoBehaviour
{
    public Transform destino; 

    private void OnTriggerEnter(Collider other)
    {
        //Coloca el punto a dónde se tepea el jugador en el inspector de la hitbox
        if (!other.CompareTag("Player")) return;

        CharacterController cc = other.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // El tp del jugador al Empty
        other.transform.position = destino.position;
        other.transform.rotation = destino.rotation;

        if (cc != null) cc.enabled = true;

        StartCoroutine(DisableTriggerTemporarily(0.2f));
    }

    private IEnumerator DisableTriggerTemporarily(float time)
    {
        Collider col = GetComponent<Collider>();
        if (col == null) yield break;
        col.enabled = false;
        yield return new WaitForSeconds(time);
        col.enabled = true;
    }
}
