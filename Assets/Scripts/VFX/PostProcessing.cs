using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class PostProcessing : MonoBehaviour

{
    public float lensDistortionIntensity;
    public float chromaticAbberationIntensity;
    public float bloomIntensity;

    public Volume volume;
    LensDistortion lensDistortion;
    ChromaticAberration chromaticAbberation;
    Bloom bloom;

    [SerializeField] private float effectTimer;

    [SerializeField] GameSettingsSO gameSettings;
    [SerializeField] RandomEvent randomEvent;
    void Start()
    {
        volume = GetComponent<Volume>();
        volume.profile.TryGet<LensDistortion>(out lensDistortion);
        volume.profile.TryGet<ChromaticAberration>(out chromaticAbberation);
        volume.profile.TryGet<Bloom>(out bloom);

        if (SceneManager.GetActiveScene().name == "Level_1")
        {


            if (lensDistortion && chromaticAbberation && bloom)
            {
                lensDistortion.active = false;
                chromaticAbberation.active = false;
                bloom.active = false;
            }
            lensDistortionIntensity = 0;
            chromaticAbberationIntensity = 0;
            bloomIntensity = 0;
        }




    }

    void Update()
    {
        if (gameSettings.previousGameState == GameStates.inRandomEvent &&
            gameSettings.currentGameState == GameStates.inGame &&
            randomEvent.item != null &&
            randomEvent.itemName == "Lavalamp")
        {
            LavalampEffect();
        }

        else if (gameSettings.previousGameState == GameStates.inRandomEvent &&
            gameSettings.currentGameState == GameStates.inGame &&
            randomEvent.item != null &&
            randomEvent.itemName == "Moldy brownie")
        {
            ModlyBrownieEffect();
        }
    }

    private void ModlyBrownieEffect()
    {   
        lensDistortion.active = true;
        chromaticAbberation.active = true;

        lensDistortion.intensity.Override(lensDistortionIntensity);
        chromaticAbberation.intensity.Override(chromaticAbberationIntensity);

        effectTimer += Time.deltaTime;

        if (effectTimer > 0f && effectTimer <= 4f)
        {
            lensDistortionIntensity += 0.0005f;
            chromaticAbberationIntensity += 0.0005f;
        }
        else if (effectTimer > 4f && effectTimer <= 8f)
        {
            lensDistortionIntensity -= 0.0005f;
            chromaticAbberationIntensity -= 0.0005f;
        }
        else if (effectTimer > 8f)
        {
            effectTimer = 0f;
            lensDistortion.active = false;
            chromaticAbberation.active = false;
        }

    }

    private void LavalampEffect()
    {          
        bloom.active = true;
        bloom.intensity.Override(bloomIntensity);
        effectTimer += Time.deltaTime;

        if (effectTimer > 0f && effectTimer <= 4f)
        {
  
            bloomIntensity += 0.001f;
        }

        else if (effectTimer > 4f && effectTimer <= 8f)
        {
            bloomIntensity -= 0.001f;

        }
        else if (effectTimer > 8f)
        {
            effectTimer = 0f;
            if (SceneManager.GetActiveScene().name == "Level_1")
            { bloom.active = false; }
        }

    }
}
