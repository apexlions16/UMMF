using System.Text;
using UMMF.Cekirdek;
using UMMF.KomutSatiri;
using UMMF.Sozlesmeler;

namespace UMMF.Cekirdek.Testleri;

public sealed class BadParentingSesModuTestleri
{
    [Fact]
    public void ProfiliExeDataUrunMonoVeSesMetadataIleTanir()
    {
        using var oyun = BadParentingDuzenegi.Olustur();
        var ortam = new OyunOrtamiAlgilayici().Tara(oyun.Kok);

        var profil = BadParentingProfili.Tani(ortam);

        Assert.True(profil.Eslesiyor);
        Assert.False(profil.BilinenDerleme);
        Assert.Contains("AudioSource", profil.SesSistemi);
        Assert.Equal(BetikArkaUcu.Mono, ortam.BetikArkaUcu);
        Assert.Equal(IslemciMimarisi.X86, ortam.Mimari);
    }

    [Fact]
    public void SesEslestirmesiniVeDosyaBasliginiDogrular()
    {
        using var oyun = BadParentingDuzenegi.Olustur();
        oyun.GecerliWavYaz("MOD/Dub/000_SON.wav");

        var sonuc = BadParentingSesEslestirmeDogrulayici.Dogrula(
            oyun.Yol("MOD/Data/dub_manifest.csv"),
            oyun.Yol("MOD/Dub"));

        Assert.True(sonuc.Gecerli);
        Assert.Equal(214, sonuc.BeklenenSesSayisi);
        Assert.Equal(1, sonuc.GecerliSesSayisi);
        Assert.Equal(213, sonuc.EksikSesSayisi);
    }

    [Fact]
    public void AyniIndekseIkiSesDosyasiniReddeder()
    {
        using var oyun = BadParentingDuzenegi.Olustur();
        oyun.GecerliWavYaz("MOD/Dub/001_MOM.wav");
        oyun.GecerliOggYaz("MOD/Dub/001_MOM.ogg");

        var sonuc = BadParentingSesEslestirmeDogrulayici.Dogrula(
            oyun.Yol("MOD/Data/dub_manifest.csv"),
            oyun.Yol("MOD/Dub"));

        Assert.False(sonuc.Gecerli);
        Assert.Contains(sonuc.Hatalar, hata => hata.Contains("birden çok", StringComparison.Ordinal));
    }

    [Fact]
    public void KurulumPlaniDegisiklikYapmadanHazirlanir()
    {
        using var oyun = BadParentingDuzenegi.Olustur();
        var ortam = new OyunOrtamiAlgilayici().Tara(oyun.Kok);

        var sonuc = new BadParentingSesKurulumYoneticisi().Planla(ortam);

        Assert.True(sonuc.Basarili);
        Assert.False(Directory.Exists(oyun.Yol("BepInEx")));
        Assert.Contains("0/214", sonuc.Aciklama);
    }

    [Fact]
    public void TekrarKurulumVeKaldirmaYedegiGuvenleGeriYukler()
    {
        using var oyun = BadParentingDuzenegi.Olustur();
        oyun.MetinYaz("doorstop_config.ini", "kullanici ayari");
        var yonetici = new BepInEx5KurulumYoneticisi();

        var ilk = yonetici.Kur(oyun.Kok);
        var ikinci = yonetici.Kur(oyun.Kok);
        var durum = yonetici.Durum(oyun.Kok);
        var rapor = yonetici.RaporOlustur(oyun.Kok);
        File.Delete(oyun.Yol("doorstop_config.ini"));
        var kaldir = yonetici.Kaldir(oyun.Kok);

        Assert.True(ilk.Basarili, ilk.Aciklama);
        Assert.True(ikinci.Basarili, ikinci.Aciklama);
        Assert.True(durum.Basarili, durum.Aciklama);
        Assert.True(rapor.Basarili, rapor.Aciklama);
        Assert.True(kaldir.Basarili, kaldir.Aciklama);
        Assert.Equal("kullanici ayari", File.ReadAllText(oyun.Yol("doorstop_config.ini"), Encoding.UTF8));
        Assert.False(File.Exists(oyun.Yol("BepInEx/UMMF/bad-parenting-kurulum.json")));
        Assert.False(File.Exists(oyun.Yol("BepInEx/plugins/UMMF/UMMF.BepInEx5.Mono.dll")));
    }

    private sealed class BadParentingDuzenegi : IDisposable
    {
        private BadParentingDuzenegi(string kok)
        {
            Kok = kok;
        }

        public string Kok { get; }

        public static BadParentingDuzenegi Olustur()
        {
            var kok = Path.Combine(Path.GetTempPath(), "ummf-bad-parenting-" + Guid.NewGuid().ToString("N"));
            var oyun = new BadParentingDuzenegi(kok);
            oyun.PeYaz("Bad Parenting 1.exe", 0x014c);
            oyun.MetinYaz("Bad Parenting 1_Data/app.info", "twoootwo\nBad Parenting 1");
            oyun.MetinYaz("Bad Parenting 1_Data/globalgamemanagers", "2022.2.0b16");
            oyun.MetinYaz("Bad Parenting 1_Data/Managed/Assembly-CSharp.dll", "Dialogue Lines StartDialogue LoadDialogue AudioSource");
            oyun.MetinYaz("Bad Parenting 1_Data/Managed/netstandard.dll", "test");
            oyun.MetinYaz("Bad Parenting 1_Data/Managed/UnityEngine.AudioModule.dll", "test");
            oyun.MetinYaz("Bad Parenting 1_Data/Managed/UnityEngine.UnityWebRequestAudioModule.dll", "test");
            oyun.MetinYaz("MOD/BepInEx_runtime/doorstop_config.ini", "ummf ayari");
            oyun.MetinYaz("MOD/BepInEx_runtime/.doorstop_version", "4.5.0");
            oyun.PeYaz("MOD/BepInEx_runtime/winhttp.dll", 0x014c);
            oyun.MetinYaz("MOD/BepInEx_runtime/BepInEx/core/BepInEx.dll", "BepInEx 5.4.23.5 test");
            oyun.MetinYaz("MOD/BepInEx_runtime/changelog.txt", "BepInEx 5.4.23.5");
            Directory.CreateDirectory(oyun.Yol("MOD/Dub"));
            oyun.ManifestYaz();
            return oyun;
        }

        public string Yol(string goreli) => Path.Combine(Kok, goreli.Replace('/', Path.DirectorySeparatorChar));

        public void MetinYaz(string goreli, string metin)
        {
            DosyaYaz(goreli, new UTF8Encoding(false).GetBytes(metin));
        }

        public void DosyaYaz(string goreli, byte[] veri)
        {
            var yol = Yol(goreli);
            Directory.CreateDirectory(Path.GetDirectoryName(yol)!);
            File.WriteAllBytes(yol, veri);
        }

        public void PeYaz(string goreli, ushort makine)
        {
            var veri = new byte[512];
            veri[0] = 0x4d;
            veri[1] = 0x5a;
            BitConverter.GetBytes(0x80).CopyTo(veri, 0x3c);
            veri[0x80] = 0x50;
            veri[0x81] = 0x45;
            BitConverter.GetBytes(makine).CopyTo(veri, 0x84);
            DosyaYaz(goreli, veri);
        }

        public void GecerliWavYaz(string goreli)
        {
            var veri = new byte[44];
            Encoding.ASCII.GetBytes("RIFF").CopyTo(veri, 0);
            Encoding.ASCII.GetBytes("WAVE").CopyTo(veri, 8);
            DosyaYaz(goreli, veri);
        }

        public void GecerliOggYaz(string goreli)
        {
            DosyaYaz(goreli, Encoding.ASCII.GetBytes("OggS-test-verisi"));
        }

        private void ManifestYaz()
        {
            var metin = new StringBuilder("index,expected_filename,role,turkish_text,english_reference\n");
            for (var i = 0; i < 214; i++)
            {
                var rol = i % 2 == 0 ? "SON" : "MOM";
                metin.Append(i).Append(',').Append(i.ToString("D3")).Append('_').Append(rol)
                    .Append(".wav,").Append(rol).Append(",Türkçe,English\n");
            }

            MetinYaz("MOD/Data/dub_manifest.csv", metin.ToString());
        }

        public void Dispose()
        {
            try
            {
                if (Directory.Exists(Kok))
                {
                    Directory.Delete(Kok, true);
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
