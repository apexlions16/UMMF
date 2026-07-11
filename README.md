# UMMF

**UMMF — Evrensel Medya Modlama Çerçevesi**, Unity oyunlarındaki dokuları, sesleri ve altyazıları çalışma zamanında keşfetmek, dışarı aktarmak ve değiştirmek için geliştirilen güncellemelere dayanıklı bir modlama altyapısıdır.

Projenin odaklandığı üç medya alanı:

- dokular ve görsel parçalar
- ses klipleri ve altyazı tetiklemeli seslendirme
- altyazılar ve yerelleştirme metinleri

UMMF tek bir Unity veya BepInEx sürümüne bağlı değildir. Ortak çekirdek; eski ve yeni Unity Mono oyunları, farklı BepInEx nesilleri ve Unity IL2CPP oyunları için ayrı çalışma zamanı hostları üzerinden kullanılacaktır.

## Güncel durum

Güncel önizleme sürümü: `0.3.0-onizleme.2`

Bu sürüm çalışma zamanı host altyapısının ilk işlevsel parçasını içerir:

- Unity oyun klasörü algılama
- Unity sürümünü `globalgamemanagers` ve benzeri dosyalardan okuma
- Mono ve IL2CPP betik arka ucu ayrımı
- Windows PE dosyalarından x86, x64, ARM32 ve ARM64 mimari algılama
- BepInEx kurulumu, sürümü ve nesli algılama
- TextMeshPro, Unity UI, Addressables, FMOD ve Wwise dosya varlığı algılama
- ortak `ICalismaZamaniHostu` sözleşmesi
- host yetenek bayrakları
- eski BepInEx Mono, BepInEx 5 Mono, BepInEx 6 Mono ve BepInEx 6 IL2CPP host seçimi
- yükleyicisiz Mono ve IL2CPP oyunlarında güvenli kurulum adayı sonucu
- Mono ve IL2CPP tarama/host seçim birim testleri
- sürümlenmiş Türkçe mod bildirimi modeli
- güncellemelere dayanıklı varlık kimliği ve eşleştirme sistemi

Bu sürüm henüz gerçek BepInEx plugin DLL'lerini veya medya değiştirme işlemlerini içermez. Host profilleri doğru oyun ortamını seçer ve sonraki gerçek host projelerinin bağlanacağı ortak sınırı sağlar.

UMMF özgün oyun dosyalarını yerinde değiştirmez. Çalışma zamanı uyarlayıcıları oyun çalışırken varlıkları bulup güvenli biçimde değiştirecektir.

## İndirme

Deneme sürümleri GitHub sayfasındaki **Sürümler** bölümünde tam GitHub Release olarak yayımlanır.

Doğrudan indirilecek çalıştırılabilir dosya:

`UMMF-v0.3.0-onizleme.2-windows-x64.exe`

Bu dosya tek başına çalışır ve ayrıca .NET kurulumu gerektirmez. ZIP açma veya kaynak kodu derleme işlemi gerekmez.

## Kullanım

İndirdiğin EXE'nin bulunduğu klasörde PowerShell aç:

```powershell
./UMMF-v0.3.0-onizleme.2-windows-x64.exe bilgi
./UMMF-v0.3.0-onizleme.2-windows-x64.exe oyun-tara "D:\Oyunlar\OrnekOyun"
./UMMF-v0.3.0-onizleme.2-windows-x64.exe host-demo
./UMMF-v0.3.0-onizleme.2-windows-x64.exe kimlik-demo
./UMMF-v0.3.0-onizleme.2-windows-x64.exe eslestirme-demo
./UMMF-v0.3.0-onizleme.2-windows-x64.exe yardim
```

### `oyun-tara <oyun-dizini>`

Bir oyun klasörünü değişiklik yapmadan inceler ve şunları raporlar:

- Unity sürümü
- Mono veya IL2CPP
- işlemci mimarisi
- işletim sistemi düzeni
- BepInEx sürümü/nesli
- TextMeshPro, Unity UI, Addressables, FMOD ve Wwise izleri
- seçilen çalışma zamanı hostu
- hostun başlatılabilmesi için eksik ön koşullar

### `host-demo`

Dört hedef ortam için host seçimini gösterir:

1. Eski Unity Mono ve eski BepInEx
2. Unity Mono ve BepInEx 5
3. Unity Mono ve BepInEx 6
4. Unity IL2CPP ve BepInEx 6

### `bilgi`

Sürümü ve önizlemenin kapsamını gösterir.

### `kimlik-demo`

Bir altyazı için güncellemeler arasında korunabilecek kararlı kimlik üretir.

### `eslestirme-demo`

İçerik özeti değişmiş bir dokunun bağlamsal bilgilerle yeniden eşleştirilmesini gösterir.

## Release doğrulaması

Release iş akışı Windows üzerinde çalışır ve şu denetimleri tamamlamadan sürüm yayımlamaz:

1. Çözümü derler.
2. Bütün birim testlerini çalıştırır.
3. Tek dosyalı ve kendi .NET çalışma zamanını içeren Windows x64 EXE üretir.
4. Üretilen EXE'yi Windows runner üzerinde gerçekten açar.
5. `bilgi` komutunun doğru sürüm çıktısını verdiğini doğrular.
6. EXE'yi doğrudan GitHub Release varlığı olarak yükler.
7. SHA-256 özetini yayımlar.

## Yol haritası

1. Gerçek BepInEx 5 Mono plugin hostunu ortak host sözleşmesine bağlamak.
2. Harici UMMF mod klasörü ve çalışma zamanı log sistemini eklemek.
3. BepInEx 6 Mono hostunu bağlamak.
4. BepInEx 6 IL2CPP interop hostunu bağlamak.
5. Eski BepInEx/Unity Mono uyumluluk sarmalayıcılarını eklemek.
6. `Texture2D`, `Sprite` ve arayüz dokularını keşfedip değiştirmek.
7. TextMeshPro ve Unity UI altyazı kaynaklarını yakalamak.
8. Altyazı kimliklerine ses dosyaları bağlamak.
9. Addressables, FMOD ve Wwise uyarlayıcılarını eklemek.

## Kapsam ve güvenlik

UMMF, yasal çevrimdışı ve tek oyunculu modlama için tasarlanmıştır. Hile koruması atlatma, çok oyunculu oyun manipülasyonu ve telif hakkıyla korunan oyun varlıklarını yeniden dağıtma proje kapsamı dışındadır.

## Belgeler

- [Mimari](belgeler/mimari.md)
- [Uyumluluk hedefleri](belgeler/uyumluluk-hedefleri.md)
- [Güncelleme dayanıklılığı](belgeler/guncelleme-dayanikliligi.md)
- [Değişiklik günlüğü](DEGISIKLIKLER.md)
- [0.3.0 Önizleme 2 sürüm notları](belgeler/surum-notlari/0.3.0-onizleme.2.md)

## Lisans

MIT Lisansı — Türkçe çeviri için `LICENSE` dosyasına bakın.
