using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using UMMF.Sozlesmeler;

namespace UMMF.Cekirdek;

public sealed class VarlikEslestirmeSonucu
{
    public VarlikEslestirmeSonucu(VarlikParmakIzi aday, double guven, IEnumerable<string> nedenler)
    {
        Aday = aday ?? throw new ArgumentNullException(nameof(aday));
        Guven = Math.Max(0.0, Math.Min(1.0, guven));
        Nedenler = new ReadOnlyCollection<string>(nedenler.ToList());
    }

    public VarlikParmakIzi Aday { get; }

    public double Guven { get; }

    public IReadOnlyList<string> Nedenler { get; }
}

public static class VarlikEslestirici
{
    public static VarlikEslestirmeSonucu Karsilastir(VarlikParmakIzi beklenen, VarlikParmakIzi aday)
    {
        if (beklenen is null)
        {
            throw new ArgumentNullException(nameof(beklenen));
        }

        if (aday is null)
        {
            throw new ArgumentNullException(nameof(aday));
        }

        var nedenler = new List<string>();
        if (beklenen.Tur != MedyaVarlikTuru.Bilinmiyor &&
            aday.Tur != MedyaVarlikTuru.Bilinmiyor &&
            beklenen.Tur != aday.Tur)
        {
            return new VarlikEslestirmeSonucu(aday, 0.0, new[] { "varlık-türü-uyuşmazlığı" });
        }

        if (BosOlmayanlarEsit(beklenen.IcerikOzeti, aday.IcerikOzeti))
        {
            return new VarlikEslestirmeSonucu(aday, 1.0, new[] { "birebir-içerik-özeti" });
        }

        var puan = 0.0;
        BirebirEkle(beklenen.KaliciAnahtar, aday.KaliciAnahtar, 0.55, "kalıcı-anahtar", nedenler, ref puan, true);
        BirebirEkle(beklenen.AnlamsalOzet, aday.AnlamsalOzet, 0.25, "anlamsal-özet", nedenler, ref puan, false);
        BirebirEkle(beklenen.KullanimYolu, aday.KullanimYolu, 0.15, "kullanım-yolu", nedenler, ref puan, true);
        BirebirEkle(beklenen.Kapsayici, aday.Kapsayici, 0.08, "kapsayıcı", nedenler, ref puan, true);
        BirebirEkle(beklenen.Ad, aday.Ad, 0.10, "ad", nedenler, ref puan, false);

        if (beklenen.Genislik.HasValue && beklenen.Yukseklik.HasValue &&
            beklenen.Genislik == aday.Genislik && beklenen.Yukseklik == aday.Yukseklik)
        {
            puan += 0.07;
            nedenler.Add("boyutlar");
        }

        if (beklenen.OrneklemeHizi.HasValue && beklenen.OrneklemeHizi == aday.OrneklemeHizi &&
            beklenen.KanalSayisi.HasValue && beklenen.KanalSayisi == aday.KanalSayisi)
        {
            puan += 0.05;
            nedenler.Add("ses-yapısı");
        }

        if (beklenen.SureSaniye.HasValue && aday.SureSaniye.HasValue)
        {
            var fark = Math.Abs(beklenen.SureSaniye.Value - aday.SureSaniye.Value);
            var tolerans = Math.Max(0.1, beklenen.SureSaniye.Value * 0.02);
            if (fark <= tolerans)
            {
                puan += 0.05;
                nedenler.Add("süre");
            }
        }

        var metinBenzerligi = MetinBenzerligi(beklenen.KaynakMetin, aday.KaynakMetin);
        if (metinBenzerligi >= 0.50)
        {
            puan += 0.20 * metinBenzerligi;
            nedenler.Add("kaynak-metin-benzerliği");
        }

        return new VarlikEslestirmeSonucu(aday, Math.Min(0.99, puan), nedenler);
    }

    private static void BirebirEkle(
        string? sol,
        string? sag,
        double agirlik,
        string neden,
        ICollection<string> nedenler,
        ref double puan,
        bool yolOlarakNormallestir)
    {
        var normSol = yolOlarakNormallestir ? VarlikKimligi.YolNormallestir(sol) : VarlikKimligi.MetinNormallestir(sol);
        var normSag = yolOlarakNormallestir ? VarlikKimligi.YolNormallestir(sag) : VarlikKimligi.MetinNormallestir(sag);

        if (normSol.Length > 0 && string.Equals(normSol, normSag, StringComparison.Ordinal))
        {
            puan += agirlik;
            nedenler.Add(neden);
        }
    }

    private static bool BosOlmayanlarEsit(string? sol, string? sag)
    {
        if (string.IsNullOrWhiteSpace(sol) || string.IsNullOrWhiteSpace(sag))
        {
            return false;
        }

        var bosOlmayanSol = sol ?? string.Empty;
        var bosOlmayanSag = sag ?? string.Empty;
        return string.Equals(bosOlmayanSol.Trim(), bosOlmayanSag.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    private static double MetinBenzerligi(string? sol, string? sag)
    {
        var solParcalar = ParcalaraAyir(sol);
        var sagParcalar = ParcalaraAyir(sag);
        if (solParcalar.Count == 0 || sagParcalar.Count == 0)
        {
            return 0.0;
        }

        var kesisim = solParcalar.Intersect(sagParcalar).Count();
        var birlesim = solParcalar.Union(sagParcalar).Count();
        return birlesim == 0 ? 0.0 : (double)kesisim / birlesim;
    }

    private static HashSet<string> ParcalaraAyir(string? deger)
    {
        var normallestirilmis = VarlikKimligi.MetinNormallestir(deger);
        return new HashSet<string>(
            Regex.Split(normallestirilmis, @"[^\p{L}\p{N}]+")
                .Where(parca => parca.Length > 0),
            StringComparer.Ordinal);
    }
}
