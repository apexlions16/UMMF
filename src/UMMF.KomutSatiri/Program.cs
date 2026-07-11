using System.Text.Json;
using System.Text.Json.Serialization;
using UMMF.Cekirdek;
using UMMF.Sozlesmeler;

namespace UMMF.KomutSatiri;

internal static class Program
{
    private const string OnizlemeSurumu = "0.2.0-onizleme.1";

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
            "kimlik-demo" => KimlikOrneginiCalistir(),
            "eslestirme-demo" => EslestirmeOrneginiCalistir(),
            "yardim" or "--yardim" or "-y" => YardimiYazdirVeDon(),
            _ => BilinmeyenKomut(args[0])
        };
    }

    private static int BilgiyiYazdir()
    {
        Console.WriteLine($"UMMF {OnizlemeSurumu}");
        Console.WriteLine("Evrensel Medya Modlama Çerçevesi komut satırı önizlemesi");
        Console.WriteLine("Bu önizleme henüz Unity/BepInEx çalışma zamanı bütünleştirmesini içermez.");
        return 0;
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
        Console.WriteLine("  bilgi                         Önizleme bilgilerini gösterir");
        Console.WriteLine("  dogrula <mod.json>            UMMF mod bildirimini doğrular");
        Console.WriteLine("  kimlik-demo                   Kararlı altyazı varlığı kimliği üretir");
        Console.WriteLine("  eslestirme-demo               Güncellemeye dayanıklı eşleştirmeyi gösterir");
        Console.WriteLine("  yardim                        Bu yardımı gösterir");
    }
}
