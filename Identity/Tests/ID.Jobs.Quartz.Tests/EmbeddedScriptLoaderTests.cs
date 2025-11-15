namespace ID.Jobs.Quartz.Tests;

public class EmbeddedScriptLoaderTests
{
    [Fact]
    public void LoadEmbeddedSqlScripts_Finds_And_Strips_Prefix_And_Orders()
    {
        var loader = new EmbeddedScriptLoader(new NullLogger<EmbeddedScriptLoader>());
        var ns = typeof(EmbeddedScriptLoaderTests).Assembly.GetName().Name + ".TestMigrations.Postgres.";

        var scripts = loader.LoadEmbeddedSqlScripts(typeof(EmbeddedScriptLoaderTests).Assembly, ns, new Dictionary<string, string>());

        scripts.Count.ShouldBe(2);
        scripts[0].Name.ShouldEndWith("001_init.sql.template");
        scripts[1].Name.ShouldEndWith("002_tokens.sql.template");
    }

    //-----------------------//

    [Fact]
    public void LoadEmbeddedSqlScripts_Replaces_Tokens()
    {
        var loader = new EmbeddedScriptLoader(new NullLogger<EmbeddedScriptLoader>());
        var ns = typeof(EmbeddedScriptLoaderTests).Assembly.GetName().Name + ".TestMigrations.Postgres.";

        var scripts = loader.LoadEmbeddedSqlScripts(typeof(EmbeddedScriptLoaderTests).Assembly, ns, new Dictionary<string, string> { ["schema"] = "my_schema" });

        var tokenScript = scripts.Single(s => s.Name.EndsWith("002_tokens.sql.template"));
        tokenScript.Contents.ShouldContain("my_schema");
        tokenScript.Contents.ShouldNotContain("${schema}");
        tokenScript.Contents.ShouldNotContain("$schema$");
    }

    //-----------------------//

    [Fact]
    public void LoadEmbeddedSqlScripts_Throws_When_No_Resources()
    {
        var loader = new EmbeddedScriptLoader(new NullLogger<EmbeddedScriptLoader>());

        Should.Throw<InvalidOperationException>(() => loader.LoadEmbeddedSqlScripts(typeof(EmbeddedScriptLoaderTests).Assembly, "No.Such.Namespace.", null));
    }

}//Cls
