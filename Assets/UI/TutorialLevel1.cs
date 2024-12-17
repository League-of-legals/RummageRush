using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialLevel1 : MonoBehaviour
{
    [SerializeField] public Sprite[] screens;
    [SerializeField] public Sprite screen14;
    [SerializeField] public int currentTutorialScreen;
    public Image image;

    // Start is called before the first frame update
    void Start()
    {
        image = this.GetComponentInChildren<Image>();

       
    }


    public Sprite GetTutorialScreen(int incomingIndex)
    {
        return screens[incomingIndex];
    }

    public void GetScreen14()
    {
        image.sprite = screen14;
    }




    public void CycleThroughTutorialScreens()
    {

        image.sprite = GetTutorialScreen(currentTutorialScreen);
        currentTutorialScreen++;
        Debug.Log($"Cycling thought tutorial. Current tutorial screen {currentTutorialScreen}");


    }
}

