using System;
using System.IO;
using System.Drawing;
using System.Text;
using System.Text.Json;
using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.Structure;
using System.Collections.Generic;

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

    public static string MatToJson(Mat mat)
    {
        int width = mat.Width;
        int height = mat.Height;

        byte[] bytes = new byte[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                byte value = mat.GetRawData(y, x)[0]; // Assuming single-channel
                bytes[y * width + x] = value;
            }
        }

        // Convert byte array to base64 string
        string base64Image = Convert.ToBase64String(bytes);

        var a = new string[] {
            base64Image, base64Image
        };

        // Serialize the image to JSON
        string json = JsonSerializer.Serialize(new 
        { 
            Image = a 
        });
        return json;
    }

    // public static string MatToJson(List<List<Mat>> words)
    // {

    //     // Convert byte array to base64 string
    //     string base64Image = Convert.ToBase64String(bytes);

    //     // Serialize the image to JSON
    //     string json = JsonSerializer.Serialize(new 
    //     { 
    //         Image = base64Image 
    //     });
    //     return json;
    // }

    private static byte[] MatToByte(Mat mat)
    {
        int width = mat.Width;
        int height = mat.Height;

        byte[] bytes = new byte[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                byte value = mat.GetRawData(y, x)[0]; // Assuming single-channel
                bytes[y * width + x] = value;
            }
        }

        return bytes;
    }
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
