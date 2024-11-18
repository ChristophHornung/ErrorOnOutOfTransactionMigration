using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

internal class TestService
{
	private readonly BloggingContext db;

	public TestService(BloggingContext db)
	{
		this.db = db;
	}

	public async Task ExecuteAsync()
	{
		if (File.Exists("blog.db"))
		{
			File.Delete("blog.db");
		}

		await this.db.Database.MigrateAsync();
	}
}

public class Program
{
	private static IHost CreateHost() =>
		Host.CreateDefaultBuilder()
			.ConfigureServices((context, services) =>
			{
				services.AddDbContext<BloggingContext>(options =>
					options.UseSqlite($"Data Source={"blog.db"}"));
				services.AddScoped<TestService>();
			})
			.UseConsoleLifetime()
			.Build();


	private static async Task Main(string[] args)
	{
		IHost host = CreateHost();
		await host.Services.GetRequiredService<TestService>().ExecuteAsync();

		await host.RunAsync();
	}
}