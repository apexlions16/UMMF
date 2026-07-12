# Uyumluluk Verisi

`oyunlar.json`, UMMF oyun uyumluluk listesinin tek makine tarafından okunan veri kaynağıdır.

## Kayıt biçimi

```json
{
  "kimlik": "ornek-oyun",
  "ad": "Örnek Oyun",
  "testEdilenOyunSurumu": "1.0.0",
  "unitySurumu": "2019.4.40f1",
  "betikArkaUcu": "Mono",
  "mimari": "x64",
  "bepInEx": "BepInEx 5.4.23.2",
  "host": "BepInEx 5 Mono",
  "doku": "test-bekliyor",
  "ses": "test-bekliyor",
  "altyazi": "test-bekliyor",
  "seslendirme": "test-bekliyor",
  "sonTestTarihi": "2026-07-12",
  "kanit": "https://github.com/apexlions16/UMMF/issues/000"
}
```

## Destek durumları

- `calisiyor`
- `kismi`
- `test-bekliyor`
- `calismiyor`
- `destek-yok`
- `bilinmiyor`

## Kurallar

- Oyunlar `ad` alanına göre alfabetik sıralanmalıdır.
- `kimlik` ve `ad` değerleri yinelenemez.
- Doğrulanmamış özellikler `calisiyor` olarak işaretlenemez.
- `kanit`, ilgili issue veya pull request bağlantısını göstermelidir.
- Oyun kaydının karşılığı mümkün olduğunda `oyun-profilleri/<kimlik>/` altında bulunmalıdır.

Listeyi yerelde üretmek için:

```powershell
python araclar/uyumluluk_listesi_uret.py
```
