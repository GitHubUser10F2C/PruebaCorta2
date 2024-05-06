using chairs_dotnet7_api;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("chairlist"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

var chairs = app.MapGroup("api/chair");

//TODO: ASIGNACION DE RUTAS A LOS ENDPOINTS
chairs.MapGet("/", GetChairs);
chairs.MapPost("/", CreateChair);
chairs.MapGet("/{nombre}", GetChairByName);
chairs.MapPut("/{id}", UpdateChair);
chairs.MapPut("/{id}/{stock}", UpdateChairStock);
chairs.MapDelete("/{id}", DeleteChair);

app.Run();

//TODO: ENDPOINTS SOLICITADOS
static IResult GetChairs(DataContext db)
{
    return TypedResults.Ok(db.Chairs.ToArray());
}

static IResult CreateChair(Chair chair, DataContext db)
{
    if (db.Chairs.Any(x => x.Nombre == chair.Nombre)){
        return TypedResults.BadRequest("Una silla con ese nombre ya existe");
    }
    
    db.Chairs.Add(chair);
    db.SaveChanges();

    return TypedResults.Created($"/chair/{chair.Id}", chair);
}

static IResult GetChairByName(string nombre, DataContext db)
{
    var chair = db.Chairs.Where(x => x.Nombre == nombre).FirstOrDefault();
    if (chair == null){
        return TypedResults.NotFound();
    }

    return TypedResults.Ok(chair);
}

static IResult UpdateChair(int id, Chair inputChair, DataContext db)
{
    var chair = db.Chairs.Find(id);

    if (chair is null)
        return TypedResults.NotFound();

    chair.Nombre = inputChair.Nombre;
    chair.Tipo = inputChair.Tipo;
    chair.Material = inputChair.Material;
    chair.Color = inputChair.Color;
    chair.Altura = inputChair.Altura;
    chair.Anchura = inputChair.Anchura;
    chair.Profundidad = inputChair.Profundidad;
    chair.Precio = inputChair.Precio;

    db.SaveChangesAsync();

    return TypedResults.NoContent();
}

static IResult UpdateChairStock(int id, int stock, DataContext db)
{
    var chair = db.Chairs.Find(id);

    if (chair is null)
        return TypedResults.NotFound();

    chair.Stock = stock;

    db.SaveChangesAsync();

    return TypedResults.Ok(chair);
}

static IResult DeleteChair(int id, DataContext db)
{
    if (db.Chairs.Find(id) is Chair chair)
    {
        db.Chairs.Remove(chair);
        db.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    return TypedResults.NotFound();
}
