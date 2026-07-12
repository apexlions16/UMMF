# Katkıcı Hızlı Başlangıç

## Yalnızca oyun testi göndermek

1. Latest Release içindeki Windows EXE'yi indirin.
2. Oyunda `oyun-tara`, uygun ortamdaysa `kur`, ardından `durum` ve `rapor` komutlarını çalıştırın.
3. Oyunu açıp kapatın.
4. `BepInEx/LogOutput.log` dosyasındaki yalnızca UMMF satırlarını ayırın.
5. GitHub'da **Oyun uyumluluğu bildir** formunu doldurun.

## Oyun profili göndermek

1. Repoyu fork edin.
2. `oyun-profilleri/_sablon` içeriğini `oyun-profilleri/<oyun-kimligi>` altına kopyalayın.
3. `profil.json` ve test notlarını gerçek bilgilerle doldurun.
4. `uyumluluk/oyunlar.json` dosyasına alfabetik kayıt ekleyin.
5. `python araclar/uyumluluk_listesi_uret.py` çalıştırın.
6. Pull request açın.

## Kod katkısı göndermek

1. Önce **Medya uyarlayıcısı geliştirme** formunu açın.
2. Hedef oyun/ortam, host ve medya alanını seçin.
3. Fork üzerinde ayrı bir katkı dalı oluşturun.
4. Kodla birlikte otomatik test ve Türkçe belge ekleyin.
5. Oyun dosyası veya telifli varlık eklemeden pull request açın.

## İnceleme sonucu

- Uyumluluk testleri listeye eklenir.
- Oyun profilleri `oyun-profilleri/` altında tutulur.
- Genel kullanım sağlayan kod ortak çekirdek veya host katmanına alınır.
- Oyuna özgü kod yalnızca genel çözüm mümkün değilse profil uyarlayıcısı olarak tutulur.
- Test edilebilir kod güncellemeleri uygun sonraki normal GitHub Release'e dahil edilir.
