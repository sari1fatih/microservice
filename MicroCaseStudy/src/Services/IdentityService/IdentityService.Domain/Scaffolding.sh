@echo off
setlocal

dotnet ef dbcontext scaffold "User ID=postgres;Password=1234;Server=127.0.0.1;Port=5432;Database=microidentity;Pooling=true;" Npgsql.EntityFrameworkCore.PostgreSQL --output-dir Entities --force --schema "public"

echo Scaffold İşlemi Tamamlandı.