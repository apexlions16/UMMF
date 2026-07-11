using UMMF.Cekirdek;
using UMMF.Sozlesmeler;

namespace UMMF.Cekirdek.Testleri;

public sealed class BepInEx5KurulumPlaniTestleri
{
    [Fact]
    public void NetstandardDosyasiVarsaModernEklentiyiSecer()
    {
        using var duzenek = OyunDuzenegi.Olustur(BetikArkaUcu.Mono, 5, netstandardVar: true);

        var plan = BepInEx5KurulumPlanlayici.Olustur(duzenek.Ortam);

        Assert.True(plan.Uygun);
        Assert.Equal(BepInEx5EklentiCercevesi.NetStandard20, plan.HedefCerceve);
        Assert.EndsWith(BepInEx5KurulumPlanlayici.EklentiDosyaAdi, plan.EklentiDosyasi);
    }

    [Fact]
    public void EskiMonoOrtamindaNet35EklentiyiSecer()
    {
        using var duzenek = OyunDuzenegi.Olustur(BetikArkaUcu.Mono, 5, netstandardVar: false);

        var plan = BepInEx5KurulumPlanlayici.Olustur(duzenek.Ortam);

        Assert.True(plan.Uygun);
        Assert.Equal(BepInEx5EklentiCercevesi.Net35, plan.HedefCerceve);
        Assert.Contains("eski Unity", plan.Aciklama, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Il2CppOyununaMonoEklentisiKurmaz()
    {
        using var duzenek = OyunDuzenegi.Olustur(BetikArkaUcu.IL2CPP, 5, netstandardVar: false);

        var plan = BepInEx5KurulumPlanlayici.Olustur(duzenek.Ortam);

        Assert.False(plan.Uygun);
        Assert.Contains("IL2CPP", plan.Aciklama, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BepInEx6OrtaminiBepInEx5KurucusuylaDegistirmez()
    {
        using var duzenek = OyunDuzenegi.Olustur(BetikArkaUcu.Mono, 6, netstandardVar: true);

        var plan = BepInEx5KurulumPlanlayici.Olustur(duzenek.Ortam);

        Assert.False(plan.Uygun);
        Assert.Contains("BepInEx 5", plan.Aciklama, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class OyunDuzenegi : IDisposable
    {
        private OyunDuzenegi(string kokDizin, OyunOrtami ortam)
        {
            KokDizin = kokDizin;
            Ortam = ortam;
        }

        public string KokDizin { get; }

        public OyunOrtami Ortam { get; }

        public static OyunDuzenegi Olustur(BetikArkaUcu arkaUc, int bepinexAnaSurumu, bool netstandardVar)
        {
            var kok = Path.Combine(Path.GetTempPath(), "UMMF-Test-" + Guid.NewGuid().ToString("N"));
            var veri = Path.Combine(kok, "OrnekOyun_Data");
            var managed = Path.Combine(veri, "Managed");
            var bepinex = Path.Combine(kok, "BepInEx");
            Directory.CreateDirectory(managed);
            Directory.CreateDirectory(Path.Combine(bepinex, "core"));

            if (netstandardVar)
            {
                File.WriteAllBytes(Path.Combine(managed, "netstandard.dll"), Array.Empty<byte>());
            }

            var ortam = new OyunOrtami
            {
                OyunDizini = kok,
                VeriDizini = veri,
                BetikArkaUcu = arkaUc,
                Mimari = IslemciMimarisi.X64,
                IsletimSistemi = IsletimSistemiTuru.Windows,
                Yukleyici = new YukleyiciBilgisi
                {
                    Tur = YukleyiciTuru.BepInEx,
                    AnaSurum = bepinexAnaSurumu,
                    Surum = bepinexAnaSurumu + ".4.0",
                    KokDizin = bepinex
                }
            };

            return new OyunDuzenegi(kok, ortam);
        }

        public void Dispose()
        {
            if (Directory.Exists(KokDizin))
            {
                Directory.Delete(KokDizin, recursive: true);
            }
        }
    }
}
