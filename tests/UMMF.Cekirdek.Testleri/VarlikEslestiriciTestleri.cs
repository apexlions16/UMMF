using UMMF.Cekirdek;
using UMMF.Sozlesmeler;

namespace UMMF.Cekirdek.Testleri;

public sealed class VarlikEslestiriciTestleri
{
    [Fact]
    public void BirebirIcerikOzetiTamGuvenDondurur()
    {
        var beklenen = new VarlikParmakIzi
        {
            Tur = MedyaVarlikTuru.Ses,
            IcerikOzeti = "abc123"
        };
        var aday = new VarlikParmakIzi
        {
            Tur = MedyaVarlikTuru.Ses,
            IcerikOzeti = "ABC123"
        };

        var sonuc = VarlikEslestirici.Karsilastir(beklenen, aday);

        Assert.Equal(1.0, sonuc.Guven);
        Assert.Contains("birebir-içerik-özeti", sonuc.Nedenler);
    }

    [Fact]
    public void FarkliTurlerSifirGuvenDondurur()
    {
        var beklenen = new VarlikParmakIzi { Tur = MedyaVarlikTuru.Doku };
        var aday = new VarlikParmakIzi { Tur = MedyaVarlikTuru.Ses };

        var sonuc = VarlikEslestirici.Karsilastir(beklenen, aday);

        Assert.Equal(0.0, sonuc.Guven);
        Assert.Contains("varlık-türü-uyuşmazlığı", sonuc.Nedenler);
    }

    [Fact]
    public void KaliciAnahtarVeBaglamGucluEslesmeOlusturur()
    {
        var beklenen = new VarlikParmakIzi
        {
            Tur = MedyaVarlikTuru.Altyazi,
            KaliciAnahtar = "Diyalog/Giris/Satir_001",
            KullanimYolu = "Tuval/Diyalog/AltyaziMetni",
            KaynakMetin = "Hemen gitmemiz gerekiyor."
        };
        var aday = new VarlikParmakIzi
        {
            Tur = MedyaVarlikTuru.Altyazi,
            KaliciAnahtar = "diyalog/giris/satir_001",
            KullanimYolu = "tuval/diyalog/altyazimetni",
            KaynakMetin = "Hemen gitmemiz gerekiyor!"
        };

        var sonuc = VarlikEslestirici.Karsilastir(beklenen, aday);

        Assert.True(sonuc.Guven >= 0.75);
        Assert.Contains("kalıcı-anahtar", sonuc.Nedenler);
        Assert.Contains("kullanım-yolu", sonuc.Nedenler);
    }

    [Fact]
    public void HashDegisseBileBaglamsalBilgilerPuanlanir()
    {
        var beklenen = new VarlikParmakIzi
        {
            Tur = MedyaVarlikTuru.Doku,
            Ad = "AnaMenuLogosu",
            KullanimYolu = "Tuval/AnaMenu/Baslik/Logo",
            Genislik = 2048,
            Yukseklik = 1024,
            IcerikOzeti = "eski"
        };
        var aday = new VarlikParmakIzi
        {
            Tur = MedyaVarlikTuru.Doku,
            Ad = "AnaMenuLogosu",
            KullanimYolu = "Tuval/AnaMenu/Baslik/Logo",
            Genislik = 2048,
            Yukseklik = 1024,
            IcerikOzeti = "yeni"
        };

        var sonuc = VarlikEslestirici.Karsilastir(beklenen, aday);

        Assert.Equal(0.32, sonuc.Guven, 2);
        Assert.Contains("kullanım-yolu", sonuc.Nedenler);
        Assert.Contains("ad", sonuc.Nedenler);
        Assert.Contains("boyutlar", sonuc.Nedenler);
    }
}
