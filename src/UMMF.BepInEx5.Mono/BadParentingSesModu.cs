using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace UMMF.BepInEx5.Mono;

internal static class BadParentingSesModu
{
    private const int TurkceDilIndeksi = 10;
    private static readonly Dictionary<int, AudioClip> Onbellek = new Dictionary<int, AudioClip>();
    private static ManualLogSource _gunluk;
    private static string _sesDizini;
    private static Type _diyalogTuru;
    private static Type _satirlarTuru;
    private static FieldInfo _metinAlani;
    private static FieldInfo _arkaPlanAlani;
    private static FieldInfo _yuklemeHiziAlani;
    private static FieldInfo _durumAlani;
    private static FieldInfo _coroutineAlani;
    private static MethodInfo _rolMetodu;
    private static MethodInfo _satirMetodu;

    public static bool Etkinlestir(string modDizini, ManualLogSource gunluk)
    {
        if (!string.Equals(Application.productName, "Bad Parenting 1", StringComparison.Ordinal))
        {
            return false;
        }

        var modKoku = Path.Combine(modDizini, "bad-parenting-ses");
        if (!File.Exists(Path.Combine(modKoku, "mod.json")))
        {
            gunluk.LogWarning("Bad Parenting profili algılandı; ses modu bildirimi kurulu değil.");
            return false;
        }

        _gunluk = gunluk;
        _sesDizini = Path.Combine(modKoku, "Dub");
        _diyalogTuru = AccessTools.TypeByName("Dialogue");
        _satirlarTuru = AccessTools.TypeByName("Lines");
        if (_diyalogTuru == null || _satirlarTuru == null)
        {
            gunluk.LogError("Bad Parenting Dialogue/Lines türleri bulunamadı; ses modu devre dışı.");
            return false;
        }

        _metinAlani = AccessTools.Field(_diyalogTuru, "text");
        _arkaPlanAlani = AccessTools.Field(_diyalogTuru, "background");
        _yuklemeHiziAlani = AccessTools.Field(_diyalogTuru, "loadSpeed");
        _durumAlani = AccessTools.Field(_diyalogTuru, "state");
        _coroutineAlani = AccessTools.Field(_diyalogTuru, "cor");
        _rolMetodu = AccessTools.Method(_diyalogTuru, "GetRole");
        _satirMetodu = AccessTools.Method(_satirlarTuru, "getLine", new[] { typeof(int) });
        if (_metinAlani == null || _arkaPlanAlani == null || _yuklemeHiziAlani == null ||
            _durumAlani == null || _coroutineAlani == null || _rolMetodu == null || _satirMetodu == null)
        {
            gunluk.LogError("Bad Parenting diyalog metadata alanları değişmiş; ses modu güvenli biçimde durduruldu.");
            return false;
        }

        MethodInfo hedef = null;
        var metotlar = _diyalogTuru.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        for (var i = 0; i < metotlar.Length; i++)
        {
            var parametreler = metotlar[i].GetParameters();
            if (metotlar[i].Name == "StartDialogue" && parametreler.Length == 3 &&
                parametreler[0].ParameterType == typeof(int) && parametreler[1].ParameterType == typeof(int))
            {
                hedef = metotlar[i];
                break;
            }
        }

        if (hedef == null)
        {
            gunluk.LogError("Bad Parenting StartDialogue imzası değişmiş; ses modu güvenli biçimde durduruldu.");
            return false;
        }

        var prefix = new HarmonyMethod(typeof(BadParentingSesModu), nameof(DiyalogBaslatPrefix));
        new Harmony("tr.ummf.bad-parenting-1.ses").Patch(hedef, prefix: prefix);
        gunluk.LogInfo("Bad Parenting 1 ses modu etkin: Unity AudioSource, satır indeksli harici WAV/OGG/MP3.");
        return true;
    }

    private static bool DiyalogBaslatPrefix(object __instance, object[] __args)
    {
        var davranis = (MonoBehaviour)__instance;
        var mevcut = _coroutineAlani.GetValue(__instance) as Coroutine;
        if (mevcut != null)
        {
            davranis.StopCoroutine(mevcut);
        }

        var from = (int)__args[0];
        var to = (int)__args[1];
        var callback = __args.Length > 2 ? __args[2] as Delegate : null;
        var yeni = davranis.StartCoroutine(DiyaloguCalistir(davranis, from, to, callback));
        _coroutineAlani.SetValue(__instance, yeni);
        return false;
    }

    private static IEnumerator DiyaloguCalistir(MonoBehaviour davranis, int from, int to, Delegate callback)
    {
        var arkaPlan = _arkaPlanAlani.GetValue(davranis);
        var metin = _metinAlani.GetValue(davranis);
        var metinOzelligi = metin.GetType().GetProperty("text");
        var etkinOzelligi = arkaPlan.GetType().GetProperty("enabled");
        var yuklemeHizi = (float)_yuklemeHiziAlani.GetValue(davranis);
        var turkce = PlayerPrefs.GetInt("language", 0) == TurkceDilIndeksi;

        etkinOzelligi.SetValue(arkaPlan, true, null);
        metinOzelligi.SetValue(metin, string.Empty, null);
        for (var indeks = from; indeks <= to; indeks++)
        {
            var parcalar = ((string)_satirMetodu.Invoke(null, new object[] { indeks })).Split('|');
            var diyalog = parcalar[parcalar.Length - 1];
            var rol = parcalar.Length > 1
                ? (string)_rolMetodu.Invoke(davranis, new object[] { parcalar[0] })
                : "SON";
            _durumAlani.SetValue(davranis, 1);

            if (turkce)
            {
                yield return SesiOynat(davranis.gameObject, indeks);
            }

            for (var i = 0; i < diyalog.Length; i++)
            {
                var gorunen = diyalog.Substring(0, i + 1);
                var gizli = diyalog.Substring(i + 1);
                metinOzelligi.SetValue(metin, rol + "\n" + gorunen + "<color=#00000000>" + gizli + "<color=#00000000>", null);
                yield return new WaitForSeconds(yuklemeHizi);
                if ((int)_durumAlani.GetValue(davranis) == 0)
                {
                    metinOzelligi.SetValue(metin, rol + "\n" + diyalog, null);
                    break;
                }
            }

            _durumAlani.SetValue(davranis, 2);
            yield return new WaitUntil(delegate { return (int)_durumAlani.GetValue(davranis) == 0; });
        }

        etkinOzelligi.SetValue(arkaPlan, false, null);
        metinOzelligi.SetValue(metin, string.Empty, null);
        if (callback != null)
        {
            callback.DynamicInvoke();
        }
    }

    private static IEnumerator SesiOynat(GameObject nesne, int indeks)
    {
        AudioClip klip;
        if (!Onbellek.TryGetValue(indeks, out klip))
        {
            var yol = SesDosyasiniBul(indeks);
            if (yol == null)
            {
                yield break;
            }

            using (var istek = new WWW(new Uri(yol).AbsoluteUri))
            {
                yield return istek;
                if (!string.IsNullOrEmpty(istek.error))
                {
                    _gunluk.LogWarning("Ses yüklenemedi: " + Path.GetFileName(yol) + " — " + istek.error);
                    yield break;
                }

                klip = istek.GetAudioClip(false, false, SesTurunuBul(yol));
                Onbellek[indeks] = klip;
            }
        }

        var kaynak = SesKaynaginiBul(nesne);
        kaynak.Stop();
        kaynak.clip = klip;
        kaynak.Play();
    }

    private static string SesDosyasiniBul(int indeks)
    {
        if (!Directory.Exists(_sesDizini))
        {
            return null;
        }

        var dosyalar = Directory.GetFiles(_sesDizini, indeks.ToString("D3") + "_*", SearchOption.TopDirectoryOnly);
        Array.Sort(dosyalar, StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < dosyalar.Length; i++)
        {
            var uzanti = Path.GetExtension(dosyalar[i]);
            if (uzanti.Equals(".wav", StringComparison.OrdinalIgnoreCase) ||
                uzanti.Equals(".ogg", StringComparison.OrdinalIgnoreCase) ||
                uzanti.Equals(".mp3", StringComparison.OrdinalIgnoreCase))
            {
                return dosyalar[i];
            }
        }

        return null;
    }

    private static AudioType SesTurunuBul(string yol)
    {
        var uzanti = Path.GetExtension(yol);
        if (uzanti.Equals(".ogg", StringComparison.OrdinalIgnoreCase))
        {
            return AudioType.OGGVORBIS;
        }

        return uzanti.Equals(".mp3", StringComparison.OrdinalIgnoreCase) ? AudioType.MPEG : AudioType.WAV;
    }

    private static AudioSource SesKaynaginiBul(GameObject nesne)
    {
        const string ad = "UMMFBadParentingSesKaynagi";
        var varOlan = nesne.transform.Find(ad);
        if (varOlan != null)
        {
            return varOlan.GetComponent<AudioSource>();
        }

        var cocuk = new GameObject(ad);
        cocuk.transform.SetParent(nesne.transform, false);
        var kaynak = cocuk.AddComponent<AudioSource>();
        kaynak.playOnAwake = false;
        return kaynak;
    }
}
