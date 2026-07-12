# Topluluk Katkı ve Oyun Uyumluluk Sistemi

UMMF'nin hedefi tek bir oyun için çalışan sabit bir eklenti değil; farklı Unity sürümleri, Mono/IL2CPP arka uçları ve BepInEx nesilleri arasında ortak mod paketleri kullanabilen genişletilebilir bir medya modlama sistemidir.

Bu hedef, belirli oyunları ve middleware yapılarını bilen topluluk geliştiricilerinin katkısıyla daha hızlı ilerleyebilir.

## Katkı akışı

```text
Oyun veya ortam seçilir
→ GitHub seçim formu açılır
→ mevcut çalışma kontrol edilir
→ fork ve katkı dalı oluşturulur
→ oyun profili / host / medya uyarlayıcısı geliştirilir
→ test ve uyumluluk verisi eklenir
→ pull request açılır
→ otomatik doğrulamalar çalışır
→ inceleme sonrasında main dalına birleştirilir
→ uygun sonraki Release'e dahil edilir
```

## GitHub seçim formları

### Oyun uyumluluğu bildir

Testçiler aşağıdaki bilgileri seçebilir:

- oyun ve oyun sürümü
- Unity sürümü
- Mono veya IL2CPP
- x86, x64, ARM32 veya ARM64
- BepInEx nesli ve tam sürümü
- doku, ses ve altyazı özellikleri
- Addressables, FMOD ve Wwise
- genel çalışma sonucu
- test adımları ve temizlenmiş log bölümleri

### Medya uyarlayıcısı geliştirme

Yazılımcılar geliştirecekleri alanları önceden seçebilir:

- çalışma zamanı hostu
- doku keşfi, dışarı aktarma veya değiştirme
- ses keşfi, dışarı aktarma veya değiştirme
- Unity UI, TextMeshPro veya Unity Localization altyazıları
- altyazı tetiklemeli seslendirme
- Addressables, FMOD veya Wwise
- kurulum ve oyun profili araçları

Bu kayıtlar aynı işin birden fazla kişi tarafından birbirinden habersiz yapılmasını azaltır.

## Uyumluluk listesi

Oyun uyumluluğu iki katmanda tutulur:

1. `uyumluluk/oyunlar.json`: makine tarafından doğrulanan tek veri kaynağı.
2. `UYUMLULUK.md`: kullanıcıların okuyacağı alfabetik tablo.

Her oyun için şu durumlar ayrı ayrı izlenir:

- doku
- ses
- altyazı
- altyazı seslendirme

Durum değerleri:

- `calisiyor`
- `kismi`
- `test-bekliyor`
- `calismiyor`
- `destek-yok`
- `bilinmiyor`

## Oyun profilleri

Bir oyun profili, oyunun kendisini içermez. Yalnızca uyumluluk metadata'sı, test notları ve katkıcının yazdığı kodu içerir.

```text
oyun-profilleri/<oyun-kimligi>/
├── profil.json
├── README.md
├── testler/
└── uyarlayicilar/
```

Genel çözüm üretilebiliyorsa oyuna özel kod yerine ortak çekirdek veya host geliştirilmelidir. Oyuna özgü kod yalnızca genel yöntem yeterli olmadığında kullanılmalıdır.

## Ana repoya kabul

Katkılar doğrudan ana repoya yazılmaz. Katkıcı kendi forkunda çalışır ve pull request açar. Otomatik testler ve proje incelemesi sonrasında uygun değişiklikler `main` dalına birleştirilir.

Kabul edilen oyun profilleri alfabetik uyumluluk listesine eklenir. Kabul edilen host ve medya uyarlayıcıları test edilebilir duruma geldiğinde bir sonraki normal GitHub Release içindeki Windows EXE'ye dahil edilir.

## Güvenlik ve lisans

Topluluk katkıları yalnızca yasal çevrimdışı ve tek oyunculu modlama kapsamındadır. Oyun varlıkları, çalıştırılabilir dosyalar, kapalı SDK dosyaları ve hile koruması atlatma kodları kabul edilmez.
