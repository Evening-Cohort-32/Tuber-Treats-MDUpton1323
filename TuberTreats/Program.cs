using TuberTreats.Models;

List<TuberOrder> tuberOrders = new List<TuberOrder>
{
    new TuberOrder { Id = 1, OrderPlacedOnDate = new DateTime(2026, 9, 20), CustomerId = 1, TuberDriverId = 1, DeliveredOnDate = null, Toppings = null},
    new TuberOrder { Id = 2, OrderPlacedOnDate = new DateTime(2026, 9, 25), CustomerId = 2, TuberDriverId = 2, DeliveredOnDate = null, Toppings = null},
    new TuberOrder { Id = 3, OrderPlacedOnDate = new DateTime(2026, 10, 1), CustomerId = 3, TuberDriverId = 3, DeliveredOnDate = null, Toppings = null}
};

List<Topping> toppings = new List<Topping>
{
    new Topping { Id = 1, Name = "Pepperoni"},
    new Topping { Id = 2, Name = "Hamburger"},
    new Topping { Id = 3, Name = "Pesto Leaf"},
    new Topping { Id = 4, Name = "The Works"},
    new Topping { Id = 5, Name = "Ham"}
};

List<TuberTopping> tuberToppings = new List<TuberTopping>
{
    new TuberTopping { Id = 1, TuberOrderId = 1, ToppingId = 1 },
    new TuberTopping { Id = 2, TuberOrderId = 2, ToppingId = 2 },
    new TuberTopping { Id = 3, TuberOrderId = 3, ToppingId = 3 }
};

List<Customer> customers = new List<Customer>
{
    new Customer { Id = 1, Name = "Elaine", Address = "7220 Waldron Dr", TuberOrders = null },
    new Customer { Id = 2, Name = "Siang", Address = "5861 Nunyah Dr", TuberOrders = null },
    new Customer { Id = 3, Name = "Nikki", Address = "1017 Brick Squad Rd", TuberOrders = null },
    new Customer { Id = 4, Name = "Marcus" , Address = "1445 Eagle View Blvd", TuberOrders = null },
    new Customer { Id = 5, Name = "Sovannary", Address = "307 Wishyouwere he ct", TuberOrders = null }
};

List<TuberDriver> tuberDrivers = new List<TuberDriver>
{
    new TuberDriver { Id = 1, Name = "Jorge", TuberDeliveries = null },
    new TuberDriver { Id = 2, Name = "Yamin", TuberDeliveries = null },
    new TuberDriver { Id = 3, Name = "Hugo", TuberDeliveries = null }
};

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

//add endpoints here

app.MapGet("/customers", () =>
{
    return customers.Select(c => new Customer
    {
        Id = c.Id,
        Name = c.Name,
        Address = c.Address
    });
});

app.MapGet("/customers/{id}", (int id) =>
{
    Customer customer = customers.FirstOrDefault(ci => ci.Id == id);
    if (customer == null)
    {
        return Results.NotFound();
    }
    customer.TuberOrders = tuberOrders.Where(o => o.CustomerId == customer.Id).ToList();
    return Results.Ok(customer);
});

app.MapDelete("/customers/{id}", (int id) =>
{
    Customer customer = customers.FirstOrDefault(cd => cd.Id == id);
    if (customer == null)
    {
        return Results.NotFound();
    }

    customers.RemoveAt(id - 1);
    return Results.NoContent();
});

app.MapPost("/customers", (Customer customer) =>
{
    customer.Id = customers.Max(cc => cc.Id) + 1;
    customers.Add(customer);

    return Results.Created($"/customer/{customer.Id}", new Customer
    {
        Id = customer.Id,
        Name = customer.Name,
        Address = customer.Address
    });
});

app.MapGet("/toppings", () =>
{
    return toppings.Select(t => new Topping
    {
        Id = t.Id,
        Name = t.Name
    });
});

app.MapGet("/toppings/{id}", (int id) =>
{
    Topping topping = toppings.FirstOrDefault(t => t.Id == id);
    if (topping == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(topping);
});

app.MapGet("/tuberDrivers", () =>
{
    return tuberDrivers.Select(td => new TuberDriver
    {
        Id = td.Id,
        Name = td.Name
    });
});

app.MapGet("/tuberDrivers/{id}", (int id) =>
{
    TuberDriver tuberDriver = tuberDrivers.FirstOrDefault(td => td.Id == id);
    if (tuberDriver == null)
    {
        return Results.NotFound();
    }
    tuberDriver.TuberDeliveries = tuberOrders.Where(o => o.TuberDriverId == tuberDriver.Id).ToList();
    return Results.Ok(tuberDriver);
});

app.MapGet("/tuberOrders", () =>
{
    return tuberOrders.Select(to => new TuberOrder
    {
        Id = to.Id,
        OrderPlacedOnDate = to.OrderPlacedOnDate,
        CustomerId = to.CustomerId,
        TuberDriverId = to.TuberDriverId,
        DeliveredOnDate = to.DeliveredOnDate,
        Toppings = to.Toppings
    });
});

app.MapGet("/tuberOrders/{id}", (int id) =>
{
    TuberOrder tuberOrder = tuberOrders.FirstOrDefault(tg => tg.Id == id);
    if (tuberOrder == null)
    {
        return Results.NotFound();
    }
    tuberOrder.Toppings = tuberToppings
    .Where(tuberTopping => tuberTopping.TuberOrderId == tuberOrder.Id)
    .Select(tuberTopping => toppings.First(topping => topping.Id == tuberTopping.ToppingId)).ToList();
    tuberOrder.Customer = customers.FirstOrDefault(c => c.Id == tuberOrder.CustomerId);
    tuberOrder.TuberDriver = tuberDrivers.FirstOrDefault(td => td.Id == tuberOrder.TuberDriverId);

    return Results.Ok(tuberOrder);
});

app.MapPost("/tuberOrders", (TuberOrder tuberOrder) =>
{
    tuberOrder.Id = tuberOrders.Max(to => to.Id) + 1;
    tuberOrder.OrderPlacedOnDate = DateTime.Now;
    tuberOrders.Add(tuberOrder);

    return Results.Created($"/tuberOrder/{tuberOrder.Id}", new TuberOrder
    {
        Id = tuberOrder.Id,
        OrderPlacedOnDate = tuberOrder.OrderPlacedOnDate,
        CustomerId = tuberOrder.CustomerId
    });
});

app.MapPut("/tuberOrders/{id}", (int id, TuberOrder tuberOrder) =>
{
    TuberOrder orderToUpdate = tuberOrders.FirstOrDefault(tp => tp.Id == id);
    if (orderToUpdate == null)
    {
        return Results.NotFound();
    }
    if (id != tuberOrder.Id)
    {
        return Results.BadRequest();
    }

    orderToUpdate.Id = tuberOrder.Id;
    orderToUpdate.TuberDriverId = tuberOrder.TuberDriverId;
    orderToUpdate.CustomerId = tuberOrder.CustomerId;
    orderToUpdate.OrderPlacedOnDate = tuberOrder.OrderPlacedOnDate;

    return Results.NoContent();
});

app.MapPost("/tuberOrders/{id}/complete", (int id) =>
{
    TuberOrder tuberOrderComplete = tuberOrders.FirstOrDefault(tc => tc.Id == id);
    if (tuberOrderComplete == null)
    {
        return Results.NotFound();
    }
    tuberOrderComplete.DeliveredOnDate = DateTime.Now;

    return Results.NoContent();
});

app.MapGet("/tuberToppings", () =>
{
    return tuberToppings.Select(tt => new TuberTopping
    {
        Id = tt.Id,
        TuberOrderId = tt.TuberOrderId,
        ToppingId = tt.ToppingId
    });
});

app.MapDelete("/tuberToppings/{id}", (int id) =>
{
    TuberTopping tuberTopping = tuberToppings.FirstOrDefault(tt => tt.Id == id);
    if (tuberTopping == null)
    {
        return Results.NotFound();
    }

    tuberToppings.RemoveAt(id - 1);
    return Results.NoContent();
});

app.MapPost("/tuberToppings", (TuberTopping tuberTopping) =>
{
    tuberTopping.Id = tuberToppings.Max(cc => cc.Id) + 1;
    tuberToppings.Add(tuberTopping);

    return Results.Created($"/tuberToppings/{tuberTopping.Id}", new TuberTopping
    {
        Id = tuberTopping.Id,
        TuberOrderId = tuberTopping.TuberOrderId,
        ToppingId = tuberTopping.ToppingId
    });
});

app.Run();
//don't touch or move this!
public partial class Program { }