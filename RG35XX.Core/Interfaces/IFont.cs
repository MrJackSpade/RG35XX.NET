namespace RG35XX.Core.Interfaces
{
    public interface IFont
    {
        public bool FixedWidth { get; }

        Dictionary<int, byte[][]> Data { get; }

        int Height { get; }

        int Width { get; }
    }
}