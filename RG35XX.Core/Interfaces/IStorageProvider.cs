namespace RG35XX.Core.Interfaces
{
    public interface IStorageProvider
    {
        public string MMC { get; }

        public string ROOT { get; }

        public string SD { get; }

        void Initialize();
    }
}