using System.Drawing.Drawing2D;

namespace UMMF.Masaustu;

internal static class ArayuzTemasi
{
    internal static readonly Color ArkaPlan = Color.FromArgb(17, 20, 27);
    internal static readonly Color YanPanel = Color.FromArgb(22, 26, 35);
    internal static readonly Color Kart = Color.FromArgb(29, 34, 45);
    internal static readonly Color KartVurgu = Color.FromArgb(36, 43, 57);
    internal static readonly Color Cizgi = Color.FromArgb(58, 66, 84);
    internal static readonly Color Metin = Color.FromArgb(239, 242, 248);
    internal static readonly Color SolukMetin = Color.FromArgb(156, 166, 186);
    internal static readonly Color Vurgu = Color.FromArgb(91, 124, 250);
    internal static readonly Color VurguKoyu = Color.FromArgb(66, 91, 199);
    internal static readonly Color Basarili = Color.FromArgb(62, 194, 126);
    internal static readonly Color Uyari = Color.FromArgb(244, 183, 64);
    internal static readonly Color Hata = Color.FromArgb(233, 91, 103);

    internal static Button Dugme(string metin, EventHandler? tiklama = null, bool vurgu = false)
    {
        var dugme = new Button
        {
            Text = metin,
            AutoSize = false,
            Height = 40,
            FlatStyle = FlatStyle.Flat,
            BackColor = vurgu ? Vurgu : KartVurgu,
            ForeColor = Metin,
            Font = new Font("Segoe UI Semibold", 9.5f),
            Cursor = Cursors.Hand,
            TabStop = false,
            Padding = new Padding(12, 0, 12, 0)
        };
        dugme.FlatAppearance.BorderSize = 0;
        dugme.FlatAppearance.MouseOverBackColor = vurgu ? VurguKoyu : Color.FromArgb(47, 55, 72);
        dugme.FlatAppearance.MouseDownBackColor = vurgu ? Color.FromArgb(55, 76, 168) : Color.FromArgb(39, 46, 61);
        if (tiklama is not null)
        {
            dugme.Click += tiklama;
        }

        return dugme;
    }

    internal static Label Etiket(string metin, float boyut = 10f, bool kalin = false, Color? renk = null)
    {
        return new Label
        {
            Text = metin,
            AutoSize = true,
            ForeColor = renk ?? Metin,
            Font = new Font("Segoe UI", boyut, kalin ? FontStyle.Bold : FontStyle.Regular),
            BackColor = Color.Transparent
        };
    }

    internal static Panel KartPaneli(int dolgu = 18)
    {
        var panel = new YuvarlatilmisPanel
        {
            BackColor = Kart,
            Padding = new Padding(dolgu),
            Margin = new Padding(0, 0, 14, 14)
        };
        return panel;
    }

    internal static TextBox MetinKutusu(string yerTutucu = "")
    {
        return new TextBox
        {
            PlaceholderText = yerTutucu,
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.FromArgb(22, 27, 36),
            ForeColor = Metin,
            Font = new Font("Segoe UI", 10f),
            Height = 36
        };
    }

    internal static ComboBox SecimKutusu()
    {
        return new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(22, 27, 36),
            ForeColor = Metin,
            Font = new Font("Segoe UI", 10f),
            Height = 36
        };
    }
}

internal sealed class YuvarlatilmisPanel : Panel
{
    private const int YariCap = 14;

    protected override void OnResize(EventArgs eventargs)
    {
        base.OnResize(eventargs);
        Region?.Dispose();
        Region = new Region(YuvarlatilmisYol(ClientRectangle, YariCap));
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using var yol = YuvarlatilmisYol(new Rectangle(0, 0, Width - 1, Height - 1), YariCap);
        using var kalem = new Pen(ArayuzTemasi.Cizgi, 1f);
        e.Graphics.DrawPath(kalem, yol);
        base.OnPaint(e);
    }

    private static GraphicsPath YuvarlatilmisYol(Rectangle dikdortgen, int yariCap)
    {
        var cap = yariCap * 2;
        var yol = new GraphicsPath();
        yol.AddArc(dikdortgen.X, dikdortgen.Y, cap, cap, 180, 90);
        yol.AddArc(dikdortgen.Right - cap, dikdortgen.Y, cap, cap, 270, 90);
        yol.AddArc(dikdortgen.Right - cap, dikdortgen.Bottom - cap, cap, cap, 0, 90);
        yol.AddArc(dikdortgen.X, dikdortgen.Bottom - cap, cap, cap, 90, 90);
        yol.CloseFigure();
        return yol;
    }
}
