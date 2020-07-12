using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ZoneType
{
    Bathroom,
    Alley,
    Cafetaria
}

public class zone : MonoBehaviour
{
    public ZoneType zoneType;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().EnteringZone(zoneType);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().ExitingZone(zoneType);
        }
    }
}
