namespace NotSoTotalCommanderApp.Messages
{
    public class ReloadFileSystemMessage
    {
        public string CurrentDirectory { get; }

        public ReloadFileSystemMessage(string currentDirectory)
        {
            CurrentDirectory = currentDirectory;
        }
    }
}