# UMMF

**UMMF — Evrensel Medya Modlama Çerçevesi**, Unity oyunlarındaki dokuları, sesleri ve altyazıları çalışma zamanında keşfetmek, dışarı aktarmak ve değiştirmek için geliştirilen güncellemelere dayanıklı bir modlama altyapısıdır.

Projenin odaklandığı üç medya alanı:

- dokular ve görsel parçalar
- ses klipleri ve altyazı tetiklemeli seslendirme
- altyazılar ve yerelleştirme metinleri

İlk çalışma zamanı hedefi, BepInEx 5 kullanan Windows x64 Unity Mono oyunlarıdır. IL2CPP, Addressables, FMOD ve Wwise destekleri ayrı uyarlayıcılar olarak planlanmaktadır.

## Güncel durum

Bu sürüm çalışma zamanından bağımsız temeli ve Türkçe komut satırı önizlemesini içerir:

- sürümlenmiş Türkçe mod bildirimi modeli
- kalıcı ve otomatik medya varlığı kimlikleri
- oyun güncellemelerinden sonra güven puanlı yeniden eşleştirme
- mod bildirimi doğrulama
- Türkçe JSON şeması ve örnek medya modu
- otomatik derleme, test ve GitHub sürümü

UMMF özgün oyun dosyalarını yerinde değiştirmez. İleride eklenecek çalışma zamanı uyarlayıcıları, oyun çalışırken varlıkları bulup güvenli biçimde değiştirecektir.

## İndirme

Deneme paketleri GitHub sayfasındaki **Sürümler** bölümünde yayımlanır. Windows x64 paketi kendi .NET çalışma zamanını içerir.

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

Geliştirme için .NET 8 SDK veya daha yeni bir sürüm önerilir. Unity ile paylaşılacak çekirdek kitaplıklar, eski Unity Mono ortamlarıyla uyumluluk için `netstandard2.0` hedefler.

## Yol haritası

1. Mod bildirimi sürüm geçişlerini tamamlamak.
2. BepInEx 5 Mono eklenti girişini eklemek.
3. `Texture2D`, `Sprite` ve arayüz dokularını keşfedip değiştirmek.
4. TextMeshPro ve Unity UI altyazı kaynaklarını yakalamak.
5. Altyazı kimliklerine ses dosyaları bağlamak.
6. Oyun yapısı katalogları ve güncelleme geçiş raporları oluşturmak.
7. IL2CPP, Addressables, FMOD ve Wwise uyarlayıcılarını eklemek.

## Kapsam ve güvenlik

UMMF, yasal çevrimdışı ve tek oyunculu modlama için tasarlanmıştır. Hile koruması atlatma, çok oyunculu oyun manipülasyonu ve telif hakkıyla korunan oyun varlıklarını yeniden dağıtma proje kapsamı dışındadır.

## Belgeler

- [Mimari](belgeler/mimari.md)
- [Güncelleme dayanıklılığı](belgeler/guncelleme-dayanikliligi.md)
- [Sürüm notları](belgeler/surum-notlari/0.2.0-onizleme.1.md)

## Lisans

MIT Lisansı — Türkçe çeviri için `LICENSE` dosyasına bakın.
