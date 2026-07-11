# Mimari

UMMF, oyuna özgü çalışma zamanı erişimini kararlı medya modu modelinden ayırır. Çekirdek; Unity sürümü, BepInEx sürümü, Mono veya IL2CPP seçimi ve işlemci mimarisinden bağımsız kalır.

## Katmanlar

### UMMF.Sozlesmeler

Tarayıcılar, mod araçları ve çalışma zamanı uyarlayıcıları tarafından paylaşılacak bağımlılıksız veri sözleşmelerini içerir:

- medya varlığı türleri
- güncellemelere dayanıklı parmak izleri
- mod bildirimleri
- değiştirme yönergeleri
- çalışma zamanı yetenekleri
- oyun ortamı tanımları

Kitaplık, eski Unity Mono çalışma zamanları tarafından kullanılabilmesi için `netstandard2.0` hedefler.

### UMMF.Cekirdek

Çalışma zamanından bağımsız davranışları içerir:

- belirlenimci varlık kimliği üretimi
- oyun güncellemelerinden sonra aday puanlama ve yeniden eşleştirme
- mod bildirimi doğrulama
- uyarlayıcı seçimi
- yetenek denetimi ve güvenli geri dönüşler

Bu katman Unity, BepInEx, Harmony, IL2CPP birlikte çalışma kitaplıkları, FMOD veya Wwise'a başvuru eklememelidir.

### UMMF.OrtamAlgilama

Oyun başlatılmadan veya host yüklenirken aşağıdaki bilgileri belirler:

- Unity sürümü
- Mono veya IL2CPP betik arka ucu
- x86 veya x64 işlemci mimarisi
- işletim sistemi
- kurulu BepInEx sürümü veya kullanılabilecek yükleyici
- TextMeshPro, Unity UI, Addressables, FMOD ve Wwise varlığı

Algılama sonucu ortak bir `OyunOrtami` modeline dönüştürülür.

### Çalışma zamanı hostları

Her host yalnızca kendi hedef çalışma ortamına bağımlıdır ve ortak `ICalismaZamaniHostu` sözleşmesini uygular.

Planlanan hostlar:

- eski Unity Mono uyumluluk hostu
- eski BepInEx nesilleri için Mono hostu
- BepInEx 5 Unity Mono hostu
- BepInEx 6 Unity Mono hostu
- BepInEx 6 Unity IL2CPP hostu

Aynı mod paketi bütün hostlarda ortak UMMF bildirimini kullanır. Host yalnızca oyun nesnelerine nasıl erişileceğini ve değişikliklerin nasıl uygulanacağını belirler.

### Medya uyarlayıcıları

Host tarafından sunulan çalışma zamanı erişimini medya türüne özgü davranışlara dönüştürür:

- Unity `Texture2D` ve `Sprite`
- Unity `AudioClip` ve `AudioSource`
- TextMeshPro altyazıları
- Unity UI altyazıları
- Unity Localization
- Addressables
- FMOD
- Wwise

## Yetenek sistemi

Eski Unity ve BepInEx sürümlerinin bütün API'leri aynı değildir. Bu nedenle hostlar özellik desteklerini çalışma zamanında bildirir:

- doku tarama
- GPU üzerinden okunamayan doku çıkarma
- ses klibi değiştirme
- ses klibi örnek verisi çıkarma
- TextMeshPro yakalama
- Unity UI Text yakalama
- IL2CPP tür kaydı
- Addressables yükleme yakalama

Bir özellik yoksa UMMF işlemi zorlamaz. Modu kısmen çalıştırır, özelliği kapatır ve uyumluluk raporuna açık bir neden yazar.

## Çalışma akışı

1. Oyun, Unity sürümü, betik arka ucu, mimari ve yükleyici belirlenir.
2. Ortama uygun çalışma zamanı hostu seçilir.
3. Host kendi yeteneklerini çekirdeğe bildirir.
4. Kurulu mod bildirimleri yüklenir.
5. Etkin dokular, ses klipleri ve altyazı kaynakları keşfedilir.
6. Keşifler `VarlikParmakIzi` değerlerine dönüştürülür.
7. Değişiklikler güven puanlarıyla eşleştirilir.
8. Yalnızca güven eşiğini geçen ve host tarafından desteklenen eşleşmeler uygulanır.
9. Çözülemeyen, belirsiz veya desteklenmeyen işlemler inceleme için kaydedilir.

## Paketleme yaklaşımı

Tek bir evrensel DLL yerine ortak çekirdek ve hedefe özgü dağıtımlar üretilecektir:

```text
UMMF.Ortak/
UMMF.UnityMono.Eski/
UMMF.BepInEx5.Mono/
UMMF.BepInEx6.Mono/
UMMF.BepInEx6.IL2CPP/
```

Kurucu veya tarayıcı doğru paketi otomatik seçebilecek, ileri düzey kullanıcılar paketi elle de kurabilecektir.

## Güvenlik sınırları

UMMF; hile koruması atlatma, çok oyunculu oyun manipülasyonu veya çalıştırılabilir oyun yaması dağıtımı sağlamaz. Hedef, desteklenebilir masaüstü Unity oyunlarında çevrimdışı ve tek oyunculu medya modlamasıdır. Uyumluluk kapsamı geniş olsa da her Unity/BepInEx kombinasyonu otomatik olarak desteklenmiş sayılmaz; her ortam test edilip uyumluluk matrisine eklenir.
