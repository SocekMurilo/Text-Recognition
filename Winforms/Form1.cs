using System;
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
    private TextBox textBox = new TextBox { Dock = DockStyle.Top, Multiline = true, Height = 200 };

    public MainForm()
    {
        bitmap = new Bitmap(pb.Width, pb.Height);
        pb.Image = bitmap;

        timer = new Timer { Interval = 16 };

        this.Load += Form_Load;

        timer.Tick += Timer_Tick;
        WindowState = FormWindowState.Maximized;
        FormBorderStyle = FormBorderStyle.None;

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

    private void btnSaveFrame_Click(object sender, EventArgs e)
    {
        // Cria um bitmap para conter a captura de tela do formulário
        Bitmap screenshot = new Bitmap(this.Width, this.Height);

        // Captura a tela inteira do formulário
        this.DrawToBitmap(screenshot, new Rectangle(0, 0, this.Width, this.Height));

        // Salva o bitmap atual como uma imagem
        string fileName = $"frame_{frameCount}.png";
        screenshot.Save(fileName, ImageFormat.Png);
        frameCount++;
    }
}
