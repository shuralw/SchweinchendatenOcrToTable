using System.Drawing.Imaging;

public class ImageCompressor
{

    public string CompressImage(string sourcePath, long quality = 75L, int maxSizeInBytes = 4 * 1024 * 1024)
    {
        using (Image image = Image.FromFile(sourcePath))
        {
            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;

            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, quality);
            myEncoderParameters.Param[0] = myEncoderParameter;

            var memoryStream = new MemoryStream();
            image.Save(memoryStream, jpgEncoder, myEncoderParameters);

            while (memoryStream.Length > maxSizeInBytes && quality > 5)
            {
                quality -= 5;
                myEncoderParameter = new EncoderParameter(myEncoder, quality);
                myEncoderParameters.Param[0] = myEncoderParameter;

                memoryStream.SetLength(0); // Reset the stream before saving again
                image.Save(memoryStream, jpgEncoder, myEncoderParameters);
            }

            var compressedImagePath = Path.Combine(Path.GetDirectoryName(sourcePath), "compressed_" + DateTime.Now.ToString("_dd-MM-yy HH-mm-ss" + Path.GetFileName(sourcePath) ));
            File.WriteAllBytes(compressedImagePath, memoryStream.ToArray());

            return compressedImagePath;
        }
    }

    private ImageCodecInfo GetEncoder(ImageFormat format)
    {
        ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
        foreach (ImageCodecInfo codec in codecs)
        {
            if (codec.FormatID == format.Guid)
            {
                return codec;
            }
        }
        return null;
    }
}