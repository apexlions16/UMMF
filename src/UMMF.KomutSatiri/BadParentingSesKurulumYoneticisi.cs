using System.Text;
using System.Text.Json;
using UMMF.Cekirdek;
using UMMF.Sozlesmeler;

namespace UMMF.KomutSatiri;

internal sealed class BadParentingSesKurulumYoneticisi
{
    private const string ModKimligi = "bad-parenting-ses";
    private const string DurumDosyasiAdi = "bad-parenting-kurulum.json";

    public BadParentingSesIslemSonucu Planla(OyunOrtami ortam)
    {
        var profil = BadParentingProfili.Tani(ortam);
        if (!profil.Eslesiyor)
        {
            return BadParentingSesIslemSonucu.Uygulanmaz(profil.Aciklama);
        }

        var manifest = Path.Combine(profil.ModKaynakDizini, "Data", "dub_manifest.csv");
        var dub = Path.Combine(profil.ModKaynakDizini, "Dub");
        var dogrulama = BadParentingSesEslestirmeDogrulayici.Dogrula(manifest, dub);
        if (!dogrulama.Gecerli)
        {
            return BadParentingSesIslemSonucu.Hata("Ses eşleme doğrulaması başarısız: " + string.Join(" ", dogrulama.Hatalar), profil, dogrulama);
        }

        if (profil.BilinenDerleme && dogrulama.ManifestSha256 != BadParentingProfili.BilinenManifestSha256)
        {
            return BadParentingSesIslemSonucu.Hata("Bilinen oyun derlemesinin ses manifesti SHA-256 ile eşleşmiyor.", profil, dogrulama);
        }

        var runtime = Path.Combine(profil.ModKaynakDizini, "BepInEx_runtime");
        var zorunlu = new[]
        {
            Path.Combine(runtime, "winhttp.dll"),
            Path.Combine(runtime, "doorstop_config.ini"),
            Path.Combine(runtime, "BepInEx", "core", "BepInEx.dll")
        };
        if (zorunlu.Any(dosya => !File.Exists(dosya)))
        {
            return BadParentingSesIslemSonucu.Hata("MOD/BepInEx_runtime eksik veya geçersiz.", profil, dogrulama);
        }

        return BadParentingSesIslemSonucu.Basari(
            $"Bad Parenting ses modu planı hazır: BepInEx 5 x86 Mono, UMMF hostu ve {dogrulama.GecerliSesSayisi}/{dogrulama.BeklenenSesSayisi} ses.",
            profil,
            dogrulama);
    }

    public BadParentingSesIslemSonucu Kur(OyunOrtami ortam)
    {
        var plan = Planla(ortam);
        if (!plan.Basarili || plan.Profil is null || plan.Dogrulama is null)
        {
            return plan;
        }

        var oyunKoku = ortam.OyunDizini;
        var ummfKoku = Path.Combine(oyunKoku, "BepInEx", "UMMF");
        var durumYolu = Path.Combine(ummfKoku, DurumDosyasiAdi);
        var onceki = DurumuOku(durumYolu);
        var durum = onceki ?? new BadParentingKurulumDurumu
        {
            OyunProfili = BadParentingProfili.Kimlik,
            KurulumZamaniUtc = DateTime.UtcNow
        };
        var degisenler = new List<BadParentingGeciciDegisiklik>();

        try
        {
            var runtime = Path.Combine(plan.Profil.ModKaynakDizini, "BepInEx_runtime");
            foreach (var kaynak in Directory.EnumerateFiles(runtime, "*", SearchOption.AllDirectories))
            {
                var goreli = Path.GetRelativePath(runtime, kaynak);
                DosyayiKur(oyunKoku, kaynak, goreli, durum, degisenler);
            }

            var modKoku = Path.Combine("BepInEx", "UMMF", "modlar", ModKimligi);
            DosyayiKur(
                oyunKoku,
                Path.Combine(plan.Profil.ModKaynakDizini, "Data", "dub_manifest.csv"),
                Path.Combine(modKoku, "dub_manifest.csv"),
                durum,
                degisenler);

            foreach (var ses in plan.Dogrulama.SesDosyalari)
            {
                DosyayiKur(oyunKoku, ses, Path.Combine(modKoku, "Dub", Path.GetFileName(ses)), durum, degisenler);
            }

            var modJson = "{\n" +
                          "  \"kimlik\": \"tr.ummf.bad-parenting-1-ses\",\n" +
                          "  \"ad\": \"Bad Parenting 1 Türkçe Ses Modu\",\n" +
                          "  \"surum\": \"1.0.0\",\n" +
                          "  \"oyunProfili\": \"" + BadParentingProfili.Kimlik + "\"\n" +
                          "}\n";
            MetniKur(oyunKoku, modJson, Path.Combine(modKoku, "mod.json"), durum, degisenler);

            durum.ManifestSha256 = plan.Dogrulama.ManifestSha256;
            durum.BeklenenSesSayisi = plan.Dogrulama.BeklenenSesSayisi;
            durum.KuruluSesSayisi = plan.Dogrulama.GecerliSesSayisi;
            DurumuYaz(durumYolu, durum);
            GeciciYedekleriSil(degisenler);
            return BadParentingSesIslemSonucu.Basari(
                $"Bad Parenting altyapısı kuruldu; {plan.Dogrulama.GecerliSesSayisi}/{plan.Dogrulama.BeklenenSesSayisi} ses doğrulandı ve uygulandı.",
                plan.Profil,
                plan.Dogrulama);
        }
        catch (Exception hata) when (hata is IOException || hata is UnauthorizedAccessException || hata is InvalidOperationException)
        {
            GeriAl(oyunKoku, degisenler);
            return BadParentingSesIslemSonucu.Hata("Bad Parenting kurulumu geri alındı: " + hata.Message, plan.Profil, plan.Dogrulama);
        }
    }

    public BadParentingSesIslemSonucu Durum(OyunOrtami ortam)
    {
        var profil = BadParentingProfili.Tani(ortam);
        if (!profil.Eslesiyor)
        {
            return BadParentingSesIslemSonucu.Uygulanmaz(profil.Aciklama);
        }

        var durumYolu = Path.Combine(ortam.OyunDizini, "BepInEx", "UMMF", DurumDosyasiAdi);
        var durum = DurumuOku(durumYolu);
        if (durum is null)
        {
            return BadParentingSesIslemSonucu.Hata("Bad Parenting ses modu kurulu değil.", profil);
        }

        var bozuk = durum.Dosyalar.Count(kayit =>
        {
            var yol = Path.Combine(ortam.OyunDizini, kayit.GoreliYol);
            return !File.Exists(yol) || BadParentingProfili.DosyaOzetiniHesapla(yol) != kayit.KuruluSha256;
        });
        return bozuk == 0
            ? BadParentingSesIslemSonucu.Basari($"Bad Parenting ses modu sağlam; {durum.KuruluSesSayisi}/{durum.BeklenenSesSayisi} ses kurulu.", profil)
            : BadParentingSesIslemSonucu.Hata($"Bad Parenting kurulumunda {bozuk} dosya eksik veya değiştirilmiş.", profil);
    }

    public BadParentingSesIslemSonucu Kaldir(OyunOrtami ortam)
    {
        var profil = BadParentingProfili.Tani(ortam);
        if (!profil.Eslesiyor)
        {
            return BadParentingSesIslemSonucu.Uygulanmaz(profil.Aciklama);
        }

        var durumYolu = Path.Combine(ortam.OyunDizini, "BepInEx", "UMMF", DurumDosyasiAdi);
        var durum = DurumuOku(durumYolu);
        if (durum is null)
        {
            return BadParentingSesIslemSonucu.Basari("Bad Parenting ses modu zaten kaldırılmış.", profil);
        }

        var korunan = 0;
        foreach (var kayit in durum.Dosyalar.AsEnumerable().Reverse())
        {
            var hedef = Path.Combine(ortam.OyunDizini, kayit.GoreliYol);
            if (!File.Exists(hedef))
            {
                if (!string.IsNullOrWhiteSpace(kayit.YedekGoreliYolu))
                {
                    var yedek = Path.Combine(ortam.OyunDizini, kayit.YedekGoreliYolu);
                    if (!File.Exists(yedek) || BadParentingProfili.DosyaOzetiniHesapla(yedek) != kayit.YedekSha256)
                    {
                        korunan++;
                        continue;
                    }

                    Directory.CreateDirectory(Path.GetDirectoryName(hedef)!);
                    File.Copy(yedek, hedef, true);
                    File.Delete(yedek);
                }

                continue;
            }

            if (BadParentingProfili.DosyaOzetiniHesapla(hedef) != kayit.KuruluSha256)
            {
                korunan++;
                continue;
            }

            if (!string.IsNullOrWhiteSpace(kayit.YedekGoreliYolu))
            {
                var yedek = Path.Combine(ortam.OyunDizini, kayit.YedekGoreliYolu);
                if (!File.Exists(yedek) || BadParentingProfili.DosyaOzetiniHesapla(yedek) != kayit.YedekSha256)
                {
                    korunan++;
                    continue;
                }

                File.Copy(yedek, hedef, true);
                File.Delete(yedek);
            }
            else if (kayit.UMMFTarafindanOlusturuldu)
            {
                File.Delete(hedef);
            }
        }

        if (korunan > 0)
        {
            return BadParentingSesIslemSonucu.Hata($"Kullanıcı değişikliği bulunan {korunan} dosya korundu; durum kaydı silinmedi.", profil);
        }

        File.Delete(durumYolu);
        BosDizinleriSil(ortam.OyunDizini);
        return BadParentingSesIslemSonucu.Basari("Bad Parenting ses modu kaldırıldı; yedekler geri yüklendi.", profil);
    }

    public object? RaporBilgisi(OyunOrtami ortam)
    {
        var profil = BadParentingProfili.Tani(ortam);
        if (!profil.Eslesiyor)
        {
            return null;
        }

        var durum = DurumuOku(Path.Combine(ortam.OyunDizini, "BepInEx", "UMMF", DurumDosyasiAdi));
        return new
        {
            profil = BadParentingProfili.Kimlik,
            profil.BilinenDerleme,
            profil.SesSistemi,
            kurulu = durum is not null,
            beklenenSesSayisi = durum?.BeklenenSesSayisi ?? 0,
            kuruluSesSayisi = durum?.KuruluSesSayisi ?? 0,
            manifestSha256 = durum?.ManifestSha256,
            izlenenDosyaSayisi = durum?.Dosyalar.Count ?? 0
        };
    }

    private static void DosyayiKur(string oyunKoku, string kaynak, string goreliHedef, BadParentingKurulumDurumu durum, List<BadParentingGeciciDegisiklik> degisenler)
    {
        var veri = File.ReadAllBytes(kaynak);
        VeriyiKur(oyunKoku, veri, goreliHedef, durum, degisenler);
    }

    private static void MetniKur(string oyunKoku, string metin, string goreliHedef, BadParentingKurulumDurumu durum, List<BadParentingGeciciDegisiklik> degisenler)
    {
        VeriyiKur(oyunKoku, new UTF8Encoding(false).GetBytes(metin), goreliHedef, durum, degisenler);
    }

    private static void VeriyiKur(string oyunKoku, byte[] veri, string goreliHedef, BadParentingKurulumDurumu durum, List<BadParentingGeciciDegisiklik> degisenler)
    {
        var guvenliGoreli = goreliHedef.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        var hedef = Path.GetFullPath(Path.Combine(oyunKoku, guvenliGoreli));
        var guvenliKok = Path.GetFullPath(oyunKoku).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
        if (!hedef.StartsWith(guvenliKok, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Kurulum hedefi oyun dizini dışında.");
        }

        var yeniOzet = Ozetle(veri);
        var kayit = durum.Dosyalar.FirstOrDefault(d => d.GoreliYol.Equals(guvenliGoreli, StringComparison.OrdinalIgnoreCase));
        if (kayit is not null && File.Exists(hedef))
        {
            var mevcut = BadParentingProfili.DosyaOzetiniHesapla(hedef);
            if (mevcut != kayit.KuruluSha256)
            {
                throw new InvalidOperationException("Önceki kurulumdan sonra değiştirilen dosya korunuyor: " + guvenliGoreli);
            }

            if (mevcut == yeniOzet)
            {
                return;
            }
        }

        Directory.CreateDirectory(Path.GetDirectoryName(hedef)!);
        if (kayit is null)
        {
            kayit = new BadParentingDosyaKaydi { GoreliYol = guvenliGoreli };
            if (File.Exists(hedef))
            {
                var yedekGoreli = Path.Combine("BepInEx", "UMMF", "yedekler", "bad-parenting", guvenliGoreli);
                var yedek = Path.Combine(oyunKoku, yedekGoreli);
                Directory.CreateDirectory(Path.GetDirectoryName(yedek)!);
                File.Copy(hedef, yedek, true);
                kayit.YedekGoreliYolu = yedekGoreli;
                kayit.YedekSha256 = BadParentingProfili.DosyaOzetiniHesapla(yedek);
            }
            else
            {
                kayit.UMMFTarafindanOlusturuldu = true;
            }

            durum.Dosyalar.Add(kayit);
        }

        var geciciYedek = hedef + ".ummf-onceki";
        if (File.Exists(geciciYedek))
        {
            throw new InvalidOperationException("Önceki yarım işlem dosyası bulundu: " + guvenliGoreli);
        }

        var onceVardi = File.Exists(hedef);
        if (onceVardi)
        {
            File.Copy(hedef, geciciYedek, false);
        }

        degisenler.Add(new BadParentingGeciciDegisiklik
        {
            HedefYolu = hedef,
            GeciciYedekYolu = geciciYedek,
            OnceVardi = onceVardi
        });

        var gecici = hedef + ".ummf-yeni";
        File.WriteAllBytes(gecici, veri);
        File.Copy(gecici, hedef, true);
        File.Delete(gecici);
        kayit.KuruluSha256 = yeniOzet;
    }

    private static void GeriAl(string oyunKoku, IEnumerable<BadParentingGeciciDegisiklik> degisenler)
    {
        foreach (var degisiklik in degisenler.Reverse())
        {
            if (degisiklik.OnceVardi && File.Exists(degisiklik.GeciciYedekYolu))
            {
                File.Copy(degisiklik.GeciciYedekYolu, degisiklik.HedefYolu, true);
            }
            else if (!degisiklik.OnceVardi && File.Exists(degisiklik.HedefYolu))
            {
                File.Delete(degisiklik.HedefYolu);
            }

            if (File.Exists(degisiklik.GeciciYedekYolu))
            {
                File.Delete(degisiklik.GeciciYedekYolu);
            }
        }
    }

    private static void GeciciYedekleriSil(IEnumerable<BadParentingGeciciDegisiklik> degisenler)
    {
        foreach (var degisiklik in degisenler)
        {
            if (File.Exists(degisiklik.GeciciYedekYolu))
            {
                File.Delete(degisiklik.GeciciYedekYolu);
            }
        }
    }

    private static string Ozetle(byte[] veri)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        return Convert.ToHexString(sha.ComputeHash(veri)).ToLowerInvariant();
    }

    private static BadParentingKurulumDurumu? DurumuOku(string yol)
    {
        return File.Exists(yol)
            ? JsonSerializer.Deserialize<BadParentingKurulumDurumu>(File.ReadAllText(yol, Encoding.UTF8))
            : null;
    }

    private static void DurumuYaz(string yol, BadParentingKurulumDurumu durum)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(yol)!);
        var gecici = yol + ".yeni";
        File.WriteAllText(gecici, JsonSerializer.Serialize(durum, new JsonSerializerOptions { WriteIndented = true }), new UTF8Encoding(false));
        File.Copy(gecici, yol, true);
        File.Delete(gecici);
    }

    private static void BosDizinleriSil(string oyunKoku)
    {
        var adaylar = new[]
        {
            Path.Combine(oyunKoku, "BepInEx", "UMMF", "modlar", ModKimligi, "Dub"),
            Path.Combine(oyunKoku, "BepInEx", "UMMF", "modlar", ModKimligi),
            Path.Combine(oyunKoku, "BepInEx", "plugins", "UMMF"),
            Path.Combine(oyunKoku, "BepInEx", "core"),
            Path.Combine(oyunKoku, "BepInEx")
        };
        foreach (var dizin in adaylar)
        {
            if (Directory.Exists(dizin) && !Directory.EnumerateFileSystemEntries(dizin).Any())
            {
                Directory.Delete(dizin);
            }
        }
    }
}

internal sealed class BadParentingSesIslemSonucu
{
    public bool Basarili { get; private init; }
    public bool Uygulanabilir { get; private init; }
    public string Aciklama { get; private init; } = string.Empty;
    public BadParentingProfilSonucu? Profil { get; private init; }
    public SesEslestirmeDogrulamaSonucu? Dogrulama { get; private init; }

    public static BadParentingSesIslemSonucu Basari(string aciklama, BadParentingProfilSonucu profil, SesEslestirmeDogrulamaSonucu? dogrulama = null) =>
        new() { Basarili = true, Uygulanabilir = true, Aciklama = aciklama, Profil = profil, Dogrulama = dogrulama };

    public static BadParentingSesIslemSonucu Hata(string aciklama, BadParentingProfilSonucu profil, SesEslestirmeDogrulamaSonucu? dogrulama = null) =>
        new() { Uygulanabilir = true, Aciklama = aciklama, Profil = profil, Dogrulama = dogrulama };

    public static BadParentingSesIslemSonucu Uygulanmaz(string aciklama) => new() { Aciklama = aciklama };
}

internal sealed class BadParentingKurulumDurumu
{
    public string OyunProfili { get; set; } = string.Empty;
    public DateTime KurulumZamaniUtc { get; set; }
    public string ManifestSha256 { get; set; } = string.Empty;
    public int BeklenenSesSayisi { get; set; }
    public int KuruluSesSayisi { get; set; }
    public List<BadParentingDosyaKaydi> Dosyalar { get; set; } = new();
}

internal sealed class BadParentingDosyaKaydi
{
    public string GoreliYol { get; set; } = string.Empty;
    public string KuruluSha256 { get; set; } = string.Empty;
    public bool UMMFTarafindanOlusturuldu { get; set; }
    public string? YedekGoreliYolu { get; set; }
    public string? YedekSha256 { get; set; }
}

internal sealed class BadParentingGeciciDegisiklik
{
    public string HedefYolu { get; set; } = string.Empty;
    public string GeciciYedekYolu { get; set; } = string.Empty;
    public bool OnceVardi { get; set; }
}
