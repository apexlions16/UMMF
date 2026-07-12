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
        BadParentingSesIslemSonucu? badParentingKurulumu = null;
        try
        {
            var ortam = new OyunOrtamiAlgilayici().Tara(oyunDizini);
            var badParentingYoneticisi = new BadParentingSesKurulumYoneticisi();
            var badParentingPlani = badParentingYoneticisi.Planla(ortam);
            if (badParentingPlani.Uygulanabilir)
            {
                badParentingKurulumu = badParentingYoneticisi.Kur(ortam);
                if (!badParentingKurulumu.Basarili)
                {
                    return KurulumIslemSonucu.BasarisizlikOlustur(badParentingKurulumu.Aciklama);
                }

                ortam = new OyunOrtamiAlgilayici().Tara(oyunDizini);
                BadParentingYukleyicisiniDogrula(ortam);
            }

            var plan = BepInEx5KurulumPlanlayici.Olustur(ortam);
            if (!plan.Uygun)
            {
                if (badParentingKurulumu?.Basarili == true)
                {
                    badParentingYoneticisi.Kaldir(ortam);
                }

                return KurulumIslemSonucu.BasarisizlikOlustur(plan.Aciklama);
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

            return KurulumIslemSonucu.BasariOlustur(
                badParentingKurulumu?.Basarili == true
                    ? "UMMF BepInEx 5 Mono eklentisi ve Bad Parenting ses modu başarıyla kuruldu. " + badParentingKurulumu.Aciklama
                    : "UMMF BepInEx 5 Mono eklentisi başarıyla kuruldu.",
                plan,
                DosyaOzetiniHesapla(plan.EklentiDosyasi));
        }
        catch (Exception hata) when (hata is IOException ||
                                     hata is UnauthorizedAccessException ||
                                     hata is ArgumentException ||
                                     hata is DirectoryNotFoundException ||
                                     hata is InvalidOperationException)
        {
            if (badParentingKurulumu?.Basarili == true)
            {
                try
                {
                    var ortam = new OyunOrtamiAlgilayici().Tara(oyunDizini);
                    new BadParentingSesKurulumYoneticisi().Kaldir(ortam);
                }
                catch
                {
                    // Asıl kurulum hatası korunur; durum kaydı güvenli kaldırmaya izin verir.
                }
            }

            return KurulumIslemSonucu.BasarisizlikOlustur("Kurulum tamamlanamadı: " + hata.Message);
        }
    }

    public KurulumIslemSonucu SesPlanla(string oyunDizini)
    {
        try
        {
            var ortam = new OyunOrtamiAlgilayici().Tara(oyunDizini);
            var sonuc = new BadParentingSesKurulumYoneticisi().Planla(ortam);
            return sonuc.Basarili
                ? KurulumIslemSonucu.BasariOlustur(sonuc.Aciklama, BosPlanOlustur(ortam))
                : KurulumIslemSonucu.BasarisizlikOlustur(sonuc.Aciklama);
        }
        catch (Exception hata) when (hata is IOException || hata is UnauthorizedAccessException || hata is ArgumentException || hata is DirectoryNotFoundException)
        {
            return KurulumIslemSonucu.BasarisizlikOlustur("Ses planı oluşturulamadı: " + hata.Message);
        }
    }

    public KurulumIslemSonucu Durum(string oyunDizini)
    {
        try
        {
            var ortam = new OyunOrtamiAlgilayici().Tara(oyunDizini);
            BadParentingYukleyicisiniDogrula(ortam);
            var plan = BepInEx5KurulumPlanlayici.Olustur(ortam);
            if (!plan.Uygun)
            {
                return KurulumIslemSonucu.BasarisizlikOlustur(plan.Aciklama);
            }

            if (!File.Exists(plan.EklentiDosyasi))
            {
                return KurulumIslemSonucu.BasarisizlikOlustur("UMMF eklentisi kurulu değil.", plan);
            }

            var badParenting = new BadParentingSesKurulumYoneticisi().Durum(ortam);
            if (badParenting.Uygulanabilir && !badParenting.Basarili)
            {
                return KurulumIslemSonucu.BasarisizlikOlustur(badParenting.Aciklama, plan);
            }

            return KurulumIslemSonucu.BasariOlustur(
                badParenting.Uygulanabilir
                    ? "UMMF eklentisi kurulu. " + badParenting.Aciklama
                    : "UMMF eklentisi kurulu ve dosya erişilebilir.",
                plan,
                DosyaOzetiniHesapla(plan.EklentiDosyasi));
        }
        catch (Exception hata) when (hata is IOException ||
                                     hata is UnauthorizedAccessException ||
                                     hata is ArgumentException ||
                                     hata is DirectoryNotFoundException)
        {
            return KurulumIslemSonucu.BasarisizlikOlustur("Durum denetlenemedi: " + hata.Message);
        }
    }

    public KurulumIslemSonucu RaporOlustur(string oyunDizini)
    {
        try
        {
            var ortam = new OyunOrtamiAlgilayici().Tara(oyunDizini);
            BadParentingYukleyicisiniDogrula(ortam);
            var plan = BepInEx5KurulumPlanlayici.Olustur(ortam);
            if (!plan.Uygun)
            {
                return KurulumIslemSonucu.BasarisizlikOlustur(plan.Aciklama);
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
                badParentingSesModu = new BadParentingSesKurulumYoneticisi().RaporBilgisi(ortam),
                oyunDosyalariDegistirildi = false
            };

            File.WriteAllText(
                raporYolu,
                JsonSerializer.Serialize(rapor, new JsonSerializerOptions { WriteIndented = true }),
                new UTF8Encoding(false));

            return KurulumIslemSonucu.BasariOlustur("Teşhis raporu oluşturuldu: " + raporYolu, plan);
        }
        catch (Exception hata) when (hata is IOException ||
                                     hata is UnauthorizedAccessException ||
                                     hata is ArgumentException ||
                                     hata is DirectoryNotFoundException)
        {
            return KurulumIslemSonucu.BasarisizlikOlustur("Rapor oluşturulamadı: " + hata.Message);
        }
    }

    public KurulumIslemSonucu Kaldir(string oyunDizini)
    {
        try
        {
            var ortam = new OyunOrtamiAlgilayici().Tara(oyunDizini);
            BadParentingYukleyicisiniDogrula(ortam);
            var plan = BepInEx5KurulumPlanlayici.Olustur(ortam);
            if (!plan.Uygun)
            {
                return KurulumIslemSonucu.BasarisizlikOlustur(plan.Aciklama);
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

            var badParenting = new BadParentingSesKurulumYoneticisi().Kaldir(ortam);
            if (badParenting.Uygulanabilir && !badParenting.Basarili)
            {
                return KurulumIslemSonucu.BasarisizlikOlustur(badParenting.Aciklama, plan);
            }

            return KurulumIslemSonucu.BasariOlustur(
                badParenting.Uygulanabilir
                    ? "UMMF plugin DLL'si ve Bad Parenting ses modu kaldırıldı. " + badParenting.Aciklama
                    : "UMMF plugin DLL'si kaldırıldı. Kullanıcı modları ve raporlar korunmuştur.",
                plan);
        }
        catch (Exception hata) when (hata is IOException ||
                                     hata is UnauthorizedAccessException ||
                                     hata is ArgumentException ||
                                     hata is DirectoryNotFoundException)
        {
            return KurulumIslemSonucu.BasarisizlikOlustur("Kaldırma tamamlanamadı: " + hata.Message);
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

    private static BepInEx5KurulumPlani BosPlanOlustur(UMMF.Sozlesmeler.OyunOrtami ortam)
    {
        var bepinex = Path.Combine(ortam.OyunDizini, "BepInEx");
        return new BepInEx5KurulumPlani
        {
            Uygun = true,
            Aciklama = "Dry-run ses planı",
            EklentiDizini = Path.Combine(bepinex, "plugins", "UMMF"),
            EklentiDosyasi = Path.Combine(bepinex, "plugins", "UMMF", BepInEx5KurulumPlanlayici.EklentiDosyaAdi),
            UMMFKokDizini = Path.Combine(bepinex, "UMMF"),
            ModDizini = Path.Combine(bepinex, "UMMF", "modlar"),
            RaporDizini = Path.Combine(bepinex, "UMMF", "raporlar")
        };
    }

    private static void BadParentingYukleyicisiniDogrula(UMMF.Sozlesmeler.OyunOrtami ortam)
    {
        if (ortam.Yukleyici.AnaSurum.HasValue || !BadParentingProfili.Tani(ortam).Eslesiyor)
        {
            return;
        }

        var bepinex = Path.Combine(ortam.OyunDizini, "BepInEx");
        var durum = Path.Combine(bepinex, "UMMF", "bad-parenting-kurulum.json");
        if (File.Exists(durum) && File.Exists(Path.Combine(bepinex, "core", "BepInEx.dll")))
        {
            ortam.Yukleyici = new UMMF.Sozlesmeler.YukleyiciBilgisi
            {
                Tur = UMMF.Sozlesmeler.YukleyiciTuru.BepInEx,
                AnaSurum = 5,
                Surum = "5.4.23.5 (Bad Parenting adapter kaydı)",
                KokDizin = bepinex
            };
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

    public static KurulumIslemSonucu BasariOlustur(
        string aciklama,
        BepInEx5KurulumPlani plan,
        string? sha256 = null)
    {
        return new KurulumIslemSonucu(true, aciklama, plan, sha256);
    }

    public static KurulumIslemSonucu BasarisizlikOlustur(
        string aciklama,
        BepInEx5KurulumPlani? plan = null)
    {
        return new KurulumIslemSonucu(false, aciklama, plan, null);
    }
}
