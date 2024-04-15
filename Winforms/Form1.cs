using System;
using System.CodeDom;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
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
        Rectangle rect = new Rectangle(0, 0, pb.Width, 300);
        pb.DrawToBitmap(bitmap, rect);
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
        // Cria um bitmap para conter a captura de tela do formulário
        Bitmap screenshot = new Bitmap(this.Width, 300);

        // Captura a tela inteira do formulário
        this.DrawToBitmap(screenshot, new Rectangle(0, 0, this.Width, 300));

        // Salva o bitmap atual como uma imagem
        string fileName = $"frame_{frameCount}.png";
        screenshot.Save(fileName, ImageFormat.Png);
        frameCount++;
    }

    private void SaveNextFrame()
    {
        // Create a bitmap to contain the screen capture of the form
        Bitmap screenshot = new Bitmap(this.Width, 300);

        // Capture the entire form's screen
        this.DrawToBitmap(screenshot, new Rectangle(0, 0, this.Width, 300));

        // Save the current bitmap as an image
        string fileName = $"frame_{frameCount}.png";
        screenshot.Save(fileName, ImageFormat.Png);
        frameCount++;
    }
}
