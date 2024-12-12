using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MuteButton : MonoBehaviour
{
    private Sprite soundMuteImage;
    public Sprite soundUnmuteImage;
    [SerializeField] Button button;
    private bool soundOn = true;


    void Start()
    {
        soundMuteImage = button.image.sprite;
        soundOn = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ButtonClicked()
    {
        if (soundOn)
        {
            button.image.sprite = soundUnmuteImage;
            soundOn = false;
        }
        else
        {
            button.image.sprite = soundMuteImage;
            soundOn =true;
        }
    }

}
