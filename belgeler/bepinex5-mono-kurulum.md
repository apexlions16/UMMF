# BepInEx 5 Unity Mono kurulumu

Bu belge, UMMF Windows EXE kullanılarak BepInEx 5 tabanlı Unity Mono oyununa ilk çalışma zamanı hostunun kurulmasını açıklar.

## Ön koşullar

- Windows x64
- Unity Mono tabanlı bir oyun
- Oyunun ana klasöründe çalışan BepInEx 5 kurulumu
- Tek oyunculu veya çevrimdışı, modlamaya izin verilen bir test ortamı

UMMF, IL2CPP oyununa veya BepInEx 6 kurulumuna BepInEx 5 Mono DLL'sini yerleştirmez. Ortam doğrulanamazsa kurulum güvenli biçimde durur.

## Ortamı tarama

```powershell
./UMMF-v0.5.0-onizleme.1-windows-x64.exe oyun-tara "D:\Oyunlar\OrnekOyun"
```

Beklenen temel sonuçlar:

- Betik arka ucu: `Mono`
- Yükleyici: `BepInEx 5.x`
- Seçilen host: `BepInEx 5 Mono hostu`

## Kurulum

```powershell
./UMMF-v0.5.0-onizleme.1-windows-x64.exe kur "D:\Oyunlar\OrnekOyun"
```

Kurucu oyun ortamına göre iki DLL'den birini seçer:

- `net35`: eski Unity Mono oyunları
- `netstandard2.0`: Managed klasöründe `netstandard.dll` bulunan daha yeni Unity Mono oyunları

Kurulum sonucunda şu yapı oluşur:

```text
BepInEx/
├── plugins/
│   └── UMMF/
│       └── UMMF.BepInEx5.Mono.dll
└── UMMF/
    ├── modlar/
    ├── raporlar/
    └── kurulum-bilgisi.json
```

Özgün Unity varlık dosyaları ve oyun çalıştırılabilir dosyası değiştirilmez.

## Durum denetimi

```powershell
./UMMF-v0.5.0-onizleme.1-windows-x64.exe durum "D:\Oyunlar\OrnekOyun"
```

Komut plugin dosyasını, seçilen hedef çerçeveyi ve SHA-256 özetini gösterir.

## Oyunda doğrulama

Oyunu bir kez başlatıp ana menüye ulaştıktan sonra kapat. Ardından `BepInEx/LogOutput.log` içinde şu iletileri ara:

```text
UMMF 0.5.0-onizleme.1 başlatılıyor.
Çalışma zamanı hostu: BepInEx 5 Unity Mono
UMMF mod klasörü hazır
UMMF başlangıcı başarıyla tamamlandı.
```

Oyun içi host ayrıca şu raporu üretir:

```text
BepInEx/UMMF/raporlar/uyumluluk-raporu.json
```

## Teşhis raporu

Oyun açılmadan önce kurulum raporu üretmek için:

```powershell
./UMMF-v0.5.0-onizleme.1-windows-x64.exe rapor "D:\Oyunlar\OrnekOyun"
```

Rapor konumu:

```text
BepInEx/UMMF/raporlar/kurulum-teshis-raporu.json
```

## Kaldırma

```powershell
./UMMF-v0.5.0-onizleme.1-windows-x64.exe kaldir "D:\Oyunlar\OrnekOyun"
```

Kaldırma yalnızca UMMF plugin DLL'sini ve kurulum bilgisini siler. Kullanıcının `modlar` klasörü ile raporları korunur.

## Sorun bildirirken gönderilecek dosyalar

- PowerShell komut çıktısı
- `BepInEx/LogOutput.log`
- `BepInEx/UMMF/raporlar/uyumluluk-raporu.json`
- `BepInEx/UMMF/raporlar/kurulum-teshis-raporu.json`

Oyun dosyalarının veya telif hakkıyla korunan varlıkların paylaşılması gerekmez.
