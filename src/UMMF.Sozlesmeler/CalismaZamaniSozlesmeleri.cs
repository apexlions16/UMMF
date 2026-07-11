using System;

namespace UMMF.Sozlesmeler;

public enum BetikArkaUcu
{
    Bilinmiyor = 0,
    Mono = 1,
    IL2CPP = 2
}

public enum IslemciMimarisi
{
    Bilinmiyor = 0,
    X86 = 1,
    X64 = 2,
    Arm32 = 3,
    Arm64 = 4
}

public enum IsletimSistemiTuru
{
    Bilinmiyor = 0,
    Windows = 1,
    Linux = 2,
    MacOS = 3
}

public enum YukleyiciTuru
{
    Yok = 0,
    BepInEx = 1,
    Diger = 2
}

[Flags]
public enum CalismaZamaniYetenegi
{
    Yok = 0,
    OrtamTespiti = 1 << 0,
    ModKesfi = 1 << 1,
    VarlikKatalogu = 1 << 2,
    DokuKesfi = 1 << 3,
    DokuDisariAktarma = 1 << 4,
    DokuDegistirme = 1 << 5,
    SesKesfi = 1 << 6,
    SesDisariAktarma = 1 << 7,
    SesDegistirme = 1 << 8,
    AltyaziYakalama = 1 << 9,
    AltyaziDegistirme = 1 << 10,
    SeslendirmeOynatma = 1 << 11,
    Addressables = 1 << 12,
    Fmod = 1 << 13,
    Wwise = 1 << 14
}

public sealed class YukleyiciBilgisi
{
    public YukleyiciTuru Tur { get; set; }

    public string? Surum { get; set; }

    public int? AnaSurum { get; set; }

    public string? KokDizin { get; set; }

    public bool Kurulu => Tur != YukleyiciTuru.Yok;
}

public sealed class OyunOrtami
{
    public string OyunDizini { get; set; } = string.Empty;

    public string? CalistirilabilirDosya { get; set; }

    public string? VeriDizini { get; set; }

    public string? UnitySurumu { get; set; }

    public BetikArkaUcu BetikArkaUcu { get; set; }

    public IslemciMimarisi Mimari { get; set; }

    public IsletimSistemiTuru IsletimSistemi { get; set; }

    public YukleyiciBilgisi Yukleyici { get; set; } = new();

    public bool TextMeshProVar { get; set; }

    public bool UnityUiVar { get; set; }

    public bool AddressablesVar { get; set; }

    public bool FmodVar { get; set; }

    public bool WwiseVar { get; set; }

    public bool UnityOyunuMu => !string.IsNullOrWhiteSpace(VeriDizini) || BetikArkaUcu != BetikArkaUcu.Bilinmiyor;
}

public sealed class HostUyumlulukSonucu
{
    public bool Uyumlu { get; set; }

    public bool Calistirilabilir { get; set; }

    public string Aciklama { get; set; } = string.Empty;

    public static HostUyumlulukSonucu Uyumsuz(string aciklama)
    {
        return new HostUyumlulukSonucu
        {
            Uyumlu = false,
            Calistirilabilir = false,
            Aciklama = aciklama
        };
    }

    public static HostUyumlulukSonucu UyumluSonuc(bool calistirilabilir, string aciklama)
    {
        return new HostUyumlulukSonucu
        {
            Uyumlu = true,
            Calistirilabilir = calistirilabilir,
            Aciklama = aciklama
        };
    }
}

public interface ICalismaZamaniHostu
{
    string Kimlik { get; }

    string Ad { get; }

    int Oncelik { get; }

    CalismaZamaniYetenegi Yetenekler { get; }

    HostUyumlulukSonucu Degerlendir(OyunOrtami ortam);
}

public sealed class HostSecimSonucu
{
    public ICalismaZamaniHostu? Host { get; set; }

    public HostUyumlulukSonucu Degerlendirme { get; set; } = HostUyumlulukSonucu.Uyumsuz("Uygun host bulunamadı.");

    public bool HostBulundu => Host is not null;
}
