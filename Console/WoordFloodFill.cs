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

            byte pixel = ptr[img.ElementSize * x + y * img.Width];

            if (pixel == 255)
                continue;

            ptr[img.ElementSize * x + y * img.Width] = 255;

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

    public static unsafe void PerformSegmentation(string imagePath)
    {
        Mat org = CvInvoke.Imread(imagePath, Emgu.CV.CvEnum.ImreadModes.Color);
        Mat img = new Mat();
        CvInvoke.CvtColor(org, img, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
        CvInvoke.Threshold(img, img, 110, 255, Emgu.CV.CvEnum.ThresholdType.Binary);

        byte* ptr = (byte*)org.DataPointer;
        byte* imptr = (byte*)img.DataPointer;
        
        if (ptr[0] < 120)
        {
            CvInvoke.BitwiseNot(org, org);
        }

        List<Rectangle> rects = new List<Rectangle>();
        for (int i = 0; i < org.Rows; i++)
        {
            for (int k = 0; k < org.Cols; k++)
            {
                if (ptr[org.ElementSize * k + i * org.Width] == 0)
                {
                    var rect = Find(org, k, i);
                    rects.Add(new Rectangle(rect.Item1.x, rect.Item1.y, rect.Item2.x - rect.Item1.x, rect.Item2.y - rect.Item1.y));
                }
            }
        }

        Mat mark = org.Clone();
        List<Mat> croppedImages = new List<Mat>();

        foreach (var rect in rects)
        {
            CvInvoke.Rectangle(mark, rect, new MCvScalar(0, 255, 0), 2);
            Mat croppedImg = new Mat(org, rect);
            croppedImages.Add(croppedImg.Clone());
        }

        string outputFolder = "Words";

        if (!System.IO.Directory.Exists(outputFolder))
        {
            System.IO.Directory.CreateDirectory(outputFolder);
        }

        for (int i = 0; i < croppedImages.Count; i++)
        {
            string output_path = System.IO.Path.Combine(outputFolder, $"crop_{i}.png");
            croppedImages[i].Save(output_path);
        }
    }
}
