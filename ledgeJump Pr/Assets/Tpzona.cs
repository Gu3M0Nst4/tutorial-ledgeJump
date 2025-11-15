using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
En tu escena debes tener un objeto con Is Trigger que ser'a tu tp de origen y otro gameobject que sera tu lugar de destino
El objeto de origen tendra el script y en el transform publico pondras el objeto de destino
*/

public class Tpzona : MonoBehaviour
{
    public Transform destino; //El lugar a donde se va a teletransportar el player

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.position = destino.position;
        }
    }
}