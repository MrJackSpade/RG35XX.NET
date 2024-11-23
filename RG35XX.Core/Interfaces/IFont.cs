namespace RG35XX.Core.Interfaces
{
    public interface IFont
    {
        Dictionary<int, byte[][]> Data { get; }

        public bool FixedWidth { get; }

        int Height { get; }

        int Width { get; }
    }
}