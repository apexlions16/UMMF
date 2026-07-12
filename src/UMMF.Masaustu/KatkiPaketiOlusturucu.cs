using System.IO.Compression;
using System.Text;
using System.Text.Json;
using UMMF.Sozlesmeler;

namespace UMMF.Masaustu;

internal sealed class KatkiPaketiGirdisi
{
    public required OyunOrtami Ortam { get; init; }
    public required string OyunAdi { get; init; }
    public string OyunSurumu { get; init; } = "Bilinmiyor";
    public string Doku { get; init; } = "test-edilmedi";
    public string Ses { get; init; } = "test-edilmedi";
    public string Altyazi { get; init; } = "test-edilmedi";
    public string Seslendirme { get; init; } = "test-edilmedi";
    public string Notlar { get; init; } = string.Empty;
}

internal static class KatkiPaketiOlusturucu
{
    internal static string Olustur(KatkiPaketiGirdisi girdi, string hedefZip)
    {
        ArgumentNullException.ThrowIfNull(girdi);
        if (string.IsNullOrWhiteSpace(hedefZip))
        {
            throw new ArgumentException("Katkı paketi için hedef dosya seçilmedi.", nameof(hedefZip));
        }

        var tamHedef = Path.GetFullPath(hedefZip);
        Directory.CreateDirectory(Path.GetDirectoryName(tamHedef)!);
        var geciciKok = Path.Combine(Path.GetTempPath(), "UMMF-Katki-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(geciciKok);

        try
        {
            var kimlik = KimlikOlustur(girdi.OyunAdi);
            var profilDizini = Path.Combine(geciciKok, "oyun-profilleri", kimlik);
            var testDizini = Path.Combine(profilDizini, "testler");
            Directory.CreateDirectory(testDizini);

            var profil = new
            {
                semaSurumu = 1,
                kimlik,
                ad = Temizle(girdi.OyunAdi),
                testEdilenOyunSurumu = Temizle(girdi.OyunSurumu),
                unitySurumu = Temizle(girdi.Ortam.UnitySurumu),
                betikArkaUcu = girdi.Ortam.BetikArkaUcu.ToString(),
                mimari = girdi.Ortam.Mimari.ToString(),
                isletimSistemi = girdi.Ortam.IsletimSistemi.ToString(),
                bepinEx = girdi.Ortam.Yukleyici.Kurulu
                    ? Temizle($"{girdi.Ortam.Yukleyici.Tur} {girdi.Ortam.Yukleyici.Surum}")
                    : "Yok",
                host = HostAdi(girdi.Ortam),
                medya = new
                {
                    doku = girdi.Doku,
                    ses = girdi.Ses,
                    altyazi = girdi.Altyazi,
                    seslendirme = girdi.Seslendirme
                },
                ortamIzleri = new
                {
                    textMeshPro = girdi.Ortam.TextMeshProVar,
                    unityUi = girdi.Ortam.UnityUiVar,
                    addressables = girdi.Ortam.AddressablesVar,
                    fmod = girdi.Ortam.FmodVar,
                    wwise = girdi.Ortam.WwiseVar
                },
                sonTestTarihi = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                kanit = "GitHub issue veya pull request bağlantısını ekleyin"
            };

            var ayarlar = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(
                Path.Combine(profilDizini, "profil.json"),
                JsonSerializer.Serialize(profil, ayarlar),
                new UTF8Encoding(false));

            var testOzeti = new
            {
                olusturmaZamaniUtc = DateTime.UtcNow,
                ummfSurumu = Program.Surum,
                unityOyunu = girdi.Ortam.UnityOyunuMu,
                calistirilabilirDosyaAdi = Path.GetFileName(girdi.Ortam.CalistirilabilirDosya),
                veriDiziniAdi = Path.GetFileName(girdi.Ortam.VeriDizini?.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)),
                notlar = Temizle(girdi.Notlar),
                gizlilik = "Tam oyun yolları, kullanıcı adı ve telifli dosyalar pakete eklenmemiştir."
            };
            File.WriteAllText(
                Path.Combine(testDizini, "test-ozeti.json"),
                JsonSerializer.Serialize(testOzeti, ayarlar),
                new UTF8Encoding(false));

            File.WriteAllText(
                Path.Combine(profilDizini, "README.md"),
                README(girdi, kimlik),
                new UTF8Encoding(false));
            File.WriteAllText(
                Path.Combine(geciciKok, "GONDERMEDEN-ONCE.txt"),
                GuvenlikMetni(),
                new UTF8Encoding(false));

            if (File.Exists(tamHedef))
            {
                File.Delete(tamHedef);
            }

            ZipFile.CreateFromDirectory(geciciKok, tamHedef, CompressionLevel.Optimal, false);
            return tamHedef;
        }
        finally
        {
            if (Directory.Exists(geciciKok))
            {
                Directory.Delete(geciciKok, true);
            }
        }
    }

    private static string README(KatkiPaketiGirdisi girdi, string kimlik)
    {
        var metin = new StringBuilder();
        metin.AppendLine("# " + Temizle(girdi.OyunAdi) + " UMMF uyumluluk profili");
        metin.AppendLine();
        metin.AppendLine("Profil kimliği: `" + kimlik + "`");
        metin.AppendLine();
        metin.AppendLine("## Test edilen alanlar");
        metin.AppendLine();
        metin.AppendLine("- Doku: " + girdi.Doku);
        metin.AppendLine("- Ses: " + girdi.Ses);
        metin.AppendLine("- Altyazı: " + girdi.Altyazi);
        metin.AppendLine("- Altyazı seslendirme: " + girdi.Seslendirme);
        metin.AppendLine();
        metin.AppendLine("## Katkı notu");
        metin.AppendLine();
        metin.AppendLine(string.IsNullOrWhiteSpace(girdi.Notlar) ? "Test notu eklenmedi." : Temizle(girdi.Notlar));
        metin.AppendLine();
        metin.AppendLine("Bu paket oyun dosyası, varlık, tam kullanıcı yolu veya kapalı SDK içermez.");
        return metin.ToString();
    }

    private static string GuvenlikMetni()
    {
        return "UMMF TOPLULUK KATKISI\r\n\r\n" +
               "Bu ZIP yalnızca ortam metadatası ve test notları içerir.\r\n" +
               "Göndermeden önce profil.json ve test-ozeti.json dosyalarını inceleyin.\r\n\r\n" +
               "EKLEMEYİN:\r\n" +
               "- oyun EXE veya DLL dosyaları\r\n" +
               "- asset bundle, doku, ses, video veya altyazılar\r\n" +
               "- kayıt dosyaları ve kullanıcı profilleri\r\n" +
               "- hile koruması atlatma veya çok oyunculu manipülasyon kodu\r\n";
    }

    private static string HostAdi(OyunOrtami ortam)
    {
        if (!ortam.Yukleyici.Kurulu)
        {
            return "Yükleyici bulunamadı";
        }

        return ortam.BetikArkaUcu switch
        {
            BetikArkaUcu.Mono when ortam.Yukleyici.AnaSurum == 5 => "UMMF BepInEx 5 Mono",
            BetikArkaUcu.Mono when ortam.Yukleyici.AnaSurum == 6 => "UMMF BepInEx 6 Mono adayı",
            BetikArkaUcu.IL2CPP when ortam.Yukleyici.AnaSurum == 6 => "UMMF BepInEx 6 IL2CPP adayı",
            _ => "Uyumluluk hostu belirlenecek"
        };
    }

    private static string KimlikOlustur(string deger)
    {
        var normal = deger.Normalize(NormalizationForm.FormD).ToLowerInvariant();
        var metin = new StringBuilder();
        var tire = false;
        foreach (var karakter in normal)
        {
            var kategori = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(karakter);
            if (kategori == System.Globalization.UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            if (char.IsLetterOrDigit(karakter))
            {
                metin.Append(karakter);
                tire = false;
            }
            else if (!tire && metin.Length > 0)
            {
                metin.Append('-');
                tire = true;
            }
        }

        return metin.ToString().Trim('-');
    }

    private static string Temizle(string? deger)
    {
        if (string.IsNullOrWhiteSpace(deger))
        {
            return "Bilinmiyor";
        }

        return deger.Replace(Environment.UserName, "<kullanici>", StringComparison.OrdinalIgnoreCase).Trim();
    }
}
