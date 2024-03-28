using UnityEngine;

public interface IObject
{
    string ID { get; }
    Color objectColour { get; set; }
    int Priority { get; }
    void Remove();
}
