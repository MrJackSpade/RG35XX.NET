namespace RG35XX.Core.Interfaces
{
    public interface IFont
    {
        Dictionary<int, byte[][]> Data { get; }

        int Height { get; }

        int Width { get; }
    }
}