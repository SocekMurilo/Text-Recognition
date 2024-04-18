using System;
using System.Collections.Generic;
using System.IO;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;

public class ImageLoader
{
    public List<Image<Gray, byte>> LoadImagesFromFolder(string folder)
    {
        List<Image<Gray, byte>> images = new List<Image<Gray, byte>>();
        foreach (string filename in Directory.GetFiles(folder))
        {
            Image<Gray, byte> img = new Image<Gray, byte>(filename);
            if (img is not null)
            {
                images.Add(img);
            }
        }
        return images;
    }

    public int CountFolders(string directory)
    {
        int folderCount = Directory.GetDirectories(directory).Length;
        return folderCount;
    }

    public Image<Gray, byte> Show(Image<Gray, byte> img)
    {
        ImageViewer viewer = new ImageViewer(img);
        viewer.ShowDialog();
        return img;
    }
}