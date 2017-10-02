using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates a Timer running the given amount of seconds.
/// </summary>
public class MyTimer { 

    private float max;
    private float counter = 0f;
    public bool Finished = false; // True if the timer is over.
    private bool running = false;

    public MyTimer (float pMax)
    {
        max = pMax;
    }
	
	// Update is called once per frame
	public void Update () {
        if (running && !Finished)
        {
            counter += Time.deltaTime;
            if (counter >= max)
            {
                Finished = true;
            }
        }
	}

    public void timerStart()
    {
        running = true;
        Finished = false;
    }

    public void reset()
    {
        counter = 0;
    }

    public void timerStop()
    {
        running = false;
    }

    public bool hasFinished()
    {
        return Finished && running;
    }
}
