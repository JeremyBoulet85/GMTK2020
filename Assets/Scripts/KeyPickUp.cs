using UnityEngine;

public class KeyPickUp : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Destroy(gameObject);
            GameManager.instance.PickUpKey();
            // play a little ding sound
        }
    }
}
