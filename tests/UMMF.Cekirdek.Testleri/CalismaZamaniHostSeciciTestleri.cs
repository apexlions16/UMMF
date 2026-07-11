using UMMF.Cekirdek;
using UMMF.Sozlesmeler;

namespace UMMF.Cekirdek.Testleri;

public sealed class CalismaZamaniHostSeciciTestleri
{
    [Theory]
    [InlineData(BetikArkaUcu.Mono, 4, "eski-bepinex-mono")]
    [InlineData(BetikArkaUcu.Mono, 5, "bepinex5-mono")]
    [InlineData(BetikArkaUcu.Mono, 6, "bepinex6-mono")]
    [InlineData(BetikArkaUcu.IL2CPP, 6, "bepinex6-il2cpp")]
    public void Sec_DesteklenenOrtamaUygunHostuSecer(
        BetikArkaUcu arkaUc,
        int bepinexAnaSurumu,
        string beklenenHostKimligi)
    {
        var ortam = BepInExOrtamiOlustur(arkaUc, bepinexAnaSurumu);
        var secici = new CalismaZamaniHostSecici(VarsayilanCalismaZamaniHostlari.Olustur());

        var sonuc = secici.Sec(ortam);

        Assert.True(sonuc.HostBulundu);
        Assert.NotNull(sonuc.Host);
        Assert.Equal(beklenenHostKimligi, sonuc.Host!.Kimlik);
        Assert.True(sonuc.Degerlendirme.Uyumlu);
        Assert.True(sonuc.Degerlendirme.Calistirilabilir);
    }

    [Theory]
    [InlineData(BetikArkaUcu.Mono, "unity-mono-kurulum")]
    [InlineData(BetikArkaUcu.IL2CPP, "unity-il2cpp-kurulum")]
    public void Sec_YukleyicisizOyundaKurulumAdayiniSecer(
        BetikArkaUcu arkaUc,
        string beklenenHostKimligi)
    {
        var ortam = new OyunOrtami
        {
            OyunDizini = "ornek",
            BetikArkaUcu = arkaUc,
            Yukleyici = new YukleyiciBilgisi { Tur = YukleyiciTuru.Yok }
        };
        var secici = new CalismaZamaniHostSecici(VarsayilanCalismaZamaniHostlari.Olustur());

        var sonuc = secici.Sec(ortam);

        Assert.Equal(beklenenHostKimligi, sonuc.Host?.Kimlik);
        Assert.True(sonuc.Degerlendirme.Uyumlu);
        Assert.False(sonuc.Degerlendirme.Calistirilabilir);
    }

    [Fact]
    public void Sec_BilinmeyenArkaUctaHostSecmez()
    {
        var ortam = new OyunOrtami
        {
            OyunDizini = "ornek",
            BetikArkaUcu = BetikArkaUcu.Bilinmiyor
        };
        var secici = new CalismaZamaniHostSecici(VarsayilanCalismaZamaniHostlari.Olustur());

        var sonuc = secici.Sec(ortam);

        Assert.False(sonuc.HostBulundu);
        Assert.False(sonuc.Degerlendirme.Uyumlu);
    }

    [Fact]
    public void Il2CppHostu_MonoHostundanFarkliYetenekBildirir()
    {
        var hostlar = VarsayilanCalismaZamaniHostlari.Olustur();
        var mono = Assert.Single(hostlar, host => host.Kimlik == "bepinex6-mono");
        var il2cpp = Assert.Single(hostlar, host => host.Kimlik == "bepinex6-il2cpp");

        Assert.True(mono.Yetenekler.HasFlag(CalismaZamaniYetenegi.MonoBaglantisi));
        Assert.False(mono.Yetenekler.HasFlag(CalismaZamaniYetenegi.Il2CppBaglantisi));
        Assert.True(il2cpp.Yetenekler.HasFlag(CalismaZamaniYetenegi.Il2CppBaglantisi));
        Assert.False(il2cpp.Yetenekler.HasFlag(CalismaZamaniYetenegi.MonoBaglantisi));
    }

    private static OyunOrtami BepInExOrtamiOlustur(BetikArkaUcu arkaUc, int anaSurum)
    {
        return new OyunOrtami
        {
            OyunDizini = "ornek",
            BetikArkaUcu = arkaUc,
            Mimari = IslemciMimarisi.X64,
            IsletimSistemi = IsletimSistemiTuru.Windows,
            Yukleyici = new YukleyiciBilgisi
            {
                Tur = YukleyiciTuru.BepInEx,
                AnaSurum = anaSurum,
                Surum = anaSurum + ".x"
            }
        };
    }
}
