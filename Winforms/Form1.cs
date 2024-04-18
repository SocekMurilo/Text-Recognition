using System;
using System.CodeDom;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text;
using Emgu.CV;
using System.Collections.Generic;
using Emgu.CV.Structure;


public partial class MainForm : Form
{
    private Bitmap bitmap = null;
    private Graphics g = null;
    private int frameCount = 0;
    private string resp = "DIGITE ALGO ALI EM CIMA E APERTE CTRL";
    private Timer timer;
    private PictureBox pb = new PictureBox { Dock = DockStyle.Fill };
    private Button btnSaveFrame = new Button { Text = "Salvar Próximo Frame", Dock = DockStyle.Bottom };

    private TextBox textBox = new TextBox
    {
        Dock = DockStyle.Top,
        Multiline = true,
        Height = 300,
        Font = new Font("Arial", 32)
    };

    public MainForm()
    {
        WindowState = FormWindowState.Maximized;
        FormBorderStyle = FormBorderStyle.None;

        bitmap = new Bitmap(pb.Width, pb.Height);
        pb.Image = bitmap;

        timer = new Timer { Interval = 16 };

        Load += Form_Load;
        timer.Tick += Timer_Tick;

        this.KeyPreview = true;
        this.KeyDown += KeyboardDown;

        Controls.Add(this.pb);
        Controls.Add(this.btnSaveFrame);
        Controls.Add(this.textBox);

        this.btnSaveFrame.Click += btnSaveFrame_Click;

        Text = "Teixto";
    }

    private void Form_Load(object sender, EventArgs e)
    {
        this.bitmap = new Bitmap(pb.Width, pb.Height);

        this.g = Graphics.FromImage(this.bitmap);
        this.g.InterpolationMode = InterpolationMode.NearestNeighbor;
        this.g.Clear(Color.White);
        this.pb.Image = bitmap;
        this.timer.Start();
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        g.Clear(Color.White);
        g.DrawString(resp, SystemFonts.DefaultFont, Brushes.Black, new PointF(10, 105));
        // Captura o conteúdo do pb como um bitmap
        Rectangle rect = new Rectangle(0, 0, pb.Width, pb.Height);
        pb.DrawToBitmap(bitmap, rect);

        pb.Refresh();
    }

    private void KeyboardDown(object sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.Escape:
                Application.Exit();
                break;

            case Keys.ControlKey:
                SaveNextFrame();
                break;

        }
    }


    private void btnSaveFrame_Click(object sender, EventArgs e)
    {
        Bitmap screenshot = new Bitmap(this.Width, 300);

        this.DrawToBitmap(screenshot, new Rectangle(0, 0, this.Width, 300));

        string fileName = $"../../../frame_0.png";
        screenshot.Save(fileName, ImageFormat.Png);
        Segmentation.PerformSegmentation(screenshot);

    }

    private async void SaveNextFrame()
    {
        Bitmap screenshot = new Bitmap(this.Width, 300);

        this.DrawToBitmap(screenshot, new Rectangle(0, 0, this.Width, 300));

        string fileName = $"frame_{0}.png";
        screenshot.Save(fileName, ImageFormat.Png);

        // var resizedImages = Segmentation.PerformSegmentation("frame_0.png");
        var KK = Segmentation.PerformSegmentation(screenshot);

        // ImageLoader imageLoader = new ImageLoader();
        // int folders = imageLoader.CountFolders("Words");
        // List<List<Image<Gray, byte>>> data = new List<List<Image<Gray, byte>>>();

        // for (int i = 0; i < folders; i++)
        // {
        //     List<Image<Gray, byte>> imgs = imageLoader.LoadImagesFromFolder($"Words/Word_{i}/");
        //     data.Add(imgs);
        // }
        
        // imageLoader.Show(data[0][0]);
        // MessageBox.Show(data[0].Count.ToString());


        var json = ImageTreat.ImgToJson("../../../frame_0.png");
        // // MessageBox.Show(json);

        string apiUrl = "http://127.0.0.1:5000/json/";
        // MessageBox.Show(json);

        try
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(apiUrl)
                };

                using HttpResponseMessage response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Response from server:\n" + responseContent);
                    resp = responseContent;
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

    }
}
