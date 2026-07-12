# UMMF

**UMMF — Evrensel Medya Modlama Çerçevesi**, Unity oyunlarındaki dokuları, sesleri ve altyazıları çalışma zamanında keşfetmek, dışarı aktarmak ve değiştirmek için geliştirilen güncellemelere dayanıklı bir modlama altyapısıdır.

Projenin odaklandığı üç medya alanı:

- dokular, sprite'lar ve görsel parçalar
- ses klipleri ve altyazı tetiklemeli seslendirme
- altyazılar ve yerelleştirme metinleri

UMMF tek bir Unity veya BepInEx sürümüne bağlı değildir. Ortak çekirdek; eski ve yeni Unity Mono oyunları, farklı BepInEx nesilleri ve Unity IL2CPP oyunları için ayrı çalışma zamanı hostları üzerinden kullanılacaktır.

## Hızlı bağlantılar

- [Latest Release ve Windows EXE](https://github.com/apexlions16/UMMF/releases/latest)
- [Alfabetik oyun uyumluluk listesi](UYUMLULUK.md)
- [Katkı rehberi](CONTRIBUTING.md)
- [Oyun profili standardı](oyun-profilleri/README.md)
- [GitHub katkı ve uyumluluk formları](https://github.com/apexlions16/UMMF/issues/new/choose)
- [Açık geliştirme görevleri](https://github.com/apexlions16/UMMF/issues)

## Güncel durum

Güncel önizleme sürümü: `0.5.0-onizleme.1`

Bu sürüm ilk gerçek oyun içi hostu içerir:

- BepInEx 5 Unity Mono için gerçek `BaseUnityPlugin` girişi
- eski Unity Mono oyunları için `net35` plugin
- daha yeni Unity Mono oyunları için `netstandard2.0` plugin
- oyun ortamına göre otomatik plugin seçimi
- plugin DLL'lerini içinde taşıyan tek dosyalı Windows kurucu EXE
- `kur`, `durum`, `rapor` ve `kaldir` komutları
- Türkçe BepInEx başlangıç logları
- `BepInEx/UMMF/modlar` altında harici mod klasörü
- kurulum ve oyun içi uyumluluk raporları
- IL2CPP veya yanlış BepInEx nesline kurulumu durduran güvenlik kontrolleri
- plugin dosyası için SHA-256 durum doğrulaması
- eski ve yeni Mono hedef seçimi birim testleri
- Bad Parenting 1: Mr. Red Face için doğrulanmış Unity Mono x86 oyun profili
- `MOD` klasöründen geri alınabilir BepInEx 5 x86 ve ses modu kurulumu
- 214 satırlık ses eşlemesi ile WAV/OGG/MP3 doğrulaması
- Türkçe diyalog satırlarında Unity `AudioSource` ile seslendirme oynatma
- `ses-planla` dry-run komutu ve SHA-256 izlemeli yedek/geri yükleme

Genel doku ve altyazı değiştirme henüz etkin değildir. Ses oynatma ilk olarak Bad Parenting profili için etkindir.

UMMF özgün Unity varlık dosyalarını veya oyun çalıştırılabilir dosyasını yerinde değiştirmez.

## İndirme

Deneme sürümleri GitHub sayfasındaki **Releases** bölümünde normal **Latest Release** olarak yayımlanır.

Doğrudan indirilecek dosya:

`UMMF-v0.5.0-onizleme.1-windows-x64.exe`

EXE kendi .NET çalışma zamanını içerir. ZIP açma veya kaynak kodu derleme gerekmez. Gerekli BepInEx plugin DLL'leri EXE içine gömülüdür.

## Hızlı kullanım

İndirdiğin EXE'nin bulunduğu klasörde PowerShell aç:

```powershell
./UMMF-v0.5.0-onizleme.1-windows-x64.exe bilgi
./UMMF-v0.5.0-onizleme.1-windows-x64.exe oyun-tara "D:\Oyunlar\OrnekOyun"
./UMMF-v0.5.0-onizleme.1-windows-x64.exe kur "D:\Oyunlar\OrnekOyun"
./UMMF-v0.5.0-onizleme.1-windows-x64.exe durum "D:\Oyunlar\OrnekOyun"
./UMMF-v0.5.0-onizleme.1-windows-x64.exe rapor "D:\Oyunlar\OrnekOyun"
./UMMF-v0.5.0-onizleme.1-windows-x64.exe kaldir "D:\Oyunlar\OrnekOyun"
```

### Bad Parenting 1 ses modu

Oyun kökündeki `MOD` klasörüne izinli/kullanıcı üretimi sesleri `000_SON.wav`, `001_MOM.ogg` gibi manifest adlarıyla koy. Önce değişiklik yapmayan planı çalıştır:

```powershell
./UMMF-v0.5.0-onizleme.1-windows-x64.exe ses-planla "D:\SteamLibrary\steamapps\common\Bad Parenting 1 Mr. Red Face"
./UMMF-v0.5.0-onizleme.1-windows-x64.exe kur "D:\SteamLibrary\steamapps\common\Bad Parenting 1 Mr. Red Face"
```

`durum`, `rapor` ve `kaldir` aynı oyun yolu ile çalışır. `MOD/Dub` boşsa altyapı kurulur fakat ses oynatılmaz; sonuç `0/214` olarak raporlanır.

### `oyun-tara <oyun-dizini>`

Bir oyun klasörünü değiştirmeden inceler ve Unity sürümü, Mono/IL2CPP, mimari, BepInEx nesli, TextMeshPro, Unity UI, Addressables, FMOD, Wwise ve seçilen hostu raporlar.

### `kur <oyun-dizini>`

Yalnızca doğrulanmış BepInEx 5 Unity Mono ortamına kurulum yapar. Managed klasöründe `netstandard.dll` varsa `netstandard2.0`, aksi durumda eski Unity uyumluluğu için `net35` plugin seçilir.

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

### `durum <oyun-dizini>`

Kurulu plugin dosyasını, seçilen hedef çerçeveyi ve SHA-256 özetini gösterir.

### `rapor <oyun-dizini>`

`BepInEx/UMMF/raporlar/kurulum-teshis-raporu.json` dosyasını üretir.

### `kaldir <oyun-dizini>`

Yalnızca UMMF plugin DLL'sini ve kurulum bilgisini kaldırır. Kullanıcı modları ile raporlar korunur.

## Oyun içi doğrulama

Kurulumdan sonra oyunu bir kez açıp kapat. `BepInEx/LogOutput.log` içinde şu satırların bulunması beklenir:

```text
UMMF 0.5.0-onizleme.1 başlatılıyor.
Çalışma zamanı hostu: BepInEx 5 Unity Mono
UMMF mod klasörü hazır
UMMF başlangıcı başarıyla tamamlandı.
```

Plugin ayrıca şu dosyayı üretir:

`BepInEx/UMMF/raporlar/uyumluluk-raporu.json`

## Topluluk katkısı

Unity oyunlarının sürüm, arka uç, yükleyici ve medya sistemi çeşitliliği çok yüksektir. Bu nedenle UMMF, topluluk katkısıyla çok daha hızlı gelişebilir.

Katkıda bulunmak için yazılımcı olmanız zorunlu değildir:

### Testçi olarak

- bir oyunu `oyun-tara` ile analiz edebilir,
- UMMF kurulumunu deneyebilir,
- hangi doku, ses ve altyazı özelliklerinin çalıştığını bildirebilir,
- kişisel bilgiler temizlenmiş UMMF/BepInEx loglarını paylaşabilir,
- oyunun alfabetik uyumluluk listesine eklenmesini sağlayabilirsiniz.

GitHub'da **Oyun uyumluluğu bildir** formunu kullanın:

https://github.com/apexlions16/UMMF/issues/new/choose

### Oyun profili hazırlayarak

Her oyun için aşağıdaki gibi küçük bir profil klasörü gönderilebilir:

```text
oyun-profilleri/
└── oyun-kimligi/
    ├── profil.json
    ├── README.md
    ├── testler/
    └── uyarlayicilar/
```

Oyun profili; oyun sürümünü, Unity sürümünü, Mono/IL2CPP bilgisini, mimariyi, BepInEx sürümünü ve doku/ses/altyazı destek durumlarını içerir.

Şablon: [`oyun-profilleri/_sablon/profil.json`](oyun-profilleri/_sablon/profil.json)

### Yazılımcı olarak

Aşağıdaki alanlardan biri geliştirilebilir:

- eski BepInEx, BepInEx 5, BepInEx 6 Mono veya BepInEx 6 IL2CPP hostları
- doku keşfi, dışarı aktarma ve değiştirme
- ses keşfi, dışarı aktarma ve değiştirme
- Unity UI ve TextMeshPro altyazıları
- Unity Localization anahtarları
- altyazı değiştirme
- altyazı kimliğine WAV/OGG ses bağlama
- Addressables, FMOD ve Wwise uyarlayıcıları
- oyunlara özgü, açık kaynaklı uyumluluk profilleri

Geliştirmeye başlamadan önce GitHub'daki **Medya uyarlayıcısı geliştirme** formundan hedef ortamı ve medya alanlarını seçin. Bu formda doku, ses, altyazı, seslendirme, Addressables, FMOD ve Wwise ayrı ayrı işaretlenebilir.

Katkı akışı:

1. Repoyu fork edin.
2. Katkı dalınızı oluşturun.
3. Kod, test, oyun profili ve Türkçe belgeyi ekleyin.
4. Pull request açın.
5. İnceleme ve otomatik testler sonrasında katkı ana repodaki `main` dalına birleştirilir.

Ayrıntılı kurallar: [CONTRIBUTING.md](CONTRIBUTING.md)

## Oyun uyumluluk listesi

[`UYUMLULUK.md`](UYUMLULUK.md), oyunları alfabetik sırayla gösterir ve her oyun için şu alanları ayrı ayrı izler:

- test edilen oyun sürümü
- Unity sürümü
- Mono veya IL2CPP
- mimari
- BepInEx sürümü ve seçilen host
- doku desteği
- ses desteği
- altyazı desteği
- altyazı tetiklemeli seslendirme
- son test tarihi
- doğrulama issue veya PR bağlantısı

Uyumluluk verisinin tek kaynağı `uyumluluk/oyunlar.json` dosyasıdır. GitHub Actions:

- oyunları alfabetik sırada tutar,
- yinelenen oyun kimliklerini engeller,
- geçersiz destek durumlarını reddeder,
- `UYUMLULUK.md` tablosunun veriyle aynı olduğunu doğrular,
- oyun profili JSON dosyalarının zorunlu alanlarını denetler.

## Katkı güvenliği

Projeye şunları göndermeyin:

- oyun EXE veya DLL dosyaları
- Unity asset bundle'ları
- telifli dokular, sesler, videolar veya altyazılar
- oyun kayıtları veya kullanıcı profilleri
- kapalı lisanslı SDK dosyaları
- hile korumasını atlatan kod
- çok oyunculu oyunu manipüle eden özellikler

Bunların yerine UMMF raporları, metadata, hash değerleri, test sonuçları ve kendi yazdığınız açık kaynak kodu gönderin.

## Release doğrulaması

Windows yayın iş akışı şu denetimler tamamlanmadan Release oluşturmaz:

1. `net35` ve `netstandard2.0` plugin DLL'lerini derler.
2. Çözümün bütün birim testlerini çalıştırır.
3. Her iki plugin DLL'sini tek dosyalı Windows x64 EXE içine gömer.
4. EXE'yi Windows runner üzerinde gerçekten açar.
5. `bilgi` komutunun doğru sürümü gösterdiğini doğrular.
6. EXE içindeki gömülü plugin kaynaklarının bulunduğunu denetler.
7. Doğrudan EXE ve SHA-256 özetini normal Latest Release olarak yayımlar.

## Yol haritası

1. BepInEx 5 Mono hostunu gerçek oyunlarda doğrulamak ve uyumluluk listesini büyütmek.
2. İlk `Texture2D` kataloğunu ve PNG dışarı aktarmayı eklemek.
3. PNG ile doku değiştirmeyi eklemek.
4. TextMeshPro ve Unity UI altyazı yakalamayı eklemek.
5. Altyazı kimliklerine WAV/OGG ses bağlamak.
6. BepInEx 6 Mono hostunu bağlamak.
7. BepInEx 6 IL2CPP interop hostunu bağlamak.
8. Eski BepInEx/Unity Mono uyumluluk katmanını genişletmek.
9. Addressables, FMOD ve Wwise uyarlayıcılarını eklemek.

## Kapsam ve güvenlik

UMMF, yasal çevrimdışı ve tek oyunculu modlama için tasarlanmıştır. Hile koruması atlatma, çok oyunculu oyun manipülasyonu ve telif hakkıyla korunan oyun varlıklarını yeniden dağıtma proje kapsamı dışındadır.

## Belgeler

- [Katkı rehberi](CONTRIBUTING.md)
- [Oyun uyumluluk listesi](UYUMLULUK.md)
- [Oyun profili standardı](oyun-profilleri/README.md)
- [BepInEx 5 Mono kurulum ve test](belgeler/bepinex5-mono-kurulum.md)
- [Mimari](belgeler/mimari.md)
- [Uyumluluk hedefleri](belgeler/uyumluluk-hedefleri.md)
- [Güncelleme dayanıklılığı](belgeler/guncelleme-dayanikliligi.md)
- [Değişiklik günlüğü](DEGISIKLIKLER.md)
- [0.5.0 Önizleme 1 sürüm notları](belgeler/surum-notlari/0.5.0-onizleme.1.md)

## Lisans

MIT Lisansı — Türkçe çeviri için `LICENSE` dosyasına bakın.
