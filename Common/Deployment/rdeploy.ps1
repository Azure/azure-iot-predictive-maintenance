Param(
    [parameter()]
    [string]
    $password="Passw0rd!"
)

(New-Object System.Net.WebClient).DownloadFile("https://aka.ms/azureiot/predictivemaintenance-r/RServerSetup.exe", "RServerSetup.exe")
$psi = New-Object System.Diagnostics.ProcessStartInfo;
$psi.FileName = "RServerSetup.exe";
$psi.Arguments = "/quiet /install";
$psi.WorkingDirectory = (Get-Item -Path ".\" -Verbose).FullName;
$psi.UseShellExecute =$False;
$psi.RedirectStandardOutput = $true;
$p = [System.Diagnostics.Process]::Start($psi);
$p.WaitForExit();
$p.StandardOutput.ReadToEnd();

$env:Path += ";C:\Program Files\dotnet"

$deployrPath="C:\Program Files\Microsoft\R Server\R_SERVER\DeployR"
$folderName = "pmdeploy"
$zipfile = $folderName+".zip"

Invoke-WebRequest -Uri https://aka.ms/azureiot/predictivemaintenance-r/rdeployzip -OutFile $zipfile
Expand-Archive -Path $zipfile -Force

cd $folderName
$args = ("ConfigMRS.dll", "-setpassword", $password, "-deployrpath", "`"$deployrPath`"")

Start-Process -FilePath "dotnet" -ArgumentList $args -WindowStyle Hidden
cd ..

exit 0