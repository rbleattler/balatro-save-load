using BalatroSaveToolkit.Services.Interfaces;

namespace BalatroSaveToolkit.Services.Implementations
{
    public class MockGameProcessService : IGameProcessService
    {
        private bool _isGameRunning = false;

        public event EventHandler<bool> GameStatusChanged;

        public MockGameProcessService()
        {
        }

        public Task<string> GetGameExecutablePath()
        {
            return Task.FromResult("C:\\Program Files\\Steam\\steamapps\\common\\Balatro\\Balatro.exe");
        }

        public Task<bool> IsGameRunningAsync()
        {
            return Task.FromResult(_isGameRunning);
        }

        public Task<bool> LaunchGameAsync(string gamePath = null)
        {
            _isGameRunning = true;
            GameStatusChanged?.Invoke(this, true);
            return Task.FromResult(true);
        }

        // Mock methods to simulate game starting and stopping
        public void SimulateGameStart()
        {
            _isGameRunning = true;
            GameStatusChanged?.Invoke(this, true);
        }

        public void SimulateGameStop()
        {
            _isGameRunning = false;
            GameStatusChanged?.Invoke(this, false);
        }
    }
}
