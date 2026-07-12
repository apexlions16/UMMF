namespace UMMF.Masaustu;

internal static class Program
{
    internal const string Surum = "0.6.0-onizleme.1";

    [STAThread]
    private static int Main(string[] args)
    {
        ApplicationConfiguration.Initialize();

        if (args.Length > 0 && args[0].Equals("--arayuz-testi", StringComparison.OrdinalIgnoreCase))
        {
            var sonucYolu = args.Length > 1
                ? Path.GetFullPath(args[1])
                : Path.Combine(Path.GetTempPath(), "UMMF-ARAYUZ-TESTI.txt");

            using var pencere = new AnaPencere();
            var sonuc = pencere.AraryuzDumanTestiniCalistir();
            Directory.CreateDirectory(Path.GetDirectoryName(sonucYolu)!);
            File.WriteAllText(sonucYolu, sonuc, new System.Text.UTF8Encoding(false));
            return sonuc.Contains("BAŞARILI", StringComparison.Ordinal) ? 0 : 1;
        }

        Application.Run(new AnaPencere());
        return 0;
    }
}
