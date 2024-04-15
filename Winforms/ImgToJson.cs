using System;
using System.IO;
using System.Drawing;
using System.Text;
using System.Text.Json;

static class ImageTreat
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

}
