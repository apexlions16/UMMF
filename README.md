# UMMF

**UMMF — Evrensel Medya Modlama Çerçevesi**, Unity oyunlarındaki doku, ses ve altyazı sistemlerini keşfetmek, modlamak ve oyun güncellemelerine karşı daha dayanıklı hâle getirmek için geliştirilen açık kaynaklı bir altyapıdır.

UMMF tek bir Unity veya BepInEx sürümüne bağlı değildir. Ortak çekirdek; eski ve yeni Unity Mono oyunları, farklı BepInEx nesilleri ve Unity IL2CPP oyunları için ayrı çalışma zamanı hostları üzerinden genişletilir.

## Hızlı bağlantılar

- [Latest Release ve doğrudan Windows EXE](https://github.com/apexlions16/UMMF/releases/latest)
- [Alfabetik oyun uyumluluk listesi](UYUMLULUK.md)
- [Katkı rehberi](CONTRIBUTING.md)
- [Oyun profili standardı](oyun-profilleri/README.md)
- [GitHub katkı ve uyumluluk formları](https://github.com/apexlions16/UMMF/issues/new/choose)
- [Açık geliştirme görevleri](https://github.com/apexlions16/UMMF/issues)

## Güncel sürüm

Güncel arayüz önizlemesi:

```text
0.6.0-onizleme.1
```

Bu sürümün ana teslimatı tamamen Türkçe Windows masaüstü arayüzüdür. Kullanıcıların temel işlemler için PowerShell komutu yazması gerekmez.

Release sayfasından indirilecek tek uygulama:

```text
UMMF-v0.6.0-onizleme.1-windows-x64.exe
```

EXE kendi .NET çalışma zamanını içerir. Kurulum sihirbazı veya ZIP açma işlemi gerekmez; dosyaya çift tıklamak yeterlidir.

## Masaüstü arayüzü

Arayüz koyu temalı, sol menülü ve kart tabanlı bir Windows uygulamasıdır.

### Ana Sayfa

- UMMF kapsam ve sürüm özeti
- oyun seçmeye hızlı geçiş
- güvenli kurulum açıklaması
- GitHub ve Latest Release bağlantıları

### Oyun ve Kurulum

Bir Unity oyun klasörü seçildiğinde UMMF şunları gösterir:

- Unity sürümü
- Mono veya IL2CPP betik arka ucu
- x86, x64 veya ARM mimarisi
- BepInEx kurulumu ve sürümü
- seçilen çalışma zamanı hostu
- TextMeshPro ve Unity UI izleri
- FMOD ve Wwise izleri
- hostun başlatılabilirlik durumu

Aynı ekrandan şu işlemler yapılabilir:

- **Ses planı:** desteklenen oyuna ait ses modunu değişiklik yapmadan doğrular.
- **UMMF'yi kur:** uygun BepInEx hostunu ve oyun profilini güvenli biçimde kurar.
- **Durumu denetle:** kurulu plugin ve SHA-256 bilgisini kontrol eder.
- **Rapor oluştur:** Türkçe JSON teşhis raporu üretir.
- **Kaldır:** UMMF dosyalarını geri alır; kullanıcı modlarını ve raporları korur.

### Uyumluluk

Uygulama, `uyumluluk/oyunlar.json` verisinden üretilen alfabetik listeyi gömülü olarak gösterir. Her oyun için şu alanlar ayrı ayrı izlenir:

- oyun ve Unity sürümü
- Mono veya IL2CPP
- mimari ve BepInEx sürümü
- doku desteği
- ses desteği
- altyazı desteği
- altyazı tetiklemeli seslendirme desteği

### Topluluk Katkısı

Testçiler ve yazılımcılar uygulama içinden katkı paketi hazırlayabilir.

Arayüzde:

- oyun ve sürüm bilgisi girilir,
- doku, ses, altyazı ve seslendirme durumu seçilir,
- test notları yazılır,
- **Katkı ZIP'i oluştur** düğmesine basılır.

Üretilen ZIP yalnızca ortam metadatası, profil JSON'u ve test notlarını içerir. Oyun EXE/DLL dosyaları, Unity varlıkları, dokular, sesler veya altyazılar pakete eklenmez.

### Günlük

Tarama, kurulum, durum, rapor ve kaldırma işlemleri uygulama içinde Türkçe olarak kaydedilir. Günlük metin dosyasına dışarı aktarılabilir.

## Desteklenen ilk çalışma zamanı

İlk gerçek oyun içi host:

- BepInEx 5 Unity Mono
- eski Unity Mono oyunları için `net35`
- daha yeni Unity Mono oyunları için `netstandard2.0`
- oyun ortamına göre otomatik hedef seçimi
- yanlışlıkla IL2CPP veya BepInEx 6 ortamına Mono plugin kurmayı engelleyen kontroller

Kurulum yapısı:

```text
BepInEx/
├── plugins/
│   └── UMMF/
│       └── UMMF.BepInEx5.Mono.dll
└── UMMF/
    ├── modlar/
    ├── raporlar/
    └── kurulum-bilgisi.json
```

Oyun açıldığında plugin `BepInEx/LogOutput.log` içine Türkçe başlangıç bilgileri yazar ve şu raporu üretir:

```text
BepInEx/UMMF/raporlar/uyumluluk-raporu.json
```

## Bad Parenting 1 ses profili

`Bad Parenting 1: Mr. Red Face` için ilk oyuna özgü ses profili bulunur:

- Unity Mono x86 algılama
- BepInEx 5 x86 kurulumu
- 214 satırlık ses eşlemesi
- WAV, OGG ve MP3 kaynak doğrulaması
- Türkçe diyalog sırasında Unity `AudioSource` ile ses oynatma
- yedekleme, durum raporu ve güvenli kaldırma

Kullanıcı tarafından üretilmiş veya kullanma izni bulunan sesler oyun kökündeki `MOD` klasörüne manifestte belirtilen adlarla yerleştirilir. Arayüzde önce **Ses planı**, ardından **UMMF'yi kur** kullanılır.

## Toplulukla geliştirme

Unity oyunlarının sürüm, arka uç, middleware ve arayüz yapıları çok çeşitlidir. Proje topluluk katkısıyla daha hızlı gelişebilir.

Yazılımcı olmayan kullanıcılar:

- oyunlarını arayüzde tarayabilir,
- kurulum ve medya özelliklerini test edebilir,
- katkı ZIP'i oluşturabilir,
- GitHub'daki **Oyun uyumluluğu bildir** formunu doldurabilir.

Yazılımcılar şu alanlardan birine katkı yapabilir:

- eski BepInEx hostları
- BepInEx 5 ve BepInEx 6 Mono
- BepInEx 6 IL2CPP
- `Texture2D`, Sprite ve atlas desteği
- ses keşfi ve değiştirme
- Unity UI, TextMeshPro ve Unity Localization altyazıları
- altyazı kimliğine ses bağlama
- Addressables, FMOD ve Wwise uyarlayıcıları
- oyuna özgü açık kaynak uyumluluk profilleri

Katkı akışı:

1. Repoyu fork edin.
2. Katkı dalı oluşturun.
3. Kod, test, oyun profili ve Türkçe belge ekleyin.
4. Pull request açın.
5. Otomatik test ve inceleme sonrasında uygun katkı ana repoya birleştirilir.

Ayrıntılar: [CONTRIBUTING.md](CONTRIBUTING.md)

## Release doğrulaması

Bir sürüm yayımlanmadan önce GitHub Actions:

1. bütün çözümü sıfır hata ile derler,
2. birim testlerini çalıştırır,
3. `net35` ve `netstandard2.0` BepInEx pluginlerini üretir,
4. self-contained ve tek dosyalı Windows x64 GUI EXE oluşturur,
5. GUI EXE'yi `--arayuz-testi` modunda çalıştırır,
6. Ana Sayfa, Oyun ve Kurulum, Uyumluluk, Topluluk Katkısı ve Günlük ekranlarını doğrular,
7. BepInEx 5 kurulum, durum, rapor ve kaldırma testlerini tekrar çalıştırır,
8. doğrudan EXE ve SHA-256 özetini normal **Latest Release** olarak yayımlar.

## Yol haritası

1. Masaüstü arayüzünü gerçek oyunlarda doğrulamak.
2. Genel `Texture2D` kataloğu ve PNG dışarı aktarma sistemi.
3. PNG ile doku değiştirme.
4. TextMeshPro, Unity UI ve Unity Localization altyazı yakalama.
5. Altyazı kimliklerine WAV/OGG ses bağlama.
6. BepInEx 6 Mono hostu.
7. BepInEx 6 IL2CPP hostu.
8. Eski Unity ve BepInEx uyumluluk katmanları.
9. Addressables, FMOD ve Wwise uyarlayıcıları.

## Kapsam ve güvenlik

UMMF yasal çevrimdışı ve tek oyunculu modlama için tasarlanmıştır. Proje şunları kabul etmez:

- oyun EXE veya DLL dosyalarının yeniden dağıtılması
- telifli asset bundle, doku, ses, video veya altyazılar
- kapalı SDK dosyaları
- hile koruması atlatma kodları
- çok oyunculu oyunu manipüle eden özellikler

Bunların yerine UMMF raporları, metadata, hash değerleri, test sonuçları ve katkıcının kendi açık kaynak kodu paylaşılır.

## Belgeler

- [Masaüstü arayüz tasarımı](belgeler/arayuz-tasarimi.md)
- [BepInEx 5 Mono kurulum ve test](belgeler/bepinex5-mono-kurulum.md)
- [Topluluk katkı sistemi](belgeler/topluluk-katki-sistemi.md)
- [Uyumluluk hedefleri](belgeler/uyumluluk-hedefleri.md)
- [Mimari](belgeler/mimari.md)
- [Güncelleme dayanıklılığı](belgeler/guncelleme-dayanikliligi.md)
- [Değişiklik günlüğü](DEGISIKLIKLER.md)

## Lisans

MIT Lisansı — ayrıntılar için `LICENSE` dosyasına bakın.
