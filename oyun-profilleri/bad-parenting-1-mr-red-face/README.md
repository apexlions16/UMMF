# Bad Parenting 1: Mr. Red Face

Desteklenen profil: Windows x86, Unity `2022.2.0b16`, Mono, Unity `AudioSource` ve `UnityWebRequestAudioModule`. FMOD/Wwise kullanılmaz.

```powershell
ummf ses-planla "<oyun-dizini>"
ummf kur "<oyun-dizini>"
ummf durum "<oyun-dizini>"
ummf rapor "<oyun-dizini>"
ummf kaldir "<oyun-dizini>"
```

Sesler oyun kökündeki `MOD/Dub` içinde manifestteki `000_SON.wav` biçiminde bulunur. WAV, OGG ve MP3 desteklenir. Telifli oyun sesleri dağıtılmaz.

Bilinen sınırlama: doğrulanan yerel `MOD/Dub` boştu; altyapı ve güvenli dosya yaşam döngüsü test edildi, oyun içi işitsel doğrulama bekliyor.
