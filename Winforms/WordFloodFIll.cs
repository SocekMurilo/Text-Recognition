using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Linq;
// using Emgu.CV.Bitmap;

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

            ptr[img.ElementSize * (x + y * img.Width)] = 255;

            x0 = Math.Min(x0, x);
            xf = Math.Max(xf, x);
            y0 = Math.Min(y0, y);
            yf = Math.Max(yf, y);

            queue.Enqueue((x + 1, y));
            queue.Enqueue((x - 1, y));
            queue.Enqueue((x, y + 1));
            queue.Enqueue((x, y - 1));
        }

        return ((x0 - 1, y0 - 1), (xf + 1, yf + 1));
    }
    public static unsafe void PerformSegmentation(string imagePath)
    {
        Mat org = CvInvoke.Imread(imagePath, ImreadModes.Color);
        Mat img = org.Clone();
        CvInvoke.CvtColor(org, img, ColorConversion.Bgr2Gray);
        CvInvoke.Threshold(img, img, 110, 255, ThresholdType.Binary);

        byte* ptr = (byte*)org.DataPointer;
        byte* imptr = (byte*)img.DataPointer;

        if (imptr[0] < 120)
            CvInvoke.BitwiseNot(img, img);

        List<Rectangle> rects = new List<Rectangle>();
        for (int y = 0; y < img.Rows; y++)
            for (int x = 0; x < img.Cols; x++)
                if (imptr[img.ElementSize * (x + y * img.Width)] == 0)
                {
                    var rect = Find(img, x, y);
                    rects.Add(new Rectangle(rect.Item1.x, rect.Item1.y, rect.Item2.x - rect.Item1.x, rect.Item2.y - rect.Item1.y));
                }

        foreach (var i in rects)
            Console.WriteLine(i);

        Mat mark = org.Clone();
        List<Mat> resizedImages = new List<Mat>();

        foreach (var rect in rects)
        {
            CvInvoke.Rectangle(mark, rect, new MCvScalar(0, 255, 0), 2);
            Mat croppedImg = new Mat(org, rect);

            CvInvoke.Resize(croppedImg, croppedImg, new Size(640, 480), 0, 0, Inter.Linear);

            int desiredWidth = 1200;
            int desiredHeight = 900;

            Mat resizedImg = new Mat(desiredHeight, desiredWidth, croppedImg.Depth, croppedImg.NumberOfChannels);

            if (imptr[0] < 120)
                resizedImg.SetTo(new MCvScalar(255, 255, 255));
            else
                resizedImg.SetTo(new MCvScalar(255, 255, 255));

            int x = (desiredWidth - croppedImg.Width) / 2;
            int y = (desiredHeight - croppedImg.Height) / 2;

            Mat roi = new Mat(resizedImg, new Rectangle(x, y, croppedImg.Width, croppedImg.Height));
            croppedImg.CopyTo(roi);

            resizedImages.Add(resizedImg.Clone());
        }

        string outputFolder = "../../../Words";

        if (!Directory.Exists(outputFolder))
            Directory.CreateDirectory(outputFolder);

        for (int i = 0; i < resizedImages.Count; i++)
        {
            string output_path = Path.Combine(outputFolder, $"crop_{i}.png");
            resizedImages[i].Save(output_path);
        }

    }

    public static unsafe void PerformSegmentationn(Bitmap bitmap)
    {
        Mat org = bitmap.ToMat();
        Mat img = org.Clone();
        CvInvoke.CvtColor(org, img, ColorConversion.Bgr2Gray);
        CvInvoke.Threshold(img, img, 110, 255, ThresholdType.Binary);

        byte* ptr = (byte*)org.DataPointer;
        byte* imptr = (byte*)img.DataPointer;

        if (imptr[0] < 120)
            CvInvoke.BitwiseNot(img, img);

        List<Rectangle> rects = new List<Rectangle>();
        for (int y = 0; y < img.Rows; y++)
            for (int x = 0; x < img.Cols; x++)
                if (imptr[img.ElementSize * (x + y * img.Width)] == 0)
                {
                    var rect = Find(img, x, y);
                    rects.Add(new Rectangle(rect.Item1.x, rect.Item1.y, rect.Item2.x - rect.Item1.x, rect.Item2.y - rect.Item1.y));
                }

        foreach (var i in rects)
            Console.WriteLine(i);

        Mat mark = org.Clone();
        List<Mat> resizedImages = new List<Mat>();
        List<List<Mat>> words = new List<List<Mat>>();

        int xPrevios = 0;

        foreach (var rect in rects)
        {
            CvInvoke.Rectangle(mark, rect, new MCvScalar(0, 255, 0), 2);
            Mat croppedImg = new Mat(org, rect);
            
            CvInvoke.Resize(croppedImg, croppedImg, new Size(640, 480), 0, 0, Inter.Linear);

            int width = 1200;
            int height = 900;

            Mat resizedImg = new Mat(height, width, croppedImg.Depth, croppedImg.NumberOfChannels);

            resizedImg.SetTo(new MCvScalar(255, 255, 255));

            int x = (int)Math.Floor((width - croppedImg.Width) / 2.0);
            int y = (int)Math.Floor((height - croppedImg.Height) / 2.0);

            Mat roi = new Mat(resizedImg, new Rectangle(x, y, croppedImg.Width, croppedImg.Height));
            croppedImg.CopyTo(roi);

            if (rect.X - xPrevios < 36)
                resizedImages.Add(resizedImg.Clone());
            else
            {
                words.Add(resizedImages.ToList());
                resizedImages.Clear();
                resizedImages.Add(resizedImg.Clone());
            }

            xPrevios = rect.X;
        }
        words.Add(resizedImages.ToList());

        string outputFolder = "../../../Words";

        if (!Directory.Exists(outputFolder))
            Directory.CreateDirectory(outputFolder);

        for (int i = 0; i < words.Count; i++)
        {
            string wordFolder = Path.Combine(outputFolder, $"Word_{i}");
            Directory.CreateDirectory(wordFolder);

            for (int j = 0; j < words[i].Count; j++)
            {
                string output_path = Path.Combine(wordFolder, $"crop_{j}.jpg");
                words[i][j].Save(output_path);
            }
        }

    }

    public static unsafe List<List<Mat>> PerformSegmentation(Bitmap bitmap)
    {
        Mat org = bitmap.ToMat();
        Mat img = org.Clone();
        CvInvoke.CvtColor(org, img, ColorConversion.Bgr2Gray);
        CvInvoke.Threshold(img, img, 110, 255, ThresholdType.Binary);

        byte* ptr = (byte*)org.DataPointer;
        byte* imptr = (byte*)img.DataPointer;

        if (imptr[0] < 120)
            CvInvoke.BitwiseNot(img, img);

        List<Rectangle> rects = new List<Rectangle>();
        for (int y = 0; y < img.Rows; y++)
            for (int x = 0; x < img.Cols; x++)
                if (imptr[img.ElementSize * (x + y * img.Width)] == 0)
                {
                    var rect = Find(img, x, y);
                    rects.Add(new Rectangle(rect.Item1.x, rect.Item1.y, rect.Item2.x - rect.Item1.x, rect.Item2.y - rect.Item1.y));
                }

        Mat mark = org.Clone();
        List<Mat> resizedImages = new List<Mat>();
        List<List<Mat>> words = new List<List<Mat>>();

        int xPrevios = 0;

        foreach (var rect in rects)
        {
            CvInvoke.Rectangle(mark, rect, new MCvScalar(0, 255, 0), 2);
            Mat croppedImg = new Mat(org, rect);

            CvInvoke.Resize(croppedImg, croppedImg, new Size(640, 480), 0, 0, Inter.Linear);

            int width = 1200;
            int height = 900;

            Mat resizedImg = new Mat(height, width, croppedImg.Depth, croppedImg.NumberOfChannels);

            resizedImg.SetTo(new MCvScalar(255, 255, 255));

            int x = (int)Math.Floor((width - croppedImg.Width) / 2.0);
            int y = (int)Math.Floor((height - croppedImg.Height) / 2.0);

            Mat roi = new Mat(resizedImg, new Rectangle(x, y, croppedImg.Width, croppedImg.Height));
            croppedImg.CopyTo(roi);

            if (rect.X - xPrevios < 37)
                resizedImages.Add(resizedImg.Clone());
            else
            {
                words.Add(resizedImages.ToList());
                resizedImages.Clear();
                resizedImages.Add(resizedImg.Clone());
            }

            xPrevios = rect.X;
        }
        words.Add(resizedImages.ToList());

        string outputFolder = "../../../Words";

        if (!Directory.Exists(outputFolder))
            Directory.CreateDirectory(outputFolder);

        for (int i = 0; i < words.Count; i++)
        {
            string wordFolder = Path.Combine(outputFolder, $"Word_{i}");
            Directory.CreateDirectory(wordFolder);

            for (int j = 0; j < words[i].Count; j++)
            {
                string output_path = Path.Combine(wordFolder, $"crop_{j}.jpg");
                words[i][j].Save(output_path);
            }
        }

        return words;
    }
}
