using Tesseract;

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

        private void btnProcess_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_imagePath))
            {
                MessageBox.Show("Please upload an image first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
            {
                using (var img = Pix.LoadFromFile(_imagePath))
                {
                    using (var page = engine.Process(img))
                    {
                        string text = page.GetText();

                        // You might need more sophisticated logic for parsing table and fractional numbers
                        string[] lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                        using (StreamWriter sw = new StreamWriter("output.csv"))
                        {
                            foreach (var line in lines)
                            {
                                sw.WriteLine(line.Replace("\t", ","));
                            }
                        }
                    }
                }
            }

            MessageBox.Show("Processing complete!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}