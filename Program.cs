using Microsoft.EntityFrameworkCore;
using PersonApi_sync_REST;

var builder = WebApplication.CreateBuilder(args);

// Додаємо контекст бази даних в контейнер впровадження залежностей (DI). Контейнер DI надає доступ до контексту бази даних та інших служб.
builder.Services.AddDbContext<PersonDB>(static opt => opt.UseInMemoryDatabase("PersonList"));

// Включаємо браузер API, який являє собою службу, яка надає метадані про HTTP-API. Оглядач API використовується Swagger для створення документа Swagger.
builder.Services.AddEndpointsApiExplorer();

// Включаємо 
builder.Services.ConfigureCors();

// Додаємо генератор документів Swagger OpenAPI до служб додатків і налаштовуємо його для надання додаткових відомостей про API, таких як назва та версія.
// Щоб отримати додаткові відомості про API, див. у статті "Початок роботи з NSwag" та
// ASP.NET Core (https://learn.microsoft.com/uk-ua/aspnet/core/tutorials/getting-started-with-nswag?view=aspnetcore-8.0#customize-api-documentation)
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "PersonAPI";
    config.Title = "PersonAPI v1";
    config.Version = "v1";
});

var app = builder.Build();

// Організуємо опис кінцевих точок.
app.MapGet("/api/", GetAllPerson); // Перегляд всієї бази даних з використанням GET
app.MapPost("/api/", CreatePerson); // Створення запису в базі даних з використанням POST
app.MapGet("/api/{unzr}", GetPerson); // Перегляд запису бази даних за значенням УНЗР з використанням GET
app.MapPost("/api/{unzr}", UpdatePerson); // Корегування запису бази даних за значенням УНЗР з використанням POST
app.MapPut("/api/{unzr}", UpdatePerson); // Те саме з використанням PUT
app.MapDelete("/api/{unzr}", DeletePerson); // Видалення запису в базы даних за значенням УНЗР з використанням DELETE

if (app.Environment.IsDevelopment())
{
    // Додаємо проміжне програмне забезпечення для обслуговування документів OpenAPI 3.0
    // Доступно за адресою: http://localhost:<port>/swagger/v1/swagger.json
    app.UseOpenApi();

    // Додаємо веб-інтерфейс для взаємодії з документом
    // Доступно за адресою: http://localhost:<port>/swagger
    app.UseSwaggerUi();
}

// 
app.Run();

// Ці методи повертають об'єкти, що реалізують IResult. Будемо використовувати TypedResults замість Results.
// Це має кілька переваг, включаючи тестування і автоматично повертаючи метадані типу відповіді для OpenAPI, щоб описати кінцеву точку.

static async Task<IResult> GetAllPerson(PersonDB db)
{
    return TypedResults.Ok(await db.Persons.ToArrayAsync());
}

static async Task<IResult> CreatePerson(PersonItem inputPers, PersonDB db)
{
    PersonItem Pers = new()
    {
        Name = inputPers.Name,
        Surname = inputPers.Surname,
        Patronym = inputPers.Patronym,
        DateOfBirth = inputPers.DateOfBirth,
        Gender = inputPers.Gender,
        Rnokpp = inputPers.Rnokpp,
        PassportNumber = inputPers.PassportNumber,
        Unzr = inputPers.Unzr,
        Inserted = DateTime.Now,
        LastUpdated = DateTime.Now
    };

    try
    {
        db.Persons.Add(Pers);
        await db.SaveChangesAsync();
    }
    catch (Exception e)
    {
        return TypedResults.Problem(e.Message);
    }

    return TypedResults.Created($"/api/{Pers.Unzr}");
};

static async Task<IResult> GetPerson(string unzr, PersonDB db)
{
    var pers = await db.Persons.FindAsync(unzr);

    if (pers is null) return TypedResults.NotFound($"Not found person with UNZR = {unzr}");

    return TypedResults.Ok(pers);

};

static async Task<IResult> UpdatePerson(string unzr, PersonItem inputPers, PersonDB db)
{

    var pers = await db.Persons.FindAsync(unzr);

    if (pers is null) return TypedResults.NotFound($"Not found person with UNZR = {unzr}");

    pers.Name = inputPers.Name;
    pers.Surname = inputPers.Surname;
    pers.Patronym = inputPers.Patronym;
    pers.DateOfBirth = inputPers.DateOfBirth;
    pers.Gender = inputPers.Gender;
    pers.Rnokpp = inputPers.Rnokpp;
    pers.PassportNumber = inputPers.PassportNumber;
    pers.LastUpdated = DateTime.Now;
    try
    {
        await db.SaveChangesAsync();
        return TypedResults.Ok(pers);
    }
    catch (Exception e)
    {
        return TypedResults.Problem(detail: e.Message, statusCode: 400);
    }
};

static async Task<IResult> DeletePerson(string unzr, PersonDB db)
{
    if (await db.Persons.FindAsync(unzr) is PersonItem pers)
    {
        try
        {
            db.Persons.Remove(pers);
            await db.SaveChangesAsync();
        }
        catch (Exception e)
        {
            return TypedResults.Problem(detail: e.Message, statusCode: 400);
        }
        return TypedResults.Ok(pers);
    }
    return TypedResults.NotFound($"Not found person with UNZR = {unzr}");
}
