# UMMF Oyun Profilleri

Bu klasör, belirli Unity oyunlarının UMMF ile nasıl algılandığını ve hangi medya özelliklerinin test edildiğini tanımlayan topluluk katkılarını içerir.

## Klasör düzeni

Her oyun için küçük harfli, kısa ve kararlı bir kimlik kullanın:

```text
oyun-profilleri/
└── ornek-oyun/
    ├── profil.json
    ├── README.md
    ├── testler/
    │   └── 1.2.3-windows-x64.md
    └── uyarlayicilar/
        └── istege-bagli-kod.cs
```

### `profil.json`

Zorunlu oyun, Unity, arka uç, mimari, BepInEx ve medya uyumluluk alanlarını taşır. Başlangıç için `_sablon/profil.json` dosyasını kopyalayın.

### `README.md`

Oyuna özgü kurulum notlarını, bilinen sınırlamaları ve test edilen sürümleri açıklar.

### `testler/`

Kullanılan UMMF sürümü, oyun sürümü, test adımları ve sonuçlar burada tutulur. Yalnızca ilgili log satırlarını ekleyin; kullanıcı adı, kurulum yolu ve diğer kişisel bilgileri temizleyin.

### `uyarlayicilar/`

Oyuna özel kod gerçekten gerekiyorsa burada tutulabilir. Mümkün olduğunda genel çekirdeğe veya ortak hosta katkı tercih edilir.

## Yasaklanan içerik

Aşağıdakileri repoya eklemeyin:

- oyun çalıştırılabilir dosyaları
- Unity asset bundle veya kaynak dosyaları
- telif hakkıyla korunan doku, ses, video ve altyazılar
- dağıtıma kapalı SDK veya middleware dosyaları
- kullanıcıya ait kayıt ve profil dosyaları
- hile korumasını atlatan veya çok oyunculu oyunu manipüle eden kod

## Kabul edilen içerik

- `profil.json`
- UMMF `oyun-tara` ve `rapor` çıktılarından kişisel bilgiler temizlenmiş özetler
- BepInEx logunun yalnızca ilgili UMMF satırları
- test adımları ve sonuçları
- sizin yazdığınız host, tarayıcı, eşleştirici veya medya uyarlayıcısı kodu
- açık lisanslı küçük test verileri

## Listeye eklenme

Bir oyun profili kabul edildiğinde aynı PR içinde `uyumluluk/oyunlar.json` kaydı eklenir. Otomasyon `UYUMLULUK.md` dosyasını oyun adına göre alfabetik üretir.
