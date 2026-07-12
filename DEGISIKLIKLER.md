# Değişiklik Günlüğü

## 0.6.0-onizleme.1

- UMMF'nin temel kullanımını PowerShell'den çıkaran Türkçe Windows masaüstü arayüzü eklendi.
- Koyu temalı, sol menülü ve kart tabanlı yeni `UMMF.Masaustu` projesi eklendi.
- Ana Sayfa, Oyun ve Kurulum, Uyumluluk, Topluluk Katkısı ve Günlük ekranları eklendi.
- Windows klasör seçiciyle Unity oyunu seçme ve otomatik tarama eklendi.
- Unity sürümü, Mono/IL2CPP, mimari, BepInEx, host, TextMeshPro, Unity UI, FMOD ve Wwise bilgileri arayüze taşındı.
- Ses planı, kurulum, durum, rapor ve kaldırma işlemleri düğmelere bağlandı.
- `uyumluluk/oyunlar.json` içeriğini gösteren gömülü alfabetik uyumluluk tablosu eklendi.
- Doku, ses, altyazı ve seslendirme durumlarının seçilebildiği topluluk katkı formu eklendi.
- Oyun dosyası veya telifli varlık içermeyen güvenli katkı ZIP'i üretimi eklendi.
- Türkçe işlem günlüğü, renkli başarı/uyarı/hata durumları ve metin dışa aktarma eklendi.
- Masaüstü uygulaması için Windows DPI ve asInvoker bildirimi eklendi.
- GUI EXE'nin beş ekranını doğrulayan `--arayuz-testi` duman testi eklendi.
- Release iş akışı komut satırı EXE'si yerine self-contained tek dosyalı GUI EXE yayımlayacak şekilde değiştirildi.
- CLI kurulum motoru arayüz tarafından yeniden kullanılabilir hâle getirildi.
- GUI, CLI ve BepInEx plugin sürümleri `0.6.0-onizleme.1` olarak eşitlendi.

## 0.5.0-onizleme.1

- Bad Parenting 1: Mr. Red Face için EXE, Data, ürün metadata’sı, Unity Mono x86 ve bilinen dosya özetlerine dayalı oyun profili eklendi.
- `MOD` kaynağındaki BepInEx 5 x86 altyapısını ve satır indeksli sesleri kuran adapter eklendi.
- 214 satırlık CSV eşlemesi ile WAV/OGG/MP3 dosya başlığı, yinelenen indeks ve eksik ses doğrulaması eklendi.
- `Dialogue.StartDialogue` akışı Harmony ile bağlanarak Türkçe dilinde satır başına tek Unity `AudioSource` oynatımı eklendi.
- `ses-planla` dry-run komutu; kurulum, durum, rapor, tekrar kurulum ve güvenli kaldırma desteği eklendi.
- Çakışan oyun/yükleyici dosyaları için SHA-256 izleme, otomatik yedek ve koşullu geri yükleme eklendi.
- Bad Parenting algılama, plan, eşleme, idempotent kurulum ve kaldırma testleri eklendi.

## Ana dal — topluluk katkı ve uyumluluk altyapısı

- README, topluluk desteği ve katkı yollarını görünür biçimde açıklayacak şekilde genişletildi.
- Testçiler için GitHub **Oyun uyumluluğu bildir** seçim formu eklendi.
- Yazılımcılar için doku, ses, altyazı, seslendirme, Addressables, FMOD ve Wwise alanlarını seçebilen **Medya uyarlayıcısı geliştirme** formu eklendi.
- Fork, branch ve pull request akışını açıklayan `CONTRIBUTING.md` eklendi.
- Oyunlara özgü `oyun-profilleri/<oyun-kimligi>/` klasör standardı ve JSON şablonu eklendi.
- Oyun adı, Unity sürümü, Mono/IL2CPP, mimari, BepInEx/host ve medya alanlarını izleyen alfabetik `UYUMLULUK.md` listesi eklendi.
- `uyumluluk/oyunlar.json` tek uyumluluk veri kaynağı olarak tanımlandı.
- Yinelenen oyunları, geçersiz durumları ve alfabetik sıralamayı doğrulayan liste üreticisi eklendi.
- Uyumluluk tablosunun ve oyun profillerinin pull requestlerde otomatik denetlenmesini sağlayan GitHub Actions iş akışı eklendi.
- Oyun dosyalarının ve telifli varlıkların gönderilmesini engelleyen katkı güvenlik kuralları eklendi.
- Topluluk katkıları için ayrıntılı pull request kontrol listesi eklendi.

## 0.4.0-onizleme.1

- Gerçek BepInEx 5 Unity Mono plugin projesi eklendi.
- Aynı plugin kaynak kodu `net35` ve `netstandard2.0` hedefleri için derlenir hale getirildi.
- Eski Unity Mono oyunlarında `net35`, Managed klasöründe `netstandard.dll` bulunan oyunlarda `netstandard2.0` seçimi eklendi.
- `BepInPlugin` ve `BaseUnityPlugin` tabanlı gerçek oyun içi giriş sınıfı eklendi.
- Türkçe başlangıç, ortam, mod keşfi ve hata logları eklendi.
- `BepInEx/UMMF/modlar` ve `BepInEx/UMMF/raporlar` klasörleri eklendi.
- Oyun içi `uyumluluk-raporu.json` üretimi eklendi.
- Windows EXE içine iki BepInEx plugin DLL'sini gömme altyapısı eklendi.
- `kur`, `durum`, `rapor` ve `kaldir` komutları eklendi.
- IL2CPP, BepInEx 6 veya doğrulanamayan oyun ortamına yanlış plugin kurulması engellendi.
- Plugin DLL'si için SHA-256 durum denetimi eklendi.
- Kaldırma sırasında kullanıcı modlarının ve raporlarının korunması sağlandı.
- Eski/yeni Mono hedef seçimi ve güvenli reddetme davranışları için birim testleri eklendi.
- BepInEx 5 Mono kurulum ve gerçek oyun test belgesi eklendi.

## 0.3.0-onizleme.2

- GitHub Release çıktısı ZIP yerine doğrudan Windows x64 EXE olacak şekilde değiştirildi.
- Özel kaynak kodu ZIP paketi Release varlıklarından kaldırıldı.
- EXE kendi .NET çalışma zamanını içeren tek dosyalı uygulama olarak paketlenir hale getirildi.
- Release derlemesi `windows-latest` üzerinde çalışacak şekilde değiştirildi.
- Üretilen EXE'nin Windows üzerinde gerçekten açıldığı doğrulanan açılış testi eklendi.
- `bilgi` komutunun doğru sürümü verdiği doğrulanmadan Release oluşturulmaması sağlandı.
- Release içinde doğrudan `.exe` varlığı bulunması zorunlu hale getirildi.
- EXE için SHA-256 doğrulama dosyası oluşturulması korundu.
- README indirme ve test yönergeleri doğrudan EXE kullanımına göre güncellendi.

## 0.3.0-onizleme.1

- Ortak `OyunOrtami`, yükleyici bilgisi ve çalışma zamanı host sözleşmeleri eklendi.
- Unity oyun klasörü ve `*_Data` dizini algılama eklendi.
- Mono ve IL2CPP betik arka ucu ayrımı eklendi.
- Unity sürümünü `globalgamemanagers` ve benzeri dosyalardan okuma eklendi.
- PE başlığından x86, x64, ARM32 ve ARM64 mimari algılama eklendi.
- BepInEx kurulumu, sürümü ve nesli algılama eklendi.
- TextMeshPro, Unity UI, Addressables, FMOD ve Wwise dosya izleri algılanır hale getirildi.
- Eski BepInEx Mono, BepInEx 5 Mono, BepInEx 6 Mono ve BepInEx 6 IL2CPP host profilleri eklendi.
- Yükleyicisiz Mono ve IL2CPP oyunları için güvenli kurulum adayı sonucu eklendi.
- `oyun-tara` ve `host-demo` komutları eklendi.
- Dört host senaryosu ile Mono/IL2CPP klasör tarama birim testleri eklendi.
- README, mimari ve uyumluluk belgeleri yeni altyapıya göre güncellendi.

## 0.2.0-onizleme.2

- Türkçeleştirme sonrasındaki bütün GitHub iş akışları son biçimine getirildi.
- Sürüm çalışmaları eşzamanlı çakışmaları önlemek için sıraya alındı.
- Etiket ve GitHub Release kaydı atomik biçimde oluşturulacak şekilde yayın sistemi düzeltildi.
- Başarılı yayın sonrasında eski geçici hata kayıtlarını doğrulayıp kapatan denetim eklendi.
- Eski görev, sorun ve birleştirme isteği metinleri Türkçeleştirildi.
- Komut satırı uygulamasının sürüm bilgisi güncellendi.

## 0.2.0-onizleme.1

- Projenin kullanıcıya görünen tüm metinleri Türkçeleştirildi.
- Kaynak kodundaki UMMF'ye ait sınıf, metot, özellik ve test adları Türkçeleştirildi.
- Mod bildirimi anahtarları ve JSON şeması Türkçeleştirildi.
- Komut satırı komutları, hata iletileri ve örnek çıktılar Türkçeleştirildi.
- GitHub Actions iş adımları, hata raporları ve sürüm notları Türkçeleştirildi.
- Belge, örnek, şema ve proje klasörleri Türkçe adlarla yeniden düzenlendi.

## 0.1.0 Önizleme 1

Tarihsel Git etiketi: `v0.1.0-preview.1`

- Çalışma zamanından bağımsız çekirdek oluşturuldu.
- Güncellemelere dayanıklı varlık kimliği ve eşleştirme sistemi eklendi.
- Mod bildirimi doğrulama sistemi eklendi.
- İlk komut satırı önizlemesi ve otomatik sürüm üretimi eklendi.
