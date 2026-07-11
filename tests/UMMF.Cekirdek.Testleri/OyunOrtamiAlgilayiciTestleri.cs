using System.Text;
using UMMF.Cekirdek;
using UMMF.Sozlesmeler;

namespace UMMF.Cekirdek.Testleri;

public sealed class OyunOrtamiAlgilayiciTestleri
{
    [Fact]
    public void Tara_MonoOyununuVeBepInEx6DuzeniniAlgilar()
    {
        using var oyun = GeciciOyunDizini.Olustur("OrnekOyun");
        oyun.PeDosyasiYaz("OrnekOyun.exe", 0x8664);
        oyun.DosyaYaz("OrnekOyun_Data/Managed/Assembly-CSharp.dll", Array.Empty<byte>());
        oyun.DosyaYaz("OrnekOyun_Data/Managed/Unity.TextMeshPro.dll", Array.Empty<byte>());
        oyun.MetinYaz("OrnekOyun_Data/globalgamemanagers", "başlık 2019.4.40f1 son");
        oyun.DosyaYaz("BepInEx/core/BepInEx.Unity.Mono.dll", Array.Empty<byte>());

        var ortam = new OyunOrtamiAlgilayici().Tara(oyun.KokDizin);

        Assert.True(ortam.UnityOyunuMu);
        Assert.Equal(BetikArkaUcu.Mono, ortam.BetikArkaUcu);
        Assert.Equal(IslemciMimarisi.X64, ortam.Mimari);
        Assert.Equal(IsletimSistemiTuru.Windows, ortam.IsletimSistemi);
        Assert.Equal("2019.4.40f1", ortam.UnitySurumu);
        Assert.True(ortam.TextMeshProVar);
        Assert.Equal(YukleyiciTuru.BepInEx, ortam.Yukleyici.Tur);
        Assert.Equal(6, ortam.Yukleyici.AnaSurum);
    }

    [Fact]
    public void Tara_Il2CppOyununuVeBepInEx6DuzeniniAlgilar()
    {
        using var oyun = GeciciOyunDizini.Olustur("Il2CppOyun");
        oyun.PeDosyasiYaz("Il2CppOyun.exe", 0x8664);
        oyun.PeDosyasiYaz("GameAssembly.dll", 0x8664);
        oyun.DosyaYaz("Il2CppOyun_Data/il2cpp_data/Metadata/global-metadata.dat", new byte[] { 1, 2, 3 });
        oyun.MetinYaz("Il2CppOyun_Data/globalgamemanagers", "2021.3.12f1");
        oyun.DosyaYaz("BepInEx/core/BepInEx.Unity.IL2CPP.dll", Array.Empty<byte>());
        oyun.DosyaYaz("Il2CppOyun_Data/Managed/Unity.Addressables.dll", Array.Empty<byte>());

        var ortam = new OyunOrtamiAlgilayici().Tara(oyun.KokDizin);

        Assert.Equal(BetikArkaUcu.IL2CPP, ortam.BetikArkaUcu);
        Assert.Equal(IslemciMimarisi.X64, ortam.Mimari);
        Assert.Equal("2021.3.12f1", ortam.UnitySurumu);
        Assert.True(ortam.AddressablesVar);
        Assert.Equal(6, ortam.Yukleyici.AnaSurum);
    }

    [Fact]
    public void Tara_UnityOlmayanKlasordeBilinmeyenOrtamDondurur()
    {
        using var oyun = GeciciOyunDizini.Olustur("BosKlasor");
        oyun.MetinYaz("not.txt", "Unity oyunu değil");

        var ortam = new OyunOrtamiAlgilayici().Tara(oyun.KokDizin);

        Assert.False(ortam.UnityOyunuMu);
        Assert.Equal(BetikArkaUcu.Bilinmiyor, ortam.BetikArkaUcu);
        Assert.Equal(IslemciMimarisi.Bilinmiyor, ortam.Mimari);
        Assert.Equal(YukleyiciTuru.Yok, ortam.Yukleyici.Tur);
    }

    private sealed class GeciciOyunDizini : IDisposable
    {
        private GeciciOyunDizini(string kokDizin)
        {
            KokDizin = kokDizin;
        }

        public string KokDizin { get; }

        public static GeciciOyunDizini Olustur(string oyunAdi)
        {
            var kok = Path.Combine(Path.GetTempPath(), "ummf-test-" + Guid.NewGuid().ToString("N"), oyunAdi);
            Directory.CreateDirectory(kok);
            return new GeciciOyunDizini(kok);
        }

        public void DosyaYaz(string goreliYol, byte[] veri)
        {
            var tamYol = Path.Combine(KokDizin, goreliYol.Replace('/', Path.DirectorySeparatorChar));
            var ustDizin = Path.GetDirectoryName(tamYol);
            if (!string.IsNullOrWhiteSpace(ustDizin))
            {
                Directory.CreateDirectory(ustDizin);
            }

            File.WriteAllBytes(tamYol, veri);
        }

        public void MetinYaz(string goreliYol, string metin)
        {
            DosyaYaz(goreliYol, Encoding.ASCII.GetBytes(metin));
        }

        public void PeDosyasiYaz(string goreliYol, ushort makine)
        {
            var veri = new byte[512];
            veri[0] = 0x4D;
            veri[1] = 0x5A;
            BitConverter.GetBytes(0x80).CopyTo(veri, 0x3C);
            veri[0x80] = 0x50;
            veri[0x81] = 0x45;
            BitConverter.GetBytes(makine).CopyTo(veri, 0x84);
            DosyaYaz(goreliYol, veri);
        }

        public void Dispose()
        {
            try
            {
                var silinecekKok = Directory.GetParent(KokDizin)?.FullName;
                if (!string.IsNullOrWhiteSpace(silinecekKok) && Directory.Exists(silinecekKok))
                {
                    Directory.Delete(silinecekKok, true);
                }
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }
    }
}
