
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int score = 0;
    public int totalCoins;

    public string nextSceneName = "Level2";

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // Count all coins in the level
        totalCoins = GameObject.FindGameObjectsWithTag("Coin").Length;
    }

    public void AddScore(int amount)
    {
        score += amount;

        Debug.Log("Coins collected: " + score);

        // Check if all coins are collected
        if (score >= totalCoins)
        {
            Debug.Log("All coins collected! Loading next scene...");

            SceneManager.LoadScene(nextSceneName);
        }
    }
}
