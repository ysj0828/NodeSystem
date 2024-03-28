public interface IClickable
{
    bool DisableClick { get; }
    void OnPointerDown();
    void OnPointerUp();
}
