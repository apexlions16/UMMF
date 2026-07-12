# UMMF Oyun Uyumluluk Listesi

Bu liste topluluk ve proje geliştiricileri tarafından gerçek oyunlarda yapılan testlerden üretilir. Kayıtların tek veri kaynağı `uyumluluk/oyunlar.json` dosyasıdır ve liste oyun adına göre alfabetik sıralanır.

## Durum işaretleri

- ✅ Çalışıyor: Özellik gerçek oyunda doğrulandı.
- 🟡 Kısmi: Özelliğin yalnızca bir bölümü çalışıyor veya oyun sürümüne bağlı sınırlama var.
- ⏳ Test bekliyor: Altyapı hazır ancak gerçek oyun doğrulaması tamamlanmadı.
- ❌ Çalışmıyor: Test edildi ve başarısız oldu.
- —: İlgili özellik bu oyun kaydında hedeflenmiyor.
- ❔ Bilinmiyor: Henüz yeterli bilgi yok.

## Oyunlar

> Henüz gerçek kullanıcı testiyle doğrulanmış bir oyun kaydı yok. İlk kaydı eklemek için GitHub'daki **Oyun uyumluluğu bildir** formunu kullanın.

## Genel host durumu

| Ortam | Kurulum/başlatma | Doku | Ses | Altyazı | Altyazı seslendirme |
|---|---|---|---|---|---|
| Eski Unity Mono + eski BepInEx | Planlanıyor | ⏳ | ⏳ | ⏳ | ⏳ |
| Unity Mono + BepInEx 5 | ✅ İlk host hazır; gerçek oyun testleri sürüyor | ⏳ | ⏳ | ⏳ | ⏳ |
| Unity Mono + BepInEx 6 | Host altyapısı planlandı | ⏳ | ⏳ | ⏳ | ⏳ |
| Unity IL2CPP + BepInEx 6 | Host altyapısı planlandı | ⏳ | ⏳ | ⏳ | ⏳ |

## Katkı gönderme

1. GitHub **Oyun uyumluluğu bildir** formunu doldurun.
2. Ayrıntılı destek için `oyun-profilleri/<oyun-kimligi>/` klasörü ekleyin.
3. Telifli oyun dosyalarını değil; `profil.json`, UMMF raporu, ilgili log satırları, test notları ve kendi kodunuzu gönderin.
4. Pull request içinde hangi medya alanlarının gerçekten test edildiğini açıkça işaretleyin.

Son veri güncellemesi: `2026-07-12`
