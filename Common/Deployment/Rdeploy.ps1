Param(
    [parameter()]
    [string]
    $password="Passw0rd!"
)

$deployrPath="C:\Program Files\Microsoft\R Server\R_SERVER\DeployR"
$folderName = "pmdeploy"
$zipfile = $folderName+".zip"

Invoke-WebRequest -Uri https://iotsuiteshare.blob.core.windows.net/pmtemplate/pmdeploy.zip -OutFile $zipfile
Expand-Archive -Path $zipfile -Force

cd $folderName
$args = ("ConfigMRS.dll", "-setpassword", $password, "-deployrpath", "`"$deployrPath`"")
Start-Process -FilePath "dotnet" -ArgumentList $args -WindowStyle Hidden
cd ..

exit 0