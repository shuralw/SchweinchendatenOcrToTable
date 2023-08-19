namespace SchweinchendatenOcrToTable
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private string _imagePath = string.Empty;

        private void btnUpload_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "Image Files|*.jpg;*.png;*.bmp", ValidateNames = true, Multiselect = false })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _imagePath = ofd.FileName;
                }
            }
        }

        private async void btnProcess_Click(object sender, EventArgs e)
        {
            AzureFormRecognizer azureFormRecognizer = new AzureFormRecognizer();
            ImageCompressor imageCompressor = new ImageCompressor();
            var compressedImagePath = imageCompressor.CompressImage(_imagePath);

            await azureFormRecognizer.ProcessImage(compressedImagePath);

            MessageBox.Show("Processing complete!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}