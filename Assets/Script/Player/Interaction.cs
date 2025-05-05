using UnityEngine;

public class Interaction : MonoBehaviour
{
    public Transform[] roomPositions;  
    private bool playerInTrigger = false;

    void Update()
    {
        if (playerInTrigger && Input.GetKeyDown(KeyCode.W))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null && roomPositions.Length > 0)
            {
                int randomIndex = Random.Range(0, roomPositions.Length);
                player.transform.position = roomPositions[randomIndex].position;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
        }
    }
}
