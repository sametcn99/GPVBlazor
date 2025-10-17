using System.Text;

namespace GPVBlazor.Endpoints;

public static class SitemapEndpoints
{
    public static void MapSitemapEndpoints(this WebApplication app)
    {
        // Static sitemap.xml endpoint
        app.MapGet("/sitemap.xml", async (HttpContext context) =>
        {
            var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";
            var sitemap = GenerateSitemap(baseUrl);

            context.Response.ContentType = "application/xml";
            await context.Response.WriteAsync(sitemap);
        });

        // robots.txt is already served from wwwroot, but we can add a fallback
        app.MapGet("/robots.txt", async (HttpContext context) =>
        {
            var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";
            var robotsTxt = $@"# robots.txt for GitHub Profile Viewer

User-agent: *
Allow: /
Allow: /p/*

# Sitemap
Sitemap: {baseUrl}/sitemap.xml

# Crawl-delay
Crawl-delay: 1

# Disallow private or sensitive paths
Disallow: /Error
";

            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync(robotsTxt);
        });
    }

    private static string GenerateSitemap(string baseUrl)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

        // Home page
        sb.AppendLine("    <url>");
        sb.AppendLine($"        <loc>{baseUrl}/</loc>");
        sb.AppendLine($"        <lastmod>{DateTime.UtcNow:yyyy-MM-dd}</lastmod>");
        sb.AppendLine("        <changefreq>daily</changefreq>");
        sb.AppendLine("        <priority>1.0</priority>");
        sb.AppendLine("    </url>");

        // You can add more static or dynamic URLs here
        // For example, popular GitHub profiles if you track them

        sb.AppendLine("</urlset>");

        return sb.ToString();
    }
}
