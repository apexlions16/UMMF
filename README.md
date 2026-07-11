# UMMF

**UMMF — Evrensel Medya Modlama Çerçevesi**, Unity oyunlarındaki dokuları, sesleri ve altyazıları çalışma zamanında keşfetmek, dışarı aktarmak ve değiştirmek için geliştirilen güncellemelere dayanıklı bir modlama altyapısıdır.

Projenin odaklandığı üç medya alanı:

- dokular ve görsel parçalar
- ses klipleri ve altyazı tetiklemeli seslendirme
- altyazılar ve yerelleştirme metinleri

UMMF tek bir Unity, BepInEx veya betik arka ucu sürümüne bağlı olmayacaktır. Hedef kapsam; eski Unity Mono oyunlarını, farklı BepInEx nesillerini, güncel Unity Mono oyunlarını ve IL2CPP oyunlarını ortak bir çekirdek üzerinde ayrı çalışma zamanı uyarlayıcılarıyla desteklemektir.

## Güncel durum

Güncel önizleme sürümü: `0.2.0-onizleme.2`

Bu sürüm çalışma zamanından bağımsız temeli ve Türkçe komut satırı önizlemesini içerir:

- sürümlenmiş Türkçe mod bildirimi modeli
- kalıcı ve otomatik medya varlığı kimlikleri
- oyun güncellemelerinden sonra güven puanlı yeniden eşleştirme
- mod bildirimi doğrulama
- Türkçe JSON şeması ve örnek medya modu
- Türkçe uygulama çıktıları ve hata iletileri
- Türkçe GitHub iş akışları ve sorun kayıtları
- otomatik derleme, test ve GitHub sürümü

UMMF özgün oyun dosyalarını yerinde değiştirmez. Çalışma zamanı uyarlayıcıları, oyun çalışırken varlıkları bulup güvenli biçimde değiştirecektir.

## Hedeflenen çalışma ortamları

Uyumluluk tek bir eklenti DLL'siyle değil, ortak çekirdeği kullanan ayrı host paketleriyle sağlanacaktır:

- eski Unity Mono oyunları için uyumluluk hostu
- eski BepInEx nesilleriyle çalışabilen Mono uyarlayıcıları
- BepInEx 5 Unity Mono hostu
- BepInEx 6 Unity Mono hostu
- BepInEx 6 IL2CPP hostu ve IL2CPP birlikte çalışma katmanı
- x86 ve x64 oyunlar için ayrı paketleme
- ilerleyen aşamalarda Windows dışındaki desteklenebilir masaüstü platformları

Her eski Unity veya BepInEx sürümünün aynı özellikleri desteklemesi garanti edilemez. UMMF oyun açılışında yetenek algılama yapacak; desteklenmeyen özelliği yanlış biçimde uygulamak yerine kapatıp raporlayacaktır.

## İndirme

Deneme paketleri GitHub sayfasındaki **Sürümler** bölümünde yayımlanır. Windows x64 paketi kendi .NET çalışma zamanını içerir.

İndirilecek paket:

`UMMF-v0.2.0-onizleme.2-windows-x64.zip`

## Komut satırı kullanımı

```powershell
./ummf.exe bilgi
./ummf.exe dogrula ./ornekler/OrnekMedyaModu/mod.json
./ummf.exe kimlik-demo
./ummf.exe eslestirme-demo
./ummf.exe yardim
```

### `bilgi`

Sürümü ve önizlemenin kapsamını gösterir.

### `dogrula <mod.json>`

Türkçe UMMF mod bildirimini okur ve doğrular.

### `kimlik-demo`

Bir altyazı için güncellemeler arasında korunabilecek kararlı kimlik üretir.

### `eslestirme-demo`

İçerik özeti değişmiş bir dokunun bağlamsal bilgilerle yeniden eşleştirilmesini gösterir.

## Kaynaktan derleme

```bash
dotnet restore UMMF.sln
dotnet build UMMF.sln --configuration Release
dotnet test UMMF.sln --configuration Release
```

Geliştirme için .NET 8 SDK veya daha yeni bir sürüm önerilir. Unity ile paylaşılacak çekirdek kitaplıklar, eski Unity Mono ortamlarıyla uyumluluk için `netstandard2.0` hedefler. Host projeleri ise hedefledikleri Unity ve BepInEx çalışma zamanına göre ayrı framework ve bağımlılıklarla derlenecektir.

## Yol haritası

1. Unity sürümü, Mono/IL2CPP arka ucu, işlemci mimarisi ve yükleyici sürümü algılamasını eklemek.
2. BepInEx ve Unity'den bağımsız çalışma zamanı host sözleşmelerini kurmak.
3. Eski Unity Mono ve eski BepInEx nesilleri için uyumluluk katmanı oluşturmak.
4. BepInEx 5 ve BepInEx 6 Mono hostlarını oluşturmak.
5. BepInEx 6 IL2CPP hostunu ve IL2CPP nesne köprüsünü oluşturmak.
6. `Texture2D`, `Sprite` ve arayüz dokularını keşfedip değiştirmek.
7. TextMeshPro ve Unity UI altyazı kaynaklarını yakalamak.
8. Altyazı kimliklerine ses dosyaları bağlamak.
9. Oyun yapısı katalogları ve güncelleme geçiş raporları oluşturmak.
10. Addressables, FMOD ve Wwise uyarlayıcılarını eklemek.

## Kapsam ve güvenlik

UMMF, yasal çevrimdışı ve tek oyunculu modlama için tasarlanmıştır. Hile koruması atlatma, çok oyunculu oyun manipülasyonu ve telif hakkıyla korunan oyun varlıklarını yeniden dağıtma proje kapsamı dışındadır.

## Belgeler

- [Mimari](belgeler/mimari.md)
- [Uyumluluk hedefleri](belgeler/uyumluluk-hedefleri.md)
- [Güncelleme dayanıklılığı](belgeler/guncelleme-dayanikliligi.md)
- [Değişiklik günlüğü](DEGISIKLIKLER.md)
- [0.2.0 Önizleme 2 sürüm notları](belgeler/surum-notlari/0.2.0-onizleme.2.md)

## Lisans

MIT Lisansı — Türkçe çeviri için `LICENSE` dosyasına bakın.
