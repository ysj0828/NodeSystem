using UnityEngine;


public interface ISelectable
{
    Vector3[] Handles { get; set; }
    void Select();
    void Deselect();
    void Remove();
}