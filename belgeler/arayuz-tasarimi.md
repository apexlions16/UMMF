# UMMF masaüstü arayüz tasarımı

## Amaç

UMMF'nin tarama, kurulum, uyumluluk ve topluluk katkısı işlemlerini PowerShell komutları gerektirmeden tek bir Türkçe Windows uygulamasında sunmak.

## Ekranlar

### Ana Sayfa

- ürün ve sürüm bilgisi
- oyun seçme ve analiz etmeye hızlı geçiş
- güvenli kurulum ve topluluk katkısı açıklamaları
- GitHub ve Latest Release bağlantıları

### Oyun ve Kurulum

- klasör seçici
- Unity sürümü
- Mono veya IL2CPP bilgisi
- x86/x64/ARM mimari bilgisi
- BepInEx nesli ve sürümü
- seçilen çalışma zamanı hostu
- TextMeshPro, Unity UI, FMOD ve Wwise izleri
- ses planı, kurulum, durum, rapor ve kaldırma işlemleri

### Uyumluluk

- `uyumluluk/oyunlar.json` verisinden gömülü alfabetik liste
- oyun, Unity, arka uç, mimari ve BepInEx bilgileri
- doku, ses, altyazı ve seslendirme destek durumları
- GitHub'daki canlı uyumluluk listesine bağlantı

### Topluluk Katkısı

- taranan ortam bilgilerinden otomatik profil
- doku, ses, altyazı ve seslendirme için seçim kutuları
- test notu alanı
- oyun dosyası içermeyen ZIP katkı paketi
- GitHub katkı formuna doğrudan bağlantı

### Günlük

- tarih/saat içeren Türkçe işlem kayıtları
- tarama ve kurulum sonuçları
- SHA-256 ve oluşturulan klasör yolları
- metin dosyasına dışarı aktarma

## Görsel dil

- koyu arka plan
- mavi ana vurgu
- yeşil başarı, sarı uyarı ve kırmızı hata durumları
- sol gezinme menüsü
- kart tabanlı içerik düzeni
- Windows DPI ölçeklendirme desteği

## Güvenlik

Arayüz oyun dosyalarını katkı paketine eklemez. Katkı paketi yalnızca ortam metadatası, dosya adları, test sonuçları ve kullanıcı notlarından oluşur. Tam kullanıcı yolları ve kullanıcı adı temizlenir.
