using SharpHook.Data;

namespace Zone_Radio.Utility
{
    internal class ZKeybind
    {
        public required object Parent { get; init; } // Typically ComboBox
        public required KeyCode Code { get; set; }
        public required Func<Task> KeyAction { get; init; } // Async since these make API calls
    }
}
