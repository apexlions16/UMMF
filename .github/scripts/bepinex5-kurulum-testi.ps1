param(
    [Parameter(Mandatory = $true)]
    [string]$ExeYolu,

    [Parameter(Mandatory = $true)]
    [string]$Surum,

    [string]$GunlukYolu = "artifacts/KURULUM-TESTI.txt"
)

$ErrorActionPreference = 'Stop'
$exe = (Resolve-Path $ExeYolu).Path
New-Item -ItemType Directory -Path (Split-Path $GunlukYolu -Parent) -Force | Out-Null
"UMMF $Surum gömülü plugin kurulum testi başladı." | Set-Content $GunlukYolu -Encoding utf8
$script:bepinexDll = $null

function Gunluge-Yaz([string]$metin) {
    $metin | Add-Content $GunlukYolu -Encoding utf8
}

function Yeni-SahteOyun([string]$ad, [bool]$netstandard) {
    $kok = Join-Path $env:RUNNER_TEMP $ad
    if (Test-Path $kok) {
        Remove-Item $kok -Recurse -Force
    }

    $veri = Join-Path $kok "OrnekOyun_Data"
    $managed = Join-Path $veri "Managed"
    $core = Join-Path $kok "BepInEx/core"
    New-Item -ItemType Directory -Path $managed -Force | Out-Null
    New-Item -ItemType Directory -Path $core -Force | Out-Null

    Copy-Item $env:ComSpec (Join-Path $kok "OrnekOyun.exe") -Force
    Set-Content (Join-Path $managed "Assembly-CSharp.dll") -Value "UMMF test düzeneği" -Encoding utf8
    Set-Content (Join-Path $veri "globalgamemanagers") -Value "2019.4.40f1" -Encoding ascii
    Copy-Item $script:bepinexDll.FullName (Join-Path $core "BepInEx.dll") -Force

    if ($netstandard) {
        Set-Content (Join-Path $managed "netstandard.dll") -Value "UMMF test düzeneği" -Encoding utf8
    }

    return $kok
}

function Komutu-Calistir([string]$komut, [string]$oyun) {
    Gunluge-Yaz ""
    Gunluge-Yaz "> $ExeYolu $komut `"$oyun`""
    $cikti = & $exe $komut $oyun 2>&1
    $kod = $LASTEXITCODE
    $cikti | Add-Content $GunlukYolu -Encoding utf8
    return [pscustomobject]@{
        Kod = $kod
        Cikti = ($cikti -join "`n")
    }
}

function Kurulumu-Dogrula([string]$oyun, [string]$beklenenCerceve, [string]$beklenenDll) {
    $kur = Komutu-Calistir "kur" $oyun
    if ($kur.Kod -ne 0) {
        throw "Kurulum komutu başarısız oldu: $oyun"
    }
    if ($kur.Cikti -notmatch [regex]::Escape($beklenenCerceve)) {
        throw "Beklenen hedef çerçeve seçilmedi: $beklenenCerceve"
    }

    $kuruluDll = Join-Path $oyun "BepInEx/plugins/UMMF/UMMF.BepInEx5.Mono.dll"
    if (-not (Test-Path $kuruluDll)) {
        throw "Plugin DLL'si kurulmadı: $kuruluDll"
    }

    $kuruluOzet = (Get-FileHash $kuruluDll -Algorithm SHA256).Hash
    $beklenenOzet = (Get-FileHash $beklenenDll -Algorithm SHA256).Hash
    if ($kuruluOzet -ne $beklenenOzet) {
        throw "EXE içinden çıkarılan plugin beklenen $beklenenCerceve derlemesiyle aynı değil."
    }

    $durum = Komutu-Calistir "durum" $oyun
    if ($durum.Kod -ne 0) {
        throw "Durum komutu başarısız oldu."
    }

    $rapor = Komutu-Calistir "rapor" $oyun
    if ($rapor.Kod -ne 0) {
        throw "Rapor komutu başarısız oldu."
    }
    if (-not (Test-Path (Join-Path $oyun "BepInEx/UMMF/raporlar/kurulum-teshis-raporu.json"))) {
        throw "Kurulum teşhis raporu üretilmedi."
    }

    $kaldir = Komutu-Calistir "kaldir" $oyun
    if ($kaldir.Kod -ne 0) {
        throw "Kaldırma komutu başarısız oldu."
    }
    if (Test-Path $kuruluDll) {
        throw "Plugin DLL'si kaldırılamadı."
    }
    if (-not (Test-Path (Join-Path $oyun "BepInEx/UMMF/modlar"))) {
        throw "Kaldırma kullanıcı mod klasörünü silmemeliydi."
    }

    Gunluge-Yaz "$beklenenCerceve kurulumu, durum, rapor ve kaldırma testleri geçti."
}

try {
    $paketKoku = Join-Path $env:USERPROFILE ".nuget/packages/bepinex.core"
    if (-not (Test-Path $paketKoku)) {
        throw "BepInEx.Core NuGet paket klasörü bulunamadı: $paketKoku"
    }

    $script:bepinexDll = Get-ChildItem $paketKoku -Recurse -File -ErrorAction SilentlyContinue |
        Where-Object { $_.Name -eq "BepInEx.dll" -or $_.Name -eq "BepInEx.Core.dll" } |
        Sort-Object FullName -Descending |
        Select-Object -First 1

    if ($null -eq $script:bepinexDll) {
        $adaylar = Get-ChildItem $paketKoku -Recurse -File -Filter "BepInEx*.dll" -ErrorAction SilentlyContinue |
            Select-Object -ExpandProperty FullName
        Gunluge-Yaz "Bulunan BepInEx adayları: $($adaylar -join ', ')"
        throw "NuGet önbelleğinde BepInEx.dll veya BepInEx.Core.dll bulunamadı."
    }
    Gunluge-Yaz "Test BepInEx çekirdek DLL'si: $($script:bepinexDll.FullName)"

    $eskiOyun = Yeni-SahteOyun "UMMF-Eski-Mono" $false
    $modernOyun = Yeni-SahteOyun "UMMF-Modern-Mono" $true

    Kurulumu-Dogrula `
        $eskiOyun `
        "net35" `
        "src/UMMF.BepInEx5.Mono/bin/Release/net35/UMMF.BepInEx5.Mono.dll"

    Kurulumu-Dogrula `
        $modernOyun `
        "netstandard2.0" `
        "src/UMMF.BepInEx5.Mono/bin/Release/netstandard2.0/UMMF.BepInEx5.Mono.dll"

    Gunluge-Yaz "Bütün gömülü plugin kurulum testleri başarıyla tamamlandı."
}
catch {
    Gunluge-Yaz "HATA: $($_.Exception.Message)"
    throw
}
