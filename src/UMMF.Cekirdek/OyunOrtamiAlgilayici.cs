using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UMMF.Sozlesmeler;

namespace UMMF.Cekirdek;

public sealed class OyunOrtamiAlgilayici
{
    private static readonly Regex UnitySurumuDeseni = new(
        @"(?<![0-9])([0-9]{1,4}\.[0-9]+\.[0-9]+[abcfp][0-9]+)(?![0-9])",
        RegexOptions.CultureInvariant);

    public OyunOrtami Tara(string oyunDizini)
    {
        if (string.IsNullOrWhiteSpace(oyunDizini))
        {
            throw new ArgumentException("Oyun dizini boş olamaz.", nameof(oyunDizini));
        }

        var tamDizin = Path.GetFullPath(oyunDizini);
        if (!Directory.Exists(tamDizin))
        {
            throw new DirectoryNotFoundException($"Oyun dizini bulunamadı: {tamDizin}");
        }

        var veriDizini = VeriDizininiBul(tamDizin);
        var calistirilabilirDosya = CalistirilabilirDosyayiBul(tamDizin, veriDizini);
        var betikArkaUcu = BetikArkaUcunuBul(tamDizin, veriDizini);
        var mimariKaynak = betikArkaUcu == BetikArkaUcu.IL2CPP
            ? IlkVarOlanDosya(Path.Combine(tamDizin, "GameAssembly.dll"), calistirilabilirDosya)
            : calistirilabilirDosya;

        return new OyunOrtami
        {
            OyunDizini = tamDizin,
            CalistirilabilirDosya = calistirilabilirDosya,
            VeriDizini = veriDizini,
            UnitySurumu = UnitySurumunuBul(veriDizini),
            BetikArkaUcu = betikArkaUcu,
            Mimari = MimariyiBul(mimariKaynak),
            IsletimSistemi = IsletimSisteminiBul(tamDizin, calistirilabilirDosya),
            Yukleyici = YukleyiciyiBul(tamDizin),
            TextMeshProVar = HerhangiBirDosyaVar(tamDizin, "Unity.TextMeshPro.dll") ||
                             HerhangiBirDosyaVar(tamDizin, "UnityEngine.TextMeshProModule.dll"),
            UnityUiVar = HerhangiBirDosyaVar(tamDizin, "UnityEngine.UI.dll") ||
                         HerhangiBirDosyaVar(tamDizin, "UnityEngine.UIModule.dll"),
            AddressablesVar = HerhangiBirDosyaVar(tamDizin, "Unity.Addressables.dll"),
            FmodVar = HerhangiBirDosyaVar(tamDizin, "FMODUnity.dll") ||
                      HerhangiBirDosyaVar(tamDizin, "fmod.dll") ||
                      HerhangiBirDosyaVar(tamDizin, "fmodstudio.dll"),
            WwiseVar = HerhangiBirDosyaVar(tamDizin, "AkSoundEngine.dll") ||
                       HerhangiBirDosyaVar(tamDizin, "AK.Wwise.Unity.MonoBehaviour.dll")
        };
    }

    private static string? VeriDizininiBul(string oyunDizini)
    {
        try
        {
            return Directory.EnumerateDirectories(oyunDizini, "*_Data", SearchOption.TopDirectoryOnly)
                .OrderBy(dizin => dizin, StringComparer.OrdinalIgnoreCase)
                .FirstOrDefault();
        }
        catch (IOException)
        {
            return null;
        }
        catch (UnauthorizedAccessException)
        {
            return null;
        }
    }

    private static string? CalistirilabilirDosyayiBul(string oyunDizini, string? veriDizini)
    {
        if (!string.IsNullOrWhiteSpace(veriDizini))
        {
            var veriAdi = Path.GetFileName(veriDizini);
            if (veriAdi.EndsWith("_Data", StringComparison.OrdinalIgnoreCase))
            {
                var oyunAdi = veriAdi.Substring(0, veriAdi.Length - "_Data".Length);
                var eslesenExe = Path.Combine(oyunDizini, oyunAdi + ".exe");
                if (File.Exists(eslesenExe))
                {
                    return eslesenExe;
                }

                var eslesenLinux = Path.Combine(oyunDizini, oyunAdi + ".x86_64");
                if (File.Exists(eslesenLinux))
                {
                    return eslesenLinux;
                }
            }
        }

        try
        {
            return Directory.EnumerateFiles(oyunDizini, "*", SearchOption.TopDirectoryOnly)
                .Where(DesteklenenCalistirilabilirDosyaMi)
                .OrderBy(dosya => dosya, StringComparer.OrdinalIgnoreCase)
                .FirstOrDefault();
        }
        catch (IOException)
        {
            return null;
        }
        catch (UnauthorizedAccessException)
        {
            return null;
        }
    }

    private static bool DesteklenenCalistirilabilirDosyaMi(string dosya)
    {
        var ad = Path.GetFileName(dosya);
        if (ad.StartsWith("UnityCrashHandler", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var uzanti = Path.GetExtension(dosya);
        return uzanti.Equals(".exe", StringComparison.OrdinalIgnoreCase) ||
               uzanti.Equals(".x86", StringComparison.OrdinalIgnoreCase) ||
               uzanti.Equals(".x86_64", StringComparison.OrdinalIgnoreCase);
    }

    private static BetikArkaUcu BetikArkaUcunuBul(string oyunDizini, string? veriDizini)
    {
        if (!string.IsNullOrWhiteSpace(veriDizini))
        {
            var metadata = Path.Combine(veriDizini, "il2cpp_data", "Metadata", "global-metadata.dat");
            if (File.Exists(metadata) || File.Exists(Path.Combine(oyunDizini, "GameAssembly.dll")))
            {
                return BetikArkaUcu.IL2CPP;
            }

            var managed = Path.Combine(veriDizini, "Managed");
            if (Directory.Exists(managed) || File.Exists(Path.Combine(managed, "Assembly-CSharp.dll")))
            {
                return BetikArkaUcu.Mono;
            }
        }

        if (Directory.Exists(Path.Combine(oyunDizini, "Mono")) ||
            Directory.Exists(Path.Combine(oyunDizini, "MonoBleedingEdge")))
        {
            return BetikArkaUcu.Mono;
        }

        return BetikArkaUcu.Bilinmiyor;
    }

    private static string? UnitySurumunuBul(string? veriDizini)
    {
        if (string.IsNullOrWhiteSpace(veriDizini))
        {
            return null;
        }

        var adaylar = new[]
        {
            Path.Combine(veriDizini, "globalgamemanagers"),
            Path.Combine(veriDizini, "data.unity3d"),
            Path.Combine(veriDizini, "globalgamemanagers.assets")
        };

        foreach (var aday in adaylar)
        {
            var surum = DosyadanUnitySurumunuOku(aday);
            if (!string.IsNullOrWhiteSpace(surum))
            {
                return surum;
            }
        }

        return null;
    }

    private static string? DosyadanUnitySurumunuOku(string dosya)
    {
        if (!File.Exists(dosya))
        {
            return null;
        }

        try
        {
            const int enFazlaBayt = 8 * 1024 * 1024;
            using var akis = File.OpenRead(dosya);
            var uzunluk = (int)Math.Min(akis.Length, enFazlaBayt);
            var veri = new byte[uzunluk];
            var okunan = akis.Read(veri, 0, veri.Length);
            var metin = Encoding.ASCII.GetString(veri, 0, okunan);
            var eslesme = UnitySurumuDeseni.Match(metin);
            return eslesme.Success ? eslesme.Groups[1].Value : null;
        }
        catch (IOException)
        {
            return null;
        }
        catch (UnauthorizedAccessException)
        {
            return null;
        }
    }

    private static IslemciMimarisi MimariyiBul(string? dosya)
    {
        if (string.IsNullOrWhiteSpace(dosya) || !File.Exists(dosya))
        {
            return IslemciMimarisi.Bilinmiyor;
        }

        try
        {
            using var akis = File.OpenRead(dosya);
            using var okuyucu = new BinaryReader(akis);
            if (akis.Length < 64 || okuyucu.ReadUInt16() != 0x5A4D)
            {
                return IslemciMimarisi.Bilinmiyor;
            }

            akis.Position = 0x3C;
            var peKonumu = okuyucu.ReadInt32();
            if (peKonumu < 0 || peKonumu + 6 > akis.Length)
            {
                return IslemciMimarisi.Bilinmiyor;
            }

            akis.Position = peKonumu;
            if (okuyucu.ReadUInt32() != 0x00004550)
            {
                return IslemciMimarisi.Bilinmiyor;
            }

            return okuyucu.ReadUInt16() switch
            {
                0x014C => IslemciMimarisi.X86,
                0x8664 => IslemciMimarisi.X64,
                0x01C4 => IslemciMimarisi.Arm32,
                0xAA64 => IslemciMimarisi.Arm64,
                _ => IslemciMimarisi.Bilinmiyor
            };
        }
        catch (IOException)
        {
            return IslemciMimarisi.Bilinmiyor;
        }
        catch (UnauthorizedAccessException)
        {
            return IslemciMimarisi.Bilinmiyor;
        }
    }

    private static IsletimSistemiTuru IsletimSisteminiBul(string oyunDizini, string? calistirilabilirDosya)
    {
        if (oyunDizini.EndsWith(".app", StringComparison.OrdinalIgnoreCase))
        {
            return IsletimSistemiTuru.MacOS;
        }

        if (!string.IsNullOrWhiteSpace(calistirilabilirDosya))
        {
            var uzanti = Path.GetExtension(calistirilabilirDosya);
            if (uzanti.Equals(".exe", StringComparison.OrdinalIgnoreCase))
            {
                return IsletimSistemiTuru.Windows;
            }

            if (uzanti.Equals(".x86", StringComparison.OrdinalIgnoreCase) ||
                uzanti.Equals(".x86_64", StringComparison.OrdinalIgnoreCase))
            {
                return IsletimSistemiTuru.Linux;
            }
        }

        return IsletimSistemiTuru.Bilinmiyor;
    }

    private static YukleyiciBilgisi YukleyiciyiBul(string oyunDizini)
    {
        var bepinexDizini = Path.Combine(oyunDizini, "BepInEx");
        if (!Directory.Exists(bepinexDizini))
        {
            return new YukleyiciBilgisi { Tur = YukleyiciTuru.Yok };
        }

        var coreDizini = Path.Combine(bepinexDizini, "core");
        var anaKutuphane = Path.Combine(coreDizini, "BepInEx.dll");
        string? surum = null;
        int? anaSurum = null;

        if (File.Exists(anaKutuphane))
        {
            try
            {
                var bilgi = FileVersionInfo.GetVersionInfo(anaKutuphane);
                surum = bilgi.ProductVersion ?? bilgi.FileVersion;
                anaSurum = AnaSurumuAyristir(surum);
            }
            catch (Exception hata) when (hata is IOException || hata is UnauthorizedAccessException || hata is ArgumentException)
            {
                surum = null;
            }
        }

        if (!anaSurum.HasValue &&
            (File.Exists(Path.Combine(coreDizini, "BepInEx.Unity.Mono.dll")) ||
             File.Exists(Path.Combine(coreDizini, "BepInEx.Unity.IL2CPP.dll"))))
        {
            anaSurum = 6;
            surum = surum ?? "6.x (dosya düzeninden algılandı)";
        }

        return new YukleyiciBilgisi
        {
            Tur = YukleyiciTuru.BepInEx,
            Surum = surum,
            AnaSurum = anaSurum,
            KokDizin = bepinexDizini
        };
    }

    private static int? AnaSurumuAyristir(string? surum)
    {
        if (string.IsNullOrWhiteSpace(surum))
        {
            return null;
        }

        var ilkParca = surum.Split('.', '-', '+')[0];
        return int.TryParse(ilkParca, out var anaSurum) ? anaSurum : null;
    }

    private static bool HerhangiBirDosyaVar(string kokDizin, string dosyaAdi)
    {
        try
        {
            return Directory.EnumerateFiles(kokDizin, dosyaAdi, SearchOption.AllDirectories).Any();
        }
        catch (IOException)
        {
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
    }

    private static string? IlkVarOlanDosya(params string?[] adaylar)
    {
        return adaylar.FirstOrDefault(aday => !string.IsNullOrWhiteSpace(aday) && File.Exists(aday));
    }
}
