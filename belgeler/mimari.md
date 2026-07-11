# Mimari

UMMF, oyuna özgü çalışma zamanı erişimini kararlı medya modu modelinden ayırır.

## Katmanlar

### UMMF.Sozlesmeler

Tarayıcılar, mod araçları ve çalışma zamanı uyarlayıcıları tarafından paylaşılacak bağımlılıksız veri sözleşmelerini içerir:

- medya varlığı türleri
- güncellemelere dayanıklı parmak izleri
- mod bildirimleri
- değiştirme yönergeleri

Kitaplık, eski Unity Mono çalışma zamanları tarafından kullanılabilmesi için `netstandard2.0` hedefler.

### UMMF.Cekirdek

Çalışma zamanından bağımsız davranışları içerir:

- belirlenimci varlık kimliği üretimi
- oyun güncellemelerinden sonra aday puanlama ve yeniden eşleştirme
- mod bildirimi doğrulama

Bu katman Unity, BepInEx, Harmony, FMOD veya Wwise'a başvuru eklememelidir.

### Çalışma zamanı uyarlayıcıları

Planlanan uyarlayıcılar, oyun nesnelerini keşfedip ortak sözleşmelere dönüştürecektir:

- BepInEx Mono
- BepInEx IL2CPP
- Unity ses sistemi
- FMOD
- Wwise
- Unity doku sistemi
- TextMeshPro altyazıları
- Unity UI altyazıları

## Çalışma akışı

1. Oyun, Unity sürümü ve betik altyapısı belirlenir.
2. Kurulu mod bildirimleri yüklenir.
3. Etkin dokular, ses klipleri ve altyazı kaynakları keşfedilir.
4. Keşifler `VarlikParmakIzi` değerlerine dönüştürülür.
5. Değişiklikler güven puanlarıyla eşleştirilir.
6. Yalnızca güven eşiğini geçen eşleşmeler uygulanır.
7. Çözülemeyen veya belirsiz eşleşmeler inceleme için kaydedilir.

## Güvenlik sınırları

UMMF; hile koruması atlatma, çok oyunculu oyun manipülasyonu veya çalıştırılabilir oyun yaması dağıtımı sağlamaz. İlk desteklenen ortam, Windows x64 üzerinde çevrimdışı veya tek oyunculu Unity Mono oyunlarıdır.
