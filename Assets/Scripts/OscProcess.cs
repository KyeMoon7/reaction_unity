using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using extOSC;

public class OscProcess : MonoBehaviour
{
    public GameManager gameManager;
    public extOSC.OSCReceiver oscReceiver;
    public extOSC.OSCTransmitter oscTransmitter;
    // Start is called before the first frame update
    void Start()
    {
        oscReceiver.Bind("/but0", TraiterMessageBut0);
    }

    // Update is called once per frame
    void Update()
    {

    }
    void TraiterMessageBut0(OSCMessage message)
    {
        // Validez qu’il y a bien le nombre attendu d’arguments (1 dans l’exemple) :
        if (message.Values.Count != 1)
        {
            Debug.Log("Le message " + message.Address + " n’a pas le bon nombre d’arguments");
            return; // Quitte la fonction sans exécuter la suite
        }

        // Vérifiez que l’argument est du type attendu (`int` dans l’exemple) :
        if (message.Values[0].Type != OSCValueType.Int)
        {
            Debug.Log("Le premier argument du message " + message.Address + "n’est pas un entier");
            return; // Quitte la fonction sans exécuter la suite
        }

        // Récupérer la valeur de l’argument :
        int valeur = message.Values[0].IntValue;

        // Deboguer
        // Debug.Log("Reçu : " + message.Address + " " + valeur);

        // TRAITER LA VALEUR ICI !
        if (valeur == 1)
        {
            gameManager.Trigger();
        }

    }

}
