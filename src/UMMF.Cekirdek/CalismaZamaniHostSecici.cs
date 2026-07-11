using System;
using System.Collections.Generic;
using System.Linq;
using UMMF.Sozlesmeler;

namespace UMMF.Cekirdek;

public sealed class CalismaZamaniHostSecici
{
    private readonly IReadOnlyList<ICalismaZamaniHostu> hostlar;

    public CalismaZamaniHostSecici(IEnumerable<ICalismaZamaniHostu> hostlar)
    {
        if (hostlar is null)
        {
            throw new ArgumentNullException(nameof(hostlar));
        }

        this.hostlar = hostlar.ToArray();
    }

    public HostSecimSonucu Sec(OyunOrtami ortam)
    {
        if (ortam is null)
        {
            throw new ArgumentNullException(nameof(ortam));
        }

        var aday = hostlar
            .Select(host => new
            {
                Host = host,
                Degerlendirme = host.Degerlendir(ortam)
            })
            .Where(sonuc => sonuc.Degerlendirme.Uyumlu)
            .OrderByDescending(sonuc => sonuc.Degerlendirme.Calistirilabilir)
            .ThenByDescending(sonuc => sonuc.Host.Oncelik)
            .FirstOrDefault();

        if (aday is null)
        {
            return new HostSecimSonucu
            {
                Degerlendirme = HostUyumlulukSonucu.Uyumsuz(
                    "Algılanan oyun ortamı için kayıtlı bir çalışma zamanı hostu bulunamadı.")
            };
        }

        return new HostSecimSonucu
        {
            Host = aday.Host,
            Degerlendirme = aday.Degerlendirme
        };
    }
}

public static class VarsayilanCalismaZamaniHostlari
{
    public static IReadOnlyList<ICalismaZamaniHostu> Olustur()
    {
        var temel = CalismaZamaniYetenegi.OrtamTespiti | CalismaZamaniYetenegi.HostSecimi;

        return new ICalismaZamaniHostu[]
        {
            new KuralliCalismaZamaniHostu(
                "bepinex6-il2cpp",
                "BepInEx 6 IL2CPP hostu",
                500,
                temel | CalismaZamaniYetenegi.Il2CppBaglantisi,
                ortam => DegerlendirBepInEx6(ortam, BetikArkaUcu.IL2CPP)),

            new KuralliCalismaZamaniHostu(
                "bepinex6-mono",
                "BepInEx 6 Mono hostu",
                400,
                temel | CalismaZamaniYetenegi.MonoBaglantisi,
                ortam => DegerlendirBepInEx6(ortam, BetikArkaUcu.Mono)),

            new KuralliCalismaZamaniHostu(
                "bepinex5-mono",
                "BepInEx 5 Mono hostu",
                300,
                temel | CalismaZamaniYetenegi.MonoBaglantisi,
                DegerlendirBepInEx5Mono),

            new KuralliCalismaZamaniHostu(
                "eski-bepinex-mono",
                "Eski BepInEx Mono uyumluluk hostu",
                200,
                temel | CalismaZamaniYetenegi.MonoBaglantisi,
                DegerlendirEskiBepInExMono),

            new KuralliCalismaZamaniHostu(
                "unity-mono-kurulum",
                "Unity Mono uyumluluk kurulum adayı",
                100,
                temel | CalismaZamaniYetenegi.MonoBaglantisi,
                DegerlendirYukleyicisizMono),

            new KuralliCalismaZamaniHostu(
                "unity-il2cpp-kurulum",
                "Unity IL2CPP uyumluluk kurulum adayı",
                90,
                temel | CalismaZamaniYetenegi.Il2CppBaglantisi,
                DegerlendirYukleyicisizIl2Cpp)
        };
    }

    private static HostUyumlulukSonucu DegerlendirBepInEx6(OyunOrtami ortam, BetikArkaUcu beklenenArkaUc)
    {
        if (ortam.BetikArkaUcu != beklenenArkaUc)
        {
            return HostUyumlulukSonucu.Uyumsuz("Betik arka ucu bu hostla eşleşmiyor.");
        }

        if (ortam.Yukleyici.Tur != YukleyiciTuru.BepInEx || ortam.Yukleyici.AnaSurum < 6)
        {
            return HostUyumlulukSonucu.Uyumsuz("BepInEx 6 veya daha yeni bir sürüm algılanmadı.");
        }

        return HostUyumlulukSonucu.UyumluSonuc(
            true,
            beklenenArkaUc == BetikArkaUcu.IL2CPP
                ? "BepInEx 6 IL2CPP dosya düzeni algılandı. Gerçek IL2CPP köprüsü sonraki aşamada bağlanacak."
                : "BepInEx 6 Mono ortamı algılandı. Gerçek plugin hostu sonraki aşamada bağlanacak.");
    }

    private static HostUyumlulukSonucu DegerlendirBepInEx5Mono(OyunOrtami ortam)
    {
        if (ortam.BetikArkaUcu != BetikArkaUcu.Mono ||
            ortam.Yukleyici.Tur != YukleyiciTuru.BepInEx ||
            ortam.Yukleyici.AnaSurum != 5)
        {
            return HostUyumlulukSonucu.Uyumsuz("BepInEx 5 Mono ortamı algılanmadı.");
        }

        return HostUyumlulukSonucu.UyumluSonuc(
            true,
            "BepInEx 5 Mono ortamı algılandı. Gerçek plugin hostu sonraki aşamada bağlanacak.");
    }

    private static HostUyumlulukSonucu DegerlendirEskiBepInExMono(OyunOrtami ortam)
    {
        if (ortam.BetikArkaUcu != BetikArkaUcu.Mono || ortam.Yukleyici.Tur != YukleyiciTuru.BepInEx)
        {
            return HostUyumlulukSonucu.Uyumsuz("Eski BepInEx Mono ortamı algılanmadı.");
        }

        if (ortam.Yukleyici.AnaSurum.HasValue && ortam.Yukleyici.AnaSurum.Value >= 5)
        {
            return HostUyumlulukSonucu.Uyumsuz("Daha yeni BepInEx hostu kullanılmalı.");
        }

        return HostUyumlulukSonucu.UyumluSonuc(
            true,
            ortam.Yukleyici.AnaSurum.HasValue
                ? "Eski BepInEx Mono sürümü için uyumluluk hostu seçildi."
                : "BepInEx nesli belirlenemedi; güvenli eski Mono uyumluluk profili seçildi.");
    }

    private static HostUyumlulukSonucu DegerlendirYukleyicisizMono(OyunOrtami ortam)
    {
        if (ortam.BetikArkaUcu != BetikArkaUcu.Mono || ortam.Yukleyici.Kurulu)
        {
            return HostUyumlulukSonucu.Uyumsuz("Yükleyicisiz Unity Mono ortamı algılanmadı.");
        }

        return HostUyumlulukSonucu.UyumluSonuc(
            false,
            "Unity Mono oyunu algılandı; uygun BepInEx/uyumluluk yükleyicisi kurulmadan host başlatılamaz.");
    }

    private static HostUyumlulukSonucu DegerlendirYukleyicisizIl2Cpp(OyunOrtami ortam)
    {
        if (ortam.BetikArkaUcu != BetikArkaUcu.IL2CPP || ortam.Yukleyici.Kurulu)
        {
            return HostUyumlulukSonucu.Uyumsuz("Yükleyicisiz Unity IL2CPP ortamı algılanmadı.");
        }

        return HostUyumlulukSonucu.UyumluSonuc(
            false,
            "Unity IL2CPP oyunu algılandı; uygun BepInEx IL2CPP paketi kurulmadan host başlatılamaz.");
    }

    private sealed class KuralliCalismaZamaniHostu : ICalismaZamaniHostu
    {
        private readonly Func<OyunOrtami, HostUyumlulukSonucu> degerlendirici;

        public KuralliCalismaZamaniHostu(
            string kimlik,
            string ad,
            int oncelik,
            CalismaZamaniYetenegi yetenekler,
            Func<OyunOrtami, HostUyumlulukSonucu> degerlendirici)
        {
            Kimlik = kimlik;
            Ad = ad;
            Oncelik = oncelik;
            Yetenekler = yetenekler;
            this.degerlendirici = degerlendirici ?? throw new ArgumentNullException(nameof(degerlendirici));
        }

        public string Kimlik { get; }

        public string Ad { get; }

        public int Oncelik { get; }

        public CalismaZamaniYetenegi Yetenekler { get; }

        public HostUyumlulukSonucu Degerlendir(OyunOrtami ortam)
        {
            return degerlendirici(ortam);
        }
    }
}
