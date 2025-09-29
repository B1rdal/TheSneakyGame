using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalScript : MonoBehaviour
{

    public string winMessage = "You Win!";
    public GameObject winPanel; // optional UI panel

    bool won;

    void OnTriggerEnter(Collider other)
    {
        if (won) return;
        if (other.CompareTag("Player"))
        {
            won = true;
            Debug.Log(winMessage);
            if (winPanel) winPanel.SetActive(true);
            Time.timeScale = 0.1f; // slow-mo moment
        }
    }
}
