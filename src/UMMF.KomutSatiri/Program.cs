using System.Text.Json;
using System.Text.Json.Serialization;
using UMMF.Cekirdek;
using UMMF.Sozlesmeler;

namespace UMMF.KomutSatiri;

internal static class Program
{
    internal const string OnizlemeSurumu = "0.6.0-onizleme.1";

    private static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            YardimiYazdir();
            return 0;
        }

        return args[0].ToLowerInvariant() switch
        {
            "bilgi" or "--surum" or "-s" => BilgiyiYazdir(),
            "dogrula" => BildirimiDogrula(args),
            "oyun-tara" => OyunuTara(args),
            "ses-planla" => BepInEx5KomutunuCalistir(args, yonetici => yonetici.SesPlanla(args[1])),
            "kur" => BepInEx5KomutunuCalistir(args, yonetici => yonetici.Kur(args[1])),
            "durum" => BepInEx5KomutunuCalistir(args, yonetici => yonetici.Durum(args[1])),
            "rapor" => BepInEx5KomutunuCalistir(args, yonetici => yonetici.RaporOlustur(args[1])),
            "kaldir" => BepInEx5KomutunuCalistir(args, yonetici => yonetici.Kaldir(args[1])),
            "host-demo" => HostOrneginiCalistir(),
            "kimlik-demo" => KimlikOrneginiCalistir(),
            "eslestirme-demo" => EslestirmeOrneginiCalistir(),
            "yardim" or "--yardim" or "-y" => YardimiYazdirVeDon(),
            _ => BilinmeyenKomut(args[0])
        };
    }

    private static int BilgiyiYazdir()
    {
        Console.WriteLine($"UMMF {OnizlemeSurumu}");
        Console.WriteLine("Evrensel Medya Modlama Çerçevesi");
        Console.WriteLine("Bu sürüm Türkçe Windows masaüstü arayüzünü, BepInEx 5 Unity Mono hostunu ve güvenli kurulum motorunu içerir.");
        Console.WriteLine("Eski Unity oyunları için net35, daha yeni Mono oyunları için netstandard2.0 eklentisi otomatik seçilir.");
        Console.WriteLine("Bad Parenting 1 için satır indeksli harici ses modu desteği etkindir.");
        return 0;
    }

    private static int BepInEx5KomutunuCalistir(
        string[] args,
        Func<BepInEx5KurulumYoneticisi, KurulumIslemSonucu> islem)
    {
        if (args.Length != 2)
        {
            Console.Error.WriteLine($"Kullanım: ummf {args[0]} <oyun-dizini>");
            return 2;
        }

        var sonuc = islem(new BepInEx5KurulumYoneticisi());
        var cikti = sonuc.Basarili ? Console.Out : Console.Error;
        cikti.WriteLine(sonuc.Basarili ? "BAŞARILI" : "BAŞARISIZ");
        cikti.WriteLine(sonuc.Aciklama);

        if (sonuc.Plan is not null)
        {
            cikti.WriteLine($"Hedef çerçeve: {HedefCerceveyiYaz(sonuc.Plan.HedefCerceve)}");
            cikti.WriteLine($"Plugin dosyası: {sonuc.Plan.EklentiDosyasi}");
            cikti.WriteLine($"Mod klasörü: {sonuc.Plan.ModDizini}");
            cikti.WriteLine($"Rapor klasörü: {sonuc.Plan.RaporDizini}");
        }

        if (!string.IsNullOrWhiteSpace(sonuc.Sha256))
        {
            cikti.WriteLine($"Plugin SHA-256: {sonuc.Sha256}");
        }

        return sonuc.Basarili ? 0 : 1;
    }

    private static string HedefCerceveyiYaz(BepInEx5EklentiCercevesi cerceve)
    {
        return cerceve == BepInEx5EklentiCercevesi.Net35 ? "net35" : "netstandard2.0";
    }

    private static int BildirimiDogrula(string[] args)
    {
        if (args.Length != 2)
        {
            Console.Error.WriteLine("Kullanım: ummf dogrula <mod.json-yolu>");
            return 2;
        }

        try
        {
            var ayarlar = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };
            ayarlar.Converters.Add(new JsonStringEnumConverter());

            var json = File.ReadAllText(args[1]);
            var bildirim = JsonSerializer.Deserialize<ModBildirimi>(json, ayarlar);
            if (bildirim is null)
            {
                Console.Error.WriteLine("GEÇERSİZ: dosyada bir mod bildirimi nesnesi bulunamadı.");
                return 1;
            }

            var hatalar = BildirimDogrulayici.Dogrula(bildirim);
            if (hatalar.Count > 0)
            {
                Console.Error.WriteLine("GEÇERSİZ");
                foreach (var hata in hatalar)
                {
                    Console.Error.WriteLine($"- {hata}");
                }

                return 1;
            }

            Console.WriteLine("GEÇERLİ");
            Console.WriteLine($"Mod: {bildirim.Ad} ({bildirim.Kimlik})");
            Console.WriteLine($"Sürüm: {bildirim.Surum}");
            Console.WriteLine($"Değişiklik sayısı: {bildirim.Degisiklikler.Count}");
            foreach (var grup in bildirim.Degisiklikler.GroupBy(oge => oge.Tur).OrderBy(grup => grup.Key))
            {
                Console.WriteLine($"- {grup.Key}: {grup.Count()}");
            }

            return 0;
        }
        catch (FileNotFoundException)
        {
            Console.Error.WriteLine($"Dosya bulunamadı: {args[1]}");
            return 2;
        }
        catch (IOException hata)
        {
            Console.Error.WriteLine($"Mod bildirimi okunamadı: {hata.Message}");
            return 2;
        }
        catch (JsonException hata)
        {
            Console.Error.WriteLine($"GEÇERSİZ JSON: {hata.Message}");
            return 1;
        }
    }

    private static int OyunuTara(string[] args)
    {
        if (args.Length != 2)
        {
            Console.Error.WriteLine("Kullanım: ummf oyun-tara <oyun-dizini>");
            return 2;
        }

        try
        {
            var ortam = new OyunOrtamiAlgilayici().Tara(args[1]);
            var secici = new CalismaZamaniHostSecici(VarsayilanCalismaZamaniHostlari.Olustur());
            var secim = secici.Sec(ortam);

            Console.WriteLine(ortam.UnityOyunuMu ? "Unity oyun ortamı algılandı" : "Unity oyun ortamı doğrulanamadı");
            Console.WriteLine();
            Console.WriteLine($"Oyun dizini: {ortam.OyunDizini}");
            Console.WriteLine($"Çalıştırılabilir dosya: {DegerVeyaBilinmiyor(ortam.CalistirilabilirDosya)}");
            Console.WriteLine($"Veri dizini: {DegerVeyaBilinmiyor(ortam.VeriDizini)}");
            Console.WriteLine($"Unity sürümü: {DegerVeyaBilinmiyor(ortam.UnitySurumu)}");
            Console.WriteLine($"Betik arka ucu: {ortam.BetikArkaUcu}");
            Console.WriteLine($"Mimari: {ortam.Mimari}");
            Console.WriteLine($"İşletim sistemi: {ortam.IsletimSistemi}");
            Console.WriteLine($"Yükleyici: {YukleyiciyiYaz(ortam.Yukleyici)}");
            Console.WriteLine($"TextMeshPro: {VarYok(ortam.TextMeshProVar)}");
            Console.WriteLine($"Unity UI: {VarYok(ortam.UnityUiVar)}");
            Console.WriteLine($"Addressables: {VarYok(ortam.AddressablesVar)}");
            Console.WriteLine($"FMOD: {VarYok(ortam.FmodVar)}");
            Console.WriteLine($"Wwise: {VarYok(ortam.WwiseVar)}");
            Console.WriteLine();

            if (!secim.HostBulundu || secim.Host is null)
            {
                Console.WriteLine("Seçilen host: Yok");
                Console.WriteLine($"Durum: {secim.Degerlendirme.Aciklama}");
                return ortam.UnityOyunuMu ? 1 : 2;
            }

            Console.WriteLine($"Seçilen host: {secim.Host.Ad}");
            Console.WriteLine($"Host kimliği: {secim.Host.Kimlik}");
            Console.WriteLine($"Başlatılabilir: {(secim.Degerlendirme.Calistirilabilir ? "Evet" : "Hayır")}");
            Console.WriteLine($"Yetenekler: {secim.Host.Yetenekler}");
            Console.WriteLine($"Durum: {secim.Degerlendirme.Aciklama}");
            return secim.Degerlendirme.Calistirilabilir ? 0 : 1;
        }
        catch (ArgumentException hata)
        {
            Console.Error.WriteLine(hata.Message);
            return 2;
        }
        catch (DirectoryNotFoundException hata)
        {
            Console.Error.WriteLine(hata.Message);
            return 2;
        }
    }

    private static int HostOrneginiCalistir()
    {
        var ortamlar = new[]
        {
            OrtamOlustur("Eski Unity Mono + eski BepInEx", BetikArkaUcu.Mono, 4),
            OrtamOlustur("Unity Mono + BepInEx 5", BetikArkaUcu.Mono, 5),
            OrtamOlustur("Unity Mono + BepInEx 6", BetikArkaUcu.Mono, 6),
            OrtamOlustur("Unity IL2CPP + BepInEx 6", BetikArkaUcu.IL2CPP, 6)
        };

        var secici = new CalismaZamaniHostSecici(VarsayilanCalismaZamaniHostlari.Olustur());
        foreach (var ortam in ortamlar)
        {
            var sonuc = secici.Sec(ortam.Ortam);
            Console.WriteLine($"{ortam.Ad}: {sonuc.Host?.Ad ?? "Host bulunamadı"}");
        }

        return 0;
    }

    private static (string Ad, OyunOrtami Ortam) OrtamOlustur(string ad, BetikArkaUcu arkaUc, int bepinexAnaSurumu)
    {
        return (
            ad,
            new OyunOrtami
            {
                OyunDizini = ad,
                BetikArkaUcu = arkaUc,
                Mimari = IslemciMimarisi.X64,
                IsletimSistemi = IsletimSistemiTuru.Windows,
                Yukleyici = new YukleyiciBilgisi
                {
                    Tur = YukleyiciTuru.BepInEx,
                    AnaSurum = bepinexAnaSurumu,
                    Surum = bepinexAnaSurumu + ".x"
                }
            });
    }

    private static int KimlikOrneginiCalistir()
    {
        var parmakIzi = new VarlikParmakIzi
        {
            Tur = MedyaVarlikTuru.Altyazi,
            KaliciAnahtar = "Diyalog/Giris/Satir_001",
            KullanimYolu = "Tuval/Diyalog/AltyaziMetni",
            KaynakMetin = "Hemen gitmemiz gerekiyor."
        };

        Console.WriteLine(VarlikKimligi.KaliciKimlikOlustur(parmakIzi));
        return 0;
    }

    private static int EslestirmeOrneginiCalistir()
    {
        var beklenen = new VarlikParmakIzi
        {
            Tur = MedyaVarlikTuru.Doku,
            Ad = "AnaMenuLogosu",
            KullanimYolu = "Tuval/AnaMenu/Baslik/Logo",
            Genislik = 2048,
            Yukseklik = 1024,
            IcerikOzeti = "eski-yapi-ozeti"
        };

        var guncellenmisOyunVarligi = new VarlikParmakIzi
        {
            Tur = MedyaVarlikTuru.Doku,
            Ad = "AnaMenuLogosu",
            KullanimYolu = "Tuval/AnaMenu/Baslik/Logo",
            Genislik = 2048,
            Yukseklik = 1024,
            IcerikOzeti = "yeni-yapi-ozeti"
        };

        var sonuc = VarlikEslestirici.Karsilastir(beklenen, guncellenmisOyunVarligi);
        Console.WriteLine($"Güven: {sonuc.Guven:0.00}");
        Console.WriteLine($"Nedenler: {string.Join(", ", sonuc.Nedenler)}");
        Console.WriteLine(sonuc.Guven >= 0.75 ? "Karar: uyumlu aday" : "Karar: inceleme gerekli");
        return 0;
    }

    private static string DegerVeyaBilinmiyor(string? deger)
    {
        return string.IsNullOrWhiteSpace(deger) ? "Bilinmiyor" : deger;
    }

    private static string YukleyiciyiYaz(YukleyiciBilgisi yukleyici)
    {
        if (!yukleyici.Kurulu)
        {
            return "Yok";
        }

        return yukleyici.Surum is null
            ? yukleyici.Tur.ToString()
            : $"{yukleyici.Tur} {yukleyici.Surum}";
    }

    private static string VarYok(bool deger)
    {
        return deger ? "Var" : "Yok";
    }

    private static int YardimiYazdirVeDon()
    {
        YardimiYazdir();
        return 0;
    }

    private static int BilinmeyenKomut(string komut)
    {
        Console.Error.WriteLine($"Bilinmeyen komut: {komut}");
        YardimiYazdir();
        return 2;
    }

    private static void YardimiYazdir()
    {
        Console.WriteLine($"UMMF {OnizlemeSurumu}");
        Console.WriteLine();
        Console.WriteLine("Komutlar:");
        Console.WriteLine("  bilgi                         Sürüm ve kapsam bilgilerini gösterir");
        Console.WriteLine("  oyun-tara <oyun-dizini>       Unity, Mono/IL2CPP, mimari ve host ortamını tarar");
        Console.WriteLine("  ses-planla <oyun-dizini>      Bad Parenting ses kurulumunu değiştirmeden doğrular");
        Console.WriteLine("  kur <oyun-dizini>             Uygun BepInEx 5 Mono pluginini güvenli biçimde kurar");
        Console.WriteLine("  durum <oyun-dizini>           UMMF plugin kurulumunu ve SHA-256 özetini denetler");
        Console.WriteLine("  rapor <oyun-dizini>           Kurulum teşhis raporu oluşturur");
        Console.WriteLine("  kaldir <oyun-dizini>          Plugin DLL'sini kaldırır; modları ve raporları korur");
        Console.WriteLine("  dogrula <mod.json>            UMMF mod bildirimini doğrular");
        Console.WriteLine("  host-demo                     Dört örnek ortamda host seçimini gösterir");
        Console.WriteLine("  kimlik-demo                   Kararlı altyazı varlığı kimliği üretir");
        Console.WriteLine("  eslestirme-demo               Güncellemeye dayanıklı eşleştirmeyi gösterir");
        Console.WriteLine("  yardim                        Bu yardımı gösterir");
    }
}
