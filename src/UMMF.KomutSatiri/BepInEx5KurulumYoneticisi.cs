using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using UMMF.Cekirdek;

namespace UMMF.KomutSatiri;

internal sealed class BepInEx5KurulumYoneticisi
{
    private const string Net35KaynakAdi = "UMMF.Kaynaklar.BepInEx5.Mono.net35.dll";
    private const string NetStandardKaynakAdi = "UMMF.Kaynaklar.BepInEx5.Mono.netstandard2.0.dll";
    private const string KurulumBilgisiDosyaAdi = "kurulum-bilgisi.json";

    public KurulumIslemSonucu Kur(string oyunDizini)
    {
        try
        {
            var ortam = new OyunOrtamiAlgilayici().Tara(oyunDizini);
            var plan = BepInEx5KurulumPlanlayici.Olustur(ortam);
            if (!plan.Uygun)
            {
                return KurulumIslemSonucu.Basarisiz(plan.Aciklama);
            }

            Directory.CreateDirectory(plan.EklentiDizini);
            Directory.CreateDirectory(plan.UMMFKokDizini);
            Directory.CreateDirectory(plan.ModDizini);
            Directory.CreateDirectory(plan.RaporDizini);

            var kaynakAdi = plan.HedefCerceve == BepInEx5EklentiCercevesi.Net35
                ? Net35KaynakAdi
                : NetStandardKaynakAdi;
            EklentiyiCikar(kaynakAdi, plan.EklentiDosyasi);
            KurulumBilgisiniYaz(plan, ortam);

            return KurulumIslemSonucu.Basarili(
                "UMMF BepInEx 5 Mono eklentisi başarıyla kuruldu.",
                plan,
                DosyaOzetiniHesapla(plan.EklentiDosyasi));
        }
        catch (Exception hata) when (hata is IOException ||
                                     hata is UnauthorizedAccessException ||
                                     hata is ArgumentException ||
                                     hata is DirectoryNotFoundException ||
                                     hata is InvalidOperationException)
        {
            return KurulumIslemSonucu.Basarisiz("Kurulum tamamlanamadı: " + hata.Message);
        }
    }

    public KurulumIslemSonucu Durum(string oyunDizini)
    {
        try
        {
            var ortam = new OyunOrtamiAlgilayici().Tara(oyunDizini);
            var plan = BepInEx5KurulumPlanlayici.Olustur(ortam);
            if (!plan.Uygun)
            {
                return KurulumIslemSonucu.Basarisiz(plan.Aciklama);
            }

            if (!File.Exists(plan.EklentiDosyasi))
            {
                return KurulumIslemSonucu.Basarisiz("UMMF eklentisi kurulu değil.", plan);
            }

            return KurulumIslemSonucu.Basarili(
                "UMMF eklentisi kurulu ve dosya erişilebilir.",
                plan,
                DosyaOzetiniHesapla(plan.EklentiDosyasi));
        }
        catch (Exception hata) when (hata is IOException ||
                                     hata is UnauthorizedAccessException ||
                                     hata is ArgumentException ||
                                     hata is DirectoryNotFoundException)
        {
            return KurulumIslemSonucu.Basarisiz("Durum denetlenemedi: " + hata.Message);
        }
    }

    public KurulumIslemSonucu RaporOlustur(string oyunDizini)
    {
        try
        {
            var ortam = new OyunOrtamiAlgilayici().Tara(oyunDizini);
            var plan = BepInEx5KurulumPlanlayici.Olustur(ortam);
            if (!plan.Uygun)
            {
                return KurulumIslemSonucu.Basarisiz(plan.Aciklama);
            }

            Directory.CreateDirectory(plan.RaporDizini);
            var raporYolu = Path.Combine(plan.RaporDizini, "kurulum-teshis-raporu.json");
            var rapor = new
            {
                ummfSurumu = Program.OnizlemeSurumu,
                olusturmaZamaniUtc = DateTime.UtcNow,
                oyunDizini = ortam.OyunDizini,
                veriDizini = ortam.VeriDizini,
                unitySurumu = ortam.UnitySurumu,
                betikArkaUcu = ortam.BetikArkaUcu.ToString(),
                mimari = ortam.Mimari.ToString(),
                yukleyici = ortam.Yukleyici.Tur.ToString(),
                yukleyiciSurumu = ortam.Yukleyici.Surum,
                hedefCerceve = HedefCerceveyiYaz(plan.HedefCerceve),
                eklentiDosyasi = plan.EklentiDosyasi,
                eklentiKurulu = File.Exists(plan.EklentiDosyasi),
                eklentiSha256 = File.Exists(plan.EklentiDosyasi)
                    ? DosyaOzetiniHesapla(plan.EklentiDosyasi)
                    : null,
                modDizini = plan.ModDizini,
                oyunDosyalariDegistirildi = false
            };

            File.WriteAllText(
                raporYolu,
                JsonSerializer.Serialize(rapor, new JsonSerializerOptions { WriteIndented = true }),
                new UTF8Encoding(false));

            return KurulumIslemSonucu.Basarili("Teşhis raporu oluşturuldu: " + raporYolu, plan);
        }
        catch (Exception hata) when (hata is IOException ||
                                     hata is UnauthorizedAccessException ||
                                     hata is ArgumentException ||
                                     hata is DirectoryNotFoundException)
        {
            return KurulumIslemSonucu.Basarisiz("Rapor oluşturulamadı: " + hata.Message);
        }
    }

    public KurulumIslemSonucu Kaldir(string oyunDizini)
    {
        try
        {
            var ortam = new OyunOrtamiAlgilayici().Tara(oyunDizini);
            var plan = BepInEx5KurulumPlanlayici.Olustur(ortam);
            if (!plan.Uygun)
            {
                return KurulumIslemSonucu.Basarisiz(plan.Aciklama);
            }

            if (File.Exists(plan.EklentiDosyasi))
            {
                File.Delete(plan.EklentiDosyasi);
            }

            var kurulumBilgisi = Path.Combine(plan.UMMFKokDizini, KurulumBilgisiDosyaAdi);
            if (File.Exists(kurulumBilgisi))
            {
                File.Delete(kurulumBilgisi);
            }

            DizinBossaSil(plan.EklentiDizini);

            return KurulumIslemSonucu.Basarili(
                "UMMF plugin DLL'si kaldırıldı. Kullanıcı modları ve raporlar korunmuştur.",
                plan);
        }
        catch (Exception hata) when (hata is IOException ||
                                     hata is UnauthorizedAccessException ||
                                     hata is ArgumentException ||
                                     hata is DirectoryNotFoundException)
        {
            return KurulumIslemSonucu.Basarisiz("Kaldırma tamamlanamadı: " + hata.Message);
        }
    }

    private static void EklentiyiCikar(string kaynakAdi, string hedefDosya)
    {
        var derleme = Assembly.GetExecutingAssembly();
        using var kaynak = derleme.GetManifestResourceStream(kaynakAdi);
        if (kaynak is null)
        {
            var bulunanlar = string.Join(", ", derleme.GetManifestResourceNames());
            throw new InvalidOperationException(
                "Gömülü plugin kaynağı bulunamadı: " + kaynakAdi + ". Bulunan kaynaklar: " + bulunanlar);
        }

        var geciciDosya = hedefDosya + ".yeni";
        using (var hedef = new FileStream(geciciDosya, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            kaynak.CopyTo(hedef);
            hedef.Flush(true);
        }

        File.Copy(geciciDosya, hedefDosya, true);
        File.Delete(geciciDosya);
    }

    private static void KurulumBilgisiniYaz(BepInEx5KurulumPlani plan, UMMF.Sozlesmeler.OyunOrtami ortam)
    {
        var bilgi = new
        {
            ummfSurumu = Program.OnizlemeSurumu,
            kurulumZamaniUtc = DateTime.UtcNow,
            host = "BepInEx 5 Unity Mono",
            hedefCerceve = HedefCerceveyiYaz(plan.HedefCerceve),
            eklentiDosyasi = plan.EklentiDosyasi,
            modDizini = plan.ModDizini,
            unitySurumu = ortam.UnitySurumu,
            oyunDosyalariDegistirildi = false
        };

        var yol = Path.Combine(plan.UMMFKokDizini, KurulumBilgisiDosyaAdi);
        File.WriteAllText(
            yol,
            JsonSerializer.Serialize(bilgi, new JsonSerializerOptions { WriteIndented = true }),
            new UTF8Encoding(false));
    }

    private static string DosyaOzetiniHesapla(string dosyaYolu)
    {
        using var akis = File.OpenRead(dosyaYolu);
        using var sha256 = SHA256.Create();
        return Convert.ToHexString(sha256.ComputeHash(akis)).ToLowerInvariant();
    }

    private static string HedefCerceveyiYaz(BepInEx5EklentiCercevesi cerceve)
    {
        return cerceve == BepInEx5EklentiCercevesi.Net35 ? "net35" : "netstandard2.0";
    }

    private static void DizinBossaSil(string dizin)
    {
        if (Directory.Exists(dizin) && !Directory.EnumerateFileSystemEntries(dizin).Any())
        {
            Directory.Delete(dizin);
        }
    }
}

internal sealed class KurulumIslemSonucu
{
    private KurulumIslemSonucu(bool basarili, string aciklama, BepInEx5KurulumPlani? plan, string? sha256)
    {
        Basarili = basarili;
        Aciklama = aciklama;
        Plan = plan;
        Sha256 = sha256;
    }

    public bool Basarili { get; }

    public string Aciklama { get; }

    public BepInEx5KurulumPlani? Plan { get; }

    public string? Sha256 { get; }

    public static KurulumIslemSonucu Basarili(
        string aciklama,
        BepInEx5KurulumPlani plan,
        string? sha256 = null)
    {
        return new KurulumIslemSonucu(true, aciklama, plan, sha256);
    }

    public static KurulumIslemSonucu Basarisiz(
        string aciklama,
        BepInEx5KurulumPlani? plan = null)
    {
        return new KurulumIslemSonucu(false, aciklama, plan, null);
    }
}
