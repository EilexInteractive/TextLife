using System;
using System.Collections.Generic;
using Godot;

public class WorldEventSave
{
    public bool IsOpen = true;
    public string OriginCountryName;
    public string InvolvedCountryName;
    public List<LifeEventSave> Events = new List<LifeEventSave>();
}