using System;
using System.IO;
using System.Drawing;
using System.Text;
using System.Text.Json;
using Emgu.CV;
using Emgu.CV.Structure;

public static class ImageTreat
{
    public static string ImgToJson(string path)
    {
        Image image = Image.FromFile(path);

        byte[] imageBytes;
        using (MemoryStream ms = new MemoryStream())
        {
            image.Save(ms, image.RawFormat);
            imageBytes = ms.ToArray();
        }

        // Convert byte array to base64 string
        string base64Image = Convert.ToBase64String(imageBytes);

        // Serialize the image to JSON
        string json = JsonSerializer.Serialize(new { Image = base64Image });
        return json;
    }

    // public static string MatToJson(Mat mat)
    // {
    //     // Convert Mat to byte array
    //     byte[] imageBytes = mat.ToImageBytes();

    //     // Convert byte array to base64 string
    //     string base64Image = Convert.ToBase64String(imageBytes);

    //     // Serialize the image to JSON
    //     string json = JsonSerializer.Serialize(new { Image = base64Image });
    //     return json;
    // }

    // private static byte[] ToImageBytes(this Mat mat)
    // {
    //     // Convert Mat to Bitmap
    //     using (var image = mat.ToImage<Bgr, byte>())
    //     using (var ms = new MemoryStream())
    //     {
    //         // Save the image to a memory stream
    //         image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
    //         return ms.ToArray();
    //     }
    // }

}
