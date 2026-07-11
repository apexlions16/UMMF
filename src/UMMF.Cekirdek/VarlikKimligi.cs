using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UMMF.Sozlesmeler;

namespace UMMF.Cekirdek;

public static class VarlikKimligi
{
    public static string KaliciKimlikOlustur(VarlikParmakIzi parmakIzi)
    {
        if (parmakIzi is null)
        {
            throw new ArgumentNullException(nameof(parmakIzi));
        }

        var onEk = OnEkAl(parmakIzi.Tur);
        if (!string.IsNullOrWhiteSpace(parmakIzi.KaliciAnahtar))
        {
            return $"{onEk}:anahtar:{YolNormallestir(parmakIzi.KaliciAnahtar)}";
        }

        var kuralliMetin = string.Join("|", new[]
        {
            parmakIzi.Tur.ToString(),
            MetinNormallestir(parmakIzi.Ad),
            YolNormallestir(parmakIzi.Kapsayici),
            YolNormallestir(parmakIzi.KullanimYolu),
            parmakIzi.Genislik?.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
            parmakIzi.Yukseklik?.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
            parmakIzi.SureSaniye?.ToString("0.000", CultureInfo.InvariantCulture) ?? string.Empty,
            parmakIzi.OrneklemeHizi?.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
            parmakIzi.KanalSayisi?.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
            MetinNormallestir(parmakIzi.KaynakMetin)
        });

        using (var sha256 = SHA256.Create())
        {
            var ozet = sha256.ComputeHash(Encoding.UTF8.GetBytes(kuralliMetin));
            var onaltilik = BitConverter.ToString(ozet).Replace("-", string.Empty).ToLowerInvariant();
            return $"{onEk}:otomatik:{onaltilik.Substring(0, 16)}";
        }
    }

    internal static string YolNormallestir(string? deger)
    {
        var normallestirilmis = MetinNormallestir(deger);
        return normallestirilmis.Replace('\\', '/');
    }

    internal static string MetinNormallestir(string? deger)
    {
        if (string.IsNullOrWhiteSpace(deger))
        {
            return string.Empty;
        }

        var bosOlmayanDeger = deger ?? string.Empty;
        return Regex.Replace(bosOlmayanDeger.Trim().ToLowerInvariant(), @"\s+", " ");
    }

    private static string OnEkAl(MedyaVarlikTuru tur)
    {
        switch (tur)
        {
            case MedyaVarlikTuru.Doku:
                return "doku";
            case MedyaVarlikTuru.GorselParca:
                return "gorsel";
            case MedyaVarlikTuru.Ses:
                return "ses";
            case MedyaVarlikTuru.Altyazi:
                return "altyazi";
            default:
                return "varlik";
        }
    }
}
