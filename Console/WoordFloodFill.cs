using System;
using System.Drawing;
using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.Structure;

public class Segmentation
{
    public static unsafe ((int x, int y), (int x, int y)) Find(Mat img, int x, int y)
    {
        byte* ptr = (byte*)img.DataPointer;
        int x0 = x;
        int xf = x;
        int y0 = y;
        int yf = y;
        var queue = new Queue<(int, int)>();
        queue.Enqueue((x + 1, y));
        queue.Enqueue((x - 1, y));
        queue.Enqueue((x, y + 1));
        queue.Enqueue((x, y - 1));

        while (queue.Count > 0)
        {
            (x, y) = queue.Dequeue();

            if (y < 0 || y >= img.Rows)
                continue;

            if (x < 0 || x >= img.Cols)
                continue;

            byte pixel = ptr[img.ElementSize * (x + y * img.Width)];

            if (pixel == 255)
                continue;

            ptr[img.ElementSize * (x + y * img.Width) + 0] = 255;
            // ptr[img.ElementSize * (x + y * img.Width) + 1] = 255;
            // ptr[img.ElementSize * (x + y * img.Width) + 2] = 255;

            x0 = Math.Min(x0, x);
            xf = Math.Max(xf, x);
            y0 = Math.Min(y0, y);
            yf = Math.Max(yf, y);

            queue.Enqueue((x + 1, y));
            queue.Enqueue((x - 1, y));
            queue.Enqueue((x, y + 1));
            queue.Enqueue((x, y - 1));
        }

        return ((x0, y0), (xf, yf));
    }

    // public static long Size(DirectoryInfo dirInfo)
    // {
    //     long total = 0; 

    //     foreach(FileInfo file in dirInfo.GetFiles())
    //         total += file.Length; 

    //     return total;
    // }

    // private void Resize(Bitmap img)
    // {
    //     long size = Size("./Words");
        
    //     for (int i = 0; i < size; i++){
    //         Bitmap bitmap = new Bitmap($"./Words/crop_{i}.png");
    //         using (Graphics g = Graphics.FromImage(bitmap))
    //         {
    //             g.DrawRectangle(new Pen(Brushes.White, 20), new Rectangle(0, 0, bitmap.Width, bitmap.Height));
    //         }
    //         bitmap.Save($"./Words/crop_{i}.png");
    //     }
    // }


    public static unsafe void PerformSegmentation(string imagePath)
    {
        Mat org = CvInvoke.Imread(imagePath, Emgu.CV.CvEnum.ImreadModes.Color);
        Mat img = org.Clone();
        CvInvoke.CvtColor(org, img, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
        CvInvoke.Threshold(img, img, 110, 255, Emgu.CV.CvEnum.ThresholdType.Binary);

        byte* ptr = (byte*)org.DataPointer;
        byte* imptr = (byte*)img.DataPointer;
        
        if (imptr[0] < 120){
            CvInvoke.BitwiseNot(img, img);
        }

        List<Rectangle> rects = new List<Rectangle>();
        for (int y = 0; y < img.Rows; y++)
            for (int x = 0; x < img.Cols; x++)
                if (imptr[img.ElementSize * (x + y * img.Width)] == 0)
                {
                    var rect = Find(img, x, y);
                    rects.Add(new Rectangle(rect.Item1.x, rect.Item1.y, rect.Item2.x - rect.Item1.x, rect.Item2.y - rect.Item1.y));
                }

        foreach(var i in rects)
            Console.WriteLine(i);

        Mat mark = org.Clone();
        List<Mat> croppedImages = new List<Mat>();

        int borderWidth = 120; // Define a largura da borda
        MCvScalar borderColor = new MCvScalar(255, 255, 255); // Escolha a cor da borda (azul no formato BGR)
        Mat borderedImage = new Mat();


        foreach (var rect in rects)
        {
            CvInvoke.Rectangle(mark, rect, new MCvScalar(0, 255, 0), 2);
            Mat croppedImg = new Mat(org, rect);
            // CvInvoke.CopyMakeBorder(croppedImg, borderedImage, borderWidth, borderWidth, borderWidth, borderWidth, BorderType.Constant, borderColor);
            croppedImages.Add(croppedImg.Clone());
        }

        string outputFolder = "Words";

        if (!Directory.Exists(outputFolder))
            Directory.CreateDirectory(outputFolder);

        for (int i = 0; i < croppedImages.Count; i++)
        {
            string output_path = Path.Combine(outputFolder, $"crop_{i}.png");
            croppedImages[i].Save(output_path);
        }
    }
}
