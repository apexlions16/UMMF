using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using BepInEx;
using BepInEx.Logging;
using UnityEngine;

namespace UMMF.BepInEx5.Mono;

[BepInPlugin(EklentiKimligi, EklentiAdi, EklentiSurumu)]
public sealed class Plugin : BaseUnityPlugin
{
    public const string EklentiKimligi = "tr.ummf.bepinex5.mono";
    public const string EklentiAdi = "UMMF BepInEx 5 Mono Hostu";
    public const string EklentiSurumu = "0.4.0-onizleme.1";

#if NET35
    private const string HedefCerceve = "net35";
#else
    private const string HedefCerceve = "netstandard2.0";
#endif

    private ManualLogSource _gunluk;

    private void Awake()
    {
        _gunluk = Logger;

        try
        {
            _gunluk.LogInfo("UMMF " + EklentiSurumu + " başlatılıyor.");
            _gunluk.LogInfo("Çalışma zamanı hostu: BepInEx 5 Unity Mono (" + HedefCerceve + ").");
            _gunluk.LogInfo("Oyun: " + GuvenliDeger(Application.productName));
            _gunluk.LogInfo("Unity sürümü: " + GuvenliDeger(Application.unityVersion));

            var yollar = UMMFYollariniHazirla();
            var modlar = ModBildirimleriniTara(yollar.ModDizini);
            var gecerliSayisi = 0;
            var gecersizSayisi = 0;

            for (var i = 0; i < modlar.Count; i++)
            {
                var mod = modlar[i];
                if (mod.Gecerli)
                {
                    gecerliSayisi++;
                    _gunluk.LogInfo("Mod bulundu: " + mod.Ad + " (" + mod.Kimlik + ", " + mod.Surum + ")");
                }
                else
                {
                    gecersizSayisi++;
                    _gunluk.LogWarning("Geçersiz mod bildirimi: " + mod.DosyaYolu + " — " + mod.Aciklama);
                }
            }

            var raporYolu = UyumlulukRaporuYaz(yollar, modlar, gecerliSayisi, gecersizSayisi);
            _gunluk.LogInfo("UMMF mod klasörü hazır: " + yollar.ModDizini);
            _gunluk.LogInfo("Geçerli mod sayısı: " + gecerliSayisi + "; geçersiz mod sayısı: " + gecersizSayisi + ".");
            _gunluk.LogInfo("Uyumluluk raporu: " + raporYolu);
            _gunluk.LogInfo("UMMF başlangıcı başarıyla tamamlandı.");
        }
        catch (Exception hata)
        {
            _gunluk.LogError("UMMF güvenli başlangıç sırasında hata verdi ve medya işlemleri devre dışı bırakıldı.");
            _gunluk.LogError(hata);
        }
    }

    private static UMMFYollari UMMFYollariniHazirla()
    {
        var kok = Path.Combine(Paths.BepInExRootPath, "UMMF");
        var modlar = Path.Combine(kok, "modlar");
        var raporlar = Path.Combine(kok, "raporlar");

        Directory.CreateDirectory(kok);
        Directory.CreateDirectory(modlar);
        Directory.CreateDirectory(raporlar);

        return new UMMFYollari(kok, modlar, raporlar);
    }

    private static List<ModKaydi> ModBildirimleriniTara(string modDizini)
    {
        var sonuclar = new List<ModKaydi>();
        string[] dosyalar;

        try
        {
            dosyalar = Directory.GetFiles(modDizini, "mod.json", SearchOption.AllDirectories);
        }
        catch (Exception hata)
        {
            sonuclar.Add(ModKaydi.Gecersiz(modDizini, "Mod klasörü taranamadı: " + hata.Message));
            return sonuclar;
        }

        Array.Sort(dosyalar, StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < dosyalar.Length; i++)
        {
            sonuclar.Add(ModBildirimiOku(dosyalar[i]));
        }

        return sonuclar;
    }

    private static ModKaydi ModBildirimiOku(string dosyaYolu)
    {
        try
        {
            var json = File.ReadAllText(dosyaYolu, Encoding.UTF8);
            if (BosMu(json))
            {
                return ModKaydi.Gecersiz(dosyaYolu, "Dosya boş.");
            }

            var kimlik = MetinAlaniniOku(json, "kimlik");
            var ad = MetinAlaniniOku(json, "ad");
            var surum = MetinAlaniniOku(json, "surum");

            if (BosMu(kimlik) || BosMu(ad) || BosMu(surum))
            {
                return ModKaydi.Gecersiz(dosyaYolu, "kimlik, ad veya surum alanı eksik.");
            }

            return ModKaydi.GecerliKayit(dosyaYolu, kimlik, ad, surum);
        }
        catch (Exception hata)
        {
            return ModKaydi.Gecersiz(dosyaYolu, hata.Message);
        }
    }

    private static string MetinAlaniniOku(string json, string alan)
    {
        var desen = "\\\"" + Regex.Escape(alan) + "\\\"\\s*:\\s*\\\"(?<deger>(?:\\\\.|[^\\\"])*)\\\"";
        var eslesme = Regex.Match(json, desen, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (!eslesme.Success)
        {
            return null;
        }

        return JsonMetniniCoz(eslesme.Groups["deger"].Value);
    }

    private static string UyumlulukRaporuYaz(UMMFYollari yollar, IList<ModKaydi> modlar, int gecerli, int gecersiz)
    {
        var raporYolu = Path.Combine(yollar.RaporDizini, "uyumluluk-raporu.json");
        var metin = new StringBuilder();
        metin.AppendLine("{");
        JsonSatiriEkle(metin, "ummfSurumu", EklentiSurumu, true, 1);
        JsonSatiriEkle(metin, "host", "BepInEx 5 Unity Mono", true, 1);
        JsonSatiriEkle(metin, "hedefCerceve", HedefCerceve, true, 1);
        JsonSatiriEkle(metin, "olusturmaZamaniUtc", DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture), true, 1);
        JsonSatiriEkle(metin, "oyun", GuvenliDeger(Application.productName), true, 1);
        JsonSatiriEkle(metin, "oyunSurumu", GuvenliDeger(Application.version), true, 1);
        JsonSatiriEkle(metin, "unitySurumu", GuvenliDeger(Application.unityVersion), true, 1);
        JsonSatiriEkle(metin, "mimari", IntPtr.Size == 8 ? "x64" : "x86", true, 1);
        JsonSatiriEkle(metin, "modDizini", yollar.ModDizini, true, 1);
        metin.AppendLine("  \"gecerliModSayisi\": " + gecerli.ToString(CultureInfo.InvariantCulture) + ",");
        metin.AppendLine("  \"gecersizModSayisi\": " + gecersiz.ToString(CultureInfo.InvariantCulture) + ",");
        metin.AppendLine("  \"modlar\": [");

        for (var i = 0; i < modlar.Count; i++)
        {
            var mod = modlar[i];
            metin.AppendLine("    {");
            JsonSatiriEkle(metin, "dosya", mod.DosyaYolu, true, 3);
            JsonSatiriEkle(metin, "kimlik", mod.Kimlik, true, 3);
            JsonSatiriEkle(metin, "ad", mod.Ad, true, 3);
            JsonSatiriEkle(metin, "surum", mod.Surum, true, 3);
            metin.AppendLine("      \"gecerli\": " + (mod.Gecerli ? "true" : "false") + ",");
            JsonSatiriEkle(metin, "aciklama", mod.Aciklama, false, 3);
            metin.Append("    }");
            metin.AppendLine(i + 1 < modlar.Count ? "," : string.Empty);
        }

        metin.AppendLine("  ]");
        metin.AppendLine("}");
        File.WriteAllText(raporYolu, metin.ToString(), new UTF8Encoding(false));
        return raporYolu;
    }

    private static void JsonSatiriEkle(StringBuilder metin, string ad, string deger, bool virgul, int girinti)
    {
        metin.Append(new string(' ', girinti * 2));
        metin.Append('"').Append(JsonKacis(ad)).Append("\": \"");
        metin.Append(JsonKacis(deger));
        metin.Append('"');
        if (virgul)
        {
            metin.Append(',');
        }

        metin.AppendLine();
    }

    private static string JsonKacis(string deger)
    {
        if (deger == null)
        {
            return string.Empty;
        }

        return deger
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\r", "\\r")
            .Replace("\n", "\\n")
            .Replace("\t", "\\t");
    }

    private static string JsonMetniniCoz(string deger)
    {
        return deger
            .Replace("\\\"", "\"")
            .Replace("\\/", "/")
            .Replace("\\n", "\n")
            .Replace("\\r", "\r")
            .Replace("\\t", "\t")
            .Replace("\\\\", "\\");
    }

    private static string GuvenliDeger(string deger)
    {
        return BosMu(deger) ? "Bilinmiyor" : deger;
    }

    private static bool BosMu(string deger)
    {
        return deger == null || deger.Trim().Length == 0;
    }

    private sealed class UMMFYollari
    {
        public UMMFYollari(string kokDizin, string modDizini, string raporDizini)
        {
            KokDizin = kokDizin;
            ModDizini = modDizini;
            RaporDizini = raporDizini;
        }

        public string KokDizin { get; private set; }

        public string ModDizini { get; private set; }

        public string RaporDizini { get; private set; }
    }

    private sealed class ModKaydi
    {
        private ModKaydi()
        {
        }

        public string DosyaYolu { get; private set; }

        public string Kimlik { get; private set; }

        public string Ad { get; private set; }

        public string Surum { get; private set; }

        public bool Gecerli { get; private set; }

        public string Aciklama { get; private set; }

        public static ModKaydi GecerliKayit(string dosyaYolu, string kimlik, string ad, string surum)
        {
            return new ModKaydi
            {
                DosyaYolu = dosyaYolu,
                Kimlik = kimlik,
                Ad = ad,
                Surum = surum,
                Gecerli = true,
                Aciklama = "Bildirim ön doğrulamadan geçti."
            };
        }

        public static ModKaydi Gecersiz(string dosyaYolu, string aciklama)
        {
            return new ModKaydi
            {
                DosyaYolu = dosyaYolu,
                Kimlik = string.Empty,
                Ad = string.Empty,
                Surum = string.Empty,
                Gecerli = false,
                Aciklama = aciklama
            };
        }
    }
}
