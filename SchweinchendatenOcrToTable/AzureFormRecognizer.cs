using Azure;
using Azure.AI.FormRecognizer;
using Azure.AI.FormRecognizer.Models;

public class AzureFormRecognizer
{
    private string endpoint = "https://form-recognizer-instance-wessel.cognitiveservices.azure.com/";
    private string apiKey = "04a646d8c0d745b7aac4698faebc981c";

    public async Task ProcessImage(string imagePath)
    {
        var recognizedNumbers = await ExtractNumbersFromImage(imagePath);
    }

    private async Task<IEnumerable<string>> ExtractNumbersFromImage(string documentPath)
    {
        var numbersList = new List<string>();

        var credential = new AzureKeyCredential(apiKey);
        var client = new FormRecognizerClient(new Uri(endpoint), credential);

        using (FileStream stream = new FileStream(documentPath, FileMode.Open))
        {
            RecognizeContentOptions options = new RecognizeContentOptions()
            {
                ContentType = FormContentType.Jpeg
            };
            Response<FormPageCollection> response = await client.StartRecognizeContentAsync(stream, options).WaitForCompletionAsync();
            FormPageCollection pages = response.Value;

            foreach (var page in pages)
            {
                using (StreamWriter sw = new StreamWriter("output.csv"))
                {
                    if (page.Tables.Count > 0)
                    {
                        var table = page.Tables[0];

                        for (int rowIndex = 0; rowIndex < table.RowCount; rowIndex++)
                        {
                            var rowCells = new List<string>();

                            for (int colIndex = 0; colIndex < table.ColumnCount; colIndex++)
                            {
                                var cell = table.Cells.FirstOrDefault(c => c.RowIndex == rowIndex && c.ColumnIndex == colIndex);
                                if (cell != null)
                                {
                                    rowCells.Add(cell.Text.Replace(".", ","));
                                }
                                else
                                {
                                    rowCells.Add(string.Empty);  // for missing cells
                                }
                            }

                            sw.WriteLine(string.Join(";", rowCells));
                        }
                    }
                }

            }
        }
        return numbersList;
    }

    private bool IsNumeric(string input)
    {
        return input.All(char.IsDigit); // Basic check, modify if you want to allow decimals etc.
    }
}

