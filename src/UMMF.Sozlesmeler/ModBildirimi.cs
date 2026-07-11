namespace UMMF.Sozlesmeler;

public sealed class ModBildirimi
{
    public int SemaSurumu { get; set; } = 1;

    public string Kimlik { get; set; } = string.Empty;

    public string Ad { get; set; } = string.Empty;

    public string Surum { get; set; } = "0.2.0";

    public OyunHedefi Hedef { get; set; } = new OyunHedefi();

    public List<MedyaDegisikligi> Degisiklikler { get; set; } = new List<MedyaDegisikligi>();
}

public sealed class OyunHedefi
{
    public string? OyunKimligi { get; set; }

    public string? UrunAdi { get; set; }

    public string? AsgariCerceveSurumu { get; set; }
}

public sealed class MedyaDegisikligi
{
    public string Kimlik { get; set; } = string.Empty;

    public MedyaVarlikTuru Tur { get; set; }

    public VarlikParmakIzi Eslesme { get; set; } = new VarlikParmakIzi();

    public string? YeniDosya { get; set; }

    public string? AltyaziMetni { get; set; }

    public string? SesDosyasi { get; set; }

    public bool GizleninceSesiDurdur { get; set; } = true;

    public int GecikmeMilisaniye { get; set; }

    public float SesDuzeyi { get; set; } = 1.0f;
}
