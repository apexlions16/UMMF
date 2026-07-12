using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json;
using UMMF.Cekirdek;
using UMMF.KomutSatiri;
using UMMF.Sozlesmeler;

namespace UMMF.Masaustu;

internal sealed class AnaPencere : Form
{
    private readonly Dictionary<string, Panel> _sayfalar = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Button> _gezintiDugmeleri = new(StringComparer.OrdinalIgnoreCase);
    private readonly BepInEx5KurulumYoneticisi _kurulumYoneticisi = new();
    private readonly Panel _icerikPaneli = new();
    private readonly Label _sayfaBasligi = ArayuzTemasi.Etiket("Ana Sayfa", 18f, true);
    private readonly Label _sayfaAciklamasi = ArayuzTemasi.Etiket("Unity oyunlarını analiz edin ve UMMF'yi yönetin.", 9.5f, false, ArayuzTemasi.SolukMetin);
    private readonly Label _altDurum = ArayuzTemasi.Etiket("Hazır", 9f, false, ArayuzTemasi.SolukMetin);
    private readonly Label _secilenOyunEtiketi = ArayuzTemasi.Etiket("Oyun seçilmedi", 9f, false, ArayuzTemasi.SolukMetin);
    private readonly RichTextBox _gunluk = new();
    private readonly TextBox _oyunYoluKutusu = ArayuzTemasi.MetinKutusu("Unity oyununun ana klasörünü seçin");
    private readonly Dictionary<string, Label> _ortamDegerleri = new(StringComparer.OrdinalIgnoreCase);
    private readonly Label _kurulumSonucu = ArayuzTemasi.Etiket("Henüz bir işlem yapılmadı.", 10f, false, ArayuzTemasi.SolukMetin);
    private readonly DataGridView _uyumlulukTablosu = new();
    private readonly TextBox _katkiOyunAdi = ArayuzTemasi.MetinKutusu("Oyun adı");
    private readonly TextBox _katkiOyunSurumu = ArayuzTemasi.MetinKutusu("Test edilen oyun sürümü");
    private readonly TextBox _katkiNotlari = ArayuzTemasi.MetinKutusu("Test notları ve yeniden üretme adımları");
    private readonly ComboBox _katkiDoku = ArayuzTemasi.SecimKutusu();
    private readonly ComboBox _katkiSes = ArayuzTemasi.SecimKutusu();
    private readonly ComboBox _katkiAltyazi = ArayuzTemasi.SecimKutusu();
    private readonly ComboBox _katkiSeslendirme = ArayuzTemasi.SecimKutusu();

    private OyunOrtami? _sonOrtam;
    private string? _secilenOyunYolu;
    private string _aktifSayfa = "ana";

    internal AnaPencere()
    {
        Text = $"UMMF {Program.Surum} — Evrensel Medya Modlama Çerçevesi";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(1080, 700);
        Size = new Size(1320, 820);
        BackColor = ArayuzTemasi.ArkaPlan;
        ForeColor = ArayuzTemasi.Metin;
        Font = new Font("Segoe UI", 9f);
        AutoScaleMode = AutoScaleMode.Dpi;
        DoubleBuffered = true;

        AnaKabuguKur();
        SayfalariKur();
        SayfayiGoster("ana");
        UyumlulukVerisiniYukle();
        GunlukYaz($"UMMF {Program.Surum} masaüstü arayüzü hazır.");
    }

    internal string AraryuzDumanTestiniCalistir()
    {
        var gerekli = new[] { "ana", "oyun", "uyumluluk", "katki", "gunluk" };
        var eksik = gerekli.Where(ad => !_sayfalar.ContainsKey(ad)).ToArray();
        if (eksik.Length > 0)
        {
            return "BAŞARISIZ\r\nEksik sayfalar: " + string.Join(", ", eksik);
        }

        if (_uyumlulukTablosu.Columns.Count < 8)
        {
            return "BAŞARISIZ\r\nUyumluluk tablosu sütunları oluşturulamadı.";
        }

        return $"UMMF {Program.Surum}\r\n" +
               "BAŞARILI\r\n" +
               "Ana Sayfa\r\n" +
               "Oyun ve Kurulum\r\n" +
               "Uyumluluk\r\n" +
               "Topluluk Katkısı\r\n" +
               "Günlük\r\n";
    }

    private void AnaKabuguKur()
    {
        var solPanel = new Panel
        {
            Dock = DockStyle.Left,
            Width = 238,
            BackColor = ArayuzTemasi.YanPanel,
            Padding = new Padding(16, 20, 16, 16)
        };

        var marka = new Panel { Dock = DockStyle.Top, Height = 92, BackColor = Color.Transparent };
        var logo = new Label
        {
            Text = "U",
            TextAlign = ContentAlignment.MiddleCenter,
            Size = new Size(48, 48),
            Location = new Point(2, 4),
            BackColor = ArayuzTemasi.Vurgu,
            ForeColor = Color.White,
            Font = new Font("Segoe UI Black", 20f),
            AutoSize = false
        };
        var ad = ArayuzTemasi.Etiket("UMMF", 17f, true);
        ad.Location = new Point(62, 5);
        var surum = ArayuzTemasi.Etiket(Program.Surum, 8.5f, false, ArayuzTemasi.SolukMetin);
        surum.Location = new Point(64, 38);
        marka.Controls.AddRange(new Control[] { logo, ad, surum });
        solPanel.Controls.Add(marka);

        var gezinti = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 310,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            BackColor = Color.Transparent,
            Padding = new Padding(0, 8, 0, 0)
        };
        GezintiDugmesiEkle(gezinti, "ana", "⌂   Ana Sayfa");
        GezintiDugmesiEkle(gezinti, "oyun", "▣   Oyun ve Kurulum");
        GezintiDugmesiEkle(gezinti, "uyumluluk", "✓   Uyumluluk");
        GezintiDugmesiEkle(gezinti, "katki", "＋   Topluluk Katkısı");
        GezintiDugmesiEkle(gezinti, "gunluk", "≡   Günlük");
        solPanel.Controls.Add(gezinti);

        var solAlt = new Panel { Dock = DockStyle.Bottom, Height = 116, BackColor = Color.Transparent };
        var github = ArayuzTemasi.Dugme("GitHub projesini aç", (_, _) => AdresiAc("https://github.com/apexlions16/UMMF"));
        github.Dock = DockStyle.Top;
        var surumler = ArayuzTemasi.Dugme("Latest Release", (_, _) => AdresiAc("https://github.com/apexlions16/UMMF/releases/latest"));
        surumler.Dock = DockStyle.Top;
        surumler.Margin = new Padding(0, 8, 0, 0);
        solAlt.Controls.Add(surumler);
        solAlt.Controls.Add(github);
        solPanel.Controls.Add(solAlt);

        var ustPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 86,
            BackColor = ArayuzTemasi.ArkaPlan,
            Padding = new Padding(28, 18, 28, 10)
        };
        _sayfaBasligi.Location = new Point(28, 17);
        _sayfaAciklamasi.Location = new Point(30, 50);
        var surumRozeti = new Label
        {
            Text = "  ÖNİZLEME  ",
            AutoSize = true,
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            BackColor = Color.FromArgb(38, 72, 59),
            ForeColor = ArayuzTemasi.Basarili,
            Font = new Font("Segoe UI Semibold", 8.5f),
            Padding = new Padding(4),
            Location = new Point(Width - 380, 24)
        };
        ustPanel.Controls.AddRange(new Control[] { _sayfaBasligi, _sayfaAciklamasi, surumRozeti });

        _icerikPaneli.Dock = DockStyle.Fill;
        _icerikPaneli.BackColor = ArayuzTemasi.ArkaPlan;
        _icerikPaneli.Padding = new Padding(28, 8, 28, 12);

        var altPanel = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 38,
            BackColor = ArayuzTemasi.YanPanel,
            Padding = new Padding(18, 9, 18, 0)
        };
        _altDurum.Dock = DockStyle.Left;
        _secilenOyunEtiketi.Dock = DockStyle.Right;
        _secilenOyunEtiketi.TextAlign = ContentAlignment.MiddleRight;
        altPanel.Controls.Add(_secilenOyunEtiketi);
        altPanel.Controls.Add(_altDurum);

        Controls.Add(_icerikPaneli);
        Controls.Add(ustPanel);
        Controls.Add(altPanel);
        Controls.Add(solPanel);
    }

    private void GezintiDugmesiEkle(FlowLayoutPanel panel, string kimlik, string metin)
    {
        var dugme = ArayuzTemasi.Dugme(metin, (_, _) => SayfayiGoster(kimlik));
        dugme.Width = 202;
        dugme.Height = 46;
        dugme.TextAlign = ContentAlignment.MiddleLeft;
        dugme.Margin = new Padding(0, 0, 0, 7);
        panel.Controls.Add(dugme);
        _gezintiDugmeleri[kimlik] = dugme;
    }

    private void SayfalariKur()
    {
        _sayfalar["ana"] = AnaSayfayiKur();
        _sayfalar["oyun"] = OyunSayfasiniKur();
        _sayfalar["uyumluluk"] = UyumlulukSayfasiniKur();
        _sayfalar["katki"] = KatkiSayfasiniKur();
        _sayfalar["gunluk"] = GunlukSayfasiniKur();

        foreach (var sayfa in _sayfalar.Values)
        {
            sayfa.Dock = DockStyle.Fill;
            sayfa.Visible = false;
            _icerikPaneli.Controls.Add(sayfa);
        }
    }

    private Panel AnaSayfayiKur()
    {
        var sayfa = YeniSayfa();
        var akis = YeniDikeyAkis();
        sayfa.Controls.Add(akis);

        var kahraman = ArayuzTemasi.KartPaneli(26);
        kahraman.Width = 1000;
        kahraman.Height = 190;
        var baslik = ArayuzTemasi.Etiket("Unity modlamasını tek merkezden yönetin", 22f, true);
        baslik.Location = new Point(26, 25);
        var aciklama = ArayuzTemasi.Etiket(
            "Oyunu analiz edin, doğru Mono/IL2CPP ve BepInEx ortamını görün, UMMF hostunu kurun ve topluluk uyumluluk paketi hazırlayın.",
            10.5f,
            false,
            ArayuzTemasi.SolukMetin);
        aciklama.MaximumSize = new Size(700, 0);
        aciklama.Location = new Point(28, 72);
        var oyunSec = ArayuzTemasi.Dugme("Oyun seç ve analiz et", (_, _) => SayfayiGoster("oyun"), true);
        oyunSec.Size = new Size(190, 44);
        oyunSec.Location = new Point(28, 124);
        var katki = ArayuzTemasi.Dugme("Topluluğa katkı", (_, _) => SayfayiGoster("katki"));
        katki.Size = new Size(165, 44);
        katki.Location = new Point(230, 124);
        kahraman.Controls.AddRange(new Control[] { baslik, aciklama, oyunSec, katki });
        akis.Controls.Add(kahraman);

        var kartlar = new TableLayoutPanel
        {
            Width = 1000,
            Height = 175,
            ColumnCount = 3,
            RowCount = 1,
            Margin = new Padding(0, 0, 0, 12)
        };
        kartlar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
        kartlar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
        kartlar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34f));
        kartlar.Controls.Add(BilgiKarti("01", "Ortam analizi", "Unity sürümü, mimari, Mono/IL2CPP, BepInEx, TextMeshPro, FMOD ve Wwise izleri."), 0, 0);
        kartlar.Controls.Add(BilgiKarti("02", "Güvenli kurulum", "Yanlış hostu reddeder; UMMF pluginini kurar, durumunu denetler ve güvenli biçimde kaldırır."), 1, 0);
        kartlar.Controls.Add(BilgiKarti("03", "Topluluk uyumluluğu", "Doku, ses, altyazı ve seslendirme sonuçlarını seçip temiz bir katkı paketi üretir."), 2, 0);
        akis.Controls.Add(kartlar);

        var kapsam = ArayuzTemasi.KartPaneli();
        kapsam.Width = 1000;
        kapsam.Height = 150;
        var kapsamBaslik = ArayuzTemasi.Etiket("Bu arayüz sürümünün kapsamı", 13f, true);
        kapsamBaslik.Location = new Point(20, 18);
        var kapsamMetin = ArayuzTemasi.Etiket(
            "• Oyun klasörü seçimi ve ayrıntılı tarama\n• BepInEx 5 Mono kurulumu, durum, rapor ve kaldırma\n• Alfabetik uyumluluk listesi\n• Telifli dosya içermeyen katkı ZIP'i\n• Yerleşik Türkçe işlem günlüğü",
            10f,
            false,
            ArayuzTemasi.SolukMetin);
        kapsamMetin.Location = new Point(22, 52);
        kapsam.Controls.AddRange(new Control[] { kapsamBaslik, kapsamMetin });
        akis.Controls.Add(kapsam);
        return sayfa;
    }

    private Panel OyunSayfasiniKur()
    {
        var sayfa = YeniSayfa();
        var akis = YeniDikeyAkis();
        sayfa.Controls.Add(akis);

        var secimKarti = ArayuzTemasi.KartPaneli();
        secimKarti.Width = 1000;
        secimKarti.Height = 112;
        var baslik = ArayuzTemasi.Etiket("Oyun klasörü", 12.5f, true);
        baslik.Location = new Point(20, 16);
        _oyunYoluKutusu.Location = new Point(22, 53);
        _oyunYoluKutusu.Width = 690;
        _oyunYoluKutusu.ReadOnly = true;
        var sec = ArayuzTemasi.Dugme("Klasör seç", (_, _) => OyunKlasoruSec());
        sec.Size = new Size(120, 38);
        sec.Location = new Point(728, 52);
        var tara = ArayuzTemasi.Dugme("Tara", async (_, _) => await OyunuTaraAsync(), true);
        tara.Size = new Size(110, 38);
        tara.Location = new Point(862, 52);
        secimKarti.Controls.AddRange(new Control[] { baslik, _oyunYoluKutusu, sec, tara });
        akis.Controls.Add(secimKarti);

        var ortamKarti = ArayuzTemasi.KartPaneli();
        ortamKarti.Width = 1000;
        ortamKarti.Height = 235;
        var ortamBaslik = ArayuzTemasi.Etiket("Algılanan oyun ortamı", 12.5f, true);
        ortamBaslik.Location = new Point(20, 16);
        ortamKarti.Controls.Add(ortamBaslik);
        var alanlar = new[]
        {
            ("unity", "Unity sürümü"), ("arkaUc", "Betik arka ucu"), ("mimari", "Mimari"),
            ("yukleyici", "Yükleyici"), ("host", "Seçilen host"), ("arayuz", "Metin arayüzü"),
            ("ses", "Ses katmanı"), ("durum", "Başlatılabilirlik")
        };
        for (var i = 0; i < alanlar.Length; i++)
        {
            var sutun = i % 2;
            var satir = i / 2;
            var x = 22 + sutun * 480;
            var y = 54 + satir * 42;
            var ad = ArayuzTemasi.Etiket(alanlar[i].Item2, 8.7f, false, ArayuzTemasi.SolukMetin);
            ad.Location = new Point(x, y);
            var deger = ArayuzTemasi.Etiket("—", 10f, true);
            deger.Location = new Point(x + 145, y - 1);
            deger.MaximumSize = new Size(315, 35);
            _ortamDegerleri[alanlar[i].Item1] = deger;
            ortamKarti.Controls.Add(ad);
            ortamKarti.Controls.Add(deger);
        }
        akis.Controls.Add(ortamKarti);

        var islemler = ArayuzTemasi.KartPaneli();
        islemler.Width = 1000;
        islemler.Height = 178;
        var islemBaslik = ArayuzTemasi.Etiket("UMMF işlemleri", 12.5f, true);
        islemBaslik.Location = new Point(20, 16);
        islemler.Controls.Add(islemBaslik);
        var dugmeler = new FlowLayoutPanel
        {
            Location = new Point(20, 52),
            Size = new Size(950, 48),
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            BackColor = Color.Transparent
        };
        var sesPlanla = ArayuzTemasi.Dugme("Ses planı", async (_, _) => await KurulumIslemiCalistirAsync("Ses planı", y => y.SesPlanla(_secilenOyunYolu!)));
        var kur = ArayuzTemasi.Dugme("UMMF'yi kur", async (_, _) => await KurulumIslemiCalistirAsync("Kurulum", y => y.Kur(_secilenOyunYolu!)), true);
        var durum = ArayuzTemasi.Dugme("Durumu denetle", async (_, _) => await KurulumIslemiCalistirAsync("Durum", y => y.Durum(_secilenOyunYolu!)));
        var rapor = ArayuzTemasi.Dugme("Rapor oluştur", async (_, _) => await KurulumIslemiCalistirAsync("Rapor", y => y.RaporOlustur(_secilenOyunYolu!)));
        var kaldir = ArayuzTemasi.Dugme("Kaldır", async (_, _) => await KurulumIslemiCalistirAsync("Kaldırma", y => y.Kaldir(_secilenOyunYolu!)));
        foreach (var dugme in new[] { sesPlanla, kur, durum, rapor, kaldir })
        {
            dugme.Width = 170;
            dugme.Margin = new Padding(0, 0, 12, 0);
            dugmeler.Controls.Add(dugme);
        }
        _kurulumSonucu.Location = new Point(22, 116);
        _kurulumSonucu.MaximumSize = new Size(930, 45);
        islemler.Controls.Add(dugmeler);
        islemler.Controls.Add(_kurulumSonucu);
        akis.Controls.Add(islemler);
        return sayfa;
    }

    private Panel UyumlulukSayfasiniKur()
    {
        var sayfa = YeniSayfa();
        var ust = ArayuzTemasi.KartPaneli();
        ust.Dock = DockStyle.Top;
        ust.Height = 92;
        var baslik = ArayuzTemasi.Etiket("Alfabetik oyun uyumluluk listesi", 13f, true);
        baslik.Location = new Point(20, 16);
        var aciklama = ArayuzTemasi.Etiket("Liste GitHub'daki uyumluluk/oyunlar.json verisinden uygulamaya gömülür.", 9.5f, false, ArayuzTemasi.SolukMetin);
        aciklama.Location = new Point(22, 48);
        var github = ArayuzTemasi.Dugme("GitHub listesini aç", (_, _) => AdresiAc("https://github.com/apexlions16/UMMF/blob/main/UYUMLULUK.md"));
        github.Size = new Size(170, 38);
        github.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        github.Location = new Point(790, 26);
        ust.Controls.AddRange(new Control[] { baslik, aciklama, github });

        _uyumlulukTablosu.Dock = DockStyle.Fill;
        _uyumlulukTablosu.BackgroundColor = ArayuzTemasi.Kart;
        _uyumlulukTablosu.BorderStyle = BorderStyle.None;
        _uyumlulukTablosu.GridColor = ArayuzTemasi.Cizgi;
        _uyumlulukTablosu.EnableHeadersVisualStyles = false;
        _uyumlulukTablosu.ColumnHeadersDefaultCellStyle.BackColor = ArayuzTemasi.KartVurgu;
        _uyumlulukTablosu.ColumnHeadersDefaultCellStyle.ForeColor = ArayuzTemasi.Metin;
        _uyumlulukTablosu.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9f);
        _uyumlulukTablosu.DefaultCellStyle.BackColor = ArayuzTemasi.Kart;
        _uyumlulukTablosu.DefaultCellStyle.ForeColor = ArayuzTemasi.Metin;
        _uyumlulukTablosu.DefaultCellStyle.SelectionBackColor = ArayuzTemasi.VurguKoyu;
        _uyumlulukTablosu.DefaultCellStyle.SelectionForeColor = Color.White;
        _uyumlulukTablosu.RowHeadersVisible = false;
        _uyumlulukTablosu.AllowUserToAddRows = false;
        _uyumlulukTablosu.AllowUserToDeleteRows = false;
        _uyumlulukTablosu.AllowUserToResizeRows = false;
        _uyumlulukTablosu.ReadOnly = true;
        _uyumlulukTablosu.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _uyumlulukTablosu.RowTemplate.Height = 34;
        _uyumlulukTablosu.Columns.Add("oyun", "Oyun");
        _uyumlulukTablosu.Columns.Add("unity", "Unity");
        _uyumlulukTablosu.Columns.Add("arkaUc", "Arka uç");
        _uyumlulukTablosu.Columns.Add("mimari", "Mimari");
        _uyumlulukTablosu.Columns.Add("bepinex", "BepInEx");
        _uyumlulukTablosu.Columns.Add("doku", "Doku");
        _uyumlulukTablosu.Columns.Add("ses", "Ses");
        _uyumlulukTablosu.Columns.Add("altyazi", "Altyazı");
        _uyumlulukTablosu.Columns.Add("seslendirme", "Seslendirme");

        var tabloKarti = ArayuzTemasi.KartPaneli(8);
        tabloKarti.Dock = DockStyle.Fill;
        tabloKarti.Padding = new Padding(8);
        tabloKarti.Controls.Add(_uyumlulukTablosu);
        sayfa.Controls.Add(tabloKarti);
        sayfa.Controls.Add(ust);
        return sayfa;
    }

    private Panel KatkiSayfasiniKur()
    {
        var sayfa = YeniSayfa();
        var akis = YeniDikeyAkis();
        sayfa.Controls.Add(akis);

        var bilgi = ArayuzTemasi.KartPaneli();
        bilgi.Width = 1000;
        bilgi.Height = 116;
        var baslik = ArayuzTemasi.Etiket("Güvenli topluluk katkısı", 13f, true);
        baslik.Location = new Point(20, 16);
        var aciklama = ArayuzTemasi.Etiket(
            "Seçilen oyunun yalnızca ortam metadatasını ve test sonuçlarını paketler. Oyun EXE/DLL, varlık, ses, doku veya altyazı dosyası eklenmez.",
            9.5f,
            false,
            ArayuzTemasi.SolukMetin);
        aciklama.MaximumSize = new Size(730, 0);
        aciklama.Location = new Point(22, 48);
        var formuAc = ArayuzTemasi.Dugme("GitHub formunu aç", (_, _) => AdresiAc("https://github.com/apexlions16/UMMF/issues/new/choose"));
        formuAc.Size = new Size(170, 40);
        formuAc.Location = new Point(800, 39);
        bilgi.Controls.AddRange(new Control[] { baslik, aciklama, formuAc });
        akis.Controls.Add(bilgi);

        var form = ArayuzTemasi.KartPaneli();
        form.Width = 1000;
        form.Height = 385;
        var formBaslik = ArayuzTemasi.Etiket("Oyun profili ve test sonucu", 12.5f, true);
        formBaslik.Location = new Point(20, 16);
        form.Controls.Add(formBaslik);

        FormAlaniEkle(form, "Oyun adı", _katkiOyunAdi, 22, 55, 445);
        FormAlaniEkle(form, "Oyun sürümü", _katkiOyunSurumu, 515, 55, 455);
        FormAlaniEkle(form, "Doku", _katkiDoku, 22, 128, 220);
        FormAlaniEkle(form, "Ses", _katkiSes, 265, 128, 220);
        FormAlaniEkle(form, "Altyazı", _katkiAltyazi, 508, 128, 220);
        FormAlaniEkle(form, "Seslendirme", _katkiSeslendirme, 751, 128, 219);
        foreach (var kutu in new[] { _katkiDoku, _katkiSes, _katkiAltyazi, _katkiSeslendirme })
        {
            kutu.Items.AddRange(new object[] { "test-edilmedi", "calisiyor", "kismi", "destek-yok", "test-bekliyor" });
            kutu.SelectedIndex = 0;
        }

        var notEtiketi = ArayuzTemasi.Etiket("Test notları", 8.7f, false, ArayuzTemasi.SolukMetin);
        notEtiketi.Location = new Point(22, 207);
        _katkiNotlari.Location = new Point(22, 231);
        _katkiNotlari.Size = new Size(948, 70);
        _katkiNotlari.Multiline = true;
        _katkiNotlari.ScrollBars = ScrollBars.Vertical;
        form.Controls.Add(notEtiketi);
        form.Controls.Add(_katkiNotlari);

        var paket = ArayuzTemasi.Dugme("Katkı ZIP'i oluştur", (_, _) => KatkiPaketiOlustur(), true);
        paket.Size = new Size(190, 42);
        paket.Location = new Point(22, 322);
        var yenidenTara = ArayuzTemasi.Dugme("Oyunu yeniden tara", async (_, _) => await OyunuTaraAsync());
        yenidenTara.Size = new Size(170, 42);
        yenidenTara.Location = new Point(226, 322);
        var uyumluluk = ArayuzTemasi.Dugme("Uyumluluk listesini aç", (_, _) => SayfayiGoster("uyumluluk"));
        uyumluluk.Size = new Size(190, 42);
        uyumluluk.Location = new Point(410, 322);
        form.Controls.AddRange(new Control[] { paket, yenidenTara, uyumluluk });
        akis.Controls.Add(form);
        return sayfa;
    }

    private Panel GunlukSayfasiniKur()
    {
        var sayfa = YeniSayfa();
        var ust = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = Color.Transparent };
        var temizle = ArayuzTemasi.Dugme("Günlüğü temizle", (_, _) => _gunluk.Clear());
        temizle.Size = new Size(150, 38);
        temizle.Location = new Point(0, 2);
        var kaydet = ArayuzTemasi.Dugme("Metin olarak kaydet", (_, _) => GunluguKaydet());
        kaydet.Size = new Size(170, 38);
        kaydet.Location = new Point(164, 2);
        ust.Controls.AddRange(new Control[] { temizle, kaydet });

        _gunluk.Dock = DockStyle.Fill;
        _gunluk.BackColor = Color.FromArgb(12, 15, 21);
        _gunluk.ForeColor = ArayuzTemasi.Metin;
        _gunluk.BorderStyle = BorderStyle.None;
        _gunluk.Font = new Font("Cascadia Mono", 9.5f);
        _gunluk.ReadOnly = true;
        _gunluk.DetectUrls = true;
        _gunluk.LinkClicked += (_, e) => AdresiAc(e.LinkText);

        var kart = ArayuzTemasi.KartPaneli(10);
        kart.Dock = DockStyle.Fill;
        kart.Controls.Add(_gunluk);
        sayfa.Controls.Add(kart);
        sayfa.Controls.Add(ust);
        return sayfa;
    }

    private static Panel BilgiKarti(string numara, string baslik, string aciklama)
    {
        var kart = ArayuzTemasi.KartPaneli();
        kart.Dock = DockStyle.Fill;
        kart.Margin = new Padding(0, 0, 14, 0);
        var no = ArayuzTemasi.Etiket(numara, 9f, true, ArayuzTemasi.Vurgu);
        no.Location = new Point(18, 18);
        var baslikEtiketi = ArayuzTemasi.Etiket(baslik, 12f, true);
        baslikEtiketi.Location = new Point(18, 48);
        var aciklamaEtiketi = ArayuzTemasi.Etiket(aciklama, 9.2f, false, ArayuzTemasi.SolukMetin);
        aciklamaEtiketi.MaximumSize = new Size(270, 0);
        aciklamaEtiketi.Location = new Point(18, 80);
        kart.Controls.AddRange(new Control[] { no, baslikEtiketi, aciklamaEtiketi });
        return kart;
    }

    private static Panel YeniSayfa()
    {
        return new Panel
        {
            BackColor = ArayuzTemasi.ArkaPlan,
            Padding = new Padding(0),
            AutoScroll = false
        };
    }

    private static FlowLayoutPanel YeniDikeyAkis()
    {
        return new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            BackColor = ArayuzTemasi.ArkaPlan,
            Padding = new Padding(0, 0, 8, 20)
        };
    }

    private static void FormAlaniEkle(Control ebeveyn, string ad, Control alan, int x, int y, int genislik)
    {
        var etiket = ArayuzTemasi.Etiket(ad, 8.7f, false, ArayuzTemasi.SolukMetin);
        etiket.Location = new Point(x, y);
        alan.Location = new Point(x, y + 24);
        alan.Width = genislik;
        ebeveyn.Controls.Add(etiket);
        ebeveyn.Controls.Add(alan);
    }

    private void SayfayiGoster(string kimlik)
    {
        if (!_sayfalar.TryGetValue(kimlik, out var sayfa))
        {
            return;
        }

        foreach (var oge in _sayfalar)
        {
            oge.Value.Visible = oge.Key.Equals(kimlik, StringComparison.OrdinalIgnoreCase);
        }

        foreach (var oge in _gezintiDugmeleri)
        {
            oge.Value.BackColor = oge.Key.Equals(kimlik, StringComparison.OrdinalIgnoreCase)
                ? ArayuzTemasi.Vurgu
                : ArayuzTemasi.KartVurgu;
        }

        _aktifSayfa = kimlik;
        (_sayfaBasligi.Text, _sayfaAciklamasi.Text) = kimlik switch
        {
            "oyun" => ("Oyun ve Kurulum", "Unity ortamını analiz edin ve UMMF hostunu yönetin."),
            "uyumluluk" => ("Uyumluluk", "Topluluk tarafından doğrulanan oyun ve medya desteği."),
            "katki" => ("Topluluk Katkısı", "Güvenli oyun profili ve test paketi oluşturun."),
            "gunluk" => ("Günlük", "Tarama, kurulum ve arayüz işlemlerinin Türkçe kaydı."),
            _ => ("Ana Sayfa", "Unity oyunlarını analiz edin ve UMMF'yi yönetin.")
        };
        sayfa.BringToFront();
    }

    private void OyunKlasoruSec()
    {
        using var secici = new FolderBrowserDialog
        {
            Description = "Unity oyununun ana klasörünü seçin",
            ShowNewFolderButton = false,
            UseDescriptionForTitle = true
        };
        if (secici.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        _secilenOyunYolu = Path.GetFullPath(secici.SelectedPath);
        _oyunYoluKutusu.Text = _secilenOyunYolu;
        _secilenOyunEtiketi.Text = "Seçili: " + Path.GetFileName(_secilenOyunYolu.TrimEnd(Path.DirectorySeparatorChar));
        _katkiOyunAdi.Text = Path.GetFileName(_secilenOyunYolu.TrimEnd(Path.DirectorySeparatorChar));
        GunlukYaz("Oyun klasörü seçildi: " + _secilenOyunYolu);
        _ = OyunuTaraAsync();
    }

    private async Task OyunuTaraAsync()
    {
        if (!OyunYoluHazirMi())
        {
            return;
        }

        IslemDurumu("Oyun ortamı taranıyor...", ArayuzTemasi.Uyari);
        GunlukYaz("Tarama başladı.");
        try
        {
            var ortam = await Task.Run(() => new OyunOrtamiAlgilayici().Tara(_secilenOyunYolu!));
            var secim = new CalismaZamaniHostSecici(VarsayilanCalismaZamaniHostlari.Olustur()).Sec(ortam);
            _sonOrtam = ortam;

            _ortamDegerleri["unity"].Text = Deger(ortam.UnitySurumu);
            _ortamDegerleri["arkaUc"].Text = ortam.BetikArkaUcu.ToString();
            _ortamDegerleri["mimari"].Text = ortam.Mimari.ToString();
            _ortamDegerleri["yukleyici"].Text = ortam.Yukleyici.Kurulu
                ? $"{ortam.Yukleyici.Tur} {Deger(ortam.Yukleyici.Surum)}"
                : "Bulunamadı";
            _ortamDegerleri["host"].Text = secim.Host?.Ad ?? "Host bulunamadı";
            _ortamDegerleri["arayuz"].Text = $"TMP: {VarYok(ortam.TextMeshProVar)} · Unity UI: {VarYok(ortam.UnityUiVar)}";
            _ortamDegerleri["ses"].Text = $"FMOD: {VarYok(ortam.FmodVar)} · Wwise: {VarYok(ortam.WwiseVar)}";
            _ortamDegerleri["durum"].Text = secim.Degerlendirme.Calistirilabilir ? "Başlatılabilir" : secim.Degerlendirme.Aciklama;

            _katkiOyunAdi.Text = OyunAdiBul(ortam);
            _katkiOyunSurumu.Text = "Bilinmiyor";
            IslemDurumu(ortam.UnityOyunuMu ? "Unity oyun ortamı algılandı." : "Unity oyunu doğrulanamadı.", ortam.UnityOyunuMu ? ArayuzTemasi.Basarili : ArayuzTemasi.Hata);
            GunlukYaz($"Tarama tamamlandı: Unity={Deger(ortam.UnitySurumu)}, arka uç={ortam.BetikArkaUcu}, mimari={ortam.Mimari}, host={secim.Host?.Ad ?? "yok"}.");
        }
        catch (Exception hata) when (hata is IOException || hata is UnauthorizedAccessException || hata is ArgumentException || hata is DirectoryNotFoundException)
        {
            _sonOrtam = null;
            IslemDurumu("Tarama başarısız.", ArayuzTemasi.Hata);
            GunlukYaz("Tarama hatası: " + hata.Message, ArayuzTemasi.Hata);
            MessageBox.Show(this, hata.Message, "UMMF tarama hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task KurulumIslemiCalistirAsync(string islemAdi, Func<BepInEx5KurulumYoneticisi, KurulumIslemSonucu> islem)
    {
        if (!OyunYoluHazirMi())
        {
            return;
        }

        IslemDurumu(islemAdi + " çalışıyor...", ArayuzTemasi.Uyari);
        _kurulumSonucu.Text = islemAdi + " çalışıyor...";
        GunlukYaz(islemAdi + " başladı.");
        try
        {
            var sonuc = await Task.Run(() => islem(_kurulumYoneticisi));
            _kurulumSonucu.Text = sonuc.Aciklama;
            _kurulumSonucu.ForeColor = sonuc.Basarili ? ArayuzTemasi.Basarili : ArayuzTemasi.Hata;
            IslemDurumu(sonuc.Basarili ? islemAdi + " tamamlandı." : islemAdi + " başarısız.", sonuc.Basarili ? ArayuzTemasi.Basarili : ArayuzTemasi.Hata);
            GunlukYaz((sonuc.Basarili ? "BAŞARILI: " : "BAŞARISIZ: ") + sonuc.Aciklama, sonuc.Basarili ? ArayuzTemasi.Basarili : ArayuzTemasi.Hata);
            if (sonuc.Plan is not null)
            {
                GunlukYaz("Plugin: " + sonuc.Plan.EklentiDosyasi);
                GunlukYaz("Mod klasörü: " + sonuc.Plan.ModDizini);
            }
            if (!string.IsNullOrWhiteSpace(sonuc.Sha256))
            {
                GunlukYaz("SHA-256: " + sonuc.Sha256);
            }

            if (sonuc.Basarili)
            {
                await OyunuTaraAsync();
            }
            else
            {
                MessageBox.Show(this, sonuc.Aciklama, "UMMF " + islemAdi, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        catch (Exception hata)
        {
            _kurulumSonucu.Text = hata.Message;
            _kurulumSonucu.ForeColor = ArayuzTemasi.Hata;
            IslemDurumu(islemAdi + " sırasında beklenmeyen hata.", ArayuzTemasi.Hata);
            GunlukYaz("Beklenmeyen hata: " + hata, ArayuzTemasi.Hata);
            MessageBox.Show(this, hata.Message, "UMMF beklenmeyen hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void KatkiPaketiOlustur()
    {
        if (_sonOrtam is null)
        {
            MessageBox.Show(this, "Önce Oyun ve Kurulum ekranında bir oyun seçip tarayın.", "UMMF katkı paketi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SayfayiGoster("oyun");
            return;
        }

        if (string.IsNullOrWhiteSpace(_katkiOyunAdi.Text))
        {
            MessageBox.Show(this, "Oyun adı boş bırakılamaz.", "UMMF katkı paketi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using var kaydet = new SaveFileDialog
        {
            Title = "UMMF topluluk katkı paketini kaydet",
            Filter = "ZIP arşivi (*.zip)|*.zip",
            FileName = "UMMF-katki-" + DateTime.Now.ToString("yyyyMMdd-HHmm") + ".zip",
            AddExtension = true,
            DefaultExt = "zip"
        };
        if (kaydet.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            var yol = KatkiPaketiOlusturucu.Olustur(new KatkiPaketiGirdisi
            {
                Ortam = _sonOrtam,
                OyunAdi = _katkiOyunAdi.Text,
                OyunSurumu = _katkiOyunSurumu.Text,
                Doku = _katkiDoku.SelectedItem?.ToString() ?? "test-edilmedi",
                Ses = _katkiSes.SelectedItem?.ToString() ?? "test-edilmedi",
                Altyazi = _katkiAltyazi.SelectedItem?.ToString() ?? "test-edilmedi",
                Seslendirme = _katkiSeslendirme.SelectedItem?.ToString() ?? "test-edilmedi",
                Notlar = _katkiNotlari.Text
            }, kaydet.FileName);
            GunlukYaz("Topluluk katkı paketi oluşturuldu: " + yol, ArayuzTemasi.Basarili);
            IslemDurumu("Katkı paketi hazır.", ArayuzTemasi.Basarili);
            var sonuc = MessageBox.Show(this, "Katkı paketi oluşturuldu. Dosyanın bulunduğu klasör açılsın mı?", "UMMF katkı paketi", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (sonuc == DialogResult.Yes)
            {
                Process.Start(new ProcessStartInfo("explorer.exe", $"/select,\"{yol}\"") { UseShellExecute = true });
            }
        }
        catch (Exception hata) when (hata is IOException || hata is UnauthorizedAccessException || hata is ArgumentException)
        {
            GunlukYaz("Katkı paketi hatası: " + hata.Message, ArayuzTemasi.Hata);
            MessageBox.Show(this, hata.Message, "UMMF katkı paketi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void UyumlulukVerisiniYukle()
    {
        try
        {
            using var akis = Assembly.GetExecutingAssembly().GetManifestResourceStream("UMMF.Kaynaklar.uyumluluk.oyunlar.json");
            if (akis is null)
            {
                throw new InvalidOperationException("Gömülü uyumluluk verisi bulunamadı.");
            }

            using var belge = JsonDocument.Parse(akis);
            var oyunlar = belge.RootElement.GetProperty("oyunlar").EnumerateArray()
                .OrderBy(oyun => oyun.GetProperty("ad").GetString(), StringComparer.CurrentCultureIgnoreCase)
                .ToArray();
            foreach (var oyun in oyunlar)
            {
                _uyumlulukTablosu.Rows.Add(
                    Metin(oyun, "ad"),
                    Metin(oyun, "unitySurumu"),
                    Metin(oyun, "betikArkaUcu"),
                    Metin(oyun, "mimari"),
                    Metin(oyun, "bepInEx"),
                    DestekMetni(Metin(oyun, "doku")),
                    DestekMetni(Metin(oyun, "ses")),
                    DestekMetni(Metin(oyun, "altyazi")),
                    DestekMetni(Metin(oyun, "seslendirme")));
            }
        }
        catch (Exception hata) when (hata is IOException || hata is JsonException || hata is InvalidOperationException)
        {
            GunlukYaz("Uyumluluk verisi yüklenemedi: " + hata.Message, ArayuzTemasi.Hata);
        }
    }

    private void GunluguKaydet()
    {
        using var kaydet = new SaveFileDialog
        {
            Title = "UMMF günlüğünü kaydet",
            Filter = "Metin dosyası (*.txt)|*.txt",
            FileName = "UMMF-gunluk-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".txt",
            AddExtension = true
        };
        if (kaydet.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        File.WriteAllText(kaydet.FileName, _gunluk.Text, new UTF8Encoding(false));
        IslemDurumu("Günlük kaydedildi.", ArayuzTemasi.Basarili);
    }

    private void GunlukYaz(string metin, Color? renk = null)
    {
        if (_gunluk.IsDisposed)
        {
            return;
        }

        _gunluk.SelectionStart = _gunluk.TextLength;
        _gunluk.SelectionLength = 0;
        _gunluk.SelectionColor = renk ?? ArayuzTemasi.Metin;
        _gunluk.AppendText($"[{DateTime.Now:HH:mm:ss}] {metin}{Environment.NewLine}");
        _gunluk.SelectionColor = ArayuzTemasi.Metin;
        _gunluk.ScrollToCaret();
    }

    private void IslemDurumu(string metin, Color renk)
    {
        _altDurum.Text = metin;
        _altDurum.ForeColor = renk;
    }

    private bool OyunYoluHazirMi()
    {
        if (!string.IsNullOrWhiteSpace(_secilenOyunYolu) && Directory.Exists(_secilenOyunYolu))
        {
            return true;
        }

        MessageBox.Show(this, "Önce geçerli bir oyun klasörü seçin.", "UMMF", MessageBoxButtons.OK, MessageBoxIcon.Information);
        SayfayiGoster("oyun");
        return false;
    }

    private static string OyunAdiBul(OyunOrtami ortam)
    {
        if (!string.IsNullOrWhiteSpace(ortam.CalistirilabilirDosya))
        {
            return Path.GetFileNameWithoutExtension(ortam.CalistirilabilirDosya);
        }

        return Path.GetFileName(ortam.OyunDizini.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
    }

    private static string Deger(string? deger) => string.IsNullOrWhiteSpace(deger) ? "Bilinmiyor" : deger;

    private static string VarYok(bool deger) => deger ? "Var" : "Yok";

    private static string Metin(JsonElement oge, string alan)
    {
        return oge.TryGetProperty(alan, out var deger) ? deger.GetString() ?? "—" : "—";
    }

    private static string DestekMetni(string deger)
    {
        return deger switch
        {
            "calisiyor" => "Çalışıyor",
            "kismi" => "Kısmi",
            "destek-yok" => "Destek yok",
            "test-bekliyor" => "Test bekliyor",
            "planlandi" => "Planlandı",
            _ => "Test edilmedi"
        };
    }

    private static void AdresiAc(string adres)
    {
        try
        {
            Process.Start(new ProcessStartInfo(adres) { UseShellExecute = true });
        }
        catch
        {
            // Tarayıcı açılamazsa uygulamanın ana akışı etkilenmez.
        }
    }
}
