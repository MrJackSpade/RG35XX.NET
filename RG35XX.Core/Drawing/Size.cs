namespace RG35XX.Core.Drawing
{
    public struct Size
    {
        public float Height { get; set; }

        public float Width { get; set; }

        public Size(float width, float height) : this()
        {
            Width = width;
            Height = height;
        }
    }
}