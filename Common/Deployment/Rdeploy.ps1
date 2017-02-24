Param(
    [parameter()]
    [string]
    $password="Passw0rd!"
)

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