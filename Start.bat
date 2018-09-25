echo "Starting Build"
dotnet build Northwind.RestApis\Northwind.RestApis.sln
dotnet build Northwind.WebApps\Northwind.WebApps.sln

echo "Starting Publish"
dotnet publish Northwind.RestApis\Northwind.Api\Northwind.Api.csproj -c Release
dotnet publish Northwind.WebApps\Northwind.Mvc\Northwind.Mvc.csproj -c Release
    
pushd Northwind.RestApis\Northwind.Api\bin\Release\netcoreapp2.1\publish\
start "Northwind Rest API" cmd /c "dotnet Northwind.Api.dll --server.urls ""http://localhost:5000""
popd

pushd Northwind.WebApps\Northwind.Mvc\bin\Release\netcoreapp2.1\publish\
start "Northwind Web App (MVC)" cmd /c "dotnet Northwind.Mvc.dll --server.urls ""http://localhost:5001""
popd