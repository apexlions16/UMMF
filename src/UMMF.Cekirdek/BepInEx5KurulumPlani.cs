using System;
using System.IO;
using UMMF.Sozlesmeler;

namespace UMMF.Cekirdek;

public enum BepInEx5EklentiCercevesi
{
    Net35,
    NetStandard20
}

public sealed class BepInEx5KurulumPlani
{
    public bool Uygun { get; set; }

    public string Aciklama { get; set; } = string.Empty;

    public BepInEx5EklentiCercevesi HedefCerceve { get; set; }

    public string EklentiDizini { get; set; } = string.Empty;

    public string EklentiDosyasi { get; set; } = string.Empty;

    public string UMMFKokDizini { get; set; } = string.Empty;

    public string ModDizini { get; set; } = string.Empty;

    public string RaporDizini { get; set; } = string.Empty;
}

public static class BepInEx5KurulumPlanlayici
{
    public const string EklentiDosyaAdi = "UMMF.BepInEx5.Mono.dll";

    public static BepInEx5KurulumPlani Olustur(OyunOrtami ortam)
    {
        if (ortam is null)
        {
            throw new ArgumentNullException(nameof(ortam));
        }

        if (!ortam.UnityOyunuMu)
        {
            return UygunDegil("Seçilen dizinde doğrulanmış bir Unity oyunu bulunamadı.");
        }

        if (ortam.BetikArkaUcu != BetikArkaUcu.Mono)
        {
            return UygunDegil("Bu kurucu yalnızca Unity Mono oyunlarını destekler; IL2CPP oyuna Mono eklentisi kurulmadı.");
        }

        if (!ortam.Yukleyici.Kurulu || ortam.Yukleyici.AnaSurum != 5)
        {
            return UygunDegil("BepInEx 5 kurulumu bulunamadı. UMMF eklentisi yalnızca doğrulanmış BepInEx 5 Mono ortamına kurulabilir.");
        }

        if (string.IsNullOrWhiteSpace(ortam.Yukleyici.KokDizin))
        {
            return UygunDegil("BepInEx kök dizini belirlenemedi.");
        }

        var bepinexKoku = Path.GetFullPath(ortam.Yukleyici.KokDizin);
        var eklentiDizini = Path.Combine(bepinexKoku, "plugins", "UMMF");
        var ummfKoku = Path.Combine(bepinexKoku, "UMMF");
        var modDizini = Path.Combine(ummfKoku, "modlar");
        var raporDizini = Path.Combine(ummfKoku, "raporlar");
        var cerceve = NetStandardVarMi(ortam)
            ? BepInEx5EklentiCercevesi.NetStandard20
            : BepInEx5EklentiCercevesi.Net35;

        return new BepInEx5KurulumPlani
        {
            Uygun = true,
            Aciklama = cerceve == BepInEx5EklentiCercevesi.NetStandard20
                ? "BepInEx 5 Mono ortamı doğrulandı; netstandard2.0 eklentisi seçildi."
                : "BepInEx 5 Mono ortamı doğrulandı; eski Unity uyumluluğu için net35 eklentisi seçildi.",
            HedefCerceve = cerceve,
            EklentiDizini = eklentiDizini,
            EklentiDosyasi = Path.Combine(eklentiDizini, EklentiDosyaAdi),
            UMMFKokDizini = ummfKoku,
            ModDizini = modDizini,
            RaporDizini = raporDizini
        };
    }

    private static bool NetStandardVarMi(OyunOrtami ortam)
    {
        if (string.IsNullOrWhiteSpace(ortam.VeriDizini))
        {
            return false;
        }

        var managedDizini = Path.Combine(ortam.VeriDizini, "Managed");
        return File.Exists(Path.Combine(managedDizini, "netstandard.dll"));
    }

    private static BepInEx5KurulumPlani UygunDegil(string aciklama)
    {
        return new BepInEx5KurulumPlani
        {
            Uygun = false,
            Aciklama = aciklama
        };
    }
}
