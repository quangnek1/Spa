git # Spa

## Thực hiện tạo migration database

	cd src

	dotnet ef migrations add "Init_DB_" --project core\Spa.Infrastructure --startup-project core\Spa.WebAPI --output-dir Data\Migrations

	dotnet ef migrations remove --project core\Spa.Infrastructure --startup-project core\Spa.WebAPI

	dotnet ef database update --project core\Spa.Infrastructure --startup-project core\Spa.WebAPI

