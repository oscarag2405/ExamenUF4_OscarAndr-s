using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI CoinText;
    public static GameManager gameManager;
    public int Coins = 0;
    private void Awake()
    {
        if (GameManager.gameManager != null && GameManager.gameManager != this)
        {
            Destroy(gameObject);
        }
        else
        {
            GameManager.gameManager = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    public void CoinCollected()
    {
        Coins++;  
        CoinText.text = "Coins: " + Coins;
    }
}
