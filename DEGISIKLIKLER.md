# Değişiklik Günlüğü

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
