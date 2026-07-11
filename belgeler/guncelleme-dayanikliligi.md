# Güncelleme Dayanıklılığı

Oyun güncellemeleri medya varlıklarını yeniden adlandırabilir, taşıyabilir, yeniden sıkıştırabilir veya yeniden oluşturabilir. Bu nedenle UMMF tek bir dosya özetini kalıcı kimlik olarak kullanmaz.

## Parmak izi önceliği

Eşleştirme, bulunabilen en güçlü işaretleri kullanır:

1. yerelleştirme, Addressables veya oyun tarafından verilen kalıcı anahtar
2. birebir içerik özeti
3. anlamsal veya algısal özet
4. GameObject ve bileşen kullanım yolu
5. paket, varlık veya kapsayıcı adı
6. nesne adı
7. doku boyutları veya ses yapısı
8. altyazı metni benzerliği

Birebir içerik özeti değişmemiş varlıkları tanımlar. Kalıcı anahtarlar ve bağlamsal bilgiler, içerik değişse bile aynı mod kaydının korunmasını sağlar.

## Güven politikası

İlk eşikler:

- `0,95-1,00`: otomatik uygula
- `0,75-0,95`: uygula ve raporla
- `0,50-0,75`: inceleme iste
- `0,50` altı: uygulama

Çekirdek eşleştirici hem puanı hem de Türkçe eşleşme nedenlerini döndürür. Böylece sürüm geçişleri denetlenebilir.

## Sürüm görüntüleri

Tarayıcı, algılanan her oyun yapısı için katalog oluşturacaktır. Oyun yapısı değiştiğinde UMMF modun medya dosyalarını değiştirmek yerine eski ve yeni katalogları karşılaştıracaktır. Onaylanan yeniden eşlemeler küçük uyumluluk katmanlarında saklanacaktır.

```text
OyunProfilleri/
  temel.json
  1.4.x.katman.json
  1.5.0.katman.json
```

## Hata davranışı

Eksik veya belirsiz hedef güvenli biçimde başarısız olmalıdır: değişiklik atlanır ve günlüğe yazılır. UMMF, bir modu çalışır tutmak uğruna düşük güvenli bir doku, ses veya altyazıyı ilgisiz bir varlığa uygulamamalıdır.
