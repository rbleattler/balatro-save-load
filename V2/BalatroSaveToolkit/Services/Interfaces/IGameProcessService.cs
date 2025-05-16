namespace BalatroSaveToolkit.Services.Interfaces
{
    public interface IGameProcessService
    {
        Task<bool> IsGameRunningAsync();
        Task<bool> LaunchGameAsync(string gamePath = null);
        Task<string> GetGameExecutablePath();
        
        // Event for when the game is started or closed
        event EventHandler<bool> GameStatusChanged;
    }
}
