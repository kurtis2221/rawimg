using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

class Form1 : Form
{
    const int PAL_LEN = 768;
    const int CON_MRG = 2;

    ImgDisp ct_out;
    NumericUpDown nm_w, nm_h, nm_o;
    CheckBox ch_pal, ch_mon, ch_rgb;
    Button bt_ap, bt_br, bt_lp, bt_sv;
    Bitmap img;

    byte[] data;
    byte[] pal, pal2;
    bool loaded;

    OpenFileDialog ofd;
    SaveFileDialog sfd;

    public Form1()
    {
        Text = "RAW Image Viewer";
        ClientSize = new Size(360, 256);
        StartPosition = FormStartPosition.CenterScreen;
        ct_out = new ImgDisp();
        ct_out.Bounds = new Rectangle(0, 0, 256, 256);
        ct_out.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
        ct_out.BackColor = Color.Black;
        ct_out.BackgroundImageLayout = ImageLayout.Zoom;
        Controls.Add(ct_out);
        nm_w = new NumericUpDown();
        nm_w.Location = new Point(ClientSize.Width - 80, 2);
        nm_w.Size = new Size(64, 24);
        nm_w.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        nm_w.Minimum = 1;
        nm_w.Maximum = 65535;
        nm_w.ValueChanged += nm_ValueChanged;
        Controls.Add(nm_w);
        nm_h = new NumericUpDown();
        nm_h.Location = new Point(nm_w.Location.X, nm_w.Bottom + CON_MRG);
        nm_h.Size = new Size(64, 24);
        nm_h.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        nm_h.Minimum = 1;
        nm_h.Maximum = 65535;
        nm_h.ValueChanged += nm_ValueChanged;
        Controls.Add(nm_h);
        nm_o = new NumericUpDown();
        nm_o.Location = new Point(nm_w.Location.X, nm_h.Bottom + CON_MRG);
        nm_o.Size = new Size(68, 24);
        nm_o.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        nm_o.Minimum = 0;
        nm_o.Maximum = 9999999;
        nm_o.ValueChanged += nm_ValueChanged;
        Controls.Add(nm_o);
        ch_pal = new CheckBox();
        ch_pal.Text = "Palette";
        ch_pal.Location = new Point(nm_w.Location.X, nm_o.Bottom + CON_MRG);
        ch_pal.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        ch_pal.CheckedChanged += ch_pal_CheckedChanged;
        Controls.Add(ch_pal);
        ch_mon = new CheckBox();
        ch_mon.Text = "Mono";
        ch_mon.Location = new Point(nm_w.Location.X, ch_pal.Bottom + CON_MRG);
        ch_mon.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        ch_mon.CheckedChanged += ch_mon_CheckedChanged;
        Controls.Add(ch_mon);
        ch_rgb = new CheckBox();
        ch_rgb.Text = "RGB";
        ch_rgb.Location = new Point(nm_w.Location.X, ch_mon.Bottom + CON_MRG);
        ch_rgb.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        ch_rgb.CheckedChanged += ch_rgb_CheckedChanged;
        Controls.Add(ch_rgb);
        bt_ap = new Button();
        bt_ap.Text = "Apply";
        bt_ap.Location = new Point(nm_w.Location.X, ch_rgb.Bottom + CON_MRG);
        bt_ap.Size = new Size(64, 24);
        bt_ap.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        bt_ap.Click += bt_ap_Click;
        Controls.Add(bt_ap);
        bt_br = new Button();
        bt_br.Text = "Browse";
        bt_br.Location = new Point(nm_w.Location.X, bt_ap.Bottom + CON_MRG);
        bt_br.Size = new Size(64, 24);
        bt_br.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        bt_br.Click += bt_br_Click;
        Controls.Add(bt_br);
        bt_lp = new Button();
        bt_lp.Text = "Palette";
        bt_lp.Location = new Point(nm_w.Location.X, bt_br.Bottom + CON_MRG);
        bt_lp.Size = new Size(64, 24);
        bt_lp.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        bt_lp.Click += bt_lp_Click;
        Controls.Add(bt_lp);
        bt_sv = new Button();
        bt_sv.Text = "Save";
        bt_sv.Location = new Point(nm_w.Location.X, bt_lp.Bottom + CON_MRG);
        bt_sv.Size = new Size(64, 24);
        bt_sv.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        bt_sv.Click += bt_sv_Click;
        Controls.Add(bt_sv);
        ofd = new OpenFileDialog();
        ofd.Filter = "All files|*.*";
        sfd = new SaveFileDialog();
        sfd.Filter = "Bitmap|*.bmp";
        loaded = false;
    }

    void ch_pal_CheckedChanged(object sender, EventArgs e)
    {
        if (ch_pal.Checked)
        {
            ch_mon.Checked = false;
            ch_rgb.Checked = false;
        }
    }

    void ch_mon_CheckedChanged(object sender, EventArgs e)
    {
        if (ch_mon.Checked)
        {
            ch_pal.Checked = false;
            ch_rgb.Checked = false;
        }
    }

    void ch_rgb_CheckedChanged(object sender, EventArgs e)
    {
        if (ch_rgb.Checked)
        {
            ch_pal.Checked = false;
            ch_mon.Checked = false;
        }
    }

    void nm_ValueChanged(object sender, EventArgs e)
    {
        Generate();
    }

    void bt_ap_Click(object sender, EventArgs e)
    {
        Generate();
    }

    void bt_br_Click(object sender, EventArgs e)
    {
        if (ofd.ShowDialog() == DialogResult.OK)
        {
            data = File.ReadAllBytes(ofd.FileName);
            pal = new byte[PAL_LEN];
            for (int i = 0; i < PAL_LEN; i++)
                pal[i] = data[i];
            loaded = true;
            Generate();
        }
    }

    void bt_lp_Click(object sender, EventArgs e)
    {
        if (ch_pal.Enabled)
        {
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                pal2 = File.ReadAllBytes(ofd.FileName);
                bt_lp.Text = "Release";
                ch_pal.Enabled = false;
                ch_mon.Enabled = false;
                ch_rgb.Enabled = false;
            }
        }
        else
        {
            bt_lp.Text = "Palette";
            ch_pal.Enabled = true;
            ch_mon.Enabled = true;
            ch_rgb.Enabled = true;
        }
    }

    void bt_sv_Click(object sender, EventArgs e)
    {
        if (!loaded) return;
        if (sfd.ShowDialog() == DialogResult.OK)
            img.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
    }

    void Generate()
    {
        if (!loaded) return;
        int i, i2;
        img = new Bitmap((int)nm_w.Value, (int)nm_h.Value);
        ct_out.BackgroundImage = img;
        if (!ch_pal.Enabled)
        {
            i2 = 0;
            int offs = (int)nm_o.Value;
            int max = (int)(nm_w.Value * nm_h.Value) + offs;
            if (max > data.Length) max = data.Length;
            for (i = offs; i < max; i++, i2++)
                img.SetPixel(i2 % (int)nm_w.Value, i2 / (int)nm_w.Value, Color.FromArgb(pal2[data[i] * 3], pal2[data[i] * 3 + 1], pal2[data[i] * 3 + 2]));
        }
        else if (ch_mon.Checked)
        {
            int i3 = 0;
            int max = (int)(nm_w.Value * nm_h.Value);
            int offs = (int)nm_o.Value;
            int top = max / 8 + offs;
            if (top > data.Length) top = data.Length;
            if (max > data.Length * 8) max = data.Length * 8;
            for (i = offs; i < top; i++)
                for (i2 = 0; i2 < 8; i2++, i3++)
                {
                    if (i3 == max) goto end;
                    if ((data[i] & (1 << i2)) > 0)
                        img.SetPixel(i3 % (int)nm_w.Value, i3 / (int)nm_w.Value, Color.White);
                }
        }
        else if (ch_pal.Checked)
        {
            i2 = 0;
            int offs = PAL_LEN + (int)nm_o.Value;
            int max = (int)(nm_w.Value * nm_h.Value) + offs;
            if (max > data.Length) max = data.Length;
            for (i = offs; i < max; i++, i2++)
                img.SetPixel(i2 % (int)nm_w.Value, i2 / (int)nm_w.Value, Color.FromArgb(pal[data[i] * 3], pal[data[i] * 3 + 1], pal[data[i] * 3 + 2]));
        }
        else if (ch_rgb.Checked)
        {
            i2 = 0;
            int offs = (int)nm_o.Value * 3;
            int max = (int)(nm_w.Value * nm_h.Value * 3) + offs;
            if (max > data.Length) max = data.Length;
            for (i = offs; i < max - 2; i += 3, i2++)
                img.SetPixel(i2 % (int)nm_w.Value, i2 / (int)nm_w.Value, Color.FromArgb(data[i], data[i + 1], data[i + 2]));
        }
        else
        {
            i2 = 0;
            int offs = (int)nm_o.Value;
            int max = (int)(nm_w.Value * nm_h.Value) + offs;
            if (max > data.Length) max = data.Length;
            for (i = offs; i < max; i++, i2++)
                img.SetPixel(i2 % (int)nm_w.Value, i2 / (int)nm_w.Value, Color.FromArgb(data[i], data[i], data[i]));
        }
    end:
        Invalidate();
    }
}

class ImgDisp : Control
{
    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
        e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
        base.OnPaint(e);
    }
}

static class Program
{
    [STAThread]
    static void Main()
    {
        Application.Run(new Form1());
    }
}