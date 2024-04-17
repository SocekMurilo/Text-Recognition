using System;
using System.CodeDom;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

public partial class MainForm : Form
{
    private Bitmap bitmap = null;
    private Graphics g = null;
    private int frameCount = 0;
    private Timer timer;
    private PictureBox pb = new PictureBox { Dock = DockStyle.Fill };
    private Button btnSaveFrame = new Button { Text = "Salvar Próximo Frame", Dock = DockStyle.Bottom };
    private TextBox textBox = new TextBox { 
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
        // Captura o conteúdo do pb como um bitmap
        Rectangle rect = new Rectangle(0, 0, pb.Width, pb.Height);
        pb.DrawToBitmap(bitmap, rect);
    }

    private async void KeyboardDown(object sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.Escape:
                Application.Exit();
                break;

            case Keys.ControlKey:
                await SaveNextFrameAsync();
                break;

        }
    }


    private void btnSaveFrame_Click(object sender, EventArgs e)
    {
        // Cria um bitmap para conter a captura de tela do formulário
        Bitmap screenshot = new Bitmap(this.Width, 300);

        // Captura a tela inteira do formulário
        this.DrawToBitmap(screenshot, new Rectangle(0, 0, this.Width, 300));

        // Salva o bitmap atual como uma imagem
        string fileName = $"../../../frame_{frameCount}.png";
        screenshot.Save(fileName, ImageFormat.Png);
        frameCount++;
    }

    private async Task SaveNextFrameAsync()
    {
        // Create a bitmap to contain the screen capture of the form
        Bitmap screenshot = new Bitmap(this.Width, 300);

        // Capture the entire form's screen
        this.DrawToBitmap(screenshot, new Rectangle(0, 0, this.Width, 300));

        // Save the current bitmap as an image
        string fileName = $"../../../frame_{0}.png";
        screenshot.Save(fileName, ImageFormat.Png);

        // var resizedImages = Segmentation.PerformSegmentation("frame_0.png");
        var json = ImageTreat.ImgToJson("../../../Words/crop_0.png");
        // MessageBox.Show(json);

        string apiUrl = "http://127.0.0.1:5000/json";
        MessageBox.Show(json);

        using (HttpClient client = new HttpClient())
        {
            // Prepare the content to send in the request (if needed)
            HttpContent content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            try
            {
                // Send the POST request
                HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                // Check if the request was successful (status code 200-299)
                if (response.IsSuccessStatusCode)
                {
                    // Read the response content
                    string responseContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("Response from server:");
                    MessageBox.Show(responseContent);
                }
                else
                {
                    // Display error message if request failed
                    MessageBox.Show($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            catch (HttpRequestException ex)
            {
                // Handle exception if request couldn't be sent
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

    }
}
