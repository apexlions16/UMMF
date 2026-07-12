using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UMMF.Sozlesmeler;

namespace UMMF.Cekirdek;

public sealed class BadParentingProfilSonucu
{
    public bool Eslesiyor { get; set; }

    public bool BilinenDerleme { get; set; }

    public string Aciklama { get; set; } = string.Empty;

    public string SesSistemi { get; set; } = string.Empty;

    public string ModKaynakDizini { get; set; } = string.Empty;
}

public static class BadParentingProfili
{
    public const string Kimlik = "bad-parenting-1-mr-red-face";
    public const string BeklenenUnitySurumu = "2022.2.0b16";
    public const string BilinenExeSha256 = "010560faa4359b7fc463487fa5ef2fb3d4745d5f2728c6935cdd2b192e1781a4";
    public const string BilinenAssemblyCSharpSha256 = "a056094e5bddb67580a45ac11eac6eff531b3186f44305744ca3785c0e2043a1";
    public const string BilinenManifestSha256 = "f9234d2b8c56b4a4db90243eec4f9f7a99f1fa071d89db261cfff6b809124e35";

    public static BadParentingProfilSonucu Tani(OyunOrtami ortam)
    {
        if (ortam is null)
        {
            throw new ArgumentNullException(nameof(ortam));
        }

        var sonuc = new BadParentingProfilSonucu
        {
            ModKaynakDizini = Path.Combine(ortam.OyunDizini, "MOD"),
            SesSistemi = "Unity AudioSource + UnityWebRequestAudioModule (FMOD/Wwise yok)"
        };

        var exe = Path.Combine(ortam.OyunDizini, "Bad Parenting 1.exe");
        var veri = Path.Combine(ortam.OyunDizini, "Bad Parenting 1_Data");
        var managed = Path.Combine(veri, "Managed");
        var assemblyCSharp = Path.Combine(managed, "Assembly-CSharp.dll");
        var appInfo = Path.Combine(veri, "app.info");

        if (!File.Exists(exe) || !Directory.Exists(veri) ||
            !YolEsit(ortam.CalistirilabilirDosya, exe) || !YolEsit(ortam.VeriDizini, veri))
        {
            sonuc.Aciklama = "Bad Parenting 1 EXE/Data eşleşmesi bulunamadı.";
            return sonuc;
        }

        if (ortam.BetikArkaUcu != BetikArkaUcu.Mono || ortam.Mimari != IslemciMimarisi.X86)
        {
            sonuc.Aciklama = "Bad Parenting profili yalnızca doğrulanan Unity Mono x86 sürümünü destekler.";
            return sonuc;
        }

        if (!string.Equals(ortam.UnitySurumu, BeklenenUnitySurumu, StringComparison.Ordinal))
        {
            sonuc.Aciklama = $"Desteklenmeyen Unity sürümü: {ortam.UnitySurumu ?? "bilinmiyor"}; beklenen: {BeklenenUnitySurumu}.";
            return sonuc;
        }

        if (!AppBilgisiDogru(appInfo) || !File.Exists(assemblyCSharp) ||
            !File.Exists(Path.Combine(managed, "UnityEngine.AudioModule.dll")) ||
            !File.Exists(Path.Combine(managed, "UnityEngine.UnityWebRequestAudioModule.dll")) ||
            !AssemblyDiyalogIzleriniIceriyor(assemblyCSharp))
        {
            sonuc.Aciklama = "Ürün veya Dialogue/Lines/Unity ses metadata izleri doğrulanamadı.";
            return sonuc;
        }

        sonuc.Eslesiyor = true;
        sonuc.BilinenDerleme = DosyaOzetiniHesapla(exe) == BilinenExeSha256 &&
                               DosyaOzetiniHesapla(assemblyCSharp) == BilinenAssemblyCSharpSha256;
        sonuc.Aciklama = sonuc.BilinenDerleme
            ? "Bad Parenting 1: Mr. Red Face bilinen Steam derlemesi doğrulandı."
            : "Bad Parenting 1: Mr. Red Face metadata ile doğrulandı; dosya özeti bilinen derlemeden farklı.";
        return sonuc;
    }

    private static bool AppBilgisiDogru(string yol)
    {
        if (!File.Exists(yol))
        {
            return false;
        }

        var satirlar = File.ReadAllLines(yol);
        return satirlar.Length >= 2 &&
               satirlar[0].Trim().Equals("twoootwo", StringComparison.Ordinal) &&
               satirlar[1].Trim().Equals("Bad Parenting 1", StringComparison.Ordinal);
    }

    private static bool AssemblyDiyalogIzleriniIceriyor(string yol)
    {
        const int sinir = 4 * 1024 * 1024;
        using var akis = File.OpenRead(yol);
        var veri = new byte[(int)Math.Min(akis.Length, sinir)];
        var okunan = akis.Read(veri, 0, veri.Length);
        var metin = Encoding.ASCII.GetString(veri, 0, okunan);
        return metin.Contains("Dialogue") && metin.Contains("Lines") && metin.Contains("StartDialogue");
    }

    private static bool YolEsit(string? sol, string sag)
    {
        return !string.IsNullOrWhiteSpace(sol) &&
               Path.GetFullPath(sol!).TrimEnd(Path.DirectorySeparatorChar)
                   .Equals(Path.GetFullPath(sag).TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase);
    }

    public static string DosyaOzetiniHesapla(string yol)
    {
        using var akis = File.OpenRead(yol);
        using var sha = SHA256.Create();
        return BitConverter.ToString(sha.ComputeHash(akis)).Replace("-", string.Empty).ToLowerInvariant();
    }
}

public sealed class SesEslestirmeDogrulamaSonucu
{
    public bool Gecerli => Hatalar.Count == 0;

    public int BeklenenSesSayisi { get; set; }

    public int GecerliSesSayisi { get; set; }

    public int EksikSesSayisi => BeklenenSesSayisi - GecerliSesSayisi;

    public string ManifestSha256 { get; set; } = string.Empty;

    public List<string> Hatalar { get; } = new();

    public List<string> SesDosyalari { get; } = new();
}

public static class BadParentingSesEslestirmeDogrulayici
{
    public const int BeklenenSatirSayisi = 214;

    public static SesEslestirmeDogrulamaSonucu Dogrula(string manifestYolu, string sesDizini)
    {
        var sonuc = new SesEslestirmeDogrulamaSonucu();
        if (!File.Exists(manifestYolu))
        {
            sonuc.Hatalar.Add("dub_manifest.csv bulunamadı.");
            return sonuc;
        }

        sonuc.ManifestSha256 = BadParentingProfili.DosyaOzetiniHesapla(manifestYolu);
        List<List<string>> satirlar;
        try
        {
            satirlar = CsvOku(File.ReadAllText(manifestYolu, Encoding.UTF8));
        }
        catch (InvalidDataException hata)
        {
            sonuc.Hatalar.Add(hata.Message);
            return sonuc;
        }

        if (satirlar.Count == 0 || satirlar[0].Count < 2 ||
            satirlar[0][0] != "index" || satirlar[0][1] != "expected_filename")
        {
            sonuc.Hatalar.Add("Manifest başlığı geçersiz.");
            return sonuc;
        }

        var kayitlar = satirlar.Skip(1).Where(s => s.Any(a => a.Length > 0)).ToList();
        sonuc.BeklenenSesSayisi = kayitlar.Count;
        if (kayitlar.Count != BeklenenSatirSayisi)
        {
            sonuc.Hatalar.Add($"Manifest {BeklenenSatirSayisi} yerine {kayitlar.Count} kayıt içeriyor.");
        }

        var gorulen = new HashSet<int>();
        foreach (var satir in kayitlar)
        {
            if (satir.Count < 2 || !int.TryParse(satir[0], out var indeks) || indeks < 0 || indeks >= BeklenenSatirSayisi)
            {
                sonuc.Hatalar.Add("Geçersiz satır indeksi bulundu.");
                continue;
            }

            if (!gorulen.Add(indeks))
            {
                sonuc.Hatalar.Add($"Yinelenen satır indeksi: {indeks}.");
                continue;
            }

            var beklenenOnEk = indeks.ToString("D3") + "_";
            var dosyaAdi = Path.GetFileName(satir[1]);
            if (!dosyaAdi.StartsWith(beklenenOnEk, StringComparison.OrdinalIgnoreCase) ||
                !DesteklenenUzanti(Path.GetExtension(dosyaAdi)))
            {
                sonuc.Hatalar.Add($"Geçersiz ses eşlemesi: {satir[1]}.");
                continue;
            }

            var beklenenGovde = Path.GetFileNameWithoutExtension(dosyaAdi);
            var bulunanlar = Directory.Exists(sesDizini)
                ? Directory.EnumerateFiles(sesDizini, indeks.ToString("D3") + "_*", SearchOption.TopDirectoryOnly)
                    .Where(d => DesteklenenUzanti(Path.GetExtension(d)) &&
                                Path.GetFileNameWithoutExtension(d).Equals(beklenenGovde, StringComparison.OrdinalIgnoreCase)).ToList()
                : new List<string>();
            if (bulunanlar.Count > 1)
            {
                sonuc.Hatalar.Add($"{indeks:D3} için birden çok ses dosyası var.");
            }
            else if (bulunanlar.Count == 1)
            {
                if (SesBasligiGecerli(bulunanlar[0]))
                {
                    sonuc.GecerliSesSayisi++;
                    sonuc.SesDosyalari.Add(bulunanlar[0]);
                }
                else
                {
                    sonuc.Hatalar.Add($"Ses başlığı geçersiz: {Path.GetFileName(bulunanlar[0])}.");
                }
            }
        }

        return sonuc;
    }

    private static bool DesteklenenUzanti(string uzanti)
    {
        return uzanti.Equals(".wav", StringComparison.OrdinalIgnoreCase) ||
               uzanti.Equals(".ogg", StringComparison.OrdinalIgnoreCase) ||
               uzanti.Equals(".mp3", StringComparison.OrdinalIgnoreCase);
    }

    private static bool SesBasligiGecerli(string yol)
    {
        using var akis = File.OpenRead(yol);
        var baslik = new byte[12];
        var adet = akis.Read(baslik, 0, baslik.Length);
        if (adet < 4)
        {
            return false;
        }

        var uzanti = Path.GetExtension(yol);
        if (uzanti.Equals(".wav", StringComparison.OrdinalIgnoreCase))
        {
            return adet >= 12 && Encoding.ASCII.GetString(baslik, 0, 4) == "RIFF" &&
                   Encoding.ASCII.GetString(baslik, 8, 4) == "WAVE";
        }

        if (uzanti.Equals(".ogg", StringComparison.OrdinalIgnoreCase))
        {
            return Encoding.ASCII.GetString(baslik, 0, 4) == "OggS";
        }

        return Encoding.ASCII.GetString(baslik, 0, 3) == "ID3" ||
               (baslik[0] == 0xff && (baslik[1] & 0xe0) == 0xe0);
    }

    private static List<List<string>> CsvOku(string metin)
    {
        var sonuc = new List<List<string>>();
        var satir = new List<string>();
        var alan = new StringBuilder();
        var tirnak = false;
        for (var i = 0; i < metin.Length; i++)
        {
            var karakter = metin[i];
            if (karakter == '"')
            {
                if (tirnak && i + 1 < metin.Length && metin[i + 1] == '"')
                {
                    alan.Append('"');
                    i++;
                }
                else
                {
                    tirnak = !tirnak;
                }
            }
            else if (karakter == ',' && !tirnak)
            {
                satir.Add(alan.ToString());
                alan.Clear();
            }
            else if ((karakter == '\r' || karakter == '\n') && !tirnak)
            {
                if (karakter == '\r' && i + 1 < metin.Length && metin[i + 1] == '\n')
                {
                    i++;
                }

                satir.Add(alan.ToString());
                alan.Clear();
                sonuc.Add(satir);
                satir = new List<string>();
            }
            else
            {
                alan.Append(karakter);
            }
        }

        if (tirnak)
        {
            throw new InvalidDataException("Manifest içinde kapanmamış tırnak var.");
        }

        if (alan.Length > 0 || satir.Count > 0)
        {
            satir.Add(alan.ToString());
            sonuc.Add(satir);
        }

        return sonuc;
    }
}
