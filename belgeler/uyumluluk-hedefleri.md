# Uyumluluk hedefleri

UMMF'nin amacı tek bir Unity veya BepInEx sürümüne bağlı bir eklenti üretmek değildir. Ortak mod biçimi ve medya çekirdeği, farklı çalışma zamanı hostları üzerinden mümkün olan en geniş Unity masaüstü oyun grubunda kullanılacaktır.

## Betik arka uçları

### Unity Mono

Hedeflenen kapsam:

- eski Unity Mono oyunları
- eski .NET/Mono profilini kullanan Unity sürümleri
- farklı BepInEx nesilleri
- BepInEx 5 Unity Mono
- BepInEx 6 Unity Mono

Mono tarafında reflection, Harmony tabanlı yakalama ve yönetilen Unity assembly erişimi kullanılabilir. Eski Unity sürümlerinde eksik API'ler için özellik algılama ve uyumluluk sarmalayıcıları gerekecektir.

### Unity IL2CPP

IL2CPP, Unity'nin Mono'ya alternatif AOT betik arka ucudur. UMMF IL2CPP oyunları için ayrı host kullanacaktır.

Hedeflenen kapsam:

- Windows x64 IL2CPP oyunları
- uygun olduğunda Windows x86 IL2CPP oyunları
- BepInEx IL2CPP birlikte çalışma katmanı
- üretilmiş interop assembly'leri üzerinden Unity nesnelerine erişim
- IL2CPP tür kaydı ve wrapper yönetimi

IL2CPP desteği Mono desteğinin bir uzantısı değil, ayrı bir çalışma zamanı uyarlayıcısıdır. Kod stripping, AOT sınırlamaları ve oyun güncellemelerinde değişen metadata nedeniyle bazı işlemler yalnızca kısmi olarak kullanılabilir.

## Yükleyici nesilleri

| Host ailesi | Ortam algılama/seçim | Gerçek plugin hostu | Medya işlemleri |
|---|---:|---:|---:|
| Eski Unity Mono kurulum adayı | Hazır | Planlandı | Planlandı |
| Eski BepInEx Mono hostu | Hazır | Planlandı | Planlandı |
| BepInEx 5 Mono hostu | Hazır | Sıradaki aşama | Planlandı |
| BepInEx 6 Mono hostu | Hazır | Planlandı | Planlandı |
| BepInEx 6 IL2CPP hostu | Hazır | Planlandı | Planlandı |

“Hazır” ifadesi gerçek oyuna enjekte edilen pluginin tamamlandığı anlamına gelmez. `0.3.0-onizleme.1`, oyun ortamını tanır ve hangi host ailesinin kullanılması gerektiğini seçer.

## Algılanan bilgiler

Tarayıcı şu bilgileri üretir:

- Unity veri dizini ve çalıştırılabilir dosya
- Unity sürümü
- Mono veya IL2CPP arka ucu
- x86, x64, ARM32 veya ARM64 mimarisi
- Windows, Linux veya macOS dosya düzeni
- BepInEx kurulumu ve mümkünse ana sürümü
- TextMeshPro
- Unity UI
- Addressables
- FMOD
- Wwise

Bilinmeyen bilgiler varsayımla doldurulmaz. Uyumluluk raporunda açıkça `Bilinmiyor` olarak kalır.

## Uyumluluk ilkeleri

1. Ortak mod paketleri host sürümünden bağımsızdır.
2. Ortak çekirdek Unity veya BepInEx assembly'lerine doğrudan bağlanmaz.
3. Hostlar yeteneklerini çalışma zamanında bildirir.
4. Eksik bir yetenek bütün modu çökertmez.
5. Belirsiz veya desteklenmeyen değişiklikler güvenli biçimde atlanır.
6. Her oyun ortamı tarama raporuyla tanımlanır.
7. Doğrulanmış kombinasyonlar bir uyumluluk matrisinde tutulur.
8. Yükleyici bulunmayan oyun için yanlış host başlatılmaz; kurulum adayı raporlanır.
9. IL2CPP ve Mono aynı mod bildirimini kullanır fakat farklı gerçek hostlarda çalışır.

## 0.3.0-onizleme.1 teslimatı

Tamamlanan parçalar:

- `OyunOrtami` veri modeli
- Unity sürümü algılama
- Mono/IL2CPP ayrımı
- x86/x64/ARM mimari algılama
- BepInEx kurulumu ve sürüm nesli algılama
- `ICalismaZamaniHostu` sözleşmesi
- host yetenek bayrakları
- host kayıt ve öncelikli seçim sistemi
- yükleyicisiz Mono ve IL2CPP için güvenli kurulum adayı
- sahte Mono ve IL2CPP ortamlarıyla birim testleri
- gerçekçi klasör düzeni kullanan tarama testleri
- `oyun-tara` ve `host-demo` komutları

## Sonraki teslimat

Bir sonraki altyapı aşaması:

- gerçek BepInEx 5 Mono plugin projesi
- plugin yaşam döngüsü
- ortak host sözleşmesine bağlanan log ve başlangıç servisi
- harici UMMF mod klasörü keşfi
- oyun yapısı parmak izi ve uyumluluk raporunun JSON çıktısı
- örnek oyunda plugin yüklenme doğrulaması

Bunun ardından BepInEx 6 Mono ve BepInEx 6 IL2CPP gerçek hostları aynı ortak sözleşmeye bağlanacaktır.
