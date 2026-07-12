# UMMF Katkı Rehberi

UMMF; farklı Unity sürümleri, Mono/IL2CPP arka uçları, BepInEx nesilleri ve oyunlara özgü medya sistemleri nedeniyle topluluk katkısıyla çok daha hızlı gelişebilir.

Yazılımcılar, çevirmenler, testçiler ve belirli bir oyunun dosya yapısını bilen kullanıcılar katkıda bulunabilir.

## Katkı türleri

### 1. Oyun uyumluluk testi

Kod yazmadan katkı sağlamak için:

1. Son UMMF EXE sürümünü indirin.
2. `oyun-tara`, `kur`, `durum` ve `rapor` komutlarını uygun bir çevrimdışı/tek oyunculu oyunda çalıştırın.
3. Oyunu açıp BepInEx logunda UMMF satırlarını kontrol edin.
4. GitHub'daki **Oyun uyumluluğu bildir** formunu doldurun.
5. Doğrulanmış sonuçlar kabul edildiğinde oyun alfabetik uyumluluk listesine eklenir.

### 2. Oyun profili

Belirli bir oyun için `oyun-profilleri/<oyun-kimligi>/` klasörü ekleyebilirsiniz.

Asgari içerik:

```text
oyun-profilleri/ornek-oyun/
├── profil.json
├── README.md
└── testler/
    └── test-edilen-surum.md
```

Şablon: `oyun-profilleri/_sablon/profil.json`

### 3. Çalışma zamanı hostu

Aşağıdaki ortamlardan biri için host veya uyumluluk katmanı geliştirilebilir:

- eski Unity Mono ve eski BepInEx
- BepInEx 5 Mono
- BepInEx 6 Mono
- BepInEx 6 IL2CPP
- gelecekte desteklenecek başka açık mod yükleyicileri

Host katkıları ortak `ICalismaZamaniHostu` sözleşmesini kullanmalı ve desteklemediği özellikleri açıkça bildirmelidir.

### 4. Medya uyarlayıcısı

Uyarlayıcılar şu alanlardan birini veya birkaçını kapsayabilir:

- `Texture2D`, `Sprite`, atlas ve UI doku keşfi
- PNG dışarı aktarma ve doku değiştirme
- `AudioClip` keşfi, dışarı aktarma ve değiştirme
- Unity UI ve TextMeshPro altyazı yakalama
- Unity Localization anahtarları
- altyazı değiştirme
- altyazı kimliğine WAV/OGG ses bağlama
- Addressables
- FMOD
- Wwise

Geliştirmeye başlamadan önce **Medya uyarlayıcısı geliştirme** formunu açın. Böylece aynı işin iki kişi tarafından paralel ve uyumsuz biçimde yapılması önlenir.

## Fork ve pull request akışı

1. Repoyu kendi GitHub hesabınıza fork edin.
2. `main` dalını güncel tutun.
3. Açıklayıcı bir dal oluşturun:

```text
katki/oyun-adi-uyumluluk
katki/bepinex6-il2cpp-hostu
katki/texture2d-disari-aktarma
```

4. Değişiklikleri küçük ve anlaşılır commitlerle yapın.
5. Testleri çalıştırın.
6. Pull request açın ve şablondaki tüm maddeleri doldurun.
7. İnceleme sonrasında kabul edilen katkı ana repodaki `main` dalına birleştirilir.

## Kod ve dil kuralları

- Kullanıcıya görünen metinler Türkçe olmalıdır.
- UMMF'ye ait sınıf, metot ve özellik adları mümkün olduğunda Türkçe proje düzeniyle uyumlu olmalıdır.
- Platform veya API tarafından zorunlu teknik adlar çevrilmez.
- Yeni davranışlar için test eklenmelidir.
- Hata durumları oyunu çökertmek yerine güvenli biçimde raporlanmalıdır.
- Çalışma zamanı bağımlılıkları ortak çekirdeğe taşınmamalı; ilgili host projesinde tutulmalıdır.
- Bir özellik desteklenmiyorsa sessizce başarılı görünmemeli, yetenek sisteminde açıkça belirtilmelidir.

## Uyumluluk verisi

`uyumluluk/oyunlar.json`, alfabetik oyun listesinin tek veri kaynağıdır.

Her kayıt asgari olarak şunları içermelidir:

- kararlı oyun kimliği
- oyun adı
- test edilen oyun sürümü
- Unity sürümü
- Mono veya IL2CPP
- mimari
- BepInEx sürümü/hostu
- doku, ses, altyazı ve seslendirme durumları
- son test tarihi
- doğrulama issue veya PR bağlantısı

Kayıtlar oyun adına göre alfabetik olmalıdır. GitHub Actions sıralamayı, yinelenen kimlikleri ve geçersiz durum değerlerini otomatik denetler.

## Test kanıtı

Bir özelliği `çalışıyor` olarak işaretlemek için aşağıdakilerden en az biri bulunmalıdır:

- ilgili GitHub issue'sunda tekrarlanabilir test adımları
- UMMF raporu
- kişisel bilgiler temizlenmiş BepInEx/UMMF log satırları
- otomatik test
- incelemeye açık uyarlayıcı kodu

Yalnızca tahmin edilen veya başka bir oyundan çıkarılan sonuçlar uyumluluk listesine doğrulanmış olarak eklenmez.

## Gönderilmemesi gereken içerikler

- oyun EXE veya DLL dosyaları
- Unity asset bundle'ları
- telifli dokular, sesler, videolar veya altyazılar
- oyun kayıt dosyaları ve kullanıcı profilleri
- gizli anahtarlar veya kişisel yollar
- kapalı lisanslı middleware SDK dosyaları
- hile korumasını atlatan kod
- çok oyunculu oyunu manipüle eden özellikler

Büyük oyun dosyaları yerine UMMF tarafından üretilen metadata, hash, rapor ve aday listelerini gönderin.

## Lisans

Gönderilen kodun proje lisansı altında dağıtılmasına izin vermiş olursunuz. Başka projelerden alınan kodun lisansı açıkça belirtilmeli ve UMMF lisansıyla uyumlu olmalıdır.
