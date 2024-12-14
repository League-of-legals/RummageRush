using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialLevel1 : MonoBehaviour
{
    [SerializeField] public Sprite[] screens;
    [SerializeField] public int currentTutorialScreen;
    private Image image;

    // Start is called before the first frame update
    void Start()
    {
        image = this.GetComponentInChildren<Image>();

    }


    public Sprite GetTutorialScreen(int incomingIndex)
    {
        return screens[incomingIndex];
    }

    

    
    public void CycleThroughTutorialScreens()
    {
       
       image.sprite = GetTutorialScreen(currentTutorialScreen);
       currentTutorialScreen++;
        Debug.Log($"Cycling thought tutorial. Current tutorial screen {currentTutorialScreen}");

    }
}

