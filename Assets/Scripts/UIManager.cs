using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject startMenu;
    [SerializeField] private GameObject endMenu;
    [SerializeField] private GameObject transitionMenu;
    [SerializeField] private TextMeshProUGUI scoreText;

    public void Awake()
    {
        startMenu.SetActive(false);
        transitionMenu.SetActive(false);
        endMenu.SetActive(false);
        
    }
    
    public void ShowStartMenu()
    {
        
       startMenu.SetActive(true);
       
    }

    public void HideStartMenu()
    {
        startMenu.SetActive(false);
    }
    
    public void ShowTransitionMenu()
    {
        transitionMenu.SetActive(true);
    }
    
    public void HideTransitionMenu()
    {
        transitionMenu.SetActive(false);
    }

    public void ShowEndMenu(int score)
    {
        endMenu.SetActive(true);
        scoreText.text = $"Score: {score}";
    }
}