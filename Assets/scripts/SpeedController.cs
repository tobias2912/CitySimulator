using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedController : MonoBehaviour
{
    private GameController _gameController;

    // Start is called before the first frame update
    void Start()
    {
        _gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    public void SetSpeed(int speed)
    {
            Debug.Log("set timescale " + speed);
            Time.timeScale = speed;
    }

    public void setSpeed1(bool isEnabled)
    {
        if (isEnabled)
        {
            SetSpeed(1);
            //get child gameobject with name toggle10 and set isOn to false
            transform.Find("Toggle10").GetComponent<UnityEngine.UI.Toggle>().isOn = false;
            transform.Find("Toggle100").GetComponent<UnityEngine.UI.Toggle>().isOn = false;
        }
    }
    public void setSpeed10(bool isEnabled)
    {
        if (isEnabled)
        {
            SetSpeed(10);
            transform.Find("Toggle1").GetComponent<UnityEngine.UI.Toggle>().isOn = false;
            transform.Find("Toggle100").GetComponent<UnityEngine.UI.Toggle>().isOn = false;
        }
    }
    public void setSpeed100(bool isEnabled)
    {
        if (isEnabled)
        {
            SetSpeed(100);
            transform.Find("Toggle1").GetComponent<UnityEngine.UI.Toggle>().isOn = false;
            transform.Find("Toggle10").GetComponent<UnityEngine.UI.Toggle>().isOn = false;
        }
    }
}