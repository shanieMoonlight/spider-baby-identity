namespace ID.Infrastructure.Auth.JWT.LocalServices.Abs;

public interface IKeyIdBuilder
{
    string GenerateKidFromPem(string publicPem);
    string GenerateKidFromXml(string publicXmlKey);
}