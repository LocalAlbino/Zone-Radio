using SharpHook;
using SharpHook.Data;

namespace Zone_Radio.Utility
{
    internal class ZHook : IDisposable
    {
        public event EventHandler UpdateUiElements;
        private Dictionary<KeyCode, List<ZKeybind>> _keybinds; // Multiple actions per key is okay
        private SimpleGlobalHook _hook;

        public ZHook()
        {
            _keybinds = new Dictionary<KeyCode, List<ZKeybind>>();
            _hook = new SimpleGlobalHook();
            _hook.KeyPressed += OnKeyPressed;
        }

        public void Start()
        {
            _hook.Run();
        }

        public void Dispose()
        {
            _hook.Dispose();
        }

        private void TogglePda(EventArgs e)
        {
            UpdateUiElements?.Invoke(this, e);
        }

        public void UpdateKeybind(object parent, KeyCode keyCode)
        {
            foreach (var pair in _keybinds)
            {
            }
        }

        public void OnKeyPressed(object? sender, KeyboardHookEventArgs e)
        {
            var keyCode = e.Data.KeyCode;
            if (keyCode == KeyCode.VcUndefined) return;

            List<ZKeybind> binds;
            _keybinds.TryGetValue(keyCode, out binds);

            if (binds == null || binds.Count == 0) return;

            // Invoke all actions associated with this key
            foreach (var bind in binds)
            {
                bind.KeyAction?.Invoke();
            }
        }
    }
}
