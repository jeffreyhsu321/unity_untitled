using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Celestials : MonoBehaviour
{
    //Celestials
    public GameObject Sun;
    public GameObject Moon;

    //intensity of sunlight
    public Light sunlight;
    public float maxSunlight;

    //intensity of moonlight
    public Light moonLight;
    public float maxMoonLight;

    //time sin curve
    private float currentTimeOfDay;
    public float dayLength;
    private float currentTimeOfNight;
    public float nightLength;

    //sun height
    private float sunHeight;
    public float sunAmplitude;

    //moon height
    private float moonHeight;
    public float moonAmplitude;

    //lightening and darkening sky
    public GameObject sky;
    public GameObject nightSky;
    public GameObject shader1;
    public GameObject shader2;

    //fade outer glow during sundown
    public GameObject sunGlowOuter;

    //calculates day n light cycle for reference
    public bool isSunHanging;
    public bool isMoonHanging;
    public float hungTimeSun;
    public float hungTimeMoon;
    public bool isPassHorizonSun;
    public bool isPassHorizonMoon;

    public bool isDay;

    private void Start()
    {
        isSunHanging = false;
        isPassHorizonSun = true;
        isDay = true;
    }

    private void Update()
    {
        /*
         moving sun up and down  (up > hang > down > hang > up > hang > etc)
         adjust intensity of directional light
         adjust darkness of sky
         keeps track of day and night
         */

        if (!isSunHanging)
        {
            if (isDay)
            {
                currentTimeOfDay += Time.deltaTime; //if not paused, time goes on
            }
        }
        else
        {
            hungTimeSun += Time.deltaTime; //keeps track of how long spent pausing

            if (hungTimeSun >= (dayLength / 4) && isDay)
            {  //if paused for a quarter of a cycle
                hungTimeSun = 0;
                isSunHanging = false;
                isPassHorizonSun = false;
                isDay = (Sun.transform.position.y > 0) ? true : false;
            }
        }

        if (!isMoonHanging)
        {
            if (!isDay)
            {
                currentTimeOfNight += Time.deltaTime;
            }
        }
        else
        {
            hungTimeMoon += Time.deltaTime;

            if (hungTimeMoon >= (nightLength / 4) && !isDay)
            {
                hungTimeMoon = 0;
                isMoonHanging = false;
                isPassHorizonMoon = false;
                isDay = (Moon.transform.position.y > 0) ? false : true;     //switch isDay boolean when hanging below horizon
            }
        }

        if ((Mathf.Abs(Sun.transform.position.y - 0) < 1) && isDay) {
            isPassHorizonSun = true;
            //isDay = (Sun.transform.position.y > 0) ? true : false;  //keeps track of day or night by whether the sun is above or below the horizon
        }

        if ((Mathf.Abs(Moon.transform.position.y - 0) < 1) && !isDay)
        {
            isPassHorizonMoon = true;
            //isDay = (Moon.transform.position.y < 0) ? true : false;  //keeps track of day or night by whether the sun is above or below the horizon
        }

        //isPassHorizonSun = (Mathf.Abs(Sun.transform.position.y - 0) < 1) ? true: isPassHorizonSun;

        if ((Sun.transform.position.y >= (sunAmplitude - 0.05) || Sun.transform.position.y <= (-sunAmplitude + 0.05)) && isPassHorizonSun)  //when sun reaches top of the sin curve and passed the x-axis 
        {
            isSunHanging = true;
        }

        if ((Moon.transform.position.y >= (moonAmplitude - 0.1) || Moon.transform.position.y <= (-moonAmplitude + 0.1)) && isPassHorizonMoon)
        {
            isMoonHanging = true;
        }

        float dayFrequency = 1 / dayLength;
        sunHeight = Mathf.Sin(currentTimeOfDay * 2 * Mathf.PI * dayFrequency);  //calculate the height of the sun from the time of day (sin curve)
        sunHeight *= sunAmplitude;

        Sun.transform.position = new Vector3(Sun.transform.position.x, sunHeight, Sun.transform.position.z);    //move the sun in accordance to sun height
        sunlight.intensity = sunHeight * (maxSunlight / sunAmplitude);  //adjust the intensity of sunlight(Point) in accordance to sun height

        float nightFrequency = 1 / nightLength;
        moonHeight = -Mathf.Cos(currentTimeOfNight * 2 * Mathf.PI * nightFrequency);
        moonHeight *= moonAmplitude;

        Moon.transform.position = new Vector3(Moon.transform.position.x, moonHeight, Moon.transform.position.z);
        moonLight.intensity = moonHeight * (maxMoonLight / moonAmplitude);

        if (sky != null)
        {
            sky.GetComponent<SpriteRenderer>().color = new Color(((sunHeight + sunAmplitude) * (0.5f / sunAmplitude)), ((sunHeight + sunAmplitude) * (0.5f / sunAmplitude)), ((sunHeight + sunAmplitude) * (0.5f / sunAmplitude)));
        }

        if (nightSky != null)
        {
            nightSky.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, ((moonHeight + moonAmplitude) * (0.5f / moonAmplitude)));
        }

        if (shader1 != null && shader2 != null)
        {
            shader1.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 1- sunlight.intensity);
            shader2.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 1- sunlight.intensity);
        }

        //fade sun's outer pink glow during sundown
        if (Sun.transform.position.y < -(sunAmplitude/2))
        {
            if (sunGlowOuter.GetComponent<SpriteRenderer>().color.a > 0) {
                sunGlowOuter.GetComponent<SpriteRenderer>().color += new Color(0, 0, 0, -dayLength / 2000);
            }
        }
        else {
            if (sunGlowOuter.GetComponent<SpriteRenderer>().color.a < 1)
            {
                sunGlowOuter.GetComponent<SpriteRenderer>().color += new Color(0, 0, 0, dayLength / 20000);
            }
        }

        /*
        if (Input.GetKeyDown(KeyCode.P)) {
            isChangingSunIntensity = true;
            sunTargetIntensity = (sunlight.intensity > 0) ? 0 : 0.6f;
            Debug.Log(sunTargetIntensity);
        }

        if (isChangingSunIntensity) {
            sunlight.intensity = Mathf.SmoothDamp(sunlight.intensity, sunTargetIntensity, ref sunIntensityDamper, 0.5f);
            if (Mathf.Abs(sunlight.intensity - sunTargetIntensity) < 0.05) {
                isChangingSunIntensity = false;
                sunlight.intensity = sunTargetIntensity;
            }
        }
        */
    }
}
