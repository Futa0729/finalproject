using System;

[AttributeUsage(AttributeTargets.Class)]
public class HabitatAttribute : Attribute
{
    public string Region { get; }

    public HabitatAttribute(string region)
    {
        Region = region;
    }
}
