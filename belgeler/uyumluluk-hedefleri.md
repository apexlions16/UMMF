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

Planlanan host ailesi:

| Host | Amaç | Durum |
|---|---|---|
| Eski Unity Mono uyumluluk hostu | Eski Unity/Mono API'leri | Planlandı |
| Eski BepInEx Mono hostu | Eski BepInEx nesilleri | Planlandı |
| BepInEx 5 Mono hostu | Yaygın Unity Mono oyunları | Planlandı |
| BepInEx 6 Mono hostu | Yeni Mono çalışma zamanı | Planlandı |
| BepInEx 6 IL2CPP hostu | IL2CPP oyunları | Planlandı |

BepInEx bulunmayan veya desteklenmeyen oyunlar için gelecekte farklı bir yükleyici köprüsü değerlendirilebilir; ancak ilk altyapı BepInEx uyarlayıcılarına odaklanacaktır.

## Uyumluluk ilkeleri

1. Ortak mod paketleri host sürümünden bağımsızdır.
2. Ortak çekirdek Unity veya BepInEx assembly'lerine doğrudan bağlanmaz.
3. Hostlar yeteneklerini çalışma zamanında bildirir.
4. Eksik bir yetenek bütün modu çökertmez.
5. Belirsiz veya desteklenmeyen değişiklikler güvenli biçimde atlanır.
6. Her oyun ortamı tarama raporuyla tanımlanır.
7. Doğrulanmış kombinasyonlar bir uyumluluk matrisinde tutulur.

## İlk altyapı teslimatı

Bir sonraki geliştirme aşaması şu parçaları oluşturacaktır:

- `OyunOrtami` veri modeli
- Unity sürümü algılama
- Mono/IL2CPP ayrımı
- x86/x64 mimari algılama
- BepInEx sürümü algılama
- `ICalismaZamaniHostu` sözleşmesi
- host yetenek bayrakları
- host kayıt ve seçim sistemi
- sahte Mono ve IL2CPP hostlarıyla birim testleri
- uyumluluk raporu modeli

Bu altyapı tamamlandıktan sonra gerçek BepInEx 5 Mono ve BepInEx 6 IL2CPP hostları aynı çekirdeğe ayrı ayrı bağlanacaktır.
