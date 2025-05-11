using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace Squido.Test.TestHelpers;

public class TestDbContext : SquidoDbContext
{
    public TestDbContext() : base(new DbContextOptionsBuilder<SquidoDbContext>()
        .UseInMemoryDatabase(databaseName: "TestDb")
        .Options)
    {
    }
} 