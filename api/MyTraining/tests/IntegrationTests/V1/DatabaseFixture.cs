using System.Data.Common;
using System.Net.Http;
using System.Threading.Tasks;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using Xunit;

namespace IntegrationTests.V1;

public class DatabaseFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    public DefaultDbContext Context { get; private set; }
    private Respawner _respawner;
    private DbConnection _connection;


    public DatabaseFixture()
    {
        var options = new DbContextOptionsBuilder<DefaultDbContext>()
            .UseNpgsql("host=localhost;port=5433;database=mytraining_test;username=docker;password=masterkey")
            .Options;

        Context = new DefaultDbContext(options);
    }

    public HttpClient HttpClient { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        HttpClient = CreateClient();

        await Context.Database.EnsureDeletedAsync();
        await Context.Database.MigrateAsync();

        var respawnerOptions = new RespawnerOptions
        {
            SchemasToInclude = new[]
            {
                "public"
            },
            DbAdapter = DbAdapter.Postgres
        };

        _connection = Context.Database.GetDbConnection();
        await _connection.OpenAsync();

        _respawner = await Respawner.CreateAsync(_connection, respawnerOptions);
    }

    public async Task ResetDatabase()
    {
        await _respawner.ResetAsync(_connection);
    }

    public async Task DisposeAsync()
    {
        await Context.Database.EnsureDeletedAsync();
        await Context.DisposeAsync();
        await _connection.CloseAsync();
    }
}