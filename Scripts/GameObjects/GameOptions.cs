using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOptions
{
    public float cameraSensitivity = 1f;
    public float audioVolume = 0.5f;
    public int autosaveFrequencyMinutes = 0;

	public delegate void OptionsApplied();
	public static event OptionsApplied OnOptionsApplied;

	public void Apply()
    {
		OnOptionsApplied?.Invoke();
	}
}
