using UMMF.Cekirdek;
using UMMF.Sozlesmeler;

namespace UMMF.Cekirdek.Testleri;

public sealed class VarlikKimligiTestleri
{
    [Fact]
    public void KaliciAnahtarVarsaAnahtarTabanliKimlikUretir()
    {
        var parmakIzi = new VarlikParmakIzi
        {
            Tur = MedyaVarlikTuru.Altyazi,
            KaliciAnahtar = "Diyalog/Giris/Satir_001"
        };

        var kimlik = VarlikKimligi.KaliciKimlikOlustur(parmakIzi);

        Assert.Equal("altyazi:anahtar:diyalog/giris/satir_001", kimlik);
    }

    [Fact]
    public void KaliciAnahtarYoksaBelirlenimciKimlikUretir()
    {
        var parmakIzi = new VarlikParmakIzi
        {
            Tur = MedyaVarlikTuru.Doku,
            Ad = "AnaMenuLogosu",
            KullanimYolu = "Tuval/AnaMenu/Baslik/Logo",
            Genislik = 2048,
            Yukseklik = 1024
        };

        var ilk = VarlikKimligi.KaliciKimlikOlustur(parmakIzi);
        var ikinci = VarlikKimligi.KaliciKimlikOlustur(parmakIzi);

        Assert.Equal(ilk, ikinci);
        Assert.StartsWith("doku:otomatik:", ilk, StringComparison.Ordinal);
    }
}
