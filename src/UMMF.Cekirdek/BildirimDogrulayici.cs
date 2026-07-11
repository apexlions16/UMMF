using UMMF.Sozlesmeler;

namespace UMMF.Cekirdek;

public static class BildirimDogrulayici
{
    public static IReadOnlyList<string> Dogrula(ModBildirimi bildirim)
    {
        if (bildirim is null)
        {
            throw new ArgumentNullException(nameof(bildirim));
        }

        var hatalar = new List<string>();
        if (bildirim.SemaSurumu != 1)
        {
            hatalar.Add("Yalnızca şema sürümü 1 destekleniyor.");
        }

        if (string.IsNullOrWhiteSpace(bildirim.Kimlik))
        {
            hatalar.Add("Mod kimliği zorunludur.");
        }

        if (string.IsNullOrWhiteSpace(bildirim.Ad))
        {
            hatalar.Add("Mod adı zorunludur.");
        }

        if (string.IsNullOrWhiteSpace(bildirim.Surum))
        {
            hatalar.Add("Mod sürümü zorunludur.");
        }

        var kimlikler = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var degisiklik in bildirim.Degisiklikler)
        {
            if (string.IsNullOrWhiteSpace(degisiklik.Kimlik))
            {
                hatalar.Add("Her değişiklik için bir kimlik gerekir.");
            }
            else if (!kimlikler.Add(degisiklik.Kimlik))
            {
                hatalar.Add($"Yinelenen değişiklik kimliği: {degisiklik.Kimlik}");
            }

            if (degisiklik.Tur == MedyaVarlikTuru.Bilinmiyor)
            {
                hatalar.Add($"'{degisiklik.Kimlik}' değişikliği bir medya türü belirtmelidir.");
            }

            if (degisiklik.Eslesme is null)
            {
                hatalar.Add($"'{degisiklik.Kimlik}' değişikliği bir eşleşme parmak izi gerektirir.");
            }

            if (string.IsNullOrWhiteSpace(degisiklik.YeniDosya) &&
                string.IsNullOrWhiteSpace(degisiklik.AltyaziMetni) &&
                string.IsNullOrWhiteSpace(degisiklik.SesDosyasi))
            {
                hatalar.Add($"'{degisiklik.Kimlik}' değişikliği herhangi bir medyayı değiştirmiyor.");
            }

            if (degisiklik.GecikmeMilisaniye < 0)
            {
                hatalar.Add($"'{degisiklik.Kimlik}' değişikliğinin gecikmesi negatif olamaz.");
            }

            if (degisiklik.SesDuzeyi < 0.0f || degisiklik.SesDuzeyi > 2.0f)
            {
                hatalar.Add($"'{degisiklik.Kimlik}' değişikliğinin ses düzeyi 0 ile 2 arasında olmalıdır.");
            }
        }

        return hatalar;
    }
}
