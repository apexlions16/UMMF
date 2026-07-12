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

| Oyun | Test edilen sürüm | Unity | Arka uç | Mimari | BepInEx / Host | Doku | Ses | Altyazı | Altyazı seslendirme | Son test | Kanıt |
|---|---|---|---|---|---|---|---|---|---|---|---|
| Bad Parenting 1: Mr. Red Face | Steam derlemesi (2026-07-12) | 2022.2.0b16 | Mono | x86 | UMMF BepInEx 5 Mono | — | 🟡 Kısmi | — | ⏳ Test bekliyor | 2026-07-12 | [Kayıt](https://github.com/apexlions16/UMMF/pull/25) |

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
