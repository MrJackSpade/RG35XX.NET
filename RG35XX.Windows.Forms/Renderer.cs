
namespace RG35XX.Windows.Forms
{
    public partial class Renderer : Form
    {
        private PictureBox? pictureBox;
        public Renderer()
        {
            this.InitializeComponent();

            FormBorderStyle = FormBorderStyle.FixedToolWindow;
        }

        protected override void OnClosed(EventArgs e)
        {
            System.Environment.Exit(0);
        }
        public void Initialize(int width, int height)
        {
            ClientSize = new Size()
            {
                Width = width,
                Height = height
            };

            pictureBox = new()
            {
                Width = width,
                Height = height,
                Dock = DockStyle.Fill
            };

            Bitmap bitmap = new(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bitmap.SetPixel(x, y, Color.Black);
                }
            }

            pictureBox.Image = bitmap;

            Controls.Add(pictureBox);
        }

        public void SetImage(Bitmap bitmap)
        {
            if (pictureBox is null)
            {
                throw new System.InvalidOperationException();
            }

            pictureBox.Image?.Dispose();

            pictureBox.Image = bitmap;
        }

        private void Renderer_KeyDown(object sender, KeyEventArgs e)
        {
            KeyBus.OnKeyDown(e);
        }

        private void Renderer_KeyUp(object sender, KeyEventArgs e)
        {
            KeyBus.OnKeyUp(e);
        }
    }
}