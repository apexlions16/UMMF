#!/usr/bin/env python3
"""UMMF oyun uyumluluk verisini doğrular ve alfabetik Markdown tablosu üretir."""

from __future__ import annotations

import json
import sys
import unicodedata
from pathlib import Path
from typing import Any

KOK = Path(__file__).resolve().parents[1]
VERI_YOLU = KOK / "uyumluluk" / "oyunlar.json"
CIKTI_YOLU = KOK / "UYUMLULUK.md"

DURUMLAR = {
    "calisiyor": "✅ Çalışıyor",
    "kismi": "🟡 Kısmi",
    "test-bekliyor": "⏳ Test bekliyor",
    "calismiyor": "❌ Çalışmıyor",
    "destek-yok": "—",
    "bilinmiyor": "❔ Bilinmiyor",
}


def siralama_anahtari(metin: str) -> str:
    sade = unicodedata.normalize("NFKD", metin.casefold())
    return "".join(karakter for karakter in sade if not unicodedata.combining(karakter))


def gerekli_metin(oyun: dict[str, Any], alan: str) -> str:
    deger = oyun.get(alan)
    if not isinstance(deger, str) or not deger.strip():
        raise ValueError(f"'{oyun.get('kimlik', '<kimlik-yok>')}' kaydında '{alan}' zorunludur.")
    return deger.strip()


def durumu_yaz(oyun: dict[str, Any], alan: str) -> str:
    deger = oyun.get(alan, "bilinmiyor")
    if deger not in DURUMLAR:
        raise ValueError(
            f"'{oyun.get('kimlik', '<kimlik-yok>')}' kaydındaki '{alan}' değeri geçersiz: {deger}"
        )
    return DURUMLAR[deger]


def markdown_hucresi(deger: Any) -> str:
    if deger is None or deger == "":
        return "—"
    return str(deger).replace("|", "\\|").replace("\n", " ")


def veriyi_oku() -> dict[str, Any]:
    try:
        veri = json.loads(VERI_YOLU.read_text(encoding="utf-8"))
    except FileNotFoundError as hata:
        raise ValueError(f"Uyumluluk veri dosyası bulunamadı: {VERI_YOLU}") from hata
    except json.JSONDecodeError as hata:
        raise ValueError(f"Uyumluluk JSON dosyası geçersiz: {hata}") from hata

    if veri.get("semaSurumu") != 1:
        raise ValueError("Desteklenmeyen uyumluluk şema sürümü.")
    if not isinstance(veri.get("oyunlar"), list):
        raise ValueError("'oyunlar' alanı bir liste olmalıdır.")
    return veri


def oyunlari_dogrula(oyunlar: list[dict[str, Any]]) -> list[dict[str, Any]]:
    kimlikler: set[str] = set()
    adlar: set[str] = set()

    for oyun in oyunlar:
        if not isinstance(oyun, dict):
            raise ValueError("Her oyun kaydı bir JSON nesnesi olmalıdır.")

        kimlik = gerekli_metin(oyun, "kimlik")
        ad = gerekli_metin(oyun, "ad")
        gerekli_metin(oyun, "unitySurumu")
        gerekli_metin(oyun, "betikArkaUcu")
        gerekli_metin(oyun, "bepInEx")
        gerekli_metin(oyun, "testEdilenOyunSurumu")
        gerekli_metin(oyun, "sonTestTarihi")

        if kimlik in kimlikler:
            raise ValueError(f"Yinelenen oyun kimliği: {kimlik}")
        kimlikler.add(kimlik)

        ad_anahtari = siralama_anahtari(ad)
        if ad_anahtari in adlar:
            raise ValueError(f"Yinelenen oyun adı: {ad}")
        adlar.add(ad_anahtari)

        for alan in ("doku", "ses", "altyazi", "seslendirme"):
            durumu_yaz(oyun, alan)

    sirali = sorted(oyunlar, key=lambda oyun: siralama_anahtari(oyun["ad"]))
    if oyunlar != sirali:
        raise ValueError("'uyumluluk/oyunlar.json' oyun adına göre alfabetik sıralanmalıdır.")
    return sirali


def markdown_uret(veri: dict[str, Any], oyunlar: list[dict[str, Any]]) -> str:
    satirlar = [
        "# UMMF Oyun Uyumluluk Listesi",
        "",
        "Bu liste topluluk ve proje geliştiricileri tarafından gerçek oyunlarda yapılan testlerden üretilir. "
        "Kayıtların tek veri kaynağı `uyumluluk/oyunlar.json` dosyasıdır ve liste oyun adına göre alfabetik sıralanır.",
        "",
        "## Durum işaretleri",
        "",
        "- ✅ Çalışıyor: Özellik gerçek oyunda doğrulandı.",
        "- 🟡 Kısmi: Özelliğin yalnızca bir bölümü çalışıyor veya oyun sürümüne bağlı sınırlama var.",
        "- ⏳ Test bekliyor: Altyapı hazır ancak gerçek oyun doğrulaması tamamlanmadı.",
        "- ❌ Çalışmıyor: Test edildi ve başarısız oldu.",
        "- —: İlgili özellik bu oyun kaydında hedeflenmiyor.",
        "- ❔ Bilinmiyor: Henüz yeterli bilgi yok.",
        "",
        "## Oyunlar",
        "",
    ]

    if not oyunlar:
        satirlar.extend(
            [
                "> Henüz gerçek kullanıcı testiyle doğrulanmış bir oyun kaydı yok. "
                "İlk kaydı eklemek için GitHub'daki **Oyun uyumluluğu bildir** formunu kullanın.",
                "",
            ]
        )
    else:
        satirlar.extend(
            [
                "| Oyun | Test edilen sürüm | Unity | Arka uç | Mimari | BepInEx / Host | Doku | Ses | Altyazı | Altyazı seslendirme | Son test | Kanıt |",
                "|---|---|---|---|---|---|---|---|---|---|---|---|",
            ]
        )
        for oyun in oyunlar:
            kanit = oyun.get("kanit")
            kanit_hucresi = f"[Kayıt]({kanit})" if kanit else "—"
            host = oyun.get("host") or oyun["bepInEx"]
            satirlar.append(
                "| "
                + " | ".join(
                    [
                        markdown_hucresi(oyun["ad"]),
                        markdown_hucresi(oyun["testEdilenOyunSurumu"]),
                        markdown_hucresi(oyun["unitySurumu"]),
                        markdown_hucresi(oyun["betikArkaUcu"]),
                        markdown_hucresi(oyun.get("mimari")),
                        markdown_hucresi(host),
                        durumu_yaz(oyun, "doku"),
                        durumu_yaz(oyun, "ses"),
                        durumu_yaz(oyun, "altyazi"),
                        durumu_yaz(oyun, "seslendirme"),
                        markdown_hucresi(oyun["sonTestTarihi"]),
                        kanit_hucresi,
                    ]
                )
                + " |"
            )
        satirlar.append("")

    satirlar.extend(
        [
            "## Genel host durumu",
            "",
            "| Ortam | Kurulum/başlatma | Doku | Ses | Altyazı | Altyazı seslendirme |",
            "|---|---|---|---|---|---|",
            "| Eski Unity Mono + eski BepInEx | Planlanıyor | ⏳ | ⏳ | ⏳ | ⏳ |",
            "| Unity Mono + BepInEx 5 | ✅ İlk host hazır; gerçek oyun testleri sürüyor | ⏳ | ⏳ | ⏳ | ⏳ |",
            "| Unity Mono + BepInEx 6 | Host altyapısı planlandı | ⏳ | ⏳ | ⏳ | ⏳ |",
            "| Unity IL2CPP + BepInEx 6 | Host altyapısı planlandı | ⏳ | ⏳ | ⏳ | ⏳ |",
            "",
            "## Katkı gönderme",
            "",
            "1. GitHub **Oyun uyumluluğu bildir** formunu doldurun.",
            "2. Ayrıntılı destek için `oyun-profilleri/<oyun-kimligi>/` klasörü ekleyin.",
            "3. Telifli oyun dosyalarını değil; `profil.json`, UMMF raporu, ilgili log satırları, test notları ve kendi kodunuzu gönderin.",
            "4. Pull request içinde hangi medya alanlarının gerçekten test edildiğini açıkça işaretleyin.",
            "",
            f"Son veri güncellemesi: `{veri.get('sonGuncelleme', 'bilinmiyor')}`",
            "",
        ]
    )
    return "\n".join(satirlar)


def ana() -> int:
    try:
        veri = veriyi_oku()
        oyunlar = oyunlari_dogrula(veri["oyunlar"])
        CIKTI_YOLU.write_text(markdown_uret(veri, oyunlar), encoding="utf-8", newline="\n")
        print(f"Uyumluluk listesi üretildi: {CIKTI_YOLU}")
        return 0
    except ValueError as hata:
        print(f"UYUMLULUK HATASI: {hata}", file=sys.stderr)
        return 1


if __name__ == "__main__":
    raise SystemExit(ana())
