using Moq;
using System.Collections.Concurrent;

namespace BalatroSaveToolkit.Tests.Mocks
{
    /// <summary>
    /// A mock file system for testing file system operations without touching the real file system.
    /// </summary>
    public class FileSystemMock
    {
        private readonly ConcurrentDictionary<string, MockFileInfo> _files;
        private readonly ConcurrentDictionary<string, bool> _directories;
        private readonly Dictionary<Environment.SpecialFolder, string> _specialFolders;
        private readonly string _pathSeparator;

        public char PathSeparatorChar { get; }
        public bool IsCaseSensitive { get; }

        /// <summary>
        /// Creates a new instance of FileSystemMock.
        /// </summary>
        /// <param name="pathSeparator">The path separator character to use.</param>
        /// <param name="isCaseSensitive">Whether path operations should be case-sensitive.</param>
        public FileSystemMock(char pathSeparator = '\\', bool isCaseSensitive = false)
        {
            _files = new ConcurrentDictionary<string, MockFileInfo>(
                isCaseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);
            _directories = new ConcurrentDictionary<string, bool>(
                isCaseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);
            _specialFolders = new Dictionary<Environment.SpecialFolder, string>(
                new EnumComparer<Environment.SpecialFolder>());

            PathSeparatorChar = pathSeparator;
            _pathSeparator = pathSeparator.ToString();
            IsCaseSensitive = isCaseSensitive;
        }

        /// <summary>
        /// Normalizes a path based on the mock file system's settings.
        /// </summary>
        /// <param name="path">The path to normalize.</param>
        /// <returns>The normalized path.</returns>
        public string NormalizePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return path;

            // Replace path separators with the configured one
            path = path.Replace(PathSeparatorChar == '/' ? '\\' : '/', PathSeparatorChar);

            // Remove redundant separators
            while (path.Contains(_pathSeparator + _pathSeparator))
                path = path.Replace(_pathSeparator + _pathSeparator, _pathSeparator);

            // Remove trailing separator if exists and path is not a root path
            if (path.EndsWith(_pathSeparator) && path.Length > 1 && !IsRootPath(path))
                path = path.Substring(0, path.Length - 1);

            return path;
        }

        private bool IsRootPath(string path)
        {
            // For Windows
            if (PathSeparatorChar == '\\')
            {
                return path.Length == 3 && path[1] == ':' && path[2] == '\\';
            }

            // For Unix-like systems
            return path == "/";
        }

        /// <summary>
        /// Sets up a special folder path.
        /// </summary>
        /// <param name="folder">The special folder enum value.</param>
        /// <param name="path">The path for the special folder.</param>
        public void SetupSpecialFolder(Environment.SpecialFolder folder, string path)
        {
            path = NormalizePath(path);
            _specialFolders[folder] = path;

            // Automatically create the directory for this special folder
            CreateDirectory(path);
        }

        /// <summary>
        /// Gets a special folder path.
        /// </summary>
        /// <param name="folder">The special folder enum value.</param>
        /// <returns>The path for the special folder, or null if not defined.</returns>
        public string GetSpecialFolderPath(Environment.SpecialFolder folder)
        {
            return _specialFolders.TryGetValue(folder, out var path) ? path : null;
        }

        /// <summary>
        /// Creates a directory in the mock file system.
        /// </summary>
        /// <param name="path">The directory path to create.</param>
        public void CreateDirectory(string path)
        {
            path = NormalizePath(path);
            _directories[path] = true;

            // Create parent directories
            var parent = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(parent) && parent != path)
                CreateDirectory(parent);
        }

        /// <summary>
        /// Checks if a directory exists in the mock file system.
        /// </summary>
        /// <param name="path">The directory path to check.</param>
        /// <returns>True if the directory exists, false otherwise.</returns>
        public bool DirectoryExists(string path)
        {
            path = NormalizePath(path);
            return _directories.ContainsKey(path);
        }

        /// <summary>
        /// Creates a file in the mock file system.
        /// </summary>
        /// <param name="path">The file path to create.</param>
        /// <param name="content">Optional content for the file.</param>
        public void CreateFile(string path, string content = "")
        {
            path = NormalizePath(path);

            // Create parent directory
            var parent = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(parent))
                CreateDirectory(parent);

            _files[path] = new MockFileInfo(path, content);
        }

        /// <summary>
        /// Checks if a file exists in the mock file system.
        /// </summary>
        /// <param name="path">The file path to check.</param>
        /// <returns>True if the file exists, false otherwise.</returns>
        public bool FileExists(string path)
        {
            path = NormalizePath(path);
            return _files.ContainsKey(path);
        }

        /// <summary>
        /// Gets the content of a file in the mock file system.
        /// </summary>
        /// <param name="path">The file path to read.</param>
        /// <returns>The content of the file, or null if the file doesn't exist.</returns>
        public string GetFileContent(string path)
        {
            path = NormalizePath(path);
            return _files.TryGetValue(path, out var file) ? file.Content : null;
        }

        /// <summary>
        /// Sets up a mock directory structure.
        /// </summary>
        /// <param name="directoryTree">Dictionary with directory paths as keys and list of file names as values.</param>
        /// <param name="rootPath">The root path to prepend to all directories and files.</param>
        public void SetupDirectoryStructure(Dictionary<string, List<string>> directoryTree, string rootPath = "")
        {
            rootPath = NormalizePath(rootPath);

            foreach (var dir in directoryTree)
            {
                var dirPath = string.IsNullOrEmpty(rootPath) ? dir.Key : Path.Combine(rootPath, dir.Key);
                dirPath = NormalizePath(dirPath);
                CreateDirectory(dirPath);

                foreach (var file in dir.Value)
                {
                    var filePath = Path.Combine(dirPath, file);
                    filePath = NormalizePath(filePath);
                    CreateFile(filePath);
                }
            }
        }

        /// <summary>
        /// Sets up Moq to mock file and directory operations using this mock file system.
        /// </summary>
        /// <returns>A File and Directory operation mocking container.</returns>
        public FileSystemMockOperations SetupFileSystemMocks()
        {
            var fileUtilsMock = new Mock<IFileUtils>();
            var dirUtilsMock = new Mock<IDirectoryUtils>();

            // File operations
            fileUtilsMock.Setup(f => f.Exists(It.IsAny<string>()))
                .Returns<string>(path => FileExists(path));

            fileUtilsMock.Setup(f => f.ReadAllText(It.IsAny<string>()))
                .Returns<string>(path => {
                    var content = GetFileContent(path);
                    if (content == null)
                        throw new FileNotFoundException($"File not found: {path}");
                    return content;
                });

            fileUtilsMock.Setup(f => f.WriteAllText(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((path, content) => CreateFile(path, content));

            // Directory operations
            dirUtilsMock.Setup(d => d.Exists(It.IsAny<string>()))
                .Returns<string>(path => DirectoryExists(path));

            dirUtilsMock.Setup(d => d.CreateDirectory(It.IsAny<string>()))
                .Callback<string>(path => CreateDirectory(path))
                .Returns<string>(path => {
                    CreateDirectory(path);
                    return new DirectoryInfo(path);
                });

            dirUtilsMock.Setup(d => d.GetFiles(It.IsAny<string>()))
                .Returns<string>(path => {
                    path = NormalizePath(path);
                    if (!DirectoryExists(path))
                        throw new DirectoryNotFoundException($"Directory not found: {path}");

                    return _files.Keys
                        .Where(f => Path.GetDirectoryName(f) == path)
                        .ToArray();
                });

            dirUtilsMock.Setup(d => d.GetDirectories(It.IsAny<string>()))
                .Returns<string>(path => {
                    path = NormalizePath(path);
                    if (!DirectoryExists(path))
                        throw new DirectoryNotFoundException($"Directory not found: {path}");

                    var prefix = path + PathSeparatorChar;
                    return _directories.Keys
                        .Where(d => d != path && d.StartsWith(prefix) && !d.Substring(prefix.Length).Contains(PathSeparatorChar))
                        .ToArray();
                });

            // Create environment mock
            var environmentMock = new Mock<IEnvironmentUtils>();
            environmentMock.Setup(e => e.GetFolderPath(It.IsAny<Environment.SpecialFolder>()))
                .Returns<Environment.SpecialFolder>(folder => {
                    var path = GetSpecialFolderPath(folder);
                    if (path == null)
                        throw new DirectoryNotFoundException($"Special folder not found: {folder}");
                    return path;
                });

            environmentMock.Setup(e => e.GetEnvironmentVariable(It.IsAny<string>()))
                .Returns<string>(variable => GetEnvironmentVariable(variable));

            environmentMock.Setup(e => e.PathSeparator)
                .Returns(PathSeparatorChar);

            return new FileSystemMockOperations(
                fileUtilsMock.Object,
                dirUtilsMock.Object,
                environmentMock.Object,
                fileUtilsMock,
                dirUtilsMock,
                environmentMock
            );
        }

        // Environment variables storage
        private readonly Dictionary<string, string> _environmentVariables = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Sets an environment variable in the mock.
        /// </summary>
        /// <param name="name">The name of the environment variable.</param>
        /// <param name="value">The value of the environment variable.</param>
        public void SetEnvironmentVariable(string name, string value)
        {
            _environmentVariables[name] = value;
        }

        /// <summary>
        /// Gets an environment variable from the mock.
        /// </summary>
        /// <param name="name">The name of the environment variable.</param>
        /// <returns>The value of the environment variable, or null if not defined.</returns>
        public string GetEnvironmentVariable(string name)
        {
            return _environmentVariables.TryGetValue(name, out var value) ? value : null;
        }
    }

    /// <summary>
    /// Represents a file in the mock file system.
    /// </summary>
    public class MockFileInfo
    {
        public string Path { get; }
        public string Content { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastAccessTime { get; set; }
        public DateTime LastWriteTime { get; set; }

        public MockFileInfo(string path, string content = "")
        {
            Path = path;
            Content = content;
            var now = DateTime.Now;
            CreationTime = now;
            LastAccessTime = now;
            LastWriteTime = now;
        }
    }

    /// <summary>
    /// Contains the mocked file system operations.
    /// </summary>
    public class FileSystemMockOperations
    {
        public IFileUtils File { get; }
        public IDirectoryUtils Directory { get; }
        public IEnvironmentUtils Environment { get; }

        public Mock<IFileUtils> FileMock { get; }
        public Mock<IDirectoryUtils> DirectoryMock { get; }
        public Mock<IEnvironmentUtils> EnvironmentMock { get; }

        public FileSystemMockOperations(
            IFileUtils file,
            IDirectoryUtils directory,
            IEnvironmentUtils environment,
            Mock<IFileUtils> fileMock,
            Mock<IDirectoryUtils> directoryMock,
            Mock<IEnvironmentUtils> environmentMock)
        {
            File = file;
            Directory = directory;
            Environment = environment;
            FileMock = fileMock;
            DirectoryMock = directoryMock;
            EnvironmentMock = environmentMock;
        }
    }

    /// <summary>
    /// Interface for file utilities to be mocked.
    /// </summary>
    public interface IFileUtils
    {
        bool Exists(string path);
        string ReadAllText(string path);
        void WriteAllText(string path, string content);
    }

    /// <summary>
    /// Interface for directory utilities to be mocked.
    /// </summary>
    public interface IDirectoryUtils
    {
        bool Exists(string path);
        DirectoryInfo CreateDirectory(string path);
        string[] GetFiles(string path);
        string[] GetDirectories(string path);
    }

    /// <summary>
    /// Interface for environment utilities to be mocked.
    /// </summary>
    public interface IEnvironmentUtils
    {
        string GetFolderPath(Environment.SpecialFolder folder);
        string GetEnvironmentVariable(string variable);
        char PathSeparator { get; }
    }

    /// <summary>
    /// Comparer for enums to use in dictionaries.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    internal class EnumComparer<TEnum> : IEqualityComparer<TEnum>
        where TEnum : Enum
    {
        public bool Equals(TEnum x, TEnum y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(TEnum obj)
        {
            return obj.GetHashCode();
        }
    }
}
