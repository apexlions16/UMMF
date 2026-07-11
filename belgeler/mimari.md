# Mimari

UMMF, oyuna özgü çalışma zamanı erişimini kararlı medya modu modelinden ayırır. Çekirdek; Unity sürümü, BepInEx sürümü, Mono veya IL2CPP seçimi ve işlemci mimarisinden bağımsız kalır.

## Katmanlar

### UMMF.Sozlesmeler

Tarayıcılar, mod araçları ve çalışma zamanı uyarlayıcıları tarafından paylaşılacak bağımlılıksız veri sözleşmelerini içerir:

- medya varlığı türleri
- güncellemelere dayanıklı parmak izleri
- mod bildirimleri
- değiştirme yönergeleri
- `OyunOrtami` ve `YukleyiciBilgisi`
- Mono/IL2CPP, mimari ve işletim sistemi türleri
- `CalismaZamaniYetenegi` bayrakları
- `ICalismaZamaniHostu` sözleşmesi
- host uyumluluk ve seçim sonuçları

Kitaplık, eski Unity Mono çalışma zamanları tarafından kullanılabilmesi için `netstandard2.0` hedefler.

### UMMF.Cekirdek

Çalışma zamanından bağımsız davranışları içerir:

- belirlenimci varlık kimliği üretimi
- oyun güncellemelerinden sonra aday puanlama ve yeniden eşleştirme
- mod bildirimi doğrulama
- oyun klasörü ve çalışma ortamı algılama
- host kaydı, önceliklendirme ve seçim
- eksik yükleyici için güvenli kurulum adayı sonucu

Bu katman Unity, BepInEx, Harmony, IL2CPP birlikte çalışma kitaplıkları, FMOD veya Wwise'a başvuru eklemez.

### Oyun ortamı algılama

`OyunOrtamiAlgilayici` oyun klasöründe değişiklik yapmadan şu izleri arar:

- `*_Data` Unity veri dizini
- `Managed` ve `Assembly-CSharp.dll` Mono düzeni
- `GameAssembly.dll` ve `global-metadata.dat` IL2CPP düzeni
- `globalgamemanagers` içindeki Unity sürüm metni
- PE başlığından x86, x64, ARM32 veya ARM64 mimarisi
- BepInEx çekirdek dosyaları ve sürüm bilgisi
- TextMeshPro, Unity UI, Addressables, FMOD ve Wwise assembly/yerel kitaplıkları

Algılanamayan bir değer tahmin edilmez; `Bilinmiyor` olarak raporlanır.

### Çalışma zamanı hostları

Her gerçek host yalnızca kendi hedef çalışma ortamına bağımlı olacak ve ortak `ICalismaZamaniHostu` sözleşmesini uygulayacaktır.

`0.3.0-onizleme.1` sürümünde gerçek plugin hostları yerine seçim altyapısını doğrulayan tanımsal host profilleri bulunur:

- eski BepInEx Mono uyumluluk profili
- BepInEx 5 Unity Mono profili
- BepInEx 6 Unity Mono profili
- BepInEx 6 Unity IL2CPP profili
- yükleyicisiz Unity Mono kurulum adayı
- yükleyicisiz Unity IL2CPP kurulum adayı

Bu profiller gerçek medya işlemi uygulamaz. Ortamın hangi gerçek host projesine yönlendirileceğini, ön koşulların hazır olup olmadığını ve Mono/IL2CPP bağlantı yeteneğini bildirir.

### Medya uyarlayıcıları

Gerçek host tarafından sunulan çalışma zamanı erişimini medya türüne özgü davranışlara dönüştürecektir:

- Unity `Texture2D` ve `Sprite`
- Unity `AudioClip` ve `AudioSource`
- TextMeshPro altyazıları
- Unity UI altyazıları
- Unity Localization
- Addressables
- FMOD
- Wwise

## Host seçim kuralları

Host seçici bütün kayıtlı hostları değerlendirir:

1. Betik arka ucu ve yükleyici nesliyle uyumsuz hostlar elenir.
2. Başlatılabilir hostlar, yalnızca kurulum önerisi olan profillerin önüne geçer.
3. Aynı düzeydeki adaylarda en yüksek öncelikli host seçilir.
4. Uygun host yoksa açıklamalı güvenli başarısızlık sonucu döndürülür.
5. Yükleyici bulunmayan Mono ve IL2CPP oyunlarında oyun reddedilmez; uygun kurulum adayı seçilir fakat `Başlatılabilir: Hayır` raporlanır.

## Yetenek sistemi

Hostlar desteklerini bayraklarla bildirir. İlk altyapıda etkin bayraklar:

- ortam tespiti
- host seçimi
- Mono bağlantısı
- IL2CPP bağlantısı

Doku, ses, altyazı, Addressables, FMOD ve Wwise bayrakları gerçek uyarlayıcılar eklendikçe etkinleştirilecektir. Bir özellik yoksa UMMF işlemi zorlamaz; özelliği kapatır ve uyumluluk raporuna neden yazar.

## Çalışma akışı

1. Oyun klasörü taranır.
2. Unity sürümü, betik arka ucu, mimari, işletim sistemi ve yükleyici belirlenir.
3. Ortama uygun çalışma zamanı hostu veya kurulum adayı seçilir.
4. Host yetenekleri raporlanır.
5. Gerçek host hazır olduğunda kurulu mod bildirimleri yüklenir.
6. Dokular, ses klipleri ve altyazı kaynakları keşfedilir.
7. Keşifler `VarlikParmakIzi` değerlerine dönüştürülür.
8. Değişiklikler güven puanlarıyla eşleştirilir.
9. Yalnızca güven eşiğini geçen ve host tarafından desteklenen eşleşmeler uygulanır.
10. Çözülemeyen veya desteklenmeyen işlemler inceleme için kaydedilir.

## Paketleme yaklaşımı

Tek bir evrensel DLL yerine ortak çekirdek ve hedefe özgü dağıtımlar üretilecektir:

```text
UMMF.Ortak/
UMMF.UnityMono.Eski/
UMMF.BepInEx5.Mono/
UMMF.BepInEx6.Mono/
UMMF.BepInEx6.IL2CPP/
```

Tarayıcı doğru paketi önerecek; ileride kurucu bu seçimi otomatik uygulayacaktır.

## Güvenlik sınırları

UMMF; hile koruması atlatma, çok oyunculu oyun manipülasyonu veya çalıştırılabilir oyun yaması dağıtımı sağlamaz. Hedef, desteklenebilir masaüstü Unity oyunlarında çevrimdışı ve tek oyunculu medya modlamasıdır. Her Unity/BepInEx kombinasyonu test edilmeden desteklenmiş sayılmaz.
